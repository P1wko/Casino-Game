using UnityEngine;
using UnityEngine.SceneManagement;

public class TexasHudController : MonoBehaviour
{
    public async void Back()
    {
        SceneData.CanvasToShow = "ChooseGameCanvas";
        await SceneManager.LoadSceneAsync("MainMenuScene");
    }
}

public static class SceneData
{
    public static string CanvasToShow = null;
}