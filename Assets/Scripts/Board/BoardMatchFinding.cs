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

    public List<PieceItem> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<PieceItem> matches = new List<PieceItem>();

        PieceItem startPiece = null;

        if (_boardCreator.IsWithinBounds(startX, startY))
        {
            startPiece = _boardCreator.PieceItems[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }

        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (_boardCreator.width > _boardCreator.height) ? _boardCreator.width : _boardCreator.height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!_boardCreator.IsWithinBounds(nextX, nextY))
            {
                break;
            }

            PieceItem nextPiece = _boardCreator.PieceItems[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.poolItemType == startPiece.poolItemType && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }

                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }

        return null;
    }

    private List<PieceItem> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<PieceItem> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<PieceItem> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<PieceItem>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<PieceItem>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    private List<PieceItem> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<PieceItem> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<PieceItem> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<PieceItem>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<PieceItem>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;
    }

    public List<PieceItem> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<PieceItem> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<PieceItem> vertMatches = FindVerticalMatches(x, y, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<PieceItem>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<PieceItem>();
        }

        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    private void HighlightMatches()
    {
        var allMatchedPieces = new HashSet<PieceItem>();

        for (var x = 0; x < _boardCreator.width; x++)
        {
            for (var y = 0; y < _boardCreator.height; y++)
            {
                var horizontalMatches = FindMatchesAt(x, y);
                var verticalMatches = FindMatchesAt(x, y);

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

    public void ClearPieceAt(int x, int y)
    {
        var pieceToClear = _boardCreator.PieceItems[x, y];

        if (pieceToClear != null)
        {
            _boardCreator.PieceItems[x, y] = null;
            _pool.DeactivateObject(pieceToClear.gameObject, pieceToClear.poolItemType);
        }

        HighlightTileOff(x, y);
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