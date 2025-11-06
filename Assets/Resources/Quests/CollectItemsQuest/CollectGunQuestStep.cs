    using UnityEngine;

    public class CollectGunQuestStep : QuestStep
    {
    private int gunsCollected = 0;
    public int gunsComplete = 1;

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onItemsCollected += GunCollected;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onItemsCollected -= GunCollected;
    }

    private void GunCollected()
    {
        if(gunsCollected < gunsComplete)
        {
            gunsCollected++;
        }

        if(gunsCollected >= gunsComplete)
        {
            FinishQuestStep();
        }
    }
    
    }
