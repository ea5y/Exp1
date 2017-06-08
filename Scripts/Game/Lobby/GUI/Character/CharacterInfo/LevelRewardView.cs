using UnityEngine;
using System.Collections;
using Scm.Common.Master;

public class LevelRewardView : MonoBehaviour {
    public UISprite icon;
    public UILabel level;
    public UILabel title;

    public void Setup(CharaLevelMasterData charaLevelMasterData, int avatarId)
    {
        gameObject.SetActive(true);
        level.text = charaLevelMasterData.Level.ToString();
        RewardMasterData data;
        if(MasterData.TryGetRewardMasterData(charaLevelMasterData.RewardId, out data))
        {
            title.text = data.Description;
        }
    }
}
