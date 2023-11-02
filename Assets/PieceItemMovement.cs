using DG.Tweening;
using UnityEngine;

public class PieceItemMovement : MonoBehaviour
{

    private Tile _thisTile;
    private Tile _clickedTile;
    private Tile _targetTile;

    private bool _isMoving;


    private void Awake()
    {
        _thisTile = GetComponent<Tile>();
    }

    public void OnMouseEnter()
    {
        DragToItem(_thisTile);

    }

    public void OnMouseUp()
    {
        ReleaseTile();
    }

    public void OnMouseDown()
    {
        ClickedItem(_thisTile);
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
        if (_clickedTile != null)
        {
            _targetTile = tile;
        }


    }

    public void ReleaseTile()
    {
        if (_clickedTile != null && _targetTile != null)
        {
            SwitchTiles(_clickedTile, _targetTile);
        }


    }

    private void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        if (!_isMoving)
        {
            Move(clickedTile.transform.position, targetTile.transform.position, 0.5f);
        }




        _clickedTile = null;
        _targetTile = null;
    }



    public void Move(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        // Store the current tiles in local variables to avoid null references
        Tile movingClickedTile = _clickedTile;
        Tile movingTargetTile = _targetTile;

        // Start the move using the DOTween plugin
        movingClickedTile.transform.DOMove(targetPosition, duration).OnUpdate(() =>
        {
            // This might not be necessary every frame; consider moving it out of the OnUpdate
            movingTargetTile.transform.DOMove(startPosition, duration);
            movingClickedTile.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), duration);
            movingTargetTile.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), duration);
        }).OnComplete(() =>
        {
            // After the move is complete, scale back the tiles to their original size
            movingClickedTile.transform.DOScale(Vector3.one, duration);
            movingTargetTile.transform.DOScale(Vector3.one, duration).OnComplete(() =>
            {
                // After the animations are done, reset these to null to allow for a new move
                _clickedTile = null;
                _targetTile = null;
                _isMoving = false; // Allow new moves again
            });
        });
    }
}
    

