using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInforSO questIn4Point;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;


    private bool playerIsNear = false;

    private string questId;

    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questIn4Point.id ;
        questIcon = GetComponentInChildren<QuestIcon>();
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

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!playerIsNear)
        {
            return;
        } 
        //Start or Finish Quest
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            GameEventsManager.instance.QuestEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameEventsManager.instance.QuestEvents.FinishQuest(questId);
        }
    }
    private void QuestStateChange(Quest quest)
    {
        if(quest.in4.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("Quest with Id: " + questId + "Update to state: " + currentQuestState);
            Debug.Log($"[{gameObject.name}] nhận update từ quest: {quest.in4.id} | local questId = {questId}");

            questIcon.SetState(currentQuestState, startPoint, finishPoint);
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
