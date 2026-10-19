
    module TAN_BasicNumeric
    implicit none
    contains
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
        
        
    integer function FindFirstLowerValueIndex(InCritValue,InDataList)
    implicit none
    real::InCritValue,InDataList(:)
    FindFirstLowerValueIndex=minloc(InDataList,1,InDataList>InCritValue)
    if (FindFirstLowerValueIndex==1) then
        if(all(InDataList<=InCritValue)) then
            FindFirstLowerValueIndex=size(InDataList)
        end if
    end if    
    return    
    end function
    
    !real function SolveEquByNewtonMethodWithDerive(InXGuess,Equ,DEqu)
    !implicit none
    !real::InXGuess
    !real::Equ,DEqu
    !real::XTemp
    !integer::i,MaxIterNum
    !logical::test
    !real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    !MaxIterNum=1000
    !AbsErrorCrit=0.000001
    !i=1
    !XTemp=InXGuess
    !test=i<MaxIterNum .AND. abs(Equ(XTemp))<AbsErrorCrit
    !do while(i<MaxIterNum .AND. abs(Equ(XTemp))>AbsErrorCrit)
    !    XTemp=(XTemp-Equ(XTemp)/DEqu(XTemp))
    !    i=i+1
    !end do
    !SolveEquByNewtonMethodWithDerive=XTemp
    !return
    !end function
    real function SolveEquByNewtonMethodWithDerive(InXGuess,Equ)
    implicit none
    real::InXGuess
    external::Equ
    real::EquValue,DEquValue
    real::XTemp
    integer::i,MaxIterNum
    real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    MaxIterNum=1000
    AbsErrorCrit=0.000001
    i=1
    XTemp=InXGuess
    call Equ(XTemp,EquValue,DEquValue)
    do while(i<MaxIterNum .AND. abs(EquValue)>AbsErrorCrit)
        call Equ(XTemp,EquValue,DEquValue)
        XTemp=(XTemp-EquValue/DEquValue)
        i=i+1
    end do
    SolveEquByNewtonMethodWithDerive=XTemp
    return
    end function 
    
    
    subroutine SolveMultiEquByNewtonMethodWithDerive(InXGuess,Equ,EquCoef)
    use f95_precision
    use blas95
    use lapack95
    implicit none
    real::InXGuess(:),EquCoef(:)
    external::Equ
    real,pointer::EquValue(:)
    real,pointer::TempDEquMatrix(:,:)
    !real,pointer::XTemp(:)
    integer::i,MaxIterNum,EquNum
    real::AbsErrorCrit!绝对误差准则，可以考虑相对误差准则
    integer,pointer::MKL_ipiv(:)
    EquNum=size(InXGuess)
    allocate(MKL_ipiv(EquNum))
    allocate(EquValue(EquNum))
    allocate(TempDEquMatrix(EquNum,EquNum))    
    !allocate(XTemp(EquNum))
    MaxIterNum=1000
    AbsErrorCrit=0.000001
    i=1
    !XTemp=InXGuess
    call Equ(EquNum,InXGuess,EquCoef,EquValue,TempDEquMatrix)
    do while(i<MaxIterNum .AND. any(abs(EquValue)>AbsErrorCrit))
        call Equ(EquNum,InXGuess,EquCoef,EquValue,TempDEquMatrix)
        EquValue=-EquValue
        call getrf(TempDEquMatrix,MKL_ipiv)
        call getrs(TempDEquMatrix,MKL_ipiv,EquValue)
        InXGuess=InXGuess+EquValue
        !InXGuess=(InXGuess+EquValue)
        i=i+1
    end do
    
    !SolveEquByNewtonMethodWithDerive=XTemp
    deallocate(MKL_ipiv)
    deallocate(EquValue)
    !deallocate(DEquMatrix)
    return
    end subroutine
    
    
    end module
    
