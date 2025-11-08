using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class QuestPoint : MonoBehaviour
{
    [SerializeField] private QuestInfoSO questInfo;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
            col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (questInfo == null) return;
        if (!other.CompareTag("Player")) return;

        Quest quest = QuestManager.Instance.GetQuest(questInfo.id);
        if (quest == null)
        {
            quest = new Quest(questInfo);
            QuestManager.Instance.AddQuest(quest);
        }

        if (quest.currentState == QuestState.Locked)
        {
            QuestManager.Instance.StartQuest(questInfo.id);

            if (questInfo.questStepPrefabs != null && questInfo.questStepPrefabs.Length > 0)
            {
                GameObject stepObj = Instantiate(questInfo.questStepPrefabs[0], QuestManager.Instance.transform);
                stepObj.transform.position = transform.position;
                stepObj.transform.rotation = Quaternion.identity;
                stepObj.transform.SetParent(QuestManager.Instance.transform);

                QuestStep questStep = stepObj.GetComponent<QuestStep>();
                if (questStep != null)
                {
                    questStep.InitializeStep(questInfo.id, 0);
                }
                else
                {
                    Debug.LogWarning("Prefab được gán không chứa component QuestStep!");
                }
            }
            else
            {
                Debug.LogWarning("Quest này không có QuestStep Prefab nào được gán!");
            }
        }
    }
}
