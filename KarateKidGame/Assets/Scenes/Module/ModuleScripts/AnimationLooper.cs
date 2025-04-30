using UnityEngine;

public class AnimationLooper : MonoBehaviour
{
    public Animator animator;
    public string animationStateName;
    public float clipDuration = 5f; // Seconds to play before looping

    private float timer;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        animator.Play(animationStateName, 0, 0f);
        animator.speed = 1f;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= clipDuration)
        {
            animator.Play(animationStateName, 0, 0f); // Restart animation
            timer = 0f;
        }
    }
}
