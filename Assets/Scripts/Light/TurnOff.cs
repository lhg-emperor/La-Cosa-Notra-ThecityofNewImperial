using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// TurnOff: Listens to NextScene signal and fades Light2D to dark.
/// NextScene calls TurnOffLight() → Light2D fades from bright to dark.
/// When fade completes, notifies TurnOn to turn light back on.
/// </summary>
public class TurnOff : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Duration to fade from bright to dark")]
    public float fadeDuration = 2f;

    private Light2D targetLight;
    private Coroutine fadeCoroutine;
    private TurnOn turnOnComponent;

    private void Start()
    {
        // Find Light2D on this GameObject or scene
        targetLight = GetComponent<Light2D>();
        if (targetLight == null)
        {
            targetLight = FindFirstObjectByType<Light2D>();
            if (targetLight == null)
            {
                Debug.LogError("TurnOff: No Light2D found!");
                return;
            }
        }

        // Find TurnOn component
        turnOnComponent = FindFirstObjectByType<TurnOn>();
        if (turnOnComponent == null)
        {
            Debug.LogWarning("TurnOff: No TurnOn component found in scene");
        }

        Debug.Log($"TurnOff: Ready to control Light2D on {targetLight.gameObject.name}");
    }

    /// <summary>
    /// Call this when NextScene is triggered (from NextScene.cs)
    /// </summary>
    public void TurnOffLight()
    {
        if (targetLight == null)
        {
            Debug.LogWarning("TurnOff: No Light2D to control");
            return;
        }

        Debug.Log("TurnOff: Fading light to dark...");

        // Stop current fade if any
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        // Fade from current color to black
        fadeCoroutine = StartCoroutine(FadeLightToBlack());
    }

    private IEnumerator FadeLightToBlack()
    {
        Color startColor = targetLight.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            targetLight.color = Color.Lerp(startColor, Color.black, t);
            yield return null;
        }

        targetLight.color = Color.black;
        Debug.Log($"TurnOff: Light is now dark (took {fadeDuration} seconds)");

        // Notify TurnOn to turn light back on with same duration
        if (turnOnComponent != null)
        {
            Debug.Log("TurnOff: Notifying TurnOn to turn light back on...");
            turnOnComponent.TurnOnLight(fadeDuration);
        }
    }
}

