using UnityEngine;
using UnityEngine.SceneManagement;

public class TexasHudController : MonoBehaviour
{
	public async void Back()
	{
		await SceneManager.LoadSceneAsync("MainMenuScene");
	}
}
