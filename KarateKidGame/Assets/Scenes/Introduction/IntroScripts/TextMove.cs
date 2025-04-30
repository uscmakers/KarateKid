using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TextMove : MonoBehaviour
{
    public RectTransform karateText; 
    public RectTransform kidText;  
    public GameObject pressSpaceText; 

    public Vector3 karateTargetPos = new Vector3(-800f, 300f, 0f);  
    public Vector3 kidTargetPos = new Vector3(-300f, -8f, 0f);  
    public float moveSpeed = 2f;  
    public float waitAtTarget = 5f;  
    public string moduleSceneName = "ModuleDemo";  

    public DojoMove dojoMoveScript;  
    public float maxWaitForDojo = 5f;  

    void Start()
    {
        pressSpaceText.SetActive(false);
        StartCoroutine(WaitForDojoMoveToFinish());
    }

    void Update()
    {
        if (pressSpaceText.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(moduleSceneName);  
        }
    }

    IEnumerator WaitForDojoMoveToFinish()
    {
        if (dojoMoveScript == null)
        {
            dojoMoveScript = FindObjectOfType<DojoMove>();  
        }

        float timer = 0f;
        while (dojoMoveScript != null && dojoMoveScript.isActiveAndEnabled && timer < maxWaitForDojo)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (dojoMoveScript != null && dojoMoveScript.isActiveAndEnabled)
        {
            dojoMoveScript.enabled = false;
        }

        StartCoroutine(MoveTextToPosition());
    }

    IEnumerator MoveTextToPosition()
    {
        float t = 0f;

        // Check if text objects are valid before using them
        if (karateText == null || kidText == null)
        {
            Debug.LogWarning("One or both text references are missing.");
            yield break;
        }

        Vector3 karateStartPos = karateText.anchoredPosition;
        Vector3 kidStartPos = kidText.anchoredPosition;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            if (karateText != null)
                karateText.anchoredPosition = Vector3.Lerp(karateStartPos, karateTargetPos, t);

            if (kidText != null)
                kidText.anchoredPosition = Vector3.Lerp(kidStartPos, kidTargetPos, t);

            yield return null;
        }

        // Final position snap (with safety check)
        if (karateText != null)
            karateText.anchoredPosition = karateTargetPos;

        if (kidText != null)
            kidText.anchoredPosition = kidTargetPos;

        yield return new WaitForSeconds(waitAtTarget);

        if (karateText != null)
            Destroy(karateText.gameObject);

        if (kidText != null)
            Destroy(kidText.gameObject);

        pressSpaceText.SetActive(true);
    }
}
