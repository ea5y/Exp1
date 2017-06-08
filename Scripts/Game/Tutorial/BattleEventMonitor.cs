/// <summary>
/// バトル中のイベントモニタ
/// 
/// 2014/06/30
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.GameParameter;

public class BattleEventMonitor : Singleton<BattleEventMonitor>
{
    #region Fields & Properties

    public bool IsCapturing { get; private set; }
    
    public int EnemyMainTowerDeadCount { get; private set; }
    public int FriendMainTowerDeadCount { get; private set; }
    public int EnemySubTowerDeadCount { get; private set; }
    public int FriendSubTowerDeadCount { get; private set; }
    public int TankDeadCount { get; private set; }

    private List<MainTower> mainTowers;
    private List<SubTower> subTowers;
    private List<TankBase> tanks;

    #endregion

    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();

        mainTowers = new List<MainTower>();
        subTowers = new List<SubTower>();
        tanks = new List<TankBase>();
    }

    IEnumerator Start()
    {
        while (ObjectManager.Instance.Count <= 0)
        {
            yield return null;
        }

        mainTowers.AddRange(Entrant.FindAll<MainTower>( t => true ));
        subTowers.AddRange(Entrant.FindAll<SubTower>( t => true ));
        tanks.AddRange(Entrant.FindAll<TankBase>( t => true ));

		Reset();
    }
    
    void LateUpdate()
    {
        foreach (var mt in mainTowers)
        {
            if (mt.StatusType == StatusType.Dead)
            {
                TeamTypeClient ttc = StaticClassScm.GetClientTeam(mt.TeamType);
                switch (ttc)
                {
                    case TeamTypeClient.Enemy:
                        EnemyMainTowerDeadCount++;
                        break;

                    case TeamTypeClient.Friend:
                        FriendMainTowerDeadCount++;
                        break;

                    case TeamTypeClient.Unknown:
                        break;
                }
            }
        }
        mainTowers.RemoveAll(mt => mt.StatusType == StatusType.Dead);

        foreach (var st in subTowers)
        {
            if (st.StatusType == StatusType.Dead)
            {
                TeamTypeClient ttc = StaticClassScm.GetClientTeam(st.TeamType);
                switch (ttc)
                {
                    case TeamTypeClient.Enemy:
                        EnemySubTowerDeadCount++;
                        break;

                    case TeamTypeClient.Friend:
                        FriendSubTowerDeadCount++;
                        break;

                    case TeamTypeClient.Unknown:
                        break;
                }
            }
        }
        subTowers.RemoveAll(st => st.StatusType == StatusType.Dead);

        foreach (var tb in tanks)
        {
            if (tb.StatusType == StatusType.Dead)
            {
                TankDeadCount++;
            }
        }
        tanks.RemoveAll(tb => tb.StatusType == StatusType.Dead);
    }

    #endregion

    #region Public Methods
    
    public void StartCapture()
    {
        Reset();
        IsCapturing = true;
    }
    
    public void StopCapture()
    {
        IsCapturing = false;
    }
    
    public void Reset()
    {
        EnemyMainTowerDeadCount = 0;
        FriendMainTowerDeadCount = 0;
        EnemySubTowerDeadCount = 0;
        FriendSubTowerDeadCount = 0;
        TankDeadCount = 0;
    }
    
    #endregion
}

/// <summary>
/// ファイバー用 敵メインタワーが指定個数破壊されるまで待つ
/// </summary>
public class WaitMainTowerDead : IFiberWait
{
    private int count;

    public WaitMainTowerDead(int count)
    {
        this.count = count;

        BattleEventMonitor.Instance.StartCapture();
    }

    #region IFiberWait

    public bool IsWait
    {
        get
        {
            if (BattleEventMonitor.Instance.EnemyMainTowerDeadCount >= this.count)
            {
                BattleEventMonitor.Instance.StopCapture();
                return false;
            }

            return true;
        }
    }

    #endregion
}

/// <summary>
/// ファイバー用 敵サブタワーが指定個数破壊されるまで待つ
/// </summary>
public class WaitSubTowerDead : IFiberWait
{
    private int count;
    
    public WaitSubTowerDead(int count)
    {
        this.count = count;
        
        BattleEventMonitor.Instance.StartCapture();
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            if (BattleEventMonitor.Instance.EnemySubTowerDeadCount >= this.count)
            {
                BattleEventMonitor.Instance.StopCapture();
                return false;
            }
            
            return true;
        }
    }

    #endregion
}

/// <summary>
/// ファイバー用 タンクが指定個数破壊されるまで待つ
/// </summary>
public class WaitTankDead : IFiberWait
{
    private int count;
    
    public WaitTankDead(int count)
    {
        this.count = count;
        
        BattleEventMonitor.Instance.StartCapture();
    }
    
    #region IFiberWait
    
    public bool IsWait
    {
        get
        {
            if (BattleEventMonitor.Instance.TankDeadCount >= this.count)
            {
                BattleEventMonitor.Instance.StopCapture();
                return false;
            }
            
            return true;
        }
    }
    
    #endregion
}
