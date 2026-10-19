
    module TAN_BasicFunction
    implicit none
    contains 
    !抛出错误
    subroutine JudgeCondition(Condition,ExportInfo)
    implicit none
    logical::Condition
    character(len=*) ::ExportInfo
    if (Condition) then
        write(6,*) ExportInfo
    end if    
    return
    end subroutine
    !等差数列赋值
    subroutine BuildArithmeticList(StarValue,EndVaule,OutList)
    implicit none
    real::StarValue,EndVaule,OutList(:)
    integer::ElemNum,i
    real::ValueIncr
    ElemNum=size(OutList)
    ValueIncr=ReturnIncrementValue(StarValue,EndVaule,ElemNum)
    do i=1,ElemNum
        OutList(i)=StarValue+ValueIncr*(i-1)
    end do
    end subroutine
    real function ReturnIncrementValue(StarValue,EndVaule,ElemNum)
    implicit none
    real::StarValue,EndVaule
    integer::ElemNum
    if (ElemNum>1) then
        ReturnIncrementValue=(EndVaule-StarValue)/real(ElemNum-1)
    else
        ReturnIncrementValue=0
    end if
    return
    end function
    !均匀取样
    subroutine CompressList(InList,OutList)
    implicit none
    real::InList(:),OutList(:)
    integer::InNum,OutNum,i
    integer::AssignIndex
    InNum=size(InList,1)
    OutNum=size(OutList,1)
    if (OutNum<InNum) then
        do i=1,OutNum
            AssignIndex=nint(1+real(InNum-1)/real(OutNum-1)*real(i-1))
            OutList(i)=InList(AssignIndex)
        end do
    end if
    if (OutNum==InNum) then
            OutList(:)=InList(:)
    end if
    return
    end subroutine
    subroutine Compress2DList(InList,OutList)
    implicit none
    real::InList(:,:),OutList(:,:)
    integer::InNum,OutNum,i
    integer::AssignIndex
    InNum=size(InList,2)
    OutNum=size(OutList,2)
    if (OutNum<InNum) then
        do i=1,OutNum
            AssignIndex=nint(1+real(InNum-1)/real(OutNum-1)*real(i-1))
            OutList(:,i)=InList(:,AssignIndex) 
        end do
    end if
    if (OutNum==InNum) then
            OutList(:,:)=InList(:,:)
    end if
    return
    end subroutine
    
    subroutine ExtendList(InList,OutList,NoNeedAssignList,NeedAssignList)
    implicit none
    real::InList(:),OutList(:)
    integer::NeedAssignList(:),NoNeedAssignList(:)
    integer::InNum,OutNum,i
    integer::OutAssignIndex,PreAssignIndex,j,IndexAssignPosition
    InNum=size(InList,1)
    OutNum=size(OutList,1)
    PreAssignIndex=0
    IndexAssignPosition=0
    do i=1,InNum
        OutAssignIndex=nint(1+real(OutNum-1)/real(InNum-1)*real(i-1))
        OutList(OutAssignIndex)=InList(i)
        NoNeedAssignList(i)=OutAssignIndex
        if (OutAssignIndex-PreAssignIndex>1) then
            do j=1,OutAssignIndex-PreAssignIndex-1
            NeedAssignList(j+IndexAssignPosition)=PreAssignIndex+j
            end do
            IndexAssignPosition=IndexAssignPosition+OutAssignIndex-PreAssignIndex-1
        end if
        PreAssignIndex=OutAssignIndex
    end do
    end subroutine
    
    subroutine Extend2DList(InList,OutList,NoNeedAssignList,NeedAssignList)
    implicit none
    real::InList(:,:),OutList(:,:)
    integer::NeedAssignList(:),NoNeedAssignList(:)
    integer::InNum,OutNum,i
    integer::OutAssignIndex,PreAssignIndex,j,IndexAssignPosition
    InNum=size(InList,2)
    OutNum=size(OutList,2)
    PreAssignIndex=0
    IndexAssignPosition=0
    do i=1,InNum
        OutAssignIndex=nint(1+real(OutNum-1)/real(InNum-1)*real(i-1))
        OutList(:,OutAssignIndex)=InList(:,i)
        NoNeedAssignList(i)=OutAssignIndex
        if (OutAssignIndex-PreAssignIndex>1) then
            do j=1,OutAssignIndex-PreAssignIndex-1
            NeedAssignList(j+IndexAssignPosition)=PreAssignIndex+j
            end do
            IndexAssignPosition=IndexAssignPosition+OutAssignIndex-PreAssignIndex-1
        end if
        PreAssignIndex=OutAssignIndex
    end do
    end subroutine
    integer function FindElemPosition(CritValue,ValueList,Method) !Method=1,大于CritValue的第一个元素
    implicit none
    integer::Method
    real::CritValue,ValueList(:)
    integer::i,OriginNum
    OriginNum=size(ValueList,1)
    if (method==1) Then
        do i=1,OriginNum
            if (ValueList(i)>CritValue) then
                exit
            end if
        end do
        FindElemPosition=i
    else
        do i=1,OriginNum
            if (ValueList(OriginNum-i+1)<CritValue) then
                exit
            end if
        end do
        FindElemPosition=OriginNum-i+1
    end if
    return
    end function
    
    !判断值在偏序数列中的位置
    !InOrderMethod-偏序方法，0递增，其它递减
    !返回值-1递增数列左侧，0递增数列中，1递增数列右侧
    integer function JudgeValueLocInOrderList(InValue,InValueList,InOrderMethod)
    implicit none
    real::InValue,InValueList(:)
    integer::InOrderMethod
    integer::ValueCount
    ValueCount=size(InValueList)
    if (InOrderMethod .EQ.0) then
        if (InValue .LT. InValueList(1)) then
            JudgeValueLocInOrderList=-1
        else if(InValue .GT. InValueList(ValueCount)) then
            JudgeValueLocInOrderList=1
        else
            JudgeValueLocInOrderList=0
        end if
    else
       if (InValue .GT. InValueList(ValueCount)) then
            JudgeValueLocInOrderList=-1
        else if(InValue .LT. InValueList(1)) then
            JudgeValueLocInOrderList=1
        else
            JudgeValueLocInOrderList=0
        end if
    end if
    return
    end function
    !计算从点沿指定方向和矢量延申的矢量值
    subroutine CalExtendVector(InScal,InStartPoint,InVector,OutVector)
    implicit none
    real::InScal,InStartPoint(:),InVector(:),OutVector(:)
    OutVector=InStartPoint+InScal*InVector
    return
    end subroutine
    !显示和输出
    !输出矩阵
    subroutine PrintMatrixByDeviceNum(InMatrix,InDeviceNum)
    implicit none
    real::InMatrix(:,:)
    integer::InDeviceNum
    character(len=20)::TempDataFormat
    character(len=12)::TempDataFormatMiddle
    integer::i,MatrixColumnCount
    MatrixColumnCount=size(InMatrix,2)
    write(TempDataFormatMiddle,'(I12)') MatrixColumnCount
    TempDataFormat='('//trim(TempDataFormatMiddle)//'F35.25)'
    do i=1,MatrixColumnCount
        write (InDeviceNum,TempDataFormat) InMatrix(i,:)
    
    end do
    
    end subroutine
    
    subroutine ExportUGCurvePoint(CurvePoint,FilePath)
    implicit none
    real::CurvePoint(:,:)
    integer::i,j
    character(len=*) ::FilePath
    open(unit=100,file=FilePath)
    do i=1,size(CurvePoint,2)
        write(100,*) CurvePoint(1:3,i)
    end do
    close(100)
    end subroutine
    
    
    subroutine ExportUGSurfPoint(SurfPoint,FilePath)
    implicit none
    real::SurfPoint(:,:,:)
    integer::i,j
    character(len=*) ::FilePath
    open(unit=100,file=FilePath)
    do j=1,size(SurfPoint,3)
        write(100,*) "ROW"
        do i=1,size(SurfPoint,2)
            write(100,*) SurfPoint(1:3,i,j)
        end do
    end do
    close(100)
    end subroutine
    
    
    !测试函数
    
    subroutine TestFunction(InValueList,OutVector)
    implicit none
    real::InValueList(:),OutVector(:)
    OutVector(1)=InValueList(1)
    OutVector(2)=InValueList(2)
    OutVector(3)=5+10*(-1+InValueList(1))**2+2*(1+InValueList(2))**2
    return
    end subroutine
    
    real function TestMinFunction(InValueList)
    use blas95
    implicit none
    real::InValueList(:)
    real::TempVector(3)
    call TestFunction(InValueList,TempVector)
    TestMinFunction=dot(TempVector,TempVector)
    return
    end function
    
    subroutine testfun2(InValueList,OutVector)
    implicit none
    real::InValueList(:)
    real::OutVector(:)
    OutVector(1)=sin(InValueList(1)+InValueList(2))
    OutVector(2)=cos(InValueList(1)-InValueList(2))
    OutVector(3)=cos(InValueList(1)-InValueList(2))
    end subroutine
    
    real function testgrad2(InValueList)
    implicit none
    real::InValueList(:)
    testgrad2=sin(InValueList(1)-InValueList(2)+InValueList(3))+&
        cos(InValueList(1)+InValueList(2)-InValueList(3))

    end function
    end module TAN_BasicFunction