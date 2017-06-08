using UnityEngine;
using System.Collections.Generic;

public class LoadUI : MonoBehaviour
{
    public Transform Root;
    public List<GameObject> mUI;

    private int CurInit = 0;
    // Use this for initialization
    void Awake()
    {
        float x = Screen.width * 1.0f / Screen.height;
        float y = 1.1765f * x - 1.0824f;
        if (x < 1.26f)
        {
            y = 0.7f;
        }
        else if (x < 1.34f)
        {
            y = 0.75f;
        }
        else if (x < 1.55f)
        {
            y = 0.85f;
        }
        else if (x < 1.65f)
        {
            y = 0.9f;
        }
        else
        {
            y = 1f;
        }
        Root.transform.localScale = new Vector3(y, y, y);
    }

    // Update is called once per frame
    void Update()
    {
        if (CurInit < mUI.Count)
        {
            var go = GameObject.Instantiate(mUI[CurInit]);
            go.transform.SetParent(Root);
            go.transform.localScale = mUI[CurInit].transform.localScale;
            go.transform.localPosition = mUI[CurInit].transform.localPosition;
            CurInit++;
        }
    } 
}
