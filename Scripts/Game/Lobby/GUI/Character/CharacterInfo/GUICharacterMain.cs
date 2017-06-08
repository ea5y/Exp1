using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class GUICharacterMain : MonoBehaviour {
    public Transform transOn;
    public UISprite icon;
    public UILabel title;
    public UILabel level;
    public UILabel type;
    public UILabel warcount;
    
    [HideInInspector]
    public CharaInfo charaInfo;
    CharaIcon charaIcon { get { return ScmParam.Lobby.CharaIcon; } }
    SkillIcon skillIcon { get { return ScmParam.Battle.SkillIcon; } }
    CharaBoard CharaBoard { get { return ScmParam.Lobby.CharaBoard; } }
    int SkillIconLv { get { return 1; } }

    public void Setup(CharaInfo charaInfo)
    {
        this.charaInfo = charaInfo;
        gameObject.SetActive(true);
        string typeDescription = "";
        {
            Scm.Common.Master.CharaMasterData data;
            if (MasterData.TryGetChara((int)this.charaInfo.AvatarType, out data))
            if (data != null) typeDescription = data.Description;
        }
        title.text = charaInfo.Name;
        level.text = charaInfo.Level.ToString();
        type.text = typeDescription;
        //charaIcon.GetIcon(this.charaInfo.AvatarType, false, this.SetIconSprite);
//        if (charaInfo.AvatarType == AvatarType.P012_Azurael || charaInfo.AvatarType == AvatarType.P015_Horus)
//        {
//            transOn.localScale = Vector3.one * 0.5f;
//            transOn.localPosition = new Vector3(transOn.localPosition.x, 50, transOn.localPosition.z);
//        }
//        if (charaInfo.AvatarType == AvatarType.P019_Cylinder)
//        {
//            transOn.localScale = Vector3.one * 0.5f;
//            transOn.localPosition = new Vector3(transOn.localPosition.x, 0, transOn.localPosition.z);
//        }
//        else if (charaInfo.AvatarType == AvatarType.P011_Meru)
//        {
//            transOn.localScale = Vector3.one * 0.6f;
//            transOn.localPosition = new Vector3(transOn.localPosition.x, 50, transOn.localPosition.z);
//        } 
        CharaBoard.GetBoard(this.charaInfo.AvatarType,this.charaInfo.SkinId, true, (GameObject resource) => { this.CreateBoard(this.charaInfo.AvatarType, resource, transOn, null); });
    }

    void CreateBoard(AvatarType avatarType, GameObject resource, Transform parent, UIPlayTween playTween)
    {
        // リソース読み込み完了
        if (resource == null || parent == null)
            return;
        // インスタンス化
        var go = SafeObject.Instantiate(resource) as GameObject;
        if (go == null)
            return;

        // 読み込み中に別のキャラに変更していたら破棄する
        /*
        var info = this.SelectInfo;
        if (info != null && info.AvatarType != avatarType)
        {
            Object.Destroy(go);
            return;
        }*/

        // 名前設定
        go.name = resource.name;
        // 親子付け
        var t = go.transform;
        parent.DestroyChildren();
        t.parent = parent;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        // 読み込みが完了してから再生を開始する
        if (playTween != null) playTween.Play(true);
    }

    void SetIconSprite(UIAtlas atlas, string spriteName)
    {
        icon.atlas = atlas;
        icon.spriteName = spriteName;
    }
}
