/// <summary>
/// FPSカウンター
/// 
/// 2014/05/23
/// </summary>
using UnityEngine;
using System.Collections;

public class ScmFpsCounter : FPSCounter
{
	#region フィールド＆プロパティ
	public int Count { get; set; }
	#endregion

	#region 仮想メソッド
	protected override void Update()
	{
		base.Update();
		// TODO:仮人数表示
		this.Count = 0;
		if (PersonManager.Instance != null)
			this.Count += PersonManager.Instance.Count;
		var player = GameController.GetPlayer();
		if (player != null)
			this.Count += 1;
	}
	protected override void SetText()
	{
		// UNDONE:公開前で精査している時間がないのでひとまず全部デバッグビルドでくくる
#if XW_DEBUG

#if XW_DEBUG
		if (DebugKeyCommand.Instance == null ? !ScmParam.Debug.File.IsDrawFPS : !DebugKeyCommand.DrawFPS)
		{
			this.Text = "";
			return;
		}
#endif
		base.SetText();

		var t = ApplicationController.ScreenInfo;
		if (t != null)
		{
			float w, h;
			t.GetAspectRatio(out w, out h);
			this.Text += string.Format(" {0:0.##}Inch({1:0.##}:{2:0.##})", t.Inch, w, h);
		}

		if (this.Count >= 0)
			this.Text += string.Format(" {0}人", this.Count);

#if XW_DEBUG
		if (!string.IsNullOrEmpty(GMCommand.InputString))
			this.Text += string.Format(" {0}", GMCommand.InputString);
#endif

#endif
	}
	#endregion
}
