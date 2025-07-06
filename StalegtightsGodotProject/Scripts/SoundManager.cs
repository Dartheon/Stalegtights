using Godot;
using System.Collections.Generic;
using System.Linq;

//Creating a Class to Hold Sound Request Data for the SFX
public partial class SoundRequestSFX : Node
{
    public string SoundName { get; private set; } //set when loaded
    public AudioStream Stream { get; private set; } //set when loaded
    public SoundSourceSFX Source { get; private set; } //set when loaded
    public bool CanPlayLate { get; private set; } //set when loaded
    public string SoundBus { get; private set; } //set when loaded
    public float DistanceToPlayer { get; set; } //set before priority system when next in queue to be played
    public double RequestAge { get; set; } //set when playing in audiostreamplayer2d
    public Node2D ObjectNode { get; set; }

    public enum SoundSourceSFX
    {
        Player = 3,
        Enemy = 2,
        Environment = 1,
        Default = 0
    }

    public SoundRequestSFX() { }
    public SoundRequestSFX(string soundName, AudioStream stream, SoundSourceSFX source, bool canPlayLate, string soundBus, float distanceToPlayer, double requestAge, Node2D objectNode)
    {
        SoundName = soundName;
        Stream = stream;
        Source = source;
        CanPlayLate = canPlayLate;
        SoundBus = soundBus;
        DistanceToPlayer = distanceToPlayer;
        RequestAge = requestAge;
        ObjectNode = objectNode;
    }
}

//Creating a Class to Hold SoundRequest Data for the BGM and Menu
public partial class SoundRequestBGMMenu : Node
{
    public string SoundName { get; private set; } //set when loaded
    public AudioStream Stream { get; private set; } //set when loaded
    public SoundSourceBGMMenu Source { get; private set; } //set when loaded
    public string SoundBus { get; private set; } //set when loaded

    public enum SoundSourceBGMMenu
    {
        BGM = 3,
        Ambient = 2,
        Menu = 1,
        Default = 0
    }

    public SoundRequestBGMMenu() { }
    public SoundRequestBGMMenu(string soundName, AudioStream stream, SoundSourceBGMMenu source, string soundBus)
    {
        SoundName = soundName;
        Stream = stream;
        Source = source;
        SoundBus = soundBus;
    }
}

public partial class SoundManager : Node
{
    #region Variables
    #region Class Scripts
    private SaveLoadManager slManager;
    private GameManager gameManager;
    private Player playerScript;
    private TimerClock nextRequestTimerClockSFX;
    private Timer expireTimerSFX;
    #endregion

    #region Dictionary Sound Library
    private Dictionary<string, SoundRequestSFX> sfxPlayer; //playersfx use AudioStreamPlayer2D
    private Dictionary<string, SoundRequestSFX> sfxEnemy; //enemysfx use AudioStreamPlayer2D
    private Dictionary<string, SoundRequestSFX> sfxEnvironment; //environmentsfx use AudioStreamPlayer2D
    private Dictionary<string, SoundRequestBGMMenu> bgmMenu; //menu sounds use AudioStreamPlayer
    private Dictionary<string, SoundRequestBGMMenu> bgmAmbient; //bgm and ambient use AudioStreamPlayer
    #endregion

    #region Queues
    private Queue<SoundRequestBGMMenu> bgmQueue = new();
    private Queue<SoundRequestSFX> sfxQueue = new();

    [Export] private int maxPlayers = 8; //change the max amount of audioplayers that are created
    [Export] private int maxPlayers2D = 16; //change max amount of audioplayers2D that are created, 16 seems to be the sweet spot
    #endregion

    #region Dictionary AudioStreamPlayers
    private Dictionary<string, AudioStreamPlayer> players = new();
    private Dictionary<string, AudioStreamPlayer2D> players2D = new();
    public Dictionary<string, SoundRequestSFX> playerToRequestSFX = new();
    private Dictionary<string, SoundRequestBGMMenu> playerToRequestBGM = new();


