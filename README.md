Stalactites/Cave Diver
Game Description
2D Metroidvania with a focus on puzzle platforming, with a high skill ceiling
Game Theme/Narrative
The overall goal of the game. What’s the player trying to accomplish and why?
Explore caves (Explore)
Alien planet
Defeat bosses to obtain ship parts to repair ship and beat the game

You’re essentially a space scout who works independently for companies or governments who get contracted for jobs. Jobs could include anything really, but for you they often involve exploring dangerous or remote regions to determine their potential for resources, finding uncontacted or extinct civilizations, or potential threats/weaknesses of your contractors enemies. This job is one assigned by a company researching a material for its potential as a stabilizer for their energy generators. Interest in this planet is from scans detecting an extinct planet with signs of a previous civilization and cave networks. Hyper dense rare elements have been detected in trace amounts and there is potential for unique compounds to be discovered. You’re assigned to further investigation via spelunking and drilling into the planet as the materials are likely below the surface if present. (Explore)

After landing the ship you start to explore entering a cave, you then are cut off from the surface and your ship from a cave floor collapsing into a tunnel. You lose your weapon and tools in the rockslide. You land in the area known as the shrine with the first Bio power up, the Stalag System. As you collect it a miniboss appears and you defeat it with your new stalagmite ability.
As you make your way through the tunnels you’ll find the stalactite ability allowing you to attack aerial enemies and create platforms and barriers. This will give you the ability to find your way back up to the surface. Your ship needs parts so you have to search the caves underground. The first module found will be the antenna enabling purchases and access to the expanded knowledge database and new research logs (essentially this is the easy mode for enabling struggling players but requires grinding). 
Game Gimmick/Main Mechanic
What’s the strong point of the game?  A gameplay mechanic?  A strange setting?  A unique art style?

Stallae G. Taits / Spike Droppes / the main character can create stalagmites and stalactites that alter the environment and damage enemies
Game Description
Game Theme/Narrative
Game Gimmick/Main Mechanic
Characters
Enemies/Obstacles
Power ups
Stages
Godot Game Structure Ideas:
Scene Switching Script:
SaveLoadManager:
GameManager:
World:
Area:
Audio:
Logic:





Characters
Player
Describe
- Impact boots
- knee bracers
- headlamp
-
-


Sketch!
Game art!


Suit Concept Art	



VFX Ideas:
Effects layer that has a glimmer/shine effect 
- use as a idle animation
White along flow lines blue sparkle from gems
Randomized to occasionally add the effect in animations



Their story: 
Where do they come from, why are they motivated to take action, what do they want to accomplish

Default Abilities:Run, jump, slide, pistol, wall pop, wall slide 
How might they be programmed?  What states do they use?

NPC 2
NPC 3
Sidekick 4
Pet 5
Enemies/Obstacles
Main villain
	What do they want to accomplish?
	How do they cause problems for, or challenge the player?
	How do they get stronger as the game progresses?
Boss ideas
‘Stalactite twins’ intended first encounter. Alters the environment by creating stalactites. Golem creature. Shifting rock snake? Open to revision. The Second Twin will unlock Stalagmites. 
‘Eyeball 1’ with optical nerve tentacles
‘Eyeball 2’ with retina light cones: pairs with visual effect of monochrome where only lit attacks are effective. Limited vision, the player needs to remember obstacles etc.
‘Hand’
‘Throat’ essentially immobilized but damages through sound “singing”. Will summon creatures and shift the environment through “songs” to move itself and the player.
‘Spine and Ribs’ crawls with ribs, grapples and swings with spine. Break ribs to expose organ weak points
Standard enemies
‘mini-hand’
‘Mosquito’
‘Spider/Beetle’
	What does it do?
	What does it look like?
	How does it animate?  Wiggle? Strut? Flicker?	
How might it be programmed?  What are its states?
Obstacle 3
Trap 4

Quick enemy design thoughts
akimbot balance struggles rapid fire weak becomes ineffective vs sniper
Enemy design struggles to balance health 
Ratchet and clank enemy that helps solve this issue: slimes that break into multiple enemies 
Less oriented to maxing damage, splitting essentially acts as a health gate and multiplies potential threats 
Can be inverse balanced by combining into single slime increasing health gate and armour encouraging a switch back to a higher damage weapon like a sniper 
Also happens within sewers with twisting , narrow, converging paths, allowing enemies to approach before being attacked allows for useful melee tactics, high fire rate/low damage, AOE, and long range on farther tunnels

