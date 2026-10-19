
    module TAN_DynamicSystem
  

    contains
    
    !构建动力学矩阵
    subroutine ConstructDynamicObject(InStepShaftList,InMeshElemList,InNodeObjectList,OutDynamicMatrixObject)
    use TAN_BasicDynamicMatrix
    use TAN_StepShaft
    use TAN_MeshElement
    implicit none
    type(CStepShaft_Tan):: InStepShaftList(:)
    type(CMeshElem_Tan)::InMeshElemList(:)
    type(CNode_Tan),target::InNodeObjectList(:)
    type(CDynamicMatrix_Tan)::OutDynamicMatrixObject
    integer::ShaftCount,MeshCount
    integer::NodeCount
    integer::i,j,TempNodeNum
    type(CNodePtr_Tan)::FirstTempNodePtr,SecTempNodePtr
    ShaftCount=size(InStepShaftList)
    NodeCount=size(InNodeObjectList)
    call ConstructNodeObjectList(InNodeObjectList,InNodeObjectList)
    do i=1,ShaftCount
        do j=1,size(InStepShaftList(i)%BeamElemList,1)
        TempNodeNum=FindNodeObjectIndex(InNodeObjectList,InStepShaftList(i)%BeamElemList(j)%NodeNum(1))
        FirstTempNodePtr%NodePtr=>InNodeObjectList(TempNodeNum)
        TempNodeNum=FindNodeObjectIndex(InNodeObjectList,InStepShaftList(i)%BeamElemList(j)%NodeNum(2))
        SecTempNodePtr%NodePtr=>InNodeObjectList(TempNodeNum)
        call AssignTwoNodeElemToFullMatrix(InStepShaftList(i)%BeamElemList(j)%MassMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutDynamicMatrixObject%MassMatrix)
        call AssignTwoNodeElemToFullMatrix(InStepShaftList(i)%BeamElemList(j)%DampMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutDynamicMatrixObject%DampMatrix)
        call AssignTwoNodeElemToFullMatrix(InStepShaftList(i)%BeamElemList(j)%StiffMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutDynamicMatrixObject%StiffMatrix)    
        end do
        do j=1,size(InStepShaftList(i)%DiscElemList,1)
        TempNodeNum=FindNodeObjectIndex(InNodeObjectList,InStepShaftList(i)%DiscElemList(j)%NodeNum)
        FirstTempNodePtr%NodePtr=>InNodeObjectList(TempNodeNum)
        call AssignSixFreedomElemToFullMatrix(InStepShaftList(i)%DiscElemList(j)%MassMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            OutDynamicMatrixObject%MassMatrix)
        end do  
    end do
    MeshCount=size(InMeshElemList)
    do i=1,MeshCount
        TempNodeNum=FindNodeObjectIndex(InNodeObjectList,InMeshElemList(i)%NodeNum(1))
        FirstTempNodePtr%NodePtr=>InNodeObjectList(TempNodeNum)
        TempNodeNum=FindNodeObjectIndex(InNodeObjectList,InMeshElemList(i)%NodeNum(2))
        SecTempNodePtr%NodePtr=>InNodeObjectList(TempNodeNum)
        call AssignTwoNodeElemToFullMatrix(InMeshElemList(i)%StiffMatrix,&
            FirstTempNodePtr%NodePtr%Freedom,&
            SecTempNodePtr%NodePtr%Freedom,&
            FirstTempNodePtr%NodePtr%MatrixIndex,&
            SecTempNodePtr%NodePtr%MatrixIndex,&
            OutDynamicMatrixObject%StiffMatrix)
    end do    
    return
    end subroutine
    



    
    !计算动力学方程特征值
    subroutine CalEigenByMKL(InDynamicObject,InEigenOrder,OutEigenValue,OutEigenRightVector)
    use TAN_BasicDynamicMatrix
    use TAN_BasicNumeric
    implicit none
    type(CDynamicMatrix_Tan)::InDynamicObject
    integer::InEigenOrder
    real::OutEigenValue(:,:),OutEigenRightVector(:,:)
    real,pointer::FirstDerivateMatrix(:,:)
    real,pointer::ZeroDerivateMatrix(:,:)
    integer::TotalFreedom
    integer::i
    TotalFreedom=size(InDynamicObject%StiffMatrix,1)
    allocate(FirstDerivateMatrix(2*TotalFreedom,2*TotalFreedom))
    allocate(ZeroDerivateMatrix(2*TotalFreedom,2*TotalFreedom))
    InEigenOrder=2*TotalFreedom
    FirstDerivateMatrix=0
    ZeroDerivateMatrix=0
    do i=1,TotalFreedom
        FirstDerivateMatrix(i,i)=real(1)
        ZeroDerivateMatrix(i,TotalFreedom+i)=real(1)
    end do
    FirstDerivateMatrix(TotalFreedom+1:InEigenOrder,TotalFreedom+1:InEigenOrder)=InDynamicObject%MassMatrix(:,:)

    ZeroDerivateMatrix(TotalFreedom+1:2*TotalFreedom,1:TotalFreedom)=-InDynamicObject%StiffMatrix(:,:)
    ZeroDerivateMatrix(TotalFreedom+1:2*TotalFreedom,TotalFreedom+1:2*TotalFreedom)=-InDynamicObject%DampMatrix(:,:) 
    !注意，特征值计算完成后矩阵会发生改变
    call  CalExtendEigenByQZ(ZeroDerivateMatrix,&
        FirstDerivateMatrix,&
        InEigenOrder,&
        OutEigenValue,&
        OutEigenRightVector)
    !测试数据

    deallocate (FirstDerivateMatrix)
    deallocate (ZeroDerivateMatrix)
    return
    end subroutine
    
    !subroutine CalEigenByMKL(InDynamicObject)
    !implicit none
    !type(CDynamicMatrix_Tan)::InDynamicObject
    !求解动力学方程，前半部分为0阶项，后半部分为1阶项
    subroutine SolveDynamicEqu(InSolveMatrix,InTimeStart,InTimeList,InYStart,OutYList)
    use TAN_BasicNumeric
    implicit none
    real,target::InSolveMatrix(:,:)
    real::InTimeStart,InTimeList(:),InYStart(:),OutYList(:,:)
    external::BuildDynamicEqu
    STDODESolveMatrix=>InSolveMatrix
    call SolveSTDODEquByRKV(BuildDynamicEqu,InSolveMatrix,InTimeStart,InTimeList,InYStart,OutYList)    
    return
    end subroutine
    !计算动力响应初值
    subroutine CalDynamicResponse(InDynamicObject,InTimeStart,InTimeEnd,InSubStep,InYStart,OutTimeList,OutYList)
    use TAN_BasicDynamicMatrix
    use TAN_BasicNumeric
    implicit none
    type(CDynamicMatrix_Tan)::InDynamicObject
    real::InTimeStart,InTimeEnd,InYStart(:),OutTimeList(:),OutYList(:,:)
    integer::InSubStep
    real,pointer::nODESolveMatrix(:,:)
    integer::EquNum,i
    real::TimeIncr
    EquNum=size(InYStart,1) 
    allocate (nODESolveMatrix(EquNum,EquNum))    
    call BuildSTDDynamicEqu(InDynamicObject,nODESolveMatrix)
    TimeIncr=real(InTimeEnd-InTimeStart)/real(InSubStep)
    do i=1,InSubStep
        OutTimeList(i)=InTimeStart+i*TimeIncr
    end do    
    call SolveDynamicEqu(nODESolveMatrix,InTimeStart,OutTimeList,InYStart,OutYList)
    deallocate (nODESolveMatrix)
    return
    end subroutine
    
    
    end module
    
    subroutine BuildDynamicEqu(EquNum,Time,y,dydt)
    use TAN_BasicNumeric
    use f95_precision
    use blas95
    implicit none
    integer::EquNum
    real::Time
    real::y(EquNum),dydt(EquNum)
    integer::i,TotalFreedom
    TotalFreedom=EquNum/2
    do i=1,TotalFreedom
        dydt(i)=y(TotalFreedom+i)
    end do
    call gemv(STDODESolveMatrix(TotalFreedom+1:EquNum,1:EquNum),y,dydt(TotalFreedom+1:EquNum))
    return
    end subroutine

    !end module