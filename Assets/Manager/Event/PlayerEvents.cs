using System;

public class PlayerEvents
{
    public event Action onDisablePlayerMovement;
    public void DisablePlayerMovement()
    {
        if (onDisablePlayerMovement != null)
        {
            onDisablePlayerMovement();
        }
    }

    public event Action onEnablePlayerMovement;
    public void EnablePlayerMovement()
    {
        if (onEnablePlayerMovement != null)
        {
            onEnablePlayerMovement();
        }
    }


    public event Action<int> onPlayerProgress;
    public void PlayerProgress(int Pro)
    {
        if (onPlayerProgress != null)
        {
            onPlayerProgress(Pro);
        }
    }

}