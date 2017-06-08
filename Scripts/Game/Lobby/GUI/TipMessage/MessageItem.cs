using UnityEngine;
using System.Collections;

public class MessageItem : MonoBehaviour {
    public UILabel message;

    public float lifeTime = 0.5f;
    public float time = 0;

    public Vector3 targetPos;
    
    public void Destroy()
    {
        DestroyObject(this.gameObject);
    }
}
