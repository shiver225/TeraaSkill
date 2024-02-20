using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void MainMenu_ButtonClick()
    {
        SceneManager.LoadScene(0);
    }
    public void Play_ButtonClick()
    {
        SceneManager.LoadScene(1);
    }
    public void Options_ButtonClick()
    {
        SceneManager.LoadScene(2);
    }
    public void QuitGame_ButtonClick()
    {
        Application.Quit();
    }
}
