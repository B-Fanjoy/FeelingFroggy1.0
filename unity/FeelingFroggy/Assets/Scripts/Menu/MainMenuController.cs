using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject optionsMenu;
    public GameObject controlsMenu;

    private GameObject _prevMenu;

    public Transform levelSlides;

    private void Start()
    {
        var levelNum = 1;

        foreach (Transform levelSlide in levelSlides)
        {
            // Get level slide button
            var levelButton = levelSlide.GetComponent<Button>();

            // Subscribe button click to load the level
            var currentLevelNum = levelNum;
            levelButton.onClick.AddListener(() => LoadLevel(currentLevelNum));

            // Increase the level num once each level button.
            levelNum++;
        }

        // Fix layout components
        foreach (var menu in new[] { mainMenu, playMenu, optionsMenu, controlsMenu })
        {
            menu.SetActive(true);
            menu.SetActive(false);
        }

        // Set main menu active
        ShowMainMenu();
    }

    private void LoadLevel(int levelNum)
    {
        var level = LevelManager.Levels[levelNum - 1];

        LevelManager.LoadLevelScene(level);
    }

    private void ShowMenu(GameObject menu)
    {
        if (_prevMenu != null)
        {
            _prevMenu.SetActive(false);
        }

        menu.SetActive(true);
        _prevMenu = menu;
    }

    public void ShowPlayMenu() => ShowMenu(playMenu);

    public void ShowOptionsMenu() => ShowMenu(optionsMenu);

    public void ShowControlsMenu() => ShowMenu(controlsMenu);

    public void ShowMainMenu() => ShowMenu(mainMenu);

    public void QuitGame()
    {
        Application.Quit();
    }
}

