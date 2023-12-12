using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField] private GameObject pixelPref;
    [SerializeField] private Transform pixelParent;
    [SerializeField] private Transform simTableTransform;

    // Default value is 10 and all algorith is calculated according to this assumption
    private int simScaleFactor = 10;

    private Pixel[,] _pixelArr;

    private int _arrX;
    private int _arrY;

    public static TableController Instance;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        _arrX = (int)(simTableTransform.localScale.x * simScaleFactor);
        _arrY = (int)(simTableTransform.localScale.y * simScaleFactor);

        _pixelArr = new Pixel[_arrY, _arrX];
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector2Int tablePos = GetPositionOnTable();

            if (tablePos.y >= _arrY || tablePos.y < 0 || tablePos.x >= _arrX || tablePos.x < 0)
            {
                return;
            }

            if (_pixelArr[tablePos.y, tablePos.x] == null)
            {
                GameObject go = Instantiate(pixelPref, GetFixedMousePosition(), Quaternion.identity, pixelParent);

                if (go.TryGetComponent(out Pixel pixel))
                {
                    pixel.SetPixelData(tablePos.x, tablePos.y, go);
                    _pixelArr[tablePos.y, tablePos.x] = pixel;
                }
                else
                {
                    Destroy(go);
                }
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector2Int tablePos = GetPositionOnTable();

            if (tablePos.y >= _arrY || tablePos.y < 0 || tablePos.x >= _arrX || tablePos.x < 0)
            {
                return;
            }

            if (_pixelArr[tablePos.y, tablePos.x] != null)
            {
                GameObject selectedGo = _pixelArr[tablePos.y, tablePos.x].gameObject;
                _pixelArr[tablePos.y, tablePos.x] = null;

                Destroy(selectedGo);
            }
        }
    }

    private void FixedUpdate()
    {
        for (int j = 0; j < _arrY; j++)
        {
            for (int i = 0; i < _arrX; i++)
            {
                if (_pixelArr[j, i] != null)
                {
                    if (j == 0)
                    {
                        continue;
                    }

                    _pixelArr[j, i].ApplyPhysics(_pixelArr);
                }
            }
        }
    }

    private Vector2Int GetPositionOnTable()
    {
        Vector3 mouseWorldPos = GetMousePosition();

        // We used a 0.1x0.1px sprite as a pixel
        int posX = (int)(mouseWorldPos.x * simScaleFactor);
        int posY = (int)(mouseWorldPos.y * simScaleFactor);

        int i = GetCoordinateValue(posX, _arrX);
        int j = GetCoordinateValue(posY, _arrY);

        return new Vector2Int(i, j);
    }

    int GetCoordinateValue(int positionalValue, int arrLength)
    {
        int coordValue;

        // I assume that both _arrX and _arrY are odd, else an additional check for oddness and evenness must be added.
        if (positionalValue < 0)
        {
            coordValue = arrLength - Mathf.Abs(positionalValue) - ((arrLength + 1) / 2);
        }
        else
        {
            coordValue = ((arrLength - 1) / 2) + positionalValue;
        }

        return coordValue;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        return mouseWorldPos;
    }

    private Vector3 GetFixedMousePosition()
    {
        // Turns 1.1125 to 1.1 by make some casting, multiplying and dividing
        Vector3 mouseWorldPos = GetMousePosition();
        mouseWorldPos.x = (float)((int)(mouseWorldPos.x * simScaleFactor)) / simScaleFactor;
        mouseWorldPos.y = (float)((int)(mouseWorldPos.y * simScaleFactor)) / simScaleFactor;

        return mouseWorldPos;
    }

    public void MovePixel(int currentX, int currentY, int targetX, int targetY)
    {
        Vector3 oldPos = _pixelArr[currentY, currentX].transform.position;

        // Change it
        float newPosX = (oldPos.x * simScaleFactor - (currentX - targetX)) / simScaleFactor;
        float newPosY = (oldPos.y * simScaleFactor - (currentY - targetY)) / simScaleFactor;

        _pixelArr[currentY, currentX].transform.position = new Vector3(newPosX, newPosY, oldPos.z);
        _pixelArr[targetY, targetX] = _pixelArr[currentY, currentX];
        _pixelArr[currentY, currentX] = null;

        _pixelArr[targetY, targetX].SetTablePosition(targetX, targetY);
    }

    public void ChangePixels(int currentX, int currentY, int targetX, int targetY)
    {
        Pixel currentPixel = _pixelArr[currentY, currentX];
        Pixel targetPixel = _pixelArr[targetY, targetX];

        Vector3 targetPos = targetPixel.transform.position;
        Vector3 currenPos = currentPixel.transform.position;

        // Set current pixel's positions
        currentPixel.SetPosition(targetPos);
        currentPixel.SetTablePosition(targetX, targetY);

        // Set target pixel's positions
        targetPixel.SetPosition(currenPos);
        targetPixel.SetTablePosition(currentX, currentY);

        // Update pixels positions
        _pixelArr[currentY, currentX] = targetPixel;
        _pixelArr[targetY, targetX] = currentPixel;
    }
}