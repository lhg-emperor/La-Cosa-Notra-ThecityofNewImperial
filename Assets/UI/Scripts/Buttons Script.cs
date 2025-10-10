using UnityEngine;
using System.Collections;

public class MenuAnimatorController : MonoBehaviour
{
    public Animator screenAnimator;

    private string currentHover = "";

    public void OnButtonHover(string name)
    {
        if (screenAnimator == null)
        {
            Debug.LogError("Animator của Panel Screen chưa được gán!");
            return;
        }

        // Chỉ hover mới thì mới chạy
        if (currentHover != name)
        {
            currentHover = name;
            screenAnimator.SetBool("Hover", true);
            screenAnimator.SetBool(name, true);

            // Reset sau 1 frame để animation chỉ chạy 1 lần
            StartCoroutine(ResetBoolAfterTime(name, 0.4f));
        }
    }

    public void OnButtonExit()
    {
        if (screenAnimator == null) return;

        screenAnimator.SetBool("Hover", false);
        currentHover = ""; // Cho phép hover lại
    }

    private IEnumerator ResetBoolAfterTime(string param, float delay = 0.3f)
    {
        yield return new WaitForSeconds(delay);
        screenAnimator.SetBool(param, false);
    }
}
