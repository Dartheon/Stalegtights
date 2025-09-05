using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DebugUI : Control
{
    #region Variables
    //Variables will be uncommented when needed
    #region Class Scripts
    private Player playerScript;
    private SaveLoadManager slManager;
    private SoundManager soundManager;
    private StateMachine stateMachine;
    private GroundState groundStateScript;
    #endregion

    #region ColorRect Boxes
    private Vector2 generalBoxSize;
    private Vector2 animationBoxSize;
    /*private Vector2 groundBoxSize;
    private Vector2 airBoxSize;
    private Vector2 wallBoxSize;
    private Vector2 climbingBoxSize;
    private Vector2 soundBoxSize;*/
    private Vector2 pickupBoxSize;
    private Vector2 engineBoxSize;
    #endregion

    #region General
    private Label playerStatelabelGeneral;
    private Label playerVelocityGeneral;
    /*private Label playerJumpTypeGeneral;
    private Label playerJumpBufferGeneral;*/
    #endregion

    #region Animation
    //VBox1
    private Label playerSpriteFrame;
    private Label playerCurrentAnim;
    public AnimationNodeStateMachinePlayback statemachinePlaybackState;
    public AnimationNodeStateMachinePlayback statemachinePlaybackGround;
    public AnimationNodeStateMachinePlayback statemachinePlaybackAir;
    public AnimationNodeStateMachinePlayback statemachinePlaybackWall;
    public AnimationNodeStateMachinePlayback statemachinePlaybackClimb;
    private Label playerAnimPlaybackState;
    private Label playerAnimPlaybackGround;
    private Label playerAnimPlaybackAir;
    private Label playerAnimPlaybackWall;
    private Label playerAnimPlaybackClimb;

    //VBox2
    public AnimationNodeStateMachinePlayback statemachinePlaybackGroundNormal;
    public AnimationNodeStateMachinePlayback statemachinePlaybackGroundWeapon;
    public AnimationNodeStateMachinePlayback statemachinePlaybackGroundStalag;
    private Label playerAnimPlaybackGroundNormal;
    private Label playerAnimPlaybackGroundWeapon;
    private Label playerAnimPlaybackGroundStalag;

    //VBox3
    public AnimationNodeStateMachinePlayback statemachinePlaybackAirNormal;
    public AnimationNodeStateMachinePlayback statemachinePlaybackAirWeapon;
    public AnimationNodeStateMachinePlayback statemachinePlaybackAirStalag;
    private Label playerAnimPlaybackAirNormal;
    private Label playerAnimPlaybackAirWeapon;
    private Label playerAnimPlaybackAirStalag;

    //VBox4
    public AnimationNodeStateMachinePlayback statemachinePlaybackWallNormal;
    public AnimationNodeStateMachinePlayback statemachinePlaybackWallWeapon;
    public AnimationNodeStateMachinePlayback statemachinePlaybackWallStalag;
    private Label playerAnimPlaybackWallNormal;
    private Label playerAnimPlaybackWallWeapon;
    private Label playerAnimPlaybackWallStalag;

    //VBox5
    public AnimationNodeStateMachinePlayback statemachinePlaybackClimbNormal;
    public AnimationNodeStateMachinePlayback statemachinePlaybackClimbWeapon;
    public AnimationNodeStateMachinePlayback statemachinePlaybackClimbStalag;
    private Label playerAnimPlaybackClimbNormal;
    private Label playerAnimPlaybackClimbWeapon;
    private Label playerAnimPlaybackClimbStalag;
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
    private System.Collections.Generic.Dictionary<string, SoundRequestBGMMenu> bgmMenu;
    /*private Label playerSoundName;
    private Label playerBus;
    private Label playerVolume;
    private Label playerDistance;*/
    #endregion

    #region Pickup Modifiers
    private Label gravity;
    private Label playerSpeed;
    private Label acceleration;
    private Label jumpHeight;
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
        stateMachine = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");

        bgmMenu = slManager.BGMMenu;

        //Animation Playback
        //VBox1
        statemachinePlaybackState = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/playback");
        statemachinePlaybackGround = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/GROUND STATE/playback");
        statemachinePlaybackAir = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/AIR STATE/playback");
        statemachinePlaybackWall = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/WALL STATE/playback");
        statemachinePlaybackClimb = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/CLIMB STATE/playback");
        //VBox2
        statemachinePlaybackGroundNormal = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/GROUND STATE/GROUND NORMAL/playback"); statemachinePlaybackGroundWeapon = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/GROUND STATE/GROUND WEAPON/playback"); statemachinePlaybackGroundStalag = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/GROUND STATE/GROUND STALAG/playback");
        //VBox3
        statemachinePlaybackAirNormal = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/AIR STATE/AIR NORMAL/playback"); statemachinePlaybackAirWeapon = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/AIR STATE/AIR WEAPON/playback"); statemachinePlaybackAirStalag = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/AIR STATE/AIR STALAG/playback");
        //VBox4
        //Animation Tree not currently populated with Nodes, Uncomment when populated
        /*statemachinePlaybackWallNormal = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/WALL STATE/WALL NORMAL/playback"); statemachinePlaybackWallWeapon = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/WALL STATE/WALL WEAPON/playback"); statemachinePlaybackWallStalag = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/WALL STATE/WALL STALAG/playback");
        //VBox5
        statemachinePlaybackClimbNormal = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/CLIMB STATE/CLIMB NORMAL/playback"); statemachinePlaybackClimbWeapon = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/CLIMB STATE/CLIMB WEAPON/playback"); statemachinePlaybackClimbStalag = (AnimationNodeStateMachinePlayback)stateMachine.PlayerAnimTree.Get("parameters/PlayerStateMachine/CLIMB STATE/CLIMB STALAG/playback");*/

        //Color Rect Boxes
        generalBoxSize = GetNode<ColorRect>("General/GeneralColorRect").Size;
        animationBoxSize = GetNode<ColorRect>("Animation/AnimationColorRect").Size;
        /*groundBoxSize = GetNode<ColorRect>("Ground/GroundColorRect").Size;
        airBoxSize = GetNode<ColorRect>("Air/AirColorRect").Size;
        wallBoxSize = GetNode<ColorRect>("Wall/WallColorRect").Size;
        climbingBoxSize = GetNode<ColorRect>("Climbing/ClimbingColorRect").Size;
        soundBoxSize = GetNode<ColorRect>("Sounds/SoundsColorRect").Size;*/
        pickupBoxSize = GetNode<ColorRect>("Pickup/PickupColorRect").Size;
        engineBoxSize = GetNode<ColorRect>("Engine/EngineColorRect").Size;

        //General Labels
        playerStatelabelGeneral = GetNode<Label>("General/GeneralVBoxContainer/PlayerStateLabel");
        playerVelocityGeneral = GetNode<Label>("General/GeneralVBoxContainer/Velocity");
        /*playerJumpTypeAll = GetNode<Label>("General/GeneralVBoxContainer/JumpType");
        playerJumpBuffer = GetNode<Label>("General/GeneralVBoxContainer/JumpBuffer");*/

        //Animation Labels
        //VBox1
        playerSpriteFrame = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerSpriteFrame");
        playerCurrentAnim = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerCurrentAnim");
        playerAnimPlaybackState = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerAnimPlaybackState");
        playerAnimPlaybackGround = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerAnimPlaybackGround");
        playerAnimPlaybackAir = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerAnimPlaybackAir");
        playerAnimPlaybackWall = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerAnimPlaybackWall");
        playerAnimPlaybackClimb = GetNode<Label>("Animation/AnimationVBoxContainer/PlayerAnimPlaybackClimb");
        //VBox2
        playerAnimPlaybackGroundNormal = GetNode<Label>("Animation/AnimationVBoxContainer2/PlayerAnimPlaybackGroundNormal");
        playerAnimPlaybackGroundWeapon = GetNode<Label>("Animation/AnimationVBoxContainer2/PlayerAnimPlaybackGroundWeapon");
        playerAnimPlaybackGroundStalag = GetNode<Label>("Animation/AnimationVBoxContainer2/PlayerAnimPlaybackGroundStalag");
        //VBox3
        playerAnimPlaybackAirNormal = GetNode<Label>("Animation/AnimationVBoxContainer3/PlayerAnimPlaybackAirNormal");
        playerAnimPlaybackAirWeapon = GetNode<Label>("Animation/AnimationVBoxContainer3/PlayerAnimPlaybackAirWeapon");
        playerAnimPlaybackAirStalag = GetNode<Label>("Animation/AnimationVBoxContainer3/PlayerAnimPlaybackAirStalag");
        //VBox4
        playerAnimPlaybackWallNormal = GetNode<Label>("Animation/AnimationVBoxContainer4/PlayerAnimPlaybackWallNormal");
        playerAnimPlaybackWallWeapon = GetNode<Label>("Animation/AnimationVBoxContainer4/PlayerAnimPlaybackWallWeapon");
        playerAnimPlaybackWallStalag = GetNode<Label>("Animation/AnimationVBoxContainer4/PlayerAnimPlaybackWallStalag");
        //VBox5
        playerAnimPlaybackClimbNormal = GetNode<Label>("Animation/AnimationVBoxContainer5/PlayerAnimPlaybackClimbNormal");
        playerAnimPlaybackClimbWeapon = GetNode<Label>("Animation/AnimationVBoxContainer5/PlayerAnimPlaybackClimbWeapon");
        playerAnimPlaybackClimbStalag = GetNode<Label>("Animation/AnimationVBoxContainer5/PlayerAnimPlaybackClimbStalag");

        //Ground State Labels

        //Air State Labels
        //playerJumpHeight = GetNode<Label>("AirVBoxContainer/JumpHeight");

        //Wall State Labels


        //Climbing State Labels
        /*playerFirstArea = GetNode<Label>("ClimbingVBoxContainer/PlayerFirstArea");
        playerSecondArea = GetNode<Label>("ClimbingVBoxContainer/PlayerSecondArea");
        playerThirdArea = GetNode<Label>("ClimbingVBoxContainer/PlayerThirdArea");*/

        //Sounds Labels
        /*playerSoundName = GetNode<Label>("SoundsVBoxContainer/SoundName");
        playerBus = GetNode<Label>("SoundsVBoxContainer/SoundBus");
        playerVolume = GetNode<Label>("SoundsVBoxContainer/SoundVolume");
        playerDistance = GetNode<Label>("SoundsVBoxContainer/SoundDistance");*/

        //Pickup Labels
        gravity = GetNode<Label>("Pickup/PickupVBoxContainer/Gravity");
        playerSpeed = GetNode<Label>("Pickup/PickupVBoxContainer/PlayerSpeed");
        acceleration = GetNode<Label>("Pickup/PickupVBoxContainer/Acceleration");
        jumpHeight = GetNode<Label>("Pickup/PickupVBoxContainer/JumpHeight");

        //Engine Labels
        engineScale = GetNode<Label>("Engine/EngineVBoxContainer/EngineScale");
        fpsCounter = GetNode<Label>("Engine/EngineVBoxContainer/FPSCounter");
        maxFPS = GetNode<Label>("Engine/EngineVBoxContainer/MaxFPS");
    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        //Color Rect Boxes
        GetNode<ColorRect>("General/GeneralColorRect").Size = generalBoxSize;
        GetNode<ColorRect>("Animation/AnimationColorRect").Size = animationBoxSize;
        /*GetNode<ColorRect>("GroundColorRect").Size = groundBoxSize;
        GetNode<ColorRect>("AirColorRect").Size = airBoxSize;
        GetNode<ColorRect>("WallColorRect").Size = wallBoxSize;
        GetNode<ColorRect>("ClimbingColorRect").Size = climbingBoxSize;
        GetNode<ColorRect>("SoundsColorRect").Size = soundBoxSize;*/
        GetNode<ColorRect>("Pickup/PickupColorRect").Size = pickupBoxSize;
        GetNode<ColorRect>("Engine/EngineColorRect").Size = engineBoxSize;

        //General Text
        playerStatelabelGeneral.Text = $"State: {stateMachine.CurrentState.Name}";
        playerVelocityGeneral.Text = $"Velocity.X: {stateMachine.smPlayerVelocity.X:0.00}\nVelocity.Y: {stateMachine.smPlayerVelocity.Y:0.00}";
        /*playerJumpTypeAll.Text = ("Jump Type: ") + stateMachine.JumpType + "\n" + ("JTypeHold: ") + stateMachine.JumpTypeHold;
        playerJumpBuffer.Text = ("JBuffer: ") + stateMachine.JumpInputBuffer;*/

        //Animation Text
        //VBox1
        playerSpriteFrame.Text = $"Frame: {GetNode<Sprite2D>("/root/Main/World/Player/PlayerSprite").Frame}";
        playerCurrentAnim.Text = $"CurrentAnim: {stateMachine.CurrentAnimationPlaying}";
        playerAnimPlaybackState.Text = $"AnimState: {statemachinePlaybackState.GetCurrentNode()}";
        playerAnimPlaybackGround.Text = $"GroundState: {statemachinePlaybackGround.GetCurrentNode()}";
        playerAnimPlaybackAir.Text = $"AirState: {statemachinePlaybackAir.GetCurrentNode()}";
        playerAnimPlaybackWall.Text = $"WallState: {statemachinePlaybackWall.GetCurrentNode()}";
        playerAnimPlaybackClimb.Text = $"ClimbState: {statemachinePlaybackClimb.GetCurrentNode()}";
        //VBox2
        playerAnimPlaybackGroundNormal.Text = $"GroundNormal: {statemachinePlaybackGroundNormal.GetCurrentNode()}";
        playerAnimPlaybackGroundWeapon.Text = $"GroundWeapon: {statemachinePlaybackGroundWeapon.GetCurrentNode()}";
        playerAnimPlaybackGroundStalag.Text = $"GroundStalag: {statemachinePlaybackGroundStalag.GetCurrentNode()}";
        //VBox3
        playerAnimPlaybackAirNormal.Text = $"AirNormal: {statemachinePlaybackAirNormal.GetCurrentNode()}";
        playerAnimPlaybackAirWeapon.Text = $"AirWeapon: {statemachinePlaybackAirWeapon.GetCurrentNode()}";
        playerAnimPlaybackAirStalag.Text = $"AirStalag: {statemachinePlaybackAirStalag.GetCurrentNode()}";
        //VBox4
        //Animation Tree not populated, Need to populate Tree before uncommenting
        /*playerAnimPlaybackWallNormal.Text = $"WallNormal: {statemachinePlaybackWallNormal.GetCurrentNode()}";
        playerAnimPlaybackWallWeapon.Text = $"WallWeapon: {statemachinePlaybackWallWeapon.GetCurrentNode()}";
        playerAnimPlaybackWallStalag.Text = $"WallStalag: {statemachinePlaybackWallStalag.GetCurrentNode()}";
        //VBox5
        playerAnimPlaybackClimbNormal.Text = $"ClimbNormal: {statemachinePlaybackClimbNormal.GetCurrentNode()}";
        playerAnimPlaybackClimbWeapon.Text = $"ClimbWeapon: {statemachinePlaybackClimbWeapon.GetCurrentNode()}";
        playerAnimPlaybackClimbStalag.Text = $"ClimbStalag: {statemachinePlaybackClimbStalag.GetCurrentNode()}";*/

        //Ground State Text


        //Air State Text
        //playerJumpHeight.Text = ("Jump Height: " + stateMachine.WallJumpHeight);

        //Wall State Text


        //Climbing State Text
        /*playerFirstArea.Text = ("FirstArea: " + stateMachine.ClimableFirstAreaEntered);
        playerSecondArea.Text = ("SecondArea: " + stateMachine.ClimbableSecondTopEntered);
        playerThirdArea.Text = ("ThirdArea: " + stateMachine.ClimbableThirdTopEntered);*/

        //Sounds Text
        /*if (soundManager.debugPlayer != null)
        {
            playerSoundName.Text = ("Name: " + soundManager.playerToRequestSFX[soundManager.debugPlayer.Name].SoundName);
            playerBus.Text = ("Bus: " + soundManager.debugPlayer.Bus);
            playerVolume.Text = ("Volume: " + soundManager.debugPlayer.VolumeDb);
            playerDistance.Text = "Position: " + soundManager.Call("GetObjectPosition", soundManager.nextRequestSFX);
        }*/

        //Pickup Text
        //Engine Text
        gravity.Text = $"Gravity: {stateMachine.smGravity}";
        playerSpeed.Text = $"Speed: {groundStateScript.GroundMoveSpeed}";
        acceleration.Text = $"Accel: {stateMachine.RunAcceleration}";
        jumpHeight.Text = $"Jump: {stateMachine.smPlayerJumpVelocity}";

        //Engine Text
        engineScale.Text = $"Engine Scale: {Engine.TimeScale}";
        fpsCounter.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        maxFPS.Text = $"Max FPS: {Engine.MaxFps}";
    }
    #endregion

    #region Toggle Buttons
    private void GeneralCheckBoxToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("General/GeneralVBoxContainer").Visible = false;
            generalBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("General/GeneralVBoxContainer").Visible = true;
            generalBoxSize.Y = 225.0f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }

    private void AnimationCheckBoxToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("Animation/AnimationVBoxContainer").Visible = false;
            GetNode<VBoxContainer>("Animation/AnimationVBoxContainer2").Visible = false;
            GetNode<VBoxContainer>("Animation/AnimationVBoxContainer3").Visible = false;
            animationBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("Animation/AnimationVBoxContainer").Visible = true;
            GetNode<VBoxContainer>("Animation/AnimationVBoxContainer2").Visible = true;
            GetNode<VBoxContainer>("Animation/AnimationVBoxContainer3").Visible = true;
            animationBoxSize.Y = 300.0f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }

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

    private void PickupCheckBoxToggled(bool toggle)
    {
        if (!toggle)
        {
            GetNode<VBoxContainer>("Pickup/PickupVBoxContainer").Visible = false;
            pickupBoxSize.Y = 40f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
        else
        {
            GetNode<VBoxContainer>("Pickup/PickupVBoxContainer").Visible = true;
            pickupBoxSize.Y = 180.0f;
            soundManager.PlayBGMMenu(bgmMenu.GetValueOrDefault("MenuClick").Source, bgmMenu.GetValueOrDefault("MenuClick").SoundName);
        }
    }

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
}