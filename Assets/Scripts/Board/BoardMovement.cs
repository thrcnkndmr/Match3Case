using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardMovement : MonoBehaviour
{
    private Tile _clickedTile;
    private Tile _targetTile;

    [SerializeField] private float swapTime;

    public bool isMoving;

    private BoardCreator _boardCreator;
    private BoardMatchFinding _boardMatchFinding;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _boardCreator = BoardManager.Instance.boardCreator;
        _boardMatchFinding = BoardManager.Instance.boardMatchFinding;
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

    private void SwitchTiles(Tile clickedTile, Tile targetTile, float time)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile, time));
    }

    private IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile, float t)
    {
        var clickedPiece = _boardCreator.PieceItems[clickedTile.rowIndex, clickedTile.columnIndex];
        var targetPiece = _boardCreator.PieceItems[targetTile.rowIndex, targetTile.columnIndex];
        if (targetPiece != null && clickedPiece != null)
        {
            clickedPiece.GetComponent<PieceItemMovement>().MoveAction(targetTile.rowIndex, targetTile.columnIndex, t);
            targetPiece.GetComponent<PieceItemMovement>().MoveAction(clickedTile.rowIndex, clickedTile.columnIndex, t);


            yield return new WaitForSeconds(0.6f);

            if (!_boardMatchFinding.FindAndClearMatches(clickedTile, targetTile))
            {
                clickedPiece.GetComponent<PieceItemMovement>()
                    .MoveAction(clickedTile.rowIndex, clickedTile.columnIndex, t);
                targetPiece.GetComponent<PieceItemMovement>()
                    .MoveAction(targetTile.rowIndex, targetTile.columnIndex, t);
                yield return new WaitForSeconds(0.6f);
            }
        }

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