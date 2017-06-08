using UnityEngine;
using System.Collections;

public class TweenItemPF : GridItemBase
{
    protected override void OnReposition(int i)
    {
        base.OnReposition(i);
        //        int j = i / 5;
        int j = i;
        StartCoroutine(WaitTween(j++));
    }

    protected override IEnumerator WaitTween(int i)
    {
        //        yield return new WaitForSeconds(i * 0.5f);
        yield return new WaitForSeconds(i * 0.05f);
        Transform target = transform;
        target.localScale = LastLocalScale;
        Vector3 pos = target.localPosition;
        TweenPosition t = target.GetComponent<TweenPosition>();
        if (null == t)
        {
            t = target.gameObject.AddComponent<TweenPosition>();
        }
        t.enabled = false;
        t.ResetToBeginning();
        t.duration = 0.2f;
        t.from = new Vector3(pos.x, -800, pos.z);
        t.to = pos;
        t.PlayForward();
    }    
}
