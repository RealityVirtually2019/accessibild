using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintBox : Singleton<HintBox> {

    private Text _textObject;

	// Use this for initialization
	void Start () {
        _textObject = GetComponentInChildren<Text>();
	}

    public void ShowText(string text)
    {
        _textObject.text = text;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
