
    !¹ùÈðç÷Ö÷ÇúÂÊ²âÊÔ
    use TAN_CubicSurf
    use TAN_CubicSpline
    use TAN_BasicFunction
    use TAN_NumericDiff
    use TAN_ImportData
    type(CubicSurf_Tan),target::TestSurf
    real::TestCurvature(4,2)
    real::TestPointVector(3)
    real,pointer::TestD2Vector(:)
    real::TestUV(2)
    integer::TempValue
    !¹ùÈðç÷µã²âÊÔ
    real,pointer::TestLinePoint(:,:)
    type(Spline_Tan)::TstSpline
    integer::TestLinePointCount
    real::TestCalLinePoint(3)
    TestLinePointCount=CalDatFileRowNumByPath('D:\WorkSpace\softdevelop\CAD\CubicSurf\TestData\ToothProfileL.dat')
    allocate(TestLinePoint(3,TestLinePointCount))
    call ImportMatrixData_Tan('D:\WorkSpace\softdevelop\CAD\CubicSurf\TestData\ToothProfileL.dat',TestLinePoint,1)
    call CreatCubicSplineSpace(TestLinePointCount,3,TstSpline)
    call BuildArithmeticList_Tan(0d0,1d0,TstSpline%OriginPointPara)
    call BuildCubicSplineObject_Tan(TstSpline%OriginPointPara,TestLinePoint,TstSpline)
    call CalCubicSplineVectorWithExpand_Tan(1.006,TstSpline,TestCalLinePoint)
    write(6,*) TestCalLinePoint

    call ImportSurfDataAndAxis('D:\WorkSpace\softdevelop\UG CAD TEST MODEL\Principal Curvature\GuoRuiQi\Para.dat',[0.0],TestSurf,1)
    
    !call Import
    TestUV=[0.5, 0.5]
    call CreatCubicSplineSpace(4,3,TstSpline)
    !TestLinePoint
    call CalCubicSurfInterpValue_Tan(TestUV(1),TestUV(2),TestSurf,TestPointVector)
    write(6,*)'×ø±êµã'
    write(6,*)TestPointVector
    !call BuildCubicSplineObject_Tan(TstSpline)
    call CalCubicSurfPrincipalCurvatureAndDirect_Tan(TestUV(1),TestUV(2),TestSurf,TestCurvature)
    call ExportMatrixToFile_Tan(TestSurf%SurfCoef(1,::2,::2),'D:\WorkSpace\softdevelop\UG CAD TEST MODEL\Principal Curvature\GuoRuiQi\XCoor.dat')
    call ExportMatrixToFile_Tan(TestSurf%SurfCoef(2,::2,::2),'D:\WorkSpace\softdevelop\UG CAD TEST MODEL\Principal Curvature\GuoRuiQi\YCoor.dat')
    call ExportMatrixToFile_Tan(TestSurf%SurfCoef(3,::2,::2),'D:\WorkSpace\softdevelop\UG CAD TEST MODEL\Principal Curvature\GuoRuiQi\ZCoor.dat')
    allocate(TestD2Vector(3))
    write(6,*)'Ö÷ÇúÂÊ°ë¾¶'
    write(6,*)1/TestCurvature(1,:)
    !call FPartialDeriveVectorFunctionByCentralDiff_Tan(TestUV,TestDiff2Function,1,TestD2Vector)
    TestSurfPtr=>TestSurf
    call FPartialDerive2VectorFunctionByCentralDiff_Tan(TestUV,TestDiff2Function,[2,2],TestD2Vector)
    write(6,*)'2½×µ¼'
    write(6,*)TestD2Vector

    end

    