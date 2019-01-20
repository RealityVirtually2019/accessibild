using System.Collections.Generic;
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
    private List<Vector3> _hitPoints = new List<Vector3>();

    private int checkedPoints = 0;

    private static Vector3 _planeProjection = new Vector3(1f, 0f, 1f);

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
        foreach (var contact in collision.contacts)
        {
            Vector3 point = contact.point;
            _hitPoints.Add(ConvertToVec2(point));
            //for debuging uncomment to show hitpoints recognized
            //var res = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //var mat = new Material(res.GetComponent<Renderer>().sharedMaterial);
            //mat.color = Color.green;
            //res.GetComponent<Renderer>().sharedMaterial = mat;
            //res.transform.position = ConvertToVec2(point);
            //res.transform.localScale = new Vector3(0.1F, 0.1f, 0.1f);
            //res.GetComponent<Collider>().enabled = false;


            //res = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //mat = new Material(res.GetComponent<Renderer>().sharedMaterial);
            //mat.color = Color.red;
            //res.GetComponent<Renderer>().sharedMaterial = mat;
            //res.transform.position = _trans.position;
            //res.transform.localScale = new Vector3(0.1F, 0.1f, 0.1f);
            //res.GetComponent<Collider>().enabled = false;
        }

        CheckIfCanGrow();
    }

    private void CheckIfCanGrow()
    {
        CleanupPoint();
        if (_hitPoints.Count > 2)// && checkedPoints < _hitPoints.Count)
        {
            checkedPoints = _hitPoints.Count;
            var position2D = ConvertToVec2(_trans.position);
            //find most "left" hit (on circle)
            var leftIdx = 0;
            var mostLeftVal = 0F;
            var leftDir = _hitPoints[leftIdx] - position2D;
            for (int i = 1; i < checkedPoints; i++)
            {
                float tmp;
                if ((tmp = Vector3.SignedAngle(leftDir, _hitPoints[i] - position2D, Vector3.up)) < mostLeftVal)
                {
                    mostLeftVal = tmp;
                    leftIdx = i;
                }
            }
            var leftVector = _hitPoints[leftIdx] - position2D;
            var angle = 0F;
            for (int i = 0; i < _hitPoints.Count; i++)
            {
                //Debug.DrawLine(_trans.position, _hitPoints[i]);
                var cur = _hitPoints[i];
                if (Vector3.Distance(cur, _hitPoints[leftIdx]) < 0.02) continue;
                var tmp = Vector3.SignedAngle(leftVector, _hitPoints[i] - position2D, Vector3.up);
                if (tmp < 0)
                {
                    angle = 360;
                    break;
                }
                if (tmp > angle)
                {
                    angle = tmp;
                }
            }
            if (angle > 180)
            {//circle can't expand anymore
                ShutDown();
                //UnityEditor.EditorApplication.isPaused = true;
            }
        }
    }

    private void CleanupPoint()
    {
        if (_hitPoints.Count < 1) return;
        List<Vector3> accepted = new List<Vector3>();
        accepted.Add(_hitPoints[0]);
        for (int i = 1; i < _hitPoints.Count; i++)
        {
            var minDist = Vector3.Distance(_hitPoints[i], accepted[0]);
            for (int j = 1; j < accepted.Count; j++)
            {
                var tmp = Vector3.Distance(_hitPoints[i], accepted[j]);
                if (tmp < minDist)
                {
                    minDist = tmp;
                }
            }
            if (minDist > 0.02F)
            {
                accepted.Add(_hitPoints[i]);
            }
        }
        _hitPoints = accepted;
    }

    private static Vector3 ConvertToVec2(Vector3 point)
    {
        return Vector3.Scale(point, _planeProjection);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            Vector3 point = contact.point;
            _hitPoints.Add(ConvertToVec2(point));
        }
    }

    private void ShutDown()
    {
        CancelInvoke("Growing");
        if (CheckSize(false))
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
        //foreach (var item in GetComponentsInChildren<Renderer>())
        //{
        //    item.enabled = false;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (_hitPoints.Count > 3)
        {
            CheckIfCanGrow();
        }
        _hitPoints.Clear();
        if (SizeChanged != null)
        {
            bool isBigEnough = CheckSize(true);
            if (isBigEnough)
            {
                ShutDown();
            }
        }

    }

    private bool CheckSize(bool addTolerance)
    {
        CurrentSize = Mathf.Sqrt(_renderer.bounds.size.x * _renderer.bounds.size.x + _renderer.bounds.size.z * _renderer.bounds.size.z);
        //ToUpdateText.text = (CurrentSize / 2.54F * 100).ToString("0.00") + "''";
        SizeChanged.Invoke((CurrentSize / 2.54F * 100).ToString("0.00") + "''");
        bool isBigEnough = CurrentSize > _maxSize + (addTolerance ? (0.05F * _maxSize) : 0F);
        return isBigEnough;
    }
}
