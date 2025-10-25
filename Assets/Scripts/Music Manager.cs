using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource audioSource;
    public List<AudioClip> normalClips;
    public List<AudioClip> combatClips;

    private List<AudioClip> currentList;
    private int currentIndex = 0;
    private bool isCombat = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        currentList = normalClips;
        PlayNext();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
            PlayNext();
    }

    public void SetCombat(bool state)
    {
        if (isCombat == state) return;
        isCombat = state;
        currentList = isCombat ? combatClips : normalClips;
        StartCoroutine(CrossFade());
    }

    void PlayNext()
    {
        if (currentList.Count == 0) return;
        currentIndex = Random.Range(0, currentList.Count);
        audioSource.clip = currentList[currentIndex];
        audioSource.Play();
    }

    IEnumerator CrossFade()
    {
        float duration = 1.5f;
        float startVolume = audioSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        PlayNext();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }
    }
}
