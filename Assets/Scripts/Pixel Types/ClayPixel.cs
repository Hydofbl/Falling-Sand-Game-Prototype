using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayPixel : Pixel
{
    public override void ApplyPhysics(Pixel[,] pixelArr)
    {
        // if bottom is empty
        if (pixelArr[y - 1, x] == null)
        {
            TableController.Instance.MovePixel(x, y, x, y - 1);
        }
    }
}
