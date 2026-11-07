using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;


public class ImageTrackingManager : MonoBehaviour
{
    [Header("AR References")]
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private RaycastManager RaycastManagerModelInfo;

    [System.Serializable]
    public struct ImagePrefabMapping
    {
        public string imageName;
        public GameObject modelPrefab;
        public ModelData modelData;
    }

    [Header("Configuración de Modelos y Datos")]
    [SerializeField] private List<ImagePrefabMapping> imagePrefabMappings;

    private Dictionary<TrackableId, GameObject> activeModels = new Dictionary<TrackableId, GameObject>();

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
            RemoveModel(removedPair.Key);
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
                if (model.activeSelf)
                {
                    model.SetActive(false);
                    if (RaycastManagerModelInfo != null)
                    {
                        RaycastManagerModelInfo.CheckIfSelectedModelLost(model);
                    }
                }
            }
        }
    }

    private void RemoveModel(TrackableId trackableId)
    {
        if (activeModels.TryGetValue(trackableId, out GameObject modelInstance))
        {
            if (RaycastManagerModelInfo != null)
            {
                RaycastManagerModelInfo.CheckIfSelectedModelLost(modelInstance);
            }
            Destroy(modelInstance);
            activeModels.Remove(trackableId);
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
}
