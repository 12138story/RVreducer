    module TAN_ImportData
    implicit none
    integer::DefaultOtherDataEndIndex=2
    integer::DefaultMaxLineStrLength=512
    contains
    !读取文本数据
    !该段程序放到了TAN_CubicSurf
    !subroutine ImportSurfDataAndAxis(InImportPath,OutRotOriginAndAxis,OutCubicSurfObj)
    !!use TAN_BasicFunction
    !!use TAN_CubicSurf
    !implicit none
    !!character(len=*) ::InImportPath
    !!real::OutRotOriginAndAxis(:)
    !!type(CubicSurf_Tan)::OutCubicSurfObj
    !!integer::TmpFileRowNum,TmpPointRowNum,TempImportRowIndex
    !!integer::TmpUCount,TmpVCount
    !!integer::TmpFileIndex=501,TmpFileStat
    !!real,pointer::pTmpSurfData(:,:,:)
    !!integer::i,j
    !!open (UNIT=TmpFileIndex,FILE=InImportPath,FORM='FORMATTED')
    !!TmpFileRowNum=CalDatFileRowNumByIndex(TmpFileIndex)
    !!!为列点数
    !!TmpPointRowNum=CalRowNumBetweenTwoIdentifiersByIndex(TmpFileIndex,'ROW')
    !!TmpVCount=TmpPointRowNum
    !!TmpUCount=(TmpFileRowNum-DefaultOtherDataEndIndex)/(TmpVCount+1)
    !!!读取转动中心和转轴
    !!read(TmpFileIndex,*) OutRotOriginAndAxis(1:3)
    !!read(TmpFileIndex,*) OutRotOriginAndAxis(4:6)
    !!!读取齿面点
    !!allocate(pTmpSurfData(3,TmpUCount,TmpVCount))
    !!TmpFileStat=0
    !!TempImportRowIndex=0
    !!do i=1,TmpUCount
    !!    read(TmpFileIndex,*)
    !!    do j=1,TmpVCount
    !!        read(TmpFileIndex,*) pTmpSurfData(:,i,j)
    !!    end do    
    !!enddo
    !!!call ExportUGSurfPoint(pTmpSurfData,'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\Pin.dat')
    !!!生成样条曲面对象
    !!call CreatCubicSurfSpace(3,TmpUCount,TmpVCount,OutCubicSurfObj)
    !!call BuildCubicSurfObject_Tan(OutCubicSurfObj%uOriginParaList,OutCubicSurfObj%vOriginParaList,pTmpSurfData,OutCubicSurfObj)
    !!deallocate(pTmpSurfData)
    !!close(TmpFileIndex)
    !end subroutine
    !根据文件标识计算文本数据行数，计算完成后返回文本头部
    integer function CalDatFileRowNumByIndex(InOpenFileIndexNum) result(ans)
    implicit none
    integer::InOpenFileIndexNum
    integer::TmpRowNum  
    integer::TmpFileStat
    TmpRowNum=0
    TmpFileStat=0
    do while (.TRUE.)
        read(InOpenFileIndexNum,*,IOSTAT=TmpFileStat)
        if (TmpFileStat .NE. 0) exit
        TmpRowNum=TmpRowNum+1
    enddo
    ans=TmpRowNum
    rewind(InOpenFileIndexNum)
    end function
    !根据路径计算文本数据行数，计算完成后关闭文件
    integer function CalDatFileRowNumByPath(InOpenFilePath) result(ans)
    implicit none
    character(len=*) ::InOpenFilePath
    open (UNIT=502,FILE=InOpenFilePath,FORM='FORMATTED')
    ans=CalDatFileRowNumByIndex(502)
    close(502)
    return
    end function
    !导入数据点，放入到文件中
    subroutine ImportMatrixData_Tan(InOpenFilePath,OutMatrix,InOptIsTrans)
    implicit none
    character(len=*) ::InOpenFilePath
    real,pointer::OutMatrix(:,:)
    integer,optional::InOptIsTrans!是否转置
    integer::TmpFileStat,TmpFileRowNum,TmpFileRowCount,i
    open (UNIT=502,FILE=InOpenFilePath,FORM='FORMATTED')
    TmpFileRowNum=1
    TmpFileRowCount=CalDatFileRowNumByIndex(502)
    If(present(InOptIsTrans)) then
        do i=1,TmpFileRowCount
            read(502,*,IOSTAT=TmpFileStat) OutMatrix(:,i)
        end do
    else
        do i=1,TmpFileRowCount
            read(502,*,IOSTAT=TmpFileStat) OutMatrix(i,:)
        end do
    end if
    
    
    !If(present(InOptIsTrans)) then
    !    do while (.TRUE.)
    !        read(502,*,IOSTAT=TmpFileStat) OutMatrix(:,TmpFileRowNum)
    !        if (TmpFileStat .NE. 0) exit
    !        TmpFileRowNum=TmpFileRowNum+1
    !    enddo
    !else
    !    do while (.TRUE.)
    !        read(502,*,IOSTAT=TmpFileStat) OutMatrix(TmpFileRowNum,:)
    !        if (TmpFileStat .NE. 0) exit
    !        TmpFileRowNum=TmpFileRowNum+1
    !    enddo
    !end if
    return
    end subroutine
    !根据文件标识计算两行间点数，计算完成后返回文本头部
    integer function CalRowNumBetweenTwoIdentifiersByIndex(InOpenFileIndexNum,InIdentifier) result(ans)
    integer::InOpenFileIndexNum
    character(len=*)::InIdentifier
    character(len=DefaultMaxLineStrLength)::TmpLineStr
    integer::TemIdentifierCount,TmpRowNum
    TemIdentifierCount=0
    do while (TemIdentifierCount<1)
        read(InOpenFileIndexNum,*) TmpLineStr
        if (TmpLineStr==InIdentifier) then
            TemIdentifierCount=TemIdentifierCount+1
        end if
    enddo
    TmpRowNum=0
    do while (TemIdentifierCount<2)
        read(InOpenFileIndexNum,*) TmpLineStr
        if (TmpLineStr==InIdentifier) then
            TemIdentifierCount=TemIdentifierCount+1
        end if
        TmpRowNum=TmpRowNum+1
    enddo
    ans=TmpRowNum-1
    rewind(InOpenFileIndexNum)
    return
    end function
    end module