
    module TAN_NumericAlgebra
    implicit none
    contains
    !复制向量
    subroutine CopyVector_TAN(InCopiedVector,OutVector)
    use blas95
    implicit none
    real::InCopiedVector(:),OutVector(:)
    call copy(InCopiedVector,OutVector)
    return
    end subroutine
    !复制矩阵
    subroutine CopyMatrix_TAN(InCopiedMatrix,OutMatrix)
    use blas95
    implicit none
    real::InCopiedMatrix(:,:),OutMatrix(:,:)
    integer::i
    do i=1,size(InCopiedMatrix,2)
        call copy(InCopiedMatrix(:,i),OutMatrix(:,i))
    end do
    return
    end subroutine
    !线性代数
    !计算2-范数
    real function FCalVectorNorm_TAN(InVector)
    use blas95
    implicit none
    real:: InVector(:)
    !FCalVectorNorm_TAN=sqrt(sum(InVector**2))
    FCalVectorNorm_TAN=nrm2(InVector)
    return    
    end function
    !计算无穷范数
    real function CalInfiniteNormOfVector_Tan(InVector)
    implicit none
    real::InVector(:)
    CalInfiniteNormOfVector_Tan=maxval(abs(InVector))
    end function
    !矢量放缩
    subroutine CalScaleVector_TAN(InScale,InVector,OutVector) 
    use blas95
    implicit none
    real::InScale,InVector(:),OutVector(:)
    real,pointer::pTempVector(:)
    allocate(pTempVector(size(OutVector)))
    pTempVector=0d0
    call axpy(InVector,pTempVector,InScale)
    call copy(pTempVector,OutVector)
    deallocate(pTempVector)
    return
    end subroutine
    !计算单位矢量
    subroutine CalVectorNormalize_Tan(InVector,OutVector)
    implicit none
    real:: InVector(:)
    real:: OutVector(:)
    real:: VectorNorm
    VectorNorm=FCalVectorNorm_TAN(InVector)
    OutVector=InVector/VectorNorm
    return
    end subroutine
    !计算矢量点积
    real function FCalDotProduct_Tan(InVector1,InVector2) result(ans)
    use blas95
    implicit none
    real:: InVector1(:),InVector2(:)
    ans=dot(InVector1,InVector2)
    return
    end function

    !3维度矢量操作
    !计算矢量叉积
    subroutine CalVectorCrossProduct_Tan(InVector1,InVector2,OutVector)
    use blas95
    implicit none
    real:: InVector1(:),InVector2(:)
    real:: OutVector(:)
    real::TempX,TempY,TempZ!添加临时变量，InVectory与OutVector地址一致时数据错乱
    TempX=InVector1(2)*InVector2(3)-InVector1(3)*InVector2(2)
    TempY=InVector1(3)*InVector2(1)-InVector1(1)*InVector2(3)
    TempZ=InVector1(1)*InVector2(2)-InVector1(2)*InVector2(1)
    call copy([TempX,TempY,TempZ],OutVector)
    return
    end subroutine
    !计算矢量混合积
    real function FCalVectorMixedProduct_Tan(InVector1,InVector2,InVector3) result(ans)
    use blas95
    implicit none
    real:: InVector1(:),InVector2(:),InVector3(:)
    real,pointer:: pTempVector(:)
    allocate(pTempVector(3))
    call CalVectorCrossProduct_Tan(InVector1,InVector2,pTempVector)
    ans=dot(InVector3(1:3),pTempVector)
    deallocate(pTempVector)
    return
    end function
    !计算矢量球坐标
    subroutine CalVectorSpherCoor(InVector,OutVector)
    implicit none
    real:: InVector(3),OutVector(3)
    OutVector(1)=FCalVectorNorm_TAN(InVector)    
    OutVector(2)=atan(InVector(2)/InVector(1))
    OutVector(3)=acos(InVector(3)/OutVector(1))
    return
    end subroutine

    

    
    real function CalVectorTanAngZAxis_Tan(InVector) result(ans)
    implicit none
    integer::VectorDimension
    real:: InVector(:)
    real:: RadiusOnXYPlane   
    VectorDimension=size(InVector,1)
    RadiusOnXYPlane=FCalVectorNorm_TAN(InVector(1:VectorDimension-1))
    ans=RadiusOnXYPlane/InVector(VectorDimension)
    return
    end function
    
    real function CalVectorCotAngZAxis_Tan(InVector) result(ans)
    implicit none
    integer::VectorDimension
    real:: InVector(:)
    real:: RadiusOnXYPlane   
    VectorDimension=size(InVector,1)
    RadiusOnXYPlane=FCalVectorNorm_TAN(InVector(1:VectorDimension-1))
    ans=InVector(VectorDimension)/RadiusOnXYPlane
    return
    end function

    
    


    !矢量转动,绕原点,默认旋转轴已经单位化,向量为3维
    subroutine RotVectorByAxis_Tan(InRotAng,InVector,InRotAxis,OutVector)
    use blas95
    implicit none
    real::InRotAng,InRotAxis(:),InVector(:),OutVector(:)
    real,pointer::pTempVector(:,:)
    allocate(pTempVector(3,2))
    call copy(InVector,pTempVector(:,1))
    !第一组元素
    call scal(pTempVector(:,1),cos(InRotAng))
    call CalVectorCrossProduct_Tan(InRotAxis,InVector,pTempVector(:,2))
    call axpy(pTempVector(:,2),pTempVector(:,1),sin(InRotAng))
    call axpy(InRotAxis,pTempVector(:,1),dot(InRotAxis,InVector)*(1d0-cos(InRotAng)))
    call copy(pTempVector(:,1),OutVector)
    deallocate(pTempVector)
    return
    end subroutine
    !矢量转动,绕定点
    subroutine RotVectorByAxisAndOrigin_Tan(InRotAng,InVector,InRotAxis,InRotOrigin,OutVector)
    use blas95
    use TAN_BasicCoef
    implicit none
    real::InRotAng,InRotAxis(:),InRotOrigin(:),InVector(:),OutVector(:)
    real,pointer::pTempVector(:)
    allocate(pTempVector(3))  
    call RotVectorByAxis_Tan(InRotAng,InVector-InRotOrigin,InRotAxis,pTempVector)
    call axpy(InRotOrigin,pTempVector)
    call copy(pTempVector,OutVector)

    deallocate(pTempVector)
    return
    end subroutine   
    subroutine BuildArithmeticVectorList_Tan(InStartVector,InEndVector,OutVectorList)
    implicit none
    real:: InStartVector(:),InEndVector(:),OutVectorList(:,:)
    real,pointer:: nDevVector(:)
    integer::i,N
    N=size(OutVectorList,2)
    allocate(nDevVector(size(InEndVector,1)))
    nDevVector=(InEndVector-InStartVector)/real(N-1)
    do i=1,N
    OutVectorList(:,i)=InStartVector+nDevVector*(i-1)
    end do
    deallocate(nDevVector)     
    return
    end subroutine
    !计算矩阵乘以向量
    subroutine CalMatrixMultiVector_Tan(InMatrix,InVector,OutVector)
    use blas95
    implicit none
    real::InMatrix(:,:),InVector(:),OutVector(:)
    real,pointer::pTempVector(:)
    allocate(pTempVector(size(InVector,1)))
    call copy(InVector,pTempVector)
    !OutVector=0d0
    call gemv(InMatrix,pTempVector,OutVector)
    deallocate(pTempVector)
    return
    end subroutine
    
    
    !计算扩展特征值
    subroutine CalExtendEigenByQZ_Tan(InAMatrix,&
        InBMatrix,&
        InEigenOrder,&
        OutEigenValue,&
        OutEigenRightVector)
    use f95_precision
    use lapack95
    implicit none
    real::InAMatrix(:,:),InBMatrix(:,:)
    integer::InEigenOrder
    real::OutEigenValue(:,:)
    real::OutEigenRightVector(:,:)
    integer::N
    real,pointer::beta(:)
    real,pointer::EigenLeftVector(:,:)
    integer::info=1000
    N=size(InAMatrix,1)
    allocate(beta(N))
    allocate(EigenLeftVector(N,N))
    call ggev(InAMatrix,InBMatrix,&
        OutEigenValue(:,1),OutEigenValue(:,2),&
        beta,&
        vl=EigenLeftVector,info=info)
    OutEigenValue(:,1)=OutEigenValue(:,1)/beta
    OutEigenValue(:,2)=OutEigenValue(:,2)/beta
    deallocate(beta)
    deallocate(EigenLeftVector)
    end subroutine    
    !计算逆矩阵
    subroutine CalMatrixInverse_Tan(InMatrix,OutMatrix)
    use lapack95
    implicit none
    !real,pointer::InMatrix(:,:)
    !real,pointer::OutMatrix(:,:)
    !integer,pointer::MKL_ipiv(:)
    !allocate(MKL_ipiv(size(InMatrix,1)))
    !if(.not.associated(InMatrix,OutMatrix)) then
    !    OutMatrix=InMatrix
    !end if
    real::InMatrix(:,:)
    real::OutMatrix(:,:)
    integer,pointer::MKL_ipiv(:)
    allocate(MKL_ipiv(size(InMatrix,1)))
    if(loc(InMatrix).ne.loc(OutMatrix)) then
        OutMatrix=InMatrix
    end if
    call getrf(OutMatrix,MKL_ipiv)
    call getri(OutMatrix,MKL_ipiv)
    deallocate(MKL_ipiv)
    end subroutine
    !生成坐标变换矩阵
    subroutine GenCoorTransMatrix_Tan(InFromCoorList,OurMatrix,InToCoorList)
    use blas95
    implicit none
    real::InFromCoorList(:,:)
    real::OurMatrix(:,:)
    real,optional::InToCoorList(:,:)
    real,pointer::pTempOutMatrix(:,:)
    integer::i,j
    if(present(InToCoorList)) then
        allocate(pTempOutMatrix(4,4))
        pTempOutMatrix=0d0
        do j=1,3
            do i=1,3
                pTempOutMatrix(i,j)=dot(InToCoorList(1:3,i),InFromCoorList(1:3,j))
            end do
        end do
        call copy(InFromCoorList(1:3,4),pTempOutMatrix(1:3,4))
        !do i=1,3
        !    =dot(InToCoorList(1:3,i),)
        !end do    
        pTempOutMatrix(i,4)=1d0
        OurMatrix=pTempOutMatrix
        !call mkl_domatcopy('C', 'N', 4, 4, 1d0, pTempOutMatrix, 4, OurMatrix, 4)
        !call copy(pTempOutMatrix,OurMatrix)
        deallocate(pTempOutMatrix)
    else        
        OurMatrix=InFromCoorList
    end if      
    return
    end subroutine
    !搜寻数值列表最大值
    subroutine SearchRowColumnMaxAbsValueByTableList_Tan(InTableList,OutTable)    
    use blas95
    implicit none
    real::InTableList(:,:,:),OutTable(:,:)
    integer::TempRowCount,TempColumnCount,i,j
    !TempListCount=size(InTableList,1)
    TempRowCount=size(InTableList,2)
    TempColumnCount=size(InTableList,3) 
    !do j=1,TempColumnCount
    !    call copy(InTableList(1,:,j),OutTable(:,j))
    !end do
    !do k=2,TempListCount
    !    do j=1,TempColumnCount
    !        do i=1,TempRowCount
    !        end do
    !    end do
    !end do    
    do j=1,TempColumnCount
            do i=1,TempRowCount
                OutTable(i,j)=maxval(InTableList(:,i,j))
            end do
        end do
    return
    end subroutine
    !逆序输出1维向量
    subroutine ReviseVectorOrder_Tan(InVector,OutVector)
    implicit none
    real::InVector(:),OutVector(:)
    real,pointer::pTempVector(:)
    if(loc(InVector).ne.loc(OutVector)) then
        OutVector=InVector(size(InVector):1:-1)
    else
        allocate(pTempVector(size(InVector)))
        pTempVector=InVector
        OutVector=pTempVector(size(pTempVector):1:-1)
        deallocate(pTempVector)
    end if
    return
    end subroutine
    !逆列序输出矩阵
    subroutine ReviseMatrixColumnOrder_Tan(InMatrix,OutMatrix)
    implicit none
    real::InMatrix(:,:),OutMatrix(:,:)
    real,pointer::pTempMatrix(:,:)
    if(loc(InMatrix).ne.loc(OutMatrix)) then
        OutMatrix=InMatrix(:,size(InMatrix,2):1:-1)
    else
        allocate(pTempMatrix(size(InMatrix,1),size(InMatrix,2)))
        pTempMatrix=InMatrix
        OutMatrix=pTempMatrix(:,size(InMatrix,2):1:-1)
        deallocate(pTempMatrix)
    end if
    
    return
    end subroutine
    !计算两点距离
    real function FCalTwoPointsDist_Tan(InVector1,InVector2) result(ans)
    use Blas95
    implicit none
    real::InVector1(:),InVector2(:)
    ans=nrm2(InVector2-InVector1)
    end function
    !计算两点距离平方
    real function FCalTwoPointsDistPower_Tan(InVector1,InVector2) result(ans)
    use blas95
    implicit none
    real::InVector1(:),InVector2(:)
    ans=dot(InVector2-InVector1,InVector2-InVector1)
    end function
    !给出两点有向距离
    real function FCalTwoPointsDirectedDist_Tan(InToPoint,InFromPoint,InFromPointDirect,InReviseIndexCoef) result(ans)
    use blas95
    implicit none
    real::InToPoint(:),InFromPoint(:),InFromPointDirect(:)
    integer::InReviseIndexCoef!<0 反向，>0不反向
    if (InReviseIndexCoef.le.0) then
        ans=dot(-InFromPointDirect,InToPoint-InFromPoint)

    else
        ans=dot(InFromPointDirect,InToPoint-InFromPoint)
    end if    
    return
    end function
    !判定向量坐标偏差是否超过基准
    logical function IsVectorDevBeyondLimit_Tan(InBaseCoor,InMetricCoor,InLimitValueList) result(ans)
    use TAN_BasicFunction,only:IsValueDevBeyondLimit_Tan
    implicit none
    real::InBaseCoor(:),InMetricCoor(:),InLimitValueList(:)
    integer::i
    do i=1,size(InBaseCoor)
        if (IsValueDevBeyondLimit_Tan(InBaseCoor(i),InMetricCoor(i),InLimitValueList(i))) then
            ans=.true.
            return
        end if
    end do    
    ans=.false.   
    return
    end function
    logical function IsVectorDevNotBeyondLimit_Tan(InBaseCoor,InMetricCoor,InLimitValueList) result(ans)
    use TAN_BasicFunction,only:IsValueDevBeyondLimit_Tan
    implicit none
    real::InBaseCoor(:),InMetricCoor(:),InLimitValueList(:)
    integer::i
    do i=1,size(InBaseCoor)
        if (IsValueDevBeyondLimit_Tan(InBaseCoor(i),InMetricCoor(i),InLimitValueList(i))) then
            ans=.false.
            return
        end if
    end do    
    ans=.true.    
    return
    end function
    end module
    