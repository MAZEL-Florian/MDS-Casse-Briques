using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting new game, unloading MainMenu and loading Level1.");

        // Décharge MainMenu seulement
        SceneManager.UnloadSceneAsync("MainMenu");

        // Charge Level1 additivement
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
    }




    public void QuitGame()
    {
        Application.Quit();
    }
}
