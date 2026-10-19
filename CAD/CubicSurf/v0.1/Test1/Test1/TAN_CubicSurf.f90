
    module TAN_CubicSurf
    implicit none
    type::CubicSurf_Tan
        integer::uPointNum,vPointNum,PointDimension
        real,pointer::uOriginParaList(:)
        real,pointer::vOriginParaList(:)
        real,pointer::SurfCoef(:,:,:)!系数,参考p101页4.16进行排序，行对应u向，列对应v项
        integer::CreatMethod!如果是1，则为动态创建，否则为0
    end type CubicSurf_Tan
    
    type(CubicSurf_Tan),pointer:: CubicSurfEquTransPtr
    
    contains
    
    
    subroutine CreatCubicSurfSpace(PointDimension,uPointNum,vPointNum,CubicSurfName)
    implicit none
    integer ::PointDimension,uPointNum,vPointNum
    type(CubicSurf_Tan) CubicSurfName    
    allocate (CubicSurfName%uOriginParaList(uPointNum))
    allocate (CubicSurfName%vOriginParaList(vPointNum))
    allocate (CubicSurfName%SurfCoef(PointDimension,2*uPointNum,2*vPointNum))
    if(CubicSurfName%CreatMethod .EQ. 0) then
        call DeleteCubicSurfSpace(CubicSurfName)        
    end if
    CubicSurfName%uOriginParaList=0
    CubicSurfName%vOriginParaList=0
    CubicSurfName%SurfCoef=0
    CubicSurfName%uPointNum=uPointNum
    CubicSurfName%vPointNum=vPointNum
    CubicSurfName%PointDimension=PointDimension
    CubicSurfName%CreatMethod=1
    return    
    end subroutine
    
    subroutine DeleteCubicSurfSpace(CubicSurfName)
    implicit none
    type(CubicSurf_Tan) CubicSurfName
    if (CubicSurfName%CreatMethod .EQ. 1) then
    deallocate (CubicSurfName%uOriginParaList)
    deallocate (CubicSurfName%vOriginParaList)
    deallocate (CubicSurfName%SurfCoef)    
    end if
    CubicSurfName%CreatMethod=0
    return
    end subroutine
    
    subroutine Cal1DCubicSurfCoef(uList,vList,InValueTable,CubicSurfCoef)
    use TAN_BasicFunction
    use TAN_CubicSpline
    implicit none
    real::uList(:),vList(:),InValueTable(:,:),CubicSurfCoef(:,:)
    integer::uCount,vCount
    integer::i
    uCount=size(uList)
    vCount=size(vList)
    !数据检查
    call JudgeCondition(uCount .NE. size(InValueTable,1),&
        'Point count between uList and value table are not compatible')
    call JudgeCondition(vCount .NE. size(InValueTable,2),&
        'Point count between vList and value table are not compatible')
    do i=1,vCount
        call Cal1DCubicSplineCoef(uList,InValueTable(:,i),CubicSurfCoef(:,2*i-1))
    end do
    do i=1,uCount
        call Cal1DCubicSplineCoef(vList,InValueTable(i,:),CubicSurfCoef(2*i-1,:))
    end do
    do i=1,vCount
        call Cal1DCubicSplineCoef(uList,CubicSurfCoef(1:2*uCount:2,2*i),CubicSurfCoef(:,2*i))
    end do
    return
    end subroutine
 
    real function Cal1DCubicSurfValue(uValue,vValue,uList,vList,InCubicSurfCoef)
    use TAN_CubicSpline
    use f95_precision
    use blas95
    implicit none
    real::uValue,vValue,uList(:),vList(:),InCubicSurfCoef(:,:)
    integer::uIndex,vIndex
    real::uBaseVector(4),vBaseVector(4)
    real::TempVector(4)
    uIndex=FindSplineVariantIndex(uValue,uList)
    vIndex=FindSplineVariantIndex(vValue,vList)
    call CalBaseVector(uValue,&
        uList(uIndex-1),&
        uList(uIndex),&
        uBaseVector)
    call CalBaseVector(vValue,&
        vList(vIndex-1),&
        vList(vIndex),&
        vBaseVector)
    call gemv(InCubicSurfCoef(2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
        vBaseVector,TempVector)
    Cal1DCubicSurfValue=dot(uBaseVector,TempVector)
    return
    end function
    
    !计算样条曲面系数
    subroutine BuildCubicSurfObject_Tan(uList,vList,InPointTable,IOCubicSurf)
    use TAN_BasicFunction
    implicit none
    real::uList(:),vList(:),InPointTable(:,:,:)
    type(CubicSurf_Tan)::IOCubicSurf
    integer::i
    call JudgeCondition(size(InPointTable,1) .NE. IOCubicSurf%PointDimension,&
        'Point dimension differs from the cubic surf object')
    !IOCubicSurf%PointDimension=size(InPointTable,1)
    IOCubicSurf%uOriginParaList=uList
    IOCubicSurf%vOriginParaList=vList
    do i=1,IOCubicSurf%PointDimension
        call Cal1DCubicSurfCoef(uList,vList,InPointTable(i,:,:),IOCubicSurf%SurfCoef(i,:,:))
    end do
    return
    end subroutine
    
    !计算样条曲面内插值
    subroutine CalCubicSurfInterpValue_Tan(uValue,vValue,InCubicSurf,OutVector)
    implicit none
    real::uValue,vValue,OutVector(:)
    type(CubicSurf_Tan)::InCubicSurf
    integer::i
    do i=1,InCubicSurf%PointDimension
        OutVector(i)=Cal1DCubicSurfValue(&
            uValue,&
            vValue,&
            InCubicSurf%uOriginParaList,&
            InCubicSurf%vOriginParaList,&
            InCubicSurf%SurfCoef(i,:,:))
    end do
    return
    end subroutine
    !计算样条偏导数
    !VariantSign=0,对u求偏导
    !VariantSign=其它,对v求偏导
    subroutine CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,VariantSign,OutVector)
    use TAN_CubicSpline
    !use f95_precision
    use blas95
    use TAN_CubicSpline
    implicit none
    real::uValue,vValue,OutVector(:)
    integer::uIndex,vIndex
    type(CubicSurf_Tan)::InCubicSurf
    integer::VariantSign
    real::uBaseVector(4),vBaseVector(4)
    real::TempVector(4)
    integer::i
    uIndex=FindSplineVariantIndex(uValue,InCubicSurf%uOriginParaList)
    vIndex=FindSplineVariantIndex(vValue,InCubicSurf%vOriginParaList)
    if (VariantSign .EQ. 0) then
        call CalBaseVector1Derive(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            uBaseVector)
        call CalBaseVector(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            vBaseVector)        

        do i=1,InCubicSurf%PointDimension
            call gemv(InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
            vBaseVector,&
            TempVector)            
            OutVector(i)=dot(uBaseVector,TempVector)            
        end do
    else
        call CalBaseVector(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            uBaseVector)
        call CalBaseVector1Derive(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            vBaseVector)
        do i=1,InCubicSurf%PointDimension
            call gemv(InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
            vBaseVector,&
            TempVector)            
            OutVector(i)=dot(uBaseVector,TempVector)            
        end do 
    end if    
    return
    end subroutine
    
    
    
    !计算1阶全导数
    subroutine CalCubicSurfInterpTotalDerive_Tan(uValue,vValue,InCubicSurf,OutVector)
    use TAN_CubicSpline
    !use f95_precision
    use blas95
    use TAN_CubicSpline
    implicit none
    real::uValue,vValue,OutVector(:)
    integer::uIndex,vIndex
    type(CubicSurf_Tan)::InCubicSurf
    real::uBaseVector(4),vBaseVector(4)
    real::TempVector(4)
    integer::i
    uIndex=FindSplineVariantIndex(uValue,InCubicSurf%uOriginParaList)
    vIndex=FindSplineVariantIndex(vValue,InCubicSurf%vOriginParaList)        
    call CalBaseVector1Derive(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            uBaseVector)
    call CalBaseVector1Derive(vValue,&
        InCubicSurf%vOriginParaList(vIndex-1),&
        InCubicSurf%vOriginParaList(vIndex),&
        vBaseVector) 
    do i=1,InCubicSurf%PointDimension
        call gemv(InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
            vBaseVector,&
            TempVector)            
            OutVector(i)=dot(uBaseVector,TempVector)
    end do    
    return
    end subroutine
    
    !计算样条曲面线性外延值,ExtendMethod=0-u向，1-v向，其它-uv向
    subroutine CalCubicSurfLinearExtendValue_Tan(InuValue,InvValue,InCubicSurf,OutVector)
    use TAN_BasicFunction
    implicit none
    real::InuValue,InvValue,uValue,vValue,OutVector(:)
    type(CubicSurf_Tan)::InCubicSurf
    real::uStartValue,vStartValue
    integer::uStartIndex,vStartIndex!当线性外延时起始标号
    integer::uLocation,vLocation!当线性外延时方向标号-1-左侧，1-右侧
    real,pointer::ExtendStartPoint(:),ExtendTangentVector(:)
    uValue=InuValue
    vValue=InvValue
    uLocation=JudgeValueLocInOrderList(uValue,InCubicSurf%uOriginParaList,0)
    vLocation=JudgeValueLocInOrderList(vValue,InCubicSurf%vOriginParaList,0)
    allocate(ExtendStartPoint(InCubicSurf%PointDimension))
    allocate(ExtendTangentVector(InCubicSurf%PointDimension))
    if (vLocation .EQ. 0) then
        if (uLocation .EQ. 0) then    
            call CalCubicSurfInterpValue_Tan(uValue,&
            vValue,&
            InCubicSurf,&
            OutVector)
        else
            if (uLocation .EQ. -1) then
                uStartValue=InCubicSurf%uOriginParaList(1)
            else
                uStartValue=InCubicSurf%uOriginParaList(InCubicSurf%uPointNum)
            end if
            call CalCubicSurfInterpValue_Tan(uStartValue,&
                vValue,&
                InCubicSurf,&
                ExtendStartPoint)
            call CalCubicSurfInterpPartDerive_Tan(uStartValue,&
                vValue,&
                InCubicSurf,&
                0,&
                ExtendTangentVector)
            call CalExtendVector(uValue-uStartValue,&
                ExtendStartPoint,&
                ExtendTangentVector,&
                OutVector)
        end if
    else if (uLocation .EQ. 0) then
        if (vLocation .EQ. -1) then
            vStartValue=InCubicSurf%vOriginParaList(1)
        else
            vStartValue=InCubicSurf%vOriginParaList(InCubicSurf%vPointNum)
        end if        
        call CalCubicSurfInterpValue_Tan(uValue,&
            vStartValue,&
            InCubicSurf,&
            ExtendStartPoint)
        call CalCubicSurfInterpPartDerive_Tan(uValue,&
            vStartValue,&
            InCubicSurf,&
            1,&
            ExtendTangentVector)
        call CalExtendVector(vValue-vStartValue,&
            ExtendStartPoint,&
            ExtendTangentVector,&
            OutVector)
    else
        if (uLocation .EQ. -1) then
            uStartIndex=1
        else
            uStartIndex=InCubicSurf%uPointNum
        end if        
        if (vLocation .EQ. -1) then
            vStartIndex=1
        else
            vStartIndex=InCubicSurf%vPointNum
        end if
        uStartValue=InCubicSurf%uOriginParaList(uStartIndex)
        vStartValue=InCubicSurf%vOriginParaList(vStartIndex)
        ExtendStartPoint=InCubicSurf%SurfCoef(:,2*uStartIndex-1,2*vStartIndex-1)
        ExtendTangentVector=InCubicSurf%SurfCoef(:,2*uStartIndex,2*vStartIndex-1)!沿u向偏导
        call CalExtendVector(uValue-uStartValue,&
            ExtendStartPoint,&
            ExtendTangentVector,&
            OutVector)
        ExtendTangentVector=InCubicSurf%SurfCoef(:,2*uStartIndex-1,2*vStartIndex)!沿v向偏导
        call CalExtendVector(vValue-vStartValue,&
            OutVector,&
            ExtendTangentVector,&
            OutVector)
    end if
    deallocate(ExtendStartPoint)
    deallocate(ExtendTangentVector) 
    return
    end subroutine
    
    
    !计算样条曲面线性外延偏导值,ExtendMethod=0-u向，1-v向，其它-uv向
    subroutine CalCubicSurfLinearExtendPartDerive_Tan(InuValue,InvValue,InCubicSurf,OutDuVector,OutDvVector)
    use TAN_BasicFunction
    use TAN_CubicSpline
    !use f95_precision
    use blas95
    use TAN_CubicSpline
    implicit none
    real::InuValue,InvValue,OutDuVector(:),OutDvVector(:),uValue,vValue
    type(CubicSurf_Tan)::InCubicSurf
    real::uBaseVector(4),vBaseVector(4),DuBaseVector(4),DvBaseVector(4)
    real::TempVector(4)
    integer::i
    real::uStartValue,vStartValue
    integer::uStartIndex,vStartIndex,uIndex,vIndex,uLocation,vLocation!当线性外延时方向标号-1-左侧，1-右侧
    real::TempPartDerivValue,TempTotalDerivValue
    uValue=InuValue
    vValue=InvValue
    uLocation=JudgeValueLocInOrderList(uValue,InCubicSurf%uOriginParaList,0)
    vLocation=JudgeValueLocInOrderList(vValue,InCubicSurf%vOriginParaList,0)    
    if (vLocation .EQ. 0) then
        if (uLocation .EQ. 0) then
            call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,0,OutDuVector)
            call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,1,OutDvVector)
        else
            if (uLocation .EQ. -1) then!对v偏导=fv(u0,v)+fuv(u0,v)*(u-u0)
                uStartIndex=1                
            else
                uStartIndex=InCubicSurf%uPointNum
            end if
            uStartValue=InCubicSurf%uOriginParaList(uStartIndex)
            !call CalCubicSurfInterpPartDerive_Tan(uStartValue,vValue,InCubicSurf,0,OutDuVector)
            !!计算Dv
            vIndex=FindSplineVariantIndex(vValue,InCubicSurf%vOriginParaList)
            call CalBaseVector(vValue,&
                InCubicSurf%vOriginParaList(vIndex-1),&
                InCubicSurf%vOriginParaList(vIndex),&
                vBaseVector)
            call CalBaseVector1Derive(vValue,&
                InCubicSurf%vOriginParaList(vIndex-1),&
                InCubicSurf%vOriginParaList(vIndex),&
                DvBaseVector)

            do i=1,InCubicSurf%PointDimension
                OutDuVector(i)=dot(InCubicSurf%SurfCoef(i,uStartIndex+1,2*vIndex-3:2*vIndex),&
                    vBaseVector)
                TempPartDerivValue=dot(InCubicSurf%SurfCoef(i,uStartIndex,2*vIndex-3:2*vIndex),&
                    DvBaseVector)
                TempTotalDerivValue=dot(InCubicSurf%SurfCoef(i,uStartIndex+1,2*vIndex-3:2*vIndex),&
                    DvBaseVector)
                OutDvVector(i)=TempPartDerivValue+(uValue-uStartValue)*TempTotalDerivValue
                
            end do
        end if
    else if (uLocation .EQ. 0) then!对u偏导=fv(u0,v)+fuv(u0,v)*(u-u0)
        if (vLocation .EQ. -1) then
            vStartIndex=1
        else
            vStartIndex=InCubicSurf%vPointNum
        end if
        vStartValue=InCubicSurf%vOriginParaList(vStartIndex)
        uIndex=FindSplineVariantIndex(uValue,InCubicSurf%uOriginParaList)
        call CalBaseVector(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            uBaseVector)
        call CalBaseVector1Derive(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            DuBaseVector)        
        do i=1,InCubicSurf%PointDimension
            OutDvVector(i)=dot(uBaseVector,&
                InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,vStartIndex+1))
            TempPartDerivValue=dot(DuBaseVector,&
                InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,vStartIndex))
            TempTotalDerivValue=dot(DuBaseVector,&
                InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,vStartIndex+1))
            OutDuVector(i)=TempPartDerivValue+(vValue-vStartValue)*TempTotalDerivValue
        end do
        call CalCubicSurfInterpPartDerive_Tan(uValue,vStartValue,InCubicSurf,1,OutDvVector)


    else
        if (uLocation .EQ. -1) then
            uStartIndex=1
        else
            uStartIndex=InCubicSurf%uPointNum
        end if        
        if (vLocation .EQ. -1) then
            vStartIndex=1
        else
            vStartIndex=InCubicSurf%vPointNum
        end if
        
        do i=1,InCubicSurf%PointDimension
            OutDuVector(i)=InCubicSurf%SurfCoef(i,uStartIndex+1,vStartIndex)
            OutDvVector(i)=InCubicSurf%SurfCoef(i,uStartIndex,vStartIndex+1)
        end do
    end if
    return
    end subroutine
    
    !!计算样条曲面函数值，线性外延
    !subroutine CalCubicSurfValue_Tan(uValue,vValue,InCubicSurf,OutVector)
    !use TAN_BasicFunction 
    !implicit none
    !real::uValue,vValue,OutVector(:)
    !!real::uLocalPoint(:),vLocatPoint(:)!u向位置和切向矢量
    !type(CubicSurf_Tan)::InCubicSurf
    !integer::uLocation,vLocation
    !integer::uStartIndex,vStartIndex!当线性外延时起始标号
    !real,pointer::ExtendStartPoint(:),ExtendTangentVector(:)
    !uLocation=JudgeValueLocInOrderList(uValue,InCubicSurf%uOriginParaList,0)
    !vLocation=JudgeValueLocInOrderList(uValue,InCubicSurf%vOriginParaList,0)
    !if (uLocation .EQ. 0 .AND. vLocation .EQ. 0) then
    !    call CalCubicSurfInterpValue_Tan(InCubicSurf%uOriginParaList(1),&
    !        vValue,&
    !        InCubicSurf,&
    !        OutVector) 
    !else 
    !    allocate(ExtendStartPoint(InCubicSurf%PointDimension))
    !    allocate(ExtendTangentVector(InCubicSurf%PointDimension))
    !    if (uLocation .EQ. -1 .AND. vLocation .EQ. 0) then 
    !        
    !    call CalCubicSurfInterpValue_Tan(InCubicSurf%uOriginParaList(1),&
    !        vValue,&
    !        InCubicSurf,&
    !        ExtendStartPoint)
    !    call CalCubicSurfInterpPartDerive_Tan(InCubicSurf%uOriginParaList(1),&
    !        vValue,&
    !        InCubicSurf,&
    !        0,&
    !        ExtendTangentVector)
    !    call CalExtendVector(uValue-InCubicSurf%uOriginParaList(1),&
    !        ExtendStartPoint,&
    !        ExtendTangentVector,&
    !        OutVector)
    !    else if(uLocation .EQ. 1 .AND. vLocation .EQ. 0) then
    !        
    !        call CalCubicSurfInterpValue_Tan(InCubicSurf%uOriginParaList(InCubicSurf%PointNum),&
    !        vValue,&
    !        InCubicSurf,&
    !        ExtendStartPoint)
    !    call CalCubicSurfInterpPartDerive_Tan(InCubicSurf%uOriginParaList(InCubicSurf%PointNum),&
    !        vValue,&
    !        InCubicSurf,&
    !        0,&
    !        ExtendTangentVector)
    !    call CalExtendVector(uValue-InCubicSurf%uOriginParaList(InCubicSurf%PointNum),&
    !        ExtendStartPoint,&
    !        ExtendTangentVector,&
    !        OutVector)
    !    else
    !    end if
    !    deallocate(ExtendStartPoint)
    !    deallocate(ExtendTangentVector) 
    !end if
    !    
    !!if ((uLocation .EQ. 0) .AND.(vLocation .EQ. 0)) then
    !!    call 
    !!end if
    !!
    !!if (uLocation .LT. InCubicSurf%uOriginParaList(1)) then
    !!    uLocation=-1
    !!else if(uLocation .GT. InCubicSurf%uOriginParaList(InCubicSurf%PointNum)) then
    !!    uLocation=1
    !!else 
    !!    
    !!    uLocation=0
    !!end if
    !
    !
    !return
    !end subroutine
    subroutine FindSurfRootWithPartialDerive(IOVauleGuess,InCubicSurf,InEqu,InEquCoef)
    use TAN_BasicNumeric
    implicit none
    type(CubicSurf_Tan),target::InCubicSurf
    external::InEqu
    real::IOVauleGuess(:),InEquCoef(:)
    CubicSurfEquTransPtr=>InCubicSurf
    call SolveMultiEquByNewtonMethodWithDerive(IOVauleGuess,InEqu,InEquCoef)
    return
    end subroutine

    end module

    subroutine CalResidualSurfTestFunc(EquNum,InXGuess,EquCoef,EquValue,DEquMatrix)
    use TAN_CubicSurf
    implicit none
    integer::EquNum
    real::InXGuess(EquNum)
    real::EquCoef(EquNum),EquValue(EquNum),DEquMatrix(EquNum,EquNum)
    real::TempVector(3,2)
    TempVector=0
    call CalCubicSurfLinearExtendValue_Tan(InXGuess(1),InXGuess(2),CubicSurfEquTransPtr,TempVector(:,1))
    EquValue(1)=TempVector(1,1)-EquCoef(1)
    EquValue(2)=TempVector(2,1)-EquCoef(2)
    call CalCubicSurfLinearExtendPartDerive_Tan(InXGuess(1),InXGuess(2),CubicSurfEquTransPtr,TempVector(:,1),TempVector(:,2))
    DEquMatrix(:,1)=TempVector(1:2,1)
    DEquMatrix(:,2)=TempVector(1:2,2)
    return
    end subroutine