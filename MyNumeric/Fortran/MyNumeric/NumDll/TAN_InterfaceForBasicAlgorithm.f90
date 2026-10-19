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
    procedure(CDiffTypeFunction_TAN),pointer :: CCommonMultiVarFunctionPtr
    abstract interface
        real function CSingleVarTypeFunction_TAN(IOValueList,InFuncObjPrtList) result(ans)
            use iso_c_binding
            implicit none
                real::IOValueList
                type(c_ptr)::InFuncObjPrtList(*)
        end function CSingleVarTypeFunction_TAN
    end interface
    procedure(CSingleVarTypeFunction_TAN),pointer :: CCommonSingleVarFunctionPtr
    contains
    
    real function ConvertCMultiVarFunctionToFortranFunction(IOValueList,InFuncObjPrtList) result(ans)
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
        ans=CCommonMultiVarFunctionPtr(IOValueList,InFuncObjPrtList)
        return
    end function
    real function ConvertCSingleVarFunctionToFortranFunction(IOValue,InFuncObjPrtList) result(ans)
        use TAN_NumericDiff
        use iso_c_binding
        implicit none
        real::IOValue
        type(c_ptr),optional::InFuncObjPrtList(:)
        ans=CCommonSingleVarFunctionPtr(IOValue,InFuncObjPrtList)
        return
    end function
    end module
