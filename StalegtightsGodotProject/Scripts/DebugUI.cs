using Godot;
using System.Collections.Generic;

public partial class DebugUI : Control
{
    #region Variables
    //Variables will be uncommented when needed
    #region Class Scripts
    private Player playerScript;
    private SaveLoadManager slManager;
    private SoundManager soundManager;
    /*private StateMachine stateMachine;
    private GroundState groundStateScript;*/
    #endregion

    #region ColorRect Boxes
    /*private Vector2 groundBoxSize;
    private Vector2 airBoxSize;
    private Vector2 wallBoxSize;
    private Vector2 climbingBoxSize;
    private Vector2 soundBoxSize;*/
    private Vector2 engineBoxSize;
    #endregion

    #region All States
    /*private Label playerStatelabelAll;
    private Label playerVelocityAll;
    private Label playerJumpTypeAll;
    private Label playerJumpBuffer;*/
    #endregion

    #region Ground State
    #endregion

    #region Air State
    /*private Label playerJumpHeight;*/
    #endregion

    #region Wall State
    #endregion

    #region Climbing State
    #endregion

    #region Sounds
    private Dictionary<string, SoundRequestBGMMenu> bgmMenu;
    /*private Label playerSoundName;
    private Label playerBus;
    private Label playerVolume;
    private Label playerDistance;*/
    #endregion

    #region Engine
    private Label engineScale;
    private Label fpsCounter;
    private Label maxFPS;
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        playerScript = GetNode<Player>("/root/Main/World/Player");
        soundManager = GetNode<SoundManager>("/root/SoundManager");
        slManager = GetNode<SaveLoadManager>("/root/SaveLoadManager");
        //stateMachine = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");*/

        bgmMenu = slManager.BGMMenu;

        //Color Rect Boxes
        /*groundBoxSize = GetNode<ColorRect>("GroundColorRect").Size;
        airBoxSize = GetNode<ColorRect>("AirColorRect").Size;
        wallBoxSize = GetNode<ColorRect>("WallColorRect").Size;
        climbingBoxSize = GetNode<ColorRect>("ClimbingColorRect").Size;
        soundBoxSize = GetNode<ColorRect>("SoundsColorRect").Size;*/
        engineBoxSize = GetNode<ColorRect>("Engine/EngineColorRect").Size;

        //All State Tab
        /*playerStatelabelAll = GetNode<Label>("AllVBoxContainer/PlayerStateLabel");
        playerVelocityAll = GetNode<Label>("AllVBoxContainer/Velocity");
        playerJumpTypeAll = GetNode<Label>("AllVBoxContainer/JumpType");
        playerJumpBuffer = GetNode<Label>("AllVBoxContainer/JumpBuffer");*/

        //Ground State Tab

        //Air State Tab
        //playerJumpHeight = GetNode<Label>("AirVBoxContainer/JumpHeight");

        //Wall State Tab


        //Climbing State Tab
        /*playerFirstArea = GetNode<Label>("ClimbingVBoxContainer/PlayerFirstArea");
        playerSecondArea = GetNode<Label>("ClimbingVBoxContainer/PlayerSecondArea");
        playerThirdArea = GetNode<Label>("ClimbingVBoxContainer/PlayerThirdArea");*/

        //Sounds
        /*playerSoundName = GetNode<Label>("SoundsVBoxContainer/SoundName");
        playerBus = GetNode<Label>("SoundsVBoxContainer/SoundBus");
        playerVolume = GetNode<Label>("SoundsVBoxContainer/SoundVolume");
        playerDistance = GetNode<Label>("SoundsVBoxContainer/SoundDistance");*/

        //Engine
        engineScale = GetNode<Label>("Engine/EngineVBoxContainer/EngineScale");
        fpsCounter = GetNode<Label>("Engine/EngineVBoxContainer/FPSCounter");
        maxFPS = GetNode<Label>("Engine/EngineVBoxContainer/MaxFPS");
    }

    public override void _Process(double delta)
    {
        //Color Rect Boxes
        /*GetNode<ColorRect>("GroundColorRect").Size = groundBoxSize;
        GetNode<ColorRect>("AirColorRect").Size = airBoxSize;
        GetNode<ColorRect>("WallColorRect").Size = wallBoxSize;
        GetNode<ColorRect>("ClimbingColorRect").Size = climbingBoxSize;
        GetNode<ColorRect>("SoundsColorRect").Size = soundBoxSize;*/
        GetNode<ColorRect>("Engine/EngineColorRect").Size = engineBoxSize;

        //All State Tab
        /*playerStatelabelAll.Text = ("State: ") + stateMachine.CurrentState.Name;
        playerVelocityAll.Text = ("Velocity.X: ") + stateMachine.playerVelocity.X + ("\nVelocity.Y: ") + stateMachine.playerVelocity.Y;
        playerJumpTypeAll.Text = ("Jump Type: ") + stateMachine.JumpType + "\n" + ("JTypeHold: ") + stateMachine.JumpTypeHold;
        playerJumpBuffer.Text = ("JBuffer: ") + stateMachine.JumpInputBuffer;*/

        //Ground State Tab


        //Air State Tab
        //playerJumpHeight.Text = ("Jump Height: " + stateMachine.WallJumpHeight);

        //Wall State Tab


        //Climbing State Tab
        /*playerFirstArea.Text = ("FirstArea: " + stateMachine.ClimableFirstAreaEntered);
        playerSecondArea.Text = ("SecondArea: " + stateMachine.ClimbableSecondTopEntered);
        playerThirdArea.Text = ("ThirdArea: " + stateMachine.ClimbableThirdTopEntered);*/

        //Sounds
        /*if (soundManager.debugPlayer != null)
        {
            playerSoundName.Text = ("Name: " + soundManager.playerToRequestSFX[soundManager.debugPlayer.Name].SoundName);
            playerBus.Text = ("Bus: " + soundManager.debugPlayer.Bus);
            playerVolume.Text = ("Volume: " + soundManager.debugPlayer.VolumeDb);
            playerDistance.Text = "Position: " + soundManager.Call("GetObjectPosition", soundManager.nextRequestSFX);
        }*/

        //Engine
        engineScale.Text = ("Engine Scale: " + Engine.TimeScale);
        fpsCounter.Text = ("FPS: " + Engine.GetFramesPerSecond());
        maxFPS.Text = ("Max FPS: " + Engine.MaxFps);
    }

    #region Toggle Buttons
    /*private void GroundCheckButtonToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("GroundVBoxContainer").Visible = false;
            groundBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("GroundVBoxContainer").Visible = true;
            groundBoxSize.Y = 200f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }
    private void AirCheckButtonToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("AirVBoxContainer").Visible = false;
            airBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("AirVBoxContainer").Visible = true;
            airBoxSize.Y = 200f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }
    private void WallCheckButtonToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("WallVBoxContainer").Visible = false;
            wallBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("WallVBoxContainer").Visible = true;
            wallBoxSize.Y = 200f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }
    private void ClimbingCheckButtonToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("ClimbingVBoxContainer").Visible = false;
            climbingBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("ClimbingVBoxContainer").Visible = true;
            climbingBoxSize.Y = 200f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }
    private void SoundCheckButtonToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("SoundsVBoxContainer").Visible = false;
            soundBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("SoundsVBoxContainer").Visible = true;
            soundBoxSize.Y = 200f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }*/

    private void EngineCheckBoxToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("Engine/EngineVBoxContainer").Visible = false;
            engineBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("Engine/EngineVBoxContainer").Visible = true;
            engineBoxSize.Y = 155.0f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }
    #endregion
    #endregion
    #endregion
}