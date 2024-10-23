using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	public Dictionary<string, GameObject> canvasDictionary = new Dictionary<string, GameObject>();

	private GameObject currentCanvas;
	private GameObject lastCanvas;
	private Games chosenGame = Games.Texas;

	public GameObject[] canvasList;
	public CanvasGroup OptionsPanel;
	public CanvasGroup ChoosePokerPanel;

	private void Start()
	{
		foreach (GameObject canvas in canvasList)
		{
			canvasDictionary.Add(canvas.name, canvas);
			canvas.SetActive(false);
		}

		ShowCanvas("MainMenuCanvas");
	}

	public void ShowCanvas(string canvasName)
	{
		if (canvasDictionary.ContainsKey(canvasName))
		{
			if (currentCanvas != null)
			{
				lastCanvas = currentCanvas;
				currentCanvas.SetActive(false);
			}

			currentCanvas = canvasDictionary[canvasName];
			currentCanvas.SetActive(true);
		}
		else
		{
			Debug.LogWarning("Canvas at name: " + canvasName + " not found");
		}
	}

	public void SwitchGame(bool prev = false)
	{
		if (!prev) chosenGame = (Games)(((int)chosenGame + 1) % 3);
		else chosenGame = (Games)(((int)chosenGame - 1) % 3);

		switch (chosenGame)
		{
			case Games.Texas:
				ShowCanvas("ChooseTexasCanvas");
				break;
			case Games.BlackJack:
				ShowCanvas("ChooseBlackjackCanvas");
				break;
			case Games.Szwindel:
				ShowCanvas("ChooseSzwindelCanvas");
				break;
			default: break;
		}
	}

	public void PlayGame()
	{
		ShowCanvas("ChooseTexasCanvas");
	}

	public void Option()
	{
		ShowCanvas("OptionsCanvas");
	}

	public void Back()
	{
		ShowCanvas(lastCanvas.name);
	}

	public void Exit()
	{
		Application.Quit();
	}
}
