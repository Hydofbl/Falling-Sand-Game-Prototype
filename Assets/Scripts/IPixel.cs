using UnityEngine;

interface IPixel
{
    public void SetDatas(int x, int y, int scaleFactor, GameObject pixelObject);

    public void ApplyPhysics(IPixel[,] pixelArr);
}
