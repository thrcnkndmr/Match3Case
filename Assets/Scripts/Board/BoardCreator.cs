using System.Collections;
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
        Init();
    }

    private void OnEnable()
    {
        EventManager.OnStartGameEvent += OnStartGame;
        EventManager.OnLevelStart += OnLevelStart;
    }

    private void OnLevelStart()
    {
        BoardInitialize();
    }

    private void OnStartGame()
    {
       // BoardInitialize();
       Debug.Log("dur");
    }

    private void Start()
    {
        EventManager.OnStartGameInvoker();
    }

    private void BoardReset()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (_pieceItems[i, j] != null)
                {
                    _pool.DeactivateObject(_pieceItems[i, j].gameObject, _pieceItems[i, j].poolItemType);
                    _pieceItems[i, j] = null;
                }
            
                if (_tiles[i, j] != null)
                {
                    _pool.DeactivateObject(_tiles[i,j].gameObject, PoolItemType.TilePrefab);
                    _tiles[i, j] = null;
                }
            }
        }
    }

    private void Init()
    {
        _pool = Pool.Instance;
        
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

    public PoolItemType GetRandomItem()
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
    
    private void OnDisable()
    {
        EventManager.OnStartGameEvent -= OnStartGame;
    }
} 