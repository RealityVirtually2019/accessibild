using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathController : MonoBehaviour {

    public Material MeshingMaterial;

	// Use this for initialization
	void Start () {

        HoloToolkit.Unity.SpatialMapping.SpatialMappingManager instance = HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance;
        instance.SurfaceMaterial = MeshingMaterial;
        instance.DrawVisualMeshes = true;


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
