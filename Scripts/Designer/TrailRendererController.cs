using UnityEngine;
using System.Collections;

public class TrailRendererController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clear() {
        var renderers = GetComponentsInChildren<TrailRenderer>();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].Clear();
        }
    }
}
