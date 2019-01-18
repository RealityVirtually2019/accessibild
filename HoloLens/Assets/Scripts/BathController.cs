using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class BathController : MonoBehaviour, HoloToolkit.Unity.InputModule.IInputClickHandler
{

    public Material MeshingMaterial;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Vector3 clickRay;
        if (eventData.InputSource.TryGetGripPosition(eventData.SourceId, out clickRay))
        {
            HintBox.Instance.ShowText("Ray:" + (clickRay - CameraCache.Main.transform.position));
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
