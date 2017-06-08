using UnityEngine;
using System.Collections;
using System;

public class CharaModel : ObjectBase
{
    public static CharaInfo charaInfo;

    public static ScmAnimation anime;

    public override bool IsDrawEnable
    {
        get { return !IsDisappear; }
    }
        
    public static void Create(GameObject go, CharaInfo info)
    {
        charaInfo = info;

        anime = go.GetComponent<ScmAnimation>();
        if(anime == null)
        {
            anime = go.AddComponent<ScmAnimation>();
        }

        CharaModel model = go.GetComponent<CharaModel>();
        if (model == null)
        {
            model = go.AddComponent<CharaModel>();            
        }
        model.LoadCharaModel(info);
    }

    protected virtual void Awake()
    {
        //this.ScmAnimation = this.gameObject.GetSafeComponent<ScmAnimation>();
    }

    public override void LoadModelCompleted(GameObject model, AnimationReference animationData)
    {
        anime.Setup(model.GetComponent<Animation>());
        anime.LoadAnimationAssetBundle(charaInfo, animationData);
        anime.ResetAnimation();
                
        model.transform.localPosition = new Vector3(0, 0, 0);
        model.transform.localScale = new Vector3(300, 300, 300);
        //model.transform.localRotation = Quaternion.Euler(0.0f, 130.0f, 0.0f);
        model.transform.Rotate(new Vector3(0.0f, 168.0f, 0.0f));
        model.transform.SetChildLayer(LayerNumber.UI3D);

        var animation = model.GetComponentInChildren<Animation>();

        foreach(AnimationState anim in animation)
        {
            if(animation.IsPlaying(anim.name))
            {
                animation.clip = anim.clip;
            }
        }

        base.LoadModelCompleted(model, animationData);
    }
}
