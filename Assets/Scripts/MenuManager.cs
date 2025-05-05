using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting new game, unloading MainMenu and loading Level1.");

        // Unload MainMenu uniquement (il est additif au-dessus de GlobalScene)
        SceneManager.UnloadSceneAsync("MainMenu");

        // Charger Level1 additivement
        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);

        // Réinitialiser complètement le GameManager
        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.StartNewGame();
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
