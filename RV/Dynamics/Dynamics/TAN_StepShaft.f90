module TAN_StepShaft!需要编制前处理C++程序，建立节点编号与矩阵位置的联系,目前认为没有差别
    use TAN_BeamElement
    use TAN_DiscElement
    use TAN_Node
    implicit none
    type::CStepShaft_Tan!根据单元位置排序
        type(CBeamElem_Tan),pointer::BeamElemList(:)!梁单段单元
!        type(CNodePtr_Tan),pointer::BeamNodePtrList(:,:)!梁两节点在阶梯轴矩阵的位置     
        type(CDiscElem_Tan),pointer::DiscElemList(:)
!        type(CNodePtr_Tan),pointer::DiscNodePtrList(:)!圆盘节点在阶梯轴矩阵的位置
        
        integer::AssignedSign=0!动态创建标识，如果是1，则已经动态创建，否则为0
    end type
    
    contains
    subroutine CreatStepShaftSpace(InBeamSecNum,InDiscNum,OutStepShaftName)!分配空间
    implicit none
    integer::InBeamSecNum
    integer::InDiscNum
    type(CStepShaft_Tan):: OutStepShaftName
    if(OutStepShaftName%AssignedSign==1) then
    call DeleteStepShaftSpace(OutStepShaftName)
    end if
    allocate (OutStepShaftName%BeamElemList(InBeamSecNum))
!    allocate (OutStepShaftName%BeamNodePtrList(2,InBeamSecNum))
    allocate (OutStepShaftName%DiscElemList(InDiscNum))
!    allocate (OutStepShaftName%DiscNodePtrList(InDiscNum))
    OutStepShaftName%AssignedSign=1 
    return
    end subroutine
    
    
    subroutine DeleteStepShaftSpace(InStepShaftName)!删除分配的空间
    implicit none
    type(CStepShaft_Tan)::InStepShaftName
    if(InStepShaftName%AssignedSign==1) then
    deallocate (InStepShaftName%BeamElemList)
!    deallocate (InStepShaftName%BeamNodePtrList)
    deallocate (InStepShaftName%DiscElemList)
