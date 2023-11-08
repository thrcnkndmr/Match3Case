using thrcnkndmr;

public class BoardManager : MonoSingleton<BoardManager>
{
    public BoardCreator boardCreator;
    public BoardMovement boardMovement;
    public BoardMatchFinding boardMatchFinding;
    public BoardCollapseAndRefill boardCollapseAndRefill;
}