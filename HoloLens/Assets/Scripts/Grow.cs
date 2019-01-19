using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Grow : MonoBehaviour
{

    Transform _trans;
    public Vector3 GrowRate = new Vector3(1.01F, 1F, 1.01F);
    [System.Serializable]
    public class SizeChangedEvent : UnityEvent<string> { }
    public SizeChangedEvent SizeChanged;
    public UnityEvent SizingDonePositive;
    public UnityEvent SizingDoneNegative;
    private Renderer _renderer;
    public Text ToUpdateText;
    private Rigidbody _rigid;

    public float CurrentSize = 1.414213562373095F;
    private float _maxSize;

    // Use this for initialization
    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        _trans = transform;
        _renderer = GetComponentInChildren<Renderer>();
        //Invoke("Growing", 0.1f);
    }

    public void SetDiscDiameter(float size)
    {
        _maxSize = size;
    }

    private void FixedUpdate()
    {
        _trans.localScale = Vector3.Scale(_trans.localScale, GrowRate);
        //Invoke("Growing", 0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Magic number =/ don't like it 
        if (collision.impulse.sqrMagnitude > 100F)
        {
            ShutDown(false);
        }
    }

    private void ShutDown(bool result)
    {
        CancelInvoke("Growing");
        if (result)
        {
            if (SizingDonePositive != null)
            {
                if (HintBox.Instance)
                    HintBox.Instance.ShowText("<color=green><b>compliant</b></color>");
                SizingDonePositive.Invoke();
            }
        }
        else
        {
            if (SizingDoneNegative != null)
            {
                if (HintBox.Instance)
                    HintBox.Instance.ShowText("<color=red><b>not compliant</b></color>");
                SizingDoneNegative.Invoke();
            }
        }
        Destroy(_rigid);
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (SizeChanged != null)
        {
            CurrentSize = Mathf.Sqrt(_renderer.bounds.size.x * _renderer.bounds.size.x + _renderer.bounds.size.z * _renderer.bounds.size.z);
            //ToUpdateText.text = (CurrentSize / 2.54F * 100).ToString("0.00") + "''";
            SizeChanged.Invoke((CurrentSize / 2.54F * 100).ToString("0.00") + "''");
            if (CurrentSize > _maxSize + (0.05 * _maxSize))
            {
                ShutDown(true);
            }
        }

    }
}
