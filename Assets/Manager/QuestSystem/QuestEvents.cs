using System;

public static class QuestEvents
{
    // Requests coming from UI/scene to the QuestManager
    public static event Action<string> OnStartQuestRequested;
    public static event Action<string> OnAdvanceQuestRequested;
    public static event Action<string> OnFinishQuestRequested;

    // Notifications raised by QuestManager for listeners (UI, icons, points)
    public static event Action<string, QuestState> OnQuestStateChanged;

    public static void RequestStart(string questId) => OnStartQuestRequested?.Invoke(questId);
    public static void RequestAdvance(string questId) => OnAdvanceQuestRequested?.Invoke(questId);
    public static void RequestFinish(string questId) => OnFinishQuestRequested?.Invoke(questId);

    public static void QuestStateChange(string questId, QuestState newState) => OnQuestStateChanged?.Invoke(questId, newState);
}
