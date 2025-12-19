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

    //Class References
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
            //set input to false to prevent multi-inputs from happening
            inputManager.PlayerContinuousInputs["main_menu"] = false;

            //if menu is not visible make it visible, otherwise close menu
            if (!mainMenu.Visible)
            {
                gameManager.PauseManager(true); //pause game
                mainMenu.Visible = true; //open menu
            }
            else
            {
                //If Controller: Pause -> Main -> Main = Close Both Menus
                //mainMenu.Visible = false; pauseMenu.Visible = false;

                //If Keyboard:   Pause -> Main -> Main = Close Main Menu
                mainMenu.Visible = false; // close menu

                //unpause the game if there are no menus open
                if (!mainMenu.Visible && !pauseMenu.Visible && !StartMenu.Visible)
                {
                    gameManager.PauseManager(false); //pause game
                }
            }
        }

        //Pause Menu Logic
        if (inputManager.PlayerContinuousInputs["pause_menu"] && !StartMenu.Visible)
        {
            //set input to false to prevent multi-inputs from happening
            inputManager.PlayerContinuousInputs["pause_menu"] = false;

            //from game to pause, no main
            if (!pauseMenu.Visible && !mainMenu.Visible)
            {
                gameManager.PauseManager(true); //pause game
                pauseMenu.Visible = true; //open menu
            }
            //from pause to game, no main
            else if (pauseMenu.Visible && !mainMenu.Visible)
            {
                pauseMenu.Visible = false; //close menu
                gameManager.PauseManager(false); //unpause game
            }
            //from main no pause, then pause
            else if (!pauseMenu.Visible && mainMenu.Visible)
            {
                gameManager.PauseManager(true); //pause game
                pauseMenu.Visible = true; //open pause menu
                mainMenu.Visible = false; //close main menu
            }
            //from pause to main, then pause again in main
            else
            {
                gameManager.PauseManager(true); //pause game
                mainMenu.Visible = false; //close main menu
            }
        }
    }

    #endregion

    #region General Buttons Signals
    public void OnQuitToDesktopPressed()
    {
        //StartMenu
        //MainMenu
        GetTree().Quit(); //command to close the game
    }

    public void OnOptionsPressed()
    {
        //StartMenu
        //MainMenu
        optionsMenu.Visible = true; //open options menu
    }

    public void OnCloseMenuPressed(string menuType)
    {
        //Have some way to close individual menus
        //MainMenu
        //PauseMenu

        //takes the signal argument to decide which menu to close depending on where the signal came from
        switch (menuType)
        {
            case "MainMenu":
                if (mainMenu.Visible)
                {
                    mainMenu.Visible = false; //close main menu
                }
                break;
            case "PauseMenu":
                if (pauseMenu.Visible)
                {
                    pauseMenu.Visible = false; //close pause menu
                }
                break;
            default:
                GD.PushWarning("Unknown MenuType sent in signal for Close Menu");
                return;
        }

        //if there are no menues open, unpause the game
        if (!mainMenu.Visible && !pauseMenu.Visible && !StartMenu.Visible)
        {
            gameManager.PauseManager(false); //unpause game
        }
    }

    public void OnMenuBackButton(string menuName)
    {
        //take in the argument from the signal to find which back button was pressed in order to close the right menu
        switch (menuName)
        {
            case "LoadGame":
                //code for hitting back in the load game menu back to the start menu
                loadGame.Visible = false;
                break;
            case "Options":
                //code for hitting back in the options menu back to the start menu
                optionsMenu.Visible = false;
                break;
            case "Inventory":
                //code for hitting back in the Inventory menu back to the start menu
                inventoryMenu.Visible = false;
                break;
            case "Map":
                //code for hitting back in the map menu back to the start menu
                mapMenu.Visible = false;
                break;
            case "Codex":
                //code for hitting back in the codex menu back to the start menu
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
        gameManager.CallDeferred("GameLoadScenes", "PlayerTestScene"); //loads the PlayerTestScene scene into the game world

        StartMenu.Visible = false; //close start menu

        gameManager.PauseManager(false); //unpause game
    }

    public void OnLoadGamePressed()
    {
        loadGame.Visible = true; //open load game menu
    }
    #endregion

    #region Main Menu Button Signals
    public void OnQuitToStartMenuPressed()
    {
        gameManager.PauseManager(true); //pause game

        //resets the menus so the start menu is open and the others are not
        StartMenu.Visible = true;
        pauseMenu.Visible = false;
        mainMenu.Visible = false;
    }

    public void OnMainMenuToPauseMenu()
    {
        pauseMenu.Visible = true;
        mainMenu.Visible = false;
    }
    #endregion

    #region Pause Menu Button Signals
    public void OnInventoryPressed()
    {
        inventoryMenu.Visible = true; //open inventory menu
    }

    public void OnMapPressed()
    {
        mapMenu.Visible = true; //open map menu
    }

    public void OnCodexPressed()
    {
        codexMenu.Visible = true; //open codex menu
    }

    public void OnPauseMenuToMainMenu()
    {
        mainMenu.Visible = true;
        pauseMenu.Visible = false;
    }
    #endregion
    #endregion
}