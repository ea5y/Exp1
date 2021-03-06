using UnityEngine;

[AddComponentMenu("Designer/NGUI/GUISound")]
public class GUISound : MonoBehaviour
{
	[SerializeField]
	private SoundController.SeID seID;

	protected void OnClick()
	{
		if (this.enabled)
		{
			SoundController.PlaySe(seID);
		}
	}
}
