using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour
{
    [Header("Danh sách nhạc cho Scene (ít nhất 1)")]
    public AudioClip[] sceneMusicClips;

    private AudioSource audioSource;
    private int currentClipIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false; // Quản lý vòng lặp bằng script
    }

    private void OnEnable()
    {
        if (sceneMusicClips != null && sceneMusicClips.Length > 0)
            StartCoroutine(PlayMusicLoop());
    }

    private IEnumerator PlayMusicLoop()
    {
        if (sceneMusicClips.Length == 1)
        {
            // Nếu chỉ có 1 bài: dừng 2s rồi phát lại
            AudioClip clip = sceneMusicClips[0];
            while (true)
            {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(clip.length);
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            // Nếu có nhiều bài: phát nối tiếp từng bài, cách nhau 3s
            while (true)
            {
                AudioClip clip = sceneMusicClips[currentClipIndex];
                audioSource.clip = clip;
                audioSource.Play();

                yield return new WaitForSeconds(clip.length);
                yield return new WaitForSeconds(3f);

                currentClipIndex++;
                if (currentClipIndex >= sceneMusicClips.Length)
                    currentClipIndex = 0;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }
}
