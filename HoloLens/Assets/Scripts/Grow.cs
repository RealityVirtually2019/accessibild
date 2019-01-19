using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Grow : MonoBehaviour
{

    Transform _trans;
    public Vector3 GrowRate = new Vector3(1.01F, 1F, 1.01F);
    public UnityEvent SizeChanged;
    public UnityEvent SizingDone;
    private Renderer _renderer;
    public Text ToUpdateText;

    public float CurrentSize = 1.414213562373095F;
    private float _maxSize;

    // Use this for initialization
    void Start()
    {
        _trans = transform;
        _renderer = GetComponentInChildren<Renderer>();
    }

    public void SetDiscDiameter(float size)
    {
        _maxSize = size;
    }

    private void FixedUpdate()
    {
        _trans.localScale = Vector3.Scale(_trans.localScale, GrowRate);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Magic number =/ don't like it 
        if (collision.impulse.sqrMagnitude > 100F)
        {
            if (SizingDone != null)
            {
                SizingDone.Invoke();
            }
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SizeChanged != null)
        {
            CurrentSize = Mathf.Sqrt(_renderer.bounds.size.x * _renderer.bounds.size.x + _renderer.bounds.size.z * _renderer.bounds.size.z);
            ToUpdateText.text = (CurrentSize / 2.54F * 100).ToString("0.00") + "''";
            SizeChanged.Invoke();
            if (CurrentSize > _maxSize + (0.05 * _maxSize))
            {
                if (SizingDone != null)
                {
                    SizingDone.Invoke();
                }
                Destroy(this);
            }
        }

    }
}
