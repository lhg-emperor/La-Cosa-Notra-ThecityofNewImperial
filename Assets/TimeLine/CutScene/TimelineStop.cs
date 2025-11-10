using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    [Header("Cấu hình Timeline")]
    [SerializeField] private PlayableDirector timelineDirector;

    [Header("Bật/tắt Timeline")]
    public bool useTimeline = true;

    private void Start()
    {
        if (timelineDirector == null)
        {
            timelineDirector = GetComponent<PlayableDirector>();
        }

        // Nếu tắt Timeline, đảm bảo nó không chạy
        if (!useTimeline)
        {
            if (timelineDirector != null)
            {
                timelineDirector.Stop();
                timelineDirector.enabled = false;
            }
        }
    }

    private void Update()
    {
        // Nếu Timeline bị tắt, chặn phát lại
        if (!useTimeline && timelineDirector != null && timelineDirector.state == PlayState.Playing)
        {
            timelineDirector.Stop();
        }
    }

    public void PlayTimeline()
    {
        if (useTimeline && timelineDirector != null)
        {
            timelineDirector.enabled = true;
            timelineDirector.Play();
        }
    }

    public void StopTimeline()
    {
        if (timelineDirector != null)
        {
            timelineDirector.Stop();
        }
    }
}
