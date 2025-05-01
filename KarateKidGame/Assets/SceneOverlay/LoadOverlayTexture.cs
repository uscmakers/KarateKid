using UnityEngine;
using UnityEngine.UI;

public class LoadOverlayTexture : MonoBehaviour
{
    void Start()
    {
        RenderTexture rt = Resources.Load<RenderTexture>("ModuleDemoView");
        if (rt != null)
        {
            GetComponent<RawImage>().texture = rt;
        }
    }
}
