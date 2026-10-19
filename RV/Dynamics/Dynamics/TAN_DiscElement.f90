    module TAN_DiscElement
    implicit none
    type:: CDiscElem_Tan
        real::DampMatrix(6,6)
        real::MassMatrix(6,6)
        integer::NodeNum
    end type
    contains
    subroutine AssignDiscMassMatrix(m,Jx,Jy,Jz,OutMassMatrix)!÷ ¡øæÿ’Û
    implicit none
    real::m,Jx,Jy,Jz,OutMassMatrix(:,:)
    OutMassMatrix(1,1) = m
    OutMassMatrix(2,2) = m
    OutMassMatrix(3,3) = m
    OutMassMatrix(4,4) = Jx
    OutMassMatrix(5,5) = Jy
    OutMassMatrix(6,6) = Jz
    return
    end subroutine
    
    subroutine AssignDiscDampMatrix(Jz,RotVel,OutDampMatrix)!◊Ëƒ·æÿ’Û£¨Õ”¬›æÿ’Û
    implicit none
    real::Jz,RotVel,OutDampMatrix(:,:)
    OutDampMatrix(4,5) = -Jz*RotVel
    OutDampMatrix(5,4) = Jz*RotVel
    return
    end subroutine
    

    
    subroutine BuildSingleDiskElemByGeom(InPara,OutDiskElem)
    use TAN_BasicPhysic
    implicit none
    real::InPara(:)
    real::Density,Diameter,Thickness,RotVel
    type(CDiscElem_Tan):: OutDiskElem
    real::m,Jx,Jy,Jz
    OutDiskElem%NodeNum=InPara(1)
    Density=InPara(2)
    Diameter=InPara(3)
    Thickness=InPara(4)
    RotVel=InPara(5)
    m = CalSolidDiskMass(Density, Diameter, Thickness)
    Jx = CalDiskJt(m, Diameter)
    Jy = Jx
    Jz = CalDiskJp(m, Diameter)
    call AssignDiscMassMatrix(m,Jx,Jy,Jz,OutDiskElem%MassMatrix)
    call AssignDiscDampMatrix(Jz,RotVel,OutDiskElem%DampMatrix)
    return
    end subroutine    
    end module