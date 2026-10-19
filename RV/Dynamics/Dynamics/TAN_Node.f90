
    module TAN_Node
    implicit none
    type::CNode_Tan
        logical::Freedom(6)=1
        integer::Num=0
        integer::MatrixIndex=0
    end type
    type::CNodePtr_Tan
         Type(CNode_Tan),pointer::NodePtr
    end type
    contains
    
    subroutine AssignNodePara(InNodeNum,InFreedom,OutNodeObject)
    implicit none
    type(CNode_Tan)::OutNodeObject
    logical::InFreedom(:)
    integer::InNodeNum
    OutNodeObject%Freedom=InFreedom
    OutNodeObject%Num=InNodeNum
    return
    end subroutine
    !构建节点指针列表在矩阵中的位置列表
    subroutine ConstructNodeObjectList(InNodeObjectList,OutNodeObjectList)
    implicit none
    type(CNode_Tan)::InNodeObjectList(:),OutNodeObjectList(:)
    integer,pointer::NodeFreedomNumList(:)
    integer::NodeCount
    integer::i
    !计算公式，MatrixIndex(i+1)=MatrixIndex(i)+Freedom(i)
    NodeCount=size(InNodeObjectList,1)
    allocate (NodeFreedomNumList(NodeCount))
    do i=1,NodeCount
        NodeFreedomNumList(i)=sum(abs(InNodeObjectList(i)%Freedom))
    end do
    OutNodeObjectList(1)%MatrixIndex=1
    do i=2,NodeCount
        OutNodeObjectList(i)%MatrixIndex=OutNodeObjectList(i-1)%MatrixIndex + NodeFreedomNumList(i-1)
    end do
    do i=1,NodeCount    
        OutNodeObjectList(i)%Freedom=InNodeObjectList(i)%Freedom
        OutNodeObjectList(i)%Num=InNodeObjectList(i)%Num
    end do
    deallocate (NodeFreedomNumList)
    return
    end subroutine
    
    integer function CalNodeListFreedom(InNodeObjectList)
    implicit none
    type(CNode_Tan)::InNodeObjectList(:)
    integer::i, NodeCount
    NodeCount=size(InNodeObjectList,1)
    CalNodeListFreedom=0
    do i=1,NodeCount
        CalNodeListFreedom=CalNodeListFreedom+sum(abs(InNodeObjectList(i)%Freedom))
    end do
    return
    end function
    function FindNodeObjectPointer(InNodeList,FindNodeNum)
    implicit none
    type(CNode_Tan),target::InNodeList(:)
    type(CNode_Tan),pointer::FindNodeObjectPointer
    integer::FindNodeNum
    integer::i    
    do i=1,size(InNodeList,1)
        if (FindNodeNum==InNodeList(i)%Num) then
        FindNodeObjectPointer=>InNodeList(i)
        exit
        end if
    end do
    return
    end function
    
    
    integer function FindNodeObjectIndex(InNodeList,FindNodeNum)
    implicit none
    type(CNode_Tan),target::InNodeList(:)
    integer::FindNodeNum
    integer::i    
    do i=1,size(InNodeList,1)
        if (FindNodeNum==InNodeList(i)%Num) then
        FindNodeObjectIndex=i
        exit
        end if
    end do
    return
    end function
    
    end module