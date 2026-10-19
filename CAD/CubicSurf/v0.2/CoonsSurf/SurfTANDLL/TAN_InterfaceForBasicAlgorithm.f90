    module TAN_InterfaceForBasicAlgorithm
    implicit none
    abstract interface
        real function CDiffTypeFunction_TAN(IOValueList,InFuncObjPrtList) result(ans)
            use iso_c_binding
            implicit none
                real::IOValueList(*)
                type(c_ptr)::InFuncObjPrtList(*)
        end function CDiffTypeFunction_TAN
    end interface
    procedure(CDiffTypeFunction_TAN),pointer :: CCommonDiffFunctionPtr
    real,pointer::pCommonTempValueList(:)
    contains
    real function ConvertOptimizationCFunctionToFortranFunction(IOValueList,InFuncObjPrtList) result(ans)
        use TAN_NumericDiff
        use iso_c_binding
        implicit none
        real::IOValueList(:)
        type(c_ptr),optional::InFuncObjPrtList(:)
        real,pointer::TempIOValue(:)
        !type(c_ptr)::TempValuePtrListPtr
        type(c_ptr),pointer::TempFuncObjListPtr(:)
        !TempIOValue=>IOValueList
        !TempPtrList=>InFuncObjPrtList
        !TempValuePtrListPtr=c_loc(IOValueList)
        !TempFuncObjListPtr=c_loc(InFuncObjPrtList)
        !TempIOValue=>IOValueList
        ans=CCommonDiffFunctionPtr(IOValueList,InFuncObjPrtList)
        return
        end function
    end module