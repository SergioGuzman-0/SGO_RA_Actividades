using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ImageTrackingManager : MonoBehaviour
{
    [Header("AR References")]
    [SerializeField] private ARTrackedImageManager imageManager;

    [System.Serializable]
    public struct ImagePrefabMapping
    {
        public string imageName;      
        public GameObject modelPrefab; 
    }

    [Header("Configuración de Modelos")]
    [Tooltip("Relaciona el nombre de la imagen con su modelo 3D correspondiente")]
    [SerializeField] private List<ImagePrefabMapping> imagePrefabMappings;

    [Header("Interfaz de Usuario (UI)")]
    [SerializeField] private TextMeshProUGUI infoText; 

    
    private Dictionary<TrackableId, GameObject> activeModels = new Dictionary<TrackableId, GameObject>();
    private GameObject selectedModel;

    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnChanged);
    }

    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        
        foreach (var newImage in eventArgs.added)
        {
            UpdateImageModel(newImage);
        }

        
        foreach (var updatedImage in eventArgs.updated)
        {
            UpdateImageModel(updatedImage);
        }

        
        foreach (var removedPair in eventArgs.removed)
        {
            if (activeModels.TryGetValue(removedPair.Key, out GameObject modelInstance))
            {
                Destroy(modelInstance);
                activeModels.Remove(removedPair.Key);
            }
        }
    }

    private void UpdateImageModel(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        TrackableId trackableId = trackedImage.trackableId;

       
        if (!activeModels.ContainsKey(trackableId))
        {
            
            GameObject prefabToSpawn = GetPrefabForImage(imageName);
            if (prefabToSpawn != null)
            {
                
                GameObject spawnedModel = Instantiate(prefabToSpawn, trackedImage.transform);
                activeModels.Add(trackableId, spawnedModel);
            }
        }

        
        if (activeModels.TryGetValue(trackableId, out GameObject model))
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                
                model.SetActive(true);
            }
            else
            {
                
                model.SetActive(false);

                
                if (selectedModel == model)
                {
                    selectedModel = null;
                    if (infoText != null) infoText.text = "Selecciona un modelo...";
                }
            }
        }
    }

    private GameObject GetPrefabForImage(string imageName)
    {
        foreach (var mapping in imagePrefabMappings)
        {
            if (mapping.imageName == imageName)
            {
                return mapping.modelPrefab;
            }
        }
        return null;
    }

    private void Update()
    {
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            
            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                
                foreach (var kvp in activeModels)
                {
                    
                    if (clickedObject.transform.IsChildOf(kvp.Value.transform))
                    {
                        selectedModel = kvp.Value;

                        
                        string modelName = selectedModel.name.Replace("(Clone)", "").Trim();

                        
                        if (infoText != null)
                        {
                            infoText.text = "Modelo seleccionado: " + modelName;
                        }
                        break;
                    }
                }
            }
        }
    }
}
