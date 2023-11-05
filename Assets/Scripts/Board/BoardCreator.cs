using System.Collections.Generic;
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
                var randomItem = GetRandomItem();
                var newItem = _pool.SpawnObject(Vector3.zero, randomItem, null, Quaternion.identity);
                PlacementOfItem(newItem.GetComponent<PieceItem>(), i, j, randomItem);
            }
        }
    }

    private void OnDisable()
    {
        EventManager.OnStartGameEvent -= OnStartGame;
    }
}