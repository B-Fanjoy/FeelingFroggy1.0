using TMPro;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public TextMeshProUGUI levelTitle;

    private void Start()
    {
        levelTitle.text = LevelManager.CurrentLevel.levelTitle;
    }

    public void Show()
    {
        pauseMenu.SetActive(true);
    }

    public void Hide()
    {
        pauseMenu.SetActive(false);
    }

    public void ClickResume()
    {
        GameController.Instance.UnpauseGame();
    }

    public void ClickSettings()
    {
        // TODO: Implement settings menu
    }

    public void ClickMainMenu()
    {
        // TODO: Show a confirmation dialog

        LevelManager.LoadMainMenu();
    }
}
