using System;
using System.Security.Cryptography;
using Godot;

public partial class MenuUI : Control
{
    #region Variables
    private Control pauseMenu;
    public Control StartMenu { get; set; }
    private Control mainMenu;

    private GameManager gameManager;
    private InputManager inputManager;
    private CharacterBody2D playerCB2D;
    #endregion

    #region Methods
    #region Start
    public override void _Ready()
    {
        gameManager = GetNode<GameManager>("/root/GameManager");
        inputManager = GetNode<InputManager>("/root/InputManager");

        pauseMenu = GetNode<Control>("%PauseMenu");
        StartMenu = GetNode<Control>("%StartMenu");
        mainMenu = GetNode<Control>("%MainMenu");

        //Setting Initial Visibility
        pauseMenu.Visible = false;
        StartMenu.Visible = true;
        mainMenu.Visible = false;
    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        //Main Menu Logic
        if (inputManager.PlayerContinuousInputs["main_menu"])
        {
            inputManager.PlayerContinuousInputs["main_menu"] = false;

            //if menu is not visible make it visible, otherwise close menu
            if (!mainMenu.Visible)
            {
                gameManager.PauseManager(true);
                mainMenu.Visible = true;
            }
            else
            {
                //If Controller: Pause -> Main -> Main = Close Both Menus
                //mainMenu.Visible = false; pauseMenu.Visible = false;

                //If Keyboard:   Pause -> Main -> Main = Close Main Menu
                mainMenu.Visible = false;

                if (!mainMenu.Visible && !pauseMenu.Visible && !StartMenu.Visible)
                {
                    gameManager.PauseManager(false);
                }
            }
        }

        //Pause Menu Logic
        if (inputManager.PlayerContinuousInputs["pause_menu"] && !StartMenu.Visible)
        {
            inputManager.PlayerContinuousInputs["pause_menu"] = false;

            //from game to pause, no main
            if (!pauseMenu.Visible && !mainMenu.Visible)
            {
                gameManager.PauseManager(true);
                pauseMenu.Visible = true;
            }
            //from pause to game, no main
            else if (pauseMenu.Visible && !mainMenu.Visible)
            {
                pauseMenu.Visible = false;
                gameManager.PauseManager(false);
            }
            //from main no pause, then pause
            else if (!pauseMenu.Visible && mainMenu.Visible)
            {
                gameManager.PauseManager(true);
                pauseMenu.Visible = true;
                mainMenu.Visible = false;
            }
            //from pause to main, then pause again in main
            else
            {
                gameManager.PauseManager(true);
                mainMenu.Visible = false;
            }
        }
    }

    #endregion

    #region General Buttons Signals
    public void OnQuitToDesktopPressed()
    {
        //Add code here...
        //StartMenu
        //MainMenu
        GD.Print("Pressed 'QuitToDesktop' Button");
        GetTree().Quit();
    }

    public void OnOptionsPressed()
    {
        //Add code here...
        //StartMenu
        //MainMenu
        GD.Print("Pressed 'Options' Button");
    }

    public void OnCloseMenuPressed(int menuType)
    {
        //Add code here...
        //Have some way to close all menus
        //MainMenu = 0
        //PauseMenu = 1
        GD.Print("Pressed 'CloseMenu' Button");

        switch (menuType)
        {
            case 0:
                if (mainMenu.Visible)
                {
                    mainMenu.Visible = false;
                }
                break;
            case 1:
                if (pauseMenu.Visible)
                {
                    pauseMenu.Visible = false;
                }
                break;
            default:
                GD.PushWarning("Unknown MenuType sent in signal for Close Menu");
                return;
        }

        if (!mainMenu.Visible && !pauseMenu.Visible)
        {
            gameManager.PauseManager(false);
        }
    }
    #endregion

    #region Start Menu Button Signals
    public void OnNewGamePressed()
    {
        GD.Print("Pressed 'NewGame' Button");

        gameManager.CallDeferred("GameLoadScenes", "PlayerTestScene");

        StartMenu.Visible = false;

        gameManager.PauseManager(false);
    }

    public void OnLoadGamePressed()
    {
        //Add code here...
        GD.Print("Pressed 'LoadGame' Button");
    }
    #endregion

    #region Main Menu Button Signals
    public void OnQuitToStartMenuPressed()
    {
        //Add code here...
        GD.Print("Pressed 'QuitToStartMenu' Button");

        StartMenu.Visible = true;
        pauseMenu.Visible = false;
        mainMenu.Visible = false;

        gameManager.PauseManager(true);
    }
    #endregion

    #region Pause Menu Button Signals
    public void OnInventoryPressed()
    {
        //Add code here...
        GD.Print("Pressed 'Inventory' Button");
    }

    public void OnMapPressed()
    {
        //Add code here...
        GD.Print("Pressed 'Map' Button");
    }

    public void OnCodexPressed()
    {
        //Add code here...
        GD.Print("Pressed 'Codex' Button");
    }
    #endregion
    #endregion
}