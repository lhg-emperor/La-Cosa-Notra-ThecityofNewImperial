using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, Quest> questMap = new Dictionary<string, Quest>();

    private ChapterQuest activeChapterQuest;
    private int currentQuestIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterChapterQuest(ChapterQuest chapterQuest)
    {
        activeChapterQuest = chapterQuest;
        currentQuestIndex = 0;

        Debug.Log("Danh sách QuestPoint nhận từ ChapterQuest:");
        for (int i = 0; i < activeChapterQuest.questPoints.Count; i++)
        {
            GameObject qp = activeChapterQuest.questPoints[i];
            Debug.Log($"  [{i}] {qp?.name ?? "null"} | Active: {qp?.activeSelf ?? false}");
        }
    }

    private void ShowQuestPoint(int index)
    {
        if (index < activeChapterQuest.questPoints.Count)
        {
            GameObject qp = activeChapterQuest.questPoints[index];
            qp.SetActive(true);
            Debug.Log("QuestPoint hiện lên: " + qp.name + " | Active: " + qp.activeSelf);
        }
    }

    public void CompleteCurrentQuestPoint()
    {
        if (activeChapterQuest == null || currentQuestIndex >= activeChapterQuest.questPoints.Count) return;

        GameObject completedQP = activeChapterQuest.questPoints[currentQuestIndex];
        if (completedQP != null)
        {
            Debug.Log("QuestPoint đã hoàn thành: " + completedQP.name);
            Destroy(completedQP);
        }

        currentQuestIndex++;

        if (currentQuestIndex < activeChapterQuest.questPoints.Count)
        {
            Debug.Log("Quest tiếp theo là: " + activeChapterQuest.questPoints[currentQuestIndex].name);
            ShowQuestPoint(currentQuestIndex);
            Debug.Log("QuestPoint tiếp theo đã xuất hiện? " +
                      activeChapterQuest.questPoints[currentQuestIndex].activeSelf);
        }
        else
        {
            Debug.Log("Tất cả QuestPoint trong Chapter đã hoàn thành!");
        }

        // Kiểm tra trạng thái Quest trước đó
        Debug.Log("Quest trước đã bị destroy? " + (completedQP == null));
    }

    // ==== Quản lý Quest (tương tác với QuestStep) ====

    public void AddQuest(Quest quest)
    {
        if (!questMap.ContainsKey(quest.questId))
        {
            questMap.Add(quest.questId, quest);
            Debug.Log("Đã thêm nhiệm vụ: " + quest.questName);
        }
    }

    public Quest GetQuest(string id)
    {
        questMap.TryGetValue(id, out Quest quest);
        return quest;
    }

    public void StartQuest(string id)
    {
        Quest quest = GetQuest(id);
        if (quest != null)
        {
            Debug.Log("Bắt đầu Quest: " + quest.questName);
            quest.StartQuest();
        }
    }

    public void CompleteQuest(string id)
    {
        Quest quest = GetQuest(id);
        if (quest != null)
        {
            Debug.Log("Quest đã hoàn thành: " + quest.questName);
            quest.CompleteQuest();

            // Thông báo QuestPoint hoàn thành
            CompleteCurrentQuestPoint();
        }
    }
}
