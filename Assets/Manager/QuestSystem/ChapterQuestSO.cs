using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewChapterQuest", menuName = "Quest/ChapterQuestSO")]
public class ChapterQuestSO : ScriptableObject
{
    [Header("Scene")]
    public SceneAsset scene; // chỉ dùng để tham chiếu Scene trong Editor

    [Header("Tên Chapter")]
    public string chapterName;

    [Header("Chapter Quest Prefab")]
    public GameObject chapterQuestPrefab; // Prefab chứa tất cả QuestPoint
}
