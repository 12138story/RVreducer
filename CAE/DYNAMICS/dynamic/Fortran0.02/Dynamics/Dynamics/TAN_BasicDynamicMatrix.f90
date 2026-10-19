  
    include 'link_fnl_shared.h'
    module TAN_BasicDynamicMatrix
    use TAN_Node
    implicit none
    type::CDynamicMatrix_Tan
        real,pointer::StiffMatrix(:,:)
        real,pointer::MassMatrix(:,:)
        real,pointer::DampMatrix(:,:)
        integer::AssignedSign=0
    end type

    contains
    subroutine CreatDynamicMatrixSpace(InTotalFreedom,OutDynamicMatrix)!分配空间
    implicit none
    integer::InTotalFreedom
    type(CDynamicMatrix_Tan):: OutDynamicMatrix
    if(OutDynamicMatrix%AssignedSign==1) then
    call DeleteDynamicEquSpace(OutDynamicMatrix)
    end if
    allocate (OutDynamicMatrix%StiffMatrix(InTotalFreedom,InTotalFreedom))
    allocate (OutDynamicMatrix%MassMatrix(InTotalFreedom,InTotalFreedom))
    allocate (OutDynamicMatrix%DampMatrix(InTotalFreedom,InTotalFreedom))
    OutDynamicMatrix%AssignedSign=1 
    return
    end subroutine
    
    subroutine DeleteDynamicEquSpace(InDynamicMatrix)!分配空间
    implicit none
    type(CDynamicMatrix_Tan):: InDynamicMatrix    
    if(InDynamicMatrix%AssignedSign==1) then
    deallocate (InDynamicMatrix%StiffMatrix)
    deallocate (InDynamicMatrix%MassMatrix)
    deallocate (InDynamicMatrix%DampMatrix)
    end if
    InDynamicMatrix%AssignedSign = 0 
    return
    end subroutine

    
    !根据自由度将子矩阵组装到总体矩阵
    subroutine AssignSixFreedomElemToFullMatrix(InSixFreedomElemMatrix,InRowFreedomList,InColFreedomList,InFullMatrixAssignRowStartIndex,InFullMatrixAssignColStartIndex,OutFullMatrix)
    implicit none
    logical::InRowFreedomList(6),InColFreedomList(6)
    integer::InFullMatrixAssignRowStartIndex,InFullMatrixAssignColStartIndex
    real::InSixFreedomElemMatrix(:,:),OutFullMatrix(:,:)
    integer,pointer::AssignRowIndex(:),AssignColIndex(:)
    integer::AssignRowNum,AssignColNum
    integer::i,AssignListIndex
    AssignRowNum=sum(abs(InRowFreedomList(:)))
    AssignColNum=sum(abs(InColFreedomList(:)))
    allocate (AssignRowIndex(AssignRowNum))
    allocate (AssignColIndex(AssignColNum))
    AssignListIndex=1
    do i=1,6
        if(InRowFreedomList(i)) then
            AssignRowIndex(AssignListIndex)=i
            AssignListIndex=AssignListIndex+1
        end if
    end do
    AssignListIndex=1
    do i=1,6
        if(InColFreedomList(i)) then
            AssignColIndex(AssignListIndex)=i
            AssignListIndex=AssignListIndex+1
        end if
    end do
    OutFullMatrix(InFullMatrixAssignRowStartIndex:(InFullMatrixAssignRowStartIndex-1)+AssignRowNum,InFullMatrixAssignColStartIndex:(InFullMatrixAssignColStartIndex-1)+AssignColNum)=&
        OutFullMatrix(InFullMatrixAssignRowStartIndex:(InFullMatrixAssignRowStartIndex-1)+AssignRowNum,InFullMatrixAssignColStartIndex:(InFullMatrixAssignColStartIndex-1)+AssignColNum)+&
        InSixFreedomElemMatrix(AssignRowIndex,AssignColIndex)
    return
    end subroutine
    
    

    !根据自由度将两节点子矩阵组装到总体矩阵
    subroutine AssignTwoNodeElemToFullMatrix(InElemMatrix,&
        InFstNodeFreedomList,&
        InSecNodeFreedomList,&
        InFstNodeStartIndex,&
        InSecNodeStartIndex,&
        OutFullMatrix)
    implicit none
    logical::InFstNodeFreedomList(6),InSecNodeFreedomList(6)
    integer::InFstNodeStartIndex,InSecNodeStartIndex
    real::InElemMatrix(:,:),OutFullMatrix(:,:)
    call AssignSixFreedomElemToFullMatrix(InElemMatrix(1:6,1:6),InFstNodeFreedomList,InFstNodeFreedomList,InFstNodeStartIndex,InFstNodeStartIndex,OutFullMatrix)!1-1位置
    call AssignSixFreedomElemToFullMatrix(InElemMatrix(1:6,7:12),InFstNodeFreedomList,InSecNodeFreedomList,InFstNodeStartIndex,InSecNodeStartIndex,OutFullMatrix)!1-2位置
    call AssignSixFreedomElemToFullMatrix(InElemMatrix(7:12,1:6),InSecNodeFreedomList,InFstNodeFreedomList,InSecNodeStartIndex,InFstNodeStartIndex,OutFullMatrix)!2-1位置
    call AssignSixFreedomElemToFullMatrix(InElemMatrix(7:12,7:12),InSecNodeFreedomList,InSecNodeFreedomList,InSecNodeStartIndex,InSecNodeStartIndex,OutFullMatrix)!2-1位置
    return
    end subroutine
        
    !构建一阶标准动力学求解矩阵    
    subroutine BuildSTDDynamicEqu(InDynamicObject,OutSolveMatrix)
    use f95_precision
    use blas95
    implicit none
    type(CDynamicMatrix_Tan)::InDynamicObject
    real::OutSolveMatrix(:,:)
    real,pointer::MassInverseMatrix(:,:)
    integer::i,TotalFreedom
    TotalFreedom=size(InDynamicObject%StiffMatrix,1)
    OutSolveMatrix=real(0)
    do i=1,TotalFreedom
        OutSolveMatrix(i,i)=real(1)
    end do
    allocate(MassInverseMatrix(TotalFreedom,TotalFreedom))
    call DLINDS(TotalFreedom,InDynamicObject%MassMatrix,TotalFreedom,&
        MassInverseMatrix,TotalFreedom)
    call gemm(MassInverseMatrix,&
        InDynamicObject%StiffMatrix,&
        OutSolveMatrix(TotalFreedom+1:2*TotalFreedom,1:TotalFreedom),alpha=real(-1))
    call gemm(MassInverseMatrix,&
        InDynamicObject%dampMatrix,&
        OutSolveMatrix(TotalFreedom+1:2*TotalFreedom,TotalFreedom+1:2*TotalFreedom),alpha=real(-1))
    deallocate(MassInverseMatrix)
    return
    end subroutine
    
  
    !subroutine CalDynamicEquEigen(InDynamicMatrixObject,OutEigenValue,OutEigenVector)
    !implicit none
    !type(CDynamicMatrix_Tan)::OutDynamicMatrixObject
    !real::OutEigenValue
    !
    !return
    !end subroutine

    end module
    
