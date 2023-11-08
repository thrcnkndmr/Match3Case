using System;
using UnityEngine;
public class PieceItem : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;
    public PoolItemType poolItemType;
    public PieceItemMovement pieceItemMovement;

    private void Awake()
    {
        pieceItemMovement = GetComponent<PieceItemMovement>();
    }

    public void SetCoordinates(int x, int y, PoolItemType itemType)
    {
        rowIndex = x;
        columnIndex = y;
        poolItemType = itemType;
    }
}