using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.UnloadSceneAsync("MainMenu");

        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
    }




    public void QuitGame()
    {
        Application.Quit();
    }
}
