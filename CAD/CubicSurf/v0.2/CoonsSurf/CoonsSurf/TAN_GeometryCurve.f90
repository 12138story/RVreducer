 
    module TAN_GeometryCurve
    implicit none
    contains
    real function FEllipseEqu_Tan(InX,InY,InA,InB) result(ans)
    implicit none
    real::InX,InY,InA,InB
    ans=1d0-(InX/InA)**2-(InY/InB)**2
    return
    end function
    end module