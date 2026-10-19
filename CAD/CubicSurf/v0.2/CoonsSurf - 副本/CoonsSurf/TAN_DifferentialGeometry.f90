    module TAN_DifferentialGeometry
    
    contains
    !计算第一基本型系数
    subroutine CalFirstFundamentalFormCoef(InDSDu,InDSDv,OutFundamentalCoef)
    use blas95
    implicit none
    real::InDSDu(:),InDSDv(:)
    real::OutFundamentalCoef(3)!对应E,F,G
    OutFundamentalCoef(1)=dot(InDSDu,InDSDu)
    OutFundamentalCoef(2)=dot(InDSDu,InDSDv)
    OutFundamentalCoef(3)=dot(InDSDv,InDSDv)
    return
    end subroutine
    !计算第二基本型系数
    subroutine CalSecondFundamentalFormCoef(InNormalVector,InDSDuu,InDSDuv,InDSDvv,OutFundamentalCoef)
    use blas95
    implicit none
    real::InDSDuu(:),InDSDuv(:),InDSDvv(:),InNormalVector(:)
    real::OutFundamentalCoef(3)!对应L,M,N
    OutFundamentalCoef(1)=dot(InNormalVector,InDSDuu)
    OutFundamentalCoef(2)=dot(InNormalVector,InDSDuv)
    OutFundamentalCoef(3)=dot(InNormalVector,InDSDvv)
    return
    end subroutine   
    !计算Du与Dv值
    subroutine CalDuDvValue(InNormalVector,InDSDu,InDSDv,InMetricDirection,OutDu,OutDv)
    use TAN_Vector
    implicit none
    real::InNormalVector(:),InDSDu(:),InDSDv(:),InMetricDirection(:)
    real::OutDu,OutDv!对应Dv/Du值
    real::CommonDenominator
    CommonDenominator=CalVectorMixedProduct(InNormalVector,InDSDv,InDSDu)
    OutDu=CalVectorMixedProduct(InNormalVector,InDSDv,InMetricDirection)/CommonDenominator
    OutDv=-CalVectorMixedProduct(InNormalVector,InDSDu,InMetricDirection)/CommonDenominator
    return
    end subroutine
    !计算DvDu方向基本型值
    real function CalFundamentalFormValueByDvDu(InFundamentalCoef,InDu,InDv)
    use blas95
    implicit none
    real::InFundamentalCoef(:),InDu,InDv
    CalFundamentalFormValueByDvDu=InFundamentalCoef(1)*InDu**2+2*InFundamentalCoef(2)*InDu*InDv+InFundamentalCoef(3)*(InDv**2)
    return
    end function
    
    !根据基本型计算曲面法曲率
    real function CalSurfCurvatureByFundamentalForm(InFundamentalCoef,InDu,InDv)
    implicit none
    real::InFundamentalCoef(2,3)!对应{E,F,G;L,M,N}
    real::InDu,InDv!对应Dv/Du值
    CalSurfCurvatureByFundamentalForm=CalFundamentalFormValueByDvDu(InFundamentalCoef(2,:),InDu,InDv)&
        /CalFundamentalFormValueByDvDu(InFundamentalCoef(1,:),InDu,InDv)
    return
    end function
    !根据曲面矢量计算度量方向DvDu值
    real function CalDvDuBySurfVector(InNormalVector,InDSDu,InDSDv,InMetricDirection)
    use TAN_Vector
    implicit none
    real::InNormalVector(:),InDSDu(:),InDSDv(:),InMetricDirection(:)
    CalDvDuBySurfVector=-CalVectorMixedProduct(InNormalVector,InDSDu,InMetricDirection)&
        /CalVectorMixedProduct(InNormalVector,InDSDv,InMetricDirection)    
    return    
    end function        
    !根据曲面任意方向切矢量计算曲率
    real function CalSurfCurvatureByVector(InNormalVector,InDSDu,InDSDv,InDSDuu,InDSDuv,InDSDvv,InMetricDirection)
    !use TAN_Vector
    implicit none
    real::InNormalVector(:),InDSDu(:),InDSDv(:),InDSDuu(:),InDSDuv(:),InDSDvv(:),InMetricDirection(:)
    real::FundamentalCoef(2,3)!对应{E,F,G;L,M,N}
    real::Du,Dv!对应Dv/Du值
    call CalFirstFundamentalFormCoef(InDSDu,InDSDv,FundamentalCoef(1,:))
    call CalSecondFundamentalFormCoef(InNormalVector,InDSDuu,InDSDuv,InDSDvv,FundamentalCoef(2,:))  
    !DvDu=CalDvDuBySurfVector(InNormalVector,InDSDu,InDSDv,InMetricDirection)
    call CalDuDvValue(InNormalVector,InDSDu,InDSDv,InMetricDirection,Du,Dv)
    !CommonDenominator=CalVectorMixedProduct(InNormalVector,InDSDv,InDSDu)
    !Du=CalVectorMixedProduct(InNormalVector,InDSDv,InMetricDirection)/CommonDenominator
    !Dv=-CalVectorMixedProduct(InNormalVector,InDSDu,InMetricDirection)/CommonDenominator
    CalSurfCurvatureByVector=CalSurfCurvatureByFundamentalForm(FundamentalCoef,Du,Dv)
    return
    end function
    !根据曲面dudv值计算测地挠率
    real function CalSurfTorsionByFundamentalForm(InFundamentalCoef,InDu,InDv)
    implicit none
    real::InFundamentalCoef(2,3)!对应{E,F,G;L,M,N}
    real::InDu,InDv!对应Dv/Du值
    real::CommonDenominator
    CalSurfTorsionByFundamentalForm=((-(InFundamentalCoef(1,2)*InFundamentalCoef(2,1)) + InFundamentalCoef(1,1)*InFundamentalCoef(2,2))*InDu**2 + &
        (-(InFundamentalCoef(1,3)*InFundamentalCoef(2,1)) + InFundamentalCoef(1,1)*InFundamentalCoef(2,3))*InDu*InDv + &
        (-(InFundamentalCoef(1,3)*InFundamentalCoef(2,2)) + InFundamentalCoef(1,2)*InFundamentalCoef(2,3))*InDv**2)&
        /(sqrt(-(InFundamentalCoef(1,2))**2+InFundamentalCoef(1,1)*InFundamentalCoef(1,3))*CalFundamentalFormValueByDvDu(InFundamentalCoef(1,:),InDu,InDv))
    return
    end function
    !根据曲面任意方向切矢量计算测地挠率
    real function CalSurfTorsionByVector(InNormalVector,InDSDu,InDSDv,InDSDuu,InDSDuv,InDSDvv,InMetricDirection)
    implicit none
    real::InNormalVector(:),InDSDu(:),InDSDv(:),InDSDuu(:),InDSDuv(:),InDSDvv(:),InMetricDirection(:)
    real::FundamentalCoef(2,3)!对应{E,F,G;L,M,N}
    real::Du,Dv!对应Dv/Du值
    call CalFirstFundamentalFormCoef(InDSDu,InDSDv,FundamentalCoef(1,:))
    call CalSecondFundamentalFormCoef(InNormalVector,InDSDuu,InDSDuv,InDSDvv,FundamentalCoef(2,:))  
    call CalDuDvValue(InNormalVector,InDSDu,InDSDv,InMetricDirection,Du,Dv)
    CalSurfTorsionByVector=CalSurfTorsionByFundamentalForm(FundamentalCoef,Du,Dv)
    !CalSurfTorsionByFundamentalForm=1.0d0
    return
    end function
    
    !计算主曲率和主方向
    subroutine CalPrincipalCurvatureAndDvDu(InFundamentalCoef,OutCurvature)
    implicit none
    real::InFundamentalCoef(2,3)
    real::OutCurvature(3,2)!两方向的曲率。第一个维度，1-曲率，2-DvDu或DuDv,3-DvDu标识符号，0对应DvDu,1对应DuDv,2对应脐点无主方向；第二个维度：1-最大值，2-最小值。
    real::TempCurvature(3,2),CommonPara,DvDuDenominator
    !real::EE,FF,GG,LL,MM,NN
    !temp=InFundamentalCoef(1,3)*InFundamentalCoef(2,1) - InFundamentalCoef(1,1)*InFundamentalCoef(2,3)
    !EE=InFundamentalCoef(1,1)
    !FF=InFundamentalCoef(1,2)
    !GG=InFundamentalCoef(1,3)
    !LL=InFundamentalCoef(2,1)
    !MM=InFundamentalCoef(2,2)
    !NN=InFundamentalCoef(2,3)
    
    !CommonPara=sqrt((GG*LL - EE*NN)**2 + 4*(FF*LL - EE*MM)*(-(GG*MM) + FF*NN))
    
    CommonPara=sqrt((InFundamentalCoef(1,3)*InFundamentalCoef(2,1) - InFundamentalCoef(1,1)*InFundamentalCoef(2,3))**2 + &
        4*(InFundamentalCoef(1,2)*InFundamentalCoef(2,1) - InFundamentalCoef(1,1)*InFundamentalCoef(2,2))*(-(InFundamentalCoef(1,3)*InFundamentalCoef(2,2)) +&
        InFundamentalCoef(1,2)*InFundamentalCoef(2,3)))
    !主曲率计算
    TempCurvature(1,1)=(-(InFundamentalCoef(1,3)*InFundamentalCoef(2,1)) + 2*InFundamentalCoef(1,2)*InFundamentalCoef(2,2) - InFundamentalCoef(1,1)*InFundamentalCoef(2,3) - CommonPara)/(2*(InFundamentalCoef(1,2)**2 - InFundamentalCoef(1,1)*InFundamentalCoef(1,3)))
    TempCurvature(1,2)=(-(InFundamentalCoef(1,3)*InFundamentalCoef(2,1)) + 2*InFundamentalCoef(1,2)*InFundamentalCoef(2,2) - InFundamentalCoef(1,1)*InFundamentalCoef(2,3) + CommonPara)/(2*(InFundamentalCoef(1,2)**2 - InFundamentalCoef(1,1)*InFundamentalCoef(1,3)))
    !主曲率对应的DvDu或DuDv
    
    !GG*MM - FF*NN是否等于0
    if (abs(InFundamentalCoef(1,3)*InFundamentalCoef(2,2) - InFundamentalCoef(1,2)*InFundamentalCoef(2,3)) .GT. 0.000001) then
        TempCurvature(2,1)=(-(InFundamentalCoef(1,3)*InFundamentalCoef(2,1)) + InFundamentalCoef(1,1)*InFundamentalCoef(2,3) + CommonPara)/(2*InFundamentalCoef(1,3)*InFundamentalCoef(2,2) - 2*InFundamentalCoef(1,2)*InFundamentalCoef(2,3))
        TempCurvature(2,2)=(-(InFundamentalCoef(1,3)*InFundamentalCoef(2,1)) + InFundamentalCoef(1,1)*InFundamentalCoef(2,3) - CommonPara)/(2*InFundamentalCoef(1,3)*InFundamentalCoef(2,2) - 2*InFundamentalCoef(1,2)*InFundamentalCoef(2,3))
        TempCurvature(3,1)=0
        TempCurvature(3,2)=0
    !FF*LL - EE*MM是否等于0
    else if (abs(InFundamentalCoef(1,2)*InFundamentalCoef(2,1) - InFundamentalCoef(1,1)*InFundamentalCoef(2,2)) .GT. 0.000001) then
        TempCurvature(2,1)=(InFundamentalCoef(1,3)*InFundamentalCoef(2,1) - InFundamentalCoef(1,1)*InFundamentalCoef(2,3) + CommonPara)/(2.*(-(InFundamentalCoef(1,2)*InFundamentalCoef(2,1)) + InFundamentalCoef(1,1)*InFundamentalCoef(2,2)))
        TempCurvature(2,2)=(-(InFundamentalCoef(1,3)*InFundamentalCoef(2,1)) + InFundamentalCoef(1,1)*InFundamentalCoef(2,3) + CommonPara)/(2*InFundamentalCoef(1,2)*InFundamentalCoef(2,1) - 2*InFundamentalCoef(1,1)*InFundamentalCoef(2,2))
        TempCurvature(3,1)=1d0
        TempCurvature(3,2)=1d0
    else
        TempCurvature(3,1)=2d0
        TempCurvature(3,2)=2d0
    end if
    OutCurvature(3,1)=TempCurvature(3,1)
    OutCurvature(3,2)=TempCurvature(3,2)
    if (TempCurvature(1,1) .LT. TempCurvature(1,2)) then
        OutCurvature(1,1)=TempCurvature(1,2)
        OutCurvature(1,2)=TempCurvature(1,1)
        OutCurvature(2,1)=TempCurvature(2,2)
        OutCurvature(2,2)=TempCurvature(2,1)
    else
        OutCurvature(1,1)=TempCurvature(1,1)        
        OutCurvature(1,2)=TempCurvature(1,2)
        OutCurvature(2,1)=TempCurvature(2,1)
        OutCurvature(2,2)=TempCurvature(2,2)        
    end if
    return
    end subroutine
    !计算基本型主曲率等基本几何量
    subroutine CalSurfGeomParas(InNormalVector,InDSDu,InDSDv,InDSDuu,InDSDuv,InDSDvv,OutFundamentalCoef,OutCurvature)
    use TAN_Vector
    implicit none
    real::InNormalVector(:),InDSDu(:),InDSDv(:),InDSDuu(:),InDSDuv(:),InDSDvv(:)
    real::OutFundamentalCoef(2,3)!对应{E,F,G;L,M,N}
    real::OutCurvature(6,2)!两方向的曲率。第一个维度，1-曲率，2-DvDv，3-DvDu标识符号(0对应DvDu,1对应DuDv),4：6-主方向对应单位切矢量;第二个维度：1-最大值，2-最小值。
    real::TempVector(3)
    call CalFirstFundamentalFormCoef(InDSDu,InDSDv,OutFundamentalCoef(1,:))
    call CalSecondFundamentalFormCoef(InNormalVector,InDSDuu,InDSDuv,InDSDvv,OutFundamentalCoef(2,:))  
    call CalPrincipalCurvatureAndDvDu(OutFundamentalCoef,OutCurvature(1:3,1:2))
    if (abs(OutCurvature(3,1)) .LT. 0.1) then
        call CalVectorNormalize(InDSDu(1:3)+OutCurvature(2,1)*InDSDv(1:3),OutCurvature(4:6,1))
        call CalVectorNormalize(InDSDu(1:3)+OutCurvature(2,2)*InDSDv(1:3),OutCurvature(4:6,2))
    else if (abs(OutCurvature(3,1)) .LT. 1.1) then
        call CalVectorNormalize(OutCurvature(2,1)*InDSDu(1:3)+InDSDv(1:3),OutCurvature(4:6,1))
        call CalVectorNormalize(OutCurvature(2,2)*InDSDu(1:3)+InDSDv(1:3),OutCurvature(4:6,2))
    else
        call CalVectorNormalize(InDSDu,OutCurvature(4:6,1))
        call CalVectorCrossProduct(InNormalVector,InDSDu,TempVector)
        call CalVectorNormalize(TempVector,OutCurvature(4:6,2))
    end if    
    return
    end subroutine
    
    end module
    