    private List<AudioStreamPlayer2D> sortedPlayers = new();
    public SoundRequestSFX nextRequestSFX;
    private AudioStreamPlayer2D availablePlayer2D;
    private AudioStreamPlayer availablePlayer;
    private SoundRequestBGMMenu nextRequestBGM;
    private AudioStreamPlayer currentBGMPlayer;

    private bool expiredNextRequestSFX;
    private bool expiredNextRequestBGM;

    //used for displaying in the ui debug menu
    //public AudioStreamPlayer2D debugPlayer;
    #endregion
    #endregion

    #region Methods
    #region Ready
    public override void _Ready()
    {
        InitGrabNodes();

        SpawnAudioStreamPlayers();
    }
    #endregion

    #region Initialize Nodes and Variables
    private void InitGrabNodes()
    {
        slManager = GetNode<SaveLoadManager>("/root/SaveLoadManager");
        gameManager = GetNode<GameManager>("/root/GameManager");
        playerScript = GetNode<Player>("/root/Main/World/Player");
        nextRequestTimerClockSFX = GetNode<TimerClock>("/root/SoundManager/NextRequestTimerClockSFX");
        expireTimerSFX = GetNode<Timer>("/root/SoundManager/ExpireTimerSFX");

        //assign to new variables to shorten code
        sfxPlayer = slManager.SFXPlayer;
        sfxEnemy = slManager.SFXEnemy;
        sfxEnvironment = slManager.SFXEnvironment;
        bgmMenu = slManager.BGMMenu;
        bgmAmbient = slManager.BGMAmbient;
    }
    #endregion

    #region AudioStreamPlayers
    private void SpawnAudioStreamPlayers()
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            AudioStreamPlayer player = new()
            {
                Name = $"AudioStreamPlayer{i}"
            };
            Timer ambientMenuTimer = new()
            {
                Name = $"TimeClear{i}",
                OneShot = true
            };

