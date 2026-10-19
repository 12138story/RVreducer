    module TAN_NumericSolveEqu
    use iso_c_binding, only: c_ptr
    implicit none
    
    contains
    !一元牛顿迭代
    !默认是割线法
    subroutine FindScalarEquRootByIteration_Tan(InFunction,IOVarList,InFuncObjPrtList,InVarDiff,InAbsolueError,InMaxCycleNum,&
        InSolveVarIndex,InIterationMethod)
    use TAN_NumericDiff
    use TAN_NumericOptimization
    use TAN_BasicFunction
    use TAN_BasicCoef
    use blas95
    implicit none
    real,target::IOVarList(:)
    real,optional,target::InVarDiff
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    real,optional::InAbsolueError
    integer,optional,target::InMaxCycleNum
    integer,optional::InSolveVarIndex
    integer,optional::InIterationMethod    
    real,pointer::TempVarDiff
    type(c_ptr),pointer::TempFuncObjPrtList(:)    
    real::TempAbsolueError
    integer,pointer::TempMaxCycleNum
    integer::TempSolveVarIndex,TempIterationMethod
    integer::VarCount,i
    real::Fxk0,Fxk1,Fxk2
    real,pointer::pTempVarList(:,:),xk0(:),xk1(:),xk2(:)
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if(present(InAbsolueError)) then
        TempAbsolueError=InAbsolueError
    else
        TempAbsolueError=DefaultAbsolueError
    end if
    if(present(InMaxCycleNum)) then
        TempMaxCycleNum=>InMaxCycleNum
    else
        TempMaxCycleNum=>DefaultMaxCycleNum
    end if
    if(present(InVarDiff)) then
        TempVarDiff=>InVarDiff
    else        
        TempVarDiff=>DefaultVarDiff(1)  
    end if    
    if(present(InSolveVarIndex)) then
        TempSolveVarIndex=InSolveVarIndex
    else        
        TempSolveVarIndex=1  
    end if
    if(present(InIterationMethod)) then
        TempIterationMethod=InIterationMethod
    else        
        TempIterationMethod=2  
    end if
    i=0
    select case(TempIterationMethod)!1-牛顿法，2-割线法
    case(1)
        Fxk1=InFunction(IOVarList,TempFuncObjPrtList)
        do while((abs(Fxk1)>TempAbsolueError).AND.(i<TempMaxCycleNum))
            IOVarList(TempSolveVarIndex)=IOVarList(TempSolveVarIndex)-Fxk1/FPartialDeriveScalarFunctionByCentralDiff_Tan(IOVarList,InFunction,TempSolveVarIndex,TempFuncObjPrtList,TempVarDiff)
            Fxk1=InFunction(IOVarList,TempFuncObjPrtList)
            i=i+1
        end do
    case(2)!割线法
        allocate(pTempVarList(2,size(IOVarList)))
        xk1=>pTempVarList(1,:)
        xk2=>pTempVarList(2,:)
        call copy(IOVarList,xk1)
        call copy(IOVarList,xk2)
        Fxk1=InFunction(xk1,TempFuncObjPrtList)
        xk2(TempSolveVarIndex)=xk2(TempSolveVarIndex)+TempVarDiff
        Fxk2=InFunction(xk2,TempFuncObjPrtList)
        do while((abs(Fxk2)>TempAbsolueError).AND.(i<TempMaxCycleNum))
            xk0=>xk1
            Fxk0=Fxk1
            xk1=>xk2
            Fxk1=Fxk2
            xk2=>xk0
            xk2(TempSolveVarIndex)=xk1(TempSolveVarIndex)-Fxk1*(xk1(TempSolveVarIndex)-xk0(TempSolveVarIndex))/(Fxk1-Fxk0)
            Fxk2=InFunction(xk2,TempFuncObjPrtList)
            i=i+1
        end do
        call copy(xk2,IOVarList)
        deallocate(pTempVarList)
    end select   
    call JudgeCondition_Tan(i .GE. TempMaxCycleNum,'Iteration number of single equation exceeds defined maximum Number')   
    end subroutine
        !一元牛顿迭代
    subroutine FindScalarEquRootByNewton_Tan(InFunction,IOVarList,InFuncObjPrtList,InVarDiff,InAbsolueError,InMaxCycleNum,&
        InSolveVarIndex)
    use TAN_NumericDiff
    use TAN_NumericOptimization
    use TAN_BasicFunction
    use TAN_BasicCoef
    implicit none
    real,target::IOVarList(:)
    real,optional,target::InVarDiff
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    real,optional::InAbsolueError
    integer,optional,target::InMaxCycleNum
    integer,optional::InSolveVarIndex
    integer::TempSolveVarIndex
    real,pointer::TempVarDiff
    type(c_ptr),pointer::TempFuncObjPrtList(:)    
    real::TempAbsolueError
    integer,pointer::TempMaxCycleNum
    integer::VarCount,i
    real::Fxk
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if(present(InAbsolueError)) then
        TempAbsolueError=InAbsolueError
    else
        TempAbsolueError=DefaultAbsolueError
    end if
    if(present(InMaxCycleNum)) then
        TempMaxCycleNum=>InMaxCycleNum
    else
        TempMaxCycleNum=>DefaultMaxCycleNum
    end if
    if(present(InVarDiff)) then
        TempVarDiff=>InVarDiff
    else        
        TempVarDiff=>DefaultVarDiff(1)  
    end if    
    if(present(InSolveVarIndex)) then
        TempSolveVarIndex=InSolveVarIndex
    else        
        TempSolveVarIndex=1  
    end if
    i=0
    Fxk=InFunction(IOVarList,TempFuncObjPrtList)
    do while((abs(Fxk)>TempAbsolueError).AND.(i<TempMaxCycleNum))
        IOVarList(TempSolveVarIndex)=IOVarList(TempSolveVarIndex)-Fxk/FPartialDeriveScalarFunctionByCentralDiff_Tan(IOVarList,InFunction,TempSolveVarIndex,TempFuncObjPrtList,TempVarDiff)
        Fxk=InFunction(IOVarList,TempFuncObjPrtList)
        i=i+1
    end do
    call JudgeCondition_Tan(i .GE. TempMaxCycleNum,'Iteration number of single equation exceeds defined maximum Number')   
    end subroutine    
        
    
    real function TestEqu(InVar,InCoef) result(ans)
    implicit none
    real::InVar(:)
    type(c_ptr),optional::InCoef(:)
    ans=InVar(1)**4-InVar(2)
    end function
        
        
    !real function SolveEquByNewtonMethodWithDerive(InXGuess,Equ,EquCoef)
    !use TAN_BasicFunction
    !implicit none
    !real::InXGuess
    !external::Equ
    !real::EquValue,DEquValue,EquCoef(:)
    !real::XTemp
    !integer::i,MaxIterNum,EquCoefNum
    !real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    !EquCoefNum=size(EquCoef)
    !MaxIterNum=1000
    !AbsErrorCrit=0.000001
    !i=1
    !XTemp=InXGuess
    !call Equ(EquCoefNum,XTemp,EquCoef,EquValue,DEquValue)
    !do while(i<MaxIterNum .AND. abs(EquValue)>AbsErrorCrit)
    !    call Equ(EquCoefNum,XTemp,EquCoef,EquValue,DEquValue)
    !    XTemp=(XTemp-EquValue/DEquValue)
    !    i=i+1
    !end do
    !call JudgeCondition_Tan(i .GE. MaxIterNum,&
    !    'Iteration number of single equation exceeds defined maximum Number.')
    !call JudgeCondition_Tan(isnan(InXGuess),&
    !    'The guess value of an equation is NaN.')
    !SolveEquByNewtonMethodWithDerive=XTemp
    !return
    !end function 
    !
    !!多元牛顿迭代
    !subroutine SolveMultiEquByNewtonMethodWithDerive(InXGuess,Equ,EquCoef)
    !use f95_precision
    !use blas95
    !use lapack95
    !use TAN_BasicFunction
    !implicit none
    !real::InXGuess(:),EquCoef(:)
    !external::Equ
    !real,pointer::EquValue(:)
    !real,pointer::TempDEquMatrix(:,:)
    !integer::i,MaxIterNum,EquNum,EquCoefNum
    !real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    !integer,pointer::MKL_ipiv(:)
    !EquNum=size(InXGuess)
    !allocate(MKL_ipiv(EquNum))
    !allocate(EquValue(EquNum))
    !allocate(TempDEquMatrix(EquNum,EquNum))    
    !EquCoefNum=size(EquCoef)
    !MaxIterNum=1000
    !AbsErrorCrit=0.000001
    !i=1
    !call Equ(EquNum,EquCoefNum,InXGuess,EquCoef,EquValue,TempDEquMatrix)
    !do while(i<MaxIterNum .AND. any(abs(EquValue)>AbsErrorCrit))
    !    call Equ(EquNum,EquCoefNum,InXGuess,EquCoef,EquValue,TempDEquMatrix)
    !    EquValue=-EquValue
    !    call getrf(TempDEquMatrix,MKL_ipiv)
    !    call getrs(TempDEquMatrix,MKL_ipiv,EquValue)
    !    InXGuess=InXGuess+EquValue
    !    i=i+1
    !end do
    !call JudgeCondition_Tan(i .GE. MaxIterNum,&
    !    'Iteration number of multi-equation exceeds defined maximum Number.')
    !call JudgeCondition_Tan(isnan(InXGuess(1)) .OR. isnan(InXGuess(1)),&
    !    'A guess value of equations is NaN.')
    !deallocate(MKL_ipiv)
    !deallocate(EquValue)
    !deallocate(TempDEquMatrix)
    !return
    !end subroutine

    end module
    