Power ups
Weapon 1
	What does it do?	
	What are its strengths and weaknesses?
	How might it be programmed?  What are its states?
Ability 2
Potion 3
Obtain multiple elements ie. fire, water, steel
Increase to Jump height to allow easier platforming
Longer duration to stalags that are created for easier timing
Powerslide function to make short dashes along the ground and quick verticals along wall surfaces
Health and Damage upgrades are obtained after semi-big events to give a bigger boost and increase their respective stat

UI
Opening menu
 Visual
Reflection of the visor sees the planet
Sees the shrine last saved in
Arm menu 
Armour panel flips up for screen


Stages
Overall world:
Is it a planet with many ecosystems?  An underground bunker? A galaxy with spaceships?
Alien planet (Explore)
The Shrine: contains the first ability the Stalag System gained through an alien technology, has images and statues that visually demonstrate how moves work and what buttons to use. When you enter the Shrine the rockslide that was triggered when the cave collapsed will have killed the alien that has the matching Stalag ability. The area will expand whenever you gain new alien abilities. Shrines allow the player to warp from any other shrine totem unlocked (new alien ability).
The ship landing area (ground zero) Has the ship that can leave the planet once you have returned power and a flight module to the ship. Returning to the ship allows for you to gain new tech abilities/upgrades, as a consequence there is no warp to the ship. However new abilities will shorten the distance to the ship via shortcuts. (Explore)

Hub:
Defeating bosses unlock shrines that can act as teleporters to other unlocked shrines allowing for quick traversal to already explored areas
Player ship will act as a safe area where the player can make upgrades to equipment

Stage 1:
Inspirational pics, sketches, or level map here




Tiles here






Setting:
What’s the scenery like?
Caves/Underground
Different parts of the underground have different elemental themes ie. Water, Fire, Steel, Ice, Stone,  hardened alien structures, desert (expansion area?)
Above Ground/Surface
 desert (expansion area?)

Gimmick:
What mechanic or plot element does it teach the player?  IE: Encounter the easiest enemies. Learn to jump on them to kill them.
Come across terrain that is impassable and player needs to figure out how to use the stalag system to make platforms need to progress
Alien Warp Gates, the secondary fast travel method mainly for shortcuts for returning Modules to the ship as they cannot warp with the Shrines
Gates connect via manual override of the coordinates
Some Alien Warp Gates only connect when the entire series has been overridden; this allows for separate challenges on linear pathways to return Modules to the ship without backtracking and for fast travel to areas without just being an instant return to the ship with new modules. 
Nice for going to central areas that are farther from the ship (hard areas)
General terrain:
	What will the player be traversing?  Lots of jumping?  Pits to avoid?  Ropes to climb?
Loads of jumping, all the jumping, so much jumping the came will never stop shaking
Player will be required to create ‘tites’ using the stalag system to gain the vertical and horizontal movement necessary to traverse
Enemies/Power ups found here:
	-
	-
	-
Plot elements; what and where:
Any notable events or cut scenes that progress the story?
The tunnel collapse that sets the story for having no tools/weapons and introduces the alien abilities and gameplay
Reaching your ship on the surface gets attacked and damaged requiring the player to go back down into the underground to defeat the bosses that have critical ship components in order to repair the ship to leave

Unique Goal: 
Does this stage have a unique victory condition?


Notable feature:  
Is there special scenery or an event, or a particularly difficult spot that this stage might be remembered for?
	-
How might the stage’s second half progress the mechanics\gimmicks introduced in its first half?
Second half might be from when the player gets back to the ship after starting in the caves and making their way back to the surface

Bonus rooms/alternate exits? So many What, where, and where do they take the player?


Boss ideas:
	-

Stage 2:
Stage 3:
Final Challenge:

Show off!
"Elevator" pitch:  
Describe your game in less than 5 sentences.  Does it sound fun?  Would a stranger remember it if you told them?
	-
