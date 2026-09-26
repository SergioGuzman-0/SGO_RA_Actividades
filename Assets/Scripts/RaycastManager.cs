using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using TMPro;

public class RaycastManager : MonoBehaviour
{
    [Header("Componentes AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Prefabs según Orientación")]
    [SerializeField] private GameObject horizontalPrefab;
    [SerializeField] private GameObject verticalPrefab;

    [Header("UI / Debug")]
    [SerializeField] private TMP_Text debugText;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (activeTouches.Count == 0)
        {
            if (debugText != null) debugText.text = "Buscando planos...";
            return;
        }

        var touch = activeTouches[0];

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        
        if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            
            ARPlane hitPlane = planeManager.GetPlane(hits[0].trackableId);

            if (hitPlane != null)
            {
                
                if (hitPlane.alignment == PlaneAlignment.Vertical)
                {
                    Instantiate(verticalPrefab, hitPose.position, hitPose.rotation);
                    if (debugText != null) debugText.text = "Objeto colocado en Plano Vertical";
                }
                else
                {
                    Instantiate(horizontalPrefab, hitPose.position, hitPose.rotation);
                    if (debugText != null) debugText.text = "Objeto colocado en Plano Horizontal";
                }
            }
        }
        else
        {
            if (debugText != null) debugText.text = "Toca sobre un plano detectado";
        }
    }
}
