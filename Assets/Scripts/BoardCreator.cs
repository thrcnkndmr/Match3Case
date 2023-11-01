using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    private Pool _pool;

    private Tile[,] _tiles;

    private void Awake()
    {
        _pool = Pool.Instance;
    }

    private void Start()
    {
        CreatingNewArray(width,height);
        CreatingTiles();
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
            }
        }
    }
}
