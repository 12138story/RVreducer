
    module TAN_BeamElement
    implicit none
    type::CBeamElem_TAN
        real::StiffMatrix(12,12)
        real::MassMatrix(12,12)
        real::DampMatrix(12,12)
        integer::NodeNum(2)
    end type
    contains
    

    

    
    subroutine AssignEulerBeamStiffMatrix(Ix, Iy, Ipz, Emodulus, Gmodulus, length, Area, OutStiffMatrix)!欧拉梁刚度矩阵
    implicit none
    real::Ix, Iy, Ipz, Emodulus, Gmodulus, length, Area
    real::OutStiffMatrix(:,:)
	OutStiffMatrix(1,1)=(12*Emodulus*Iy)/length**3
	OutStiffMatrix(1,2)=0
	OutStiffMatrix(1,3)=0
	OutStiffMatrix(1,4)=0
	OutStiffMatrix(1,5)=(6*Emodulus*Iy)/length**2
	OutStiffMatrix(1,6)=0
	OutStiffMatrix(1,7)=(-12*Emodulus*Iy)/length**3
	OutStiffMatrix(1,8)=0
	OutStiffMatrix(1,9)=0
	OutStiffMatrix(1,10)=0
	OutStiffMatrix(1,11)=(6*Emodulus*Iy)/length**2
	OutStiffMatrix(1,12)=0
	OutStiffMatrix(2,1)=0
	OutStiffMatrix(2,2)=(12*Emodulus*Ix)/length**3
	OutStiffMatrix(2,3)=0
	OutStiffMatrix(2,4)=(-6*Emodulus*Ix)/length**2
	OutStiffMatrix(2,5)=0
	OutStiffMatrix(2,6)=0
	OutStiffMatrix(2,7)=0
	OutStiffMatrix(2,8)=(-12*Emodulus*Ix)/length**3
	OutStiffMatrix(2,9)=0
	OutStiffMatrix(2,10)=(-6*Emodulus*Ix)/length**2
	OutStiffMatrix(2,11)=0
	OutStiffMatrix(2,12)=0
	OutStiffMatrix(3,1)=0
	OutStiffMatrix(3,2)=0
	OutStiffMatrix(3,3)=(Area*Emodulus)/length
	OutStiffMatrix(3,4)=0
	OutStiffMatrix(3,5)=0
	OutStiffMatrix(3,6)=0
	OutStiffMatrix(3,7)=0
	OutStiffMatrix(3,8)=0
	OutStiffMatrix(3,9)=-((Area*Emodulus)/length)
	OutStiffMatrix(3,10)=0
	OutStiffMatrix(3,11)=0
	OutStiffMatrix(3,12)=0
	OutStiffMatrix(4,1)=0
	OutStiffMatrix(4,2)=(-6*Emodulus*Ix)/length**2
	OutStiffMatrix(4,3)=0
	OutStiffMatrix(4,4)=(4*Emodulus*Ix)/length
	OutStiffMatrix(4,5)=0
	OutStiffMatrix(4,6)=0
	OutStiffMatrix(4,7)=0
	OutStiffMatrix(4,8)=(6*Emodulus*Ix)/length**2
	OutStiffMatrix(4,9)=0
	OutStiffMatrix(4,10)=(2*Emodulus*Ix)/length
	OutStiffMatrix(4,11)=0
	OutStiffMatrix(4,12)=0
	OutStiffMatrix(5,1)=(6*Emodulus*Iy)/length**2
	OutStiffMatrix(5,2)=0
	OutStiffMatrix(5,3)=0
	OutStiffMatrix(5,4)=0
	OutStiffMatrix(5,5)=(4*Emodulus*Iy)/length
	OutStiffMatrix(5,6)=0
	OutStiffMatrix(5,7)=(-6*Emodulus*Iy)/length**2
	OutStiffMatrix(5,8)=0
	OutStiffMatrix(5,9)=0
	OutStiffMatrix(5,10)=0
	OutStiffMatrix(5,11)=(2*Emodulus*Iy)/length
	OutStiffMatrix(5,12)=0
	OutStiffMatrix(6,1)=0
	OutStiffMatrix(6,2)=0
	OutStiffMatrix(6,3)=0
	OutStiffMatrix(6,4)=0
	OutStiffMatrix(6,5)=0
	OutStiffMatrix(6,6)=(Gmodulus*Ipz)/length
	OutStiffMatrix(6,7)=0
	OutStiffMatrix(6,8)=0
	OutStiffMatrix(6,9)=0
	OutStiffMatrix(6,10)=0
	OutStiffMatrix(6,11)=0
	OutStiffMatrix(6,12)=-((Gmodulus*Ipz)/length)
	OutStiffMatrix(7,1)=(-12*Emodulus*Iy)/length**3
	OutStiffMatrix(7,2)=0
	OutStiffMatrix(7,3)=0
	OutStiffMatrix(7,4)=0
	OutStiffMatrix(7,5)=(-6*Emodulus*Iy)/length**2
	OutStiffMatrix(7,6)=0
	OutStiffMatrix(7,7)=(12*Emodulus*Iy)/length**3
	OutStiffMatrix(7,8)=0
	OutStiffMatrix(7,9)=0
	OutStiffMatrix(7,10)=0
	OutStiffMatrix(7,11)=(-6*Emodulus*Iy)/length**2
	OutStiffMatrix(7,12)=0
	OutStiffMatrix(8,1)=0
	OutStiffMatrix(8,2)=(-12*Emodulus*Ix)/length**3
	OutStiffMatrix(8,3)=0
	OutStiffMatrix(8,4)=(6*Emodulus*Ix)/length**2
	OutStiffMatrix(8,5)=0
	OutStiffMatrix(8,6)=0
	OutStiffMatrix(8,7)=0
	OutStiffMatrix(8,8)=(12*Emodulus*Ix)/length**3
	OutStiffMatrix(8,9)=0
	OutStiffMatrix(8,10)=(6*Emodulus*Ix)/length**2
	OutStiffMatrix(8,11)=0
	OutStiffMatrix(8,12)=0
	OutStiffMatrix(9,1)=0
	OutStiffMatrix(9,2)=0
	OutStiffMatrix(9,3)=-((Area*Emodulus)/length)
	OutStiffMatrix(9,4)=0
	OutStiffMatrix(9,5)=0
	OutStiffMatrix(9,6)=0
	OutStiffMatrix(9,7)=0
	OutStiffMatrix(9,8)=0
	OutStiffMatrix(9,9)=(Area*Emodulus)/length
	OutStiffMatrix(9,10)=0
	OutStiffMatrix(9,11)=0
	OutStiffMatrix(9,12)=0
	OutStiffMatrix(10,1)=0
	OutStiffMatrix(10,2)=(-6*Emodulus*Ix)/length**2
	OutStiffMatrix(10,3)=0
	OutStiffMatrix(10,4)=(2*Emodulus*Ix)/length
	OutStiffMatrix(10,5)=0
	OutStiffMatrix(10,6)=0
	OutStiffMatrix(10,7)=0
	OutStiffMatrix(10,8)=(6*Emodulus*Ix)/length**2
	OutStiffMatrix(10,9)=0
	OutStiffMatrix(10,10)=(4*Emodulus*Ix)/length
	OutStiffMatrix(10,11)=0
	OutStiffMatrix(10,12)=0
	OutStiffMatrix(11,1)=(6*Emodulus*Iy)/length**2
	OutStiffMatrix(11,2)=0
	OutStiffMatrix(11,3)=0
	OutStiffMatrix(11,4)=0
	OutStiffMatrix(11,5)=(2*Emodulus*Iy)/length
	OutStiffMatrix(11,6)=0
	OutStiffMatrix(11,7)=(-6*Emodulus*Iy)/length**2
	OutStiffMatrix(11,8)=0
	OutStiffMatrix(11,9)=0
	OutStiffMatrix(11,10)=0
	OutStiffMatrix(11,11)=(4*Emodulus*Iy)/length
	OutStiffMatrix(11,12)=0
	OutStiffMatrix(12,1)=0
	OutStiffMatrix(12,2)=0
	OutStiffMatrix(12,3)=0
	OutStiffMatrix(12,4)=0
	OutStiffMatrix(12,5)=0
	OutStiffMatrix(12,6)=-((Gmodulus*Ipz)/length)
	OutStiffMatrix(12,7)=0
	OutStiffMatrix(12,8)=0
	OutStiffMatrix(12,9)=0
	OutStiffMatrix(12,10)=0
	OutStiffMatrix(12,11)=0
	OutStiffMatrix(12,12)=(Gmodulus*Ipz)/length
    return
    end subroutine
    
    
    
    
    subroutine AssignEulerBeamMassMatrix(Density, Ipz, length, Area, OutMassMatrix)!欧拉梁质量矩阵
    implicit none
    real::Density, Ipz, length, Area
    real::OutMassMatrix(:,:)
    OutMassMatrix(1,1)=(13*Area*Density*length)/35.
	OutMassMatrix(1,2)=0
	OutMassMatrix(1,3)=0
	OutMassMatrix(1,4)=0
	OutMassMatrix(1,5)=(11*Area*Density*length**2)/210.
	OutMassMatrix(1,6)=0
	OutMassMatrix(1,7)=(9*Area*Density*length)/70.
	OutMassMatrix(1,8)=0
	OutMassMatrix(1,9)=0
	OutMassMatrix(1,10)=0
	OutMassMatrix(1,11)=(-13*Area*Density*length**2)/420.
	OutMassMatrix(1,12)=0
	OutMassMatrix(2,1)=0
	OutMassMatrix(2,2)=(13*Area*Density*length)/35.
	OutMassMatrix(2,3)=0
	OutMassMatrix(2,4)=(-11*Area*Density*length**2)/210.
	OutMassMatrix(2,5)=0
	OutMassMatrix(2,6)=0
	OutMassMatrix(2,7)=0
	OutMassMatrix(2,8)=(9*Area*Density*length)/70.
	OutMassMatrix(2,9)=0
	OutMassMatrix(2,10)=(13*Area*Density*length**2)/420.
	OutMassMatrix(2,11)=0
	OutMassMatrix(2,12)=0
	OutMassMatrix(3,1)=0
	OutMassMatrix(3,2)=0
	OutMassMatrix(3,3)=(Area*Density*length)/3.
	OutMassMatrix(3,4)=0
	OutMassMatrix(3,5)=0
	OutMassMatrix(3,6)=0
	OutMassMatrix(3,7)=0
	OutMassMatrix(3,8)=0
	OutMassMatrix(3,9)=(Area*Density*length)/6.
	OutMassMatrix(3,10)=0
	OutMassMatrix(3,11)=0
	OutMassMatrix(3,12)=0
	OutMassMatrix(4,1)=0
	OutMassMatrix(4,2)=(-11*Area*Density*length**2)/210.
	OutMassMatrix(4,3)=0
	OutMassMatrix(4,4)=(Area*Density*length**3)/105.
	OutMassMatrix(4,5)=0
	OutMassMatrix(4,6)=0
	OutMassMatrix(4,7)=0
	OutMassMatrix(4,8)=(-13*Area*Density*length**2)/420.
	OutMassMatrix(4,9)=0
	OutMassMatrix(4,10)=-(Area*Density*length**3)/140.
	OutMassMatrix(4,11)=0
	OutMassMatrix(4,12)=0
	OutMassMatrix(5,1)=(11*Area*Density*length**2)/210.
	OutMassMatrix(5,2)=0
	OutMassMatrix(5,3)=0
	OutMassMatrix(5,4)=0
	OutMassMatrix(5,5)=(Area*Density*length**3)/105.
	OutMassMatrix(5,6)=0
	OutMassMatrix(5,7)=(13*Area*Density*length**2)/420.
	OutMassMatrix(5,8)=0
	OutMassMatrix(5,9)=0
	OutMassMatrix(5,10)=0
	OutMassMatrix(5,11)=-(Area*Density*length**3)/140.
	OutMassMatrix(5,12)=0
	OutMassMatrix(6,1)=0
	OutMassMatrix(6,2)=0
	OutMassMatrix(6,3)=0
	OutMassMatrix(6,4)=0
	OutMassMatrix(6,5)=0
	OutMassMatrix(6,6)=(Density*Ipz*length)/3.
	OutMassMatrix(6,7)=0
	OutMassMatrix(6,8)=0
	OutMassMatrix(6,9)=0
	OutMassMatrix(6,10)=0
	OutMassMatrix(6,11)=0
	OutMassMatrix(6,12)=(Density*Ipz*length)/6.
	OutMassMatrix(7,1)=(9*Area*Density*length)/70.
	OutMassMatrix(7,2)=0
	OutMassMatrix(7,3)=0
	OutMassMatrix(7,4)=0
	OutMassMatrix(7,5)=(13*Area*Density*length**2)/420.
	OutMassMatrix(7,6)=0
	OutMassMatrix(7,7)=(13*Area*Density*length)/35.
	OutMassMatrix(7,8)=0
	OutMassMatrix(7,9)=0
	OutMassMatrix(7,10)=0
	OutMassMatrix(7,11)=(-11*Area*Density*length**2)/210.
	OutMassMatrix(7,12)=0
	OutMassMatrix(8,1)=0
	OutMassMatrix(8,2)=(9*Area*Density*length)/70.
	OutMassMatrix(8,3)=0
	OutMassMatrix(8,4)=(-13*Area*Density*length**2)/420.
	OutMassMatrix(8,5)=0
	OutMassMatrix(8,6)=0
	OutMassMatrix(8,7)=0
	OutMassMatrix(8,8)=(13*Area*Density*length)/35.
	OutMassMatrix(8,9)=0
	OutMassMatrix(8,10)=(11*Area*Density*length**2)/210.
	OutMassMatrix(8,11)=0
	OutMassMatrix(8,12)=0
	OutMassMatrix(9,1)=0
	OutMassMatrix(9,2)=0
	OutMassMatrix(9,3)=(Area*Density*length)/6.
	OutMassMatrix(9,4)=0
	OutMassMatrix(9,5)=0
	OutMassMatrix(9,6)=0
	OutMassMatrix(9,7)=0
	OutMassMatrix(9,8)=0
	OutMassMatrix(9,9)=(Area*Density*length)/3.
	OutMassMatrix(9,10)=0
	OutMassMatrix(9,11)=0
	OutMassMatrix(9,12)=0
	OutMassMatrix(10,1)=0
	OutMassMatrix(10,2)=(13*Area*Density*length**2)/420.
	OutMassMatrix(10,3)=0
	OutMassMatrix(10,4)=-(Area*Density*length**3)/140.
	OutMassMatrix(10,5)=0
	OutMassMatrix(10,6)=0
	OutMassMatrix(10,7)=0
	OutMassMatrix(10,8)=(11*Area*Density*length**2)/210.
	OutMassMatrix(10,9)=0
	OutMassMatrix(10,10)=(Area*Density*length**3)/105.
	OutMassMatrix(10,11)=0
	OutMassMatrix(10,12)=0
	OutMassMatrix(11,1)=(-13*Area*Density*length**2)/420.
	OutMassMatrix(11,2)=0
	OutMassMatrix(11,3)=0
	OutMassMatrix(11,4)=0
	OutMassMatrix(11,5)=-(Area*Density*length**3)/140.
	OutMassMatrix(11,6)=0
	OutMassMatrix(11,7)=(-11*Area*Density*length**2)/210.
	OutMassMatrix(11,8)=0
	OutMassMatrix(11,9)=0
	OutMassMatrix(11,10)=0
	OutMassMatrix(11,11)=(Area*Density*length**3)/105.
	OutMassMatrix(11,12)=0
	OutMassMatrix(12,1)=0
	OutMassMatrix(12,2)=0
	OutMassMatrix(12,3)=0
	OutMassMatrix(12,4)=0
	OutMassMatrix(12,5)=0
	OutMassMatrix(12,6)=(Density*Ipz*length)/6.
	OutMassMatrix(12,7)=0
	OutMassMatrix(12,8)=0
	OutMassMatrix(12,9)=0
	OutMassMatrix(12,10)=0
	OutMassMatrix(12,11)=0
	OutMassMatrix(12,12)=(Density*Ipz*length)/3.
    return
    end subroutine
    
    subroutine BuildSingleEulerBeamElem(InPara, OutBeamElem)!建立单轴单元
    use TAN_BasicGeom
    use TAN_BasicPhysic
    implicit none
    real::InPara(:)
    real:: Density, Diameter, length, Emodulus, PRatio
    real:: Ix, Iy, Ipz, Gmodulus, Area
    type(CBeamElem_Tan):: OutBeamElem
    OutBeamElem%NodeNum(1)=InPara(1)
    OutBeamElem%NodeNum(2)=InPara(2)
    Density=InPara(3)
    Diameter=InPara(4)
    length=InPara(5)
    Emodulus=InPara(6)
    PRatio=InPara(7)
    Ipz=CalCircleSecPolarIntertiaMoment(Diameter)
    Area=CalCircleArea(Diameter)
    call AssignEulerBeamMassMatrix(Density, Ipz, length, Area, OutBeamElem%MassMatrix)
    Ix=CalCircleSecIntertiaMoment(Diameter)
    Iy=CalCircleSecIntertiaMoment(Diameter)
    Gmodulus=CalSteelShearModulus(Emodulus,PRatio)
    call AssignEulerBeamStiffMatrix(Ix, Iy, Ipz, Emodulus,Gmodulus,length,Area,OutBeamElem%StiffMatrix)
    return
    end subroutine
    
    end module
    