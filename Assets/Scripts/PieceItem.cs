using UnityEngine;

public class PieceItem : MonoBehaviour
{
   public int rowIndex;
   public int columnIndex;


    public void SetCoordinates(int x, int y)
    {
        rowIndex = x;
        columnIndex = y;
    }
}
