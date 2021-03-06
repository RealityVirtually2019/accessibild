﻿using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class MainMenu : MonoBehaviour
{

    [System.Serializable]
    public class MenuEntry
    {
        public string Text;
        public string SceneNameToLoad;
        public bool Enabled;
        public Texture2D BtnImage;
    }

    private const string MenuPreWord = "open ";
    public Button ButtonPrefab;

    public AudioClip ClickSound;

    public MenuEntry[] MenuEntries;

    private string currentActiveOption;
    private KeywordRecognizer keywordRecognizer;

    private Dictionary<string, string> _keywordToScene = new Dictionary<string, string>();

    private List<string> Init()
    {
        var collection = GetComponent<ObjectCollection>();
        collection.NodeList = new List<CollectionNode>();
        List<string> keywords = new List<string>();

        foreach (var item in MenuEntries)
        {
            var btn = Instantiate(ButtonPrefab, transform);
            btn.GetComponent<CompoundButtonText>().Text = item.Text;
            var btnIcon = btn.GetComponent<CompoundButtonIcon>();
            btnIcon.OverrideIcon = true;
            btnIcon.SetIconOverride(item.BtnImage);
            MeshRenderer[] meshRenderer = btnIcon.GetComponentsInChildren<MeshRenderer>();
            ChangeButtonApperance(meshRenderer);
            collection.NodeList.Add(new CollectionNode
            {
                Name = item.Text,
                transform = btn.transform
            });
            if (item.Enabled)
            {
                keywords.Add(MenuPreWord + item.Text);
                _keywordToScene[MenuPreWord + item.Text] = item.SceneNameToLoad;
            }

            btn.enabled = item.Enabled;
            var audioSrc = btn.gameObject.AddComponent<AudioSource>();
            audioSrc.playOnAwake = false;

            btn.OnButtonClicked += (obj) =>
            {
                LoadMenuItem(item.SceneNameToLoad, audioSrc);
            };
        }
        collection.UpdateCollection();
        keywords.Add("main menu");
        keywords.Add("start analyzing");
        keywords.Add("stop analyzing");
        return keywords;

    }

    private static void ChangeButtonApperance(MeshRenderer[] meshRenderer)
    {
        foreach (var renderer in meshRenderer)
        {
            if (renderer.gameObject.name == "UIButtonSquareIcon")
            {
                renderer.sharedMaterial.SetFloat("_EnableEmission", 0F);
                renderer.sharedMaterial.SetFloat("_Emission", 0F);
                renderer.sharedMaterial.SetColor("_EmissiveColor", new Color(0f, 0f, 0f, 0f));
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        var keywords = Init();
        keywordRecognizer = new KeywordRecognizer(keywords.ToArray(), ConfidenceLevel.Medium);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void LoadMenuItem(string sceneName, AudioSource audioSrc)
    {
        audioSrc.Play();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        currentActiveOption = sceneName;
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            var keys = new List<string>(_keywordToScene.Keys);
            for (int i = 0; i < _keywordToScene.Count; i++)
            {
                if (Input.GetKeyDown("" + i))
                {
                    var sceneName = _keywordToScene[keys[i]];
                    LoadMenuItem(sceneName, GetComponentInChildren<AudioSource>());
                    return;
                }
            }
            if (Input.GetKeyDown("m"))
            {
                if (string.IsNullOrEmpty(currentActiveOption)) return;
                SceneManager.UnloadSceneAsync(currentActiveOption);
            }
        }
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        //contraintuitive but low and rejected are higher than high and medium
        if (args.confidence > ConfidenceLevel.Medium) return;
        switch (args.text)
        {
            case "start analyzing":
                if (ImageCapture.Instance.IsRecording) return;
                ImageCapture.Instance.StartRecording();
                break;
            case "stop analyzing":
                ImageCapture.Instance.ResetImageCapture();
                break;
            case "main menu":
                if (string.IsNullOrEmpty(currentActiveOption)) return;
                SceneManager.UnloadSceneAsync(currentActiveOption);
                break;
            default:
                if (_keywordToScene.ContainsKey(args.text)) return;
                {
                    LoadMenuItem(_keywordToScene[args.text], GetComponentInChildren<AudioSource>());
                }
                break;
        }
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        if (scene.name == currentActiveOption)
        {
            foreach (Transform item in transform)
            {
                item.gameObject.SetActive(true);
            }
            MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
            ChangeButtonApperance(meshRenderer);
            currentActiveOption = String.Empty;
        }
    }
}
