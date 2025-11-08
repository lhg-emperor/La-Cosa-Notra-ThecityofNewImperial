using System.Collections.Generic;
using UnityEngine;

public class ChapterQuest : MonoBehaviour
{
    [Header("Danh sách QuestPoint")]
    public List<GameObject> questPoints = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < questPoints.Count; i++)
        {
            if (questPoints[i] != null)
                questPoints[i].SetActive(false);
        }


        if (questPoints.Count > 0 && questPoints[0] != null)
            questPoints[0].SetActive(true);

        // Gửi danh sách QuestPoints cho QuestManager
        if (QuestManager.Instance != null)
            QuestManager.Instance.RegisterChapterQuest(this);
    }
}
