using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wreckless.UI;
using System.Linq;

public class PauseMenu : MonoBehaviour
{

    public static PauseMenu Instance;

    public bool IsPaused { get; private set; }

    public GameObject pauseMenu;
    public GameObject helpMenu;
    public Transform buttonPanel;

    private enum MenuType { None, Main, Help};
    private MenuType type;

    private List<Button> buttons = new List<Button>();

    private int selectionIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UnpauseGame();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            if (!IsPaused)
                PauseGame();
            else if (IsPaused)
                UnpauseGame();

        if (!IsPaused)
            return;

        if (type == MenuType.Main)
        {
            if (Input.GetButtonDown("Menu Up"))
                selectionIndex--;
            else if (Input.GetButtonDown("Menu Down"))
                selectionIndex++;

            if (selectionIndex < 0)
                selectionIndex = buttons.Count - 1;
            else if (selectionIndex == buttons.Count)
                selectionIndex = 0;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].isSelected = false;
                if (i == selectionIndex)
                    buttons[i].isSelected = true;
            }

            if (Input.GetKeyDown(KeyCode.Return))
                buttons[selectionIndex].Select();
        }
        else if(type == MenuType.Help)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                type = MenuType.Main;
                UpdateMenu();
            }
        }

    }

    private void UpdateMenu()
    {

        switch (type)
        {
            case MenuType.None:
                pauseMenu.SetActive(false);
                helpMenu.SetActive(false);
                break;
            case MenuType.Main:
                pauseMenu.SetActive(true);
                helpMenu.SetActive(false);
                break;
            case MenuType.Help:
                pauseMenu.SetActive(false);
                helpMenu.SetActive(true);
                break;
        }

    }

    public void Resume()
    {
        IsPaused = false;
        type = MenuType.None;
        UpdateMenu();
    }

    public void Help()
    {
        type = MenuType.Help;
        UpdateMenu();
    }

    public void Quit()
    {

        if (Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();

    }

    private void PauseGame()
    {
        IsPaused = true;
        buttons = buttonPanel.GetComponentsInChildren<Button>().ToList();
        selectionIndex = 0;
        type = MenuType.Main;
        UpdateMenu();
    }

    private void UnpauseGame()
    {
        Resume();
    }

}
