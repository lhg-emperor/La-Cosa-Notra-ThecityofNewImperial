using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PresentsController : MonoBehaviour
{
    [Header("Timeline & Scene")]
    public PlayableDirector timeline;          // Timeline gán từ Inspector
    public string nextSceneName = "MainMenu";  // Tên scene sẽ chuyển đến

    private bool hasTransitioned = false;

    void Start()
    {
        if (timeline == null)
        {
            Debug.LogError("Timeline chưa được gán vào PresentsController!");
            return;
        }

        // Gán callback khi Timeline kết thúc
        timeline.stopped += OnTimelineFinished;
    }

    void Update()
    {
        if (!hasTransitioned && Input.anyKeyDown)
        {
            TransitionToNextScene();
        }
    }

    void OnTimelineFinished(PlayableDirector director)
    {
        if (!hasTransitioned)
        {
            TransitionToNextScene();
        }
    }

    void TransitionToNextScene()
    {
        hasTransitioned = true;
        Debug.Log("Chuyển sang scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}
