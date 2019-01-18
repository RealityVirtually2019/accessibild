using HoloToolkit.Unity;
using UnityEngine;

public class GetCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Invoke("SetCam", 1F);
    }

    void SetCam()
    {
        GetComponent<Canvas>().worldCamera = CameraCache.Main;
    }

}
