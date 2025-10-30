using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private List<AudioSource> currentSceneSources = new List<AudioSource>();
    private string currentSceneName;

    private void Awake()
    {
        // Đảm bảo chỉ có một MusicManager tồn tại
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Lắng nghe sự kiện đổi Scene
            SceneManager.activeSceneChanged += OnSceneChanged;

            // Phát nhạc của scene khởi đầu
            PlaySceneMusic(SceneManager.GetActiveScene());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        PlaySceneMusic(newScene);
    }

    private void PlaySceneMusic(Scene scene)
    {
        if (currentSceneName == scene.name) return;

        // Dừng và xóa nhạc cũ
        foreach (AudioSource src in currentSceneSources)
        {
            if (src != null) src.Stop();
        }
        currentSceneSources.Clear();

        // Tìm tất cả các GameObject có Script "Music" trong Scene
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            Music[] musicScripts = obj.GetComponentsInChildren<Music>(true);
            foreach (Music music in musicScripts)
            {
                AudioSource source = music.GetComponent<AudioSource>();
                if (source != null)
                {
                    source.loop = true;
                    source.Play();
                    currentSceneSources.Add(source);
                }
            }
        }

        currentSceneName = scene.name;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
