/// <summary>
/// チュートリアル バトル
/// 
/// 2014/06/23
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialBattle : TutorialScript
{
	#region Fields & Proterties
	
	private List<MainTower> enemyMainTowers = new List<MainTower>();
	private List<MainTower> friendMainTowers = new List<MainTower>();
	private List<SubTower> enemySubTowers = new List<SubTower>();
	private List<SubTower> friendSubTowers = new List<SubTower>();
	private List<TankBase> tanks = new List<TankBase>();

    private GameObject cameraPosition;
    private GameObject cameraTarget;

    #endregion

    #region MonoBehaviour
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    #endregion

    #region TutorialScript

    protected override IEnumerator loadResources()
    {
        yield return StartCoroutine(base.loadResources());
    }

    protected override void unloadResources()
    {
        base.unloadResources();
    }

    protected override IEnumerator setup()
    {
        yield return StartCoroutine(base.setup());
        
        // TODO 暫定処置
        while (ObjectManager.Instance.Count <= 0)
        {
            yield return null;
        }
        
        foreach (var mt in Entrant.FindAll<MainTower>(obj => true))
        {
            if (StaticClassScm.GetClientTeam(mt.TeamType) == TeamTypeClient.Enemy)
            {
                enemyMainTowers.Add(mt);
            }
            else if (StaticClassScm.GetClientTeam(mt.TeamType) == TeamTypeClient.Friend)
            {
                friendMainTowers.Add(mt);
            }
        }
        foreach (var st in Entrant.FindAll<SubTower>(obj => true))
        {
            if (StaticClassScm.GetClientTeam(st.TeamType) == TeamTypeClient.Enemy)
            {
                enemySubTowers.Add(st);
            }
            else if (StaticClassScm.GetClientTeam(st.TeamType) == TeamTypeClient.Friend)
            {
                friendSubTowers.Add(st);
            }
        }
        foreach (var tb in Entrant.FindAll<TankBase>(obj => true))
        {
            tanks.Add(tb);
        }
        
        yield break;
    }

    protected override void startTutorial()
    {
        base.startTutorial();
    }

    protected override void stopTutorial()
    {
        base.stopTutorial();
    }
    
    /// <summary>
    /// スクリプト
    /// </summary>
    protected override IEnumerator script()
    {
        // 暫定処置
        while (!MapManager.Instance.MapExists) yield return null;
        yield return new WaitFrames(1);

		setupMessageWindow();
        moveStickEnabled(false);
        skillButtonEnabled(false);
        deleteTutorialNotice();

        yield return new WaitSeconds(2.0f);

        openNextWindow();
        setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);

        setMessage("シフト終了！\r\nここがバトルフィールドだよ");
        
        yield return new WaitNext();
        
        setMessage("バトルフィールドはロビーと違って\r\n戦闘が許可されているんだ");
        
        yield return new WaitNext();
        
        setSkillButtonNotice();
        setMessage("視界の右下にボタンが見えるかな？");
        
        yield return new WaitNext();
        
        setMessage("それにタッチすると\r\nクロスアクターのスキルが\r\n発動するよ");

        yield return new WaitNext();
        
        deleteTutorialNotice();
        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
        moveStickEnabled(true);
        skillButtonEnabled(true);

        yield return new WaitPlayerSkill(3);

        yield return new WaitSeconds(2.0f);
        
        openNextWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
        moveStickEnabled(false);
        skillButtonEnabled(false);
        startTankCamera();

        setMessage("アレ？セルが迷い込んできたみたいだ");

        yield return new WaitNext();

        setMessage("丁度いいや\r\nスキルを使ってセルを駆除してみてね");

        yield return new WaitNext();
        
        stopCamera();
        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
        moveStickEnabled(true);
        skillButtonEnabled(true);

        yield return new WaitTankDead(1);

        yield return new WaitSeconds(2.0f);
        
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        moveStickEnabled(false);
        skillButtonEnabled(false);

        setMessage("流石、手際がいいね！");
    
        yield return new WaitNext();

        setMessage("バトルフィールドには\r\nメインとサブのタワーが\r\n設置されているんだ");
        
        yield return new WaitNext();

        setGuide(GUIMessageWindow.GuideMode.None);
        startFriendMainTowerCamera();

        setMessage("青い方がキミのメインタワーで");

        yield return new WaitNext();

        stopCamera();
        startEnemyMainTowerCamera();

        setMessage("赤い方が敵のメインタワーだよ");
        
        yield return new WaitNext();

        stopCamera();
        startEnemySubTowerCamera();

        setMessage("ただ、このマップでは\r\nサブタワーを壊さないと\r\nメインタワーには辿りつけないんだ");
        
        yield return new WaitNext();

        setMessage("さっそくだけど、まずサブタワーを\r\n壊してみよう！\r\nキミならできるよ！");
        
        yield return new WaitNext();

        stopCamera();
        closeWindow();
        moveStickEnabled(true);
        skillButtonEnabled(true);

        yield return new WaitSubTowerDead(1);

        yield return new WaitSeconds(2.0f);
        
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        moveStickEnabled(false);
        skillButtonEnabled(false);

        setMessage("サブタワーを破壊すると\r\nシールドが消えて\r\nメインタワーに近づけるようになるんだ");
        
        yield return new WaitNext();
        
        setMessage("さあ、次はメインタワーを破壊してみよう");

        yield return new WaitNext();
        
        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
        moveStickEnabled(true);
        skillButtonEnabled(true);

        yield return new WaitMainTowerDead(1);
        
        yield return new WaitSeconds(2.0f);
        
        openNextWindow();
		setGuide(GUIMessageWindow.GuideMode.Guide3D_UIBG);
        moveStickEnabled(false);
        skillButtonEnabled(false);

        setMessage("こんなにアッサリとクリアするなんて\r\n流石だね！");

        yield return new WaitNext();
        
        setMessage("メインタワーを破壊すると\r\nバトルは終了するんだ");
        
        yield return new WaitNext();
        
        closeWindow();
        setGuide(GUIMessageWindow.GuideMode.None);
        moveStickEnabled(true);
        skillButtonEnabled(true);
		resetMessageWindow();

		//yield return new WaitTween(GUIMessageWindow.BGPlayTween);

		gmCommand("@result");

        yield break;
    }
    
    #endregion

    #region Private Methods
    
    private void startEnemyMainTowerCamera()
    {
        stopCamera();
        startCamera(enemyMainTowers[0].gameObject.transform.position, 10.0f, -15.0f, 45.0f, -45.0f, 3.0f);
    }
    
    private void startFriendMainTowerCamera()
    {
        stopCamera();
        startCamera(friendMainTowers[0].gameObject.transform.position, 10.0f, 15.0f, 45.0f, -45.0f, 3.0f);
    }
    
    private void startEnemySubTowerCamera()
    {
        stopCamera();
        startCamera(enemySubTowers[0].gameObject.transform.position, 5.0f, -15.0f, 45.0f, -45.0f, 3.0f);
    }
    
    private void startFriendSubTowerCamera()
    {
        stopCamera();
        startCamera(friendSubTowers[0].gameObject.transform.position, 5.0f, 15.0f, 45.0f, -45.0f, 3.0f);
    }
    
    private void startTankCamera()
    {
        stopCamera();
        float sign = (enemyMainTowers[0].gameObject.transform.position.z - tanks[0].gameObject.transform.position.z) >= 0.0f ? -1.0f : 1.0f;
        startCamera(tanks[0].gameObject.transform.position, 3.0f, 10.0f * sign, 45.0f, -45.0f, 3.0f);
    }
    
    private void startCamera(Vector3 target, float targetHeight, float distance, float startAngle, float stopAngle, float time)
    {
        cameraPosition = new GameObject("TutorialCameraPosition");
        cameraTarget = new GameObject("TutorialCameraTarget");
        cameraTarget.transform.position = new Vector3(target.x, target.y + targetHeight, target.z);
        
        TutorialCamera.StartDirectCamera(cameraPosition, cameraTarget, distance, startAngle, stopAngle,time);
    }
    
    private void stopCamera()
    {
        TutorialCamera.StopDirectCamera();
        
        if (cameraPosition != null)
            Object.Destroy(cameraPosition);
        if (cameraTarget != null)
            Object.Destroy(cameraTarget);
    }
    
    private void skillButtonEnabled(bool enabled)
    {
        GUISkill.SetActive(enabled);
    }
    
    #endregion
}
