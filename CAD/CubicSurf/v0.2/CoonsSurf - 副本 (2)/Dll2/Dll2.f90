!!  Dll2.f90 
!!
!!  FUNCTIONS/SUBROUTINES exported from Dll2.dll:
!!  Dll2 - subroutine 
!!
!subroutine TestADD(InDataList,InDataNum,OutSumData)
!  !DEC$ ATTRIBUTES DLLEXPORT::TestADD
!  implicit none
!  integer::i,InDataNum
!  real(8)::InDataList(InDataNum),OutSumData
!  OutSumData=0d0
!  do i=1,InDataNum
!      OutSumData=OutSumData+InDataList(i)
!  end do
!    end subroutine TestADD
    
    
function testfunction(x,y)
  !DEC$ ATTRIBUTES DLLEXPORT::testfunction
implicit none
  REAL(8):: x(2)
  REAL(8) :: y,testfunction
  x(2)=10
  !testfunction(1)=0d0
  testfunction = (x(1)+x(2)) * y
  y=5
end function testfunction
