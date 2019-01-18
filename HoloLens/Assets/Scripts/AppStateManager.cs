using UnityEngine;

public class AppStateManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
#if !UNITY_EDITOR
        UnityEngine.SceneManagement.SceneManager.LoadScene("BathRoomScanning", UnityEngine.SceneManagement.LoadSceneMode.Additive);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
