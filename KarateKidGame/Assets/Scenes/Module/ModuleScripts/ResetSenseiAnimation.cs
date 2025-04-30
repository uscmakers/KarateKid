using UnityEngine;

public class ResetSenseiAnimation : MonoBehaviour
{
    public Animator animator;
    public string stateName = "YourAnimationState"; // e.g. "PunchCombo"
    public float resetInterval = 4f;

    private float timer = 0f;
    private int stateHash;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        stateHash = Animator.StringToHash(stateName);
        animator.Play(stateHash, 0, 0f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= resetInterval)
        {
            // Soft reset: re-play the animation from the start
            animator.Rebind(); // Reset internal Animator state
            animator.Update(0f); // Force apply changes
            animator.Play(stateHash, 0, 0f); // Replay from start
            timer = 0f;
        }
    }
}
