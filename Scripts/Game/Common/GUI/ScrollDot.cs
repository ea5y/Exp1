using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollDot : MonoBehaviour
{
    public UICenterOnChild mUICenterOnChild;
    public List<GameObject> mScrollItems;
    public List<Transform> mBackDots;
    public Transform mFrontDot;

    private GameObject LastCenter = null;

    private void Start()
    {
        mUICenterOnChild.onCenter = OnCenter;
        {

        };
    }

    private void OnCenter(GameObject go)
    {
        if (go == LastCenter)
        {
            return;
        }
        LastCenter = go;
        mFrontDot.position = mBackDots[mScrollItems.IndexOf(go)].position;
    }
};
