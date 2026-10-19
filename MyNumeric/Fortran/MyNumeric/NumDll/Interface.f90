        real function TANCalVectorNorm(InElemCount,InVector) result (ans)
        !DEC$ ATTRIBUTES DLLEXPORT::TANCalVectorNorm
        use TAN_NumericAlgebra    
        implicit none
        integer::InElemCount
        real::InVector(InElemCount)    
        ans=FCalVectorNorm_TAN(InVector)
        return
        end function
        real function TestFunPtr(InFunc,InVarCount,InVar) result(ans)
        !DEC$ ATTRIBUTES DLLEXPORT::TESTFUNPTR
        use iso_c_binding
        implicit none
        integer::InVarCount
        real(c_double)::InVar(InVarCount)
        interface    
            real function InFunc(InVec)
            implicit none
            real::InVec(*)
            end function InFunc
        end interface
        write(6,*) InFunc(InVar)
        return
        end function

    real function TANFindMinValueWithoutConstrainForC(InFunction,InVarCount,IOVarList,&
        InFuncObjPrtCount,InFuncObjPrtList) result(ans)
    !DEC$ ATTRIBUTES DLLEXPORT::TANFINDMINVALUEWITHOUTCONSTRAINFORC
    use TAN_NumericOptimization
    use TAN_InterfaceForBasicAlgorithm
    implicit none
    integer::InVarCount
    real::IOVarList(InVarCount)
    integer::InFuncObjPrtCount
    type(c_ptr)::InFuncObjPrtList(InFuncObjPrtCount)
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
            use iso_c_binding
            implicit none
            real::InFValueList(*)
            type(c_ptr)::InFuncObjPrtList(*)
        end function
    end interface
    CCommonMultiVarFunctionPtr=>InFunction 
    ans=FindMinValueWithoutConstrain_Tan(ConvertCMultiVarFunctionToFortranFunction,IOVarList,InFuncObjPrtList)
    return
    end function
    
    
    real function TANFindRootByBisectionForC(InFunction,InInitialVar,&
        InFuncObjPrtCount,InFuncObjPrtList) result(ans)
    !DEC$ ATTRIBUTES DLLEXPORT::TANFINDROOTBYBISECTIONFORC
    use TAN_NumericSolveEqu
    use TAN_InterfaceForBasicAlgorithm
    implicit none
    real::InInitialVar(2)
    integer::InFuncObjPrtCount
    type(c_ptr)::InFuncObjPrtList(InFuncObjPrtCount)
    interface
        real function InFunction(InFValueList,InFuncObjPrtList)
            use iso_c_binding
            implicit none
            real::InFValueList
            type(c_ptr)::InFuncObjPrtList(*)
        end function
    end interface
    CCommonSingleVarFunctionPtr=>InFunction 
    ans=FindScalarEquRootByBisection_Tan(ConvertCSingleVarFunctionToFortranFunction,InInitialVar,InFuncObjPrtList)
    return
    end function