using UnityEngine;

public class SpaceLevelAnim : MonoBehaviour
{
    public GameObject spaceLevelVisual;

    private Animator animator;

    void Start()
    {
        animator = spaceLevelVisual.GetComponent<Animator>();
        spaceLevelVisual.SetActive(false);
    }

    public void SetSpaceLevel(int level)
    {
        if (level >= 1 && level <= 4)
        {
            spaceLevelVisual.SetActive(true);
            animator.SetInteger("spaceLevel", level);
        }
        else
        {
            spaceLevelVisual.SetActive(false);
        }
    }
}