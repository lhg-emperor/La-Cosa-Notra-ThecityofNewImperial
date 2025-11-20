using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveGameButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button saveButton;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDuration = 2f;

    private Coroutine feedbackCoroutine;

    void Start()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveGameClicked);
        else
            Debug.LogWarning("Save Button chưa được gán!");

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    private void OnSaveGameClicked()
    {
        if (DataPersistenceManager.instance == null)
        {
            Debug.LogWarning("Không tìm thấy DataPersistenceManager!");
            return;
        }

        DataPersistenceManager.instance.SaveGame();

        if (feedbackText != null)
        {
            feedbackText.text = "Save Game Complete";
            feedbackText.gameObject.SetActive(true);

            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);

            feedbackCoroutine = StartCoroutine(HideFeedbackAfterTime(feedbackDuration));
        }
    }

    private IEnumerator HideFeedbackAfterTime(float delay)
    {
        // Dùng thời gian thực thay vì Time.timeScale
        yield return new WaitForSecondsRealtime(delay);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        feedbackCoroutine = null;
    }
}
