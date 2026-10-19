
    module TAN_NumericIntegral
    use iso_c_binding, only: c_ptr
    implicit none
    !单个高斯积分域对象
    type::GaussIntegralPara_Tan
        integer::IntegralPointCount!积分点数
        real,pointer::GaussCoorList(:)!积分坐标，由于积分坐标可能改变，因此需要进行动态调整，而权重则由积分点唯一确定
        real::JacobiValue
        integer::CreatMethod=0!如果是1，则为已动态创建，否则为0
    end type
    !含子域积分的高斯积分域对象
    type::GaussIntegralParaWithSubRegion_Tan
        integer::SubRegionCount!子域数
        type(GaussIntegralPara_Tan),pointer::GaussIntegralParaObjList(:)!单个子域的高斯积分参数列表
        integer::CreatMethod=0!如果是1，则为已动态创建，否则为0
    end type
    !高斯积分系数,分别对应不同高斯积分点数的坐标和权重，最后1维对应积分点总数，倒数第二维对应第几个积分点
    real,target::GaussIntegralCoef(10)=&
        [0d0,&!1高斯积分点
         -0.577350269d0, 0.577350269d0,&!2高斯积分点
         -0.7745966692414833770358531d0, 0d0, 0.7745966692414833770358531d0, &!3高斯积分点
         -0.8611363115940525752239465d0, -0.3399810435848562648026658d0, 0.3399810435848562648026658, 0.8611363115940525752239465d0]!4高斯积分点
    !高斯积分权重,分别对应不同高斯积分点数的坐标和权重，最后1维对应积分点总数，倒数第二维对应第几个积分点
    real,target::GaussIntegralWeight(10)=&
        [2d0,&!1高斯积分点
         1d0, 1d0,&!2高斯积分点
         0.5555555555555555555555556d0, 0.8888888888888888888888889d0, 0.5555555555555555555555556d0,&!3高斯积分点
         0.3478548451374538573730639d0, 0.6521451548625461426269361d0, 0.6521451548625461426269361d0, 0.3478548451374538573730639d0]!4高斯积分点
    integer,target::IntegralStartIndex(4)=[0,1,3,6]
    integer,parameter::DefaultGaussPointCount=4

    contains
    
    !删除高斯积分参数空间
    subroutine DeleteGaussIntegralParaSpace_Tan(IOGaussIntegralParaObj)
    implicit none
    type(GaussIntegralPara_Tan)::IOGaussIntegralParaObj
    if (IOGaussIntegralParaObj%CreatMethod==1) then
        deallocate(IOGaussIntegralParaObj%GaussCoorList)
        IOGaussIntegralParaObj%CreatMethod=0
    end if    
    end subroutine 
    !创建高斯积分参数空间
    subroutine CreatGaussIntegralParaSpace_Tan(InIntegralPointCount,IOGaussIntegralParaObj)
    implicit none
    integer:: InIntegralPointCount
    type(GaussIntegralPara_Tan)::IOGaussIntegralParaObj
    call DeleteGaussIntegralParaSpace_Tan(IOGaussIntegralParaObj)
    IOGaussIntegralParaObj%IntegralPointCount=InIntegralPointCount
    allocate (IOGaussIntegralParaObj%GaussCoorList(InIntegralPointCount))
    IOGaussIntegralParaObj%CreatMethod=1
    end subroutine
    
    
    !删除含子域的高斯积分参数空间
    subroutine DeleteGaussIntegralParaWithSubRegionSpace_Tan(IOGaussIntegralParaWithSubRegionObj)
    implicit none
    type(GaussIntegralParaWithSubRegion_Tan)::IOGaussIntegralParaWithSubRegionObj
    integer::i
    if (IOGaussIntegralParaWithSubRegionObj%CreatMethod==1) then
        do i=1,IOGaussIntegralParaWithSubRegionObj%SubRegionCount
            call DeleteGaussIntegralParaSpace_Tan(IOGaussIntegralParaWithSubRegionObj%GaussIntegralParaObjList(i))
        end do
        deallocate(IOGaussIntegralParaWithSubRegionObj%GaussIntegralParaObjList)
        IOGaussIntegralParaWithSubRegionObj%CreatMethod=0
    end if    
    end subroutine
    !创建含子域的高斯积分参数空间
    subroutine CreateGaussIntegralParaWithSubRegionSpace_Tan(InSubReginCount,InSubReginGaussPointCountList,IOGaussIntegralParaWithSubRegionObj)
    implicit none    
    integer::InSubReginCount,InSubReginGaussPointCountList(:)
    type(GaussIntegralParaWithSubRegion_Tan)::IOGaussIntegralParaWithSubRegionObj
    integer::i
    call DeleteGaussIntegralParaWithSubRegionSpace_Tan(IOGaussIntegralParaWithSubRegionObj)
    IOGaussIntegralParaWithSubRegionObj%SubRegionCount=InSubReginCount
    allocate(IOGaussIntegralParaWithSubRegionObj%GaussIntegralParaObjList(InSubReginCount))
    if (size(InSubReginGaussPointCountList)==1) then!只有子域的gauss积分数只有1个值，则所有的子域高斯积分都用这个值
        do i=1,InSubReginCount            
            call CreatGaussIntegralParaSpace_Tan(InSubReginGaussPointCountList(1),IOGaussIntegralParaWithSubRegionObj%GaussIntegralParaObjList(i))
        end do
    else
        do i=1,InSubReginCount            
            call CreatGaussIntegralParaSpace_Tan(InSubReginGaussPointCountList(i),IOGaussIntegralParaWithSubRegionObj%GaussIntegralParaObjList(i))
        end do    
    end if
    IOGaussIntegralParaWithSubRegionObj%CreatMethod=1
    end subroutine
    !根据积分上下限和高斯积分点数输出线性变换的高斯坐标和雅可比值
    subroutine CalGaussIntegralParaForLinearConvert_Tan(InX0,InX1,IOGaussIntegralParaObj,InOptNotConvertedIndex)
    implicit none
    real::InX0,InX1
    type(GaussIntegralPara_Tan)::IOGaussIntegralParaObj
    integer,optional::InOptNotConvertedIndex!不进行积分点变换标识
    real::b
    integer::TempGaussStartIndex,TempGaussEndIndex
    TempGaussStartIndex=IntegralStartIndex(IOGaussIntegralParaObj%IntegralPointCount)+1
    TempGaussEndIndex=IntegralStartIndex(IOGaussIntegralParaObj%IntegralPointCount)+IOGaussIntegralParaObj%IntegralPointCount
    if(present(InOptNotConvertedIndex)) then
        IOGaussIntegralParaObj%JacobiValue=1d0
        IOGaussIntegralParaObj%GaussCoorList(1:IOGaussIntegralParaObj%IntegralPointCount)=&
            GaussIntegralCoef(TempGaussStartIndex:TempGaussEndIndex)
    else
        IOGaussIntegralParaObj%JacobiValue=(0.5d0)*(InX1-InX0);
        b=(0.5d0)*(InX1+InX0);
        IOGaussIntegralParaObj%GaussCoorList(1:IOGaussIntegralParaObj%IntegralPointCount)=&
            IOGaussIntegralParaObj%JacobiValue*GaussIntegralCoef(TempGaussStartIndex:TempGaussEndIndex)+b
    end if
    

    end subroutine
    
    !将高斯积分点系数转换为对应的原函数值
    real function FConvertGaussPointToFunctionRealVar_Tan(InGaussIntegralValue,InOriginVarMin,InOriginVarMax) result(ans)
    implicit none
    real::InGaussIntegralValue,InOriginVarMin,InOriginVarMax
    ans=((1d0-InGaussIntegralValue)*InOriginVarMin+(1d0+InGaussIntegralValue)*InOriginVarMax)/2
    return
    end function
    !根据函数进行高斯积分,可设置为标准或非标准
    real function IntegralScalarFunction_Tan(InVarList,InFunction,InIntegralVarIndex,InFuncObjPrtList,InIntegralRange,InGaussPointCount) result(ans)
    use TAN_NumericDiff,only:DefaultObjPrtList
    use blas95
    implicit none
    real::InVarList(:)
    integer::InIntegralVarIndex
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    real,optional::InIntegralRange(:)
    integer,optional::InGaussPointCount
    integer::TempGaussPointCount,TempGaussPointOffset
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    real,pointer::pTempVarList(:),pIntegralVarValue(:)
    integer::i
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    allocate(pTempVarList(size(InVarList)))
    call copy(InVarList,pTempVarList)  
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if(present(InGaussPointCount)) then
        TempGaussPointCount=InGaussPointCount
    else        
        TempGaussPointCount=DefaultGaussPointCount
    end if
    ans=0
    TempGaussPointOffset=IntegralStartIndex(TempGaussPointCount)
    allocate(pIntegralVarValue(TempGaussPointCount))
    if(present(InIntegralRange)) then!如果不是标准高斯积分，求取标准高斯积分点对应的系数
        do i=1,TempGaussPointCount
            pIntegralVarValue(i)=FConvertGaussPointToFunctionRealVar_Tan(GaussIntegralCoef(TempGaussPointOffset+i),InIntegralRange(1),InIntegralRange(2))
        end do
    else        
       call copy(GaussIntegralCoef((TempGaussPointOffset+1):(TempGaussPointOffset+TempGaussPointCount)),pIntegralVarValue)       
    end if 
    do i=1,TempGaussPointCount
        pTempVarList(InIntegralVarIndex)=pIntegralVarValue(i)
        ans=ans+InFunction(pTempVarList,TempFuncObjPrtList)*GaussIntegralWeight(TempGaussPointOffset+i)
    end do
    if(present(InIntegralRange)) then!如果不是标准高斯积分，求取标准高斯积分点对应的系数
        ans=ans*(InIntegralRange(2)-InIntegralRange(1))/2
    end if
    deallocate(pTempVarList)
    deallocate(pIntegralVarValue)
    return
    end function
    ! 子域划分进行积分
    real function IntegralScalarFunctionWithSubregion_Tan(InSubregionCount,InVarList,InFunction,InIntegralVarIndex,InFuncObjPrtList,InIntegralRange,InGaussPointCount) result(ans)
    use TAN_NumericDiff,only:DefaultObjPrtList
    use blas95
    implicit none
    integer::InSubregionCount
    real::InVarList(:)
    integer::InIntegralVarIndex
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    real,optional::InIntegralRange(:)
    integer,optional::InGaussPointCount
    integer::TempGaussPointCount    
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    real::TempMaxVar,TempMinVar,TempVarIncre
    integer::i
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if(present(InGaussPointCount)) then
        TempGaussPointCount=InGaussPointCount
    else        
        TempGaussPointCount=DefaultGaussPointCount
    end if    
    if(present(InIntegralRange)) then!如果不是标准高斯积分，求取标准高斯积分点对应的系数
        TempMaxVar=InIntegralRange(2)
        TempMinVar=InIntegralRange(1)
    else        
        TempMaxVar=1d0
        TempMinVar=-1d0    
    end if 
    !生成积分区间列表
    TempVarIncre=(TempMaxVar-TempMinVar)/InSubregionCount
    ans=0
    do i=1,InSubregionCount
        ans=ans+IntegralScalarFunction_Tan(InVarList,InFunction,InIntegralVarIndex,TempFuncObjPrtList,&
            [TempMinVar+(i-1)*TempVarIncre,TempMinVar+i*TempVarIncre],&!积分区间
            TempGaussPointCount)    
    end do    
    end function
    !测试函数
    real function FTestIntegral(InABRatio,InOtherCoef) result(ans)
    implicit none    
    real::InABRatio(:)
    type(c_ptr),optional::InOtherCoef(:)
    ans=sqrt(1-1d0/InABRatio(1)**2)
    end function
    end module