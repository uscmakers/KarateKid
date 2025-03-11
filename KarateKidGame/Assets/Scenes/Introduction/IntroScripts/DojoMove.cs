using UnityEngine;

public class DojoMove : MonoBehaviour
{
    public float targetZ = -12f; 
    public float moveSpeed = 2f; 

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(startPosition.x, startPosition.y, targetZ);
    }

    void Update()
    {
        // Move towards the target Z position smoothly
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // Optional: Stop moving when close enough
        if (Mathf.Abs(transform.position.z - targetZ) < 0.01f)
        {
            transform.position = targetPosition; 
            enabled = false; 
        }
    }
}
