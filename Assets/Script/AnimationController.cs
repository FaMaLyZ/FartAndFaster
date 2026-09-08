using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance { get; private set; }

    [SerializeField] private Animator animator;

    
    [SerializeField]private static readonly int OpenTriggerHash = Animator.StringToHash("Open");
    [SerializeField]private static readonly int CloseTriggerHash = Animator.StringToHash("Close");
    [SerializeField]private static readonly int WinTriggerHash = Animator.StringToHash("Win");

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

    public void PlayOpenAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(OpenTriggerHash);
        }
    }
    public void PlayCloseAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(CloseTriggerHash);
        }
    }
    public void PlayWinAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(WinTriggerHash);
        }
    }
}
