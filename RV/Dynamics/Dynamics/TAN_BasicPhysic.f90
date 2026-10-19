    module TAN_BasicPhysic
    use TAN_GlobalPara
    implicit none

    contains
    real function CalSteelShearModulus(Emodulus,PRatio)!剪切模量
    implicit none
    real::Emodulus,PRatio
    CalSteelShearModulus= Emodulus / (2.0*(1 + PRatio))
    return
    end function
    
    real function CalSolidDiskMass(Density, Diameter, Thickness)!计算圆盘质量
    implicit none
    real::Density, Diameter, Thickness
    CalSolidDiskMass=(Density*(Diameter**2)*Pi*Thickness)/4
    return
    end function
    
    real function CalDiskJp(Mass, Diameter)!计算极转动惯量
    implicit none
    real::Mass, Diameter
    CalDiskJp=(Mass*(Diameter**2))/8
    return
    end function
    
    real function CalDiskJt(Mass, Diameter)!计算摆动惯量
    implicit none
    real::Mass, Diameter
    CalDiskJt=(Mass*Diameter**2)/16
    return
    end function
    
    
    
    

    end module