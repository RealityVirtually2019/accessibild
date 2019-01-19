using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections.Generic;
using UnityEngine;

public class BathController : MonoBehaviour, HoloToolkit.Unity.InputModule.IInputClickHandler
{

    public Material MeshingMaterial;

    public GameObject DiscPrefab;

    private float _floorHeight = 1.3f;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Vector3 clickRay;
        if (eventData.InputSource.TryGetGripPosition(eventData.SourceId, out clickRay))
        {
            Vector3 rayDirection = (clickRay - CameraCache.Main.transform.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(CameraCache.Main.transform.position, CameraCache.Main.transform.forward, out hitInfo))
            {
                if (hitInfo.normal.y > 0.7F)
                {//is point pretty much up
                    Instantiate(DiscPrefab, hitInfo.point, Quaternion.identity);
                }
            }
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
        //Invoke("FindFloor", 2F);
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
        if (GazeManager.Instance.HitPosition != null && GazeManager.Instance.HitPosition.y < _floorHeight)
        {
            var material = SpatialMappingManager.Instance.SurfaceMaterial;
            _floorHeight = GazeManager.Instance.HitPosition.y;
            material.SetVector("_ClipPlane", new Vector4(0f, -1f, 0f, (_floorHeight + .1f) * -1));
            SpatialMappingManager.Instance.SetSurfaceMaterial(material);
        }
    }
}
