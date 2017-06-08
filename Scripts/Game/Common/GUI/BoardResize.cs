using UnityEngine;
using System.Collections;

public class BoardResize : MonoBehaviour
{
    public float Scale;
    // Use this for initialization
    void Start()
    {
        transform.localScale = Vector3.one * Scale;
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    void OnValidate()
    {
        transform.localScale = Vector3.one * Scale;
    }
#endif
}
