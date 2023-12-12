using UnityEngine;

public class WaterPixel : Pixel
{
    public override void ApplyPhysics(Pixel[,] pixelArr)
    {
        // if bottom is empty
        if (pixelArr[y - 1, x] == null)
        {
            TableController.Instance.MovePixel(x, y, x, y - 1);
        }
        else
        {
            // if both bottom left and bottom right are empty
            if (x > 0 && x + 1 < pixelArr.GetLength(1) && pixelArr[y - 1, x - 1] == null && pixelArr[y - 1, x + 1] == null)
            {
                int rand = Random.Range(0, 2);

                // if rand is 0, then place the pixel to the bottom left.
                if (rand == 0)
                {
                    TableController.Instance.MovePixel(x, y, x - 1, y - 1);
                }
                // if else, then place the pixel to the bottom right.
                else
                {
                    TableController.Instance.MovePixel(x, y, x + 1, y - 1);
                }
                // if rand is 1, then pass the condition and place the pixel to the left.
            }
            else if (x > 0 && pixelArr[y - 1, x - 1] == null)
            {
                TableController.Instance.MovePixel(x, y, x - 1, y - 1);
            }
            // if only bottom right is empty
            else if (x + 1 < pixelArr.GetLength(1) && pixelArr[y - 1, x + 1] == null)
            {
                TableController.Instance.MovePixel(x, y, x + 1, y - 1);
            }
            // if both bottom right and bottom left are not empty, then look for right and left
            else
            {
                // if both left and right are empty
                if (x > 0 && x + 1 < pixelArr.GetLength(1) && pixelArr[y, x - 1] == null && pixelArr[y, x + 1] == null)
                {
                    int rand = Random.Range(0, 2);

                    // if rand is 0, then place the pixel to the left.
                    if (rand == 0)
                    {
                        TableController.Instance.MovePixel(x, y, x - 1, y);
                    }
                    // if else, then place the pixel to the right.
                    else
                    {
                        TableController.Instance.MovePixel(x, y, x + 1, y);
                    }
                    // if rand is 1, then pass the condition and place the pixel to the left.
                }
                else if (x > 0 && pixelArr[y, x - 1] == null)
                {
                    TableController.Instance.MovePixel(x, y, x - 1, y);
                }
                else if(x + 1 < pixelArr.GetLength(1) && pixelArr[y, x + 1] == null)
                {
                    TableController.Instance.MovePixel(x, y, x + 1, y);
                }
            }
        } 
    }
}
