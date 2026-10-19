
 module TAN_ExportAssistFunction
    contains
    !将输入的基本数据转换为CubicSurf_Tan对象，以便调用内部函数
    subroutine ConvertBasicDataToCubicSurf(&
    InPointDimension,&
    InuPointCount,&
    InvPointCount,&    
    InuParaPointer,&
    InvParaPointer,&
    InSurfCoefPointer,&
    OutCubicSurf)
    use TAN_CubicSurf
    implicit none
    integer ::InPointDimension,InuPointCount,InvPointCount
    real,target::InuParaPointer(InuPointCount),InvParaPointer(InvPointCount)
    real,target::InSurfCoefPointer(InPointDimension,2*InuPointCount,2*InvPointCount)
    type(CubicSurf_Tan)::OutCubicSurf
    OutCubicSurf%PointDimension=InPointDimension
    OutCubicSurf%uPointNum=InuPointCount
    OutCubicSurf%vPointNum=InvPointCount   
    OutCubicSurf%uOriginParaList=>InuParaPointer
    OutCubicSurf%vOriginParaList=>InvParaPointer
    OutCubicSurf%SurfCoef=>InSurfCoefPointer            
    return
    end subroutine
    !将输入的基本数据转换为Spline_Tan对象，以便调用内部函数
    subroutine ConvertBasicDataToCubicSpline(&
        InPointDimension,&
        InPointCount,&   
        InVarList,&
        InSurfCoefPointer,&
        OutCubicSpline)
    use TAN_CubicSpline
    implicit none
    integer ::InPointDimension,InPointCount
    real,target::InVarList(InPointCount)
    real,target::InSurfCoefPointer(InPointDimension,2*InPointCount)
    type(Spline_Tan)::OutCubicSpline
    OutCubicSpline%PointDimension=InPointDimension
    OutCubicSpline%PointNum=InPointCount  
    OutCubicSpline%OriginPointPara=>InVarList
    OutCubicSpline%Cscoef=>InSurfCoefPointer            
    return
    end subroutine

end module