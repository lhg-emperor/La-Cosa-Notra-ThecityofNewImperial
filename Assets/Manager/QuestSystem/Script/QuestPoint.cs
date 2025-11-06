using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInforSO questIn4Point;

    private bool playerIsNear = false;

    private string questId;

    private QuestState currentQuestState;

    private void Awake()
    {
        questId = questIn4Point.id ;
    }

    private void OnEnable()
    {
        GameEventsManager.instance.QuestEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.QuestEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (!playerIsNear)
        {
            return;
        } 
        GameEventsManager.instance.QuestEvents.StartQuest(questId);
        GameEventsManager.instance.QuestEvents.AdvanceQuest(questId);
        GameEventsManager.instance.QuestEvents.FinishQuest(questId);
    }
    private void QuestStateChange(Quest quest)
    {
        if(quest.in4.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("Quest with Id: " + questId + "Update to state: " + currentQuestState);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
