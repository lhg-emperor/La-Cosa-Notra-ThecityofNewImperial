using System;

public class MiscEvents
{
    public event Action onItemsCollected;
    public void ItemPickup()
    {
        if (onItemsCollected != null)
        {
            onItemsCollected();
        }
    }

    
}