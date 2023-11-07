using System;
using UnityEngine;
using UnityEngine.UI;

public class UIControls : MonoBehaviour
{
    public GameObject _meshRenderer;
    private MeshInteractor _meshInteractor;

    public Text _brushRadius;
    public Text _currentBrush;
    //public Text _fps;
    public Text _date;

    public Image _ffd;
    public Image _upDown;
    public Image _radPinch;
    public Image _radPush;
    public Image _flatten;

    private string _currentBrushText;

    private void Start()
    {
        _meshInteractor = _meshRenderer.GetComponent<MeshInteractor>();
        _date.text = DateTime.Now.ToString()[0..10];
        DeactivateAllImages();
    }

    private void DeactivateAllImages()
    {
        _ffd.enabled = false;
        _upDown.enabled = false;
        _radPinch.enabled = false;
        _radPush.enabled = false;
        _flatten.enabled = false;
    }

    void Update()
    {
        _brushRadius.text = (_meshInteractor.brushRadius / _meshInteractor.brushRadiusMax * 100).ToString("#") + "%";    // On a scale of 0 - 100 with no visible decimal points
        DeactivateAllImages();

        switch (_meshInteractor.selectedBrush)
        {
            case 1:
                _currentBrushText = "Free Form Displacement Brush";
                _ffd.enabled = true;
                break;
            case 2:
                _currentBrushText = "Y-Axis Displacement Brush";
                _upDown.enabled = true;
                break;
            case 3:
                _currentBrushText = "Pinch Brush";
                _radPinch.enabled = true;
                break;
            case 4:
                _currentBrushText = "Push Brush";
                _radPush.enabled = true;
                break;
            case 5:
                _currentBrushText = "Flatten Brush";
                _flatten.enabled = true;
                break;
            default:
                break;
        }

        _currentBrush.text = _currentBrushText;
        //_fps.text = (1 / Time.deltaTime).ToString("#");        // Frames per second = 1000 / Time between subsequent frames

    }
}
