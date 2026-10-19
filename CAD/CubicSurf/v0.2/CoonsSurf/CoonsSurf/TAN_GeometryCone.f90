
    !锥角等基本几何参数计算
    module TAN_GeometryCone
    implicit none
    type::Cone_TAN
        real,pointer::Apex(:),Axis(:)
        real::TanAng
        integer::CreatMethod=0!如果是1，则为动态创建，否则为0
    end type    
    contains
    subroutine DeleteConeObjSpace_Tan(IOConeObj)
    implicit none
    type(Cone_TAN)::IOConeObj
    if(IOConeObj%CreatMethod==1) then
        deallocate(IOConeObj%Apex)
        deallocate(IOConeObj%Axis)
        IOConeObj%CreatMethod=0
    end if
    return
    end subroutine
    subroutine CreateConeObjSpace_Tan(IOConeObj)
    implicit none
    type(Cone_TAN)::IOConeObj
    call DeleteConeObjSpace_Tan(IOConeObj)
    allocate(IOConeObj%Apex(3))
    allocate(IOConeObj%Axis(3))
    IOConeObj%CreatMethod=1
    IOConeObj%Apex=0d0
    IOConeObj%Axis=0d0
    IOConeObj%Axis(3)=1d0
    return
    end subroutine
    !计算锥角正切值
    real function FCalConeTan_Tan(InRadius,InAxisDist) result(ans)
    implicit none
    real::InRadius,InAxisDist
    ans=InRadius/InAxisDist
    return
    end function
    real function FCalConeTanByZAxis_Tan(InVector) result(ans)
    use blas95
    implicit none
    real::InVector(:)
    ans=nrm2(InVector(1:2))/InVector(3)
    return
    end function
    !通过锥面两点坐标计算Z轴的锥角和锥顶
    subroutine CalConeApexAndTanByTwoPoint_Tan(InPoint1,InPoint2,OutApex,OutTan)
    use TAN_NumericAlgebra
    implicit none
    real::InPoint1(:),InPoint2(:),OutApex,OutTan
    real,pointer::pTempPoint(:,:)
    allocate(pTempPoint(2,2))
    pTempPoint(1,1)=InPoint1(3)
    pTempPoint(2,1)=FCalVectorNorm_TAN(InPoint1(1:2)) 
    pTempPoint(1,2)=InPoint2(3)
    pTempPoint(2,2)=FCalVectorNorm_TAN(InPoint2(1:2)) 
    OutApex=pTempPoint(1,1)+(pTempPoint(2,1)*(pTempPoint(1,2)-pTempPoint(1,1)))/(pTempPoint(2,1)-pTempPoint(2,2))
    OutTan=(pTempPoint(2,2)-pTempPoint(2,1))/(pTempPoint(1,2)-pTempPoint(1,1))
    return
    deallocate(pTempPoint)
    end subroutine
    !点与任意顶点和方向的锥面角函数
    real function FConeTanOfPoint_TAN(InPoint,InConeApexPoint,InConeAxis) result(ans)
    use TAN_NumericAlgebra
    implicit none
    real::InPoint(:),InConeApexPoint(:),InConeAxis(:)
    real,pointer::pTempPoint(:,:)
    real::TempCos,TempSin
    allocate(pTempPoint(3,2))
    pTempPoint(:,1)=InPoint
    pTempPoint(:,1)=pTempPoint(:,1)-InConeApexPoint(1:3)
    TempCos=FCalDotProduct_Tan(pTempPoint(:,1),InConeAxis(1:3))
    call CalVectorCrossProduct_Tan(pTempPoint(:,1),InConeAxis,pTempPoint(:,2))
    TempSin=FCalVectorNorm_TAN(pTempPoint(:,2))
    ans=abs(TempSin/TempCos)
    deallocate(pTempPoint)    
    return
    end function
    !点是否在任意顶点和方向的锥面内部
    logical function IsPointInConeInner_Tan(InPoint,InConeApexPoint,InConeAxis,InConeTan) result(ans)
    implicit none
    real::InPoint(:),InConeApexPoint(:),InConeAxis(:),InConeTan
    real::TempConeAngTan
    TempConeAngTan=FConeTanOfPoint_TAN(InPoint,InConeApexPoint,InConeAxis)
    if(TempConeAngTan>InConeTan) then
        ans=.false.
    else
        ans=.true.
    end if    
    return
    end function
    end module
    
    