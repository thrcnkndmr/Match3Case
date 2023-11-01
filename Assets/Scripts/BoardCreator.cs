using System.Collections.Generic;
using UnityEngine;


public class BoardCreator : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    private Pool _pool;

    private Tile[,] _tiles;
    
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
    }

    private void BoardInitialize()
    {
        CreatingNewArray(width,height);
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

    }

    private void CreatingTiles()
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var tilePiece = _pool.SpawnObject(new Vector3(i, j, 0), PoolItemType.TilePrefab, transform,
                    Quaternion.identity);
                tilePiece.name = "Tile (" + i + "," + j+ ")";
                _tiles[i, j] = tilePiece.GetComponent<Tile>();
                _tiles[i,j].TileInitialized(i,j,this);
            }
        }
    }

    private PoolItemType GetRandomItem()
    {
        var randomItemIndex = Random.Range(0, tileItemsList.Count);
        return tileItemsList[randomItemIndex];
    }

    private void PlacementOfItem(TilePieceItem tilePieceItem, int x, int y)
    {
        var itemTransform = tilePieceItem.transform;
        itemTransform.position = new Vector3(x, y,-1);
        itemTransform.transform.rotation = Quaternion.identity;
        tilePieceItem.SetCoordinates(x,y);
        
    }

    private void FillRandomToBoard()
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                
                var randomItem = _pool.SpawnObject(Vector3.zero, GetRandomItem(), null,Quaternion.identity);
                PlacementOfItem(randomItem.GetComponent<TilePieceItem>(),i,j);
            }
        }
    }

    private void OnDisable()
    {
        EventManager.OnStartGameEvent -= OnStartGame;

    }
}
