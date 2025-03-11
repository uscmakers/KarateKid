using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PressSpaceHandler : MonoBehaviour
{
    public GameObject[] initialTexts; 
    public GameObject pressSpaceText; 
    public float maxTextMoveDuration = 5f; 
    public float delayBeforeDestroy = 3f; 
    public string moduleSceneName = "Module1Demo"; 

    public TextMove textMoveScript; 

    private bool textMoveFinished = false; 

    void Start()
    {

        pressSpaceText.SetActive(false);
        StartCoroutine(WaitForTextMoveToFinish());
    }

    void Update()
    {

        if (pressSpaceText.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(moduleSceneName);
        }
    }

    IEnumerator WaitForTextMoveToFinish()
    {

        if (textMoveScript == null)
        {
            textMoveScript = FindObjectOfType<TextMove>(); 
        }

        float timer = 0f;
        while (textMoveScript != null && textMoveScript.isActiveAndEnabled && timer < maxTextMoveDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (textMoveScript != null && textMoveScript.isActiveAndEnabled)
        {
            textMoveScript.enabled = false;
        }

        yield return new WaitForSeconds(delayBeforeDestroy);
        foreach (GameObject text in initialTexts)
        {
            if (text != null) Destroy(text);
        }

        pressSpaceText.SetActive(true);
    }
}
