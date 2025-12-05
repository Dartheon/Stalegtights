using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node
{
    #region Variables
    #region Class Scripts
    private SaveLoadManager slManager;
    private SoundManager soundManager;
    private InputManager inputManager;
    private World world;
    private StateMachine stateMachineScript;
    private GroundState groundStateScript;
    #endregion

    #region General
    private float engineTimeScale = 0.1f;
    #endregion

    #region Scenes
    //Sets Which Scenes Should be Started at Gamestart - Can be Set In Editor For Testing
    [Export] private string[] startScenes;
    #endregion

    #region Interactables Currently Entered
    public static List<Area2D> InteractablesEntered { get; set; } = new();
    #endregion

    #region Teleport Nodes
    public Dictionary<string, Teleporter> TeleporterDictionary { get; set; } = new();
    public Dictionary<string, TeleporterAllType> TeleporterSelectDictionary { get; set; } = new();
    #endregion

    #region Sounds
    #region Sound Queues
    private Dictionary<string, SoundRequestSFX> sfxPlayer;
    private Dictionary<string, SoundRequestSFX> sfxEnvironment;
    private Dictionary<string, SoundRequestBGMMenu> bgmMenu;
    private Dictionary<string, SoundRequestBGMMenu> bgmAmbient;
    private Queue<string> soundQueue = new();
    #endregion
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        InitGrabNodes();

        GameStartScenes();

        //Sets the BGM to play at the game start
        //Currently off for testing//
        //SetBGM();
    }
    #endregion

    #region Initialize Nodes and Variables
    private void InitGrabNodes()
    {
        slManager = GetNode<SaveLoadManager>("/root/SaveLoadManager");
        soundManager = GetNode<SoundManager>("/root/SoundManager");
        inputManager = GetNode<InputManager>("/root/InputManager");
        world = GetNode<World>("/root/Main/World");
        stateMachineScript = GetNode<StateMachine>("/root/Main/World/Player/PLAYERSTATEMACHINE");
        groundStateScript = GetNode<GroundState>("/root/Main/World/Player/PLAYERSTATEMACHINE/GROUND STATE");

        //Assign to new variables to shorten code
        sfxPlayer = slManager.SFXPlayer;
        sfxEnvironment = slManager.SFXEnvironment;
        bgmMenu = slManager.BGMMenu;
        bgmAmbient = slManager.BGMAmbient;
    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        #region EngineScale 
        //Changing the EngineScale Faster
        if (inputManager.PlayerContinuousInputs["engine_scale_up"])
        {
            Engine.TimeScale += engineTimeScale;
            Engine.TimeScale = Mathf.Round(Engine.TimeScale * 10f) / 10f; // Round to one decimal place

            inputManager.PlayerContinuousInputs["engine_scale_up"] = false; //ensures only one statement is called for button input
        }

        //Changing the EngineScale Slower
        if (inputManager.PlayerContinuousInputs["engine_scale_down"])
        {
            Engine.TimeScale -= engineTimeScale;
            if (Engine.TimeScale <= 0)
            {
                Engine.TimeScale = engineTimeScale;
            }
            Engine.TimeScale = Mathf.Round(Engine.TimeScale * 10f) / 10f; // Round to one decimal place

            inputManager.PlayerContinuousInputs["engine_scale_down"] = false; //ensures only one statement is called for button input
        }

        //Changing the EngineScale back to normal
        if (inputManager.PlayerContinuousInputs["engine_scale_reset"])
        {
            Engine.TimeScale = 1.0;

            PickupAllType.SPEEDMODIFIER = 1.0f;
            PickupAllType.ACCELMODIFIER = 1.0f;
            PickupAllType.GRAVITYMODIFIER = 1.0f;
            PickupAllType.JUMPMODIFIER = 1.0f;

            groundStateScript.MoveSpeedModifier = PickupAllType.SPEEDMODIFIER;
            stateMachineScript.RunAccelerationModifier = PickupAllType.ACCELMODIFIER;
            stateMachineScript.GravityModifier = PickupAllType.GRAVITYMODIFIER;
            stateMachineScript.JumpModifier = PickupAllType.JUMPMODIFIER;

            inputManager.PlayerContinuousInputs["engine_scale_reset"] = false; //ensures only one statement is called for button input
        }
        #endregion
    }
    #endregion

    #region Scenes
    //finds what scenes to start the game loaded with
    private void GameStartScenes()
    {
        if (startScenes != null)
        {
            // Update scene states based on whether the scene is in the list to load
            foreach (string sceneName in slManager.ScenesLoaded.Keys.ToList())
            {
                //The line assigns true if the scene is in sceneNamesToLoad and false otherwise
                slManager.ScenesLoaded[sceneName].SceneState = startScenes.Contains(sceneName);
            }

            // If any scene was loaded, call the world loading method
            //This is a shorthand form of a lambda expression. It means: "For each state, return true if the state is true."
            if (slManager.ScenesLoaded.Values.Any(state => state.SceneState))
            {
                //CallDeferred is used here to avoid errors when Initializing new nodes rapidly
                world.CallDeferred("LoadSceneWorld");
            }
        }
    }

    //Call this method and the name off the scene to load it into the game world
    public void ScenesManager(string sceneNamesToLoad)
    {
        // Update scene states based on whether the scene is in the list to load
        foreach (string sceneName in slManager.ScenesLoaded.Keys.ToList())
        {
            //The line assigns true if the scene is in sceneNamesToLoad and false otherwise
            slManager.ScenesLoaded[sceneName].SceneState = sceneNamesToLoad.Contains(sceneName);
        }

        // If any scene was loaded, call the world loading method
        //This is a shorthand form of a lambda expression. It means: "For each state, return true if the state is true."
        if (slManager.ScenesLoaded.Values.Any(state => state.SceneState))
        {
            //CallDeferred is used here to avoid errors when Initializing new nodes rapidly
            world.CallDeferred("LoadSceneWorld");
        }
    }

    public void ScenesManager(string[] sceneNamesToLoad)
    {
        // Update scene states based on whether the scene is in the list to load
        foreach (string sceneName in slManager.ScenesLoaded.Keys.ToList())
        {
            //The line assigns true if the scene is in sceneNamesToLoad and false otherwise
            slManager.ScenesLoaded[sceneName].SceneState = sceneNamesToLoad.Contains(sceneName);
        }

        // If any scene was loaded, call the world loading method
        //This is a shorthand form of a lambda expression. It means: "For each state, return true if the state is true."
        if (slManager.ScenesLoaded.Values.Any(state => state.SceneState))
        {
            //CallDeferred is used here to avoid errors when Initializing new nodes rapidly
            world.CallDeferred("LoadSceneWorld");
        }
    }
    #endregion

    #region Sounds
    private void SetBGM()
    {
        string startSound;
        soundManager.PlayBGMMenu(bgmAmbient.GetValueOrDefault("TitleBGM").Source, bgmAmbient.GetValueOrDefault("TitleBGM").SoundName);
        if (bgmAmbient.TryGetValue(startSound = "TitleBGM", out var soundRequest))
        {
            soundManager.PlayBGMMenu(soundRequest.Source, soundRequest.SoundName);
        }
        else
        {
            GD.PushWarning($"Sound key '{startSound}' not found in bgmAmbient.");
        }
    }

    private void QueueEnvironmentSFX(string soundKey)
    {
        if (sfxEnvironment.TryGetValue(soundKey, out SoundRequestSFX soundData))
        {
            soundQueue.Enqueue(soundData.SoundName);
            SendEnvironmentSFXRequest(); // Call after enqueueing a valid sound
        }
        else
        {
            GD.PushWarning($"QueueEnvironmentSFX Sound not found: {soundKey}");
        }
    }

    private void SendEnvironmentSFXRequest()
    {
        if (soundQueue.Count > 0)
        {
            string soundName = soundQueue.Dequeue();
            if (sfxEnvironment.TryGetValue(soundName, out SoundRequestSFX soundData))
            {
                soundManager.PlaySFX(soundData.Source, soundData.SoundName);
            }
        }
    }
    #endregion
    #endregion
}