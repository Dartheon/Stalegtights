using Godot;

public partial class MenuUI : Control
{
    #region Variables
    private Control pauseMenu;
    private Control startMenu;
    private Control mainMenu;

    private GameManager gameManager;
    private CharacterBody2D playerCB2D;
    #endregion

    #region Methods
    #region Start
    public override void _Ready()
    {
        gameManager = GetNode<GameManager>("/root/GameManager");

        pauseMenu = GetNode<Control>("%PauseMenu");
        startMenu = GetNode<Control>("%StartMenu");
        mainMenu = GetNode<Control>("%MainMenu");

        //Setting Initial Visibility
        pauseMenu.Visible = false;
        startMenu.Visible = true;
        mainMenu.Visible = false;
    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        //
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

    public void OnCloseMenuPressed()
    {
        //Add code here...
        //Have some way to close all menus
        //MainMenu
        //PauseMenu
        GD.Print("Pressed 'CloseMenu' Button");
    }
    #endregion

    #region Start Menu Button Signals
    public void OnNewGamePressed()
    {
        GD.Print("Pressed 'NewGame' Button");

        gameManager.CallDeferred("GameLoadScenes", "PlayerTestScene");

        startMenu.Visible = false;

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