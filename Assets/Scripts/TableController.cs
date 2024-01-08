using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [Header("Pixel")]
    [SerializeField] private GameObject pixelPref;
    [SerializeField] private Transform pixelParent;
    [Range(1, 15)]
    [SerializeField] private int penSize = 1;

    [Header("Table")]
    [SerializeField] private Transform simTableTransform;

    // Default value is 10 and all algorith is calculated according to this assumption
    private int simScaleFactor = 10;

    private Pixel[,] _pixelArr;

    private int _arrX;
    private int _arrY;

    public static TableController Instance;

    private void OnDisable()
    {
        // Unsubscribe Event
        PenManager.Instance.OnPenChanged -= HandlePenChange;
        PenManager.Instance.OnPenSizeChanged -= HandlePenSizeChange;
    }

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

        // Subscribe Event
        PenManager.Instance.OnPenChanged += HandlePenChange;
        PenManager.Instance.OnPenSizeChanged += HandlePenSizeChange;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Check if origin point is not on table
            if (!IsPointOnTable(GetMousesTablePosition()))
            {
                return;
            }

            foreach (Vector2Int pos in GetPositionsOnTable())
            {
                PlacePixel(pos);
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            // Check if origin point is not on table
            if (!IsPointOnTable(GetMousesTablePosition()))
            {
                return;
            }

            foreach (Vector2Int pos in GetPositionsOnTable())
            {
                RemovePixel(pos);
            }
        }
    }

    private void FixedUpdate()
    {
        ApplyPixelsPhysics();
    }

    private void HandlePenChange(GameObject penPref)
    {
        pixelPref = penPref;
    }

    private void HandlePenSizeChange(int amount)
    {
        penSize = amount;
    }

    private List<Vector2Int> GetPositionsOnTable()
    {
        Vector2Int originPoint = GetMousesTablePosition();
        List<Vector2Int> positionList = new List<Vector2Int>();

        for (int i = 0; i < penSize; i++)
        {
            for (int j = 0; j < penSize; j++)
            {
                // if pixelSize is even then factor equals to pixelSize / 2, else ((pixelSize + 1) / 2) - 1
                int factor = penSize % 2 == 0 ? penSize / 2 : ((penSize + 1) / 2) - 1;

                positionList.Add(new Vector2Int(originPoint.x - factor + i, originPoint.y - factor + j));
            }
        }

        return positionList;
    }

    private void PlacePixel(Vector2Int selectedPos)
    {
        if (!IsPointOnTable(selectedPos))
        {
            return;
        }

        if (_pixelArr[selectedPos.y, selectedPos.x] == null)
        {
            GameObject go = Instantiate(pixelPref, 
                                        new Vector3(GetPosByCoordinateValue(selectedPos.x, _arrX), GetPosByCoordinateValue(selectedPos.y, _arrY), 0f), 
                                        Quaternion.identity, pixelParent);

            if (go.TryGetComponent(out Pixel pixel))
            {
                pixel.SetPixelData(selectedPos.x, selectedPos.y, go);
                _pixelArr[selectedPos.y, selectedPos.x] = pixel;
            }
            else
            {
                Destroy(go);
            }
        }
    }

    private bool IsPointOnTable(Vector2Int selectedPos)
    {
        if (selectedPos.y >= _arrY || selectedPos.y < 0 || selectedPos.x >= _arrX || selectedPos.x < 0)
        {
            return false;
        }

        return true;
    }

    private void RemovePixel(Vector2Int selectedPos)
    {
        if (selectedPos.y >= _arrY || selectedPos.y < 0 || selectedPos.x >= _arrX || selectedPos.x < 0)
        {
            return;
        }

        if (_pixelArr[selectedPos.y, selectedPos.x] != null)
        {
            GameObject selectedGo = _pixelArr[selectedPos.y, selectedPos.x].gameObject;
            _pixelArr[selectedPos.y, selectedPos.x] = null;

            Destroy(selectedGo);
        }
    }

    private Vector2Int GetMousesTablePosition()
    {
        Vector3 mouseWorldPos = GetMousePosition();

        // We used a 0.1x0.1px sprite as a pixel
        int posX = (int)(mouseWorldPos.x * simScaleFactor);
        int posY = (int)(mouseWorldPos.y * simScaleFactor);

        int i = GetCoordinateValueByPos(posX, _arrX);
        int j = GetCoordinateValueByPos(posY, _arrY);

        return new Vector2Int(i, j);
    }

    private int GetCoordinateValueByPos(int positionalValue, int arrLength)
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

    // Get word position according to index on the table
    private float GetPosByCoordinateValue(int coordValue, int arrLength)
    {
        return (float)(coordValue - ((arrLength - 1) / 2)) / simScaleFactor;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        return mouseWorldPos;
    }

    private void ApplyPixelsPhysics()
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

    #region Public Methods
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
    #endregion
}