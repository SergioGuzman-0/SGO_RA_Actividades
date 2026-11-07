using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RaycastManager : MonoBehaviour
{
    [Header("Referencias de UI del Panel")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image iconImage;

    [Header("Estado Original / Default")]
    [SerializeField] private string defaultStateText = "Selecciona un modelo...";

    private GameObject currentSelectedModel;

    private void Start()
    {
        Debug.Log("¡El RaycastManager inició correctamente!");
        ResetUI();
    }

    private void Update()
    {
        
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 inputPosition = Pointer.current.position.ReadValue();
            Debug.Log("¡Toque/Clic detectado en las coordenadas: " + inputPosition + "!");

            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("El rayo chocó contra el objeto: " + hit.collider.gameObject.name);

                ModelReference modelRef = hit.collider.GetComponentInParent<ModelReference>();

                if (modelRef != null && modelRef.modelData != null)
                {
                    Debug.Log("¡ModelReference encontrado! Mostrando datos...");
                    currentSelectedModel = modelRef.gameObject;
                    DisplayModelData(modelRef.modelData);
                }
                else //Pruebas para comprobar que el raycast funciona
                {
                    Debug.Log("El objeto tiene colisión, pero le falta el componente ModelReference o su Data.");
                }
            }
            else
            {
                Debug.Log("El rayo se disparó, pero no hizo contacto con ningún collider.");
            }
        }
    }

    private void DisplayModelData(ModelData data)
    {
        if (nameText) nameText.text = "<b>Nombre:</b> " + data.modelName;
        if (descText) descText.text = "<b>Descripción:</b> " + data.description;
        if (categoryText) categoryText.text = "<b>Categoría:</b> " + data.category;
        if (levelText) levelText.text = "<b>Nivel:</b> " + data.level;

        if (iconImage)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = data.modelIcon;
        }
    }

    public void ResetUI()
    {
        if (nameText) nameText.text = defaultStateText;
        if (descText) descText.text = "";
        if (categoryText) descText.text = "";
        if (levelText) levelText.text = "";

        if (iconImage)
        {
            iconImage.gameObject.SetActive(false);
        }

        currentSelectedModel = null;
    }

    public void CheckIfSelectedModelLost(GameObject destroyedModel)
    {
        if (currentSelectedModel == destroyedModel)
        {
            ResetUI();
        }
    }
}
