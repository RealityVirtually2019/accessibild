using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayRelativeTo : MonoBehaviour {

    public Transform Reference;
    private Transform _trans;
    private Vector3 _relativePosition;

	// Use this for initialization
	void Start () {
        _trans = transform;
        _relativePosition = _trans.position - Reference.position;
	}
	
	// Update is called once per frame
	void Update () {
        _trans.position = Reference.position + _relativePosition;
	}
}
