using UnityEngine;

public class SandPixel : Pixel
{
    [Header("Color")]
    [SerializeField] private Color[] colors;

    private void Start()
    {
        int rand = Random.Range(0, colors.Length);
        GetComponent<SpriteRenderer>().color = colors[rand];
    }

    public override void ApplyPhysics(Pixel[,] pixelArr)
    {
        // if bottom is empty
        if (pixelArr[y - 1, x] == null)
        {
            TableController.Instance.MovePixel(x, y, x, y - 1);
        }
        else
        {
            // NOTE: First check if bottom is water or not. Or sand moves diagonally by ignoring water.
            if (pixelArr[y - 1, x].type.Equals(Type.Water))
            {
                TableController.Instance.ChangePixels(x, y, x, y - 1);
                /*
                if (pixelArr[y, x - 1] == null && pixelArr[y, x + 1] == null)
                {
                    int rand = Random.Range(0, 2);

                    if (rand == 0)
                    {
                        TableController.Instance.MovePixel(x, y - 1, x - 1, y);
                    }
                    else
                    {
                        TableController.Instance.MovePixel(x, y - 1, x + 1, y);
                    }
                }
                else if (pixelArr[y, x - 1] == null)
                {
                    TableController.Instance.MovePixel(x, y - 1, x - 1, y - 1);
                }
                else if (pixelArr[y, x + 1] == null)
                {
                    TableController.Instance.MovePixel(x, y - 1, x + 1, y - 1);
                }
                else
                {
                    TableController.Instance.ChangePixels(x, y, x, y - 1);
                }
                */
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
                // if only bottom left is empty
                else if (x > 0 && pixelArr[y - 1, x - 1] == null)
                {
                    TableController.Instance.MovePixel(x, y, x - 1, y - 1);
                }
                // if only bottom rgiht is empty
                else if (x + 1 < pixelArr.GetLength(1) && pixelArr[y - 1, x + 1] == null)
                {
                    TableController.Instance.MovePixel(x, y, x + 1, y - 1);
                }
            }
            //// Fix the sand-water interaction

        }
    }
}
