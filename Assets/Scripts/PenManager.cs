using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PenManager : MonoBehaviour
{
    [Header("Pixel Pen")]
    [SerializeField] private GameObject pixelPref;
    [SerializeField] private int penSize = 1;

    [Header("Slider")]
    public Slider PenSizeSlider;

    public static PenManager Instance;

    // Pen Size
    public delegate void PenSizeChangeHandler(int amount);
    public event PenSizeChangeHandler OnPenSizeChanged;

    // Pen
    public delegate void PenChangeHandler(GameObject penPref);
    public event PenChangeHandler OnPenChanged;

    // Singleton Check
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PenSizeSlider.value = penSize;
        ChangePenSize();
    }

    public void ChangePenSize()
    {
        penSize = (int)PenSizeSlider.value;
        OnPenSizeChanged?.Invoke(penSize);
    }

    public void ChangePen(Pen pen)
    {
        pixelPref = pen.PenPixelPrefab;
        OnPenChanged?.Invoke(pixelPref);
    }
}
