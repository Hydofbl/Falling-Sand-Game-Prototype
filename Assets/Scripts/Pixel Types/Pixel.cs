using UnityEngine;

public enum Type
{
    Sand,
    Water
}

public class Pixel : MonoBehaviour
{
    // Positions on array
    protected int x;
    protected int y;

    // For now it is not neccessary
    protected GameObject pixelObject;

    public void SetDatas(int x, int y, GameObject pixelObject)
    {
        this.pixelObject = pixelObject;
        SetPositions(x, y);
    }

    public void SetPositions(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public virtual void ApplyPhysics(Pixel[,] pixelArr)
    {
        // if bottom is empty
        if (pixelArr[y - 1, x] == null)
        {
            TableController.Instance.MovePixel(x, y, x, y - 1);
        }
        else
        {
            if (x > 0 && pixelArr[y - 1, x - 1] == null)
            {
                TableController.Instance.MovePixel(x, y, x - 1, y - 1);
            }
            // if only bottom rgiht is empty
            else
            {
                if (x + 1 < pixelArr.GetLength(1) && pixelArr[y - 1, x + 1] == null)
                {
                    TableController.Instance.MovePixel(x, y, x + 1, y - 1);
                }
            }
        }
    }
}