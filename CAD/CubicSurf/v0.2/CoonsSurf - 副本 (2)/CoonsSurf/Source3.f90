!测试锥面和线函数
    use TAN_GeometryCone
    use TAN_GeometryLine
    use TAN_CubicSurf
    use TAN_ImportData
    use TAN_BasicFunction
    implicit none
    real::TestVector2(3,3)
    real::TestPinionRotOriginAndAxis(6),TestGearRotOriginAndAxis(6)
    type(CubicSurf_Tan)::TestPinionConvexSurf,TestGearConcaveSurf
    integer::TempUVCount
    real,pointer::pTestPinionConvexUV(:,:),pTestPinionConvexContactPoint(:,:),pTestGearConcaveUV(:,:),pTestGearConcaveContactPoint(:,:)
    integer::i

    write(6,*) FCalConeTan_Tan(53d0,10d0)
    TestVector2=reshape([1:9],(/ 3, 3 /))
    write(6,*) FCalConeTanByZAxis_Tan(TestVector2(:,1))
    !call CalLinePointByOriginAndDirect_Tan(1.3,TestVector2(:,1),TestVector2(:,2),TestVector2(:,3))
    !write(6,*) TestVector2(:,3)
    write(6,*) FConeTanOfODLineByZAxis_Tan(1.3,TestVector2(:,1),TestVector2(:,2),TestVector2(1,3))
    !call CalLinePointByTwoPoint_Tan(1.3,TestVector2(:,1),TestVector2(:,2),TestVector2(:,3))
    !write(6,*) TestVector2(:,3)
    !非主要工作面
    call ImportSurfDataAndAxis('D:\WorkSpace\softdevelop\CAE\LTCA\Interface\TestData\PINIONMODEL_CONVEX_LTCA.DAT',&
        TestPinionRotOriginAndAxis,TestPinionConvexSurf)
    call ImportSurfDataAndAxis('D:\WorkSpace\softdevelop\CAE\LTCA\Interface\TestData\GEARMODEL_CONCAVE_LTCA.DAT',&
        TestGearRotOriginAndAxis,TestGearConcaveSurf)
    call ExportUGSurfPoint(TestPinionConvexSurf%SurfCoef(:,::2,::2),'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\Pin.dat')
    call ExportUGSurfPoint(TestGearConcaveSurf%SurfCoef(:,::2,::2),'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\Gear.dat')
    open (UNIT=503,FILE='D:\WorkSpace\softdevelop\CAE\LTCA\Interface\TestData\UV_CP_PV_Coons.DAT',FORM='FORMATTED')
    TempUVCount=CalDatFileRowNumByIndex(503)
    allocate(pTestPinionConvexUV(2,TempUVCount))    
    allocate(pTestPinionConvexContactPoint(3,TempUVCount))
    do i=1,TempUVCount
        read(503,*) pTestPinionConvexUV(:,i)
        call CalCubicSurfInterpValue_Tan(pTestPinionConvexUV(1,i),&
            pTestPinionConvexUV(2,i),&
            TestPinionConvexSurf,&
            pTestPinionConvexContactPoint(:,i))
    end do    
    close(503)
    call ExportUGCurvePoint(pTestPinionConvexContactPoint,'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\PinCPLine.dat')
    open (UNIT=503,FILE='D:\WorkSpace\softdevelop\CAE\LTCA\Interface\TestData\UV_CP_GC_Coons.DAT',FORM='FORMATTED')
    TempUVCount=CalDatFileRowNumByIndex(503)
    allocate(pTestGearConcaveUV(2,TempUVCount)) 
    allocate(pTestGearConcaveContactPoint(3,TempUVCount))
    do i=1,TempUVCount
        read (503,*) pTestGearConcaveUV(:,i)
        call CalCubicSurfInterpValue_Tan(pTestGearConcaveUV(1,i),&
            pTestGearConcaveUV(2,i),&
            TestGearConcaveSurf,&
            pTestGearConcaveContactPoint(:,i))
    end do    
    close(503)
    call ExportUGCurvePoint(pTestGearConcaveContactPoint,'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\GearCPLine.dat')

    end