using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// TurnOn: Receives signal from TurnOff after light fades to dark.
/// Fades light back on (000000 → FFFFFF) with the same duration as TurnOff took.
/// </summary>
public class TurnOn : MonoBehaviour
{
    private Light2D targetLight;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Find Light2D on this GameObject or scene
        targetLight = GetComponent<Light2D>();
        if (targetLight == null)
        {
            targetLight = FindFirstObjectByType<Light2D>();
            if (targetLight == null)
            {
                Debug.LogError("TurnOn: No Light2D found!");
                return;
            }
        }

        Debug.Log($"TurnOn: Ready to control Light2D on {targetLight.gameObject.name}");
    }

    /// <summary>
    /// Called by TurnOff when light fade to dark completes.
    /// Duration parameter matches the TurnOff fade duration.
    /// </summary>
    public void TurnOnLight(float fadeInDuration)
    {
        if (targetLight == null)
        {
            Debug.LogWarning("TurnOn: No Light2D to control");
            return;
        }

        Debug.Log($"TurnOn: Fading light back on (duration: {fadeInDuration}s)...");

        // Stop current fade if any
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        // Fade from black to white
        fadeCoroutine = StartCoroutine(FadeLightToWhite(fadeInDuration));
    }

    private IEnumerator FadeLightToWhite(float duration)
    {
        Color startColor = targetLight.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            targetLight.color = Color.Lerp(startColor, Color.white, t);
            yield return null;
        }

        targetLight.color = Color.white;
        Debug.Log($"TurnOn: Light is now bright (took {duration} seconds)");
    }
}
