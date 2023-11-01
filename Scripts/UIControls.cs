using UnityEngine;
using UnityEngine.UI;

public class UIControls : MonoBehaviour
{
    public GameObject _meshRenderer;
    private MeshInteractor _meshInteractor;
    public Text _brushRadius;
    public Text _currentBrush;
    private string _currentBrushText;

    private void Start()
    {
        _meshInteractor = _meshRenderer.GetComponent<MeshInteractor>();
    }
    // Update is called once per frame
    void Update()
    {
        _brushRadius.text = _meshInteractor.brushRadius.ToString();

        switch (_meshInteractor.selectedBrush)
        {
            case 1:
                _currentBrushText = "Free Form Displacement Brush";
                break;
            case 2:
                _currentBrushText = "Y-Axis Displacement Brush";
                break;
            case 3:
                _currentBrushText = "Pinch Brush";
                break;
            case 4:
                _currentBrushText = "Push Brush";
                break;
            case 5:
                _currentBrushText = "Flatten Brush";
                break;
            default:
                break;
        }

        _currentBrush.text = _currentBrushText;
    }
}
