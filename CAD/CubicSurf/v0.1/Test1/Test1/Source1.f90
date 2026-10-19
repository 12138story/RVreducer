    !
    !use TAN_CubicSpline
    !use TAN_BasicNumeric
    !implicit none
    !integer,parameter::N=5,N1=101
    !real::TestXdata(N),TestFdata(N)
    !real::xStart=-10000.0,xEnd=10000.0
    !real::TestCscoef(2*N),TestEndDeri(2)
    !real::TestXVariat(N1),TestFCriteVariat(N1),TestFVariat(N1)
    !integer:: tempIndex
    !integer::i
    !do i=1,N
    !    TestXdata(i)=xStart+(xEnd-xStart)/real((N-1))*(i-1)
    !    TestFdata(i)=TestXdata(i)**2
    !end do
    !tempIndex =FindFirstLowerValueIndex(TestXdata(N),TestXdata)
    !write(*,*) tempIndex
    !write(*,*) TestXdata(tempIndex)
    !call Cal1DCubicSplineCoef(TestXdata,TestFdata,TestCscoef,TestEndDeri(1),TestEndDeri(2))    
    !!write(*,*) TestCscoef
    !!write(*,*) TestEndDeri
    !do i=1,N1
    !   TestXVariat=xStart+(xEnd-xStart)/real((N1-1))*(i-1)
    !   TestFCriteVariat(i)=TestXVariat(i)**2
    !   TestFVariat(i)=abs(Cal1DCubicSplineValue(TestXVariat(i),TestXdata,TestCscoef)-TestFCriteVariat(i))
    !end do
    !write(*,*) maxval(TestFVariat)
    

    use TAN_BasicFunction
    use TAN_BasicNumeric
    use Tan_CubicSpline
    use TAN_CubicSurf
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
    !曲线测试参数

    external:: CalResidualTestFunc

    !曲面测试参数
    INTEGER:: uCount,vCount
    real,pointer::uList(:),vList(:),ValueTable(:,:,:),SurfCoef(:,:)
    !real::SurfCoef(2*uCount,2*vCount)
    real:: TestVector(TestDimension)
    real ::u,v
    real FUVTest1
    real:: utemp,vtemp
    integer::RandomSeed(4)=1
    real,external::dlaran
    real,pointer::TestCubicSurf(:,:,:)
    type(CubicSurf_Tan),target::TestSurf
    !曲面方程求解
    real::IOVauleGuess(2)
    real::InEquCoef(2)
    external::CalResidualSurfTestFunc
    
    F1(x)=x*x
    F2(x)=x*x
    F3(x)=-2*x
    F4(x)=3*x
    !FUVTest1(u,v)=2*u**2 + 6*u*v + 2*v**2
    FUVTest1(u,v)=0.3*u**2 +0.2*u**2*v+ 0.5*v**2

    write(*,*) '曲线测试'
    !write(*,*) [1:5:2]
    !call JudgeCondition(.true.,'Point count between uList and value table are not compatible')

    DO I=1, NDATA
        XDATA(I) = REAL(I-1)/REAL(NDATA-1)+1
        FDATA(1,I) = F1(XDATA(I))
        FDATA(2,I) = F2(XDATA(I))
        FDATA(3,I) = F3(XDATA(I))
    END DO
    !call CreatCubicSplineSpace(NDATA,TestDimension,TestSpline)
    !TestSpline%OriginPointPara=xData
    !call CalCubicSplineCoef_Tan(xData,FDATA,TestSpline)
    !do i=1,2*NDATA
    !    !write(*,*) TestSpline%Cscoef(:,i)
    !end do
    !call CalCubicSplineVector_Tan(1.0,TestSpline,TestVector)
    !write(*,*) TestVector
    !call CalCubicSplineVector1Derive_Tan(1.0,TestSpline,TestVector)
    !write(*,*) TestVector
    !    call CalCubicSplineVector_Tan(2.0,TestSpline,TestVector)
    !write(*,*) TestVector
    !call CalCubicSplineVector1Derive_Tan(2.0,TestSpline,TestVector)
    !write(*,*) TestVector
    !lroot=FindCurveRootWithDerive_Tan(1.7,3.0,TestSpline,CalResidualTestFunc)
    !call CalCubicSplineVector_Tan(lroot,TestSpline,OutVector)
    !call CalResidualTestFunc(lroot,rootValue,DrootValue)
    !write(*,*) lroot
    !write(*,*) rootValue
    !write(*,*) OutVector
    !!call DeleteCubicSplineSpace(TestSpline)
    uCount=100
    vCount=200
    allocate(uList(uCount))
    allocate(vList(vCount))
    allocate(ValueTable(3,uCount,vCount))
    allocate(SurfCoef(2*uCount,2*vCount))
    !write(*,*) [real(-500):real(500):(500+500)/real(uCount)]
    call BuildArithmeticList(-10.0,10.0,uList)
    call BuildArithmeticList(-5.0,5.0,vList)
    do i=1,uCount
        do j=1,vCount
            ValueTable(1,i,j)=uList(i)
            ValueTable(2,i,j)=vList(j)            
            ValueTable(3,i,j)=FUVTest1(uList(i),vList(j))
        end do 
    end do    
    !call Cal1DCubicSurfCoef(uList,vList,ValueTable,SurfCoef)
    !do i=1,2*uCount
    !    write(*,*) SurfCoef(i,:)
    !end do
    !do i=1,10
    !    utemp=5*dlaran(RandomSeed)
    !    vtemp=5*dlaran(RandomSeed)
    !    write(*,*) (Cal1DCubicSurfValue(utemp,vtemp,uList,vList,SurfCoef)-FUVTest1(utemp,vtemp))/FUVTest1(utemp,vtemp)        
    !end do
    call CreatCubicSurfSpace(3,uCount,vCount,TestSurf)
    call BuildCubicSurfObject_Tan(uList,vList,ValueTable,TestSurf)
    allocate (TestCubicSurf(3,uCount,vCount))
    !call CalCubicSurfLinearExtendValue_Tan(uList(6)-1.0,vList(1)-1.0,TestSurf,TestCubicSurf(:,6,1))
    do i=1,uCount
        do j=1,vCount
        TestCubicSurf(1,i,j)=1.0*uList(i)
        TestCubicSurf(2,i,j)=1.5*vList(j)
        !call CalCubicSurfLinearExtendValue_Tan(uList(i)-1.0,vList(j)-1.0,TestSurf,TestCubicSurf(:,i,j))
        call CalCubicSurfLinearExtendValue_Tan(TestCubicSurf(1,i,j),TestCubicSurf(2,i,j),TestSurf,TestCubicSurf(:,i,j))
        end do
    end do
    
    call CalCubicSurfInterpTotalDerive_Tan(-1.0,1.0,TestSurf,ValueTable(:,1,1))
    write(*,*) ValueTable(:,1,1)
    !call CalCubicSurfInterpTotalDerive_Tan(-1.0,1.0,TestSurf,ValueTable(:,1,1))
    
    !call CalCubicSurfLinearExtendPartDerive_Tan(-13.0,-6.0,TestSurf,ValueTable(:,1,1),ValueTable(:,2,1))
    write(*,*) TestSurf%uOriginParaList(1)
    write(*,*) TestSurf%vOriginParaList(1)
    write(*,*) ValueTable(:,1,1)
    write(*,*) ValueTable(:,2,1)
    IOVauleGuess=(/1.0,1.0/)
    InEquCoef=(/1.0,-1.0/)
    CubicSurfEquTransPtr=>TestSurf
    !call CalResidualSurfTestFunc(IOVauleGuess,InEquCoef,uList,ValueTable(1:2,1:2,1))
    call SolveMultiEquByNewtonMethodWithDerive(IOVauleGuess,CalResidualSurfTestFunc,InEquCoef)
    !call FindSurfRootWithPartialDerive(IOVauleGuess,TestSurf,CalResidualSurfTestFunc,InEquCoef)
    !TestCubicSurf=ValueTable
    write(*,*)IOVauleGuess
    call ExportUGCurvePoint(TestCubicSurf(:,:,1),"F:\Workspace\softdevelop\Ages-x64-2020-05-6_Update\Ages-x64-2020-05-6_Update\Ages\AgesDLl4Jinyi\Lib\x64\CADData\FitCurve.dat")
    call ExportUGSurfPoint(TestCubicSurf,"F:\Workspace\softdevelop\Ages-x64-2020-05-6_Update\Ages-x64-2020-05-6_Update\Ages\AgesDLl4Jinyi\Lib\x64\CADData\FitSurf.dat")
    !!write(*,*) 

    end
