
    module TAN_CubicSpline
    use iso_c_binding, only: c_ptr, c_f_pointer,c_loc
    implicit none
    public
    type::Spline_Tan
        integer::PointNum,PointDimension !点数
        real,pointer::OriginPointPara(:) !原始点自变量值
        real,pointer::Cscoef(:,:) !系数,C0,C1,...,D0,D1....
        integer::CreatMethod!如果是1，则为已动态创建，否则为0
    end type Spline_Tan
    type(Spline_Tan),pointer::CubicSplineEquTransPtr
    real,target::DefaultRotParaList(7)=0d0 
    contains
    !创建样条空间
    subroutine CreatCubicSplineSpace(PointNum,InDimension,CubicSplineName)
    implicit none
    integer ::PointNum,InDimension
    type(Spline_Tan):: CubicSplineName
    if (CubicSplineName%CreatMethod .EQ. 1) then
        call DeleteCubicSplineSpace(CubicSplineName)
    end if
    CubicSplineName%PointDimension=InDimension
    allocate (CubicSplineName%OriginPointPara(PointNum))
    allocate (CubicSplineName%Cscoef(InDimension,2*PointNum))
    CubicSplineName%OriginPointPara=0
    CubicSplineName%Cscoef=0
    CubicSplineName%PointNum=PointNum
    CubicSplineName%CreatMethod=1
    return    
    end subroutine
    !删除样条空间
    subroutine DeleteCubicSplineSpace(CubicSplineName)
    implicit none
    type(Spline_Tan) CubicSplineName
    if (CubicSplineName%CreatMethod .EQ. 1) then
    deallocate (CubicSplineName%OriginPointPara)
    deallocate (CubicSplineName%Cscoef)
    CubicSplineName%CreatMethod=0
    end if    
    return
    end subroutine
    !Hermite插值基函数
    subroutine CalBaseVector(InX,InXStart,InXEnd,OutBaseVector)!OutBaseVector={C0,D0,C1,D1}
    implicit none
    real::InX,InXStart,InXEnd,OutBaseVector(:)
    OutBaseVector(1)=((InX - InXEnd)**2*(2*InX + InXEnd - 3*InXStart))/(InXEnd - InXStart)**3
    OutBaseVector(2)=((InX - InXEnd)**2*(InX - InXStart))/(InXEnd - InXStart)**2
    OutBaseVector(3)=-(((InX - InXStart)**2*(2*InX - 3*InXEnd + InXStart))/(InXEnd - InXStart)**3)
    OutBaseVector(4)=((InX - InXEnd)*(InX - InXStart)**2)/(InXEnd - InXStart)**2
    return
    end subroutine
    !Hermite插值基函数1阶导
    subroutine CalBaseVector1Derive(InX,InXStart,InXEnd,OutBase1DervieVector)!OutBaseVector={C0,D0,C1,D1}
    implicit none
    real::InX,InXStart,InXEnd,OutBase1DervieVector(:)
    OutBase1DervieVector(1)=(6*(InX - InXEnd)*(InX - InXStart))/(InXEnd - InXStart)**3
    OutBase1DervieVector(2)=((InX - InXEnd)*(3*InX - InXEnd - 2*InXStart))/(InXEnd - InXStart)**2
    OutBase1DervieVector(3)=(-6*(InX - InXEnd)*(InX - InXStart))/(InXEnd - InXStart)**3
    OutBase1DervieVector(4)=((InX - InXStart)*(3*InX - 2*InXEnd - InXStart))/(InXEnd - InXStart)**2
    return
    end subroutine
    !Hermite插值基函数2阶导
    subroutine CalBaseVector2Derive(InX,InXStart,InXEnd,OutBase1DervieVector)!OutBaseVector={C0,D0,C1,D1}
    implicit none
    real::InX,InXStart,InXEnd,OutBase1DervieVector(:)
    OutBase1DervieVector(1)=(-6*(-2*InX + InXEnd + InXStart))/(InXEnd - InXStart)**3
    OutBase1DervieVector(2)=(-2*(-3*InX + 2*InXEnd + InXStart))/(InXEnd - InXStart)**2
    OutBase1DervieVector(3)=(6*(-2*InX + InXEnd + InXStart))/(InXEnd - InXStart)**3
    OutBase1DervieVector(4)=(-2*(-3*InX + InXEnd + 2*InXStart))/(InXEnd - InXStart)**2
    return
    end subroutine
    !计算样条两端1阶导数,1维
    real function CalEndPointDer(xValue,xVector,yVector)
    implicit none
    real :: xValue,xVector(4),yVector(4)
    real :: x,x0,x1,x2,x3,y0
    real :: DeriBase
    DeriBase(x,x0,x1,x2,x3,y0)=((3*x**2 + x2*x3 + x1*(x2 + x3) - 2*x*(x1 + x2 + x3))*y0)/ ((x0 - x1)*(x0 - x2)*(x0 - x3))
    CalEndPointDer=DeriBase(xValue,xVector(1),xVector(2),xVector(3),xVector(4),yVector(1))+ &
        DeriBase(xValue,xVector(2),xVector(1),xVector(3),xVector(4),yVector(2))+&
        DeriBase(xValue,xVector(3),xVector(2),xVector(1),xVector(4),yVector(3))+&
        DeriBase(xValue,xVector(4),xVector(2),xVector(3),xVector(1),yVector(4))
    return
    end function
    !计算1维样条函数值
    real function Cal1DCubicSplineValue(InXValue,InXdata,InCscoef)    
    use f95_precision
    use blas95    
    implicit none
    real::InXValue,InXdata(:),InCscoef(:)
    integer::XValueIndex
    real::BaseVector(4)
    XValueIndex=FindSplineVariantIndex(InXValue,InXdata)
    call CalBaseVector(InXValue,&
        InXdata(XValueIndex-1),&
        InXdata(XValueIndex),&
        BaseVector)
    Cal1DCubicSplineValue=dot(BaseVector,&
        InCscoef(2*XValueIndex-3:2*XValueIndex))
    return
    end function
    !计算1维样条1阶导值
    real function Cal1DCubicSpline1Derive(InXValue,InXdata,InCscoef)
    use TAN_BasicFunction
    use f95_precision
    use blas95
    implicit none
    real::InXValue,InXdata(:),InCscoef(:)
    integer::XValueIndex
    real::Base1DeriveVector(4)
    XValueIndex=FindSplineVariantIndex(InXValue,InXdata)
    call CalBaseVector1Derive(InXValue,&
        InXdata(XValueIndex-1),&
        InXdata(XValueIndex),&
        Base1DeriveVector)
    Cal1DCubicSpline1Derive=dot(Base1DeriveVector,&
        InCscoef(2*XValueIndex-3:2*XValueIndex))
    return
    end function
    !计算1维样条2阶导值
    real function Cal1DCubicSpline2Derive(InXValue,InXdata,InCscoef)
    use TAN_BasicFunction
    use f95_precision
    use blas95
    implicit none
    real::InXValue,InXdata(:),InCscoef(:)
    integer::XValueIndex
    real::Base2DeriveVector(4)
    XValueIndex=FindSplineVariantIndex(InXValue,InXdata)
    call CalBaseVector2Derive(InXValue,&
        InXdata(XValueIndex-1),&
        InXdata(XValueIndex),&
        Base2DeriveVector)
    Cal1DCubicSpline2Derive=dot(Base2DeriveVector,&
        InCscoef(2*XValueIndex-3:2*XValueIndex))
    return
    end function   
    !计算根据连接节点二阶导系数相等求解一阶导方程系数的矩阵
    subroutine CalSolveEquMatrixCoef(InXList,InFList,OutMatrixCoef,OutVectorCoef)
    implicit none
    real ::InXList(:)
    real ::InFList(:)
    real ::OutMatrixCoef(:) !单个方程左侧系数，3个
    real ::OutVectorCoef !单个方程右侧值，1个
    OutVectorCoef=(-6*InFList(1))/(InXList(1) - InXList(2))**2 + &
        6*((InXList(1) - InXList(2))**(-2) - (InXList(2) - InXList(3))**(-2))*InFList(2) + &
        (6*InFList(3))/(InXList(2) - InXList(3))**2!求解方程右矢量计算函数
    OutMatrixCoef(1)=2/(-InXList(1) + InXList(2))
    OutMatrixCoef(2)=4*(1/(-InXList(1) + InXList(2)) + 1/(-InXList(2) + InXList(3)))
    OutMatrixCoef(3)=2/(-InXList(2) + InXList(3))
    return
    end subroutine
    !根据边界一阶导条件计算1维样条系数
    subroutine Cal1DCubicSplineCoef(InXdata,InFdata,OutCscoef)
    use f95_precision
    use blas95
    use lapack95
    use TAN_BasicFunction
    implicit none
    real ::InXdata(:),InFdata(:),OutCscoef(:)
    real ::LeftEndDeri,RightEndDeri
    integer :: PointCount
    real ::LeftPoint(2,4), RightPoint(2,4)
    real,pointer ::DeriCoefSolveLeftMatrix(:,:)!(根据连接节点二阶导系数相等求解一阶导系数的矩阵)
    real,pointer ::DeriCoefSolveRightVector(:)    
    integer::i
    integer,pointer::MKL_ipiv(:)
    PointCount=size(InXdata)
    LeftPoint(1,1:4)=InXdata(1:4)
    LeftPoint(2,1:4)=InFdata(1:4)
    RightPoint(1,1:4)=InXdata(PointCount-3:PointCount)
    RightPoint(2,1:4)=InFdata(PointCount-3:PointCount)
    LeftEndDeri = CalEndPointDer(LeftPoint(1,1),LeftPoint(1,1:4),LeftPoint(2,1:4))
    RightEndDeri = CalEndPointDer(RightPoint(1,4),RightPoint(1,1:4),RightPoint(2,1:4))
    allocate(DeriCoefSolveLeftMatrix(PointCount,PointCount))
    allocate(DeriCoefSolveRightVector(PointCount))
    allocate(MKL_ipiv(PointCount))
    DeriCoefSolveLeftMatrix=0
    DeriCoefSolveLeftMatrix(1,1)=1
    DeriCoefSolveLeftMatrix(PointCount,PointCount)=1
    DeriCoefSolveRightVector(1)=LeftEndDeri
    DeriCoefSolveRightVector(PointCount)=RightEndDeri 
    do i=2,PointCount-1
        call CalSolveEquMatrixCoef(InXdata(i-1:i+1),&
            InFdata(i-1:i+1),&
            DeriCoefSolveLeftMatrix(i,i-1:i+1),&
            DeriCoefSolveRightVector(i))
    end do
    !open(20,'F:\Workspace\softdevelop\测试数据\插值矩阵.dat')
    !call PrintMatrixByDeviceNum(DeriCoefSolveLeftMatrix,6)
    call getrf(DeriCoefSolveLeftMatrix,MKL_ipiv)
    call getrs(DeriCoefSolveLeftMatrix,MKL_ipiv,DeriCoefSolveRightVector)
    call copy(InFdata,OutCscoef(1:2*PointCount:2))
    call copy(DeriCoefSolveRightVector,OutCscoef(2:2*PointCount:2))
    deallocate(DeriCoefSolveLeftMatrix)
    deallocate(DeriCoefSolveRightVector)
    deallocate(MKL_ipiv)
    return
    end subroutine
    
    !计算输入值所在1维样条值域区间
    integer function FindSplineVariantIndex(InXValue,InXdata)
    use TAN_BasicFunction
    implicit none
    real::InXValue,InXdata(:)
    if (InXValue<=InXdata(2)) then
        FindSplineVariantIndex=2
    else
        FindSplineVariantIndex=FindFirstLargerValueIndex_Tan(InXValue,InXdata)
    end if
    return
    end function    
    !计算3维样条系数
    subroutine BuildCubicSplineObject_Tan(InPointPara,InPointVector,IOSpline)
    implicit none
    type(Spline_Tan)::IOSpline
    real::InPointPara(:),InPointVector(:,:)
    integer::i,TempVectorDimension,TempPointNum
    TempVectorDimension=size(InPointVector,1)
    TempPointNum=size(InPointPara)
    !进行自变量是升序还是降序判断
    if(InPointPara(1)<InPointPara(TempPointNum)) then!自变量是升序        
        do i=1,TempVectorDimension
            call Cal1DCubicSplineCoef(InPointPara,&
                InPointVector(i,:),&
                IOSpline%Cscoef(i,:))
        end do
        IOSpline%OriginPointPara=InPointPara
    else!自变量是降序，调整到升序再构建
        IOSpline%OriginPointPara=InPointPara(TempPointNum:1:-1)
        do i=1,TempVectorDimension
            call Cal1DCubicSplineCoef(IOSpline%OriginPointPara,&
                InPointVector(i,TempPointNum:1:-1),&
                IOSpline%Cscoef(i,:))
        end do
    end if
    

    return
    end subroutine
    !计算不带线性外延的3维样条值
    subroutine CalCubicSplineInterpVector_Tan(InVariant,InSpline,OutVector)
    type(Spline_Tan)::InSpline
    real::InVariant,OutVector(:)
    integer::i,VectorDimension
    VectorDimension=size(InSpline%Cscoef,1)
    do i=1,VectorDimension
        OutVector(i)=Cal1DCubicSplineValue(InVariant,InSpline%OriginPointPara,InSpline%Cscoef(i,:))
    end do
    return
    end subroutine
   !线性外延计算自变量对应样条矢量 
    subroutine CalCubicSplineExpandVector_Tan(InPara,InSpline,OutVector)
    use TAN_BasicFunction
    implicit none
    real::InPara
    type(Spline_Tan):: InSpline
    real::OutVector(InSpline%PointDimension)
    if (InPara<InSpline%OriginPointPara(1)) then
        call CalExtendVector_Tan((InPara-InSpline%OriginPointPara(1)),&
            InSpline%Cscoef(:,1),&
            InSpline%Cscoef(:,2),&
            OutVector)
    else
        call CalExtendVector_Tan((InPara-InSpline%OriginPointPara(InSpline%PointNum)),&
            InSpline%Cscoef(:,2*InSpline%PointNum-1),&
            InSpline%Cscoef(:,2*InSpline%PointNum),&
            OutVector)
    end if
    return
    end subroutine
    !计算带线性外延3维样条值
    subroutine CalCubicSplineVectorWithExpand_Tan(InPara,InSpline,OutVector)
    implicit none
    real::InPara
    type(Spline_Tan):: InSpline
    real::OutVector(InSpline%PointDimension)
    if (Inpara<InSpline%OriginPointPara(1) .OR. &
        Inpara>InSpline%OriginPointPara(InSpline%PointNum)) then        
        call CalCubicSplineExpandVector_Tan(InPara,InSpline,OutVector)
    else        
        call CalCubicSplineInterpVector_Tan(InPara,InSpline,OutVector)
    end if
    return        
    end subroutine
    !计算非线性外延3维样条1阶导
    subroutine CalCubicSplineInterpVector1Derive_Tan(InVariant,InSpline,OutVector)
    type(Spline_Tan)::InSpline
    real::InVariant,OutVector(:)
    integer::i,VectorDimension
    VectorDimension=size(InSpline%Cscoef,1)
    do i=1,VectorDimension
        OutVector(i)=Cal1DCubicSpline1Derive(InVariant,InSpline%OriginPointPara,InSpline%Cscoef(i,:))
    end do
    return
    end subroutine
    !计算带线性外延3维样条1阶导
    subroutine CalCubicSplineVector1DeriveWithExpand_Tan(InVariant,InSpline,OutVector)
    type(Spline_Tan)::InSpline
    real::InVariant,OutVector(:)
    integer::i,VectorDimension
    if (InVariant<InSpline%OriginPointPara(1)) then
        OutVector=InSpline%Cscoef(:,1)
    else if (InVariant>InSpline%OriginPointPara(InSpline%PointNum)) then
        OutVector=InSpline%Cscoef(:,2*InSpline%PointNum-1)
    else
        call CalCubicSplineInterpVector1Derive_Tan(InVariant,InSpline,OutVector)
    end if
    return
    end subroutine
    !其他函数
    !带转动的插值曲面值计算
    subroutine CalCubicSplineVectorWithRotAndExtend_Tan(InVar,InCubicSpline,InRotParaList,OutVector)
    use TAN_NumericAlgebra
    implicit none
    real::InVar,InRotParaList(:),OutVector(:)
    type(Spline_Tan)::InCubicSpline
    call CalCubicSplineVectorWithExpand_Tan(InVar,InCubicSpline,OutVector)
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
    !点到曲线单点距离计算辅助函数,为了后面求解，曲面参数和点通过common参数传递,只适用于3维！
    real function AuxiliaryGetPointToCubicSplinePointForL2DistancePower_Tan(InVar,InFuncObjPrtList) result (ans)
    use TAN_NumericAlgebra
    use blas95
    implicit none
    real::InVar(:)
    type(c_ptr),optional::InFuncObjPrtList(:)  
    type(Spline_Tan),pointer:: TempFromSplineObjPtr
    real,pointer::TempMetricPoint(:),TempRotParaList(:)
    real,pointer::pTempVector(:)
    allocate(pTempVector(3))
    call c_f_pointer(InFuncObjPrtList(1), TempFromSplineObjPtr) 
    call c_f_pointer(InFuncObjPrtList(2), TempMetricPoint,[3]) 
    !默认有旋转运动
    call c_f_pointer(InFuncObjPrtList(3),TempRotParaList,[7])
    call CalCubicSplineVectorWithRotAndExtend_Tan(InVar(1),TempFromSplineObjPtr,TempRotParaList,pTempVector)
    call axpy(TempMetricPoint,pTempVector,-1d0)
    ans=dot(pTempVector,pTempVector)
    deallocate(pTempVector)
    end function
    !!点到曲面距离,返回值=欧几里得距离的平方，OutDistanceAndValue(1)=U值，OutDistanceAndValue(2)=V值
    real function FindPointToCubicSplineMinL2DistancePower_Tan(InPoint,InFromCubicSpline,IOVar,InRotParaList) result(ans)
    use TAN_NumericOptimization
    implicit none
    real::InPoint(:)    
    type(Spline_Tan)::InFromCubicSpline
    real::IOVar
    real,optional::InRotParaList(:)
    type(c_ptr),pointer::pTempFuncObjPrtList(:)  
    real,pointer::pTempVarRanges(:,:),pTempVar(:)
    allocate(pTempFuncObjPrtList(3))
    allocate(pTempVarRanges(2,1))
    allocate(pTempVar(1))
    !对参数进行打包
    if(present(InRotParaList)) then        
        pTempFuncObjPrtList(3)=c_loc(InRotParaList)
    else
        pTempFuncObjPrtList(3)=c_loc(DefaultRotParaList)   
    end if
    pTempFuncObjPrtList(1)=c_loc(InFromCubicSpline)
    pTempFuncObjPrtList(2)=c_loc(InPoint)
    pTempVarRanges(:,1)=InFromCubicSpline%OriginPointPara([1,InFromCubicSpline%PointNum])
    pTempVar(1)=IOVar
    ans=FindMinValueWithVarRanges_Tan(AuxiliaryGetPointToCubicSplinePointForL2DistancePower_Tan,&
        pTempVar,&
        [1],&
        pTempVarRanges,&
        pTempFuncObjPrtList)
    IOVar=pTempVar(1)
    deallocate(pTempVar)
    deallocate(pTempFuncObjPrtList)
    deallocate(pTempVarRanges)
    return
    end function

    
    
    !测试函数
    subroutine FTestCubicCurve(InTestPointNum,InCurveObj,OutPointList)
    use TAN_BasicFunction
    implicit none
    integer::InTestPointNum
    type(Spline_Tan)::InCurveObj
    real,pointer::pTestPara(:)
    real::OutPointList(:,:)
    integer::i,TempPointNum
    allocate(pTestPara(InTestPointNum))
    TempPointNum=InCurveObj%PointNum
    call BuildArithmeticList_Tan(InCurveObj%OriginPointPara(1),InCurveObj%OriginPointPara(TempPointNum),pTestPara)
    do i=1,InTestPointNum
        call CalCubicSplineInterpVector_Tan(pTestPara(i),InCurveObj,OutPointList(:,i))
    end do  
    deallocate(pTestPara)
    end subroutine
    !!!3次样条曲线求根函数
    !real function FindCurveRootWithDerive_Tan(InValueGuess,InSpline,InEquFun,InEquCoef)
    !use TAN_NumericSolveEqu
    !implicit none
    !real::InValueGuess,InEquCoef(:)
    !type(Spline_Tan),target:: InSpline
    !external::InEquFun
    !!获取残差函数参数 
    !CubicSplineEquTransPtr=>InSpline
    !FindCurveRootWithDerive_Tan=SolveEquByNewtonMethodWithDerive(InValueGuess,InEquFun,InEquCoef)
    !return
    !end function
    
    end module
    

    
    !subroutine CalResidualTestFunc(EquCoefNum,t,EquCoef,OutFunValue,OutDFunValue)
    !use TAN_CubicSpline
    !implicit none
    !integer::EquCoefNum
    !real::t,EquCoef(EquCoefNum)
    !real::OutFunValue,OutDFunValue
    !real,pointer::TempVector(:,:)
    !allocate(TempVector(CubicSplineEquTransPtr%PointDimension,2))
    !call CalCubicSplineVectorWithExpand_Tan(t,CubicSplineEquTransPtr,TempVector(:,1))
    !call CalCubicSplineVector1DeriveWithExpand_Tan(t,CubicSplineEquTransPtr,TempVector(:,2))
    !OutFunValue = TempVector(1,1)**2-EquCoef(1)
    !OutDFunValue=2*(TempVector(1,1)-EquCoef(1))*TempVector(1,2)
    !deallocate(TempVector)
    !return
    !end subroutine
