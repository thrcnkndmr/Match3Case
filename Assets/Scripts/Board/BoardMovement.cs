using UnityEngine;

public class BoardMovement : MonoBehaviour
{
    private Tile _clickedTile;
    private Tile _targetTile;

    [SerializeField] private float swapTime;

    public bool isMoving;

    private BoardCreator _boardCreator;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _boardCreator = BoardManager.Instance.boardCreator;
    }

    public void ClickedItem(Tile tile)
    {
        if (_clickedTile == null)
        {
            _clickedTile = tile;
        }
    }

    public void DragToItem(Tile tile)
    {
        if (_clickedTile != null && IsNextTo(tile, _clickedTile))
        {
            _targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (_clickedTile != null && _targetTile != null)
        {
            SwitchTiles(_clickedTile, _targetTile, swapTime);
        }
    }

    private void SwitchTiles(Tile clickedTile, Tile targetTile, float t)
    {
        var clickedPiece = _boardCreator.PieceItems[clickedTile.rowIndex, clickedTile.columnIndex];
        var targetPiece = _boardCreator.PieceItems[targetTile.rowIndex, targetTile.columnIndex];

        clickedPiece.GetComponent<PieceItemMovement>().MoveAction(targetTile.rowIndex, targetTile.columnIndex, t);
        targetPiece.GetComponent<PieceItemMovement>().MoveAction(clickedTile.rowIndex, clickedTile.columnIndex, t);
        _clickedTile = null;
        _targetTile = null;
    }


    private bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.rowIndex - end.rowIndex) == 1 && start.columnIndex == end.columnIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.columnIndex - end.columnIndex) == 1 && start.rowIndex == end.rowIndex)
        {
            return true;
        }

        return false;
    }
}