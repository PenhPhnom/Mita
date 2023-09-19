using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FindUICamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera uiCamera = GameObject.Find("cameraUI").GetComponent<Camera>();
        if (uiCamera != null)
        {
            this.GetComponent<Camera>().GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
