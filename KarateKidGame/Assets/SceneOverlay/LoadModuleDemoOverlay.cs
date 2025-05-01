using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadModuleDemoOverlay : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("ModuleDemo", LoadSceneMode.Additive);
    }
}
