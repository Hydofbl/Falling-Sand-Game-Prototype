using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField] private GameObject pixelPref;
    [SerializeField] private Transform pixelParent;
    [SerializeField] private Transform simTableTransform;
    [SerializeField] private int simScaleFactor = 10;

    private Pixel[,] _pixelArr;

    private int _arrX;
    private int _arrY;

    public static TableController Instance;

    private void Start()
    {
        if(Instance != null && Instance != this)
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
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // We used a 0.1x0.1 sprite as a pixel
            int posX = (int)(mouseWorldPos.x * simScaleFactor);
            int posY = (int)(mouseWorldPos.y * simScaleFactor);

            int i = GetCoordinateValue(posX, _arrX);
            int j = GetCoordinateValue(posY, _arrY);

            if (j >= _arrY || j < 0 || i >= _arrX || i < 0)
            {
                return;
            }

            if (_pixelArr[j, i] == null)
            {
                GameObject go = Instantiate(pixelPref, new Vector3((float)posX / simScaleFactor, (float)posY / simScaleFactor, 0), Quaternion.identity, pixelParent);

                if(go.TryGetComponent(out Pixel pixel))
                {
                    pixel.SetDatas(i, j, go);
                    _pixelArr[j, i] = pixel;
                }
                else
                {
                    Destroy(go);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        for(int j = 0; j < _arrY; j++)
        {
            for(int i = 0; i < _arrX; i++)
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

    public void MovePixel(int currentX, int currentY, int targetX, int targetY)
    {
        Vector3 oldPos = _pixelArr[currentY, currentX].transform.position;

        // Change it
        float newPosX = (oldPos.x * simScaleFactor - (currentX - targetX)) / simScaleFactor;
        float newPosY = (oldPos.y * simScaleFactor - (currentY - targetY)) / simScaleFactor;

        _pixelArr[currentY, currentX].transform.position = new Vector3(newPosX, newPosY, oldPos.z);
        _pixelArr[targetY, targetX] = _pixelArr[currentY, currentX];
        _pixelArr[currentY, currentX] = null;

        _pixelArr[targetY, targetX].SetPositions(targetX, targetY);
    }
}