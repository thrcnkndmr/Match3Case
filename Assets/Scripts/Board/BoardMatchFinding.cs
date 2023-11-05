using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardMatchFinding : MonoBehaviour
{
    private BoardMovement _boardMovement;
    private BoardCreator _boardCreator;
    private SpriteRenderer[,] _tileRenderers;

    private Dictionary<PoolItemType, Color> _itemTypeColors = new Dictionary<PoolItemType, Color>
    {
        { PoolItemType.Red, Color.red },
        { PoolItemType.Blue, Color.blue },
        { PoolItemType.Green, Color.green },
        { PoolItemType.Yellow, Color.yellow }
    };

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _boardCreator = BoardManager.Instance.boardCreator;
        _boardMovement = BoardManager.Instance.boardMovement;
    }

    private void InitializeTileRenderers()
    {
        _tileRenderers = new SpriteRenderer[_boardCreator.width, _boardCreator.height];
        for (int x = 0; x < _boardCreator.width; x++)
        {
            for (int y = 0; y < _boardCreator.height; y++)
            {
                _tileRenderers[x, y] = _boardCreator.Tiles[x, y].GetComponent<SpriteRenderer>();
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnFindMatch += OnFindMatch;
    }

    private void OnFindMatch()
    {
        InitializeTileRenderers();
        HighlightMatches();
    }

    private List<PieceItem> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<PieceItem> matches = new List<PieceItem>();

        PieceItem startPiece = _boardCreator.IsWithinBounds(startX, startY)
            ? _boardCreator.PieceItems[startX, startY]
            : null;
        if (startPiece == null) return matches;

        matches.Add(startPiece);
        int nextX, nextY;

        int maxLength = (searchDirection.x != 0) ? _boardCreator.width : _boardCreator.height;

        for (int i = 1; i < maxLength; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!_boardCreator.IsWithinBounds(nextX, nextY)) break;

            var nextPiece = _boardCreator.PieceItems[nextX, nextY];
            if (nextPiece.poolItemType == startPiece.poolItemType)
            {
                matches.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        return matches.Count >= minLength ? matches : new List<PieceItem>();
    }

    private void HighlightMatches()
    {
        ResetHighlight();

        for (int x = 0; x < _boardCreator.width; x++)
        {
            for (int y = 0; y < _boardCreator.height; y++)
            {
                List<PieceItem> horizontalMatches = FindMatches(x, y, Vector2.right);
                List<PieceItem> verticalMatches = FindMatches(x, y, Vector2.up);

                HighlightMatchedPieces(horizontalMatches);
                HighlightMatchedPieces(verticalMatches);
            }
        }
    }

    private void ResetHighlight()
    {
        for (int x = 0; x < _boardCreator.width; x++)
        {
            for (int y = 0; y < _boardCreator.height; y++)
            {
                _tileRenderers[x, y].color = new Color(_tileRenderers[x, y].color.r, _tileRenderers[x, y].color.g,
                    _tileRenderers[x, y].color.b, 1);
            }
        }
    }

    private void HighlightMatchedPieces(List<PieceItem> matchedPieces)
    {
        foreach (PieceItem piece in matchedPieces)
        {
            if (_itemTypeColors.TryGetValue(piece.poolItemType, out Color color))
            {
                _tileRenderers[piece.rowIndex, piece.columnIndex].color = color;
            }
            else
            {
                Debug.LogWarning("Color not defined for pool item type: " + piece.poolItemType);
            }
        }
    }

    private void OnDisable()
    {
        EventManager.OnFindMatch -= OnFindMatch;
    }
}