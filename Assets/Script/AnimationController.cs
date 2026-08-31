using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance { get; private set; }

    [SerializeField] private Animator animator;

    
    [SerializeField]private static readonly int WinTriggerHash = Animator.StringToHash("Win");
    [SerializeField]private static readonly int LoseTriggerHash = Animator.StringToHash("Lose");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void PlayWinAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(WinTriggerHash);
        }
    }
    public void PlayLoseAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(LoseTriggerHash);
        }
    }
}
