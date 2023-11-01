using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TilePieceItem : MonoBehaviour
{
   public int rowIndex;
   public int columnIndex;


    public void SetCoordinates(int x, int y)
    {
        rowIndex = x;
        columnIndex = y;
    }
}
