using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using TMPro;

public class RayCastScript : MonoBehaviour
{
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject objectToPlace;
    [SerializeField] TMP_Text debugText;
    
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

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
            debugText.text = "Esperando touch...";
                return;
        }

        var touch = activeTouches[0];

        Debug.Log("Touch Detectado");
        debugText.text = "TOUCH DETECTADO";

        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        if (raycastManager.Raycast(activeTouches[0].screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("HIT ENCONTRADO");

            Pose hitPose = hits[0].pose;
            debugText.text = "HIT: " + hitPose.position;
            Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
        }
        else
        {
            Debug.Log("NO SE ENCONTRO PLANO");
            debugText.text = "NO HIT";
        }
    }
}
