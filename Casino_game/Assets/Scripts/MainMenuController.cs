using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public CanvasGroup OptionsPanel;
	public CanvasGroup ChoosePokerPanel;

	public void PlayGame()
	{
		ChoosePokerPanel.alpha = 1;
		ChoosePokerPanel.blocksRaycasts = true;
	}

	public void Option()
	{
		OptionsPanel.alpha = 1;
		OptionsPanel.blocksRaycasts = true;
	}

	public void Back()
	{
		OptionsPanel.alpha = 0;
		OptionsPanel.blocksRaycasts = false;
	}

	public void Exit()
	{
		Application.Quit();
	}
}
