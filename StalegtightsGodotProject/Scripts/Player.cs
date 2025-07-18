using Godot;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    #region Variables
    #region Class Scripts
    private SaveLoadManager slManager;
    private SoundManager soundManager;
    private GameManager gameManager;
    #endregion

    #region Sound Queue
    private Dictionary<string, SoundRequestSFX> sfxPlayer;
    private Queue<string> soundQueue = new();
    #endregion

    #region Position
    [Export] public Vector2 spawnPosition;
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        InitGrabNodes();
    }
    #endregion

    #region Initialize Nodes and Variables
    private void InitGrabNodes()
    {
        slManager = GetNode<SaveLoadManager>("/root/SaveLoadManager");
        soundManager = GetNode<SoundManager>("/root/SoundManager");
        gameManager = GetNode<GameManager>("/root/GameManager");

        Position = spawnPosition;

        //assign to new variables to shorten code
        sfxPlayer = slManager.SFXPlayer;
    }
    #endregion

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
    #endregion
}