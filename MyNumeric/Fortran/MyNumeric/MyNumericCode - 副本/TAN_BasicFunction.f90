
    module TAN_BasicFunction
    implicit none
    contains 
    !抛出错误
    subroutine JudgeCondition_Tan(Condition,ExportInfo)
    implicit none
    logical::Condition
    character(len=*) ::ExportInfo
    if (Condition) then
        write(6,*) ExportInfo
    end if    
    return
    end subroutine
    !等差数列赋值
    subroutine BuildArithmeticList_Tan(StarValue,EndVaule,OutList)
    implicit none
    real::StarValue,EndVaule,OutList(:)
    integer::ElemNum,i
    real::ValueIncr
    ElemNum=size(OutList)
    ValueIncr=ReturnIncrementValue_Tan(StarValue,EndVaule,ElemNum)
    do i=1,ElemNum
        OutList(i)=StarValue+ValueIncr*(i-1)
    end do
    end subroutine
    real function ReturnIncrementValue_Tan(StarValue,EndVaule,ElemNum) result(ans)
    implicit none
    real::StarValue,EndVaule
    integer::ElemNum
    if (ElemNum>1) then
        ans=(EndVaule-StarValue)/real(ElemNum-1)
    else
        ans=0
    end if
    return
    end function
    !均匀取样
    subroutine CompressList_Tan(InList,OutList)
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
    subroutine Compress2DList_Tan(InList,OutList)
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
    
    !找到第一个比准则数大的index
    integer function FindFirstLargerValueIndex_Tan(InCritValue,InDataList) result(ans)
    use TAN_BasicCoef
    implicit none
    real::InCritValue,InDataList(:)
    integer::DataLength
    integer::low,high,mid
    integer::i
    !FindFirstLargerValueIndex=minloc(InDataList,1,InDataList>InCritValue)
    !!检验是否为最大值，因为如果为最大值时Index也为1
    !if (FindFirstLargerValueIndex==1) then
    !    if(all(InDataList<=InCritValue)) then
    !        FindFirstLargerValueIndex=size(InDataList)
    !    end if
    !end if        
    
    DataLength=size(InDataList)
    !当
    if(DataLength==0) then
        ans=1
    return
    end if
    if(InCritValue<InDataList(1)) then
        ans=1        
        return
    else if(InCritValue>InDataList(DataLength)-DefaultAbsolueError) then
        ans=DataLength
        return
    end if
    low=1
    high=DataLength
    !二分查找
    do while(low<=high)
        mid=(low+high)/2
        if ((InCritValue>=InDataList(mid)).AND.(InCritValue<InDataList(mid+1))) then
            ans=mid+1
            !write(6,*) i
            return
        else if(InCritValue<InDataList(mid)) then
            high=mid
        else
            low=mid+1
        end if        
    end do
    
    !!插值查找
    !do while(low<=high)
    !    !i=i+1
    !    !write(6,*) low+(InCritValue-InDataList(low))/(InDataList(high)-(InDataList(low)))*(high-low)
    !    mid=low+(InCritValue-InDataList(low))/(InDataList(high)-(InDataList(low)))*(high-low)
    !    if ((InCritValue>=InDataList(mid)).AND.(InCritValue<InDataList(mid+1))) then
    !        FindFirstLargerValueIndex=mid+1
    !        !write(6,*) i
    !        return
    !    else if (InCritValue<InDataList(mid)) then
    !        high=mid
    !    else
    !        low=mid
    !    end if   
    !end do
    return     
    end function
    
    subroutine ExtendList_Tan(InList,OutList,NoNeedAssignList,NeedAssignList)
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
    
    subroutine Extend2DList_Tan(InList,OutList,NoNeedAssignList,NeedAssignList)
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
    integer function FindElemPosition_Tan(CritValue,ValueList,Method) result(ans)!Method=1,大于CritValue的第一个元素
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
        ans=i
    else
        do i=1,OriginNum
            if (ValueList(OriginNum-i+1)<CritValue) then
                exit
            end if
        end do
        ans=OriginNum-i+1
    end if
    return
    end function
    
    !判断值在偏序数列中的位置
    !InOrderMethod-偏序方法，0递增，其它递减
    !返回值-1递增数列左侧，0递增数列中，1递增数列右侧
    integer function JudgeValueLocInOrderList_Tan(InValue,InValueList,InOrderMethod) result(ans)
    implicit none
    real::InValue,InValueList(:)
    integer::InOrderMethod
    integer::ValueCount
    ValueCount=size(InValueList)
    if (InOrderMethod .EQ.0) then
        if (InValue .LT. InValueList(1)) then
            ans=-1
        else if(InValue .GT. InValueList(ValueCount)) then
            ans=1
        else
            ans=0
        end if
    else
       if (InValue .GT. InValueList(ValueCount)) then
            ans=-1
        else if(InValue .LT. InValueList(1)) then
            ans=1
        else
            ans=0
        end if
    end if
    return
    end function
    !计算从点沿指定方向和矢量延申的矢量值
    subroutine CalExtendVector_Tan(InScal,InStartPoint,InVector,OutVector)
    implicit none
    real::InScal,InStartPoint(:),InVector(:),OutVector(:)
    OutVector=InStartPoint+InScal*InVector
    return
    end subroutine
    !组合数学
    !计算阶乘,InN!
    integer function CalFactorial_Tan(InN)
    implicit none
    integer::InN,i    
    CalFactorial_Tan=1
    do i=1,InN
        CalFactorial_Tan=CalFactorial_Tan*i
    end do
    return
    end function
    !计算排列,CnM
    integer function CalPermutation_Tan(InN,InM)
    implicit none
    integer::InN,InM
    CalPermutation_Tan=CalFactorial_Tan(InN)/CalFactorial_Tan(InN-InM)
    return
    end function
    !计算排列,CnM
    integer function CalBinomial_Tan(InN,InM)
    implicit none
    integer::InN,InM
    CalBinomial_Tan=CalFactorial_Tan(InN)/CalFactorial_Tan(InN-InM)/CalFactorial_Tan(InM)
    return
    end function
    !组合数生成
    !基本组合数生成，只有1，2，3，4
    subroutine GenBaseCombinaNumList_Tan(InOrginNumCount,InSingleCombinaNumCount,OutCombinaNumList)
    implicit none
    integer::InOrginNumCount,InSingleCombinaNumCount
    integer::OutCombinaNumList(:,:),TotalNumCount
    integer::i,j,k,TempStartJudgeIndex,TempOffsetIndex
    integer,pointer::TempMaxNum(:)
    integer,pointer::pTempNumIndex(:)
    allocate(pTempNumIndex(InOrginNumCount))
    pTempNumIndex=([1:InOrginNumCount])
    !TotalNumCount=size(InOrderedOrginNumList)
    TempOffsetIndex=InOrginNumCount-InSingleCombinaNumCount
    TempStartJudgeIndex=(TempOffsetIndex+1)
    OutCombinaNumList(:,1)=pTempNumIndex(1:InSingleCombinaNumCount)
    TempMaxNum=>pTempNumIndex(TempStartJudgeIndex:InOrginNumCount)
    i=1
    do while(count(OutCombinaNumList(:,i)==pTempNumIndex(TempStartJudgeIndex:InOrginNumCount)).ne.&
        InSingleCombinaNumCount)
        OutCombinaNumList(:,i+1)=OutCombinaNumList(:,i)
        do j=InSingleCombinaNumCount,1,-1
            if(OutCombinaNumList(j,i)<TempMaxNum(j)) then
                OutCombinaNumList(j,i+1)=OutCombinaNumList(j,i+1)+1
                do k=j,InSingleCombinaNumCount-1
                    OutCombinaNumList(k+1,i+1)=OutCombinaNumList(k,i+1)+1
                end do    
                exit
            end if            
        end do   
        i=i+1
    end do
    deallocate(pTempNumIndex)
    return
    end subroutine
    
    !生成双值变量的可能取值组合
    subroutine GenBase2VarCombinaList_Tan(OutVarCombinaList)
    implicit none
    integer::OutVarCombinaList(:,:)
    integer::TempTotalNum
    integer,pointer::pTempAssignIndex(:,:)
    integer::TempResultColumnIndex,i,j
    OutVarCombinaList=1
    TempResultColumnIndex=2
    TempTotalNum=size(OutVarCombinaList,1)
    do i=1,TempTotalNum
        allocate(pTempAssignIndex(i,CalBinomial_Tan(TempTotalNum,i)))
        call GenBaseCombinaNumList_Tan(TempTotalNum,i,pTempAssignIndex)
        do j=1,size(pTempAssignIndex,2)
            OutVarCombinaList(pTempAssignIndex(:,j),TempResultColumnIndex)=2
            TempResultColumnIndex=TempResultColumnIndex+1
        end do
        deallocate(pTempAssignIndex)
    end do
    !tempp=fac
    end subroutine
    
    !显示和输出
    !生成格式化字符
    subroutine GenFormateNotation_Tan(InDataLength,InDataStyle,OutNotation)
    implicit none
    integer::InDataLength
    character(len=*)::InDataStyle,OutNotation
    character(len=12)::TempDataFormatMiddle
    write(TempDataFormatMiddle,'(I12)') InDataLength
    OutNotation='('//trim(TempDataFormatMiddle)//InDataStyle//')'
    end subroutine
    !输出矩阵
    subroutine PrintMatrixByDeviceNum_Tan(InMatrix,InDeviceNum)
    implicit none
    real::InMatrix(:,:)
    integer::InDeviceNum
    character(len=20)::TempDataFormat
    character(len=12)::TempDataFormatMiddle
    integer::i,MatrixColumnCount
    MatrixColumnCount=size(InMatrix,2)
    write(TempDataFormatMiddle,'(I12)') MatrixColumnCount
    TempDataFormat='('//trim(TempDataFormatMiddle)//'F38.25)'
    do i=1,size(InMatrix,1)
        write (InDeviceNum,TempDataFormat) InMatrix(i,:)    
    end do
    return
    end subroutine
    subroutine ExportMatrixToFile_Tan(InMatrix,FilePath)
    implicit none
    real::InMatrix(:,:)
    character(len=*)::FilePath
    open(UNIT=501,File=FilePath)
    call PrintMatrixByDeviceNum_Tan(InMatrix,501)
    close(501)
    return
    end subroutine
    subroutine ExportUGCurvePoint_Tan(CurvePoint,FilePath)
    implicit none
    real::CurvePoint(:,:)
    integer::i,j,TempPointDimension
    character(len=*) ::FilePath
    open(unit=502,file=FilePath)
    TempPointDimension=size(CurvePoint(:,1))
    do i=1,size(CurvePoint,2)
        write(502,*) CurvePoint(:,i)
    end do
    close(502)
    return
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
    
    !subroutine TestFunction(InValueList,OutVector)
    !implicit none
    !real::InValueList(:),OutVector(:)
    !OutVector(1)=InValueList(1)
    !OutVector(2)=InValueList(2)
    !OutVector(3)=5+10*(-1+InValueList(1))**2+2*(1+InValueList(2))**2
    !return
    !end subroutine
    
    
    
    

    !测试函数2
    subroutine TestFunction2(InValueList,InConstList,InObjPrtList,OutVector)
    use iso_c_binding, only: c_ptr
    implicit none
    real::InValueList(:),OutVector(:)
    real::InConstList(:)
    type(c_ptr)::InObjPrtList(:)   
    OutVector(1)=InValueList(1)
    OutVector(2)=InValueList(2)
    OutVector(3)=5+10*(-1+InValueList(1))**2+2*(1+InValueList(2))**2
    return
    end subroutine
    
    real function TestMinFunction(InValueList,InConstList,InObjPrtList)
    use iso_c_binding, only: c_ptr
    use blas95
    implicit none
    real::InValueList(:)
    real::InConstList(:)
    type(c_ptr)::InObjPrtList(:)  
    real::TempVector(3)
    call TestFunction2(InValueList,InConstList,InObjPrtList,TempVector)
    TestMinFunction=dot(TempVector,TempVector)
    return
    end function
    
    
    !测试函数3
    subroutine TestFunction3(InValueList,OutVector,InObjPrtList)
    use iso_c_binding, only: c_ptr
    implicit none
    real::InValueList(:),OutVector(:)
    type(c_ptr),optional::InObjPrtList(:)   
    OutVector(1)=InValueList(1)
    OutVector(2)=InValueList(2)
    OutVector(3)=5+10*(-1+InValueList(1))**2+2*(1+InValueList(2))**2
    return
    end subroutine
    
    real function TestMinFunction3(InValueList,InObjPrtList)
    use iso_c_binding, only: c_ptr
    use blas95
    implicit none
    real::InValueList(:)
    type(c_ptr),optional::InObjPrtList(:)  
    real::TempVector(3)
    call TestFunction3(InValueList,TempVector)
    TestMinFunction3=dot(TempVector,TempVector)
    return
    end function
    
    subroutine testfun2(InValueList,OutVector,InObjPrtList)
    use iso_c_binding, only: c_ptr
    implicit none
    real::InValueList(:)
    real::OutVector(:)

    type(c_ptr),optional::InObjPrtList(:) 
    OutVector(1)=sin(InValueList(1)+InValueList(2))
    OutVector(2)=cos(InValueList(1)-InValueList(2))
    OutVector(3)=cos(InValueList(1)-InValueList(2))
    end subroutine
    !
    real function testgradfunc2(InValueList,InObjPrtList)
    use iso_c_binding, only: c_ptr
    implicit none
    type(c_ptr),optional::InObjPrtList(:) 
    real::InValueList(:)
    testgradfunc2=sin(InValueList(1)-InValueList(2)+InValueList(3))+&
        cos(InValueList(1)+InValueList(2)-InValueList(3))
    
    end function
    end module TAN_BasicFunction