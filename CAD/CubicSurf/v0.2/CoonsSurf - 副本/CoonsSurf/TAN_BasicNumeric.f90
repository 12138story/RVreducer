
    module TAN_BasicNumeric
    implicit none
    contains
    !线性代数
    !计算扩展特征值
    subroutine CalExtendEigenByQZ(InAMatrix,&
        InBMatrix,&
        InEigenOrder,&
        OutEigenValue,&
        OutEigenRightVector)
    use f95_precision
    use lapack95
    implicit none
    real::InAMatrix(:,:),InBMatrix(:,:)
    integer::InEigenOrder
    real::OutEigenValue(:,:)
    real::OutEigenRightVector(:,:)
    integer::N
    real,pointer::beta(:)
    real,pointer::EigenLeftVector(:,:)
    integer::info=1000
    N=size(InAMatrix,1)
    allocate(beta(N))
    allocate(EigenLeftVector(N,N))
    call ggev(InAMatrix,InBMatrix,&
        OutEigenValue(:,1),OutEigenValue(:,2),&
        beta,&
        vl=EigenLeftVector,info=info)
    OutEigenValue(:,1)=OutEigenValue(:,1)/beta
    OutEigenValue(:,2)=OutEigenValue(:,2)/beta
    deallocate(beta)
    deallocate(EigenLeftVector)
    end subroutine   
    !计算逆矩阵
    subroutine CalMatrixInverse_Tan(InMatrix,OutMatrix)
    use lapack95
    implicit none
    real,pointer::InMatrix(:,:)
    real,pointer::OutMatrix(:,:)
    integer,pointer::MKL_ipiv(:)
    allocate(MKL_ipiv(size(InMatrix,1)))
    if(.not.associated(InMatrix,OutMatrix)) then
        OutMatrix=InMatrix
    end if
    call getrf(OutMatrix,MKL_ipiv)
    call getri(OutMatrix,MKL_ipiv)
    deallocate(MKL_ipiv)
    end subroutine
    !计算无穷范数
    real function CalInfiniteNormOfVector_Tan(InVector)
    implicit none
    real::InVector(:)
    CalInfiniteNormOfVector_Tan=maxval(abs(InVector))
    end function
    !找到第一个比准则数大的index
    integer function FindFirstLargerValueIndex(InCritValue,InDataList)
    implicit none
    real::InCritValue,InDataList(:)
    integer::DataLength
    integer::low,high,mid
    integer::i
    !FindFirstLargerValueIndex=minloc(InDataList,1,InDataList>InCritValue)
    !!检验是否为最大值，因为如果为最大值时Index也为1
    !if (FindFirstLargerValueIndex==1) then
    !    if(all(InDataList<=InCritValue)) then
    !        FindFirstLargerValueIndex=size(InDataList)
    !    end if
    !end if    
    
    
    !二分查找
    DataLength=size(InDataList)
    !当
    if(DataLength==0) then
        FindFirstLargerValueIndex=1
    return
    end if
    if(InCritValue<InDataList(1)) then
        FindFirstLargerValueIndex=1        
        return
    else if(InCritValue>=InDataList(DataLength)) then
        FindFirstLargerValueIndex=DataLength
        return
    end if
    !!二分查找主体
    !low=1
    !high=DataLength
    !i=0
    !do while(low<=high)
    !    i=i+1
    !    mid=(low+high)/2
    !    if (mid==low) then
    !        FindFirstLargerValueIndex=mid+1
    !        write(6,*) i
    !        return
    !    else if (InCritValue<InDataList(mid)) then
    !        high=mid
    !    else
    !        low=mid
    !    end if   
    !end do
    !!插值查找
    low=1
    high=DataLength
    !i=0
    do while(low<=high)
        !i=i+1
        !write(6,*) low+(InCritValue-InDataList(low))/(InDataList(high)-(InDataList(low)))*(high-low)
        mid=low+(InCritValue-InDataList(low))/(InDataList(high)-(InDataList(low)))*(high-low)
        if ((InCritValue>=InDataList(mid)).AND.(InCritValue<InDataList(mid+1))) then
            FindFirstLargerValueIndex=mid+1
            !write(6,*) i
            return
        else if (InCritValue<InDataList(mid)) then
            high=mid
        else
            low=mid
        end if   
    end do
    return    
    
    end function
    

    !一元牛顿迭代
    real function SolveEquByNewtonMethodWithDerive(InXGuess,Equ,EquCoef)
    use TAN_BasicFunction
    implicit none
    real::InXGuess
    external::Equ
    real::EquValue,DEquValue,EquCoef(:)
    real::XTemp
    integer::i,MaxIterNum,EquCoefNum
    real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    EquCoefNum=size(EquCoef)
    MaxIterNum=1000
    AbsErrorCrit=0.000001
    i=1
    XTemp=InXGuess
    call Equ(EquCoefNum,XTemp,EquCoef,EquValue,DEquValue)
    do while(i<MaxIterNum .AND. abs(EquValue)>AbsErrorCrit)
        call Equ(EquCoefNum,XTemp,EquCoef,EquValue,DEquValue)
        XTemp=(XTemp-EquValue/DEquValue)
        i=i+1
    end do
    call JudgeCondition(i .GE. MaxIterNum,&
        'Iteration number of single equation exceeds defined maximum Number.')
    call JudgeCondition(isnan(InXGuess),&
        'The guess value of an equation is NaN.')
    SolveEquByNewtonMethodWithDerive=XTemp
    return
    end function 
    
    !多元牛顿迭代
    subroutine SolveMultiEquByNewtonMethodWithDerive(InXGuess,Equ,EquCoef)
    use f95_precision
    use blas95
    use lapack95
    use TAN_BasicFunction
    implicit none
    real::InXGuess(:),EquCoef(:)
    external::Equ
    real,pointer::EquValue(:)
    real,pointer::TempDEquMatrix(:,:)
    integer::i,MaxIterNum,EquNum,EquCoefNum
    real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    integer,pointer::MKL_ipiv(:)
    EquNum=size(InXGuess)
    allocate(MKL_ipiv(EquNum))
    allocate(EquValue(EquNum))
    allocate(TempDEquMatrix(EquNum,EquNum))    
    EquCoefNum=size(EquCoef)
    MaxIterNum=1000
    AbsErrorCrit=0.000001
    i=1
    call Equ(EquNum,EquCoefNum,InXGuess,EquCoef,EquValue,TempDEquMatrix)
    do while(i<MaxIterNum .AND. any(abs(EquValue)>AbsErrorCrit))
        call Equ(EquNum,EquCoefNum,InXGuess,EquCoef,EquValue,TempDEquMatrix)
        EquValue=-EquValue
        call getrf(TempDEquMatrix,MKL_ipiv)
        call getrs(TempDEquMatrix,MKL_ipiv,EquValue)
        InXGuess=InXGuess+EquValue
        i=i+1
    end do
    call JudgeCondition(i .GE. MaxIterNum,&
        'Iteration number of multi-equation exceeds defined maximum Number.')
    call JudgeCondition(isnan(InXGuess(1)) .OR. isnan(InXGuess(1)),&
        'A guess value of equations is NaN.')
    deallocate(MKL_ipiv)
    deallocate(EquValue)
    deallocate(TempDEquMatrix)
    return
    end subroutine
    !数值微分
    !标量函数偏微分,变量列表，函数名，对第几个参量微分
    real function FPartialDeriveScalarFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex)
    use blas95
    implicit none
    real::InValueList(:)
    integer::InPartIndex
    integer::TempValueCount,TempFunctionValueCount
    real::TempOriginValue
    interface
        real function InFunction(InValueList)
            real ::InValueList(:)            
        end function
    end interface
    TempValueCount=size(InValueList)
    TempOriginValue=InValueList(InPartIndex)
    InValueList(InPartIndex)=InValueList(InPartIndex)+0.0001d0
    FPartialDeriveScalarFunctionByCentralDiff_Tan=InFunction(InValueList)
    InValueList(InPartIndex)=InValueList(InPartIndex)-0.0002d0
    FPartialDeriveScalarFunctionByCentralDiff_Tan=(FPartialDeriveScalarFunctionByCentralDiff_Tan-InFunction(InValueList))*5000d0
    InValueList(InPartIndex)=TempOriginValue
    return
    end function
    
    !向量函数偏微分
    subroutine FPartialDeriveVectorFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex,OutVector)
    use blas95
    implicit none
    real::InValueList(:)
    integer::InPartIndex
    real::OutVector(:)
    integer::TempValueCount,TempFunctionValueCount
    real,pointer::pBackResult(:)
    real::TempOriginValue
    interface
        subroutine InFunction(InValueList,OutVector)
            real ::InValueList(:)
            real ::OutVector(:)
        end subroutine
    end interface
    TempValueCount=size(InValueList)
    TempFunctionValueCount=size(OutVector)
    TempOriginValue=InValueList(InPartIndex)
    InValueList(InPartIndex)=InValueList(InPartIndex)+0.0001d0
    call InFunction(InValueList,OutVector)
    InValueList(InPartIndex)=InValueList(InPartIndex)-0.0002d0
    allocate(pBackResult(TempFunctionValueCount))    
    call InFunction(InValueList,pBackResult)
    InValueList(InPartIndex)=TempOriginValue
    call axpy(pBackResult,OutVector,-1d0)
    call scal(OutVector,5d3)
    deallocate(pBackResult)
    return
    end subroutine
    !标量函数2重偏微分
    real function FPartialDerive2ScalarFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex1,InPartIndex2)
    use blas95
    implicit none
    real::InValueList(:)
    integer::InPartIndex1,InPartIndex2
    integer::TempValueCount,TempFunctionValueCount
    real::TempOriginValue
    real,pointer::pTempResult(:)
    interface
        function InFunction(ValueList)
            real ::ValueList(:)        
            real :: InFunction
        end function
    end interface
    allocate(pTempResult(3))
    TempOriginValue=InValueList(InPartIndex2)
    if (InPartIndex1 .EQ. InPartIndex2) then        
        pTempResult(2)=InFunction(InValueList)
        InValueList(InPartIndex2)=InValueList(InPartIndex2)+0.0002d0
        pTempResult(1)=InFunction(InValueList)
        InValueList(InPartIndex2)=InValueList(InPartIndex2)-0.0004d0
        pTempResult(3)=InFunction(InValueList)
        FPartialDerive2ScalarFunctionByCentralDiff_Tan=(pTempResult(1)+pTempResult(3)-pTempResult(2)*2d0)*25d6
    else
        InValueList(InPartIndex2)=InValueList(InPartIndex2)+0.0001d0
        pTempResult(1)=FPartialDeriveScalarFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex1)
        InValueList(InPartIndex2)=InValueList(InPartIndex2)-0.0002d0
        pTempResult(2)=FPartialDeriveScalarFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex1)
        FPartialDerive2ScalarFunctionByCentralDiff_Tan=(pTempResult(1)-pTempResult(2))*5d3
    end if
    deallocate(pTempResult)
    InValueList(InPartIndex2)=TempOriginValue
    return
    end function
    !向量函数2重偏微分
    subroutine FPartialDerive2VectorFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex1,InPartIndex2,OutVector)
    use blas95
    implicit none
    real::InValueList(:)
    integer::InPartIndex1,InPartIndex2
    real::OutVector(:)
    integer::TempValueCount,TempFunctionValueCount
    real,pointer::pTempResult(:)
    real::TempOriginValue
    interface
        subroutine InFunction(InValueList,OutVector)
            real ::InValueList(:)
            real ::OutVector(:)
        end subroutine
    end interface
    TempFunctionValueCount=size(OutVector)
    allocate(pTempResult(TempFunctionValueCount))
    TempOriginValue=InValueList(InPartIndex2)
    if (InPartIndex1 .EQ. InPartIndex2) then
        call InFunction(InValueList,pTempResult)
        InValueList(InPartIndex2)=InValueList(InPartIndex2)+0.0002d0
        call InFunction(InValueList,OutVector)
        call axpy(pTempResult,OutVector,-2d0)
        InValueList(InPartIndex2)=InValueList(InPartIndex2)-0.0004d0
        call InFunction(InValueList,pTempResult)
        call axpy(pTempResult,OutVector)
        call scal(OutVector,25d6)        
    else
        InValueList(InPartIndex2)=InValueList(InPartIndex2)+0.0001d0
        call FPartialDeriveVectorFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex1,OutVector)
        InValueList(InPartIndex2)=InValueList(InPartIndex2)-0.0002d0
        call FPartialDeriveVectorFunctionByCentralDiff_Tan(InValueList,InFunction,InPartIndex1,pTempResult)
        call axpy(pTempResult,OutVector,-1d0)
        call scal(OutVector,5d3)
    end if
    InValueList(InPartIndex2)=TempOriginValue        
    deallocate(pTempResult)
    return
    end subroutine
    
    !求标量函数梯度
    subroutine CalGradientForScalarFunction_Tan(InValueList,InFunction,OutVector)
    implicit none
    real::InValueList(:),OutVector(:)
    integer::TempValueCount,i
    interface
        real function InFunction(InValueList)
            real ::InValueList(:)            
        end function
    end interface
    TempValueCount=size(InValueList)
    do i=1,TempValueCount
        OutVector(i)=FPartialDeriveScalarFunctionByCentralDiff_Tan(InValueList,InFunction,i)
    end do
    return
    end subroutine
    !求标量函数2阶梯度
    subroutine CalGradient2ForScalarFunction_Tan(InValueList,InFunction,OutMatrix)
    implicit none
    real::InValueList(:),OutMatrix(:,:)
    integer::TempValueCount,i,j
    interface
        real function InFunction(InValueList)
            real ::InValueList(:)            
        end function
    end interface
    TempValueCount=size(InValueList)
    !计算对角元素梯度
    do i=1,TempValueCount
        OutMatrix(i,i)=FPartialDerive2ScalarFunctionByCentralDiff_Tan(InValueList,InFunction,i,i)    
    end do
    do j=2,TempValueCount
        do i=1,j-1
            OutMatrix(i,j)=FPartialDerive2ScalarFunctionByCentralDiff_Tan(InValueList,InFunction,i,j)
            OutMatrix(j,i)=OutMatrix(i,j)
        end do       
    end do
    return
    end subroutine
    
    !求向量函数梯度
    subroutine CalGradientForVectorFunction_Tan(InValueList,InFunction,OutMatrix)
    implicit none
    real::InValueList(:),OutMatrix(:,:)
    integer::TempValueCount,i
    interface
        subroutine InFunction(InValueList,OutVector)
            real ::InValueList(:),OutVector(:)     
        end subroutine
    end interface
    TempValueCount=size(InValueList)
    do i=1,TempValueCount
        call FPartialDeriveVectorFunctionByCentralDiff_Tan(InValueList,InFunction,i,OutMatrix(:,i))
    end do    
    return
    end subroutine
    
    !数值最优化方法,多元单值函数
    !牛顿法-单步
    subroutine FindMinValueByNewtonOneStep_Tan(InGradVector,InGrad2InvMatrix,IOlValueList)
    use blas95
    implicit none
    real::IOlValueList(:),InGradVector(:),InGrad2InvMatrix(:,:)
    call gemv(InGrad2InvMatrix,InGradVector,IOlValueList,-1d0,1d0)
    end subroutine
    

    !最优梯度法-单步
    subroutine FindMinValueByOptimumGradientOneStep_Tan(InGradVector,InGrad2Matrix,IOlValueList)
    use blas95
    implicit none
    real::IOlValueList(:),InGradVector(:),InGrad2Matrix(:,:)
    real,pointer::pTempVector(:)
    real::Alpha
    allocate(pTempVector(size(IOlValueList)))
    call gemv(InGrad2Matrix,InGradVector,pTempVector)
    Alpha=dot(InGradVector,InGradVector)/dot(pTempVector,InGradVector)    
    call axpy(InGradVector,IOlValueList,-Alpha)
    deallocate(pTempVector)
    end subroutine
    !共轭梯度法-单步
    subroutine FindMinValueByConjugateGradientOneStep_Tan(InGradVector,InGrad2Matrix,InSearchDirect,IOlValueList)
    use blas95
    implicit none
    real::IOlValueList(:),InGradVector(:),InGrad2Matrix(:,:),InSearchDirect(:)
    real,pointer::pTempVector(:)
    real::Lamada
    allocate(pTempVector(size(IOlValueList)))
    call gemv(InGrad2Matrix,InSearchDirect,pTempVector)
    Lamada=dot(InGradVector,InSearchDirect)/dot(pTempVector,InSearchDirect)    
    call axpy(InSearchDirect,IOlValueList,-Lamada)
    deallocate(pTempVector)
    end subroutine
    !共轭梯度法-方向
    subroutine FindConjugateGradientDirect_Tan(InGradVector0,InGradVector1,IOSearchDirect)
    use blas95
    implicit none
    real::InGradVector0(:),InGradVector1(:),IOSearchDirect(:)
    real::Alpha
    Alpha=dot(InGradVector1,InGradVector1)/dot(InGradVector0,InGradVector0)
    call scal(IOSearchDirect,Alpha)
    call axpy(InGradVector1,IOSearchDirect,-1d0)
    end subroutine
    !优化程序集合,InOptimumMethod=1-牛顿法，2-最优梯度法，3-共轭梯度法
    real function FindMinValue_Tan(InFunction,InAbsolueError,InMaxCycleNum,InOptimumMethod,IOlValueList)
    use blas95
    use TAN_BasicFunction
    implicit none
    real::IOlValueList(:),InAbsolueError
    integer::InMaxCycleNum,InOptimumMethod,ValueCount,i 
    real,pointer::pTempValue(:),pGradVector(:),pGrad2Matrix(:,:)
    real,pointer::GradVectorPointer0(:),GradVectorPointer1(:),SearchDirectPointer(:),TempGradVectorPointer(:)
    interface
        real function InFunction(InValueList)
            real ::InValueList(:)            
        end function
    end interface
    ValueCount=size(IOlValueList)
    allocate(pTempValue(ValueCount))
    if(InOptimumMethod==3) then
        allocate(pGradVector(3*ValueCount))
        TempGradVectorPointer=>pGradVector(1:ValueCount)
        GradVectorPointer1=>pGradVector(ValueCount+1:2*ValueCount)
        SearchDirectPointer=>pGradVector(2*ValueCount+1:3*ValueCount)
        GradVectorPointer0=>SearchDirectPointer
    else
        allocate(pGradVector(ValueCount))
        GradVectorPointer0=>pGradVector(1:ValueCount)
    end if
    
    allocate(pGrad2Matrix(ValueCount,ValueCount))
    call copy(IOlValueList,pTempValue)
    do i=1,InMaxCycleNum
        call CalGradientForScalarFunction_Tan(IOlValueList,InFunction,GradVectorPointer0)
        call CalGradient2ForScalarFunction_Tan(IOlValueList,InFunction,pGrad2Matrix)
        select case(InOptimumMethod)
        case(1)
            call CalMatrixInverse_Tan(pGrad2Matrix,pGrad2Matrix)
            call FindMinValueByNewtonOneStep_Tan(GradVectorPointer0,pGrad2Matrix,IOlValueList)
        case(2)
            call FindMinValueByOptimumGradientOneStep_Tan(GradVectorPointer0,pGrad2Matrix,IOlValueList)
        case(3)
            call FindMinValueByConjugateGradientOneStep_Tan(GradVectorPointer0,pGrad2Matrix,SearchDirectPointer,IOlValueList)
            call CalGradientForScalarFunction_Tan(IOlValueList,InFunction,GradVectorPointer1)
            call FindConjugateGradientDirect_Tan(GradVectorPointer0,GradVectorPointer1,SearchDirectPointer)
            GradVectorPointer0=>GradVectorPointer1
            GradVectorPointer1=>TempGradVectorPointer
            TempGradVectorPointer=>GradVectorPointer0
        end select
        
        if(CalInfiniteNormOfVector_Tan(pTempValue-IOlValueList)<InAbsolueError) then
            FindMinValue_Tan=InFunction(IOlValueList)
            return
        else
            call copy(IOlValueList,pTempValue)
        end if            
    end do
    FindMinValue_Tan=InFunction(IOlValueList)
    call JudgeCondition(i>InMaxCycleNum,'FindMinValue_TanByNewton未在设定循环次数内达到指定容差')
    deallocate(pTempValue)
    deallocate(pGradVector)
    deallocate(pGrad2Matrix)
    end function



    end module
    