            ambientMenuTimer.Timeout += () => OnTimerTimeoutBGM();
            player.AddChild(ambientMenuTimer);
            player.SetMeta("TimesUsed", 0);
            AddChild(player);
            players.Add(player.Name, player);
            playerToRequestBGM.Add(player.Name, new());
        }

        for (int i = 0; i < maxPlayers2D; i++)
        {
            AudioStreamPlayer2D player2D = new()
            {
                Name = $"AudioStreamPlayer2D{i}"
            };
            Timer timer2D = new()
            {
                Name = $"TimeClear{i}",
                OneShot = true
            };
            TimerClock ageCounter = new()
            {
                Name = $"AgeCounter{i}",
            };

            timer2D.Timeout += () => OnTimerTimeoutSFX();
            player2D.AddChild(timer2D);
            player2D.AddChild(ageCounter);
            player2D.SetMeta("TimesUsed", 0);
            AddChild(player2D);
            players2D.Add(player2D.Name, player2D);
            playerToRequestSFX.Add(player2D.Name, new());
        }
    }
    #endregion

    #region BGM and Menu Sounds
    public void PlayBGMMenu(SoundRequestBGMMenu.SoundSourceBGMMenu identity, string soundKey)
    {
        SoundRequestBGMMenu request = identity switch
        {
            SoundRequestBGMMenu.SoundSourceBGMMenu.BGM => bgmAmbient.GetValueOrDefault(soundKey),
            SoundRequestBGMMenu.SoundSourceBGMMenu.Ambient => bgmAmbient.GetValueOrDefault(soundKey),
            SoundRequestBGMMenu.SoundSourceBGMMenu.Menu => bgmMenu.GetValueOrDefault(soundKey),
            _ => throw new("Invalid BGM identity property")
        };

        if (request.Source != SoundRequestBGMMenu.SoundSourceBGMMenu.Default)
        {
            bgmQueue.Enqueue(request);

            PlayNextBGM();
        }
    }

    //To call PlayNextBGM, use 'await PlayNextBGM();' if within an asynchronous context, or use '_ = PlayNextBGM();' if called from a non-async method.
    private void PlayNextBGM()
    {
        //Leave early if the queue is empty
        if (bgmQueue.Count == 0)
        {
            GD.PushWarning("PlayNextBGM bgmQueue is empty");
            return;
        }

        //Look and set the next request in the queue
        nextRequestBGM = bgmQueue.Peek();

        //Checks to see if the source of the next sound is for the menu, If it is for the menu it should be played without waiting for priority
        if (nextRequestBGM.Source == SoundRequestBGMMenu.SoundSourceBGMMenu.Menu)
        {
            // Find the player currently playing a BGM track
            currentBGMPlayer = players.Values.FirstOrDefault(p => p.Stream != null && playerToRequestBGM[p.Name].Source == SoundRequestBGMMenu.SoundSourceBGMMenu.Menu);

            // If current Player is null that means the sound is not currently playing and the sound can be played in the first available player
            if (currentBGMPlayer != null)
            {
                currentBGMPlayer.Stream = null;
                PlayAudioBGM(currentBGMPlayer);
            }
            else
            {
                // Find the first available player that is not full
                PlayAudioBGM(players.Values.FirstOrDefault(p => p.Stream == null));
            }
        }
        else
        {
            //If the bgm is not a menu sound it goes to the HandleBGM method to be dealt with
            HandleBGM(nextRequestBGM.Source);
        }
    }

    private void PlayAudioBGM(AudioStreamPlayer player)
    {
        if (bgmQueue.Count == 0)
        {
            GD.PushWarning("PlayAudioBGM Method bgmQueue is empty");
            return;
        }

        nextRequestBGM = bgmQueue.Dequeue();

        // Update the player-to-request mapping
        playerToRequestBGM[player.Name] = nextRequestBGM;

        // Set up the available player
        player.Stream = nextRequestBGM.Stream;
        player.Bus = nextRequestBGM.SoundBus;
        player.VolumeDb = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(player.Bus));

        // Print to console
        GD.Print($"Playing Sound: {playerToRequestBGM[player.Name].SoundName}, Bus: {player.Bus}, VolumeLevel: {player.VolumeDb}");

        // Fade in the sound
        Tween tween = CreateTween();
        tween.TweenProperty(player, "volume_db", player.VolumeDb, 1.0f);

        //Play the Sound
        player.Play();
        player.SetMeta("TimesUsed", (int)player.GetMeta("TimesUsed") + 1);

        ((Timer)player.GetChild(0)).Start(player.Stream.GetLength());
    }

    private void HandleBGM(SoundRequestBGMMenu.SoundSourceBGMMenu source)
    {
        // Find the player currently playing a BGM track
        currentBGMPlayer = players.Values.FirstOrDefault(p => p.Stream != null && playerToRequestBGM[p.Name].Source == source);

        if (currentBGMPlayer != null)
        {
            if (currentBGMPlayer.Stream == nextRequestBGM.Stream)
            {
                // If it is the same, dequeue and exit
                nextRequestBGM = bgmQueue.Dequeue();
                return;
            }
            else
            {
                // If it's a different track, fade out the current track
                Tween tween = CreateTween();
                tween.TweenProperty(currentBGMPlayer, "volume_db", -80.0f, 0.4f);

                // Connect to the tween's `finished` signal to start the next BGM after fade-out
                tween.Finished += () =>
                {
                    // Clear the current track
                    currentBGMPlayer.Stream = null;

                    // Play the next BGM on an available player
                    PlayAudioBGM(players.Values.FirstOrDefault(p => p.Stream == null));
                };
            }
        }
        else
        {
            // Find the first available player and play the BGM immediately if no player is currently active
            PlayAudioBGM(players.Values.FirstOrDefault(p => p.Stream == null));
        }
    }

    private void OnTimerTimeoutBGM()
    {
        foreach (AudioStreamPlayer player in players.Values.Where(p => ((Timer)p.GetChild(0)).TimeLeft == 0).ToList())
        {
            player.Stream = null;
        }

        if (bgmQueue.Count > 0)
        {
            PlayNextBGM();
        }
    }

    //Useful if clearing out all BGM audiostreams and the bgmqueue are needed
    private void ClearBGMAudioStreams()
    {
        if (bgmQueue.Count > 0)
        {
            bgmQueue.Clear();

            foreach (AudioStreamPlayer player in players.Values.Where(p => p.Stream != null))
            {
                player.Stream = null;
            }

            GD.PushWarning("All BGM AudioStreams and BGMQueue have been cleared");
        }
    }
    #endregion

    #region SFX
    public void PlaySFX(SoundRequestSFX.SoundSourceSFX identity, string soundKey)
    {
        SoundRequestSFX request = identity switch
        {
            SoundRequestSFX.SoundSourceSFX.Player => sfxPlayer.GetValueOrDefault(soundKey),
            SoundRequestSFX.SoundSourceSFX.Enemy => sfxEnemy.GetValueOrDefault(soundKey),
            SoundRequestSFX.SoundSourceSFX.Environment => sfxEnvironment.GetValueOrDefault(soundKey),
            _ => throw new("Invalid SFX identity property")
        };

        if (request.Source != SoundRequestSFX.SoundSourceSFX.Default)
        {
            sfxQueue.Enqueue(request);

            PlayNextSFX();
        }
    }

    private void PlayNextSFX()
    {
        if (sfxQueue.Count == 0)
        {
            GD.PushWarning("sfxQueue is empty");
            return;
        }

        nextRequestSFX = sfxQueue.Peek();
        nextRequestTimerClockSFX.StartCounter();
        expireTimerSFX.Start(nextRequestSFX.Stream.GetLength());
        expiredNextRequestSFX = false;

        SetDistanceToPlayer(nextRequestSFX);
        sortedPlayers = players2D.Values.OrderBy(p => GetPriority(p.Name)).ToList();

        if (players2D.Values.All(p => p.Stream != null))
        {
            HandleOccupiedPlayers();
        }
        else
        {
            // Find and play on the first available player
            PlayAudioSFX(players2D.Values.FirstOrDefault(p => p.Stream == null));
        }
    }

    // Handle the case where all players are occupied
    private void HandleOccupiedPlayers()
    {
        if (!nextRequestSFX.CanPlayLate || expiredNextRequestSFX)
        {
            DequeueCurrentRequest();
            return;
        }

        foreach (AudioStreamPlayer2D player in sortedPlayers)
        {
            if (ShouldPlayCurrentRequestOnPlayer(player))
            {
                PlayAudioSFX(player);
                break;
            }
            else
            {
                DequeueCurrentRequest();
            }
        }
    }

    // Method to dequeue the current request and reset timers
    private void DequeueCurrentRequest()
    {
        if (sfxQueue.Count > 0)
        {
            nextRequestSFX = sfxQueue.Dequeue();
            nextRequestTimerClockSFX.StopCounter();
            expireTimerSFX.Stop();
        }
    }

    // Check if the current request should play on the given player
    private bool ShouldPlayCurrentRequestOnPlayer(AudioStreamPlayer2D player)
    {
        bool isPriorityMatch = GetPriority(player.Name) <= nextRequestSFX.Source && !expiredNextRequestSFX;
        bool isDistanceMatch = playerToRequestSFX[player.Name].DistanceToPlayer >= nextRequestSFX.DistanceToPlayer && !expiredNextRequestSFX;
        bool isAgeMatch = RequestCurrentAgeSFX(player) >= nextRequestTimerClockSFX.TimePassed && !expiredNextRequestSFX;

        return isPriorityMatch || isDistanceMatch || isAgeMatch;
    }

    // The method that sets up and plays the SFX on the selected AudioStreamPlayer2D
    private void PlayAudioSFX(AudioStreamPlayer2D player)
    {
        //for Debugging purposes to send to the ui
        //debugPlayer = player;

        if (sfxQueue.Count == 0)
        {
            GD.PushWarning("PlayAudioSFX method: sfxQueue is empty");
            return;
        }

        nextRequestSFX = sfxQueue.Dequeue();
        nextRequestTimerClockSFX.StopCounter();
        expireTimerSFX.Stop();

        playerToRequestSFX[player.Name] = nextRequestSFX;

        player.Stream = nextRequestSFX.Stream;
        player.Bus = nextRequestSFX.SoundBus;
        player.VolumeDb = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(player.Bus));

        GD.Print($"Playing Sound: {nextRequestSFX.SoundName}, Bus: {player.Bus}, Volume: {player.VolumeDb}");

        PlaySoundAtObject(nextRequestSFX.ObjectNode.GlobalPosition, player);
        player.Play();
        player.SetMeta("TimesUsed", (int)player.GetMeta("TimesUsed") + 1);

        ((Timer)player.GetChild(0)).Start(player.Stream.GetLength());
        ((TimerClock)player.GetChild(1)).StartCounter();
    }

    private double RequestCurrentAgeSFX(AudioStreamPlayer2D player)
    {
        //sets the age of the current player to the counters time and returns back the value to get compared
        playerToRequestSFX[player.Name].RequestAge = ((TimerClock)player.GetChild(1)).TimePassed;
        return playerToRequestSFX[player.Name].RequestAge;
    }

    private void SetDistanceToPlayer(SoundRequestSFX request)
    {
        // Get the position of the player character
        Vector2 playerPosition = playerScript.GetPlayerPosition();  //method to get the player's position

        // Get the position of the object that requested the sound
        Vector2 objectPosition = GetObjectPosition(request);  // method to get the object's position

        // Calculate the distance between the player and the object
        float distance = playerPosition.DistanceTo(objectPosition);

        // Set the DistanceToPlayer property of the request
        request.DistanceToPlayer = distance;
    }

    private Vector2 GetObjectPosition(SoundRequestSFX request)
    {
        // Get the position of the object that requested the sound
        return request.ObjectNode.GlobalPosition;
    }

    private SoundRequestSFX.SoundSourceSFX GetPriority(string playerName)
    {
        // Try to get the SoundRequest corresponding to the player
        if (playerToRequestSFX.TryGetValue(playerName, out SoundRequestSFX request))
        {
            // If the player exists, return the SoundSource
            return request.Source;
        }
        else
        {
            // If the player does not exist, return the default SoundSource
            return SoundRequestSFX.SoundSourceSFX.Default;
        }
    }

    //signal method for when the timer ends on the sfxplayer
    private void OnTimerTimeoutSFX()
    {
        //check each audioplayer2D for timers that hit 0 and empty the audiostream and stop the counter for how long the sound has been playing for
        foreach (AudioStreamPlayer2D player in players2D.Values.Where(p => ((Timer)p.GetChild(0)).TimeLeft == 0).ToList())
        {
            player.Stream = null;
            ((TimerClock)player.GetChild(1)).StopCounter();
        }

        if (sfxQueue.Count > 0)
        {
            PlayNextSFX();
        }
    }

    //signal for when the expired timer ends and sets the variable to false to end the sound in the queue
    private void OnExpiredTimerTimeoutSFX()
    {
        //will get rid of any sound left in the sfx queue
        if (expireTimerSFX.TimeLeft == 0)
        {
            expiredNextRequestSFX = true;
        }
    }

    //Used to find the position of the source of the sound and play it at the location at the time it gets played
    private void PlaySoundAtObject(Vector2 target, AudioStreamPlayer2D player)
    {
        if (player != null)
        {
            player.GlobalPosition = target;
        }
    }

    //Useful if clearing out all SFX audiostreams and queue are needed
    private void ClearSFXAudioStreams()
    {
        if (sfxQueue.Count > 0)
        {
            sfxQueue.Clear();
            Vector2 playerPosition = playerScript.GetPlayerPosition();

            foreach (AudioStreamPlayer2D player in players2D.Values)
            {
                player.Stream = null;
                player.GlobalPosition = playerPosition;
            }

            GD.PushWarning("All SFX AudioStreams and BGMQueue have been cleared");
        }
    }
    #endregion
    #endregion
}