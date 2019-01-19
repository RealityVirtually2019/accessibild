using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class BathController : MonoBehaviour, HoloToolkit.Unity.InputModule.IInputClickHandler
{

    public Material MeshingMaterial;

    public GameObject DiscPrefab;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Vector3 clickRay;
        if (eventData.InputSource.TryGetGripPosition(eventData.SourceId, out clickRay))
        {
            Vector3 rayDirection = (clickRay - CameraCache.Main.transform.position);
            HintBox.Instance.ShowText("Ray:" + rayDirection);
            RaycastHit hitInfo;
            if (Physics.Raycast(CameraCache.Main.transform.position, rayDirection, out hitInfo))
            {
                if (hitInfo.normal.y > 0.7F)
                {//is point pretty much up
                    Instantiate(DiscPrefab, hitInfo.point, Quaternion.identity);
                }
            }
        }
        else
        {
            HintBox.Instance.ShowText("No Position");
        }
    }

    // Use this for initialization
    void Start()
    {

        HoloToolkit.Unity.SpatialMapping.SpatialMappingManager instance = HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance;
        instance.SurfaceMaterial = MeshingMaterial;
        instance.DrawVisualMeshes = true;
        InputManager.Instance.AddGlobalListener(gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
