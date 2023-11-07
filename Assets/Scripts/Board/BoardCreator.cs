using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BoardCreator : MonoBehaviour
{
    public int width;
    public int height;

    private Pool _pool;

    private Tile[,] _tiles;
    private PieceItem[,] _pieceItems;

    public Tile[,] Tiles => _tiles;
    public PieceItem[,] PieceItems => _pieceItems;


    public List<PoolItemType> tileItemsList = new();

    private void Awake()
    {
        _pool = Pool.Instance;
    }

    private void OnEnable()
    {
        EventManager.OnStartGameEvent += OnStartGame;
    }

    private void OnStartGame()
    {
        BoardInitialize();
    }

    private void Start()
    {
        EventManager.OnStartGameInvoker();
        EventManager.OnFindMatchInvoker();
    }

    {
        _pool = Pool.Instance;
        _boardMatchFinding = BoardManager.Instance.boardMatchFinding;
        _boardMovement = BoardManager.Instance.boardMovement;
    }
    private void BoardInitialize()
    {
        CreatingNewArray(width, height);
        CreatingTiles();
        AddingItemsToList();
        FillRandomToBoard();
        
    }

    private void AddingItemsToList()
    {
        tileItemsList.Add(PoolItemType.Blue);
        tileItemsList.Add(PoolItemType.Red);
        tileItemsList.Add(PoolItemType.Yellow);
        tileItemsList.Add(PoolItemType.Green);
    }

    private void CreatingNewArray(int row, int column)
    {
        _tiles = new Tile[row, column];
        _pieceItems = new PieceItem[row, column];
    }

    private void CreatingTiles()
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var tilePiece = _pool.SpawnObject(new Vector3(i, j, 0), PoolItemType.TilePrefab, transform,
                    Quaternion.identity);
                tilePiece.name = "Tile (" + i + "," + j + ")";
                _tiles[i, j] = tilePiece.GetComponent<Tile>();
                _tiles[i, j].TileInitialized(i, j);
            }
        }
    }

    private PoolItemType GetRandomItem()
    {
        var randomItemIndex = Random.Range(0, tileItemsList.Count);
        return tileItemsList[randomItemIndex];
    }

    public void PlacementOfItem(PieceItem pieceItem, int x, int y, PoolItemType poolItemType)
    {
        var itemTransform = pieceItem.transform;
        itemTransform.position = new Vector3(x, y, -1);
        itemTransform.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            _pieceItems[x, y] = pieceItem;
        }

        pieceItem.SetCoordinates(x, y, poolItemType);
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private void FillRandomToBoard()
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                PoolItemType randomItem;
                do
                {
                    randomItem = GetRandomItem();
                } while ((i > 1 && _pieceItems[i - 1, j]?.poolItemType == randomItem &&
                          _pieceItems[i - 2, j]?.poolItemType == randomItem)
                         || (j > 1 && _pieceItems[i, j - 1]?.poolItemType == randomItem &&
                             _pieceItems[i, j - 2]?.poolItemType == randomItem));

                var newItem = _pool.SpawnObject(Vector3.zero, randomItem, null, Quaternion.identity);
                PlacementOfItem(newItem.GetComponent<PieceItem>(), i, j, randomItem);
            }
        }
    }

    private IEnumerable<PieceItem> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        var movingPieces = new List<PieceItem>();

        for (var i = 0; i < height - 1; i++)
        {
            if (PieceItems[column, i] == null)
            {
                for (var j = i + 1; j < height; j++)
                {
                    if (PieceItems[column, j] == null) continue;
                    if (PieceItems[column, j].TryGetComponent(out PieceItemMovement pieceItemMovement))
                    {
                        pieceItemMovement.MoveAction(column, i, collapseTime * (i-j));
                        PieceItems[column, i] = PieceItems[column, j];

                        PieceItems[column, i].SetCoordinates(column, i, PieceItems[column, i].poolItemType);

                        if (!movingPieces.Contains(PieceItems[column, i]))
                        {
                            movingPieces.Add(PieceItems[column, i]);
                        }

                        PieceItems[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingPieces;
    }

    private List<PieceItem> CollapseColumn(IEnumerable<PieceItem> gamePieces)
    {
        var movingPieces = new List<PieceItem>();
        var columnsToCollapse = GetColumns(gamePieces);
        return columnsToCollapse.Aggregate(movingPieces, (current, column) => current.Union(CollapseColumn(column)).ToList());
    }

    private static List<int> GetColumns(IEnumerable<PieceItem> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (var piece in gamePieces.Where(piece => !columns.Contains(piece.rowIndex)))
        {
            columns.Add(piece.rowIndex);
        }

        return columns;
    }

    public void ClearAndRefillBoard(List<PieceItem> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    private IEnumerator ClearAndRefillBoardRoutine(List<PieceItem> gamePieces)
    {

        _boardMovement.canPlayerTouch = false;
        var matches = gamePieces;

        do
        {
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            yield return null;

            //refill
           RefillBoard();
            matches = _boardMatchFinding.FindAllMatches();

            yield return new WaitForSeconds(0.5f);
        }
        while (matches.Count != 0);

        _boardMovement.canPlayerTouch = true;

    }

    private IEnumerator ClearAndCollapseRoutine(List<PieceItem> gamePieces)
    {
        var movingPieces = new List<PieceItem>();
        var matches = new List<PieceItem>();

        _boardMatchFinding.HighlightPieces(gamePieces);
        yield return new WaitForSeconds(0.25f);

        bool isFinished = false;

        while (!isFinished)
        {
            foreach (var piece in gamePieces)
            {
                _boardMatchFinding.ClearPieceAt(piece.rowIndex,piece.columnIndex);
            }
        
            yield return new WaitForSeconds(0.25f);
            movingPieces = CollapseColumn(gamePieces);
        
            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            matches = _boardMatchFinding.FindMatchesAt(movingPieces);

            if (matches.Count == 0)
            {
                isFinished = true;
              
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
    }


    private static bool IsCollapsed(List<PieceItem> gamePieces)
    {
        foreach (var piece in gamePieces)
        {

            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.columnIndex > 0.001f)
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    private void FillColumnFromAbove(int column)
    {
        for (var y = height - 1; y >= 0; y--)
        {
            if (_pieceItems[column, y] != null) continue;
            var randomItemType = GetRandomItem();
            var spawnPosition = new Vector3(column, height + y, 0); 
            var newPiece = _pool.SpawnObject(spawnPosition, randomItemType, null, Quaternion.identity).GetComponent<PieceItem>();
            PlacementOfItem(newPiece, column, y, randomItemType);
            newPiece.GetComponent<PieceItemMovement>().MoveAction(column, y, 0.5f);
        }
    }


    private void RefillBoard()
    {
        for (int column = 0; column < width; column++)
        {
            FillColumnFromAbove(column);
        }
    }


    private void OnDisable()
    {
        EventManager.OnStartGameEvent -= OnStartGame;
    }
} 