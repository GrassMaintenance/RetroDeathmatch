using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private AudioClip menuSelectClip;

	public void SettingsMenu(Menu menu)
	{
		MenuManager.Instance.OpenMenu(menu);
	}

	public void QuitGame()
	{
		Timer.SetTimer(menuSelectClip.length, () => Application.Quit());
	}
}
