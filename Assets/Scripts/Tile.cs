using UnityEngine;

public class Tile : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;


    private BoardCreator _boardCreator;
    private BoardMovement _boardMovement;


    public void TileInitialized(int x, int y)
    {
        rowIndex = x;
        columnIndex = y;
        _boardCreator = BoardManager.Instance.boardCreator;
        _boardMovement = BoardManager.Instance.boardMovement;
    }

    private void OnMouseDown()
    {
        if (_boardCreator != null)
        {
            _boardMovement.ClickedItem(this);
        }
    }

    private void OnMouseEnter()
    {
        if (_boardCreator != null)
        {
            _boardMovement.DragToItem(this);
        }
    }

    private void OnMouseUp()
    {
        if (_boardMovement != null)
        {
            _boardMovement.ReleaseTile();
        }
    }
}