Make some cool pics that you would show to people to show how cool your game is!
Cover Art:








Wordmark:
Additional art; characters, mechanics, scenery












Brainstorming:
What’s interesting to you/Where would you like to start?  Overall game idea? Interesting character?  Interesting villain?  Interesting setting?  Interesting Mechanic?

Spike Droppes / Stallae G. Taits / the main player character creates stalagmites from the ground to impale enemies; these spikes are also dangerous to the player. There may be a time limit before the spikes break which is upgradeable through discovering areas & defeating enemies. Size, number and control may also be upgradeable. There may be an upgrade to prevent damage from self created spikes but not to walk on them. Until the player unlocks stalactites they create from the roof and the ability to drop them (very early game, essentially the tutorial section of the map). When dropped aligned correctly they will create a platform the player can now walk on until the timer attached to the platform runs out. The alignment is not intensely strict, but should not be so loose it looks bad. The center of the dropping spike will always be the center of the platform. These core concepts are key to the design of the game and the map. (Explore)

What could be the in-world origin to this idea?  What might this idea progress into?  (If it’s a character, where did they come from, what do they want to do?  If it’s a mechanic, what different ways can it be used?  Can it be simplified, or made more complex?

While passing by the SH4-RP R0K5 system, the galactic energy company, Unstaibill Energenesis, contacts the Bounty Hunter Spike with an offer that will allow for unrestricted travel and access to all of their tech. Provided Spike can unearth the rare materials detected. These materials could hold the key to accessing energy density antimatter colliders the company is developing. There is evidence of the potential detected by ultra dense particles along with indications of a previous civilization within the SH4-RP R0K5 system.
The difficulty setting is shown by displaying hN(possible) planets one most favored by the company which is the intended difficulty. He also offers one less dangerous option but requires more in depth searching and digging.

Game Mechanics
What’s the player’s ultimate goal?  What does finishing the game look like?  
Explore the Caves
Kill the Final Boss(es)
100 percent explore the map (Explore)
Collect all upgrades (Explore)
Collect all items/collectables??? (Explore)



What does progression and accomplishment look like?
Moving throughout the different areas of the game world (exploring)
Defeating enemies
Defeating Bosses
Collecting power-ups/ Upgrades to help progress though tough obstacles (exploring)
Defeating the final boss 

What does failure look like?  Looking in a mirror…
Losing all hit points
Enemies dealing enough damage to kill the player
Failing to platform past obstacles 
Low skill level

List out the ways that the player can fail the game:
Defeated by enemies/ losing all health
Being killed by environmental hazards - things like falling into lava or bottomless pits

What kind of playstyle is the game?  Shooter? Adventure? Single screen? Top down?
Side scroller
Parallax background
adventure/platformer/puzzle/2D

How does your player move through the game?  Crawl?  Run? Fly?
Runs around in detail:
Letting go of the forward direction causes a rolling stop
Pressing the opposite direction results in a screeching stop (immediate, sets up for a punch)
Pressing down while Running causes a slide (will decay speed)
Pressing down and holding forwards results in a roll (maintains speed)
After rolling holding forward and down crawls (immediately locked at a slow speed)
Power Slide: Pressing Boost +
Ability to jump - possible double jump power-up
Jumping in detail:
Hold to increase height & tuck
By pressing back immediately before jumping you’ll initiate a punch for more height (less distance) 
A quick tap will do a hop, useful for a low and long setup. 
If you jump immediately on the landing frames out of this short hop you will initiate an accelerating long jump if you are following through pressing forwards with your previous momentum (exceeds normal running speed cap).
You can maintain long jumps (and momentum) if you press jump and forward right as you land, otherwise you are capped back to max run speed
Alternatively pressing jump and up will result in a high jump
Holding up will give a high jump maintaining momentum.
By tapping backward immediately before jumping you will punch and maximize your jump height.
Diving: While in air pressing down will dive left or right 
Ducking
Shielding 
Defense increase
Rolling
knockback/stun reduction
Sliding
Power Sliding

What can they interact with; climb, shoot, switch?
Basic Attack: this is a close range stomp used whenever Stalag abilities are used
Scout Rifle: the starting multi-tool attachment that was the player’s original weapon. Severely outclassed by Stalag abilities, but in the chance the player avoids obtaining them this is their main attack
Gets lost in the cave-in encounter, can be obtained again through unlocking modules and making a purchase (most likely will not be adopted as there will be access to the spike shot with the same research if the Stalag system is obtained)
Create platforms to jump on to progress though the world
Shoot spikes to break hidden walls/ floors
There will be ways to get past barriers outside of using the intended abilities, however it will require significant skill and effort, even tedium. [Ex. One shot from a spike could break an obstacle, but if the player ignores/neglects the upgrade the obstacle could be broken but it would take 1000 strikes. Performing a complex high level skilled attack would greatly reduce this.] For those who are willing to try and beat the game without following the path it will be a fun secret though.
Shoot switches/objects for puzzles.
Give subtle indicators when the player has the intended ability ex. Nearby yellow gemstones glowing in the background/foreground.
Different colors could help suggest which ability to use. Match the color with the boss
Tech upgrades from the ship for tech-based abilities
Level up the scanner for more hints and explanations (the obvious method, will be more expensive, grindy for parts)
Fighting bosses
Defeating an alien boss will allow you to attain a bio power or upgrade if they are the same type
Defeating a tech boss will give modules for upgrading the ship. Modules may unlock tech/suit powers or upgrades for the player


What kind of programming states will your player utilize?  Will they require special coding?  What might that entail?
Knowing when the player is in the air vs. when player is on the ground
When player is running, current speed, accelerating, stopping, primed for a jump
Keeping track of which element the player has access to and which ones player is using at any given time



How might “health” be lost or gained?
Health Lost:
Making contact with enemies
Enemies shooting projectiles or using melee attacks against the player
Coming into contact with environmental hazards
Health Gained:
Defeating enemies
Picking up rare healing  items around the game world
Possible ‘Hub’ area to restore lost hit points (similar to Samus’s ship in Metroid), shrines

How might points be lost or gained?
Materials gathered from defeating enemies/finding ore/crystal veins, abandoned caches, enemy loot (Explore)
Gathering Crystals/materials to use for upgrades
Money for consuming/selling materials (used for tech)


What other variables might the game keep track of?  Time, ammo, magic, stamina?  What causes them to decrease or increase?
Time remaining for created stalagmites/tites before auto destruction
Activation up-time for the power slide - limit the single use time player can use before needing to wait and ‘recharge’
Amount of resources collected



List some possible stages/levels for your game, or just different settings/scenery that might be connected to your initial inspiration:
Hub area/ Surface with ship
Water-based cavern
Lava-based cavern
Spooky-based cavern
Steel-like alien like structures

Note how you would move through them. What kind of obstacles there might be.  Is any special programming required?  What might it entail?
Basic movement to move through the game world
Stalag mechanic will be needed to make platforms to allow player to climb surfaces not readily accessible
Some enemies will need to be defeated by the stalag mechanic as they block progression into other areas
Climbing walls and jumping from one to the next will be needed to get to more difficult areas
Breakable walls 
Ice
Levers activated by spikes
Fill Buckets with spikes
Scales, player weight vs stuff
Can you reorder them from possible easy stages to more difficult ones?  Or in what order would they appear in your story?
Game world is open and places can be accessed in whichever order the player wants to go as long as the player has the skill to platform into certain areas. Player can obtain upgrades to make certain platforming easier

List some Characters, playable or non-playable:
Spike Droppes the main playable character
Stallae G. Taits 
-
-
Note how they progress the story/gameplay.
How would the player interact with them? Character objects? Cutscenes?

What can they each do, or how do they direct the player?

Is any special programming required?  What might it entail?

Where in the game might the player come across them?



List some baddies:
Final boss
Two or Three other main bosses
Several different enemy types

Note how they might impede the player.  How they physically move in the stage.
Can you come up with simpler, or more complex versions of them?
Is any special programming required?  What might it entail?
Different bosses have stolen vital ship parts that need to be reclaimed before being able to leave the planet


Can you reorder them from easiest to most challenging?
-
-
-

List some Powerups:
Powerups are divided into two distinct parts:
BIO 
These are the abilities obtained from the natural alien world and defeating bosses
Mainly designed around the Stalag (spikes) System
TECH
These are abilities primarily from returning Modules to the ship (usually found in an area locked behind a boss or ability obtained from a boss)
Some Tech abilities are purchased through the Antenna Module by gathering materials for resources, selling materials/resources, and rewards from the contractors/clients for research or tech.
Some tech upgrades will be from the Manufacturing Module exchanged for resources and materials
Modules are transported one at a time to the ship backtrack through shortcuts from new abilities and through Alien Warp Gates
Stalagmites (Bio)
Upward shot
Stalactites (Bio)
Stalactite drop
Platform creation
Platform destruction
More spikes to summon from the ground/ceiling (Bio)
Ability to gain different elemental spikes ie. water, fire, steel ect. (Bio)
Wall Cling (Tech)
Possible Double Jump (Bio)
 increases to main jump (Bio & Tech)
Damage boosts through various discovered abilities (Bio & Tech)
Ship upgrades (Tech) - collect resources while exploring
Ship Modules (Modules are collected from alien tech and resources being returned to the ship)
Defense and or health (Armour Module)
Scanner Module (explains objects)
Upgrades for enemies (may detail weaknesses)
Upgrades for puzzles, extremely expensive (requires cost per puzzle)
Radar Module (helps direct player find points of interest or next objective)
 Upgrades to help with hidden objects
Knowledge database (Tech)/Research Logs (Bio) (These help explain moves, advanced and hidden techniques for players who can’t figure them out. Basic level is cheap, advanced is expensive)
Spike Shot (Bio & Tech)
Power slide (carries momentum) (Tech)
Wall Slide
Wall climb (powerslide the wall)(carries momentum) (Tech)
Powerup Descriptions Detailed:
Rock Stalagmite, Stalactite drop, merge to create platform
Fire Stalagmite, Stalactite drop, merge to create ?firewall?
Fire Stalagmite into water = geyser
Rock Stalagmite into geyser = rising platform
Steel/hardened Stalagmite, Stalactite drop, merge to create ?Grabbable Climbing Rings/Cage?
Steel/hardened material ceilings, walls, floors can only generate steel spikes once it is unlocked.
Ice spike freeze water
Fire spike melts ice
Speed up stalagmites, Slow Down Stalactites, merge to create warp holes 

Note how they make the game easier.  Do they have weaknesses?
What kind of programming might they require?
Elements would be required to complete certain platforming puzzles/challenges
The double jump would be late game, final or secret boss could be locked behind having the power-up
Power sliding will allow for higher jumps 
wall sliding will allow for scaling walls quickly
Wall cling will help the player choose the next step they want to take as they can wait on walls
The power slide can recharge while they wait on the wall if they messed up their timing while wall sliding

Can you reorder them from least to most helpful for the player?
-
-
-
Godot Game Structure Ideas: 

Changing States:

Ground to Air State:
Player leaving the ground
Area2D on the players feet
If Area2D leaves the ground the character is in the air
Keep track of how the player enters the air

Ground to Wall State:
Player directional input into the wall + Any horizontal body collider touching a wall

Ground to Climbing State:
If character is in Area2D of climbable surface + up direction is pressed

Wall to Ground State:
Before step up is triggered
Foot Area2D is on the floor

Wall to Air State:
If not touching wall with body colliders + not touching the floor with the bottom collider

Climbing to Ground State:
Feet collider is touching floor + holding down direction
Chest collider enters the top of the climbable surface’s Area2D

Climbing to Air State:
If the jump key is pressed
When Player drops down

Air to Ground State:
If landing animation finishes + foot collider is touching floor
If foot collider is on floor + down direction is pressed
If foot collider is on floor + in stomp
Air to Climbing State:
If collider is in the climbing Area2D + up key is pressed

Air to Wall State:
If the body colliders enter the wall + direction is pressed into the wall


This is just brainstorming possibilities of how to structure the game in the engine hierarchy.


Root				AutoLoaded Scripts




-------------------------------------------------------------------------------------------------------------------

Main (Main Game Node) : NODE - used as a container for the entire game
{
		
		SaveLoadManager : NODE - used to load assets and levels
{
	Script Attached
Load all Scenes
Load all sounds
}

	UI (Main UI Node) : CONTROL - used as a container for all the UI elements in the game
	{
UI will have access to the Player Stats autoloaded script to keep the hud up-to-date

		UI node will include multiple nodes in the UI scene:
Main Menu
Player HUD
Pause Menu
Inventory Screen
Anything visual the the person playing sees will be in here
}

SoundManager (Main Sound Node) : NODE - used as a container for all the Sound elements for the game
{
Will contain everything involving the control of sounds for the game
BGM
Taking damage
Dealing damage
Using powers
Etc…
All sound requests go to the Manager to be played in order
A Priority System is in place to make sure the most important sounds are played if the game is busy
}

GameManager : NODE - used to control aspects about the game
		{
Script Attached
			World (Main Level Scene Node) : NODE
used as a container for all the level scenes the player can play through


{
All scenes that involve levels will be in here
This scene will be filled with all the different levels that would be loaded
Area1 Scene
Enemy Scenes
Tilemap
Obstacles
Environment
Resources
Everything that shows up when entering into the level
Area2 Scene
Area3 Scene and so on….
All level scenes will be here but only the certain levels that are accessible by the Player will be loaded into memory

Player (Player Scene) : CharacterBody2D -  the main character the person playing can play the game as
{

The player is a scene that will include everything needed to interact with the game world
The player will access its stats by using the autoloaded script as its main source and will update that script when changes are made

AnimatedSprite2D
CollisionBody2D
Raycast2D
Other nodes for gameplay
}


}

		}
}

Scene Switching Script:
SaveLoadManager:

public partial class SaveLoadManager : Node
{
    // Store your scenes here
    public PackedScene[] Scenes { get; set; }

    public List<int> AllLoadedScenes { get; set; }
    public List<int> WorldLoadedScenes { get; set; }

    public override void _Ready()
    {
        // Load your scenes
        Scenes = new PackedScene[] {
            (PackedScene)GD.Load("res://Area1.tscn"),
            (PackedScene)GD.Load("res://Area2.tscn"),
            (PackedScene)GD.Load("res://Area3.tscn"),
            (PackedScene)GD.Load("res://Area4.tscn"),
            (PackedScene)GD.Load("res://Area5.tscn"),
            (PackedScene)GD.Load("res://Area6.tscn"),
            (PackedScene)GD.Load("res://Area7.tscn"),
            (PackedScene)GD.Load("res://Area8.tscn"),
            (PackedScene)GD.Load("res://Area9.tscn"),

            // Add more scenes as needed
        };

        AllLoadedScenes = new();
        WorldLoadedScenes = new();

        for (int i = 0; i < Scenes.Length; i++)
        {
            if (!AllLoadedScenes.Contains(i))
            {
                AllLoadedScenes.Add(i);
            }
        }
    }
}

GameManager:

using System.Collections.Generic;
public partial class GameManager : Node
{
    private SaveLoadManager slManager;
    private SoundManager soundManager;
    private UI ui;
    private World world;

    private PackedScene[] worldScenes;

    [Export] private int[] startScene;
    private List<int> worldLoadedScenes;
    private List<int> allLoadedScenes;

    public override void _Ready()
    {
        slManager = GetNode<SaveLoadManager>("/root/Main/SaveLoadManager");
        soundManager = GetNode<SoundManager>("/root/Main/SoundManager");
        ui = GetNode<UI>("/root/Main/UI");
        world = GetNode<World>("/root/Main/GameManager/World");

        worldScenes = slManager.Scenes;
        worldLoadedScenes = slManager.WorldLoadedScenes;
        allLoadedScenes = slManager.AllLoadedScenes;


        foreach (int startSceneIndex in startScene)
        {
            if (allLoadedScenes.Contains(startSceneIndex))
            {
                worldLoadedScenes.Add(startSceneIndex);
            }
        }

        foreach (int loadSceneIndex in worldLoadedScenes) 
        {
            world.CallDeferred("LoadSceneWorld", loadSceneIndex);
        }
    }
 }

World:
public partial class World : Node2D
{
    private SaveLoadManager slManager;
    private GameManager gameManager;

    public override void _Ready()
    {
        slManager = GetNode<SaveLoadManager>("/root/Main/SaveLoadManager");
        gameManager = GetNode<GameManager>("/root/Main/GameManager");
    }

    public void LoadSceneWorld(int index)
    {
        // Make sure the index is within the array bounds
        if (index < 0 || index >= slManager.Scenes.Length)
        {
            GD.PushWarning("Invalid scene index in LoadSceneWorld Method");
            return;
        }

        string levelString = "/root/Main/GameManager/World/Area" + (index + 1);

        if (GetNodeOrNull(levelString) == null)
        {
            // Instance the scene
            Node scene = slManager.Scenes[index].Instantiate();

            // Add it to the current scene
            AddChild(scene);
        }
        else
        {
            GD.PushWarning("Node Exists, Cannot create Duplicate");
        }
    }

    public void UnloadSceneWorld(Node scene)
    {
        if (scene != null)
        {
            // Remove the scene from the tree
            RemoveChild(scene);
            scene.QueueFree();
        }
        else
        {
            GD.PushWarning("Scene null in UnloadSceneWorld");
        }
    }
}

Area: 

using System.Collections.Generic;
using System.Linq;
public partial class Area : Node2D
{
    [Export] private Vector2 startLocation;

    private SaveLoadManager slManager;
    private GameManager gameManager;
    private World world;

    [Export] private int[] ScenesToLoad;
    private List<int> worldLoadedScenes;
    private List<int> allLoadedScenes;

    public override void _Ready()
    {
        slManager = GetNode<SaveLoadManager>("/root/Main/SaveLoadManager");
        gameManager = GetNode<GameManager>("/root/Main/GameManager");
        world = GetNode<World>("/root/Main/GameManager/World");

        Position = startLocation;
        worldLoadedScenes = slManager.WorldLoadedScenes;
        allLoadedScenes = slManager.AllLoadedScenes;
    }

    public void OnLevelBoundaryBodyEntered(Node body)
    {
        if(body != null && body.IsInGroup("Player")) 
        {
            LoadCheck();
            UnLoadCheck();
        }
    }

    public void LoadCheck()
    {
        worldLoadedScenes.Clear();
        worldLoadedScenes.AddRange(ScenesToLoad.Distinct());

        foreach (int loadSceneIndex in worldLoadedScenes)
        {
            string levelString = "/root/Main/GameManager/World/Area" + (loadSceneIndex + 1);

            if (GetNodeOrNull(levelString) == null)
            {
                world.CallDeferred("LoadSceneWorld", loadSceneIndex);
            }
        }

        worldLoadedScenes.Reverse();
    }

    public void UnLoadCheck()
    {
        foreach (int unLoadSceneIndex in allLoadedScenes.Except(worldLoadedScenes))
        {
            string levelString = "/root/Main/GameManager/World/Area" + (unLoadSceneIndex + 1);
            Node sceneGet = GetNodeOrNull(levelString);

            if (sceneGet != null)
            {
                world.CallDeferred("UnloadSceneWorld", sceneGet);
                worldLoadedScenes.Remove(unLoadSceneIndex);
            }
        }
    }
}


Audio:

BGM, ambient sounds
Title screen BGM
Depends on the area
BGM more often
Ambient sounds quiet and subtle 
Player sounds
Multi-channel
Footsteps
attacks
Enemy sounds
Multi-Channel
Footsteps
Attacks
Prioritize based on where they are based on the player
Bosses get higher priority
Environment
Breakable objects
Activate switches/doors
Solve puzzles

Logic:

If that becomes a problem, I might consider something like:
Decide on the maximum number of simultaneous sounds you want to allow
Assign a "priority" (importance) value to each individual sound
When a sound request comes into the Manager...



If your max overlapped sounds hasn't been reached, just play the requested sound
If the max has been reached, see if a sound with a lower priority than the request is currently playing. If so, stop that sound and use its player for the new sound.
If the all sounds playing are of higher priority that the request, just ignore the request

For example a ranking system where the manager will cutoff sounds (when all slots are full) not just based on their priority but for example based on their volume(the lower volume will be cutted of 1st),their distance from listener and so on..That will be more accurate.
