using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
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
            if (Physics.Raycast(CameraCache.Main.transform.position, CameraCache.Main.transform.forward, out hitInfo))
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

    List<AudioSource> _speach = new List<AudioSource>();

    // Use this for initialization
    void Start()
    {
        HoloToolkit.Unity.SpatialMapping.SpatialMappingManager instance = HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance;
        instance.SurfaceMaterial = MeshingMaterial;
        instance.DrawVisualMeshes = true;
        InputManager.Instance.AddGlobalListener(gameObject);
        _speach.AddRange(GetComponents<AudioSource>());
        Invoke("PlayAudio", 1F);
    }



    public void PlayAudio()
    {
        if (_speach.Count < 1) return;
        _speach[0].Play();
        Invoke("PlayAudio", _speach[0].clip.length + 1F);
        _speach.RemoveAt(0);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
