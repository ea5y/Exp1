/// <summary>
/// チームコメントアイテム
/// 
/// 2014/12/05
/// </summary>
using UnityEngine;
using System.Collections;
using Scm.Common.Packet;

public class GUITeamCommentItem : MonoBehaviour
{
	#region フィールド＆プロパティ
	public byte CommentId { get; private set; }
	public string Comment { get; private set; }
	public UILabel uiLabel;

	#endregion

	public void SetComment(byte commentId)
	{
		this.CommentId = commentId;
		this.Comment = ObsolateSrc.TeamComment.GetTeamComment(this.CommentId);
		if(this.uiLabel != null) { this.uiLabel.text = this.Comment; }
	}

	#region NGUIリフレクション
	public void OnClick()
	{
#if OLD_TEAM_LOGIC
        GUITeamMenu.SetComment(this);
#endif
	}
	#endregion
}
