   
    module TAN_BasicString
    implicit none
    contains 
    !计算字符在字符串中出现次数
    integer function FCountNotationIn1String_Tan(InString,InNotation) result(ans)
    implicit none
    character(len=*)::InString,InNotation
    integer::i
    i=0
    ans=0
    do i=1,len(InString)-len(InNotation)+1
        if(InString(i:i+len(InNotation)-1)==InNotation)then
            ans=ans+1
        end if
    end do
    return
    end function
    !计算字符在字符串中最后出现的索引
    integer function FSearchNotationLastIndexIn1String_Tan(InString,InNotation) result(ans)
    implicit none
    character(len=*)::InString,InNotation
    integer::i
    i=0
    ans=0
    do i=1,len(InString)-len(InNotation)+1
        if(InString(i:i+len(InNotation)-1)==InNotation)then
            ans=i
        end if
    end do
    return
    end function
    !获取程序运行目录
    subroutine ObtainRunTimedirectoryWithBlank_Tan(IOString)
    implicit none
    character(len=*)::IOString
    character(len=:),allocatable::psTempProgramPath
    integer::LastNotationIndex
    allocate(character(255)::psTempProgramPath)
    call GET_COMMAND_ARGUMENT(0, psTempProgramPath)
    LastNotationIndex=FSearchNotationLastIndexIn1String_Tan(psTempProgramPath,'\')
    IOString=' '
    IOString(1:LastNotationIndex)=psTempProgramPath(1:LastNotationIndex)
    !allocate(ProgramPath(255))
    deallocate(psTempProgramPath)
    !call GET_COMMAND_ARGUMENT(0, ProgramPath)
    !TestDepthCount=FCountNotationIn1String_Tan(ProgramPath,'\')
    !TestDepthCount=FSearchNotationLastIndexIn1String_Tan(ProgramPath,'\')
    !write(6,*) ProgramPath
    !write(6,*) TestDepthCount
    !write(6,*) ProgramPath(1:TestDepthCount)
    return
    end subroutine
    

    end module