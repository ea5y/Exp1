/// <summary>
/// 元々あったEntrantEvent, EntrantResの入れ物に存在管理機能を持たせたもの.
/// 
/// 2014/11/06
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using Scm.Common.Packet;
using Scm.Common.GameParameter;

public abstract class EntrantInfo
{
	#region 定義.
	public enum CreateState
	{
		Uncreated,
		Created,
		Removed,
		Error,
	}
	#endregion

	#region フィールド.
	public int InFieldId { get; private set; }
	public string UserName { get; set; }
#if XW_DEBUG || ENABLE_GM_COMMAND
    public int Id { get; set; }
#else
	public int Id { get; private set; }
#endif
	public Vector3 StartPosition { get; private set; }
	public float StartRotation { get; private set; }
	public StatusType StatusType { get; private set; }
	public EffectType EffectType { get; private set; }
	public int HitPoint { get; private set; }
	public int MaxHitPoint { get; private set; }
	public AreaType AreaType { get; private set; }
	public int FieldId { get; private set; }
	public EntrantType EntrantType { get; private set; }
	public TeamType TeamType { get; private set; }
	public int TacticalId { get; private set; }
	public int Level { get; set; }
	public bool ResetCoolTime { get; private set; }
	public bool IsMoved { get; private set; }
	public short InFieldParentId { get; private set; }
	public int Win { get; private set; }
	public int Lose { get; private set; }
    public int SkinId { get; private set; }
    public int StarId { get; private set; }
    public int Gold { get; private set; }
    public bool NeedCalcInertia { get; private set; }

    public CreateState CreateStatus { get; private set; }

	public ObjectBase GameObject { get; private set; }

	// ObjectBaseが存在しているか(GameObjectとして生成する必要があるか).
	public bool IsInArea
	{
		get
		{
			return this.StatusType != StatusType.NotInArea && this.StatusType != StatusType.Transport;
		}
	}
	#endregion

	#region staticメソッド.
	/// <summary>
	/// playerのEntrantInfoを作成する.
	/// </summary>
	static public PlayerInfo CreatePlayer(EntrantRes packet, bool resetCoolTime = true)
	{
		var info = new PlayerInfo();
		info.SetInfo(packet, false, resetCoolTime);

		if(info.EntrantType != EntrantType.Pc)
		{
			BugReportController.SaveLogFile("CreatePlayerError! EntrantType = "+info.EntrantType);
			info.Remove(true);
		}

		return info;
	}
	/// <summary>
	/// EntrantTypeによって該当するEntrantInfoの派生クラスを作成する.
	/// Player(自分)の場合はCreatePlayerを使うこと.
	/// </summary>
	static public EntrantInfo Create(EntrantRes packet, bool isRespawn)
	{
		var info = CreateByType(packet.EntrantType);
		info.SetInfo(packet, isRespawn, true);
		return info;
	}
	/// <summary>
	/// EntrantTypeによって該当するEntrantInfoの派生クラスを作成する.
	/// Player(自分)の場合はCreatePlayerを使うこと.
	/// </summary>
	static public EntrantInfo Create(EntrantEvent packet)
	{
		var info = CreateByType(packet.GetEntrantRes().EntrantType);
		info.SetInfo(packet, true);
		return info;
	}
	static private EntrantInfo CreateByType(EntrantType entrantType)
	{
		switch (entrantType)
		{
		case EntrantType.Pc:
			return new PersonInfo();
		case EntrantType.Item:
			return new ItemDropInfo();
		case EntrantType.Npc:
		case EntrantType.MiniNpc:
		case EntrantType.Mob:
        case EntrantType.Hostage:
			return new NpcInfo();
		default:
			return new GadgetInfo();
		}
	}
	/// <summary>
	/// EntrantInfoが存在しない場合の代用品を作成.
	/// 基本的にエラー用なので呼び出し元でBugreportを記録することを推奨.
	/// </summary>
	static public EntrantInfo CreateError()
	{
		var info = new GadgetInfo();
		info.CreateStatus = CreateState.Error;

		return info;
	}
	#endregion

