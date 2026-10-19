
    module TAN_CubicSpline
    implicit none
    public
    type::Spline_Tan
        integer::PointNum,PointDimension !点数
        real,pointer::OriginPointPara(:) !原始点自变量值
        real,pointer::Cscoef(:,:) !系数,C0,C1,...,D0,D1....
        integer::CreatMethod!如果是1，则为已动态创建，否则为0
    end type Spline_Tan
    type(Spline_Tan),pointer::CubicSplineEquTransPtr
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
    use TAN_BasicNumeric
    use f95_precision
    use blas95
    implicit none
    real::InXValue,InXdata(:),InCscoef(:)
    integer::XValueIndex
    real::Base1DeriveVector(4)
    if (InXValue<InXdata(1)) then
        XValueIndex=2
    else
        XValueIndex=FindFirstLargerValueIndex(InXValue,InXdata)
    end if
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
    use TAN_BasicNumeric
    use f95_precision
    use blas95
    implicit none
    real::InXValue,InXdata(:),InCscoef(:)
    integer::XValueIndex
    real::Base2DeriveVector(4)
    if (InXValue<InXdata(1)) then
        XValueIndex=2
    else
        XValueIndex=FindFirstLargerValueIndex(InXValue,InXdata)
    end if
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
    use TAN_BasicNumeric
    implicit none
    real::InXValue,InXdata(:)
    if (InXValue<InXdata(1)) then
        FindSplineVariantIndex=2
    else
        FindSplineVariantIndex=FindFirstLargerValueIndex(InXValue,InXdata)
    end if
    return
    end function    
    !计算3维样条系数
    subroutine BuildCubicSplineObject_Tan(InPointPara,InPointVector,IOSpline)
    implicit none
    type(Spline_Tan)::IOSpline
    real::InPointPara(:),InPointVector(:,:)
    integer::i,VectorDimension
    VectorDimension=size(InPointVector,1)
    do i=1,VectorDimension
        call Cal1DCubicSplineCoef(InPointPara,&
            InPointVector(i,:),&
            IOSpline%Cscoef(i,:))
    end do
    IOSpline%OriginPointPara=InPointPara
    return
    end subroutine
    !计算非线性外延3维样条值
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
        call CalExtendVector((InPara-InSpline%OriginPointPara(1)),&
            InSpline%Cscoef(:,1),&
            InSpline%Cscoef(:,2),&
            OutVector)
    else
        call CalExtendVector((InPara-InSpline%OriginPointPara(InSpline%PointNum)),&
            InSpline%Cscoef(:,2*InSpline%PointNum-1),&
            InSpline%Cscoef(:,2*InSpline%PointNum-1),&
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
    !!3次样条曲线求根函数
    real function FindCurveRootWithDerive_Tan(InValueGuess,InSpline,InEquFun,InEquCoef)
    use TAN_BasicNumeric
    implicit none
    real::InValueGuess,InEquCoef(:)
    type(Spline_Tan),target:: InSpline
    external::InEquFun
    !获取残差函数参数 
    CubicSplineEquTransPtr=>InSpline
    FindCurveRootWithDerive_Tan=SolveEquByNewtonMethodWithDerive(InValueGuess,InEquFun,InEquCoef)
    return
    end function
    
    end module
    

    
    subroutine CalResidualTestFunc(EquCoefNum,t,EquCoef,OutFunValue,OutDFunValue)
    use TAN_CubicSpline
    implicit none
    integer::EquCoefNum
    real::t,EquCoef(EquCoefNum)
    real::OutFunValue,OutDFunValue
    real,pointer::TempVector(:,:)
    allocate(TempVector(CubicSplineEquTransPtr%PointDimension,2))
    call CalCubicSplineVectorWithExpand_Tan(t,CubicSplineEquTransPtr,TempVector(:,1))
    call CalCubicSplineVector1DeriveWithExpand_Tan(t,CubicSplineEquTransPtr,TempVector(:,2))
    OutFunValue = TempVector(1,1)**2-EquCoef(1)
    OutDFunValue=2*(TempVector(1,1)-EquCoef(1))*TempVector(1,2)
    deallocate(TempVector)
    return
    end subroutine
