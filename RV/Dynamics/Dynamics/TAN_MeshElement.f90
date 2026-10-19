
    module TAN_MeshElement
    implicit none
    type::CMeshElem_Tan
        real::StiffMatrix(12,12)
        integer::NodeNum(2)
    end type
    contains
    subroutine AssignCylInvMeshStiffMatrix(ContactAng, SpiralAng, ri, rj,kij, OutStiffMatrix)!÷ ¡øæÿ’Û
    implicit none
    real::ContactAng, SpiralAng, ri, rj, kij, OutStiffMatrix(:,:)
	OutStiffMatrix(1,1)=kij*Cos(ContactAng)**2*Cos(SpiralAng)**2
	OutStiffMatrix(1,2)=kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(1,3)=-(kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(1,4)=0
	OutStiffMatrix(1,5)=0
	OutStiffMatrix(1,6)=kij*ri*Cos(ContactAng)*Cos(SpiralAng)**2
	OutStiffMatrix(1,7)=-(kij*Cos(ContactAng)**2*Cos(SpiralAng)**2)
	OutStiffMatrix(1,8)=-(kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(1,9)=kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(1,10)=0
	OutStiffMatrix(1,11)=0
	OutStiffMatrix(1,12)=kij*rj*Cos(ContactAng)*Cos(SpiralAng)**2
	OutStiffMatrix(2,1)=kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(2,2)=kij*Cos(SpiralAng)**2*Sin(ContactAng)**2
	OutStiffMatrix(2,3)=-(kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng))
	OutStiffMatrix(2,4)=0
	OutStiffMatrix(2,5)=0
	OutStiffMatrix(2,6)=kij*ri*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(2,7)=-(kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(2,8)=-(kij*Cos(SpiralAng)**2*Sin(ContactAng)**2)
	OutStiffMatrix(2,9)=kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng)
	OutStiffMatrix(2,10)=0
	OutStiffMatrix(2,11)=0
	OutStiffMatrix(2,12)=kij*rj*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(3,1)=-(kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(3,2)=-(kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng))
	OutStiffMatrix(3,3)=kij*Sin(SpiralAng)**2
	OutStiffMatrix(3,4)=0
	OutStiffMatrix(3,5)=0
	OutStiffMatrix(3,6)=-(kij*ri*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(3,7)=kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(3,8)=kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng)
	OutStiffMatrix(3,9)=-(kij*Sin(SpiralAng)**2)
	OutStiffMatrix(3,10)=0
	OutStiffMatrix(3,11)=0
	OutStiffMatrix(3,12)=-(kij*rj*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(4,1)=0
	OutStiffMatrix(4,2)=0
	OutStiffMatrix(4,3)=0
	OutStiffMatrix(4,4)=0
	OutStiffMatrix(4,5)=0
	OutStiffMatrix(4,6)=0
	OutStiffMatrix(4,7)=0
	OutStiffMatrix(4,8)=0
	OutStiffMatrix(4,9)=0
	OutStiffMatrix(4,10)=0
	OutStiffMatrix(4,11)=0
	OutStiffMatrix(4,12)=0
	OutStiffMatrix(5,1)=0
	OutStiffMatrix(5,2)=0
	OutStiffMatrix(5,3)=0
	OutStiffMatrix(5,4)=0
	OutStiffMatrix(5,5)=0
	OutStiffMatrix(5,6)=0
	OutStiffMatrix(5,7)=0
	OutStiffMatrix(5,8)=0
	OutStiffMatrix(5,9)=0
	OutStiffMatrix(5,10)=0
	OutStiffMatrix(5,11)=0
	OutStiffMatrix(5,12)=0
	OutStiffMatrix(6,1)=kij*ri*Cos(ContactAng)*Cos(SpiralAng)**2
	OutStiffMatrix(6,2)=kij*ri*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(6,3)=-(kij*ri*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(6,4)=0
	OutStiffMatrix(6,5)=0
	OutStiffMatrix(6,6)=kij*ri**2*Cos(SpiralAng)**2
	OutStiffMatrix(6,7)=-(kij*ri*Cos(ContactAng)*Cos(SpiralAng)**2)
	OutStiffMatrix(6,8)=-(kij*ri*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(6,9)=kij*ri*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(6,10)=0
	OutStiffMatrix(6,11)=0
	OutStiffMatrix(6,12)=kij*ri*rj*Cos(SpiralAng)**2
	OutStiffMatrix(7,1)=-(kij*Cos(ContactAng)**2*Cos(SpiralAng)**2)
	OutStiffMatrix(7,2)=-(kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(7,3)=kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(7,4)=0
	OutStiffMatrix(7,5)=0
	OutStiffMatrix(7,6)=-(kij*ri*Cos(ContactAng)*Cos(SpiralAng)**2)
	OutStiffMatrix(7,7)=kij*Cos(ContactAng)**2*Cos(SpiralAng)**2
	OutStiffMatrix(7,8)=kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(7,9)=-(kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(7,10)=0
	OutStiffMatrix(7,11)=0
	OutStiffMatrix(7,12)=-(kij*rj*Cos(ContactAng)*Cos(SpiralAng)**2)
	OutStiffMatrix(8,1)=-(kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(8,2)=-(kij*Cos(SpiralAng)**2*Sin(ContactAng)**2)
	OutStiffMatrix(8,3)=kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng)
	OutStiffMatrix(8,4)=0
	OutStiffMatrix(8,5)=0
	OutStiffMatrix(8,6)=-(kij*ri*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(8,7)=kij*Cos(ContactAng)*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(8,8)=kij*Cos(SpiralAng)**2*Sin(ContactAng)**2
	OutStiffMatrix(8,9)=-(kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng))
	OutStiffMatrix(8,10)=0
	OutStiffMatrix(8,11)=0
	OutStiffMatrix(8,12)=-(kij*rj*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(9,1)=kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(9,2)=kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng)
	OutStiffMatrix(9,3)=-(kij*Sin(SpiralAng)**2)
	OutStiffMatrix(9,4)=0
	OutStiffMatrix(9,5)=0
	OutStiffMatrix(9,6)=kij*ri*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(9,7)=-(kij*Cos(ContactAng)*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(9,8)=-(kij*Cos(SpiralAng)*Sin(ContactAng)*Sin(SpiralAng))
	OutStiffMatrix(9,9)=kij*Sin(SpiralAng)**2
	OutStiffMatrix(9,10)=0
	OutStiffMatrix(9,11)=0
	OutStiffMatrix(9,12)=kij*rj*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(10,1)=0
	OutStiffMatrix(10,2)=0
	OutStiffMatrix(10,3)=0
	OutStiffMatrix(10,4)=0
	OutStiffMatrix(10,5)=0
	OutStiffMatrix(10,6)=0
	OutStiffMatrix(10,7)=0
	OutStiffMatrix(10,8)=0
	OutStiffMatrix(10,9)=0
	OutStiffMatrix(10,10)=0
	OutStiffMatrix(10,11)=0
	OutStiffMatrix(10,12)=0
	OutStiffMatrix(11,1)=0
	OutStiffMatrix(11,2)=0
	OutStiffMatrix(11,3)=0
	OutStiffMatrix(11,4)=0
	OutStiffMatrix(11,5)=0
	OutStiffMatrix(11,6)=0
	OutStiffMatrix(11,7)=0
	OutStiffMatrix(11,8)=0
	OutStiffMatrix(11,9)=0
	OutStiffMatrix(11,10)=0
	OutStiffMatrix(11,11)=0
	OutStiffMatrix(11,12)=0
	OutStiffMatrix(12,1)=kij*rj*Cos(ContactAng)*Cos(SpiralAng)**2
	OutStiffMatrix(12,2)=kij*rj*Cos(SpiralAng)**2*Sin(ContactAng)
	OutStiffMatrix(12,3)=-(kij*rj*Cos(SpiralAng)*Sin(SpiralAng))
	OutStiffMatrix(12,4)=0
	OutStiffMatrix(12,5)=0
	OutStiffMatrix(12,6)=kij*ri*rj*Cos(SpiralAng)**2
	OutStiffMatrix(12,7)=-(kij*rj*Cos(ContactAng)*Cos(SpiralAng)**2)
	OutStiffMatrix(12,8)=-(kij*rj*Cos(SpiralAng)**2*Sin(ContactAng))
	OutStiffMatrix(12,9)=kij*rj*Cos(SpiralAng)*Sin(SpiralAng)
	OutStiffMatrix(12,10)=0
	OutStiffMatrix(12,11)=0
	OutStiffMatrix(12,12)=kij*rj**2*Cos(SpiralAng)**2
    return
    end subroutine
    
    real function CalBaseCircleRadius(PitchDiameter,NormalPreAng,SpiralAng)
    implicit none
    real::PitchDiameter,NormalPreAng,SpiralAng
    CalBaseCircleRadius=PitchDiameter/(2 *Sqrt(1 + (1/Cos(SpiralAng)**2)*(Tan(NormalPreAng)**2)))
    return
    end function
    
    
    
    subroutine BuildCylInvMeshElemByGeom(InPara,OutMeshElem)
    implicit none
    real::InPara(:)
    real::ContactAng,NormalPreAng, SpiralAng, PitchDiameterI, PitchDiameterJ, kij
    real::ri,rj
    type(CMeshElem_Tan):: OutMeshElem
    OutMeshElem%NodeNum(1)=InPara(1)
    OutMeshElem%NodeNum(2)=InPara(2)
    ContactAng=InPara(3)
    NormalPreAng=InPara(4)
    SpiralAng=InPara(5)
    PitchDiameterI=InPara(6)
    PitchDiameterJ=InPara(7)
    kij = InPara(8)
    ri = CalBaseCircleRadius(PitchDiameterI,NormalPreAng,SpiralAng)
    rj = CalBaseCircleRadius(PitchDiameterJ,NormalPreAng,SpiralAng)
    call AssignCylInvMeshStiffMatrix(ContactAng, SpiralAng,&
        ri, rj, kij,&
        OutMeshElem%StiffMatrix)
    return
    end subroutine  

    end module