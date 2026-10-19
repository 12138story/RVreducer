
    use TAN_BasicFunction
    use TAN_NumericIntegral
    use TAN_NumericSolveEqu
    use TAN_NumericAlgebra
    implicit none
    real::TestEquVar(2)
    real::TestFromMatrixS0(4,4),TestToMatrixS1(4,4),TestOutMatrix(4,4)
    real::TestVec(20)
    integer::i
    !write(6,*)IntegralScalarFunction([1d0],FTestIntegral,1,InIntegralRange=[3d0,5d0])+&
    !    IntegralScalarFunction([1d0],FTestIntegral,1,InIntegralRange=[5d0,7d0])+&
    !    IntegralScalarFunction([1d0],FTestIntegral,1,InIntegralRange=[7d0,9d0])
    !write(6,*) IntegralScalarFunctionWithSubregion(6,[1d0],FTestIntegral,1,InIntegralRange=[3d0,9d0])
    !TestEquVar=2
    !call FindScalarEquRootByIteration_Tan(TestEqu,TestEquVar,InSolveVarIndex=1,InIterationMethod=2)
    !write(6,*)TestEquVar
    TestToMatrixS1=0
    do i=1,4
        TestToMatrixS1(i,i)=1d0
    end do    
    TestFromMatrixS0(:,1)=[0.707107, 0.707107, 0., 0.]
    TestFromMatrixS0(:,2)=[-0.707107, 0.707107, 0., 0.]
    TestFromMatrixS0(:,3)=[0., 0., 1., 0.]
    TestFromMatrixS0(:,4)=[0., 0., 0., 1.]
    TestFromMatrixS0(:,4)=[1d0,2d0, 0., 1.]
    call GenCoorTransMatrix_Tan(TestFromMatrixS0,TestOutMatrix)
    call PrintMatrixByDeviceNum(TestOutMatrix,6)

    end