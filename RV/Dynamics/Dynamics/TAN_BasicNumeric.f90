
    module TAN_BasicNumeric
    implicit none
    real,pointer::STDODESolveMatrix(:,:)

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
        
    !标准微分方程求解 dy=Ay
    subroutine SolveSTDODEquByRKV(InODEquation,InAMatrix,tStart,tList,yStart,yList)
    use f95_precision
    use blas95
    implicit none
    real::tStart,tList(:),yStart(:),yList(:,:)
    real,target::InAMatrix(:,:)
    real::Tol!方程容差
    real::Param(50)!方程控制参数
    integer::IDO!方程开辟空间参数
    integer::EquNum,SolveStep
    integer::i
    external::InODEquation
    real::tStartTemp
    real,pointer::yStartTemp(:)
    
    
    STDODESolveMatrix=>InAMatrix
    EquNum=size(yStart,1)   
    SolveStep=size(tList,1)
    Tol = 0.0005
    Param=real(0)
    Param(10) = 1.0 
    Param(4)=500
    tStartTemp=tStart
    IDO=1    
    allocate(yStartTemp(EquNum))
    yStartTemp=yStart
    call DIVPRK(IDO, EquNum, InODEquation, tStartTemp, tList(1), Tol, Param, yStartTemp)
    call copy(yStartTemp,yList(:,1))
    !IDO=2
    do i=2,SolveStep
        IDO=3
        call DIVPRK(IDO, EquNum, InODEquation, tStartTemp, tList(i-1), Tol, Param, yStartTemp)
        IDO=1
        call DIVPRK(IDO, EquNum, InODEquation, tStartTemp, tList(i), Tol, Param, yStartTemp)
        call copy(yStartTemp,yList(:,i))
    end do
    IDO=3
    call DIVPRK(IDO, EquNum, InODEquation, tStartTemp, tList(SolveStep), Tol, Param, yStartTemp)
    call copy(yStartTemp,yList(:,SolveStep))
    deallocate(yStartTemp)
    return
    end subroutine
    end module
    
    subroutine CalODEquation(EquNum,Time,y,dydt)
    use TAN_BasicNumeric
    use f95_precision
    use blas95
    implicit none
    integer::EquNum
    real::Time
    real::y(EquNum),dydt(EquNum)
    call gemv(STDODESolveMatrix,y,dydt)
    return
    end subroutine