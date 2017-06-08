using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckDragItem : UIDragDropItem {
    public Dictionary<Transform, Vector3> StartPos = new Dictionary<Transform,Vector3>();
    protected override void OnDragDropStart()
    {
        StartPos.Clear();
        mGrid = NGUITools.FindInParents<UIGrid>(transform);
        foreach (var item in mGrid.GetChildList())
        {
            StartPos.Add(item, item.localPosition);
        }
        base.OnDragDropStart();
    }

    protected override void OnDragDropRelease(GameObject surface)
    {
        GUIDeckEditItem FirstItem = mGrid.GetChildList()[0].GetComponent<GUIDeckEditItem>();
        if (FirstItem.CharaInfo.UUID == 0)
        {
            transform.parent = mGrid.transform;
            foreach (var item in mGrid.GetChildList())
            {
                item.localPosition = StartPos[item];
            }
            // Re-enable the collider
            if (mButton != null) mButton.isEnabled = true;
            else if (mCollider != null) mCollider.enabled = true;
            else if (mCollider2D != null) mCollider2D.enabled = true;
            return;
        }

        if (transform.GetComponent<GUIDeckEditItem>().CharaInfo.UUID == 0)
        {
            Vector3 FirstItemPos = mGrid.GetChildList()[0].transform.position;
            if (transform.position.x < FirstItemPos.x)
            {
                transform.parent = mGrid.transform;
                foreach (var item in mGrid.GetChildList())
                {
                    item.localPosition = StartPos[item];
                }
                // Re-enable the collider
                if (mButton != null) mButton.isEnabled = true;
                else if (mCollider != null) mCollider.enabled = true;
                else if (mCollider2D != null) mCollider2D.enabled = true;
                return;
            }
        }
        base.OnDragDropRelease(surface);
        GUIDeckEdit.Instance.ResetDeckOrder();
    }
}
