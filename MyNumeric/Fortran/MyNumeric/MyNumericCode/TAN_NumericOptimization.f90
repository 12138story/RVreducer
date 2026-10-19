    module TAN_NumericOptimization
    use iso_c_binding, only: c_ptr
    implicit none
    real,pointer::DefaultConstList(:)
    !type(c_ptr),pointer:: DefaultObjPrtList(:)    
    !real,target::DefaultAbsolueError=1d-6
    integer,target::DefaultMaxCycleNum=100
    
    contains
    !数值最优化方法,多元单值函数
    !牛顿法-单步
    subroutine FindMinValueByNewtonOneStep_Tan(InGradVector,InGrad2InvMatrix,IOVarList)
    use blas95
    implicit none
    real::InGrad2InvMatrix(:,:),InGradVector(:)
    real,intent(out)::IOVarList(:)
    call gemv(InGrad2InvMatrix,InGradVector,IOVarList,-1d0,1d0)
    end subroutine
    !
    !
    !最优梯度法-单步
    subroutine FindMinValueByOptimumGradientOneStep_Tan(InGradVector,InGrad2Matrix,IOVarList)
    use blas95
    implicit none
    real::IOVarList(:),InGradVector(:),InGrad2Matrix(:,:)
    real,pointer::pTempVector(:)
    real::Alpha
    allocate(pTempVector(size(IOVarList)))
    call gemv(InGrad2Matrix,InGradVector,pTempVector)
    Alpha=dot(InGradVector,InGradVector)/dot(pTempVector,InGradVector)    
    call axpy(InGradVector,IOVarList,-Alpha)
    deallocate(pTempVector)
    end subroutine
    !共轭梯度法-单步
    subroutine FindMinValueByConjugateGradientOneStep_Tan(InGradVector,InGrad2Matrix,InSearchDirect,IOVarList)
    use blas95
    implicit none
    real::IOVarList(:),InGradVector(:),InGrad2Matrix(:,:),InSearchDirect(:)
    real,pointer::pTempVector(:)
    real::Lamada
    allocate(pTempVector(size(IOVarList)))
    call gemv(InGrad2Matrix,InSearchDirect,pTempVector)
    Lamada=dot(InGradVector,InSearchDirect)/dot(pTempVector,InSearchDirect)    
    call axpy(InSearchDirect,IOVarList,-Lamada)
    deallocate(pTempVector)
    end subroutine
    !共轭梯度法-方向
    subroutine FindConjugateGradientDirect_Tan(InGradVector0,InGradVector1,IOSearchDirect)
    use blas95
    implicit none
    real::InGradVector0(:),InGradVector1(:),IOSearchDirect(:)
    real::Alpha
    Alpha=dot(InGradVector1,InGradVector1)/dot(InGradVector0,InGradVector0)
    call scal(IOSearchDirect,Alpha)
    call axpy(InGradVector1,IOSearchDirect,-1d0)
    end subroutine
    !优化程序集合,InOptimumMethod=1-牛顿法，2-最优梯度法，3-共轭梯度法
    !
    real function FindMinValueWithoutConstrain_Tan(InFunction,IOVarList,&
        InFuncObjPrtList,InVarDiffList,InAbsolueError,InMaxCycleNum,InOptimumMethod,&
        InOptimumVarIndex,&
        OutOptErrorNotation)
    use blas95
    use Tan_BasicCoef
    use Tan_NumericDiff
    use TAN_BasicFunction
    use TAN_NumericAlgebra
    implicit none
    real,target::IOVarList(:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    real,optional::InAbsolueError
    integer,optional,target::InMaxCycleNum,InOptimumMethod
    integer,optional,target::InOptimumVarIndex(:)
    integer,optional,target::OutOptErrorNotation!错误代码，0-未出错，1-迭代次数超限
    integer,pointer::TempOptimumVarIndex(:)
    real,pointer::TempVarDiffList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)    
    real::TempAbsolueError
    integer,pointer::TempMaxCycleNum
    integer::TempOptimumMethod=1    
    integer::VarCount,i,OptimumVarCount
    real,pointer::pTempValue(:),pGradVector(:),pGrad2Matrix(:,:)
    real,pointer::GradVectorPointer0(:),GradVectorPointer1(:),SearchDirectPointer(:),TempGradVectorPointer(:)
    real,pointer::pTempOptimumVarList(:)
    integer,pointer::TempErrorPtr
    integer,target::TempErrorCode
    !integer::testInt
    !real,pointer::TestMatrix(:,:),TestVarList(:)
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    VarCount=size(IOVarList)
    if(present(InOptimumVarIndex)) then
        TempOptimumVarIndex=>InOptimumVarIndex
    else        
        TempOptimumVarIndex=>DafaultDerivedVarIndex(1:VarCount)
    end if
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if (size(TempOptimumVarIndex)==0) then    
        FindMinValueWithoutConstrain_Tan=InFunction(IOVarList,TempFuncObjPrtList)!所有的变量都不需要优化
    else
        if(present(InAbsolueError)) then
            TempAbsolueError=InAbsolueError
        else
            TempAbsolueError=DefaultAbsolueError
        end if
        if(present(InMaxCycleNum)) then
            TempMaxCycleNum=>InMaxCycleNum
        else
            TempMaxCycleNum=>DefaultMaxCycleNum
        end if
        if(present(InOptimumMethod)) then
            TempOptimumMethod=InOptimumMethod
        else
            TempOptimumMethod=1
        end if

        if(present(InVarDiffList)) then
            TempVarDiffList=>InVarDiffList
        else        
            TempVarDiffList=>DefaultVarDiff(1:VarCount)
        end if  
        if(present(OutOptErrorNotation))then
            TempErrorPtr=>OutOptErrorNotation
        else
            TempErrorPtr=>TempErrorCode
        end if
        TempErrorPtr=0
        OptimumVarCount=size(TempOptimumVarIndex)
        allocate(pTempValue(OptimumVarCount))
        call copy(IOVarList(TempOptimumVarIndex),pTempValue)   
        if(TempOptimumMethod==3) then
            allocate(pGradVector(3*OptimumVarCount))
            TempGradVectorPointer=>pGradVector(1:OptimumVarCount)
            GradVectorPointer1=>pGradVector(OptimumVarCount+1:2*OptimumVarCount)
            SearchDirectPointer=>pGradVector(2*OptimumVarCount+1:3*OptimumVarCount)
            GradVectorPointer0=>SearchDirectPointer
        else
            allocate(pGradVector(OptimumVarCount))
            GradVectorPointer0=>pGradVector(1:OptimumVarCount)
        end if    
        allocate(pGrad2Matrix(OptimumVarCount,OptimumVarCount)) 
        !allocate(TestMatrix(OptimumVarCount,OptimumVarCount))
        !allocate(TestVarList(OptimumVarCount))    
        if(OptimumVarCount==VarCount) then
            pTempOptimumVarList=>IOVarList
        else
            allocate(pTempOptimumVarList(OptimumVarCount)) 
            pTempOptimumVarList=IOVarList(TempOptimumVarIndex)
        end if 
        do i=1,TempMaxCycleNum
            !TestVarList=TempOptimumVarList
            call CalGradientForScalarFunction_Tan(IOVarList,InFunction,GradVectorPointer0,TempFuncObjPrtList,TempVarDiffList,TempOptimumVarIndex)
            call CalGradient2ForScalarFunction_Tan(IOVarList,InFunction,pGrad2Matrix,TempFuncObjPrtList,TempVarDiffList,TempOptimumVarIndex)
            !TestMatrix=pGrad2Matrix
            select case(TempOptimumMethod)
            case(1)
                call CalMatrixInverse_Tan(pGrad2Matrix,pGrad2Matrix)
                call FindMinValueByNewtonOneStep_Tan(GradVectorPointer0,pGrad2Matrix,pTempOptimumVarList)
            case(2)
                call FindMinValueByOptimumGradientOneStep_Tan(GradVectorPointer0,pGrad2Matrix,pTempOptimumVarList)
            case(3)
                call FindMinValueByConjugateGradientOneStep_Tan(GradVectorPointer0,pGrad2Matrix,SearchDirectPointer,pTempOptimumVarList)
            end select
            if(OptimumVarCount.NE.VarCount) then
                IOVarList(TempOptimumVarIndex)=pTempOptimumVarList
            end if
            select case(TempOptimumMethod)
            case(3)            
                call CalGradientForScalarFunction_Tan(IOVarList,InFunction,GradVectorPointer1,TempFuncObjPrtList,TempVarDiffList,TempOptimumVarIndex)
                call FindConjugateGradientDirect_Tan(GradVectorPointer0,GradVectorPointer1,SearchDirectPointer)
                GradVectorPointer0=>GradVectorPointer1
                GradVectorPointer1=>TempGradVectorPointer
                TempGradVectorPointer=>GradVectorPointer0
            end select        
            if(CalInfiniteNormOfVector_Tan(pTempValue-pTempOptimumVarList)<TempAbsolueError) then
                exit
            else
                call copy(pTempOptimumVarList,pTempValue)
            end if
            if(any(pTempOptimumVarList>1.0/DefaultAbsolueError)) then
                exit
            end if
        end do
        FindMinValueWithoutConstrain_Tan=InFunction(IOVarList,TempFuncObjPrtList)
        if (i>TempMaxCycleNum) then
            write(6,*) 'FindMinValue_TanByNewton未在设定循环次数内达到指定容差'
            TempErrorPtr=1
            !write(6,*)  TempErrorPtr
        end if        
        deallocate(pTempValue)
        deallocate(pGradVector)
        deallocate(pGrad2Matrix)
        !deallocate(TestMatrix)
        !deallocate(TestVarList)
        if(OptimumVarCount.NE.VarCount) then
            deallocate(pTempOptimumVarList)
        end if
    end if
    return
    end function
    !!带自变量范围限制的优化方法
    !根据自变量上下限，生成全部边界值序列
    subroutine GenBoundVarSetList(InVarRangeValue,pOutVarSetList)!注意，pOutVarSetList需要由上层函数释放
    use Tan_BasicCoef
    use TAN_BasicFunction
    implicit none
    real,target::InVarRangeValue(:,:)!2*n数组，分别1行下限，2行上限
    real,pointer::pOutVarSetList(:,:)
    integer::TempTotolVarCount
    integer,pointer::pVar1BoundCountList(:),pVar2BoundCountList(:)
    integer,pointer::pTempVar2BoundIndexList(:,:)
    real,pointer::pVar1BoundValueList(:)
    integer::i,j,TempTwoBoundVarCount,TempOneBoundVarCount,TempTotalCombinaCount
    !判断哪些只有上限或下限
    TempTotolVarCount=size(InVarRangeValue,2)
    allocate(pVar2BoundCountList(TempTotolVarCount))
    allocate(pVar1BoundCountList(TempTotolVarCount))
    allocate(pVar1BoundValueList(TempTotolVarCount))
    TempTwoBoundVarCount=0
    TempOneBoundVarCount=0
    do i=1,TempTotolVarCount
        if(abs(InVarRangeValue(1,i)).eq.DefaultInfinity) then
            TempOneBoundVarCount=TempOneBoundVarCount+1
            pVar1BoundCountList(TempOneBoundVarCount)=i
            pVar1BoundValueList(TempOneBoundVarCount)=InVarRangeValue(2,i)
        else
            if (abs(InVarRangeValue(2,i)).eq.DefaultInfinity) then
                TempOneBoundVarCount=TempOneBoundVarCount+1
                pVar1BoundCountList(TempOneBoundVarCount)=i
                pVar1BoundValueList(TempOneBoundVarCount)=InVarRangeValue(1,i)
            else
                TempTwoBoundVarCount=TempTwoBoundVarCount+1
                pVar2BoundCountList(TempTwoBoundVarCount)=i
            end if      
        end if   
    end do 
    TempTotalCombinaCount=2**TempTwoBoundVarCount
    allocate(pOutVarSetList(TempTotolVarCount,TempTotalCombinaCount))
    if(TempTotalCombinaCount.ne.1) then
         allocate(pTempVar2BoundIndexList(TempTwoBoundVarCount,TempTotalCombinaCount))
         call GenBase2VarCombinaList_Tan(pTempVar2BoundIndexList)
         do i=1,TempTotalCombinaCount
             do j=1,TempTwoBoundVarCount
                 pOutVarSetList(pVar2BoundCountList(j),i)=InVarRangeValue(pTempVar2BoundIndexList(j,i),pVar2BoundCountList(j))
             end do
             pOutVarSetList(pVar1BoundCountList(1:TempOneBoundVarCount),i)=pVar1BoundValueList(1:TempOneBoundVarCount)
         end do  
         deallocate(pTempVar2BoundIndexList)
    else
        pOutVarSetList(:,1)=pVar1BoundValueList
    end if
    deallocate(pVar2BoundCountList)
    deallocate(pVar1BoundCountList)
    deallocate(pVar1BoundValueList)
    end subroutine
    !根据约束变量生成优化变量列表
    subroutine GenOptimumVarIndexByFixedIndex_Tan(InFixIndex,OutOptimumIndex,InPlusFixIndex)
    implicit none
    integer,target::InFixIndex(:),OutOptimumIndex(:)
    integer,optional,target::InPlusFixIndex(:)
    integer::TotalVarCount,i,TempIndex
    if(present(InPlusFixIndex)) then
        TotalVarCount=size(InFixIndex)+size(OutOptimumIndex)+size(InPlusFixIndex)
        TempIndex=0
        do i=1,TotalVarCount
            if(all(InFixIndex.ne.i).and.all(InPlusFixIndex.ne.i)) then
                TempIndex=TempIndex+1
                OutOptimumIndex(TempIndex)=i
            end if   
        end do   
    else
        TotalVarCount=size(InFixIndex)+size(OutOptimumIndex)
        TempIndex=0
        do i=1,TotalVarCount
            if(all(InFixIndex.ne.i)) then
                TempIndex=TempIndex+1
                OutOptimumIndex(TempIndex)=i
            end if   
        end do   
    end if  
    return
    end subroutine
    !判断变量是否符合约束
    logical function IsVarListInSetRange_Tan(InVarList,InVarRangeList) result(ans)
    implicit none
    integer::i
    real::InVarList(:),InVarRangeList(:,:)    
    ans=.true.
    do i=1,size(InVarList)
        if((InVarList(i).lt.InVarRangeList(1,i)).or.(InVarList(i).gt.InVarRangeList(2,i)).or.isNan(InVarList(i))) then
            ans=.false.
        end if        
    end do
    return
    end function
    !带变量范围约束的优化算法,注意，没有加约束检验，未检验固定变量值是否与范围约束值设为了一样
    real function FindMinValueWithVarRanges_Tan(InFunction,IOVarList,&
        InVarRangeIndexList,InVarRangeValue,&
        InFuncObjPrtList,InVarDiffList,InAbsolueError,InMaxCycleNum,InOptimumMethod,&
        InFixedVarIndexList,&
        OutOptErrorNotation) result(ans)
    use blas95
    use Tan_NumericDiff
    use Tan_BasicCoef
    use TAN_BasicFunction
    implicit none
    real,target::IOVarList(:)
    integer,target::InVarRangeIndexList(:)
    real,target::InVarRangeValue(:,:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    real,optional::InAbsolueError
    integer,optional,target::InMaxCycleNum,InOptimumMethod
    integer,optional,target::InFixedVarIndexList(:)
    integer,optional,target::OutOptErrorNotation
    integer,pointer::TempFixedVarIndexList(:)
    real,pointer::TempVarDiffList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)    
    real::TempAbsolueError
    integer,pointer::TempMaxCycleNum
    integer::TempOptimumMethod=1    
    integer::VarCount,i,j,k,OptimumVarCount
    integer::TempTotalRangedVarCount
    real,pointer::pTempVarList(:)
    integer,pointer::pTempOptimumVarIndexList(:)
    integer,pointer::pTempSelectedRangedVarIndexList(:,:)
    real,pointer::pTempSelectRangedVarSet(:,:)
    real,pointer::pTempResultVar(:)
    !integer,pointer::TempErrorPtr
    integer,target::TempErrorCode1 !未约束条件下优化错误码
    integer,target::TempErrorCode2,TempErrorCode3 !约束条件下优化错误码
    integer::TempOptValueLocation!值在边界还是在内部，0在内部，1在边界
    real::TempAns
    interface
        real function InFunction(InFValueList,InFFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFFuncObjPrtList(:)
        end function
    end interface
    VarCount=size(IOVarList)
    if(present(InAbsolueError)) then
        TempAbsolueError=InAbsolueError
    else
        TempAbsolueError=DefaultAbsolueError
    end if
    if(present(InMaxCycleNum)) then
        TempMaxCycleNum=>InMaxCycleNum
    else
        TempMaxCycleNum=>DefaultMaxCycleNum
    end if
    if(present(InOptimumMethod)) then
        TempOptimumMethod=InOptimumMethod
    else
        TempOptimumMethod=1
    end if
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if(present(InVarDiffList)) then
        TempVarDiffList=>InVarDiffList
    else        
        TempVarDiffList=>DefaultVarDiff(1:VarCount)
    end if    
    if(present(InFixedVarIndexList)) then
        TempFixedVarIndexList=>InFixedVarIndexList
    else
        allocate(TempFixedVarIndexList(0))
    end if  
    !if(present(OutOptErrorNotation)) then
    !    TempErrorPtr=>OutOptErrorNotation
    !else
    !    TempErrorPtr=>TempErrorCode
    !end if
    TempErrorCode1=0
    TempErrorCode2=0
    TempErrorCode3=0
    !先进行全局优化，判断是否在范围内
    allocate(pTempVarList(VarCount))
    allocate(pTempResultVar(VarCount))
    call copy(IOVarList,pTempVarList)
    call copy(IOVarList,pTempResultVar)
    !根据固定变量生成优化变量序列
    allocate(pTempOptimumVarIndexList(VarCount-size(TempFixedVarIndexList)))
    call GenOptimumVarIndexByFixedIndex_Tan(TempFixedVarIndexList,pTempOptimumVarIndexList)
    ans=DefaultInfinity
    TempAns=FindMinValueWithoutConstrain_Tan(InFunction,pTempVarList,TempFuncObjPrtList,TempVarDiffList,&
        TempAbsolueError,TempMaxCycleNum,TempOptimumMethod,pTempOptimumVarIndexList,TempErrorCode1)   
    !判断是否满足定义域要求
    if(IsVarListInSetRange_Tan(pTempVarList(InVarRangeIndexList),InVarRangeValue)) then
        call copy(pTempVarList,pTempResultVar)
        ans=TempAns 
        deallocate(pTempOptimumVarIndexList)
        TempOptValueLocation=0
    else
        TempTotalRangedVarCount=size(InVarRangeIndexList)
        deallocate(pTempOptimumVarIndexList)
        do i=1,size(InVarRangeIndexList)            
            allocate(pTempOptimumVarIndexList(VarCount-size(TempFixedVarIndexList)-i))            
            !生成选择约束变量序列
            allocate(pTempSelectedRangedVarIndexList(i,CalBinomial_Tan(TempTotalRangedVarCount,i)))
            call GenBaseCombinaNumList_Tan(TempTotalRangedVarCount,i,pTempSelectedRangedVarIndexList)
            do j=1,size(pTempSelectedRangedVarIndexList,2)                
                !生成需优化的变量序列
                call GenOptimumVarIndexByFixedIndex_Tan(TempFixedVarIndexList,pTempOptimumVarIndexList,&
                    pTempSelectedRangedVarIndexList(:,j))
                !!生成固定变量的取值序列
                call GenBoundVarSetList(InVarRangeValue(:,pTempSelectedRangedVarIndexList(:,j)),&
                    pTempSelectRangedVarSet)
                !计算约束下的最优值
                do k=1,size(pTempSelectRangedVarSet,2)
                    pTempVarList(pTempSelectedRangedVarIndexList(:,j))=pTempSelectRangedVarSet(:,k)
                    TempAns=FindMinValueWithoutConstrain_Tan(InFunction,pTempVarList,TempFuncObjPrtList,TempVarDiffList,&
                        TempAbsolueError,TempMaxCycleNum,TempOptimumMethod,pTempOptimumVarIndexList,TempErrorCode3)
                    if(TempErrorCode3>0) then!由于变值约束有多个，那么有一次约束优化有错误就认为有错
                        TempErrorCode2=1
                    end if
                    
                    if(IsVarListInSetRange_Tan(pTempVarList(InVarRangeIndexList),InVarRangeValue).and.TempAns<ans) then
                        call copy(pTempVarList,pTempResultVar)
                        ans=TempAns
                        TempOptValueLocation=1
                    end if
                    call copy(IOVarList,pTempVarList)
                end do
                deallocate(pTempSelectRangedVarSet)
                !copy(pTempVarList,IOVarList)
            end do            
        !    !call GenBoundVarSetList(InVarRangeValue,pOutVarSetList)
            deallocate(pTempOptimumVarIndexList)
            deallocate(pTempSelectedRangedVarIndexList)
        end do        
    end if
    if(present(OutOptErrorNotation)) then
        if((TempErrorCode1>0) .or. (TempErrorCode2>0)) then
            OutOptErrorNotation=1
        else
            OutOptErrorNotation=0
        end if   
        !if(TempOptValueLocation==0) then
        !    OutOptErrorNotation=TempErrorCode1
        !else
        !    OutOptErrorNotation=TempErrorCode2
        !end if        
    end if
    call copy(pTempResultVar,IOVarList)
    deallocate(pTempVarList)
    deallocate(pTempResultVar)
    if(size(TempFixedVarIndexList)==0) then
        deallocate(TempFixedVarIndexList)
    end if 
    return
    end function
    
        
    end module