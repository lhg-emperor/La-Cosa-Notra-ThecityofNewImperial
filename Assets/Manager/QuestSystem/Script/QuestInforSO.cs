using UnityEngine;

[CreateAssetMenu(fileName = "QuestInforSO", menuName = "ScriptableObjects/QuestInforSO", order = 1)]
public class QuestInforSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string title;

    [Header("Requirement")]
    public QuestInforSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Reward")]
    public int MoneyReward;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
