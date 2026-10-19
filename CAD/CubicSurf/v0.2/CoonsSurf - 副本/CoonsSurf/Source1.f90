   

    use TAN_BasicFunction
    use TAN_BasicNumeric
    use Tan_CubicSpline
    use TAN_CubicSurf
    use TAN_DifferentialGeometry
    use blas95
    use TAN_Vector
    use iso_c_binding
    IMPLICIT NONE
    INTEGER,PARAMETER:: TestDimension=3,&
        ColPointNum=41,RowPointNum=21,TotalPointNum=ColPointNum*RowPointNum,&
        NDATA=ColPointNum
    
    INTEGER:: i,j,TheoryEndNum
    REAL:: T!函数
    !REAL(8) X!单变量
    real:: XDATA(NDATA),FDATA(TestDimension,NDATA)
    
    !character(len=10) :: ci
    Real::x,F1,F2,F3,F4
    real::lroot,rootValue,DrootValue
    real::OutVector(TestDimension)
    type(Spline_Tan) TestSpline
    type(CubicSurf_Tan),pointer:: testcubicpointer
    type(c_ptr)::TestPtr1,TestPtr2
    !曲线测试参数
    

    external:: CalResidualTestFunc

    !曲面测试参数
    INTEGER:: uCount,vCount
    real,pointer::uList(:),vList(:),ValueTable(:,:,:),SurfCoef(:,:)
    !real::SurfCoef(2*uCount,2*vCount)
    real:: TestVector(TestDimension)
    real ::u,v
    real FUVTest1,FUVTest2,FUVTest3
    real:: utemp,vtemp
    integer::RandomSeed(4)=1
    real,external::dlaran
    real,pointer::TestCubicSurf(:,:,:)

    !曲面方程求解
    real::IOVauleGuess(2)
    real::InEquCoef(2)
    external::CalResidualSurfTestFunc
    !用2次曲面，生成测试COONS曲面测试点，曲面理论方程为{u,v,0.3*u^2+0.2*v*u^2+0.5*v^2}
    !曲率测试参数
    real::TestDVector(3,7)
    real::TestFundamentalCoef(2,3)
    real::TestCurvature(6,2)
    real::TestOtherCurvature(3,10),SurfTestCurvature(4,2)
    real::TempValue(10)
    real,pointer::bCoordsX(:),bCoordsY(:),bCoordsZ(:)
    real,pointer::TestCoords(:)
    real::Tu,Tv
    integer::ExportFileIndex
    type(CubicSurf_Tan),target::TestGearConvexSurf,TestPinionCaveSurf
    integer,target::TestReviseIndex
    real::TTOutPoint(3),time1(4)
    real::TestUV(2),TestGrad2Maxtrix(2,2)
    real,target::TestGearUV(2),TestSurfsUV(4),TestPoint222(3)
    real::TestGrid(3)
    real,target::TestGearRotPara(7)=[-0.12319971191,0.0,0.0,1.0,0.0,0.0,0.0]
    real,target::TestPinRotPara(7)=[-0.16534698177,1.0,0.0,0.0,0.0,0.0,0.0]
    !real,target::TestGearRotPara(7)=[-0.2958840726372658,0.0,0.0,1.0,0.0,0.0,0.0]
    !real,target::TestPinRotPara(7)=[-0.22034087382633916,1.0,0.0,0.0,0.0,0.0,0.0]
    FUVTest1(u,v)=0.3*u +0.2*u**2
    FUVTest2(u,v)=0.2*u**2*v+ 0.5*v**2
    FUVTest3(u,v)=0.3*u**2 +0.2*u**2*v+ 0.5*v**2
    
    write(*,*) '生成测试COONS曲面测试点'
    uCount=31
    vCount=17
    !allocate(uList(uCount))
    !allocate(vList(vCount))
    allocate(ValueTable(3,uCount,vCount))
    allocate(SurfCoef(2*uCount,2*vCount))

    allocate(TestCoords(uCount*vCount*3))
    
    !!生成-10到10的等差数列作为u变量列表值FFF
    !call BuildArithmeticList(0.0d0,1.0d0,uList)
    !!生成-5到5的等差数列作为v变量列表值
    !call BuildArithmeticList(0.0d0,1.0d0,vList)
    Open(UNIT=12,File ='D:\WorkSpace\softdevelop\CAD\CubicSurf\v0.2\CoonsSurf\x64\Debug\GearConvexPoint0.dat',status='unknown')
    do i=1,uCount*vCount
        !read(12,*)bCoordsX(i),bCoordsY(i),bCoordsZ(i)
        read(12,*) TestCoords((3*i-2):3*i)
    end do  
    ValueTable=reshape(TestCoords,(/3,uCount,vCount/))
    call CreatCubicSurfSpace(3,uCount,vCount,TestGearConvexSurf)    
    call BuildCubicSurfObject_Tan(uList,vList,ValueTable,TestGearConvexSurf)
    Close(12)
    
    Open(UNIT=12,File ='D:\WorkSpace\softdevelop\CAD\CubicSurf\v0.2\CoonsSurf\x64\Debug\PinionCavePoint0.dat',status='unknown')
    do i=1,uCount*vCount
        !read(12,*)bCoordsX(i),bCoordsY(i),bCoordsZ(i)
        read(12,*) TestCoords((3*i-2):3*i)
    end do
  
    ValueTable=reshape(TestCoords,(/3,uCount,vCount/))
    call CreatCubicSurfSpace(3,uCount,vCount,TestPinionCaveSurf)    
    call BuildCubicSurfObject_Tan(uList,vList,ValueTable,TestPinionCaveSurf)
    Close(12)

    Tu=0.5d0
    Tv=0.3d0
    call CalCubicSurfInterpValue_Tan(Tu,Tv,TestPinionCaveSurf,TTOutPoint)
    !write(6,*)TTOutPoint
    TestUV=[0.1d0,0.5d0]
    write(6,*) associated(CommonToSurfPtrForDistance)
    CommonToSurfPtrForDistance=>TestPinionCaveSurf
    CommonFromSurfPtrForDistance=>TestGearConvexSurf
    
    CommonUVOnFromSurfPtrForDistance=>TestGearUV
    CommonReviseMetricIndexForDistance=>TestReviseIndex
        write(6,*) associated(CommonToSurfPtrForDistance)
    TestReviseIndex=-1

    !write(6,*)FindPointToCubicSurfMinL2DistancePower_Tan(TTOutPoint,TestGearConvexSurf,TestUV)
    TestSurfsUV=0.5d0
    !write(6,*)AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan(TestSurfsUV(1:2))
    !write(6,*)TestGearUV
   
    !write(6,*) 'aaaa'
    !call CalCubicSurfInterpValue_Tan(TestSurfsUV(1), TestSurfsUV(2),TestPinionCaveSurf,TestPoint222)
    !CommonPointPtrForDistance=>TestPoint222
    !call CalGradient2ForScalarFunction_Tan(TestSurfsUV(3:4),AuxiliaryGetPointToCubicSurfPointForL2DistancePower_Tan,TestGrad2Maxtrix)
    !write(6,*) FindPointToCubicSurfMinL2DistancePower_Tan(TestPoint222,TestGearConvexSurf,TestSurfsUV(3:4))
    !call PrintMatrixByDeviceNum(TestGrad2Maxtrix,6)
    !write(6,*) (AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan(TestSurfsUV(1:2)+[0.0002d0,0d0])&
    !    +AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan(TestSurfsUV(1:2)+[-0.0002d0,0d0])&
    !    -AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan(TestSurfsUV(1:2))*2d0)*25d6
    !write(6,*) FPartialDerive2ScalarFunctionByCentralDiff_Tan(TestSurfsUV(1:2),AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,1,1)
    TestUV=[0.5d0,0.5d0]
        write(6,*) size(CommonRotParaToSurfPtr)
    CommonRotParaFromSurfPtr=>TestGearRotPara
    CommonRotParaToSurfPtr=>TestPinRotPara
    write(6,*)FindPointToCubicSurfMinL2DistancePower_Tan([92.8088, -8.56923, -69.3979],TestGearConvexSurf,TestUV)
    write(6,*) TestUV
    write(6,*) 'test'
    !write(6,*) FPartialDeriveScalarFunctionByCentralDiff_Tan(TestSurfsUV(1:2),AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,1)
    !write(6,*) FPartialDerive2ScalarFunctionByCentralDiff_Tan(TestSurfsUV(1:2),AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,1,1)
    !call CalGradient2ForScalarFunction_Tan(TestSurfsUV(1:2),AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,TestGrad2Maxtrix)
    !call CalGradientForScalarFunction_Tan(TestSurfsUV(1:2),AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,TestGrid(1:2))
    !write(6,*)TestGrid
    !call PrintMatrixByDeviceNum(TestGrad2Maxtrix,6)
    call CPU_TIME(time1(1)) 
