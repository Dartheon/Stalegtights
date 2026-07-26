using Godot;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    #region Variables
    #region Class Scripts
    private SaveLoadManager slManager;
    private SoundManager soundManager;
    private GameManager gameManager;
    private InputManager inputManager;
    private StateMachine stateMachineScript;
    #endregion

    #region Sound Queue
    private Dictionary<string, SoundRequestSFX> sfxPlayer;
    private Queue<string> soundQueue = new();
    #endregion

    #region Position
    [Export] private Vector2 spawnPosition;
    #endregion

    #region General
    //Climbing
    public float LadderPosX { get; set; } = 0.0f;
    public bool PlayerOnLadder { get; set; } = false;
    private float previousLadderPosX = 0.1f;
    public static HashSet<CollisionShape2D> PlayerAboveLadder { get; private set; } = new();

    private RayCast2D stepUpRightHigh;
    private RayCast2D stepUpRightLow;
    private RayCast2D stepUpLeftHigh;
    private RayCast2D stepUpLeftLow;

    private float stepHeight = 32f;
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        slManager = GetNode<SaveLoadManager>("/root/SaveLoadManager");
        soundManager = GetNode<SoundManager>("/root/SoundManager");
        gameManager = GetNode<GameManager>("/root/GameManager");
        inputManager = GetNode<InputManager>("/root/InputManager");
        stateMachineScript = GetNode<StateMachine>("PLAYERSTATEMACHINE");

        Position = spawnPosition;

        //assign to new variables to shorten code
        sfxPlayer = slManager.SFXPlayer;

        //Raycasts for step up
        stepUpRightHigh = GetNode<RayCast2D>("StepUpRightCastHigh");
        stepUpRightLow = GetNode<RayCast2D>("StepUpRightCastLow");
        stepUpLeftHigh = GetNode<RayCast2D>("StepUpLeftCastHigh");
        stepUpLeftLow = GetNode<RayCast2D>("StepUpLeftCastLow");
    }
    #endregion

    public override void _PhysicsProcess(double delta)
    {
        if (inputManager.PlayerContinuousInputs["interact"])
        {
            InteractActivate();
            inputManager.PlayerContinuousInputs["interact"] = false;
        }

        if (stepUpRightLow.IsColliding() && !stepUpRightHigh.IsColliding() && inputManager.RightIntent)
        {
            if (stateMachineScript.PlayerState == "GROUND STATE")
            {
                // Step upright
                GlobalPosition += Vector2.Up * stepHeight;
            }
        }
        else if (stepUpLeftLow.IsColliding() && !stepUpLeftHigh.IsColliding() && inputManager.LeftIntent)
        {
            if (stateMachineScript.PlayerState == "GROUND STATE")
            {
                // Step upleft
                GlobalPosition += Vector2.Up * stepHeight;
            }
        }
    }

    #region Sound Methods
    /* Example for Calling Sounds from Key Press
    if (Input.IsActionJustPressed("shoot"))
		{
            playerScript.QueuePlayerSFX("PlayerShoot"); //Used When calling Player Sounds from Outside the PlayerScript, Likely Used
			QueuePlayerSFX("PlayerShoot"); //If Called in the Player Script, Unlikely
		}
    */
    private void QueuePlayerSFX(string soundKey)
    {
        if (sfxPlayer.TryGetValue(soundKey, out SoundRequestSFX soundData))
        {
            soundData.ObjectNode = GetNode<Node2D>(GetPath());//Sets the Sounds Node to the Player for Getting Position Later
            soundQueue.Enqueue(soundData.SoundName); //Queues the Sound to Send to the SoundManager
            SendPlayerSFXRequest(); // Call after enqueuing a valid sound
        }
        else
        {
            GD.PushWarning($"QueuePlayerSFX Sound not found: {soundKey}");
        }
    }

    private void SendPlayerSFXRequest()
    {
        if (soundQueue.Count > 0)
        {
            string soundName = soundQueue.Dequeue(); //Grabs the sound from the Queue

            if (sfxPlayer.TryGetValue(soundName, out SoundRequestSFX soundData))
            {
                soundManager.PlaySFX(soundData.Source, soundData.SoundName); //Sends the Sound to the SoundManager for Playing
            }
        }
    }
    #endregion

    #region Player Position
    public Vector2 GetPlayerPosition()
    {
        //returns the Player's global location in the game world
        return GlobalPosition;
    }
    #endregion

    #region LadderDetection
    public void IsOnLadder()
    {
        if (previousLadderPosX != LadderPosX)
        {
            if (GameManager.LaddersEntered[GameManager.LaddersEntered.Count - 1] != null)
            {
                //Gets the current ladder X position
                LadderPosX = (float)GameManager.LaddersEntered[GameManager.LaddersEntered.Count - 1].Call("GetLadderGlobalPositionX");
                previousLadderPosX = LadderPosX;
            }
        }
    }

    public void SetLadderPosition(float ladderX)
    {
        LadderPosX = ladderX;
        previousLadderPosX = ladderX;
    }
    #endregion

    #region Interactables
    public void InteractActivate()
    {
        for (int i = 0; i < GameManager.InteractablesEntered.Count; i++)
        {
            GameManager.InteractablesEntered[i].CallDeferred("PlayerInteract"); //Checks for null area then calls the Interact Method from each Interactable
        }
    }
    #endregion

    #region Player Signals
    //Interactable Area2D Entered
    public void OnInteractBodyEntered(Area2D area)
    {
        if (area != null && area.IsInGroup("Interactable"))
        {
            if (GameManager.InteractablesEntered.Contains(area)) { return; }

            GameManager.InteractablesEntered?.Add(area);
        }

        if (area != null && area.IsInGroup("Ladder"))
        {
            if (GameManager.LaddersEntered.Contains(area)) { return; }

            GameManager.LaddersEntered?.Add(area);

            if (GameManager.LaddersEntered?.Count > 0)
            {
                PlayerOnLadder = true;
            }
            IsOnLadder();
        }
    }

    //Interactable Area2D Exit
    public void OnInteractBodyExited(Area2D area)
    {
        if (area != null && area.IsInGroup("Interactable"))
        {
            if (GameManager.InteractablesEntered.Contains(area))
            {
                GameManager.InteractablesEntered?.Remove(area);
            }
        }

        if (area != null && area.IsInGroup("Ladder"))
        {
            if (GameManager.LaddersEntered.Contains(area))
            {
                GameManager.LaddersEntered?.Remove(area);
            }
            if (GameManager.LaddersEntered?.Count == 0)
            {
                PlayerOnLadder = false;
            }
            previousLadderPosX = 0.0f;
        }
    }
    #endregion
    #endregion
}