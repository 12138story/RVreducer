
    use TAN_StepShaft
    use TAN_BasicDynamicMatrix
    use TAN_DynamicSystem
    use TAN_MeshElement
    implicit none

    
    type(CStepShaft_Tan)::TestShaft
    integer::TestShaftSecNum,TestDiscNum
    real,pointer::TestShaftInPara(:,:),TestDiscInPara(:,:)
    
    
    
    integer::NodeCount,MatrixDimension
    type(CDynamicMatrix_Tan)::TestDynamicMatrixObject


    type(CNode_Tan),pointer::NodeObjectList(:)
    integer::i,j
    
    
    
    type(CMeshElem_Tan)::TestMeshList(0)
    type(CStepShaft_Tan)::TestStepShaftList(1)
    type(CDynamicMatrix_Tan)::TestDynamicSystemMatrixObject
    integer::EigenOrder
    real,pointer::TestEigenValue(:,:),TestEigenVector(:,:)
    integer::TotalFreedom
    !测试动力响应变量
    real::TestTStart,TestTEnd
    real,pointer::TestTList(:),YList(:,:),TestYStart(:)
    integer::TestSubStep
    
    
    
    TestShaftSecNum=1
    TestDiscNum=0
    allocate(TestShaftInPara(7,TestShaftSecNum))
    allocate(TestDiscInPara(5,TestDiscNum))
    do i=1,TestShaftSecNum
        TestShaftInPara(:,i)=[real(i),real(i+1),real(7850),0.01,0.4,2.1d11,0.3]
    end do
    !TestShaftInPara(:,1)=[real(1),real(3),real(7850),0.01,0.1,2.1d11,0.3]
    !TestShaftInPara(:,2)=[real(3),real(2),real(7850),0.01,0.1*3,2.1d11,0.3]
    !TestDiscInPara(:,1)=[real(3),real(7850),real(0.08d0),real(0.01d0),real(0)]
    call CreatStepShaftSpace(TestShaftSecNum,TestDiscNum,TestShaft)
    call BuildStepShaftElemList(TestShaftInPara,TestDiscInPara,TestShaft)

    
    !write(*,'(6F6.4)') TestShaft%DiscElemList(1)%MassMatrix(:,:)
    
    
    
    !real,dimension(36,36)::FullMatrix
    !real,dimension(12,12)::BeamTestMatrix
    !logical,pointer::FreedomList(:,:)
    !integer::FstNodeIndex=1
    !integer::SecNodeIndex=13
    !integer::i,j
    !type(CNode_Tan),pointer::NodeTest(:)
    !type(CNodePtr_Tan),pointer::NodePtrTest(:)
    !integer::NodeCount=3
    !integer,pointer::NodeNumList(:)
    !integer,pointer::MatrixLocation(:)
    
    !TestMeshInPara=(/Pi-20*Degree, 20*Degree,0*Degree,0.08d0,0.08d0,1d10/)
    

    
    

    


    NodeCount=2
    allocate(NodeObjectList(NodeCount))
    do i=1,NodeCount
        NodeObjectList(i)%Num=i
    end do
    !NodeObjectList(1)%Num=1
    !NodeObjectList(2)%Num=2
    !NodeObjectList(3)%Num=3
    NodeObjectList(1)%Freedom=[0,0,0,0,0,0]    
    !NodeObjectList(2)%Freedom(3)=0
    !NodeObjectList(3)%Freedom=[0,0,0,1,1,0]    
    MatrixDimension=0
    do i=1,NodeCount
        MatrixDimension=MatrixDimension+sum(abs(NodeObjectList(i)%Freedom))
    end do
    
    call CreatDynamicMatrixSpace(MatrixDimension,TestDynamicMatrixObject)
    
    

    call ConstructNodeObjectList(NodeObjectList,NodeObjectList)
    call ConstructSingleStepShaftDynamicMatrixObject(TestShaft,NodeObjectList,TestDynamicMatrixObject)
    


    call CreatDynamicMatrixSpace(&
        CalNodeListFreedom(NodeObjectList),&
        TestDynamicSystemMatrixObject)
    TestStepShaftList(1)=TestShaft 
    call ConstructDynamicObject(TestStepShaftList,TestMeshList,NodeObjectList,TestDynamicSystemMatrixObject)

    TotalFreedom=size(TestDynamicSystemMatrixObject%StiffMatrix,1)
    allocate(TestEigenValue(2*TotalFreedom,2))
    allocate (TestEigenVector(2*TotalFreedom,2*TotalFreedom))

    call CalEigenByMKL(TestDynamicSystemMatrixObject,EigenOrder,TestEigenValue,TestEigenVector) 
    write(*,*)"特征值实部"
    write(*,'(12F20.3)') TestEigenValue(:,1)  
    write(*,*)"特征值虚部"
    do i=1,TotalFreedom*2
    write(*,'(12F20.3)') TestEigenValue(i,2)
    end do
    !动力响应
    TestSubStep=100
    allocate(TestTList(TestSubStep))
    allocate(YList(2*TotalFreedom,TestSubStep))
    allocate(TestYStart(2*TotalFreedom))
    TestYStart=0
    TestYStart(3)=1
    TestTStart=0
    TestTEnd=0.014
    call CalDynamicResponse(TestDynamicSystemMatrixObject,TestTStart,TestTEnd,TestSubStep,TestYStart,TestTList,YList)
    
    open(unit=100,file="F:\Workspace\softdevelop\CAE\算法\Fortran测试数据\FortranStiffMatrix.dat")
    do i=1,6
            write(100,'(6F20.5)') TestDynamicSystemMatrixObject%StiffMatrix(i,:)
    end do
    close(100)
    open(unit=100,file="F:\Workspace\softdevelop\CAE\算法\Fortran测试数据\FortranMassMatrix.dat")
    do i=1,6
            write(100,'(6F20.15)') TestDynamicSystemMatrixObject%MassMatrix(i,:)
    end do
    close(100)
    
    deallocate(TestDiscInPara)
    deallocate(TestShaftInPara)
    !open(unit=100,file="F:\Workspace\softdevelop\CAE\算法\Fortran测试数据\SolveValue.dat")
    !do i=1,TestSubStep
    !        write(100,'(18F20.10)') YList(:,i)
    !end do
    !close(100)
    !open(unit=100,file="F:\Workspace\softdevelop\CAE\算法\Fortran测试数据\TValue.dat")
    !do i=1,TestSubStep
    !        write(100,'(F20.10)') TestTList(i)
    !end do
    !close(100)


    end program