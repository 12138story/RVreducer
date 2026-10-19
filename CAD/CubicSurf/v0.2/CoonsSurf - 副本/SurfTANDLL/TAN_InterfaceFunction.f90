
    !生成插值曲面系数
    subroutine CalCubicSurfCoef_TanInter(&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InPointCoorTable,&
        OutSurfCoefPointer)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFCOEF_TANINTER
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::OutSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    real::InPointCoorTable(InPointDimension,InuPointCount,InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,OutSurfCoefPointer,TempCubicSurf)
    call BuildCubicSurfObject_Tan(InuParaPointer,InvParaPointer,InPointCoorTable,TempCubicSurf)
    return
    end subroutine
    !生成获得插值曲面值
    subroutine CalCubicSurfInterpValue_TanInter(&
        InUValue,&
        InVValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        OutPointVector)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFINTERPVALUE_TANINTER
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real::InUValue,InVValue
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    real::OutPointVector(InPointDimension)
    type(CubicSurf_Tan)::TempCubicSurf
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfInterpValue_Tan(InUValue,InVValue,TempCubicSurf,OutPointVector)
    end subroutine
    !计算插值曲面一阶偏导，未单位化
    subroutine CalCubicSurfInterpPartDerive_TanInter(&
        InUValue,&
        InVValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        InDeriveNotation,&!求偏导标识符号，0-对u求偏导，其它对v求偏导
        OutDeriveVector)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFINTERPPARTDERIVE_TANINTER    
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount,InDeriveNotation
    real::InUValue,InVValue
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    real::OutDeriveVector(InPointDimension)
    type(CubicSurf_Tan)::TempCubicSurf
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfInterpPartDerive_Tan(InUValue,InVValue,TempCubicSurf,InDeriveNotation,OutDeriveVector)
    end subroutine
    
    !计算插值曲面二阶偏导，未单位化
    subroutine CalCubicSurfInterp2PartDerive_TanInter(&
        InUValue,&
        InVValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        InDeriveNotation,&!求偏导标识符号，0-对uu求偏导，1-对uv求偏导，2-对vv求偏导
        OutDeriveVector)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFINTERP2PARTDERIVE_TANINTER  
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount,InDeriveNotation
    real::InUValue,InVValue
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    real::OutDeriveVector(InPointDimension)    
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfInterp2PartDerive_Tan(InUValue,InVValue,TempCubicSurf,InDeriveNotation,OutDeriveVector)
    end subroutine
    !计算曲面法向
    subroutine CalCubicSurfNormalVector_TanInterp(&
        InUValue,&
        InVValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        OutNormalVector)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFNORMALVECTOR_TANINTERP
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer::InPointDimension,InuPointCount,InvPointCount,InDeriveNotation
    real::InUValue,InVValue
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    real::OutNormalVector(InPointDimension) 
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfNormalVector_Tan(InUValue,InVValue,TempCubicSurf,OutNormalVector)
    end subroutine
    
    !计算主曲率和对应方向
    subroutine CalCubicSurfPrincipalCurvatureAndDirect_TanInter(&
        InUValue,&
        InVValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        OutDataList)!8元素数值，前4位为最大曲率和方向，后4位为最小曲率和方向
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFPRINCIPALCURVATUREANDDIRECT_TANINTER 
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real::InUValue,InVValue
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    real::OutDataList(8)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfPrincipalCurvatureAndDirect_Tan(InUValue,InVValue,TempCubicSurf,OutDataList)
    end subroutine
    


    
    
    
    !计算指定方向曲率和挠率
    subroutine CalCubicSurfCurvatureAndTorsionByVector_TanInter(&
        InUValue,&
        InVValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        InMetricVector,&
        OutCurvature,&
        OutTorsion)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFCURVATUREANDTORSIONBYVECTOR_TANINTER
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real::InUValue,InVValue
    real::InMetricVector(InPointDimension),InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    real::OutCurvature,OutTorsion
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfCurvatureAndTorsionByVector_Tan(InUValue,InVValue,TempCubicSurf,InMetricVector,OutCurvature,OutTorsion)
    end subroutine

    !计算插值曲面带外延的值
    subroutine CalCubicSurfValueWithLinearExtend_TanInter(&
        InuValue,&
        InvValue,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        OutVector)
    !DEC$ ATTRIBUTES DLLEXPORT::CALCUBICSURFVALUEWITHLINEAREXTEND_TANINTER 
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real::InUValue,InVValue
    real::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    real::OutVector(InPointDimension)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    call CalCubicSurfValueWithLinearExtend_Tan(InuValue,InvValue,TempCubicSurf,OutVector) 
    end subroutine
    !点到曲面距离
    real function FindPointToCubicSurfMinL2DistancePower_TanInter(&
        InPoint,&
        InPointDimension,&
        InuPointCount,&
        InvPointCount,&    
        InuParaPointer,&
        InvParaPointer,&
        InSurfCoefPointer,&
        IOUVValue)
    !DEC$ ATTRIBUTES DLLEXPORT::FINDPOINTTOCUBICSURFMINL2DISTANCEPOWER_TANINTER 
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real::InPoint(InPointDimension),InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::TempCubicSurf
    real::IOUVValue(2)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount,InvPointCount,InuParaPointer,InvParaPointer,InSurfCoefPointer,TempCubicSurf)
    FindPointToCubicSurfMinL2DistancePower_TanInter=FindPointToCubicSurfMinL2DistancePower_Tan(InPoint,TempCubicSurf,IOUVValue)
    end function
    
    
    !曲面到曲面距离
    real function FindSurfToSurfMinL2DirectedDistance_TanInter(&
        InReviseMetricDirectionIndex,&!=1不反向，-1反向
        InPointDimension,&
        InuPointCount1,&!被测面参数
        InvPointCount1,&    
        InuParaPointer1,&
        InvParaPointer1,&
        InSurfCoefPointer1,&
        InuPointCount2,&!基准面参数
        InvPointCount2,&    
        InuParaPointer2,&
        InvParaPointer2,&
        InSurfCoefPointer2,&
        IOUVValue)
    !DEC$ ATTRIBUTES DLLEXPORT::FINDSURFTOSURFMINL2DIRECTEDDISTANCE_TANINTER 
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    integer ::InReviseMetricDirectionIndex
    integer ::InPointDimension,InuPointCount1,InvPointCount1,InuPointCount2,InvPointCount2
    real::InuParaPointer1(InuPointCount1),InvParaPointer1(InvPointCount1),InuParaPointer2(InuPointCount2),InvParaPointer2(InvPointCount2)
    real::InSurfCoefPointer1(InPointDimension,2*InuPointCount1,2*InvPointCount1)
    real::InSurfCoefPointer2(InPointDimension,2*InuPointCount2,2*InvPointCount2)
    type(CubicSurf_Tan)::TempToCubicSurf,TempFromCubicSurf
    real::IOUVValue(4)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount1,InvPointCount1,InuParaPointer1,InvParaPointer1,InSurfCoefPointer1,TempToCubicSurf)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount2,InvPointCount2,InuParaPointer2,InvParaPointer2,InSurfCoefPointer2,TempFromCubicSurf)
    FindSurfToSurfMinL2DirectedDistance_TanInter=FindSurfToSurfMinL2DirectedDistance_Tan(TempToCubicSurf,TempFromCubicSurf,InReviseMetricDirectionIndex,IOUVValue)
    end function
    !旋转曲面到旋转曲面距离
    real function FindRotSurfToRotSurfMinL2DirectedDistance_TanInter(&
        InPinRotParaList,&
        InGearRotParaList,&
        InReviseMetricDirectionIndex,&!=1不反向，-1反向
        InPointDimension,&
        InuPointCount1,&!被测面参数
        InvPointCount1,&    
        InuParaPointer1,&
        InvParaPointer1,&
        InSurfCoefPointer1,&
        InuPointCount2,&!基准面参数
        InvPointCount2,&    
        InuParaPointer2,&
        InvParaPointer2,&
        InSurfCoefPointer2,&
        IOUVPointValue)
    !DEC$ ATTRIBUTES DLLEXPORT::FINDROTSURFTOROTSURFMINL2DIRECTEDDISTANCE_TANINTER 
    use TAN_ExportAssistFunction
    use TAN_CubicSurf
    implicit none
    real::InPinRotParaList(7),InGearRotParaList(7)
    integer ::InReviseMetricDirectionIndex
    integer ::InPointDimension,InuPointCount1,InvPointCount1,InuPointCount2,InvPointCount2
    real::InuParaPointer1(InuPointCount1),InvParaPointer1(InvPointCount1),InuParaPointer2(InuPointCount2),InvParaPointer2(InvPointCount2)
    real::InSurfCoefPointer1(InPointDimension,2*InuPointCount1,2*InvPointCount1)
    real::InSurfCoefPointer2(InPointDimension,2*InuPointCount2,2*InvPointCount2)
    type(CubicSurf_Tan)::TempToCubicSurf,TempFromCubicSurf
    real::IOUVPointValue(10)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount1,InvPointCount1,InuParaPointer1,InvParaPointer1,InSurfCoefPointer1,TempToCubicSurf)
    call ConvertBasicDataToCubicSurf(InPointDimension,InuPointCount2,InvPointCount2,InuParaPointer2,InvParaPointer2,InSurfCoefPointer2,TempFromCubicSurf)
    FindRotSurfToRotSurfMinL2DirectedDistance_TanInter=FindRotSurfToRotSurfMinL2DirectedDistance_Tan(InPinRotParaList,&
        InGearRotParaList,&
        TempToCubicSurf,&
        TempFromCubicSurf,&
        InReviseMetricDirectionIndex,&
        IOUVPointValue)
  
    end function