write(6,*) "旋转曲面最短距离"
    write(6,*)FindRotSurfToRotSurfMinL2DirectedDistance_Tan(TestPinRotPara,TestGearRotPara,TestPinionCaveSurf,TestGearConvexSurf,TestReviseIndex,TestSurfsUV)
    write(6,*)TestSurfsUV
    !write(6,*) CommonRotParaToSurfPtr
    !write(6,*) FindSurfToSurfMinL2DirectedDistance_Tan(TestPinionCaveSurf,TestGearConvexSurf,TestReviseIndex,TestSurfsUV)
    call CPU_TIME(time1(2))
    TestPtr1=c_loc(TestGearConvexSurf)
    call c_f_pointer(TestPtr1, testcubicpointer)
    Tu=0.5d0
    Tv=0.5d0
    call CalCubicSurfInterpValue_Tan(Tu, Tv,TestGearConvexSurf,TestDVector(:,1))
    write(6,*)TestDVector(:,1)
    call CalCubicSurfInterpValue_Tan(Tu, Tv,testcubicpointer,TestDVector(:,1))
    write(6,*)TestDVector(:,1)
    !call c_f_pointer(testcubicpointer, TestPtr1)
    
    
    !write(6,*) time1(2)-time1(1)
    !call RotVectorByAxis(0.610865,[5.0, -4.2, 3.1],[0.57735, 0.57735, 0.57735],TestGrid)
    !write(6,*)TestGrid
    !TestGrid=[1.1,-0.8,9.1]
    !call RotVectorByAxisAndOrigin(0.610865,[5.0, -4.2, 3.1],[0.57735, 0.57735, 0.57735],TestGrid,TestGrid)
    !    write(6,*)TestGrid
    !allocate(uList(5))
    !uList=[0d0,0.1d0,0.757d0,0.8d0,1d0]
    !write(6,*) FindFirstLargerValueIndex(0.78d0,uList)
    !根据生成的测试COONS曲面点云，构建coons曲面对象，曲面对象为TestSurf

    
    
    
    !do i=1,19
    !    do j=1,11
    !        !生成测试点，测试点数据为三个维度的数组
    !        !第一个维度标记坐标轴，一般长度为3，1对应x轴，2对应y轴，3对应z轴
    !        !第二个维度为u参数数目
    !        !第三个维度为v参数数目
    !        ValueTable(1,i,j)=bCoordsX(j+(i-1)*11)
    !        ValueTable(2,i,j)=bCoordsY(j+(i-1)*11)        
    !        ValueTable(3,i,j)=bCoordsZ(j+(i-1)*11)
    !    end do 
    !end do
    !
    
   !do i=1,uCount
   !     do j=1,vCount
   !         !生成测试点，测试点数据为三个维度的数组
   !         !第一个维度标记坐标轴，一般长度为3，1对应x轴，2对应y轴，3对应z轴
   !         !第二个维度为u参数数目
   !         !第三个维度为v参数数目
   !         ValueTable(1,i,j)=bCoordsX(j+(i-1)*11)
   !         ValueTable(2,i,j)=bCoordsY(j+(i-1)*11)        
   !         ValueTable(3,i,j)=bCoordsZ(j+(i-1)*11)
   !     end do 
   ! end do

    !write(6,*)ValueTable([1,2,3],[1,2],1)
    !
    !
    !call ExportUGSurfPoint(ValueTable,'D:\WorkSpace\softdevelop\CAE\BEM\BevelTest\FirstCADModel\ToothData\G1.dat')
    !call ExportUGCurvePoint(ValueTable(:,1,:),'D:\WorkSpace\softdevelop\CAE\BEM\BevelTest\FirstCADModel\ToothData\C1.dat')
    !!生成测试COONS曲面
    !!开辟COONS曲面存储空间，其中TestSurf为Spline_Tan自定义对象实例，在前面已经进行声明
    !
    !Tu=0.5d0
    !Tv=0.3d0
    !!内插值f测试，理论值为1
    !write(*,*) '对应u=-1，v=1的坐标值'
    !call CalCubicSurfInterpValue_Tan(Tu, Tv,TestSurf,TestDVector(:,1))
    !write(*,*) TestDVector(:,1)
    !!一阶偏导数fu测试，亦即u向切线，理论值为-1
    !write(*,*) '对应u=-1，v=1的1阶偏导fu'
    !call CalCubicSurfInterpPartDerive_Tan(Tu, Tv,TestSurf,&
    !    0,&!求偏导的自变量标识，0对应fu，其它对应fv
    !    TestDVector(:,2))
    !write(*,*) TestDVector(:,2)
    !!一阶偏导数fu测试，亦即v向切线，理论值为1.2
    !write(*,*) '对应u=-1，v=1的1阶偏导fv'
    !call CalCubicSurfInterpPartDerive_Tan(Tu, Tv,TestSurf,&
    !    1,&!求偏导的自变量标识，0对应fu，其它对应fv
    !    TestDVector(:,3))
    !write(*,*) TestDVector(:,3) 
    !!二阶偏导数fuv测试，理论值为-0.4
    !write(*,*) '对应u=-1，v=1的2阶偏导fuu'
    !call CalCubicSurfInterp2PartDerive_Tan(Tu, Tv,TestSurf,0,TestDVector(:,4))
    !write(*,*) TestDVector(:,4)
    !write(*,*) '对应u=-1，v=1的2阶偏导fuv'
    !call CalCubicSurfInterp2PartDerive_Tan(Tu, Tv,TestSurf,1,TestDVector(:,5))
    !write(*,*) TestDVector(:,5)
    !write(*,*) '对应u=-1，v=1的2阶偏导fvv'
    !call CalCubicSurfInterp2PartDerive_Tan(Tu, Tv,TestSurf,2,TestDVector(:,6))
    !write(*,*) TestDVector(:,6)
    !!法向测试，对应-1，1坐标的单位法线，理论值为{0.539163866017192, -0.6469966392206304, 0.539163866017192
    !write(*,*) '对应u=-1，v=1的单位法线'
    !call CalCubicSurfNormalVector_Tan(Tu, Tv,TestSurf,TestDVector(:,7))
    !write(*,*) TestDVector(:,7)
    !
    !!曲率验证
    !Tu=0.5d0
    !Tv=0.3d0
    !
    !!内插值f测试，理论值为1
    !Open(UNIT=13,File ='D:\WorkSpace\softdevelop\CAD\CubicSurf\TestData\result.dat',status='unknown')
    !ExportFileIndex=6
    !write(ExportFileIndex,*) '对应u=-1，v=1的坐标值'
    !call CalCubicSurfInterpValue_Tan(Tu,Tv,TestSurf,TestDVector(:,1))
    !write(ExportFileIndex,*) TestDVector(:,1)
    !write(ExportFileIndex,*) '样条曲面函数对应u=-1，v=1的最大主曲率'
    !call CalCubicSurfPrincipalCurvatureAndDirect_Tan(Tu,Tv,TestSurf,SurfTestCurvature)
    !write(ExportFileIndex,*) SurfTestCurvature(:,1)
    !write(ExportFileIndex,*) '样条曲面函数对应u=-1，v=1的最小主曲率'
    !write(ExportFileIndex,*) SurfTestCurvature(:,2)
    !write(ExportFileIndex,*) '样条曲面函数对应u=-1，v=1,du:dv=6:0的曲率'    
    !call CalCubicSurfCurvatureAndTorsionByVector_Tan(Tu,Tv,TestSurf,(TestDVector(:,2)*6.0),TempValue(1),TempValue(2))
    !write(ExportFileIndex,*) TempValue(1)   
    !
    !write(ExportFileIndex,*) '样条曲面函数对应u=-1，v=1,du:dv=0:1的曲率'    
    !call CalCubicSurfCurvatureAndTorsionByVector_Tan(Tu,Tv,TestSurf,(TestDVector(:,3)*1.0),TempValue(1),TempValue(2))
    !write(ExportFileIndex,*) TempValue(1)   
    !close(13)
    !!write(*,*) '样条曲面函数对应u=-1，v=1,du:dv=1:0的挠率'
    !!call CalCubicSurfCurvatureAndTorsionByVector_Tan(-1.0d0,1.0d0,TestSurf,(TestDVector(:,2)*6.0),TempValue(3),TempValue(4))
    !!write(*,*) TempValue(4)
    !!
    !!
    !!write(*,*) '对应u=-1，v=1的,验算最大曲率'
    !!write(*,*) CalSurfCurvatureByVector(TestDVector(:,7),&
    !!    TestDVector(:,2),&
    !!    TestDVector(:,3),&
    !!    TestDVector(:,4),&
    !!    TestDVector(:,5),&
    !!    TestDVector(:,6),&
    !!    TestCurvature(4:6,1))
    !!write(*,*) CalSurfCurvatureByFundamentalForm(TestFundamentalCoef,&
    !!    TestCurvature(2,1),&
    !!    1.0d0)
    !!write(*,*) '对应u=-1，v=1的,验算最小曲率'
    !!write(*,*) CalSurfCurvatureByVector(TestDVector(:,7),&
    !!    TestDVector(:,2),&
    !!    TestDVector(:,3),&
    !!    TestDVector(:,4),&
    !!    TestDVector(:,5),&
    !!    TestDVector(:,6),&
    !!    TestCurvature(4:6,2))
    !!write(*,*) CalSurfCurvatureByFundamentalForm(TestFundamentalCoef,&
    !!    TestCurvature(2,2),&
    !!    1.0d0)
    !


    end
    subroutine testfun(InValueList,OutVector)
    implicit none
    real::InValueList(:),OutVector(:)
    OutVector(1)=sin(InValueList(1)+InValueList(2))
    OutVector(2)=Cos(InValueList(1)-InValueList(2))
    end subroutine