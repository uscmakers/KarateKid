using UnityEngine;

public class FreezeAnimatorPosition : MonoBehaviour
{
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void LateUpdate()
    {
        transform.position = originalPosition;
    }
}
