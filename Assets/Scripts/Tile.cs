using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    [SerializeField] private int rowIndex;
    [SerializeField] private int columnIndex;
  
    
    private BoardCreator _boardCreator;

   

    public void TileInitialized(int x, int y, BoardCreator board)
    {
        rowIndex = x;
        columnIndex = y;
        _boardCreator = board;
    }

}
