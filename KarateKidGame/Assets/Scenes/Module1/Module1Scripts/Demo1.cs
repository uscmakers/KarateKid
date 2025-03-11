using System.Collections;
using UnityEngine;

public class Demo1 : MonoBehaviour
{
    public Animator animator; // Assign in Unity Inspector
    private string firstAnimation = "ANIM_LP TO RP SWAY"; // First animation state
    private string secondAnimation = "ANIM_RP TO LP SWAY"; // Next animation state
    public float pauseDuration = 1.5f; // Time to pause in seconds

    private bool isPaused = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // Detect when first animation is almost finished and initiate pause
        if (currentState.IsName(firstAnimation) && currentState.normalizedTime >= 0.95f && !isPaused)
        {
            StartCoroutine(PauseBeforeNextMove());
        }
    }

    IEnumerator PauseBeforeNextMove()
    {
        isPaused = true;
        animator.speed = 0; // Pause animation

        yield return new WaitForSeconds(pauseDuration); // Pause before next move

        animator.speed = 1; // Resume animation
        animator.Play(secondAnimation); // Transition to next animation

        isPaused = false;
    }
}
