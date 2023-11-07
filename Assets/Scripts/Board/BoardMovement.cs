using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BoardMovement : MonoBehaviour
{
    private Tile _clickedTile;
    private Tile _targetTile;

    [SerializeField] private float swapTime;

    public bool isMoving;
    public bool canPlayerTouch = true;


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
        if (!canPlayerTouch) yield break;
        var clickedPiece = _boardCreator.PieceItems[clickedTile.rowIndex, clickedTile.columnIndex];
        var targetPiece = _boardCreator.PieceItems[targetTile.rowIndex, targetTile.columnIndex];
        if (targetPiece.TryGetComponent(out PieceItemMovement targetPieceItemMovement) &&
            clickedPiece.TryGetComponent(out PieceItemMovement clickedPieceItemMovement))
        {
            clickedPieceItemMovement.MoveAction(targetTile.rowIndex, targetTile.columnIndex, t);
            targetPieceItemMovement.MoveAction(clickedTile.rowIndex, clickedTile.columnIndex, t);

            yield return new WaitForSeconds(t + 0.1f);

            var clickedPieceMatches = _boardMatchFinding.FindMatchesAt(clickedTile.rowIndex, clickedTile.columnIndex);
            var targetPieceMatches = _boardMatchFinding.FindMatchesAt(targetTile.rowIndex, targetTile.columnIndex);

            if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0)
            {
                clickedPieceItemMovement
                    .MoveAction(clickedTile.rowIndex, clickedTile.columnIndex, t);
                targetPieceItemMovement
                    .MoveAction(targetTile.rowIndex, targetTile.columnIndex, t);
            }
            else
            {
                yield return new WaitForSeconds(t + 0.1f);
                _boardCreator.ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());
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