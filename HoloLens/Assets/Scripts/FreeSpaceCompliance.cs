using UnityEngine;

public class FreeSpaceCompliance : MonoBehaviour
{

    private const float inch2cm = 2.54F;
    [Tooltip("True for inches false for cm")]
    public bool IsInInches = true;
    public float DiscHeight = 34F;
    private float _discHeight;
    public float DiscDiameter = 60F;
    private float _discDiameter;
    private Transform _transform;

    // Use this for initialization
    void Start()
    {
        _discHeight = (IsInInches ? DiscHeight * inch2cm : DiscHeight) / 100F;
        _discDiameter = (IsInInches ? DiscDiameter * inch2cm : DiscDiameter) / 100F;
        _transform = transform;
        float y = (_discHeight - _transform.parent.localPosition.y) / 2F;
        _transform.localScale = new Vector3(_transform.localScale.x, y, _transform.localScale.z);
        _transform.localPosition = new Vector3(_transform.localPosition.x, y, _transform.localPosition.z);
        SendMessageUpwards("SetDiscDiameter", _discDiameter);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
