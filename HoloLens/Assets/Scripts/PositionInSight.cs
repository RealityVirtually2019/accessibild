using HoloToolkit.Unity;
using UnityEngine;

public class PositionInSight : MonoBehaviour
{

    public Vector3 DesiredPosition = new Vector3(0, 0, 1);

    // Use this for initialization
    void Start()
    {
        transform.position = CameraCache.Main.transform.position + DesiredPosition;
        GetComponent<SphereBasedTagalong>().enabled = true;
    }

}
