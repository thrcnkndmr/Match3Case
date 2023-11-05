using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMatchFinding : MonoBehaviour
{
    private BoardCreator _boardCreator;
    private SpriteRenderer[,] _tileRenderers;
    private Pool _pool;


    private readonly Dictionary<PoolItemType, Color> _itemTypeColors = new Dictionary<PoolItemType, Color>
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
        _pool = Pool.Instance;
    }

    private void InitializeTileRenderers()
    {
        _tileRenderers = new SpriteRenderer[_boardCreator.width, _boardCreator.height];
        for (var x = 0; x < _boardCreator.width; x++)
        {
            for (var y = 0; y < _boardCreator.height; y++)
            {
                _tileRenderers[x, y] = _boardCreator.Tiles[x, y].GetComponent<SpriteRenderer>();
            }
        }
    }

    private void OnEnable()
    {
        EventManager.OnFindMatch += OnFindMatch;
        EventManager.OnMovedItem += OnMovedItem;
    }

    private void OnMovedItem()
    {
        throw new System.NotImplementedException();
    }

    private void OnFindMatch()
    {
        InitializeTileRenderers();
    }

    public List<PieceItem> FindMatchesAt(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        {
            var matches = new List<PieceItem>();
            var startPiece = _boardCreator.IsWithinBounds(startX, startY)
                ? _boardCreator.PieceItems[startX, startY]
                : null;
            if (startPiece == null) return matches;

            matches.Add(startPiece);

            var maxLength = (searchDirection.x != 0) ? _boardCreator.width : _boardCreator.height;

            for (var i = 1; i < maxLength; i++)
            {
                var nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
                var nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

                if (!_boardCreator.IsWithinBounds(nextX, nextY)) break;

                var nextPiece = _boardCreator.PieceItems[nextX, nextY];
                if (nextPiece != null && nextPiece.poolItemType == startPiece.poolItemType)
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
    }
    
    public bool FindAndClearMatches(Tile clickedTile, Tile targetTile)
    {
        var allMatches = new HashSet<PieceItem>();
        allMatches.UnionWith(FindMatchesAt(clickedTile.rowIndex, clickedTile.columnIndex,
            Vector2.right));
        allMatches.UnionWith(
            FindMatchesAt(clickedTile.rowIndex, clickedTile.columnIndex, Vector2.up));
        allMatches.UnionWith(FindMatchesAt(targetTile.rowIndex, targetTile.columnIndex,
            Vector2.right));
        allMatches.UnionWith(FindMatchesAt(targetTile.rowIndex, targetTile.columnIndex, Vector2.up));
        if (allMatches.Count <= 0) return false;
        foreach (var piece in allMatches)
        {
            ClearPieceAt(piece.rowIndex, piece.columnIndex);
        }

        return true;
    }
    
    private void ClearPieceAt(int x, int y)
    {
        var pieceToClear = _boardCreator.PieceItems[x, y];

        if (pieceToClear != null)
        {
            _boardCreator.PieceItems[x, y] = null;
            _boardCreator.Tiles[x, y] = null;
            _pool.DeactivateObject(pieceToClear.gameObject, pieceToClear.poolItemType);
        }

        HighlightTileOff(x, y);
    }

    private void HighlightMatches()
    {
        var allMatchedPieces = new HashSet<PieceItem>();

        for (var x = 0; x < _boardCreator.width; x++)
        {
            for (var y = 0; y < _boardCreator.height; y++)
            {
                var horizontalMatches = FindMatchesAt(x, y, Vector2.right);
                var verticalMatches = FindMatchesAt(x, y, Vector2.up);

                foreach (var piece in horizontalMatches)
                {
                    allMatchedPieces.Add(piece);
                }

                foreach (var piece in verticalMatches)
                {
                    allMatchedPieces.Add(piece);
                }
            }
        }

        foreach (var piece in allMatchedPieces)
        {
            if (_itemTypeColors.TryGetValue(piece.poolItemType, out var color))
            {
                HighlightTileOn(piece.rowIndex, piece.columnIndex, color);
            }
            else
            {
                Debug.LogWarning("Color not defined for pool item type: " + piece.poolItemType);
            }
        }

        for (var x = 0; x < _boardCreator.width; x++)
        {
            for (var y = 0; y < _boardCreator.height; y++)
            {
                if (!allMatchedPieces.Any(p => p.rowIndex == x && p.columnIndex == y))
                {
                    HighlightTileOff(x, y);
                }
            }
        }
    }

    public void HighlightTileOff(int x, int y)
    {
        var spriteRenderer = _tileRenderers[x, y];
        spriteRenderer.color = new Color(0.439f, 0f, 0.420f, 1f);
    }

    private void HighlightTileOn(int x, int y, Color color)
    {
        var spriteRenderer = _tileRenderers[x, y];
        spriteRenderer.color = color;
    }

    private void OnDisable()
    {
        EventManager.OnFindMatch -= OnFindMatch;
        EventManager.OnMovedItem -= OnMovedItem;

    }
}