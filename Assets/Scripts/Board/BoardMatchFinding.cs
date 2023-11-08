using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMatchFinding : MonoBehaviour
{
    private BoardCreator _boardCreator;
    private SpriteRenderer[,] _tileRenderers;
    private Pool _pool;
    private Color _defaultTileColor;


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
        _defaultTileColor = new Color(112 / 255f, 0 / 255f, 107 / 255f, 1f);
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
        EventManager.OnLevelStart += OnLevelStart;
    }

    private void OnLevelStart()
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

    public List<PieceItem> FindMatchesAt(List<PieceItem> gamePieces, int minLength = 3)
    {
        List<PieceItem> matches = new List<PieceItem>();

        foreach (PieceItem piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.rowIndex, piece.columnIndex, minLength)).ToList();
        }

        return matches;
    }

    public List<PieceItem> FindAllMatches()
    {
        List<PieceItem> combinedMatches = new List<PieceItem>();

        for (int i = 0; i < _boardCreator.width; i++)
        {
            for (int j = 0; j < _boardCreator.height; j++)
            {
                List<PieceItem> matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }

        return combinedMatches;
    }


    public void ClearPieceAt(int x, int y)
    {
        var pieceToClear = _boardCreator.PieceItems[x, y];

        if (pieceToClear != null)
        {
            _boardCreator.PieceItems[x, y] = null;
            _pool.DeactivateObject(pieceToClear.gameObject, pieceToClear.poolItemType);
            EventManager.OnFindMatchInvoker();
        }

        HighlightTileOff(x, y);
    }

    private void HighlightTileOff(int x, int y)
    {
        var spriteRenderer = _tileRenderers[x, y];
        spriteRenderer.color = _defaultTileColor;
    }

    private void HighlightTileOn(int x, int y, Color col)
    {
        var spriteRenderer = _tileRenderers[x, y];
        spriteRenderer.color = col;
    }

    public void ClearAllHighlights()
    {
        for (var x = 0; x < _boardCreator.width; x++)
        {
            for (var y = 0; y < _boardCreator.height; y++)
            {
                HighlightTileOff(x, y);
            }
        }
    }

    public void HighlightPieces(List<PieceItem> gamePieces)
    {
        ClearAllHighlights();
        foreach (var piece in gamePieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.rowIndex, piece.columnIndex, _itemTypeColors[piece.poolItemType]);
            }
        }
    }


    private void OnDisable()
    {
        EventManager.OnLevelStart -= OnLevelStart;
    }
}