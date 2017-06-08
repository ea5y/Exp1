using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scm.Common;
using Scm.Common.GameParameter;

public class DetectRay : MonoBehaviour
{
    public static DetectRay Instance;
    public Transform mTransform;
    public Transform mCamRoot;
//    public float x = 20;
//    public float y = 12;
//    public float z = 1;
//    public float w = 1;
//    public int e = 12;
    private float x = 20;
    private float y = 12;
    private float z = 1;
    private float w = 1;
    private int e = 12;

    public CamType camType = CamType.Normal;

    public enum CamType
    {
        Normal = 0,
        Lock,
        NoLock,
    }

    //    private LayerMask mask = 1 << LayerNumber.vsPlayer_Bullet | 1 << LayerNumber.vsPlayer;
    private LayerMask mask = 1 << LayerNumber.vsPlayer_Bullet;
    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //        Detect();
    }

    public void NextCamType()
    {
        int tCur = (int) camType;
        tCur++;
        tCur = tCur%3;
        camType = (CamType) tCur;
        //GUITargetButton.Instance.ChangeSprite(camType);
    }

    public void Detect()
    {
        if (camType == CamType.NoLock)
        {
            OUILockon.LockNone();
            return;
        }
        if (camType == CamType.Lock)
        {
            return;
        }
        OUILockon.LockNone();
        Player p = GameController.GetPlayer();
        if (null == mTransform || null == p)
        {
            return;
        }
        Vector3 O = mTransform.position;
        Vector3 F = p.InputDirection;
//        Debug.LogError(p.InputDirection);
        if (F == Vector3.zero)
        {
            F = mTransform.transform.forward;
        }

        {
            Dictionary<Collider, float> tStore = new Dictionary<Collider, float>();
            RaycastHit[] hits;

            for (int i = 1; i < e; i++)
            {
                Matrix2x2 t1 = new Matrix2x2(0.1f * i);
                Matrix2x2 t2 = new Matrix2x2(-0.1f * i);
                Vector3 FL = t1 * F;
                Vector3 FR = t2 * F;
                for (int j = 0; j < z; j++)
                {
                    Vector3 O_ = O + j * Vector3.up * 0.1f + Vector3.up * w;
#if UNITY_EDITOR
                    Debug.DrawLine(O_, O_ + FL * x, Color.blue);
                    Debug.DrawLine(O_, O_ + FR * x, Color.red);
                    Debug.DrawLine(O_, O_ + F * x, Color.green);
#endif


                    if (1 == i)
                    {
                        hits = Physics.RaycastAll(O_, F, x, mask);
                        AddRayHitToDic(ref tStore, ref hits);
                    }

                    hits = Physics.RaycastAll(O_, FL, x, mask);
                    AddRayHitToDic(ref tStore, ref hits);

                    hits = Physics.RaycastAll(O_, FR, x, mask);
                    AddRayHitToDic(ref tStore, ref hits);
                }
            }

            //            tStore.OrderBy(pair => pair.Value);
            var list = tStore.OrderBy(pair => pair.Value).ToList();

            foreach (var hit in list)
            {
                var t = hit.Key.GetComponent<ObjectBase>();
                if (null != t && CanAttackTarget(t))
                {
                    GUIObjectUI.LockOnTarget(t);
                    return;
                }
            }
        }

        for (int i = e; i < y; i++)
        {
            Matrix2x2 t1 = new Matrix2x2(0.1f * i);
            Matrix2x2 t2 = new Matrix2x2(-0.1f * i);
            Vector3 FL = t1 * F;
            Vector3 FR = t2 * F;
            for (int j = 0; j < z; j++)
            {
                Vector3 O_ = O + j * Vector3.up * 0.1f + Vector3.up * w;
#if UNITY_EDITOR
                Debug.DrawLine(O_, O_ + FL * x, Color.blue);
                Debug.DrawLine(O_, O_ + FR * x, Color.red);
                Debug.DrawLine(O_, O_ + F * x, Color.green);
#endif

                List<RaycastHit> tStore = new List<RaycastHit>();
                RaycastHit[] hits;

                hits = Physics.RaycastAll(O_, FL, x, mask);

                for (int k = 0; k < hits.Length; ++k)
                {
                    var t = hits[k].collider.GetComponent<ObjectBase>();
                    if (null != t && CanAttackTarget(t))
                    {
                        GUIObjectUI.LockOnTarget(t);
                        return;
                    }
                }

                hits = Physics.RaycastAll(O_, FR, x, mask);

                for (int k = 0; k < hits.Length; ++k)
                {
                    var t = hits[k].collider.GetComponent<ObjectBase>();
                    if (null != t && CanAttackTarget(t))
                    {
                        GUIObjectUI.LockOnTarget(t);
                        return;
                    }
                }
            }
        }
    }

    private bool CanAttackTarget(ObjectBase pTarget)
    {
//        Debug.Log(pTarget.EntrantInfo.StatusType);
        if (pTarget.EntrantInfo.StatusType != StatusType.Normal)
        {
//            Debug.LogError("===>" );
//            return false;
        }
        Player p = GameController.GetPlayer();
        // 同じチーム
        if (p.TeamType == pTarget.TeamType)
        { return false; }
        // 特定オブジェクト
        switch (pTarget.EntrantType)
        {
            case EntrantType.Unknown:
            case EntrantType.Jump:
//            case EntrantType.Transporter:
            case EntrantType.Item:
            case EntrantType.Wall:
            case EntrantType.Barrier:
            case EntrantType.Start:
            case EntrantType.Respawn:
            case EntrantType.Hostage:   // Grabbing a host may cause bug
                return false;
            case EntrantType.MiniNpc:
            case EntrantType.Mob:
                if (!pTarget.IsBreakable)
                {
                    // 破壊不可能なものを対象から外す
                    return false;
                }
                break;
        }
        return true;
    }

    private void AddRayHitToDic(ref Dictionary<Collider, float> pDic, ref RaycastHit[] pHits)
    {
        for (int i = 0; i < pHits.Length; ++i)
        {
            pDic[pHits[i].collider] = pHits[i].distance;
        }
    }
}
