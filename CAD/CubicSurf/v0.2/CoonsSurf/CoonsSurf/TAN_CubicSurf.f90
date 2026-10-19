
    module TAN_CubicSurf
    use iso_c_binding, only: c_ptr, c_f_pointer,c_loc
    implicit none
    type::CubicSurf_Tan
        integer::uPointNum,vPointNum,PointDimension
        real,pointer::uOriginParaList(:)
        real,pointer::vOriginParaList(:)
        real,pointer::SurfCoef(:,:,:)!系数,参考p101页4.16进行排序，行对应u向，列对应v项
        integer::CreatMethod=0!如果是1，则为动态创建，否则为0
    end type CubicSurf_Tan
    real,target::DefaultRotParaList(7)=0d0    
    type(CubicSurf_Tan),pointer::TestSurfPtr
    !type(CubicSurf_Tan),pointer:: CubicSurfEquTransPtr
    !type(CubicSurf_Tan),pointer:: CommonFromSurfPtrForDistance
    !type(CubicSurf_Tan),pointer:: CommonToSurfPtrForDistance
    !real,pointer::CommonPointPtrForDistance(:)
    !real,pointer::CommonUVOnFromSurfPtrForDistance(:)
    !integer,pointer::CommonReviseMetricIndexForDistance!度量方向反向标识，1不反向，-1反向(最初对于大齿轮凸面，反向才指向轮齿外侧)
    !real,pointer::CommonRotParaFromSurfPtr(:)!7维数组，1-转角，2-4转动矢量，5-7转动原点
    !real,pointer::CommonRotParaToSurfPtr(:)!7维数组，1-转角，2-4转动矢量，5-7转动原点
    contains
    
    
    subroutine CreatCubicSurfSpace(PointDimension,uPointNum,vPointNum,CubicSurfName)
    use TAN_BasicFunction
    implicit none
    integer ::PointDimension,uPointNum,vPointNum
    type(CubicSurf_Tan)::CubicSurfName    
    allocate (CubicSurfName%uOriginParaList(uPointNum))
    allocate (CubicSurfName%vOriginParaList(vPointNum))
    allocate (CubicSurfName%SurfCoef(PointDimension,2*uPointNum,2*vPointNum))
    if(CubicSurfName%CreatMethod .EQ. 0) then
        call DeleteCubicSurfSpace(CubicSurfName)        
    end if
    call BuildArithmeticList_Tan(real(0),real(1),CubicSurfName%uOriginParaList)
    call BuildArithmeticList_Tan(real(0),real(1),CubicSurfName%vOriginParaList)
    !CubicSurfName%uOriginParaList=0
    !CubicSurfName%vOriginParaList=0
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
    !根据曲面边缘索引创建样条线对象
    subroutine CreatSplineObjBySurfEdge_TAN(InEdgeNotation,InSurfObj,OutSpline)
    use TAN_NumericAlgebra
    use TAN_CubicSpline
    implicit none
    integer::InEdgeNotation!边缘标识符号，10-Surf中u=0，11Surf中u=1，20-Surf中v=0，21-Surf中v=1
    type(CubicSurf_Tan)::InSurfObj
    type(Spline_Tan)::OutSpline
    select case(InEdgeNotation)
    case(10)
        call CreatCubicSplineSpace(size(InSurfObj%vOriginParaList),3,OutSpline)
        call CopyVector_TAN(InSurfObj%vOriginParaList,OutSpline%OriginPointPara)
        call CopyMatrix_TAN(InSurfObj%SurfCoef(:,1,:),OutSpline%Cscoef)
    case(11)
        call CreatCubicSplineSpace(size(InSurfObj%vOriginParaList),3,OutSpline)
        call CopyVector_TAN(InSurfObj%vOriginParaList,OutSpline%OriginPointPara)
        call CopyMatrix_TAN(InSurfObj%SurfCoef(:,2*InSurfObj%uPointNum-1,:),OutSpline%Cscoef)
    case(20)
        call CreatCubicSplineSpace(size(InSurfObj%uOriginParaList),3,OutSpline)
        call CopyVector_TAN(InSurfObj%uOriginParaList,OutSpline%OriginPointPara)
        call CopyMatrix_TAN(InSurfObj%SurfCoef(:,:,1),OutSpline%Cscoef)
    case(21)
        call CreatCubicSplineSpace(size(InSurfObj%uOriginParaList),3,OutSpline)
        call CopyVector_TAN(InSurfObj%uOriginParaList,OutSpline%OriginPointPara)
        call CopyMatrix_TAN(InSurfObj%SurfCoef(:,:,2*InSurfObj%vPointNum-1),OutSpline%Cscoef)
    end select
    
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
    call JudgeCondition_Tan(uCount .NE. size(InValueTable,1),&
        'Point count between uList and value table are not compatible')
    call JudgeCondition_Tan(vCount .NE. size(InValueTable,2),&
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
    real,pointer::puvTempBaseVector(:,:)!uvBaseVector(4),vBaseVector(4),TempVector(4)    
    integer::i
    allocate(puvTempBaseVector(4,3))
    uIndex=FindSplineVariantIndex(uValue,uList)
    vIndex=FindSplineVariantIndex(vValue,vList)
    call CalBaseVector(uValue,&
        uList(uIndex-1),&
        uList(uIndex),&
        puvTempBaseVector(:,1))
    call CalBaseVector(vValue,&
        vList(vIndex-1),&
        vList(vIndex),&
        puvTempBaseVector(:,2))
    call gemv(InCubicSurfCoef(2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
        puvTempBaseVector(:,2),puvTempBaseVector(:,3))
    Cal1DCubicSurfValue=dot(puvTempBaseVector(:,1),puvTempBaseVector(:,3))
    deallocate(puvTempBaseVector)
    return
    end function
    
    !计算样条曲面系数
    subroutine BuildCubicSurfObject_Tan(uList,vList,InPointTable,IOCubicSurf)
    use TAN_BasicFunction
    implicit none
    real::uList(:),vList(:),InPointTable(:,:,:)
    type(CubicSurf_Tan)::IOCubicSurf
    integer::i
    call JudgeCondition_Tan(size(InPointTable,1) .NE. IOCubicSurf%PointDimension,&
        'Point dimension differs from the cubic surf object;a value list from 0 to 1 is set.')
    if (size(uList)==0) then
        write(6,*) 'Warning: the size of origin u list for the cubic surface is zero;'
        write(6,*) '         a value list from 0 to 1 is set.'
        call BuildArithmeticList_Tan(0.0d0,1.0d0,IOCubicSurf%uOriginParaList)
    else        
        IOCubicSurf%uOriginParaList=uList
    end if 
    if (size(vList)==0) then
        write(6,*) 'Warning: the size of origin v list for the cubic surface is zero;'
        write(6,*) '         a value list from 0 to 1 is set.'
        call BuildArithmeticList_Tan(0.0d0,1.0d0,IOCubicSurf%vOriginParaList)
    else        
        IOCubicSurf%vOriginParaList=vList
    end if 
    !IOCubicSurf%PointDimension=size(InPointTable,1)
    
    !IOCubicSurf%vOriginParaList=vList
    do i=1,IOCubicSurf%PointDimension
        call Cal1DCubicSurfCoef(IOCubicSurf%uOriginParaList,&
            IOCubicSurf%vOriginParaList,&
            InPointTable(i,:,:),&
            IOCubicSurf%SurfCoef(i,:,:))
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
    !use TAN_CubicSpline
    implicit none
    real::uValue,vValue,OutVector(:)
    integer::uIndex,vIndex
    type(CubicSurf_Tan)::InCubicSurf
    integer::VariantSign
    real,pointer::puvTempBaseVector(:,:)!:uvBaseVector(4),vBaseVector(4),TempVector(4)    
    integer::i
    allocate(puvTempBaseVector(4,3))
    uIndex=FindSplineVariantIndex(uValue,InCubicSurf%uOriginParaList)
    vIndex=FindSplineVariantIndex(vValue,InCubicSurf%vOriginParaList)
    if (VariantSign .EQ. 0) then
        call CalBaseVector1Derive(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            puvTempBaseVector(:,1))
        call CalBaseVector(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            puvTempBaseVector(:,2))        

        do i=1,InCubicSurf%PointDimension
            call gemv(InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
            puvTempBaseVector(:,2),&
            puvTempBaseVector(:,3))            
            OutVector(i)=dot(puvTempBaseVector(:,1),puvTempBaseVector(:,3))            
        end do
    else
        call CalBaseVector(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            puvTempBaseVector(:,1))
        call CalBaseVector1Derive(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            puvTempBaseVector(:,2))
        do i=1,InCubicSurf%PointDimension
            call gemv(InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
            puvTempBaseVector(:,2),&
            puvTempBaseVector(:,3))            
            OutVector(i)=dot(puvTempBaseVector(:,1),puvTempBaseVector(:,3))            
        end do 
    end if    
    deallocate(puvTempBaseVector)
    return
    end subroutine
    
    
    
    !计算2阶偏导数
    !VariantSign=0,对uu求偏导
    !VariantSign=1,对uv求偏导
    !VariantSign=其它,对vv求偏导
    subroutine CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,VariantSign,OutVector)
    use TAN_CubicSpline
    !use f95_precision
    use blas95
    use TAN_CubicSpline
    implicit none
    real::uValue,vValue,OutVector(:)
    integer::VariantSign    
    type(CubicSurf_Tan)::InCubicSurf
    integer::uIndex,vIndex
    real,pointer::puvTempBaseVector(:,:)!:uvBaseVector(4),vBaseVector(4),TempVector(4) 
    integer::i
    allocate(puvTempBaseVector(4,3))
    uIndex=FindSplineVariantIndex(uValue,InCubicSurf%uOriginParaList)
    vIndex=FindSplineVariantIndex(vValue,InCubicSurf%vOriginParaList)
    if (VariantSign .EQ. 0) then
        call CalBaseVector2Derive(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            puvTempBaseVector(:,1))
        call CalBaseVector(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            puvTempBaseVector(:,2)) 
    else if (VariantSign .EQ. 1) then
        call CalBaseVector1Derive(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            puvTempBaseVector(:,1))
        call CalBaseVector1Derive(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            puvTempBaseVector(:,2))
    else
        call CalBaseVector(uValue,&
            InCubicSurf%uOriginParaList(uIndex-1),&
            InCubicSurf%uOriginParaList(uIndex),&
            puvTempBaseVector(:,1))
        call CalBaseVector2Derive(vValue,&
            InCubicSurf%vOriginParaList(vIndex-1),&
            InCubicSurf%vOriginParaList(vIndex),&
            puvTempBaseVector(:,2))
    end if    

    do i=1,InCubicSurf%PointDimension
        call gemv(InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vIndex-3:2*vIndex),&
            puvTempBaseVector(:,2),&
            puvTempBaseVector(:,3))            
            OutVector(i)=dot(puvTempBaseVector(:,1),puvTempBaseVector(:,3))
    end do   
    deallocate(puvTempBaseVector)
    return
    end subroutine
    !计算法矢量,维度限制    
    subroutine CalCubicSurfNormalVector_Tan(uValue,vValue,InCubicSurf,OutVector)
    use TAN_NumericAlgebra
    use TAN_BasicFunction
    implicit none
    real::uValue,vValue    
    type(CubicSurf_Tan)::InCubicSurf
    real::OutVector(:)
    integer::OutVectorDimension
    real,pointer::u1OrderDerive(:),v1OrderDerive(:)
    OutVectorDimension=size(OutVector)
    OutVector=0
    if (OutVectorDimension .LT. 3) then
        call JudgeCondition_Tan(OutVectorDimension .LT. 3,&
            'The output vector of calculating surface normal vector is less than 3')
    else        
        allocate(u1OrderDerive(InCubicSurf%PointDimension))
        allocate(v1OrderDerive(InCubicSurf%PointDimension))
        call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,0,u1OrderDerive)
        call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,1,v1OrderDerive)
        call CalVectorCrossProduct_Tan(u1OrderDerive(1:3),v1OrderDerive(1:3),OutVector(1:3))
        call CalVectorNormalize_Tan(OutVector,OutVector)
        deallocate(u1OrderDerive)
        deallocate(v1OrderDerive)
    end if
    return
    end subroutine
    
    !主曲率与主方向计算
    subroutine CalCubicSurfPrincipalCurvatureAndDirect_Tan(uValue,vValue,InCubicSurf,OutCurvature)
    use TAN_DifferentialGeometry   
    use TAN_NumericAlgebra
    implicit none
    real::uValue,vValue    
    type(CubicSurf_Tan)::InCubicSurf
    real::OutCurvature(4,2)!维度1，1对应主曲率值，2：4对应方向;维度2，1对应最大主曲率，2对应最小主曲率
    real,pointer::SurfDeriveVector(:,:)!曲面基本矢量,Su,Sv,Suu,Suv,Suv,n
    real,pointer::SurfFundamentalCoef(:,:)!曲面基本型
    real,pointer::TempCurvature(:,:)
    allocate(SurfDeriveVector(3,6))
    allocate(SurfFundamentalCoef(2,3))
    allocate(TempCurvature(6,2))
    call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,0,SurfDeriveVector(:,1))
    call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,1,SurfDeriveVector(:,2))
    call CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,0,SurfDeriveVector(:,3))
    call CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,1,SurfDeriveVector(:,4))
    call CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,2,SurfDeriveVector(:,5))
    !write(6,*) SurfDeriveVector(:,5)
    !SurfDeriveVector(:,5)=[0.00309682, 0.296921, 3.41944D-7]
    call CalVectorCrossProduct_Tan(SurfDeriveVector(:,1),SurfDeriveVector(:,2),SurfDeriveVector(:,6))
    call CalVectorNormalize_Tan(SurfDeriveVector(:,6),SurfDeriveVector(:,6))
    call CalSurfGeomParas(SurfDeriveVector(:,6),&
        SurfDeriveVector(:,1),&
        SurfDeriveVector(:,2),&
        SurfDeriveVector(:,3),&
        SurfDeriveVector(:,4),&
        SurfDeriveVector(:,5),&
        SurfFundamentalCoef,&
        TempCurvature)
    OutCurvature(1,:)=TempCurvature(1,:)
    OutCurvature(2:4,:)=TempCurvature(4:6,:)
    call CalVectorCrossProduct_Tan(OutCurvature(2:4,2),SurfDeriveVector(:,6),OutCurvature(2:4,1))
    deallocate(SurfDeriveVector)
    deallocate(SurfFundamentalCoef)
    deallocate(TempCurvature)
    return
    end subroutine
    !任意方向曲率与挠率计算
    subroutine CalCubicSurfCurvatureAndTorsionByVector_Tan(uValue,vValue,InCubicSurf,InMetricVector,OutCurvature,OutTorsion)
    use TAN_DifferentialGeometry 
    use TAN_NumericAlgebra
    implicit none
    real::uValue,vValue,InMetricVector(:) 
    type(CubicSurf_Tan)::InCubicSurf
    real::OutCurvature,OutTorsion
    real,pointer::SurfDeriveVector(:,:)!曲面基本矢量,Su,Sv,Suu,Suv,Suv,n
    real,pointer::SurfFundamentalCoef(:,:)!曲面基本型
    real::Du,Dv
    allocate(SurfDeriveVector(3,6))
    allocate(SurfFundamentalCoef(2,3))
    call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,0,SurfDeriveVector(:,1))
    call CalCubicSurfInterpPartDerive_Tan(uValue,vValue,InCubicSurf,1,SurfDeriveVector(:,2))
    call CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,0,SurfDeriveVector(:,3))
    call CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,1,SurfDeriveVector(:,4))
    call CalCubicSurfInterp2PartDerive_Tan(uValue,vValue,InCubicSurf,2,SurfDeriveVector(:,5))
    call CalVectorCrossProduct_Tan(SurfDeriveVector(:,1),SurfDeriveVector(:,2),SurfDeriveVector(:,6))
    call CalVectorNormalize_Tan(SurfDeriveVector(:,6),SurfDeriveVector(:,6))
    call CalFirstFundamentalFormCoef(SurfDeriveVector(:,1),SurfDeriveVector(:,2),SurfFundamentalCoef(1,:))
    call CalSecondFundamentalFormCoef(SurfDeriveVector(:,6),&
        SurfDeriveVector(:,3),&
        SurfDeriveVector(:,4),&
        SurfDeriveVector(:,5),&
        SurfFundamentalCoef(2,:))  
    call CalDuDvValue(SurfDeriveVector(:,6),SurfDeriveVector(:,1),SurfDeriveVector(:,2),InMetricVector,Du,Dv)
    OutCurvature=CalSurfCurvatureByFundamentalForm(SurfFundamentalCoef,Du,Dv)
    OutTorsion=CalSurfTorsionByFundamentalForm(SurfFundamentalCoef,Du,Dv)
    deallocate(SurfDeriveVector)
    deallocate(SurfFundamentalCoef)
    return
    end subroutine
    !主曲率和对应坐标系轴，列主序，所以没列元素是一根轴
    subroutine CalSurfPrincipalCurvatureAndMetricCoor_Tan(InUValue,InVValue,InCubicSurf,OutCurvature,OutMetricCoorList)
    use TAN_NumericAlgebra
    use blas95
    implicit none
    real::InUValue,InVValue,OutMetricCoorList(4,4),OutCurvature(2)
    type(CubicSurf_Tan)::InCubicSurf
    OutMetricCoorList=0d0
    call CalCubicSurfPrincipalCurvatureAndDirect_Tan(InUValue,InVValue,InCubicSurf,OutMetricCoorList(:,3:4))
    OutCurvature(1)=OutMetricCoorList(1,3)
    OutCurvature(2)=OutMetricCoorList(1,4)
    call copy(OutMetricCoorList(2:4,3),OutMetricCoorList(1:3,1))
    call copy(OutMetricCoorList(2:4,4),OutMetricCoorList(1:3,2))
    call CalVectorCrossProduct_Tan(OutMetricCoorList(1:3,1),OutMetricCoorList(1:3,2),OutMetricCoorList(1:3,3))
    call CalCubicSurfInterpValue_Tan(InUValue,InVValue,InCubicSurf,OutMetricCoorList(1:3,4))
    OutMetricCoorList(4,3)=0d0
    OutMetricCoorList(4,4)=1d0
    return
    end subroutine
    subroutine CalRotSurfPrincipalCurvatureAndMetricCoor_Tan(InUValue,InVValue,InRotAng,InRotAxisOrigin,InCubicSurf,OutCurvature,OutMetricCoorList)
    use TAN_NumericAlgebra
    implicit none
    real::InUValue,InVValue,InRotAng,InRotAxisOrigin(6),OutCurvature(2),OutMetricCoorList(4,4)
    type(CubicSurf_Tan)::InCubicSurf
    integer::i
    call CalSurfPrincipalCurvatureAndMetricCoor_Tan(InUValue,InVValue,InCubicSurf,OutCurvature,OutMetricCoorList)
    do i=1,3
        call RotVectorByAxis_Tan(InRotAng,OutMetricCoorList(1:3,i),InRotAxisOrigin(1:3),OutMetricCoorList(1:3,i))
    end do    
    call RotVectorByAxisAndOrigin_Tan(InRotAng,OutMetricCoorList(1:3,4),InRotAxisOrigin(1:3),InRotAxisOrigin(4:6),OutMetricCoorList(1:3,4))
    return
    end subroutine
    
    
    
    !计算带线性外延样条曲面值,ExtendMethod=0-u向，1-v向，其它-uv向
    subroutine CalCubicSurfValueWithLinearExtend_Tan(InuValue,InvValue,InCubicSurf,OutVector)
    use TAN_BasicFunction
    implicit none
    real::InuValue,InvValue,uValue,vValue,OutVector(:)
    type(CubicSurf_Tan)::InCubicSurf
    real::uStartValue,vStartValue
    integer::uStartIndex,vStartIndex!当线性外延时起始标号
    integer::uLocation,vLocation!当线性外延时方向标号-1-左侧，1-右侧
    real,pointer::ExtendStartPoint(:)
    real,pointer::ExtendTangentVector(:)
    uValue=InuValue
    vValue=InvValue
    uLocation=JudgeValueLocInOrderList_Tan(uValue,InCubicSurf%uOriginParaList,0)
    vLocation=JudgeValueLocInOrderList_Tan(vValue,InCubicSurf%vOriginParaList,0)
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
            call CalExtendVector_Tan(uValue-uStartValue,&
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
        call CalExtendVector_Tan(vValue-vStartValue,&
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
        call CalExtendVector_Tan(uValue-uStartValue,&
            ExtendStartPoint,&
            ExtendTangentVector,&
            OutVector)
        ExtendTangentVector=InCubicSurf%SurfCoef(:,2*uStartIndex-1,2*vStartIndex)!沿v向偏导
        call CalExtendVector_Tan(vValue-vStartValue,&
            OutVector,&
            ExtendTangentVector,&
            OutVector)
    end if
    deallocate(ExtendStartPoint)
    deallocate(ExtendTangentVector) 
    return
    end subroutine
    
    
    !计算带线性外延样条曲面偏导值,ExtendMethod=0-u向，1-v向，其它-uv向
    subroutine CalCubicSurfPartDeriveWithLinearExtend_Tan(InuValue,InvValue,InCubicSurf,OutDuVector,OutDvVector)
    use TAN_BasicFunction
    use TAN_CubicSpline
    use blas95
    use TAN_CubicSpline
    implicit none
    real::InuValue,InvValue,OutDuVector(:),OutDvVector(:),uValue,vValue
    type(CubicSurf_Tan)::InCubicSurf
    !real,pointer::UVDuDvVectorStorage(:,:)
    real,pointer::uBaseVector(:),vBaseVector(:),DuBaseVector(:),DvBaseVector(:)
    !real::TempVector(4)
    integer::i
    real::uStartValue,vStartValue
    integer::uStartIndex,vStartIndex,uIndex,vIndex,uLocation,vLocation!当线性外延时方向标号-1-左侧，1-右侧
    real::TempPartDerivValue,TempTotalDerivValue
    allocate(uBaseVector(4))
    allocate(vBaseVector(4))
    allocate(DuBaseVector(4))
    allocate(DvBaseVector(4))
    uValue=InuValue
    vValue=InvValue
    uLocation=JudgeValueLocInOrderList_Tan(uValue,InCubicSurf%uOriginParaList,0)
    vLocation=JudgeValueLocInOrderList_Tan(vValue,InCubicSurf%vOriginParaList,0)    
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
                OutDuVector(i)=dot(InCubicSurf%SurfCoef(i,2*uStartIndex,2*vIndex-3:2*vIndex),&
                    vBaseVector)
                TempPartDerivValue=dot(InCubicSurf%SurfCoef(i,2*uStartIndex-1,2*vIndex-3:2*vIndex),&
                    DvBaseVector)
                TempTotalDerivValue=dot(InCubicSurf%SurfCoef(i,2*uStartIndex,2*vIndex-3:2*vIndex),&
                    DvBaseVector)
                OutDvVector(i)=TempPartDerivValue+(uValue-uStartValue)*TempTotalDerivValue
                
            end do
        end if
    else if (uLocation .EQ. 0) then!原始曲线=f(u,v0)+fv(u,v0)*(v-v0)!对u偏导=fu(u,v0)+fuv(u,v0)*(v-v0)
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
                InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vStartIndex))            
            
            TempPartDerivValue=dot(DuBaseVector,&
                InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vStartIndex-1))
            TempTotalDerivValue=dot(DuBaseVector,&
                InCubicSurf%SurfCoef(i,2*uIndex-3:2*uIndex,2*vStartIndex))
            OutDuVector(i)=TempPartDerivValue+(vValue-vStartValue)*TempTotalDerivValue
        end do
    else
        if (uLocation .EQ. -1) then
            uStartIndex=1
        else
            uStartIndex=2*InCubicSurf%uPointNum-1
        end if        
        if (vLocation .EQ. -1) then
            vStartIndex=1
        else
            vStartIndex=2*InCubicSurf%vPointNum-1
        end if
        
        do i=1,InCubicSurf%PointDimension
            OutDuVector(i)=InCubicSurf%SurfCoef(i,uStartIndex+1,vStartIndex)
            OutDvVector(i)=InCubicSurf%SurfCoef(i,uStartIndex,vStartIndex+1)
        end do

    end if
    !    write(*,*)"OutDuVector"
    !        write(*,*)OutDuVector
    !write(*,*)"OutDvVector"
    !        write(*,*)OutDvVector
    deallocate(uBaseVector)
    deallocate(vBaseVector)
    deallocate(DuBaseVector)
    deallocate(DvBaseVector)
    return
    end subroutine
    !计算带线性外延样条曲面法向,ExtendMethod=0-u向，1-v向，其它-uv向
    !计算法矢量,维度限制    
    subroutine CalCubicSurfNormalVectoreWithLinearExtend_Tan(uValue,vValue,InCubicSurf,OutVector)
    use TAN_NumericAlgebra
    use TAN_BasicFunction
    implicit none
    real::uValue,vValue    
    type(CubicSurf_Tan)::InCubicSurf
    real::OutVector(:)
    integer::OutVectorDimension
    real,pointer::pu1v1OrderDerive(:,:)
    allocate(pu1v1OrderDerive(3,2))
    call CalCubicSurfPartDeriveWithLinearExtend_Tan(uValue,vValue,InCubicSurf,pu1v1OrderDerive(:,1),pu1v1OrderDerive(:,2))
    call CalVectorCrossProduct_Tan(pu1v1OrderDerive(:,1),pu1v1OrderDerive(:,2),OutVector(1:3))
    call CalVectorNormalize_Tan(OutVector,OutVector)
    deallocate(pu1v1OrderDerive)
    return
    end subroutine
    
    subroutine CalCubicSurfNormalVectorWithRotAndExtend_Tan(InUV,InCubicSurf,InRotParaList,OutVector)
    use TAN_NumericAlgebra
    implicit none
    real::InUV(:),InRotParaList(:),OutVector(:)
    type(CubicSurf_Tan)::InCubicSurf
    call CalCubicSurfNormalVectoreWithLinearExtend_Tan(InUV(1),InUV(2),InCubicSurf,OutVector)
    !判断是否需要转动    
    if (Abs(InRotParaList(1)).GT.0.000001) then            
            call RotVectorByAxis_Tan(InRotParaList(1),&
                OutVector,&
                InRotParaList(2:4),&
                OutVector) 
    end if   
    return
    end subroutine
    !其他函数
    !带转动的插值曲面值计算
    subroutine CalCubicSurfValueWithRotAndExtend_Tan(InUV,InCubicSurf,InRotParaList,OutVector)
    use TAN_BasicCoef
    use TAN_NumericAlgebra
    implicit none
    real::InUV(:),InRotParaList(:),OutVector(:)
    type(CubicSurf_Tan)::InCubicSurf
    call CalCubicSurfValueWithLinearExtend_Tan(InUV(1),InUV(2),InCubicSurf,OutVector)
    !判断是否需要转动    
    if (Abs(InRotParaList(1)).GT.0.000001) then            
            call RotVectorByAxisAndOrigin_Tan(InRotParaList(1),&
                OutVector,&
                InRotParaList(2:4),&
                InRotParaList(5:7),OutVector) 
    end if   
    return
    end subroutine
        

    !距离函数
    !点到曲面单点距离计算辅助函数,为了后面求解，曲面参数和点通过common参数传递,只适用于3维！！
    real function AuxiliaryGetPointToCubicSurfPointForL2DistancePower_Tan(InUV,InFuncObjPrtList)
    use TAN_NumericAlgebra
    use blas95
    use TAN_BasicCoef
    implicit none
    real::InUV(:)
    type(c_ptr),optional::InFuncObjPrtList(:)  
    type(CubicSurf_Tan),pointer:: TempFromSurfObjPtr
    real,pointer::TempMetricPoint(:),TempRotParaList(:)
    real,pointer::pTempVector(:)
    allocate(pTempVector(3))
    call c_f_pointer(InFuncObjPrtList(1), TempFromSurfObjPtr) 
    call c_f_pointer(InFuncObjPrtList(2), TempMetricPoint,[3]) 
    !默认有旋转原动
    call c_f_pointer(InFuncObjPrtList(3),TempRotParaList,[7])
    call CalCubicSurfValueWithRotAndExtend_Tan(InUV,TempFromSurfObjPtr,TempRotParaList,pTempVector)
    !if (BasicTestNotation > 0 .and.BasicTestNotation1<2) then
    !    write(6,*) InUV
    !    write(6,*) pTempVector
    !    write(6,*) TempRotParaList
    !    
    !end if
    call axpy(TempMetricPoint,pTempVector,-1d0)
    AuxiliaryGetPointToCubicSurfPointForL2DistancePower_Tan=dot(pTempVector,pTempVector)
    !if (BasicTestNotation > 0.and.BasicTestNotation1<2) then
    !    write(6,*) AuxiliaryGetPointToCubicSurfPointForL2DistancePower_Tan
    !    BasicTestNotation1=BasicTestNotation1+1
    !end if
    
    deallocate(pTempVector)
    return
    end function
    !点到曲面距离,返回值=欧几里得距离的平方，OutDistanceAndValue(1)=U值，OutDistanceAndValue(2)=V值
    real function FindPointToCubicSurfMinL2DistancePower_Tan(InPoint,InFromCubicSurf,IOUVValue,InRotParaList,InOptSurfSearchGrid,OutOptErrorNotation)
    use TAN_NumericOptimization
    implicit none
    real::InPoint(:)    
    type(CubicSurf_Tan)::InFromCubicSurf
    real::IOUVValue(:)
    real,optional::InRotParaList(:)
    real,optional::InOptSurfSearchGrid(:,:)!在齿面哪个UV范围内进行搜寻，若未定义则采用原始曲面对象的自变量范围
    Integer,optional,target::OutOptErrorNotation
    type(c_ptr),pointer::pTempFuncObjPrtList(:)  
    integer,pointer::TempErrorPtr
    integer,target::TempErrorCode
    real,pointer::pTempVarRanges(:,:)
    allocate(pTempFuncObjPrtList(3))
    allocate(pTempVarRanges(2,2))
    !对参数进行打包
    if(present(InRotParaList)) then        
        pTempFuncObjPrtList(3)=c_loc(InRotParaList)
    else
        pTempFuncObjPrtList(3)=c_loc(DefaultRotParaList)   
    end if
    if(present(OutOptErrorNotation)) then
        TempErrorPtr=>OutOptErrorNotation
    else
        TempErrorPtr=>TempErrorCode
    end if
    pTempFuncObjPrtList(1)=c_loc(InFromCubicSurf)
    pTempFuncObjPrtList(2)=c_loc(InPoint)
    if(present(InOptSurfSearchGrid)) then
        pTempVarRanges(:,1)=InOptSurfSearchGrid(:,1)
        pTempVarRanges(:,2)=InOptSurfSearchGrid(:,2)    
    else        
        pTempVarRanges(:,1)=InFromCubicSurf%uOriginParaList([1,InFromCubicSurf%uPointNum])
        pTempVarRanges(:,2)=InFromCubicSurf%vOriginParaList([1,InFromCubicSurf%vPointNum])    
    end if    
    FindPointToCubicSurfMinL2DistancePower_Tan=FindMinValueWithVarRanges_Tan(AuxiliaryGetPointToCubicSurfPointForL2DistancePower_Tan,&
        IOUVValue,&
        [1,2],&
        pTempVarRanges,&
        pTempFuncObjPrtList,&
        OutOptErrorNotation=TempErrorPtr)
    !if (TempErrorPtr>0) then
    !    write(6,*) TempErrorPtr
    !end if
    !write(6,*) FindPointToCubicSurfMinL2DistancePower_Tan
    deallocate(pTempFuncObjPrtList)
    deallocate(pTempVarRanges)
    return
    end function
    
    
    
    !点到曲面的“有向”距离,可选转动
    !目前更改的版本去掉了有向部分?用于网格曲面法搜寻
    real function FindPointToCubicSurfMinL2DirectedDistance_Tan(InPoint,&
        InFromCubicSurf,&
        InReviseIndex,&
        IOUVValue,&
        InOptPointRotParaList,&
        InOptSurfRotParaList,&
        InOptSurfSearchGrid,&
        OutOptErrorNotation) result(ans)
    use TAN_NumericAlgebra
    implicit none
    real::InPoint(:)    
    type(CubicSurf_Tan)::InFromCubicSurf
    integer,target::InReviseIndex!-1-反向，1-不反向
    real::IOUVValue(:)
    real,optional::InOptPointRotParaList(:)
    real,optional::InOptSurfRotParaList(:)
    real,optional::InOptSurfSearchGrid(:,:)!在齿面哪个UV范围内进行搜寻，若未定义则采用原始曲面对象的自变量范围
    integer,optional,target::OutOptErrorNotation
    real,pointer::pTempVarRanges(:,:)
    real,pointer::pTempStorage(:,:)
    real,pointer::TempRotPointVecter(:),TempSurfMinDistPoint(:),TempSurfMinDistNormal(:)
    integer,pointer::TempErrorPtr
    integer,target::TempErrorCode
    !找最短点
    allocate(pTempVarRanges(2,2))
    allocate(pTempStorage(3,3))
    TempRotPointVecter=>pTempStorage(:,1)
    TempSurfMinDistPoint=>pTempStorage(:,2)
    TempSurfMinDistNormal=>pTempStorage(:,3)
    if(present(InOptPointRotParaList)) then
        call RotVectorByAxisAndOrigin_Tan(InOptPointRotParaList(1),InPoint,InOptPointRotParaList(2:4),InOptPointRotParaList(5:7),TempRotPointVecter)
    else
        TempRotPointVecter=InPoint
    end if
    if(present(InOptSurfSearchGrid)) then
        pTempVarRanges(:,1)=InOptSurfSearchGrid(:,1)
        pTempVarRanges(:,2)=InOptSurfSearchGrid(:,2)    
    else        
        pTempVarRanges(:,1)=InFromCubicSurf%uOriginParaList([1,InFromCubicSurf%uPointNum])
        pTempVarRanges(:,2)=InFromCubicSurf%vOriginParaList([1,InFromCubicSurf%vPointNum])    
    end if 
    if(present(OutOptErrorNotation)) then
        TempErrorPtr=>OutOptErrorNotation
    else
        TempErrorPtr=>TempErrorCode
    end if
    if(present(InOptSurfRotParaList)) then
        ans=sqrt(FindPointToCubicSurfMinL2DistancePower_Tan(TempRotPointVecter,InFromCubicSurf,IOUVValue,InOptSurfRotParaList,pTempVarRanges,OutOptErrorNotation=TempErrorPtr))
    else
        ans=sqrt(FindPointToCubicSurfMinL2DistancePower_Tan(TempRotPointVecter,InFromCubicSurf,IOUVValue,InOptSurfSearchGrid=pTempVarRanges,OutOptErrorNotation=TempErrorPtr))
    end if
    
    !if (TempErrorPtr>0) then
    !    write(6,*) 'FindPointToCubicSurfMinL2DirectedDistance_Tan错误码'
    !end if
    
    !确定法向距离
    !计算法向
    call CalCubicSurfValueWithLinearExtend_Tan(IOUVValue(1),&
        IOUVValue(2),&
        InFromCubicSurf,&
        TempSurfMinDistPoint)
    call CalCubicSurfNormalVectoreWithLinearExtend_Tan(IOUVValue(1),&
        IOUVValue(2),&
        InFromCubicSurf,&
        TempSurfMinDistNormal)
    if (present(InOptSurfRotParaList)) then
        if (Abs(InOptSurfRotParaList(1)).GT.0.000001) then
            call RotVectorByAxisAndOrigin_Tan(InOptSurfRotParaList(1),&
                TempSurfMinDistPoint,&
                InOptSurfRotParaList(2:4),&
                InOptSurfRotParaList(5:7),&
                TempSurfMinDistPoint) 
            call RotVectorByAxis_Tan(InOptSurfRotParaList(1),&
                TempSurfMinDistNormal,&
                InOptSurfRotParaList(2:4),&
                TempSurfMinDistNormal) 
        end if        
    end if      
    !if((FCalDotProduct_Tan(TempSurfMinDistNormal,TempRotPointVecter-TempSurfMinDistPoint)*InReviseIndex).le.0d0) then
    !    ans=-ans
    !end if
    ans=(FCalDotProduct_Tan(TempSurfMinDistNormal*InReviseIndex,TempRotPointVecter-TempSurfMinDistPoint))
    !if (TempErrorPtr>0) then
    !    write(6,*) TempErrorPtr
    !end if
    deallocate(pTempStorage)
    deallocate(pTempVarRanges)
    return
    end function
    !曲面点到曲面“有向”距离计算辅助函数，为了后面求解，曲面参数和点通过common参数传递,只适用于3维！！
    !将法线差异也作为权重系数代入了进去，不管贯入还是脱离
    !取消了有向距离，只考虑绝对距离
    real function AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan(InUV,InFuncObjPrtList) result (ans)
    use blas95
    use TAN_NumericAlgebra
    implicit none
    real,parameter::NoramlCoef=0d0
    real::InUV(:)
    type(c_ptr),optional::InFuncObjPrtList(:)  
    type(CubicSurf_Tan),pointer:: TempToSurfObjPtr,TempFromSurfObjPtr
    real,pointer::TempToSurfRotParaList(:),TempFromSurfRotParaList(:)
    real,pointer::pTempVector(:,:)!TempVector(3),TempOriginVector(3),TempOriginNormalVector(3)
    real,pointer::TempFromSurfUV(:)
    integer,pointer::TempReviseIndex!法线度量方向是否反向
    real,pointer::pTempFromSurfUVForEqu(:)
    integer,pointer::TempErrorCount
    integer::TempErrorNotation
    
    !解包
    call c_f_pointer(InFuncObjPrtList(1), TempToSurfObjPtr)  
    call c_f_pointer(InFuncObjPrtList(2), TempFromSurfObjPtr)
    call c_f_pointer(InFuncObjPrtList(3), TempReviseIndex) 
    call c_f_pointer(InFuncObjPrtList(4), TempToSurfRotParaList,[7]) 
    call c_f_pointer(InFuncObjPrtList(5), TempFromSurfRotParaList,[7]) 
    call c_f_pointer(InFuncObjPrtList(6), TempFromSurfUV,[2]) 
    call c_f_pointer(InFuncObjPrtList(7), TempErrorCount) 
    allocate(pTempVector(3,4))
    allocate(pTempFromSurfUVForEqu(2))
    !TempErrorCount=0
    pTempFromSurfUVForEqu=TempFromSurfUV
    call CalCubicSurfValueWithRotAndExtend_Tan(InUV,TempToSurfObjPtr,TempToSurfRotParaList,pTempVector(:,1))    
    ans=FindPointToCubicSurfMinL2DistancePower_Tan(pTempVector(:,1),&
        TempFromSurfObjPtr,&
        pTempFromSurfUVForEqu,&
        TempFromSurfRotParaList,&
        OutOptErrorNotation=TempErrorNotation)
    !write(6,*) pTempFromSurfUVForEqu
    TempErrorCount=TempErrorCount+TempErrorNotation
    
    !计算源面对应点和法线，考虑的接触方向
    !call CalCubicSurfValueWithRotAndExtend_Tan(pTempFromSurfUVForEqu,TempFromSurfObjPtr,TempFromSurfRotParaList,pTempVector(:,2))
    !call CalCubicSurfNormalVectorWithRotAndExtend_Tan(pTempFromSurfUVForEqu,TempFromSurfObjPtr,TempFromSurfRotParaList,pTempVector(:,3))
    !if((dot(pTempVector(:,3),pTempVector(:,1)-pTempVector(:,2))*TempReviseIndex).le.0d0) then
    !    ans=-ans
    !end if  
    !补充法线权重？
    !计算到法线
    !call CalCubicSurfNormalVectorWithRotAndExtend_Tan(InUV,TempToSurfObjPtr,TempToSurfRotParaList,pTempVector(:,4))
    !ans=ans+NoramlCoef*(1d0-abs(FCalDotProduct_Tan(pTempVector(:,3),pTempVector(:,4))))!后面是法线差异
    deallocate(pTempVector)
    deallocate(pTempFromSurfUVForEqu)
    return
    end function
    
    !曲面到曲面“有向”距离函数，为了后面求解，曲面参数和点通过common参数传递,只适用于3维！！
    !IOSurfUV,前2个是ToSurfUV,后两个是基准面FromSurfUV
    !取消了有向距离，只考虑绝对距离
    real function FindSurfToSurfMinL2DirectedDistance_Tan(InToSurfObj,InFromSurfObj,InReviseIndex,IOUVPointList,InToSurfRotPara,InFromSurfRotPara,OutOptErrorCode)  result(ans)
    use TAN_NumericOptimization
    implicit none
    type(CubicSurf_Tan),target::InFromSurfObj,InToSurfObj
    integer,target::InReviseIndex
    real,target::IOUVPointList(:)  !UV值加最短距离点坐标，一共10维度  
    real,optional,target::InToSurfRotPara(:),InFromSurfRotPara(:)
    integer,optional,target::OutOptErrorCode
    real,pointer::TempToSurfRotPtr(:),TempFromSurfRotPtr(:)
    type(c_ptr),pointer::pTempFuncObjPrtList(:)
    real,pointer::pTempVarRanges(:,:)
    real::TempNoUseValue
    integer::TempErrorCount
    !封包
    allocate(pTempFuncObjPrtList(7))
    allocate(pTempVarRanges(2,2))
    TempErrorCount=0
    pTempFuncObjPrtList(1)=c_loc(InToSurfObj)
    pTempFuncObjPrtList(2)=c_loc(InFromSurfObj)
    pTempFuncObjPrtList(3)=c_loc(InReviseIndex)
    if(present(InToSurfRotPara)) then        
        TempToSurfRotPtr=>InToSurfRotPara
    else 
        TempToSurfRotPtr=>DefaultRotParaList
    end if
    if(present(InFromSurfRotPara)) then  
        TempFromSurfRotPtr=>InFromSurfRotPara
    else
        TempFromSurfRotPtr=>DefaultRotParaList          
    end if
    pTempFuncObjPrtList(4)=c_loc(TempToSurfRotPtr) 
    pTempFuncObjPrtList(5)=c_loc(TempFromSurfRotPtr) 
    pTempFuncObjPrtList(6)=c_loc(IOUVPointList(3:4))
    pTempFuncObjPrtList(7)=c_loc(TempErrorCount)
    pTempVarRanges(:,1)=InToSurfObj%uOriginParaList([1,InToSurfObj%uPointNum])
    pTempVarRanges(:,2)=InToSurfObj%vOriginParaList([1,InToSurfObj%vPointNum])
    if(present(OutOptErrorCode)) then
        ans=FindMinValueWithVarRanges_Tan(&
            AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,&
            IOUVPointList(1:2),&
            [1,2],&
            pTempVarRanges,&
            pTempFuncObjPrtList,&
            InOptimumMethod=1,&
            OutOptErrorNotation=OutOptErrorCode)
        !write(6,*) ans
    else
        ans=FindMinValueWithVarRanges_Tan(&
            AuxiliaryGetSurfPointToSurfForL2DirectDistancePower_Tan,&
            IOUVPointList(1:2),&
            [1,2],&
            pTempVarRanges,&
            pTempFuncObjPrtList,&
            InOptimumMethod=1)
    end if
    !if (TempErrorCount>2) then
    !    OutOptErrorCode=1
    !end if
    !重新寻找大轮最短距离的坐标点，因为被FromSurf的UV被覆写
    call CalCubicSurfValueWithRotAndExtend_Tan(IOUVPointList(1:2),InToSurfObj,TempToSurfRotPtr,IOUVPointList(5:7))
    !IOUVPointList(3:4)=5d-1!初始化求解值
    TempNoUseValue=FindPointToCubicSurfMinL2DistancePower_Tan(IOUVPointList(5:7),InFromSurfObj,IOUVPointList(3:4),TempFromSurfRotPtr)
    !ans=sign(sqrt(abs(ans)),ans)
    ans=sqrt(ans)
    

    !输出坐标点
    call CalCubicSurfValueWithRotAndExtend_Tan(IOUVPointList(3:4),InFromSurfObj,TempFromSurfRotPtr,IOUVPointList(8:10))
    deallocate(pTempFuncObjPrtList)
    deallocate(pTempVarRanges)
    return
    end function
    
    !读取文本数据
    subroutine ImportSurfDataAndAxis(InImportPath,OutRotAxisAndOrigin,OutCubicSurfObj,InOptNotReadAxis)
    use TAN_BasicFunction
    use TAN_ImportData
    implicit none
    character(len=*) ::InImportPath
    real::OutRotAxisAndOrigin(:)
    type(CubicSurf_Tan)::OutCubicSurfObj
    !type(CubicSurf_Tan)::OutCubicSurfObj
    integer,optional::InOptNotReadAxis
    integer::TmpFileRowNum,TmpPointRowNum,TempImportRowIndex
    integer::TmpUCount,TmpVCount
    integer::TmpFileIndex=501,TmpFileStat
    real,pointer::pTmpSurfData(:,:,:)
    integer::i,j
    open (UNIT=TmpFileIndex,FILE=InImportPath,FORM='FORMATTED')
    TmpFileRowNum=CalDatFileRowNumByIndex(TmpFileIndex)
    !为列点数
    TmpPointRowNum=CalRowNumBetweenTwoIdentifiersByIndex(TmpFileIndex,'ROW')
    TmpVCount=TmpPointRowNum
    if (present(InOptNotReadAxis)) then
    !如果没有转动中心和转轴,则点的起始数据无偏移量
        TmpUCount=TmpFileRowNum/(TmpVCount+1)
    else  
        TmpUCount=(TmpFileRowNum-DefaultOtherDataEndIndex)/(TmpVCount+1)
    end if    
    !读取转动中心和转轴,可选
    if (present(InOptNotReadAxis)) then
    else        
        read(TmpFileIndex,*) OutRotAxisAndOrigin(4:6)
        read(TmpFileIndex,*) OutRotAxisAndOrigin(1:3)
    end if    
    !读取齿面点
    allocate(pTmpSurfData(3,TmpUCount,TmpVCount))
    TmpFileStat=0
    TempImportRowIndex=0
    do i=1,TmpUCount
        read(TmpFileIndex,*)
        do j=1,TmpVCount
            read(TmpFileIndex,*) pTmpSurfData(:,i,j)
        end do    
    enddo
    !call ExportUGSurfPoint(pTmpSurfData,'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\Pin.dat')
    !生成样条曲面对象
    call CreatCubicSurfSpace(3,TmpUCount,TmpVCount,OutCubicSurfObj)
    call BuildCubicSurfObject_Tan(OutCubicSurfObj%uOriginParaList,OutCubicSurfObj%vOriginParaList,pTmpSurfData,OutCubicSurfObj)
    deallocate(pTmpSurfData)
    close(TmpFileIndex)
    end subroutine
    
    !曲线相关距离
    !曲线点到曲线“有向”距离计算辅助函数，为了后面求解，曲面参数和点通过common参数传递,只适用于3维！！
    real function AuxiliaryGetSplinePointToSplineForL2DirectDistancePower_Tan(InVar,InFuncObjPrtList) result (ans)
    use blas95
    use TAN_CubicSpline
    use TAN_NumericAlgebra
    implicit none
    real::InVar(:)
    type(c_ptr),optional::InFuncObjPrtList(:)  
    type(Spline_Tan),pointer:: TempToSplineObjPtr,TempFromSplineObjPtr
    real,pointer::TempToSplineRotParaList(:),TempFromSplineRotParaList(:)
    real,pointer::pTempVector(:,:)!TempVector(3),TempOriginVector(3),TempOriginNormalVector(3)
    real,pointer::TempFromSplineVar(:)
    integer,pointer::TempMetricNormalMethod!曲面法线求取方法，0-不考虑，1-根据曲面方程和UV插值曲线求取
    type(CubicSurf_Tan),pointer::TempFromCubicSurfPtr
    integer,pointer::TempIsReviseNormal
    type(Spline_Tan),pointer:: TempFromSplineUVObjPtr
    real::TempFromSplineVarForEqu
    !real,pointer::pTempFromSplineVarForEqu(:)
    real,pointer::pTempUV(:)
    !integer,pointer::TempReviseIndex!法线度量方向是否反向,是否加入曲面法线方向？
    !解包
    call c_f_pointer(InFuncObjPrtList(1), TempToSplineObjPtr)  
    call c_f_pointer(InFuncObjPrtList(2), TempFromSplineObjPtr)
    call c_f_pointer(InFuncObjPrtList(3), TempMetricNormalMethod) 
    call c_f_pointer(InFuncObjPrtList(4), TempToSplineRotParaList,[7]) 
    call c_f_pointer(InFuncObjPrtList(5), TempFromSplineRotParaList,[7]) 
    call c_f_pointer(InFuncObjPrtList(6), TempFromSplineVar,[1]) 
    allocate(pTempVector(3,3))
    !allocate(pTempFromSplineVarForEqu(1))
    !pTempFromSplineVarForEqu(1)=TempFromSplineVar(1)
    !TempFromSplineVar=[(TempFromSplineObjPtr%OriginPointPara(1)+TempFromSplineObjPtr%OriginPointPara(size(TempFromSplineObjPtr%OriginPointPara)))/2d0]
    TempFromSplineVarForEqu=TempFromSplineVar(1)
    call CalCubicSplineVectorWithRotAndExtend_Tan(InVar(1),TempToSplineObjPtr,TempToSplineRotParaList,pTempVector(:,1))
    !计算最短距离
    ans=FindPointToCubicSplineMinL2DistancePower_Tan(pTempVector(:,1),&
        TempFromSplineObjPtr,&
        TempFromSplineVarForEqu,&
        TempFromSplineRotParaList)
    !如果考虑曲面法线
    if(TempMetricNormalMethod==1) then
        !计算FromSpline所在面的法线
        !计算点UV
        allocate(pTempUV(2))
        !通过UV插值线求法线
        call c_f_pointer(InFuncObjPrtList(7), TempIsReviseNormal) 
        call c_f_pointer(InFuncObjPrtList(8), TempFromCubicSurfPtr) 
        call c_f_pointer(InFuncObjPrtList(9), TempFromSplineUVObjPtr) 
        !计算度量基准点
        call CalCubicSplineVectorWithRotAndExtend_Tan(TempFromSplineVarForEqu,TempFromSplineObjPtr,TempFromSplineRotParaList,pTempVector(:,2))
        !!通过查找点到曲面最短距离求UV
        call CalCubicSplineInterpVector_Tan(TempFromSplineVarForEqu,TempFromSplineUVObjPtr,pTempUV)
        !计算源面法线
        call CalCubicSurfNormalVectorWithRotAndExtend_Tan(pTempUV,TempFromCubicSurfPtr,TempFromSplineRotParaList,pTempVector(:,3))        
        !call CalCubicSurfNormalVectoreWithLinearExtend_Tan(pTempUV(1),&
        !    pTempUV(2),&
        !    TempFromCubicSurfPtr,&
        !    pTempVector(:,3))
        !if (Abs(TempFromSplineRotParaList(1)).GT.0.000001) then
        !    call RotVectorByAxis_Tan(TempFromSplineRotParaList(1),&
        !        pTempVector(:,3),&
        !        TempFromSplineRotParaList(2:4),&
        !        pTempVector(:,3)) 
        !end if
        if((dot(pTempVector(:,3),pTempVector(:,1)-pTempVector(:,2))*TempIsReviseNormal).le.0d0) then
            ans=-ans
        end if 
        !call CalCubicSplineVectorWithExpand_Tan(TempFromSplineVar(1),TempFromSplineObjPtr,pTempVector(:,2))
        !pTempUV(1)=(TempFromCubicSurfPtr%uOriginParaList(1)+TempFromCubicSurfPtr%uOriginParaList(size(TempFromCubicSurfPtr%uOriginParaList)))/2d0
        !pTempUV(2)=(TempFromCubicSurfPtr%vOriginParaList(1)+TempFromCubicSurfPtr%vOriginParaList(size(TempFromCubicSurfPtr%vOriginParaList)))/2d0
        !write(6,*) FindPointToCubicSurfMinL2DistancePower_Tan(pTempVector(:,2),TempFromCubicSurfPtr,pTempUV)
        !write(6,*)ans
        deallocate(pTempUV)
    end if
    deallocate(pTempVector)
    
    return
    end function
    !曲线到曲线“有向”距离函数，为了后面求解，曲面参数和点通过common参数传递,只适用于3维！！
    !IOSurfUV,前2个是ToSurfUV,后两个是基准面FromSurfUV
    real function FindSplineToSplineMinL2DirectedDistance_Tan(InToSplineObj,InFromSplineObj,&
        InMetricNormalMethod,&
        IOVarList,&        
        InToSplineRotPara,InFromSplineRotPara,&
        InIsReviseNormal,InMetricSurfObj,InUVInterpCurveObj)  result(ans)
    use TAN_NumericOptimization
    use TAN_CubicSpline
    implicit none
    type(Spline_Tan),target::InFromSplineObj,InToSplineObj    
    real,target::IOVarList(:)  !Var值加最短距离点坐标，一共8维度  ,1-ToSpline曲线参数，2-FromSpline曲线参数，3-5-ToSpline曲线点，6-8FromSpline曲线点
    integer::InMetricNormalMethod!法线判定方法，0-不考虑方向,1-考虑所在曲面法向
    real,optional,target::InToSplineRotPara(:),InFromSplineRotPara(:)
    real,pointer::TempToSplineRotPtr(:),TempFromSplineRotPtr(:)
    integer,optional::InIsReviseNormal!法线系数，-1反向,1-不反向
    type(CubicSurf_Tan),optional::InMetricSurfObj
    type(Spline_Tan),optional::InUVInterpCurveObj    
    type(c_ptr),pointer::pTempFuncObjPrtList(:)
    real,pointer::pTempVarRanges(:,:)
    real::TempNoUseValue
    !封包
    allocate(pTempFuncObjPrtList(9))
    allocate(pTempVarRanges(2,1))
    pTempFuncObjPrtList(1)=c_loc(InToSplineObj)
    pTempFuncObjPrtList(2)=c_loc(InFromSplineObj)
    pTempFuncObjPrtList(3)=c_loc(InMetricNormalMethod)
    if(present(InToSplineRotPara)) then        
        TempToSplineRotPtr=>InToSplineRotPara
    else 
        TempToSplineRotPtr=>DefaultRotParaList
    end if
    if(present(InFromSplineRotPara)) then  
        TempFromSplineRotPtr=>InFromSplineRotPara
    else
        TempFromSplineRotPtr=>DefaultRotParaList          
    end if
    pTempFuncObjPrtList(4)=c_loc(TempToSplineRotPtr) 
    pTempFuncObjPrtList(5)=c_loc(TempFromSplineRotPtr) 
    pTempFuncObjPrtList(6)=c_loc(IOVarList(2:2))
    if(present(InIsReviseNormal)) then
        if(InMetricNormalMethod==1) then
            pTempFuncObjPrtList(7)=c_loc(InIsReviseNormal)
            pTempFuncObjPrtList(8)=c_loc(InMetricSurfObj)
            pTempFuncObjPrtList(9)=c_loc(InUVInterpCurveObj)
        end if
    end if   
    pTempVarRanges(:,1)=InToSplineObj%OriginPointPara([1,InToSplineObj%PointNum])
    ans=FindMinValueWithVarRanges_Tan(&
        AuxiliaryGetSplinePointToSplineForL2DirectDistancePower_Tan,&
        IOVarList(1:1),&
        [1],&
        pTempVarRanges,&
        pTempFuncObjPrtList,&
        InOptimumMethod=1)
    !重新寻找大轮最短距离的坐标点，因为被FromSurf的UV被覆写
    call CalCubicSplineVectorWithRotAndExtend_Tan(IOVarList(1),InToSplineObj,TempToSplineRotPtr,IOVarList(3:5))
    !call CalCubicSurfValueWithRotAndExtend_Tan(IOUVPointList(1:2),InToSurfObj,TempToSurfRotPtr,IOUVPointList(5:7))
    !IOVarList(2:2)=(InToSplineObj%OriginPointPara(1)+InToSplineObj%OriginPointPara(size(InToSplineObj%OriginPointPara)))/2
    TempNoUseValue=FindPointToCubicSplineMinL2DistancePower_Tan(IOVarList(3:5),InFromSplineObj,IOVarList(2),TempFromSplineRotPtr)
    ans=sign(sqrt(abs(ans)),ans)
    
    !输出坐标点
    call CalCubicSplineVectorWithRotAndExtend_Tan(IOVarList(2),InFromSplineObj,TempFromSplineRotPtr,IOVarList(6:8))
    deallocate(pTempFuncObjPrtList)
    deallocate(pTempVarRanges)
    return
    end function
    !曲线到曲面的绝对距离函数，为了后面求解，曲面参数和点通过common参数传递,只适用于3维！！
    !IOSurfUV,前2个是ToSurfUV,后两个是基准面FromSurfUV
    real function AuxiliaryGetSplineToSurfForL2DirectDistancePower_Tan(InVar,InFuncObjPrtList) result (ans)
    use blas95
    use TAN_CubicSpline
    use TAN_NumericAlgebra
    implicit none
    real::InVar(:)
    type(c_ptr),optional::InFuncObjPrtList(:)  
    type(Spline_Tan),pointer:: TempToSplineObjPtr
    type(CubicSurf_Tan),pointer::TempFromCubicSurfPtr
    integer,pointer::TempMetricNormalMethod!曲面法线求取方法，0-不考虑，1-根据曲面方程和UV插值曲线求取
    real,pointer::TempToSplineRotParaList(:),TempFromSplineRotParaList(:)
    real,pointer::pTempVector(:,:)!TempVector(3),TempOriginVector(3),TempOriginNormalVector(3)
    real,pointer::TempFromSurfVar(:)    
    integer,pointer::TempIsReviseNormal
    type(Spline_Tan),pointer:: TempFromSplineUVObjPtr
    real,pointer::TempFromSurfVarForEqu(:)
    !real,pointer::pTempFromSplineVarForEqu(:)
    !integer,pointer::TempReviseIndex!法线度量方向是否反向,是否加入曲面法线方向？
    !解包
    call c_f_pointer(InFuncObjPrtList(1), TempToSplineObjPtr)  
    call c_f_pointer(InFuncObjPrtList(2), TempFromCubicSurfPtr)
    call c_f_pointer(InFuncObjPrtList(3), TempMetricNormalMethod) 
    call c_f_pointer(InFuncObjPrtList(4), TempToSplineRotParaList,[7]) 
    call c_f_pointer(InFuncObjPrtList(5), TempFromSplineRotParaList,[7]) 
    call c_f_pointer(InFuncObjPrtList(6), TempFromSurfVar,[2]) 
    allocate(pTempVector(3,2))
    TempFromSurfVarForEqu=>pTempVector(1:2,2)
    TempFromSurfVarForEqu=TempFromSurfVar(1:2)
    !计算曲线点坐标
    call CalCubicSplineVectorWithRotAndExtend_Tan(InVar(1),TempToSplineObjPtr,TempToSplineRotParaList,pTempVector(:,1))
    !计算曲线点坐标到曲面的距离
    ans= FindPointToCubicSurfMinL2DistancePower_Tan(pTempVector(:,1),&
        TempFromCubicSurfPtr,&
        TempFromSurfVarForEqu,&
        TempFromSplineRotParaList)   
    deallocate(pTempVector)    
    return
    end function    
    !曲线到曲面的最短距离
    real function FindSplineToSurfMinL2Distance_Tan(InToSplineObj,InFromSurfObj,IOTUVPointList,InToSplineRotPara,InFromSurfRotPara)  result(ans)
    use TAN_NumericOptimization
    use TAN_CubicSpline
    implicit none
    type(Spline_Tan),target:: InToSplineObj
    type(CubicSurf_Tan),target::InFromSurfObj
    integer::NoUseReviseIndex
    real,target::IOTUVPointList(:)  !TUV值加最短距离点坐标，一共9维度  
    real,optional,target::InToSplineRotPara(:),InFromSurfRotPara(:)
    real,pointer::TempToSplineRotPtr(:),TempFromSurfRotPtr(:)
    type(c_ptr),pointer::pTempFuncObjPrtList(:)
    real,pointer::pTempVarRanges(:,:)
    real::TempNoUseValue
    integer::TempErrorCount
    !封包
    allocate(pTempFuncObjPrtList(6))
    allocate(pTempVarRanges(2,1))
    TempErrorCount=0
    pTempFuncObjPrtList(1)=c_loc(InToSplineObj)
    pTempFuncObjPrtList(2)=c_loc(InFromSurfObj)
    pTempFuncObjPrtList(3)=c_loc(NoUseReviseIndex)
    if(present(InToSplineRotPara)) then        
        TempToSplineRotPtr=>InToSplineRotPara
    else 
        TempToSplineRotPtr=>DefaultRotParaList
    end if
    if(present(InFromSurfRotPara)) then  
        TempFromSurfRotPtr=>InFromSurfRotPara
    else
        TempFromSurfRotPtr=>DefaultRotParaList          
    end if
    pTempFuncObjPrtList(4)=c_loc(TempToSplineRotPtr) 
    pTempFuncObjPrtList(5)=c_loc(TempFromSurfRotPtr) 
    pTempFuncObjPrtList(6)=c_loc(IOTUVPointList(2:3))
    pTempVarRanges(:,1)=InToSplineObj%OriginPointPara([1,InToSplineObj%PointNum])
    ans=FindMinValueWithVarRanges_Tan(&
        AuxiliaryGetSplineToSurfForL2DirectDistancePower_Tan,&
        IOTUVPointList(1:1),&
        [1],&
        pTempVarRanges,&
        pTempFuncObjPrtList,&
        InOptimumMethod=1)
    !重新寻找大轮最短距离的坐标点，因为被FromSurf的UV被覆写
    call CalCubicSplineVectorWithRotAndExtend_Tan(IOTUVPointList(1),InToSplineObj,TempToSplineRotPtr,IOTUVPointList(4:6))
    !IOUVPointList(3:4)=5d-1!初始化求解值
    TempNoUseValue=FindPointToCubicSurfMinL2DistancePower_Tan(IOTUVPointList(4:6),InFromSurfObj,IOTUVPointList(2:3),TempFromSurfRotPtr)
    !ans=sign(sqrt(abs(ans)),ans)
    ans=sqrt(ans) 
    !输出坐标点
    call CalCubicSurfValueWithRotAndExtend_Tan(IOTUVPointList(2:3),InFromSurfObj,TempFromSurfRotPtr,IOTUVPointList(7:9))
    deallocate(pTempFuncObjPrtList)
    deallocate(pTempVarRanges)
    return
    end function
    !判断点是否落在了边缘，并返回标识码
    integer function ReturnPointLocalteOnSurfNontationByUV_TAN(InSurfUV,InSurfObj,InOptDevRange) result(ans)
    use TAN_BasicCoef
    implicit none
    real::InSurfUV(:)
    type(CubicSurf_Tan)::InSurfObj
    real,optional::InOptDevRange
    real::TempDevRange
    if (present(InOptDevRange)) then
        TempDevRange=abs(InOptDevRange)
    else
        TempDevRange=DefaultAbsolueError
    end if    
    if (InSurfUV(1)<InSurfObj%uOriginParaList(1)+TempDevRange) then
        ans=10
    else if(InSurfUV(1)>InSurfObj%uOriginParaList(InSurfObj%uPointNum)-TempDevRange) then
        ans=11
    else if(InSurfUV(2)<InSurfObj%vOriginParaList(1)+TempDevRange) then
        ans=20
    else if(InSurfUV(2)>InSurfObj%vOriginParaList(InSurfObj%vPointNum)-TempDevRange) then
        ans=21
    else
        ans=0
    end if
    
    return
    end function
    !!加入法线因素定义最短距离
    !real function AuxiliaryGetPointToCubicSurfPointForL2DistPowerWithNormal_Tan(InUV,InFuncObjPrtList) result(ans)
    !use TAN_NumericAlgebra
    !use blas95
    !implicit none
    !real::InUV(:)
    !type(c_ptr),optional::InFuncObjPrtList(:)  
    !type(CubicSurf_Tan),pointer:: TempFromSurfObjPtr
    !real,pointer::TempMetricPoint(:),TempRotParaList(:),TempPointNormal
    !real,pointer::pTempVector(:),pTempNormal
    !real,pointer::pTempStorage(:)
    !allocate(pTempVector(3))
    !call c_f_pointer(InFuncObjPrtList(1), TempFromSurfObjPtr) 
    !call c_f_pointer(InFuncObjPrtList(2), TempMetricPoint,[3]) 
    !!默认有旋转原动
    !call c_f_pointer(InFuncObjPrtList(3),TempRotParaList,[7])
    !!点的对应法线
    !call c_f_pointer(InFuncObjPrtList(4),TempPointNormal,[3])    
    !call CalCubicSurfValueWithRotAndExtend_Tan(InUV,TempFromSurfObjPtr,TempRotParaList,pTempVector)
    !!计算法线
    !call CalCubicSurfPartDeriveWithLinearExtend_Tan(InUV,TempFromSurfObjPtr,TempRotParaList,pTempVector)
    !
    !call axpy(TempMetricPoint,pTempVector,-1d0)
    !ans=dot(pTempVector,pTempVector)
    !deallocate(pTempVector)
    !return
    !end function


    
    
    !!曲面到曲面“有向”距离函数，含转角参数
    !!IOUVPointList=1-2小轮UV,3-4大轮UV，5-7小轮点，8-10大轮点
    !real function FindRotSurfToRotSurfMinL2DirectedDistance_Tan(InToSurfRotParaList,&
    !    InFromSurfRotParaList,&
    !    InToSurfObj,InFromSurfObj,InReviseIndex,IOUVPointList)  
    !use TAN_Vector
    !implicit none
    !type(CubicSurf_Tan),target::InFromSurfObj,InToSurfObj
    !real,target::InToSurfRotParaList(:)
    !real,target::InFromSurfRotParaList(:)
    !integer,target::InReviseIndex
    !real,target::IOUVPointList(:)
    !CommonRotParaToSurfPtr=>InToSurfRotParaList
    !CommonRotParaFromSurfPtr=>InFromSurfRotParaList
    !FindRotSurfToRotSurfMinL2DirectedDistance_Tan=FindSurfToSurfMinL2DirectedDistance_Tan(InToSurfObj,InFromSurfObj,InReviseIndex,IOUVPointList)
    !call CalCubicSurfValueWithLinearExtend_Tan(IOUVPointList(1),IOUVPointList(2),CommonToSurfPtrForDistance,IOUVPointList(5:7))
    !call CalCubicSurfValueWithLinearExtend_Tan(IOUVPointList(3),IOUVPointList(4),CommonToSurfPtrForDistance,IOUVPointList(8:10))
    !if (associated(CommonRotParaToSurfPtr)) then
    !    if (Abs(CommonRotParaToSurfPtr(1)).GT.0.000001) then
    !        call RotVectorByAxisAndOrigin_Tan(CommonRotParaToSurfPtr(1),&
    !            IOUVPointList(5:7),&
    !            CommonRotParaToSurfPtr(2:4),&
    !            CommonRotParaToSurfPtr(5:7),IOUVPointList(5:7)) 
    !    end if        
    !end if
    !if (associated(CommonRotParaFromSurfPtr))then
    !    if (Abs(CommonRotParaFromSurfPtr(1)).GT.0.000001) then
    !        call RotVectorByAxisAndOrigin_Tan(CommonRotParaFromSurfPtr(1),&
    !            IOUVPointList(8:10),&
    !            CommonRotParaFromSurfPtr(2:4),&
    !            CommonRotParaFromSurfPtr(5:7),IOUVPointList(8:10)) 
    !    end if
    !end if
    !
    !nullify(CommonRotParaToSurfPtr)
    !nullify(CommonRotParaFromSurfPtr)
    !return
    !end function
    
    
    
    
    
    
    
    !曲面到曲面“有向”距离计算辅助函数
    !CommonIsReviseMetricDirectionForDistance
    
    
    !!之前给精益做的时候弄的代码
    !subroutine FindSurfRootWithPartialDerive(IOVauleGuess,InCubicSurf,InEqu,InEquCoef)
    !use TAN_BasicNumeric
    !implicit none
    !type(CubicSurf_Tan),target::InCubicSurf
    !external::InEqu
    !real::IOVauleGuess(:),InEquCoef(:)
    !CubicSurfEquTransPtr=>InCubicSurf
    !call SolveMultiEquByNewtonMethodWithDerive(IOVauleGuess,InEqu,InEquCoef)
    !return
    !end subroutine
    !
    subroutine TestDiff2Function(InFValueList,OutFVector,InFFuncObjPrtList)
    use iso_c_binding
    implicit none
        real::InFValueList(:)   
        type(c_ptr),optional::InFFuncObjPrtList(:)
        real::OutFVector(:)
        call CalCubicSurfInterpValue_Tan(InFValueList(1),InFValueList(2),TestSurfPtr,OutFVector)
    end subroutine
    end module
    !
    !!测试函数
    !subroutine CalResidualSurfTestFunc(EquNum,InXGuess,EquCoef,EquValue,DEquMatrix)
    !use TAN_CubicSurf
    !implicit none
    !integer::EquNum
    !real::InXGuess(EquNum)
    !real::EquCoef(EquNum),EquValue(EquNum),DEquMatrix(EquNum,EquNum)
    !real::TempVector(3,2)
    !TempVector=0
    !call CalCubicSurfValueWithLinearExtend_Tan(InXGuess(1),InXGuess(2),CubicSurfEquTransPtr,TempVector(:,1))
    !EquValue(1)=TempVector(1,1)-EquCoef(1)
    !EquValue(2)=TempVector(2,1)-EquCoef(2)
    !call CalCubicSurfPartDeriveWithLinearExtend_Tan(InXGuess(1),InXGuess(2),CubicSurfEquTransPtr,TempVector(:,1),TempVector(:,2))
    !DEquMatrix(:,1)=TempVector(1:2,1)
    !DEquMatrix(:,2)=TempVector(1:2,2)
    !return
    !end subroutine