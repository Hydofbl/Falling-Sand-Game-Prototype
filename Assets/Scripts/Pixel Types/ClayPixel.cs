public class ClayPixel : Pixel
{
    public override void ApplyPhysics(Pixel[,] pixelArr)
    {
        if(y <= 0) 
            return;

        // if bottom is empty
        if (pixelArr[y - 1, x] == null)
        {
            TableController.Instance.MovePixel(x, y, x, y - 1);
        }
        else
        {
            if (pixelArr[y - 1, x].type.Equals(Type.Water))
            {
                TableController.Instance.ChangePixels(x, y, x, y - 1);
            }
        }
    }
}
