/// <summary>
/// スキルアイコン
/// 
/// 2015/02/23
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Scm.Common.Master;

public class SkillIcon
{
	#region フィールド&プロパティ
	/// <summary>
	/// アセットバンドル内のバンドル名
	/// </summary>
	private const string BundleName = "ui_ic_skill_001";

	/// <summary>
	/// 複数のアセットバンドルの管理
	/// </summary>
	private BundleDataManager BundleDataManager { get; set; }
	#endregion

	#region 初期化
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SkillIcon() { MemberInit(); }

	/// <summary>
	/// クリア
	/// </summary>
	public void Clear()
	{
		// リソースは参照切るだけで消える？
		this.MemberInit();
	}

	/// <summary>
	/// メンバー初期化
	/// </summary>
	private void MemberInit()
	{
		this.BundleDataManager = new BundleDataManager();
	}
	#endregion

	#region アイコン取得
	/// <summary>
	/// スキルアイコンを取得する
	/// </summary>
	public void GetSkillIcon(CharaButtonSetMasterData buttonSet, int lv, SkillButtonType buttonType, bool keepAssetReference, System.Action<UIAtlas, string> callback)
	{
		// スキルアイコンを取得する
		string iconName;
		if(!TryGetSkillIconName(buttonSet, lv, buttonType, out iconName))
		{
			Debug.LogWarning(string.Format("Failure SetIconName SkillButtonType={0}", buttonType));
			if (callback != null)
				callback(null, "");
			return;
		}

		// アトラス取得
		var bundleData = this.BundleDataManager.GetBundleData<StateIconBundleData>(BundleName);
		if (bundleData != null)
		{
			bundleData.GetStateIconResource(BundleName, true,
								(UIAtlas res) =>
								{
									if (callback != null)
										callback(res, iconName);
								});
		}
	}
	#endregion

	#region スキルアイコン情報
	/// <summary>
	/// スキルアイコン名を取得する
	/// </summary>
	private bool TryGetSkillIconName(CharaButtonSetMasterData buttonSet, int lv, SkillButtonType buttonType, out string iconName)
	{
		iconName = string.Empty;

		if(buttonSet != null)
		{
			// 取得したいスキルタイプにあったボタン名をセットする
			switch(buttonType)
			{
				case SkillButtonType.Normal:
					iconName = buttonSet.AttackButton.IconFile;
					break;
				case SkillButtonType.Skill1:
					iconName = buttonSet.Skill1Button.IconFile;
					break;
				case SkillButtonType.Skill2:
					iconName = buttonSet.Skill2Button.IconFile;
					break;
				case SkillButtonType.SpecialSkill:
					iconName = buttonSet.SpecialSkillButton.IconFile;
					break;
				case SkillButtonType.TechnicalSkill:
					iconName = buttonSet.TechnicalSkillButton.IconFile;
					break;
			}
			return true;
		}
		return false;
	}
	#endregion
}

/// <summary>
/// アセットバンドル内のスキルアイコンデータ
/// </summary>
public class SkillIconBundleData : BundleData
{
	/// <summary>
	/// アセットバンドル内のアイコンパス
	/// </summary>
	private const string SkillIconPath = "Atlas.prefab";
	
	/// <summary>
	/// スキルアイコン
	/// </summary>
	private UIAtlas iconAtlas = null;

	/// <summary>
	/// 全てのリソースを読み込んでいるかどうか
	/// </summary>
	protected override bool IsFinishAllResource()
	{
		if (this.iconAtlas == null)
			return false;
		return true;
	}
	
	/// <summary>
	/// スキルアイコン取得
	/// </summary>
	public void GetSkillIcon(string bundleName, bool keepAssetReference, System.Action<UIAtlas> callback)
	{
		if (this.iconAtlas == null)
		{
			this.GetAssetAsync<UIAtlas>(bundleName, SkillIconPath, keepAssetReference,
								(UIAtlas resource) =>
								{
									this.iconAtlas = resource;
									if (callback != null)
										callback(resource);
								});
		}
		else
		{
			if (callback != null)
				callback(this.iconAtlas);
		}
	}
}