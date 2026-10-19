
    module TAN_Vector
    implicit none
    contains
    !矢量基本运算
    !real function CalVectorDistance(InVector)
    !real::InVector(:)
    !CalVectorDistance=sqrt(sum(InVector(:)**2))
    !return
    !end function
    !计算矢量2范数
    real function CalVectorNorm(InVector)
    use blas95
    implicit none
    real:: InVector(:)
    !CalVectorNorm=sqrt(sum(InVector**2))
    CalVectorNorm=nrm2(InVector)
    return    
    end function
    !计算单位矢量
    subroutine CalVectorNormalize(InVector,OutVector)
    implicit none
    real:: InVector(:)
    real:: OutVector(:)
    real:: VectorNorm
    VectorNorm=CalVectorNorm(InVector)
    OutVector=InVector/VectorNorm
    return
    end subroutine
    !3维度矢量操作
    !计算矢量叉积
    subroutine CalVectorCrossProduct(InVector1,InVector2,OutVector)
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
    real function CalVectorMixedProduct(InVector1,InVector2,InVector3)
    use blas95
    implicit none
    real:: InVector1(:),InVector2(:),InVector3(:)
    real,pointer:: pTempVector(:)
    allocate(pTempVector(3))
    call CalVectorCrossProduct(InVector1,InVector2,pTempVector)
    CalVectorMixedProduct=dot(InVector3(1:3),pTempVector)
    deallocate(pTempVector)
    return
    end function
    !计算矢量球坐标
    subroutine CalVectorSpherCoor(InVector,OutVector)
    implicit none
    real:: InVector(3),OutVector(3)
    OutVector(1)=CalVectorNorm(InVector)    
    OutVector(2)=atan(InVector(2)/InVector(1))
    OutVector(3)=acos(InVector(3)/OutVector(1))
    return
    end subroutine

    

    
    real function CalVectorTanAngZAxis(InVector)
    implicit none
    integer::VectorDimension
    real:: InVector(:)
    real:: RadiusOnXYPlane   
    VectorDimension=size(InVector,1)
    RadiusOnXYPlane=CalVectorNorm(InVector(1:VectorDimension-1))
    CalVectorTanAngZAxis=RadiusOnXYPlane/InVector(VectorDimension)
    return
    end function
    
    real function CalVectorCotAngZAxis(InVector)
    implicit none
    integer::VectorDimension
    real:: InVector(:)
    real:: RadiusOnXYPlane   
    VectorDimension=size(InVector,1)
    RadiusOnXYPlane=CalVectorNorm(InVector(1:VectorDimension-1))
    CalVectorCotAngZAxis=InVector(VectorDimension)/RadiusOnXYPlane
    return
    end function

    
    


    !矢量转动,绕原点,默认旋转轴已经单位化,向量为3维
    subroutine RotVectorByAxis(InRotAng,InVector,InRotAxis,OutVector)
    use blas95
    implicit none
    real::InRotAng,InRotAxis(:),InVector(:),OutVector(:)
    real,pointer::pTempVector(:,:)
    allocate(pTempVector(3,2))
    call copy(InVector,pTempVector(:,1))
    !第一组元素
    call scal(pTempVector(:,1),cos(InRotAng))
    call CalVectorCrossProduct(InRotAxis,InVector,pTempVector(:,2))
    call axpy(pTempVector(:,2),pTempVector(:,1),sin(InRotAng))
    call axpy(InRotAxis,pTempVector(:,1),dot(InRotAxis,InVector)*(1d0-cos(InRotAng)))
    call copy(pTempVector(:,1),OutVector)
    deallocate(pTempVector)
    return
    end subroutine
    !矢量转动,绕定点
    subroutine RotVectorByAxisAndOrigin(InRotAng,InVector,InRotAxis,InRotOrigin,OutVector)
    use blas95
    implicit none
    real::InRotAng,InRotAxis(:),InRotOrigin(:),InVector(:),OutVector(:)
    real,pointer::pTempVector(:)
    allocate(pTempVector(3))    
    call RotVectorByAxis(InRotAng,InVector-InRotOrigin,InRotAxis,pTempVector)
    call axpy(InRotOrigin,pTempVector)
    call copy(pTempVector,OutVector)
    deallocate(pTempVector)
    return
    end subroutine   
    subroutine BuildArithmeticVectorList(InStartVector,InEndVector,OutVectorList)
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
    end module