using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

[RequireComponent(typeof(ARPlane))]
[RequireComponent(typeof(MeshRenderer))]

public class CustomARPlane : MonoBehaviour
{
    private ARPlane arPlane;
    private MeshRenderer meshRenderer;

    [Header("UI del Plano")]
    public TMP_Text dimensionText; 

    private Color colorHorizontal = new Color(1f, 0.92f, 0.016f, 0.4f); // Amarillo 
    private Color colorVertical = new Color(0.12f, 0.56f, 1f, 0.4f);    // Azul 

    void Awake()
    {
        arPlane = GetComponent<ARPlane>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnEnable()
    {
        
        arPlane.boundaryChanged += UpdatePlaneInfo;
        UpdatePlaneInfo(default);
    }

    void OnDisable()
    {
        arPlane.boundaryChanged -= UpdatePlaneInfo;
    }

    void UpdatePlaneInfo(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        
        if (arPlane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
        {
            meshRenderer.material.color = colorVertical;
        }
        else
        {
            meshRenderer.material.color = colorHorizontal;
        }

        // (ancho x largo)
        if (dimensionText != null)
        {
            dimensionText.text = $"{arPlane.size.x:F2}m x {arPlane.size.y:F2}m";

            // Corrección de texto
            dimensionText.transform.rotation = Quaternion.LookRotation(dimensionText.transform.position - Camera.main.transform.position);
        }
    }
}