	#region 初期化.
	protected EntrantInfo() { }
	protected void SetInfo(EntrantEvent packet, bool isRespawn)
	{
        var res = packet.GetEntrantRes();

		this.InFieldId = res.InFieldId;
		this.UserName = res.UserName;
		this.Id = res.Id;
		this.StartPosition = new Vector3(res.Position[0], res.Position[1], res.Position[2]);
		this.StartRotation = res.Rotation;
		this.StatusType = res.StatusType;
		this.EffectType = res.EffectType;
		this.HitPoint = res.HitPoint;
		this.MaxHitPoint = res.MaxHitPoint;
		this.AreaType = res.AreaType;
		this.FieldId = res.AreaId;
		this.EntrantType = res.EntrantType;
		this.TeamType = res.TeamType;
		this.TacticalId = res.TacticalId;
		this.Level = res.BattleLevel;
		this.InFieldParentId = res.InFieldParentId.GetValueOrDefault();
		this.IsMoved = !isRespawn;
		this.Win = res.Win;
		this.Lose = res.Lose;
        this.SkinId = res.SkinId;
        this.StarId = res.StarId;
        this.NeedCalcInertia = res.NeedCalcInertia;
	}
	protected void SetInfo(EntrantRes packet, bool isRespawn, bool resetCoolTime)
	{
		this.InFieldId = packet.InFieldId;
		this.UserName = packet.UserName;
		this.Id = packet.Id;
		this.StartPosition = new Vector3(packet.Position[0], packet.Position[1], packet.Position[2]);
		this.StartRotation = packet.Rotation;
		this.StatusType = packet.StatusType;
		this.EffectType = packet.EffectType;
		this.HitPoint = packet.HitPoint;
		this.MaxHitPoint = packet.MaxHitPoint;
		this.AreaType = packet.AreaType;
		this.FieldId = packet.AreaId;
		this.EntrantType = packet.EntrantType;
		this.TeamType = packet.TeamType;
		this.TacticalId = packet.TacticalId;
		this.Level = packet.BattleLevel;
		this.InFieldParentId = packet.InFieldParentId.GetValueOrDefault();
		this.ResetCoolTime = resetCoolTime;
		this.IsMoved = !isRespawn;
		this.Win = packet.Win;
		this.Lose = packet.Lose;
        this.SkinId = packet.SkinId;
        this.StarId = packet.StarId;
        this.Gold = packet.Gold;
        this.NeedCalcInertia = packet.NeedCalcInertia;
	}
	#endregion

	#region 実体化＆無効化
	abstract public void CreateObject();
	public void Created(ObjectBase objectBase)
	{
		this.GameObject = objectBase;
		this.CreateStatus = CreateState.Created;
	}
	public void Remove(bool isDestroy)
	{
		this.CreateStatus = CreateState.Removed;
		if(isDestroy && this.GameObject != null)
		{
			this.GameObject.Remove();
		}
	}
	#endregion

	#region 通信処理メソッド.
	// 順番に全て処理するパケット.
	private Action<ObjectBase> unsettledPacket;
	// 最後の一つだけ処理するパケット群.
	private Action<ObjectBase> unsettledHitPacket;
	private Action<ObjectBase> unsettledMovePacket;

	public void Hit(HitInfo hitInfo)
	{
		if(this.GameObject != null)
		{
			this.GameObject.Hit(hitInfo);
		}
		else
		{
			unsettledHitPacket = (ObjectBase objectBase) =>
			{
				objectBase.Hit(hitInfo);
			};
		}
	}
	public void Move(Vector3 oldPosition, Vector3 position, Quaternion rotation, bool forceGrounded)
	{
		if(this.GameObject != null)
		{
			this.GameObject.Move(oldPosition, position, rotation, forceGrounded);
		}
		else
		{
			unsettledMovePacket = (ObjectBase objectBase) =>
			{
				objectBase.Move(oldPosition, position, rotation, forceGrounded);
			};
		}
	}

	public void RunUnsettledPacket(ObjectBase objectBase)
	{
		if(unsettledPacket != null)
		{
			unsettledPacket(objectBase);
			unsettledPacket = null;
		}

		if(unsettledHitPacket != null)
		{
			unsettledHitPacket(objectBase);
			unsettledHitPacket = null;
		}

		if(unsettledMovePacket != null)
		{
			unsettledMovePacket(objectBase);
			unsettledMovePacket = null;
		}
	}
	#endregion
}
