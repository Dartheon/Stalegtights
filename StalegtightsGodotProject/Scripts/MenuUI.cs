using System;
using System.Security.Cryptography;
using Godot;

public partial class MenuUI : Control
{
    #region Variables
    //Start Menu
    public Control StartMenu { get; set; }
    private Control loadGame;

    //Main Menu
    private Control mainMenu;

    //Pause Menu
    private Control pauseMenu;
    private Control inventoryMenu;
    private Control mapMenu;
    private Control codexMenu;

    //Options Menu
    private Control optionsMenu;

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

        //Start Menu
        StartMenu = GetNode<Control>("%StartMenu");
        loadGame = GetNode<Control>("%LoadGame");
        //Main Menu
        mainMenu = GetNode<Control>("%MainMenu");
        //Pause Menu
        pauseMenu = GetNode<Control>("%PauseMenu");
        inventoryMenu = GetNode<Control>("%InventoryMenu");
        mapMenu = GetNode<Control>("%MapMenu");
        codexMenu = GetNode<Control>("%CodexMenu");
        //Options Menu
        optionsMenu = GetNode<Control>("%OptionsMenu");

        //Setting Initial Visibility
        //Start Menu
        StartMenu.Visible = true;
        loadGame.Visible = false;
        //Main Menu
        mainMenu.Visible = false;
        //Pause Menu
        pauseMenu.Visible = false;
        inventoryMenu.Visible = false;
        mapMenu.Visible = false;
        codexMenu.Visible = false;
        //Options Menu
        optionsMenu.Visible = false;
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
        //StartMenu
        //MainMenu
        GD.Print("Pressed 'QuitToDesktop' Button");
        GetTree().Quit();
    }

    public void OnOptionsPressed()
    {
        //StartMenu
        //MainMenu
        GD.Print("Pressed 'Options' Button");
        optionsMenu.Visible = true;
    }

    public void OnCloseMenuPressed(string menuType)
    {
        //Have some way to close individual menus
        //MainMenu
        //PauseMenu
        GD.Print("Pressed 'CloseMenu' Button");

        switch (menuType)
        {
            case "MainMenu":
                if (mainMenu.Visible)
                {
                    mainMenu.Visible = false;
                }
                break;
            case "PauseMenu":
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

    public void OnMenuBackButton(string menuName)
    {
        switch (menuName)
        {
            case "LoadGame":
                loadGame.Visible = false;
                break;
            case "Options":
                //code for hitting back in the options menu back to the start menu
                optionsMenu.Visible = false;
                break;
            case "Inventory":
                //code for hitting back in the options menu back to the start menu
                inventoryMenu.Visible = false;
                break;
            case "Map":
                //code for hitting back in the options menu back to the start menu
                mapMenu.Visible = false;
                break;
            case "Codex":
                //code for hitting back in the options menu back to the start menu
                codexMenu.Visible = false;
                break;
            default:
                GD.PushWarning("Incorrect Back Button Name for MenuBackButton signal");
                return;
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
        GD.Print("Pressed 'LoadGame' Button");
        loadGame.Visible = true;
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
        GD.Print("Pressed 'Inventory' Button");
        inventoryMenu.Visible = true;
    }

    public void OnMapPressed()
    {
        GD.Print("Pressed 'Map' Button");
        mapMenu.Visible = true;
    }

    public void OnCodexPressed()
    {
        GD.Print("Pressed 'Codex' Button");
        codexMenu.Visible = true;
    }
    #endregion
    #endregion
}