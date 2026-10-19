    module TAN_NumericDiff
    use iso_c_binding, only: c_ptr
    implicit none
    real,target::DefaultVarDiff(100)=1d-4
    integer,target::DafaultDerivedVarIndex(100)=([1:100])
    type(c_ptr),pointer:: DefaultObjPrtList(:)

    contains
    !数值微分
    !标量函数偏微分,变量列表，函数名，对第几个参量微分
    real function FPartialDeriveScalarFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndex,InFuncObjPrtList,InVarDiff)
    use blas95
    implicit none
    real::InVarList(:)
    integer::InPartIndex
    real,optional,target::InVarDiff
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    real,pointer::TempVarDiff
    real::TempOriginValue
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    if(present(InVarDiff)) then
        TempVarDiff=>InVarDiff
    else        
        TempVarDiff=>DefaultVarDiff(1)   
    end if
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    TempOriginValue=InVarList(InPartIndex)
    InVarList(InPartIndex)=InVarList(InPartIndex)+TempVarDiff
    FPartialDeriveScalarFunctionByCentralDiff_Tan=InFunction(InVarList,TempFuncObjPrtList)
    InVarList(InPartIndex)=InVarList(InPartIndex)-TempVarDiff*2d0
    FPartialDeriveScalarFunctionByCentralDiff_Tan=(FPartialDeriveScalarFunctionByCentralDiff_Tan-&
        InFunction(InVarList,TempFuncObjPrtList))/(TempVarDiff*2d0)
    InVarList(InPartIndex)=TempOriginValue
    return
    end function
    
    !向量函数偏微分
    subroutine FPartialDeriveVectorFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndex,OutVector,InFuncObjPrtList,InVarDiff)
    use blas95
    implicit none
    real::InVarList(:)
    integer::InPartIndex    
    real::OutVector(:)
    real,optional,target::InVarDiff
    type(c_ptr),optional,target::InFuncObjPrtList(:)     
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    integer::TempVarCount,TempFunctionVarCount
    real,pointer::pBackResult(:)
    real::TempOriginValue
    real,pointer::TempVarDiff
    interface
        subroutine InFunction(InFValueList,OutFVector,InFFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFFuncObjPrtList(:)
            real::OutFVector(:)
        end subroutine
    end interface
    if(present(InVarDiff)) then
        TempVarDiff=>InVarDiff
    else        
        TempVarDiff=>DefaultVarDiff(1)
    end if
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    TempVarCount=size(InVarList)
    TempFunctionVarCount=size(OutVector)
    TempOriginValue=InVarList(InPartIndex)
    InVarList(InPartIndex)=InVarList(InPartIndex)+TempVarDiff
    call InFunction(InVarList,OutVector,TempFuncObjPrtList)
    InVarList(InPartIndex)=InVarList(InPartIndex)-TempVarDiff*2d0
    allocate(pBackResult(TempFunctionVarCount))    
    call InFunction(InVarList,pBackResult,TempFuncObjPrtList) 
    InVarList(InPartIndex)=TempOriginValue
    call axpy(pBackResult,OutVector,-1d0)
    call scal(OutVector,1d0/(2d0*TempVarDiff))
    deallocate(pBackResult)
    return
    end subroutine
    
    
    
    
    
    !标量函数2重偏微分
    real function FPartialDerive2ScalarFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndexList,InFuncObjPrtList,InVarDiffList)
    use blas95
    implicit none
    real::InVarList(:)
    integer::InPartIndexList(:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    real::TempOriginValue
    real,pointer::TempVarDiff(:)
    real::TempResult
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFuncObjPrtList(:)
        end function
    end interface
    if(present(InVarDiffList)) then
        TempVarDiff=>InVarDiffList
    else        
        TempVarDiff=>DefaultVarDiff(1:2)   
    end if
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    TempOriginValue=InVarList(InPartIndexList(2))
    InVarList(InPartIndexList(2))=InVarList(InPartIndexList(2))+TempVarDiff(2)
    TempResult=FPartialDeriveScalarFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndexList(1),TempFuncObjPrtList,TempVarDiff(1))
    InVarList(InPartIndexList(2))=InVarList(InPartIndexList(2))-(TempVarDiff(2)*2d0)
    TempResult=TempResult-FPartialDeriveScalarFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndexList(1),TempFuncObjPrtList,TempVarDiff(1))
    FPartialDerive2ScalarFunctionByCentralDiff_Tan=(TempResult)/(TempVarDiff(2)*2d0)
    InVarList(InPartIndexList(2))=TempOriginValue
    
    return
    end function
    !向量函数2重偏微分
    subroutine FPartialDerive2VectorFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndexList,OutVector,InFuncObjPrtList,InVarDiffList)
    use blas95
    implicit none
    real::InVarList(:)
    integer::InPartIndexList(:)
    real::OutVector(:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    real,pointer::pTempResult(:)
    integer::TempVarCount,TempFunctionVarCount
    real::TempOriginValue
    real,pointer::TempVarDiff(:)
    interface
        subroutine InFunction(InFValueList,OutFVector,InFFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFFuncObjPrtList(:)
            real::OutFVector(:)
        end subroutine
    end interface
    TempFunctionVarCount=size(OutVector)
    allocate(pTempResult(TempFunctionVarCount))
    TempOriginValue=InVarList(InPartIndexList(2))
    if(present(InVarDiffList)) then
        TempVarDiff=>InVarDiffList
    else        
        TempVarDiff=>DefaultVarDiff(1:2)  
    end if
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    InVarList(InPartIndexList(2))=InVarList(InPartIndexList(2))+TempVarDiff(2)
    call FPartialDeriveVectorFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndexList(1),OutVector,TempFuncObjPrtList,TempVarDiff(1))
    InVarList(InPartIndexList(2))=InVarList(InPartIndexList(2))-(TempVarDiff(2)*2d0)
    call FPartialDeriveVectorFunctionByCentralDiff_Tan(InVarList,InFunction,InPartIndexList(1),pTempResult,TempFuncObjPrtList,TempVarDiff(1))
    call axpy(pTempResult,OutVector,-1d0)
    call scal(OutVector,1d0/(TempVarDiff(2)*2d0))
    !end if
    InVarList(InPartIndexList(2))=TempOriginValue        
    deallocate(pTempResult)
    return
    end subroutine
    !
    !求标量函数梯度
    subroutine CalGradientForScalarFunction_Tan(InVarList,InFunction,OutVector,InFuncObjPrtList,InVarDiffList,InDerivedVarIndexList)
    implicit none
    real::InVarList(:),OutVector(:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    integer,optional,target::InDerivedVarIndexList(:)
    integer,pointer::TempDerivedVarIndexList(:)
    integer::TempVarCount,i,TempIndex
    real,pointer::TempVarDiffList(:)
    interface
        real function InFunction(InFValueList,InFFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFFuncObjPrtList(:)
        end function
    end interface
    TempVarCount=size(InVarList)
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    if(present(InVarDiffList)) then
        TempVarDiffList=>InVarDiffList
    else
        TempVarDiffList=>DefaultVarDiff(1:TempVarCount)
    end if    
    
    !生成计算梯度的序列
    if(present(InDerivedVarIndexList)) then
        TempDerivedVarIndexList=>InDerivedVarIndexList
    else
        TempDerivedVarIndexList=>DafaultDerivedVarIndex(1:TempVarCount)
    end if   
    
    do i=1,size(TempDerivedVarIndexList)
        TempIndex=TempDerivedVarIndexList(i)
        OutVector(i)=FPartialDeriveScalarFunctionByCentralDiff_Tan(InVarList,InFunction,TempIndex,TempFuncObjPrtList,TempVarDiffList(TempIndex))
  
    end do
    return
    end subroutine
    !求标量函数2阶梯度
    subroutine CalGradient2ForScalarFunction_Tan(InVarList,InFunction,OutMatrix,InFuncObjPrtList,InVarDiffList,InDerivedVarIndexList)
    implicit none
    real::InVarList(:),OutMatrix(:,:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    integer,optional,target::InDerivedVarIndexList(:)
    integer,pointer::TempDerivedVarIndexList(:)
    integer::TempVarCount,i,j,TempIndex,TempIIndex,TempJIndex
    real,pointer::TempVarDiffList(:)
    interface
        real function InFunction(InFValueList,InFFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFFuncObjPrtList(:)
        end function
    end interface
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList    
    end if
    TempVarCount=size(InVarList)    
    
    if(present(InVarDiffList)) then
        TempVarDiffList=>InVarDiffList
    else
        TempVarDiffList=>DefaultVarDiff(1:TempVarCount)
    end if
    !生成计算梯度的序列
     if(present(InDerivedVarIndexList)) then
        TempDerivedVarIndexList=>InDerivedVarIndexList
     else
         TempDerivedVarIndexList=>DafaultDerivedVarIndex(1:TempVarCount)
    end if        
    !计算对角元素梯度
    do i=1,size(TempDerivedVarIndexList)
        TempIIndex=TempDerivedVarIndexList(i)
        OutMatrix(i,i)=FPartialDerive2ScalarFunctionByCentralDiff_Tan(&
            InVarList,InFunction,[TempIIndex,TempIIndex],TempFuncObjPrtList,[TempVarDiffList(TempIIndex),TempVarDiffList(TempIIndex)])
    end do     
    TempJIndex=2
    do j=2,size(TempDerivedVarIndexList)
        TempJIndex=TempDerivedVarIndexList(j)
        do i=1,j-1
            TempIIndex=TempDerivedVarIndexList(i)
            OutMatrix(i,j)=FPartialDerive2ScalarFunctionByCentralDiff_Tan(InVarList,InFunction,&
                [TempIIndex,TempJIndex],TempFuncObjPrtList,[TempVarDiffList(TempIIndex),TempVarDiffList(TempJIndex)])
            OutMatrix(j,i)=OutMatrix(i,j)
        end do  
    end do
    return
    end subroutine
    !
    !!求向量函数梯度,未添加变量约束程序
    subroutine CalGradientForVectorFunction_Tan(InVarList,InFunction,OutMatrix,InFuncObjPrtList,InVarDiffList)
    implicit none
    real::InVarList(:),OutMatrix(:,:)
    real,optional,target::InVarDiffList(:)
    type(c_ptr),optional,target::InFuncObjPrtList(:)
    type(c_ptr),pointer::TempFuncObjPrtList(:)
    integer::TempVarCount,i
    real,pointer::TempVarDiffList(:)
    interface
        subroutine InFunction(InFValueList,OutFVector,InFFuncObjPrtList)
        use iso_c_binding
            implicit none
            real::InFValueList(:)   
            type(c_ptr),optional::InFFuncObjPrtList(:)
            real::OutFVector(:)
        end subroutine
    end interface
    TempVarCount=size(InVarList)
    if(present(InFuncObjPrtList)) then
        TempFuncObjPrtList=>InFuncObjPrtList
    else        
        TempFuncObjPrtList=>DefaultObjPrtList
    end if
    if(present(InVarDiffList)) then
        TempVarDiffList=>InVarDiffList
    else
        TempVarDiffList=>DefaultVarDiff(1:TempVarCount)
    end if
    do i=1,TempVarCount
        call FPartialDeriveVectorFunctionByCentralDiff_Tan(InVarList,InFunction,i,OutMatrix(:,i),TempFuncObjPrtList,TempVarDiffList(i))
    end do       
    return
    end subroutine
    end module