!    deallocate (InStepShaftName%DiscNodePtrList)
    end if
    InStepShaftName%AssignedSign = 0 
    return
    end subroutine
    
    subroutine BuildStepShaftElemList(InShaftSecParaList,InDiscParaList,OutStepShaftName)
    use TAN_DiscElement
    use TAN_BeamElement
    use TAN_Node
    implicit none
    real::InShaftSecParaList(:,:),InDiscParaList(:,:)
    type(CStepShaft_Tan)::OutStepShaftName
    integer::i,j,ShaftSecNum,ShaftDiscNum
    ShaftSecNum=size(InShaftSecParaList,2)
    ShaftDiscNum=size(InDiscParaList,2)
    !call CreatStepShaftSpace(ShaftSecNum,ShaftDiscNum,OutStepShaftName)
    do i=1,ShaftSecNum
        call BuildSingleEulerBeamElem(InShaftSecParaList(:,i),OutStepShaftName%BeamElemList(i))
    end do
    do i=1,ShaftDiscNum
        call BuildSingleDiskElemByGeom(InDiscParaList(:,i),OutStepShaftName%DiscElemList(i))
    end do  
    
    !do i=1,ShaftNodeNum-1
    !    call BuildSingleEulerBeamElem(InShaftParaList(i,:),OutStepShaftName%BeamElemList(i))
    !    OutStepShaftName%NodeList(i)=
    
    return
    end subroutine
    !构建单阶梯轴总体矩阵
    
    

    
    !!将阶梯轴组装到总体矩阵的函数
    !subroutine AssignBeamElemToFullMatrix(InBeamElemMatrix,InFstNodeFreedomList,InSecNodeFreedomList,InFstNodeStartIndex,InSecNodeStartIndex,OutFullMatrix)
    !use TAN_BasicDynamicMatrix
    !implicit none
    !logical::InFstNodeFreedomList(6),InSecNodeFreedomList(6)
    !integer::InFstNodeStartIndex,InSecNodeStartIndex
    !real::InBeamElemMatrix(:,:),OutFullMatrix(:,:)
    !call AssignSixFreedomElemToFullMatrix(InBeamElemMatrix(1:6,1:6),InFstNodeFreedomList,InFstNodeFreedomList,InFstNodeStartIndex,InFstNodeStartIndex,OutFullMatrix)!1-1位置
    !call AssignSixFreedomElemToFullMatrix(InBeamElemMatrix(1:6,7:12),InFstNodeFreedomList,InSecNodeFreedomList,InFstNodeStartIndex,InSecNodeStartIndex,OutFullMatrix)!1-2位置
    !call AssignSixFreedomElemToFullMatrix(InBeamElemMatrix(7:12,1:6),InSecNodeFreedomList,InFstNodeFreedomList,InSecNodeStartIndex,InFstNodeStartIndex,OutFullMatrix)!2-1位置
    !call AssignSixFreedomElemToFullMatrix(InBeamElemMatrix(7:12,7:12),InSecNodeFreedomList,InSecNodeFreedomList,InSecNodeStartIndex,InSecNodeStartIndex,OutFullMatrix)!2-1位置
    !return
    !end subroutine
    

    !计算自由度
    integer function CalStepShaftFreedomCount(InStepShaft,InNodeList)
    implicit none
    type(CStepShaft_Tan):: InStepShaft
    type(CNode_Tan)::InNodeList(:)
    integer,pointer::TempBeamNodeNumList(:)
    integer::BeamSecCount,BeamNodeCount
    integer::i,TempNodeNum
    BeamSecCount=size(InStepShaft%BeamElemList,1)
    allocate (TempBeamNodeNumList(2*BeamSecCount))
    TempBeamNodeNumList=-100000
    BeamNodeCount=0
    do i=1,BeamSecCount
        if (all(TempBeamNodeNumList .NE. InStepShaft%BeamElemList(i)%NodeNum(1))) then
            BeamNodeCount=BeamNodeCount+1
            TempBeamNodeNumList(BeamNodeCount)=InStepShaft%BeamElemList(i)%NodeNum(1)
        end if
        if (all(TempBeamNodeNumList .NE. InStepShaft%BeamElemList(i)%NodeNum(2))) then
            BeamNodeCount=BeamNodeCount+1
            TempBeamNodeNumList(BeamNodeCount)=InStepShaft%BeamElemList(i)%NodeNum(2)
        end if        
    end do
    CalStepShaftFreedomCount=0
    do i=1,BeamNodeCount
        TempNodeNum=FindNodeObjectIndex(InNodeList,TempBeamNodeNumList(i))
        CalStepShaftFreedomCount=CalStepShaftFreedomCount+sum(abs(InNodeList(TempNodeNum)%Freedom))
    end do
    deallocate (TempBeamNodeNumList)
    return
    end function
    
    subroutine ConstructSingleStepShaftDynamicMatrix(InStepShaft,InNodeList,OutStiffMatrix,OutDampMatrix,OutMassMatrix)
    use TAN_BasicDynamicMatrix
    use TAN_Node
    implicit none
    type(CStepShaft_Tan)::InStepShaft
    type(CNode_Tan)::InNodeList(:)
    real::OutStiffMatrix(:,:),OutDampMatrix(:,:),OutMassMatrix(:,:)
    type(CNodePtr_Tan)::FirstTempNodePtr,SecTempNodePtr
    integer::i,j
    integer::BeamSecCount,DiscCount,NodeCount,TempNodeNum,BeamNodeCount
    type(CNode_Tan),pointer::BeamNodeList(:)
    integer,pointer::TempBeamNodeNumList(:)
    integer::TotalFreedom
    BeamSecCount=size(InStepShaft%BeamElemList,1) 
    DiscCount=size(InStepShaft%DiscElemList,1)
    NodeCount=size(InNodeList,1)
    !考虑加入链表，找出所有不同点，如果梁有分叉的情况，然后再进行组装
    allocate (TempBeamNodeNumList(2*BeamSecCount))
    BeamNodeCount=0
    TempBeamNodeNumList=-100000
    do i=1,BeamSecCount
        if (all(TempBeamNodeNumList .NE. InStepShaft%BeamElemList(i)%NodeNum(1))) then
            BeamNodeCount=BeamNodeCount+1
            TempBeamNodeNumList(BeamNodeCount)=InStepShaft%BeamElemList(i)%NodeNum(1)
        end if
        if (all(TempBeamNodeNumList .NE. InStepShaft%BeamElemList(i)%NodeNum(2))) then
            BeamNodeCount=BeamNodeCount+1
            TempBeamNodeNumList(BeamNodeCount)=InStepShaft%BeamElemList(i)%NodeNum(2)
        end if        
    end do
    allocate (BeamNodeList(BeamNodeCount))
    do i=1,BeamNodeCount
        BeamNodeList(i)%Num=TempBeamNodeNumList(i)
    end do
    deallocate (TempBeamNodeNumList)
    do i=1,BeamNodeCount
        TempNodeNum=FindNodeObjectIndex(InNodeList,BeamNodeList(i)%Num)
        BeamNodeList(i)=InNodeList(TempNodeNum)
    end do    
    call ConstructNodeObjectList(BeamNodeList,BeamNodeList)    
    do i=1,BeamSecCount
        TempNodeNum=FindNodeObjectIndex(BeamNodeList,InStepShaft%BeamElemList(i)%NodeNum(1))
        FirstTempNodePtr%NodePtr=>BeamNodeList(TempNodeNum)
        TempNodeNum=FindNodeObjectIndex(BeamNodeList,InStepShaft%BeamElemList(i)%NodeNum(2))
        SecTempNodePtr%NodePtr=>BeamNodeList(TempNodeNum)
        call AssignTwoNodeElemToFullMatrix(InStepShaft%BeamElemList(i)%MassMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutMassMatrix)
        call AssignTwoNodeElemToFullMatrix(InStepShaft%BeamElemList(i)%DampMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutDampMatrix)
        call AssignTwoNodeElemToFullMatrix(InStepShaft%BeamElemList(i)%StiffMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutStiffMatrix)    
    end do
    do i=1,DiscCount
        TempNodeNum=FindNodeObjectIndex(BeamNodeList,InStepShaft%DiscElemList(i)%NodeNum)
        FirstTempNodePtr%NodePtr=>BeamNodeList(TempNodeNum)
        call AssignSixFreedomElemToFullMatrix(InStepShaft%DiscElemList(i)%MassMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            OutMassMatrix)            
    end do    
    deallocate (BeamNodeList)
    return
    end subroutine
        
        
    subroutine ConstructSingleStepShaftDynamicMatrixObject(InStepShaft,InNodeList,OutDynamicObject)
    use TAN_Node
    use TAN_BasicDynamicMatrix
    implicit none
    type(CStepShaft_Tan)::InStepShaft
    type(CNode_Tan)::InNodeList(:)
    type(CDynamicMatrix_Tan)::OutDynamicObject
    call ConstructSingleStepShaftDynamicMatrix(InStepShaft,&
        InNodeList,&
        OutDynamicObject%StiffMatrix,&
        OutDynamicObject%DampMatrix,&
        OutDynamicObject%MassMatrix)
    return
    end subroutine
    
    
    
    
    
    
    
    
    
    
    !integer ObtainNodeLocationInMatrix(InNodeNum,InNodeToMatrixMap)
    !implicit none
    !return
    
    !subroutine AssignMatrixByTwoTuple(InTwoTuple,InTupleFreedom,InNodeToMatrixMap,OutMatrix)
    !implicit none
    !Integer::
    
    !subroutine ConstructSingleStepShaftDynamicMatrix(InStepShaft,InNodeFixedFreedomList,OutMassMatrix,OutDampMatrix,OutStiffMatrix)
    !implicit none
    !type(CStepShaft_Tan)::InStepShaft
    !integer::InNodeAndFreedomList(:)!第一个，节点号，2-7，自由度
    !real::OutMassMatrix(:),OutDampMatrix(:),OutStiffMatrix(:)
    !
    !return
    !end subroutine
end module