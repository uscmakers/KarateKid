using System.Collections;
using UnityEngine;
using TMPro;

public class SpriteMoveController : MonoBehaviour
{
    public GameObject targetText;  
    public float destroyTextAfter = 5f;  
    public float moveSpeed = 5f;  

    private bool canMove = false;  
    void Start()
    {
       
        if (targetText != null)
        {
            targetText.SetActive(false);
        }

        StartCoroutine(WaitForTextToAppear());
    }

    void Update()
    {
        if (canMove)
        {
            float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            transform.Translate(new Vector3(moveX, moveY, 0));
        }
    }

    IEnumerator WaitForTextToAppear()
    {
      
        while (targetText != null && !targetText.activeSelf)
        {
            yield return null; 
        }

        canMove = true;
        yield return new WaitForSeconds(destroyTextAfter);
        if (targetText != null)
        {
            Destroy(targetText);
        }
    }
}
