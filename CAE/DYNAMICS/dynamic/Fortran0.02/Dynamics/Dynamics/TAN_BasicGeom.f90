
    module TAN_BasicGeom
    use TAN_GlobalPara
    implicit none    
    contains
    real function CalCircleArea(Diameter)
    real:: Diameter
    CalCircleArea=(Diameter**2)*Pi/4.0
    return
    end function
      
    real function CalCircleSecIntertiaMoment(Diameter)!圆截面惯性矩
    implicit none
    real::Diameter
    CalCircleSecIntertiaMoment=(Diameter**4.0)*Pi/64.0
    return
    end function
    
    real function CalCircleSecPolarIntertiaMoment(Diameter)!圆截面极惯性矩
    implicit none
    real::Diameter
    CalCircleSecPolarIntertiaMoment=(Diameter**4.0)*Pi/32.0
    return
    end function   
    
    end module