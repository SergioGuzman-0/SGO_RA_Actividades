using UnityEngine;

[CreateAssetMenu(fileName = "NewModelData", menuName = "AR/Model Data")]
public class ModelData : ScriptableObject
{
    public string modelName;
    [TextArea] public string description;
    public string category;
    public string level;
    public Sprite modelIcon;
}
