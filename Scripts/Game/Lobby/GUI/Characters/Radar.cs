using UnityEngine;
using System.Collections;

public class Radar : Singleton<Radar> {
    Vector2 point1;
    Vector2 point2;
    Vector2 point3;
    Vector2 point4;
    Vector2 point5;
    Vector2 origin;

    protected Vector3 _point1;
    protected Vector3 _point2;
    protected Vector3 _point3;
    protected Vector3 _point4;
    protected Vector3 _point5;
    
    private float _value1;//ctrl
    private float _value2;//atk
    private float _value3;//def
    private float _value4;//spd
    private float _value5;//aid

    public MeshFilter _mf;
    public Material _mt;
    public MeshRenderer _mr;

    private Vector3[] newVertices;
    private Vector2[] newUV;
    private int[] newTriangles;

    private void Awake()
    {
        base.Awake();
        this.SetPoint();
    }

    public void SetValues(float value1, float value2, float value3, float value4, float value5)
    {
        this._value1 = value1;
        this._value2 = value2;
        this._value3 = value3;
        this._value4 = value4;
        this._value5 = value5;
    }
    
    void Start()
    {
        if (_mf == null)
        {
            _mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
        }
        if (_mr == null)
        {
            _mr = (MeshRenderer)this.gameObject.AddComponent(typeof(MeshRenderer));
            _mr.material = _mt;
        }
    }

    void SetPoint()
    {
        int scale = 200;
        point1 = new Vector2((float)-50 / scale, (float)12 / scale);
        point2 = new Vector2((float)0, (float)50 / scale);
        point3 = new Vector2((float)50 / scale, (float)12 / scale);
        point4 = new Vector2((float)30 / scale, (float)-50 / scale);
        point5 = new Vector2((float)-30 / scale, (float)-50 / scale);
        origin = new Vector2((float)0, (float)-8 / scale);
    }
    

    void Update()
    {
        _point1 = new Vector3(point1.x * _value1, point1.y * _value1, 0);
        _point2 = new Vector3(point2.x * _value2, point2.y * _value2, 0);
        _point3 = new Vector3(point3.x * _value3, point3.y * _value3, 0);
        _point4 = new Vector3(point4.x * _value4, point4.y * _value4, 0);
        _point5 = new Vector3(point5.x * _value5, point5.y * _value5, 0);
        
        newVertices = new Vector3[] { _point1, _point2, _point3, _point4, _point5, origin };
        
        newTriangles = new int[] { 5, 0, 1, 5, 1, 2, 5, 2, 3, 5, 3, 4, 5, 4, 0 };
        newUV = new Vector2[]{Vector2.Lerp (point1,origin,_value1),Vector2.Lerp (point2,origin,_value2),Vector2.Lerp (point3,origin,_value3),Vector2.Lerp (point4,origin,_value4)
        ,Vector2.Lerp (point5,origin,_value5),new Vector2(.5f,.5f)};

        
        _mf.mesh.vertices = newVertices;
        _mf.mesh.uv = newUV;
        _mf.mesh.triangles = newTriangles;
    }
}
