using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemBase : GridItemBase
{
    public UISprite[] iconListLeft;
    public UISprite[] iconListRight;

    public UISprite result;

    public UILabel gameType;

    public UILabel gameTime;

    public XUIButton btnEnter;
    protected override void OnReposition(int i)
    {
        base.OnReposition(i);
        StartCoroutine(WaitTween(i));
    }

    protected override IEnumerator WaitTween(int i)
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
