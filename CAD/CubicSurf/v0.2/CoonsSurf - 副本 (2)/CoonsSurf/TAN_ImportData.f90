    module TAN_ImportData
    implicit none
    integer::DefaultOtherDataEndIndex=2
    integer::DefaultMaxLineStrLength=512
    contains
    !读取文本数据
    subroutine ImportSurfDataAndAxis(InImportPath,OutRotAxisAndOrigin,OutCubicSurfObj)
    use TAN_BasicFunction
    use TAN_CubicSurf
    implicit none
    character(len=*) ::InImportPath
    real::OutRotAxisAndOrigin(:)
    type(CubicSurf_Tan)::OutCubicSurfObj
    integer::TmpFileRowNum,TmpPointRowNum,TempImportRowIndex
    integer::TmpUCount,TmpVCount
    integer::TmpFileIndex=501,TmpFileStat
    real,pointer::pTmpSurfData(:,:,:)
    integer::i,j
    open (UNIT=TmpFileIndex,FILE=InImportPath,FORM='FORMATTED')
    TmpFileRowNum=CalDatFileRowNumByIndex(TmpFileIndex)
    !为列点数
    TmpPointRowNum=CalRowNumBetweenTwoIdentifiersByIndex(TmpFileIndex,'ROW')
    TmpVCount=TmpPointRowNum
    TmpUCount=(TmpFileRowNum-DefaultOtherDataEndIndex)/(TmpVCount+1)
    !读取转动中心和转轴
    read(TmpFileIndex,*) OutRotAxisAndOrigin(4:6)
    read(TmpFileIndex,*) OutRotAxisAndOrigin(1:3)
    !读取齿面点
    allocate(pTmpSurfData(3,TmpUCount,TmpVCount))
    TmpFileStat=0
    TempImportRowIndex=0
    do i=1,TmpUCount
        read(TmpFileIndex,*)
        do j=1,TmpVCount
            read(TmpFileIndex,*) pTmpSurfData(:,i,j)
        end do    
    enddo
    !call ExportUGSurfPoint(pTmpSurfData,'D:\WorkSpace\softdevelop\CAE\LTCA\Interface\CAD\Pin.dat')
    !生成样条曲面对象
    call CreatCubicSurfSpace(3,TmpUCount,TmpVCount,OutCubicSurfObj)
    call BuildCubicSurfObject_Tan(OutCubicSurfObj%uOriginParaList,OutCubicSurfObj%vOriginParaList,pTmpSurfData,OutCubicSurfObj)
    deallocate(pTmpSurfData)
    close(TmpFileIndex)
    end subroutine
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
    end function
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
    end function
    end module