using UnityEngine;
using System.Collections;
using Scm.Common.Master;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RadarController : Singleton<RadarController>
{
    public TFTRadarChart tFTRadarChart;
    public GameObject Root;
    public GameObject goTFTRadarChart;
    const int BaseNum = 10;

    private void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start () {
        float x = Screen.width * 1.0f / Screen.height;
        float y = 1.1765f * x - 1.0824f;
        if (x < 1.26f)
        {
            y = 0.7f;
        }
        else if (x < 1.34f)
        {
            y = 0.75f;
        }
        else if (x < 1.55f)
        {
            y = 0.85f;
        }
        else if (x < 1.65f)
        {
            y = 0.9f;
        }
        else
        {
            y = 1f;
        }
        Root.transform.localScale = new Vector3(y, y, y);
	}

    static public void Open()
    {
        if (Instance != null) Instance.goTFTRadarChart.SetActive(true);
    }

    static public void Close()
    {
        if (Instance != null) Instance.goTFTRadarChart.SetActive(false);
    }
    void OnEnable()
    {
        CharacterListModel.OnCharaProfileChange += CharacterListModel_OnCharaProfileChange;
    }

    void OnDisable()
    {
        CharacterListModel.OnCharaProfileChange -= CharacterListModel_OnCharaProfileChange;
    }

    void CharacterListModel_OnCharaProfileChange(object sender, System.EventArgs e)
    {
        var charaProfileMasterData = sender as CharaProfileMasterData;
        if (charaProfileMasterData == null) return;
        float[] points = new float[5];
        points[0] = charaProfileMasterData.Atk / BaseNum;
        points[1] = charaProfileMasterData.Ctrl / BaseNum;
        points[2] = charaProfileMasterData.Aid / BaseNum;
        points[3] = charaProfileMasterData.Spd / BaseNum;
        points[4] = charaProfileMasterData.Def / BaseNum;
        tFTRadarChart.SetPoints(points);
        tFTRadarChart.SetAllDirty();
    }

    public void SetRadar(CharaInfo charaInfo)
    {
        CharaProfileMasterData data;
        MasterData.TryGetCharaProfileMasterData((int)charaInfo.AvatarType, out data);

        float[] points = new float[5];
        points[0] = data.Atk / BaseNum;
        points[1] = data.Ctrl / BaseNum;
        points[2] = data.Aid / BaseNum;
        points[3] = data.Spd / BaseNum;
        points[4] = data.Def / BaseNum;
        tFTRadarChart.SetPoints(points);
        tFTRadarChart.SetAllDirty();
    }
}
