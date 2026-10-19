
    module TAN_GeometryLine
    implicit none
    contains
    !计算直线点坐标
    subroutine CalLinePointByOriginAndDirect_Tan(InVar,InOrigin,InDirect,OutPoint)
    use blas95
    implicit none
    real::InVar,InOrigin(:),InDirect(:),OutPoint(:)
    real,pointer::pTempDirect(:)
    allocate(pTempDirect(size(InDirect,1)))
    call copy(InDirect,pTempDirect)
    call copy(InOrigin,OutPoint)
    call axpy(pTempDirect,OutPoint,InVar)
    deallocate(pTempDirect)
    return
    end subroutine
    subroutine CalLinePointByTwoPoint_Tan(InVar,InPoint1,InPoint2,OutPoint)
    use blas95
    implicit none
    real::InVar,InPoint1(:),InPoint2(:),OutPoint(:)
    real,pointer::pTempPoint(:)
    allocate(pTempPoint(size(InPoint1,1)))
    pTempPoint=InPoint2-InPoint1
    call copy(InPoint1,OutPoint)
    call axpy(pTempPoint,OutPoint,InVar)    
    deallocate(pTempPoint)    
    return
    end subroutine
    !计算锥角
    real function FConeTanOfODLineByZAxis_Tan(InVar,InOrigin,InDirect,InConeApexZ) result(ans)
    use TAN_GeometryCone, only: FCalConeTanByZAxis_Tan
    implicit none
    real::InVar,InOrigin(:),InDirect(:),InConeApexZ
    real,pointer::pTempPoint(:)
    allocate(pTempPoint(size(InOrigin,1)))
    call CalLinePointByOriginAndDirect_Tan(InVar,InOrigin,InDirect,pTempPoint)
    pTempPoint(3)=pTempPoint(3)-InConeApexZ
    ans=FCalConeTanByZAxis_Tan(pTempPoint)
    deallocate(pTempPoint)   
    return
    end function
    !求割线上下限表达式
    !直线对应x坐标的y坐标值
    real function FIntersectLineLimitByX_Tan(InX,InX0,InY0,InX1,InY1) result(ans)
    implicit none
    real::InX,InX0,InY0,InX1,InY1
    ans=InY0 + ((InX - InX0)*(InY0 - InY1))/(InX0 - InX1)
    end function
    !线上点与任意顶点和方向的锥面角函数
    real function FConeTanOfLinePoint_TAN(InVar,InOrigin,InDirect,InConeApexPoint,InConeAxis) result(ans)
    use TAN_NumericAlgebra
    use TAN_GeometryCone,only:FConeTanOfPoint_TAN
    implicit none
    real::InVar,InOrigin(:),InDirect(:),InConeApexPoint(:),InConeAxis(:)
    real,pointer::pTempPoint(:)
    !real::TempCos,TempSin
    allocate(pTempPoint(3))
    call CalLinePointByOriginAndDirect_Tan(InVar,InOrigin(1:3),InDirect(1:3),pTempPoint(:))
    ans=FConeTanOfPoint_TAN(pTempPoint,InConeApexPoint,InConeAxis)
    !
    !call CalLinePointByOriginAndDirect_Tan(InVar,InOrigin(1:3),InDirect(1:3),pTempPoint(:,1))
    !pTempPoint(:,1)=pTempPoint(:,1)-InConeApexPoint(1:3)
    !TempCos=FCalDotProduct_Tan(pTempPoint(:,1),InConeAxis(1:3))
    !call CalVectorCrossProduct_Tan(pTempPoint(:,1),InConeAxis,pTempPoint(:,2))
    !TempSin=FCalVectorNorm_TAN(pTempPoint(:,2))
    !ans=abs(TempSin/TempCos)
    deallocate(pTempPoint)    
    return
    end function
    end module
    