using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerKeyPressListener : MonoBehaviour
{
    public KeyCode keyCode = KeyCode.Escape;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
        }
    }
}
