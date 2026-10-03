using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using TMPro;

public class RaycastManager : MonoBehaviour
{
    [Header("Componentes AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Prefabs de Modelos")]
    [SerializeField] private GameObject prefabHorizontal;
    [SerializeField] private GameObject prefabVertical;

    [Header("UI del Juego")]
    [SerializeField] private TMP_Text estadoText;

    // Variables de control
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private int modeloSeleccionado = 0; 

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        planeManager.planesChanged += OnPlanesChanged; 
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        planeManager.planesChanged -= OnPlanesChanged;
    }

    private void Start()
    {
        estadoText.text = "Escaneando entorno ...";
    }

    // Botones de UI
    public void SeleccionarModeloHorizontal()
    {
        modeloSeleccionado = 0;
    }

    public void SeleccionarModeloVertical()
    {
        modeloSeleccionado = 1;
    }
    

    
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            ARPlane planoNuevo = args.added[0];
            if (planoNuevo.alignment == PlaneAlignment.Vertical)
                estadoText.text = "Superficie vertical detectada!";
            else
                estadoText.text = "Superficie horizontal detectada!";
        }
    }

    void Update()
    {
        var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (activeTouches.Count == 0) return;

        var touch = activeTouches[0];

        
        if (EventSystem.current.IsPointerOverGameObject(touch.touchId)) return;

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            ARPlane hitPlane = planeManager.GetPlane(hits[0].trackableId);

            if (hitPlane != null)
            {
                
                if (hitPlane.alignment == PlaneAlignment.Vertical && modeloSeleccionado == 1)
                {
                    Instantiate(prefabVertical, hitPose.position, hitPose.rotation);
                    estadoText.text = "Objeto colocado en plano vertical";
                }
                
                else if (hitPlane.alignment != PlaneAlignment.Vertical && modeloSeleccionado == 0)
                {
                    Instantiate(prefabHorizontal, hitPose.position, hitPose.rotation);
                    estadoText.text = "Objeto colocado en plano horizontal";
                }
            }
        }
    }
}
