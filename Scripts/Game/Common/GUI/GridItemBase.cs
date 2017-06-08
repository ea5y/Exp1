using UnityEngine;
using System.Collections;

public class GridItemBase : MonoBehaviour
{
    protected Vector3 LastLocalScale;
    private bool Inited = false;

    protected virtual void OnReposition(int i)
    {
        if (!Inited)
        {
            LastLocalScale = transform.localScale;
            Inited = true;
        }
        transform.localScale = Vector3.zero;
    }

    protected virtual IEnumerator WaitTween(int i)
    {
        yield return new WaitForSeconds(i * 0.1f);
        transform.localScale = LastLocalScale;
        Vector3 pos = transform.localPosition;
        TweenPosition t = transform.GetComponent<TweenPosition>();
        if (null == t)
        {
            t = gameObject.AddComponent<TweenPosition>();
        }
        t.enabled = false;
        t.ResetToBeginning();
        t.duration = 0.2f;
        t.from = new Vector3(pos.x, -800, pos.z);
        t.to = pos;
        t.PlayForward();
    }
}
