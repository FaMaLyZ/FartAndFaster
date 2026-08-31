using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateOverlay : MonoBehaviour
{
    private Animator animator;

    [Header("Random Blink")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;

    [Header("Blush State")]
    public bool isRed = false;

    [Header("Lost State")]
    public bool isLost = false;

    [Header("UI & Transition")]
    public Image fadeBlackImage;
    public float fadeDuration = 1.0f;
    public GameObject lostPanel;

    private bool hasTriggeredLostSequence = false;
    private Coroutine blinkCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        blinkCoroutine = StartCoroutine(BlinkRoutine());

        if (fadeBlackImage != null)
        {
            Color c = fadeBlackImage.color;
            c.a = 0f;
            fadeBlackImage.color = c;
            fadeBlackImage.gameObject.SetActive(true);
        }

        if (lostPanel != null)
        {
            lostPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (animator != null)
        {
            animator.SetBool("isRed", isRed);
            animator.SetBool("isLost", isLost);
        }

        if (isRed && isLost && !hasTriggeredLostSequence)
        {
            hasTriggeredLostSequence = true;

            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
            }

            if (animator != null)
            {
                animator.ResetTrigger("Blink");
            }

            StartCoroutine(LostSequenceRoutine());
        }
    }

    public void SetRed(bool active)
    {
        isRed = active;
    }

    public void TriggerLose()
    {
        isLost = true;
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (!isRed && !isLost && animator != null)
            {
                animator.SetTrigger("Blink");
            }
        }
    }

    IEnumerator LostSequenceRoutine()
    {
        yield return null;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }

        if (fadeBlackImage != null)
        {
            float elapsed = 0f;
            Color startColor = fadeBlackImage.color;
            Color targetColor = new Color(0f, 0f, 0f, 0.9f);

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeBlackImage.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
                yield return null;
            }

            fadeBlackImage.color = targetColor;
        }

        if (lostPanel != null)
        {
            lostPanel.SetActive(true);
        }

    }
}