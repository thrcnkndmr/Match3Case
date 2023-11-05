using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class PieceItem : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;
    public PoolItemType poolItemType;

    public void SetCoordinates(int x, int y, PoolItemType itemType)
    {
        rowIndex = x;
        columnIndex = y;
        poolItemType = itemType;
    }
}