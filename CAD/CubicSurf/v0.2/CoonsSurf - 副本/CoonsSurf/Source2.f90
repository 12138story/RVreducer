    use TAN_BasicFunction
    use TAN_BasicNumeric
    use Tan_CubicSpline
    use TAN_CubicSurf
    use TAN_DifferentialGeometry
    use blas95
    IMPLICIT NONE
    real::TestValue(2)=[0d0,0d0],aaa=5,TestGradValue(3)=[0.1d0,0.1d0,0.1d0]
    real::TestOutVec(3)
    real,target::TestMatrix(3,3)=[[1,2,3],[4,10,6],[7,8,9]]
    real,target::TestInvMatrix(3,3)
    real::TestOutMatrix(3,3)
    real,pointer::TMatrix(:,:)
    real,pointer::TMatrix2(:,:)
    real,external::func
    real,target::TestGrad2Matrix(2,2)
    real::TestGradVector(2)
    integer::i
    call FPartialDerive2VectorFunctionByCentralDiff(TestValue,testfun2,1,2,TestOutVec)
    call CalGradient2ForScalarFunction(TestGradValue,testgrad2,TestInvMatrix)
    !call FindMinValueByNewtonOneStep(TestGradVector,TestGrad2Matrix,TestValue)
    TMatrix=>TestGrad2Matrix
    !do i=1,10
    !    call CalGradientForScalarFunction(TestValue,TestMinFunction,TestGradVector)
    !    call CalGradient2ForScalarFunction(TestValue,TestMinFunction,TestGrad2Matrix)        
    !    call CalMatrixInverse(TMatrix,TMatrix)
    !    call FindMinValueByNewtonOneStep(TestGradVector,TestGrad2Matrix,TestValue)
    !    write(6,*)TestValue
    !end do
    
    !    do i=1,10
    !    call CalGradientForScalarFunction(TestValue,TestMinFunction,TestGradVector)
    !    call CalGradient2ForScalarFunction(TestValue,TestMinFunction,TestGrad2Matrix)        
    !    call FindMinValueByOptimumGradientOneStep(TestGradVector,TestGrad2Matrix,TestValue)
    !    write(6,*)TestValue
    !end do
    call CalGradientForScalarFunction(TestValue,TestMinFunction,TestGradVector)
    call CalGradient2ForScalarFunction(TestValue,TestMinFunction,TestGrad2Matrix)  
    !call FindMinValueByConjugateGradientOneStep(TestGradVector,TestGrad2Matrix,TestGradVector,TestValue)
    
    write(6,*) FindMinValue(TestMinFunction,0.000000000001d0,20,1,TestValue)
    write(6,*) TestValue
    TestValue=[0d0,0d0]
    write(6,*) FindMinValue(TestMinFunction,0.000000000001d0,20,3,TestValue)
    write(6,*) TestValue
    !call PrintMatrixByDeviceNum(TestGrad2Matrix,6)
    
    !call PrintMatrixByDeviceNum(TestGrad2Matrix,6)
    !write(6,*) FPartialDerive2ScalarFunctionByCentralDiff(TestValue,TestMinFunction,2,1)
    
    

    !TMatrix=>TestMatrix
    !TMatrix2=>TestMatrix
    !
    !write(6,*) TestMinFunction(TestValue)
    !
    !call CalMatrixInverse(TMatrix,TMatrix2)
    !call PrintMatrixByDeviceNum(TMatrix,6)
    !write(6,*) 'aaaa'
    !
    !call PrintMatrixByDeviceNum(TMatrix2,6)
    !write(6,*)  FPartialDeriveScalarFunctionByCentralDiff([1.0,-2.0],func,1)
    !call CalGradientForVectorFunction(TestValue,TestFunction,TestOutMatrix)
    !call PrintMatrixByDeviceNum(TestOutMatrix,6)
    !
    !!call FPartialDeriveVectorFunctionByCentralDiff(TestValue,testfun2,1,TestOutVec)
    !Call CalGradientForScalarFunction([1.0,-2.0],func,TestOutVec)
    !write(6,*) TestOutVec
    end
    
    
    subroutine testfun(InValueList,OutVector)
    implicit none
    real::InValueList(:)
    real::OutVector(:)
    OutVector(1)=sin(InValueList(1)+InValueList(2))
    OutVector(2)=Cos(InValueList(1)-InValueList(2))
    OutVector(3)=Cos(InValueList(1)-InValueList(2))
    end subroutine
    
    real function func(x)
    implicit none
    real::x(:)
    func=sin(x(1))-cos(x(2))
    end function
    

