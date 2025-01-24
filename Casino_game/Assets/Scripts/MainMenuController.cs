using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public Dictionary<string, GameObject> canvasDictionary = new();

	private Stack<GameObject> menuHistoryStack = new();
	private Games chosenGame = Games.Szwindel;

	public GameObject[] canvasList;
	public CanvasGroup[] gamesList;
	public CanvasGroup OptionsPanel;
	public CanvasGroup ChoosePokerPanel;
	public TextMeshProUGUI Score;

    private void Start()
    {
		StartCoroutine(getScore(1));
        foreach (GameObject canvas in canvasList)
        {
            canvasDictionary.Add(canvas.name, canvas);
            canvas.SetActive(false);
        }

        if (!string.IsNullOrEmpty(SceneData.CanvasToShow) && canvasDictionary.ContainsKey(SceneData.CanvasToShow))
        {
            menuHistoryStack.Push(canvasDictionary[SceneData.CanvasToShow]);
        }
        else
        {
            menuHistoryStack.Push(canvasDictionary["MainMenuCanvas"]);
        }

        menuHistoryStack.Peek().SetActive(true);

        SceneData.CanvasToShow = null;
    }

    public void PushOnStackAndShow(GameObject canvas)
	{
		menuHistoryStack.Peek().SetActive(false);
		menuHistoryStack.Push(canvas);
		menuHistoryStack.Peek().SetActive(true);
	}

	public void PopFromStackAndShow()
	{
		menuHistoryStack.Peek().SetActive(false);
		menuHistoryStack.Pop();
		menuHistoryStack.Peek().SetActive(true);
	}

	public void SwitchGame(bool prev = false)
	{
		foreach (CanvasGroup canvas in gamesList)
		{
			canvas.alpha = 0;
			canvas.blocksRaycasts = false;
		}

		if (!prev) chosenGame = (Games)(((int)chosenGame + 1) % 3);
		else chosenGame = (Games)((((int)chosenGame - 1) + 3)%3);

		switch (chosenGame)
		{
			case Games.Texas:
				gamesList[0].alpha = 1;
				gamesList[0].blocksRaycasts = true;
				break;
			case Games.BlackJack:
				gamesList[1].alpha = 1;
				gamesList[1].blocksRaycasts = true;
				break;
			case Games.Szwindel:
				gamesList[2].alpha = 1;
				gamesList[2].blocksRaycasts = true;
				break;
			default: break;
		}
	}

	public async void LoadSingleGame ()
	{
		switch (chosenGame)
		{
			case Games.Texas:
				await SceneManager.LoadSceneAsync("TexasScene");
				break;
			default:
				break;
		}
	}

	public void PlayGame()
	{
		PushOnStackAndShow(canvasDictionary["ChooseGameCanvas"]);
	}

	public void Option()
	{
		PushOnStackAndShow(canvasDictionary["OptionsCanvas"]);
	}

	public void Settings()
	{
        PushOnStackAndShow(canvasDictionary["MainMenuCanvas"]);
    }

	public void Back()
	{
		PopFromStackAndShow();
	}

	public void Exit()
	{
		Application.Quit();
	}

    private IEnumerator getScore(int id)
    {
        string url = "http://localhost/szwindel/getScore.php?id=" + id.ToString();

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {

            string responseText = www.downloadHandler.text;
            Debug.Log("OdpowiedŸ serwera: " + responseText);

            int score;
            if (int.TryParse(responseText, out score))
            {
				Score.text = score.ToString();
            }
            else
            {
                Debug.LogError("B³¹d przy próbie parsowania wyniku.");
            }
        }
        else
        {
            Debug.LogError("B³¹d po³¹czenia: " + www.error);
        }
    }
}
