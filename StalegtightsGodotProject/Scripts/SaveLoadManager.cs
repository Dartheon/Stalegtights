using Godot;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; //Terminal -> New Terminal: dotnet add package Microsoft.Data.Sqlite
using System;

//SceneLoading Class Is Used as a Constructor To Initilize Important Info for each Scene to be Referenced Later
public partial class SceneLoading : Node
{
    //The Name of the Scene
    public string SceneName { get; private set; }

    //The State of the Scene, to be used for setting if it is to be loaded or not
    public bool SceneState { get; set; }

    //The Packed Scene, this is the scene that is loaded from the Resourceloader
    public PackedScene ScenePackedLoaded { get; private set; }

    //The Scenes Node, to be assigned when creating a new node that gets Instantiated in the game world
    public Node SceneNode { get; set; }

    public SceneLoading() { }
    public SceneLoading(string sceneName, bool sceneState, PackedScene scenePackedLoaded, Node sceneNode)
    {
        SceneName = sceneName;
        SceneState = sceneState;
        ScenePackedLoaded = scenePackedLoaded;
        SceneNode = sceneNode;
    }
}

public partial class SaveLoadManager : Node
{
    #region Variables
    #region Class Scripts
    private GameManager gameManager;
    #endregion

    #region Scenes
    //Dictionary to hold all SceneLoading Class variables to be called when loading new scenes
    public Dictionary<string, SceneLoading> ScenesLoaded { get; private set; }
    #endregion

    #region Sounds
    //Sound Loading Lists
    public Dictionary<string, SoundRequestSFX> SFXPlayer { get; private set; }
    public Dictionary<string, SoundRequestSFX> SFXEnemy { get; private set; }
    public Dictionary<string, SoundRequestSFX> SFXEnvironment { get; private set; }
    public Dictionary<string, SoundRequestBGMMenu> BGMMenu { get; private set; }
    public Dictionary<string, SoundRequestBGMMenu> BGMAmbient { get; private set; }

    private Node2D soundObjectSFX;
    private Node soundObjectBGMMenu;
    #endregion

    #region Inventory
    //Will Be Filled In Soon
    #endregion
    #endregion

    #region Methods
    public override void _Ready()
    {
        InitNodes();

        //Creates the Dictionary to be called on when loading new scenes into the game world
        SceneCreation();

        //Creates the Dictionaries that hold the different sounds and sound effect to be called from the game
        SoundCreation();

        //Creates the Dictionaries to Hold Inventory Items
        InventoryCreation();

        // Initialize the database
        InitializeDatabases();
    }

    #region Initialize Nodes and Variables
    public void InitNodes()
    {
        gameManager = GetNode<GameManager>("/root/GameManager");
    }
    #endregion

    #region Scenes
    private void SceneCreation()
    {
        //To load Scenes into this Dictionary
        ScenesLoaded = new()
        {
            //Example//
            //["Area1"] = new("Area1", false, (PackedScene)ResourceLoader.Load("res://Areas/Area1.tscn"), null),
            ["TestScene"] = new("TestScene", false, (PackedScene)ResourceLoader.Load("res://Scenes/Playable Scenes/TestScene.tscn"), null),
            // Add more scenes if needed...
        };
    }
    #endregion

    #region Sounds
    private void SoundCreation()
    {
        //Load your sounds
        #region Load Player Sounds
        SFXPlayer = new()
        {
            //["PlayerShoot"] = new("PlayerShoot", (AudioStream)ResourceLoader.Load("res://Sounds/player_shoot.wav"), SoundRequestSFX.SoundSourceSFX.Player, false, "Master", 0.0f, 0.0f, soundObjectSFX)
            // Add more sounds as needed...
        };
        #endregion

        #region Load Enemy Sounds
        SFXEnemy = new()
        {
            //["EnemyHit"] = new("EnemyHit", (AudioStream)ResourceLoader.Load("res://Sounds/enemy_hit.wav"), SoundRequestSFX.SoundSourceSFX.Enemy, false, "Master", 0.0f, 0.0f, soundObjectSFX)
            // Add more sounds as needed...
        };
        #endregion

        #region Load Environment Sounds
        SFXEnvironment = new()
        {
            //["ChestOpen"] = new("ChestOpen", (AudioStream)ResourceLoader.Load("res://Sounds/chest_open.wav"), SoundRequestSFX.SoundSourceSFX.Environment, true, "Master", 0.0f, 0.0f, soundObjectSFX)
            //Add more sounds as needed...
        };
        #endregion

        #region Load Menu Sounds
        BGMMenu = new()
        {
            ["MenuClick"] = new("MenuClick", (AudioStream)ResourceLoader.Load("res://Sounds/menu_click.wav"), SoundRequestBGMMenu.SoundSourceBGMMenu.Menu, "Master")
            // Add more sounds as needed
        };
        #endregion

        #region Load BGM Sounds
        BGMAmbient = new()
        {
            #region BGM
            //["TitleBGM"] = new("TitleBGM", (AudioStream)ResourceLoader.Load("res://Sounds/title_music.ogg"), SoundRequestBGMMenu.SoundSourceBGMMenu.BGM, "Master"),
            //["RaveBGM"] = new("RaveBGM", (AudioStream)ResourceLoader.Load("res://Sounds/rave_bgm.ogg"), SoundRequestBGMMenu.SoundSourceBGMMenu.BGM, "Master"),
            // Add more sounds as needed...
            #endregion

            #region Ambient
            //["EerieAmbient"] = new("EerieAmbient", (AudioStream)ResourceLoader.Load("res://Sounds/eerie_ambient.mp3"), SoundRequestBGMMenu.SoundSourceBGMMenu.Ambient, "Ambient"),
            //Add more sounds as needed...
            #endregion
        };
        #endregion
    }
    #endregion

    #region Inventory
    private void InventoryCreation()
    {
        //Will Be Filled In Soon
    }
    #endregion

    #region SaveLoadGame
    #region Create DataBases
    //Will Be Dealt With Later
    public void InitializeDatabases()
    {
        /*// Connect to the SQLite database
        using (SqliteConnection connection = new($"DataSource={GameDataDbPath};"))
        {
            connection.Open();
            GD.Print("gamedata Database connection opened.");

            // Create a table if it doesn't exist
            string createTableQuery0 = @"
                CREATE TABLE IF NOT EXISTS PlayerStats (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    maxhp INTEGER NOT NULL,
                    currenthp INTEGER NOT NULL,
                    xp INTEGER NOT NULL,
                    lastsavelocation STRING NOT NULL
                );
            ";

            // Create a backup table if it doesn't exist
            string createTableQuery1 = @"
                CREATE TABLE IF NOT EXISTS BackupPlayerStats (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    maxhp INTEGER NOT NULL,
                    currenthp INTEGER NOT NULL,
                    xp INTEGER NOT NULL,
                    lastsavelocation STRING NOT NULL
                );
            ";

            //add table to database
            using (SqliteCommand command = new(createTableQuery0, connection))
            {
                command.ExecuteNonQuery();
            }

            //add backup table to database
            using (SqliteCommand command = new(createTableQuery1, connection))
            {
                command.ExecuteNonQuery();
            }

            //update playerstats
            string checkTableQuery0 = @"SELECT COUNT(*) FROM PlayerStats";

            using (SqliteCommand command = new(checkTableQuery0, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery0 = @"
                        INSERT INTO PlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (0, 1, 1, 0, 'Default');
                        INSERT INTO PlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (1, 1, 1, 0, 'Default');
                        INSERT INTO PlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (2, 1, 1, 0, 'Default');
                        INSERT INTO PlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (3, 1, 1, 0, 'Default');
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery0, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            //update backup playerstats
            string checkTableQuery1 = @"SELECT COUNT(*) FROM BackupPlayerStats";

            using (SqliteCommand command = new(checkTableQuery1, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery1 = @"
                        INSERT INTO BackupPlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (0, 1, 1, 0, 'Default');
                        INSERT INTO BackupPlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (1, 1, 1, 0, 'Default');
                        INSERT INTO BackupPlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (2, 1, 1, 0, 'Default');
                        INSERT INTO BackupPlayerStats (id, maxhp, currenthp, xp, lastsavelocation) VALUES (3, 1, 1, 0, 'Default');
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery1, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            string createTechUpgradesTable = @"
                CREATE TABLE IF NOT EXISTS TechUpgrades (
                    key TEXT PRIMARY KEY,
                    value TEXT NOT NULL,
                    description TEXT NOT NULL );
                ";

            using (SqliteCommand command = new(createTechUpgradesTable, connection))
            {
                command.ExecuteNonQuery(); // Create the table if it doesn't exist
            }

            checkTableQuery0 = @"SELECT COUNT(*) FROM TechUpgrades";

            using (SqliteCommand command = new(checkTableQuery0, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertTechUpgrade = @"
                    INSERT INTO TechUpgrades (key, value, description) VALUES ('DefaultName', ' ?????????? ', 'Yet To Be Uncovered...');
                    INSERT INTO TechUpgrades (key, value, description) VALUES ('TechUpgrade0', 'TechUpgrade1', 'This is the first tech upgrade');
                    INSERT INTO TechUpgrades (key, value, description) VALUES ('TechUpgrade1', 'TechUpgrade2', 'This is the second tech upgrade');
                    INSERT INTO TechUpgrades (key, value, description) VALUES ('TechUpgrade2', 'TechUpgrade3', 'This is the third tech upgrade');
                    INSERT INTO TechUpgrades (key, value, description) VALUES ('TechUpgrade3', 'TechUpgrade4', 'This is the fourth tech upgrade');
                    ";
                    //Add more Tech Upgrades as needed... Don't forget to add a Description for each Name

                    using (SqliteCommand insertCommand = new(insertTechUpgrade, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            GD.Print("Tech Upgrade Table inserted successfully.");
        }

        //connection to the InventoryItems Database
        using (SqliteConnection connection = new($"DataSource={InventoryItemsDbPath};"))
        {
            connection.Open();
            GD.Print("inventoryitems Database connection opened.");

            // Create a table if it doesn't exist
            string createTableQuery0 = @"
                CREATE TABLE IF NOT EXISTS InventoryItemsSlot0 (
                    gridcontainer STRING NOT NULL,
                    itemname STRING NOT NULL,
                    quantity INTEGER NOT NULL,
                    PRIMARY KEY (gridcontainer, itemname)
                );
            ";

            // Backup data query (use this for your temp backup)
            string createBackupQuery = $@"
                    CREATE TABLE IF NOT EXISTS BackupInventoryItems (
                    gridcontainer STRING NOT NULL,
                    itemname STRING NOT NULL,
                    quantity INTEGER NOT NULL,
                    PRIMARY KEY (gridcontainer, itemname)
                );
            ";

            using (SqliteCommand command0 = new(createTableQuery0, connection))
            {
                command0.ExecuteNonQuery();
            }

            using (SqliteCommand command1 = new(createBackupQuery, connection))
            {
                command1.ExecuteNonQuery();
            }

            //Check Table to see if it Exists and insert Defaults
            string checkTableQuery0 = @"SELECT COUNT(*) FROM InventoryItemsSlot0";

            using (SqliteCommand command = new(checkTableQuery0, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery = @"
                        INSERT INTO InventoryItemsSlot0 (gridcontainer, itemname, quantity) VALUES ('Consumables', 'Default', 0);
                        INSERT INTO InventoryItemsSlot0 (gridcontainer, itemname, quantity) VALUES ('Modules', 'Default', 0);
                        INSERT INTO InventoryItemsSlot0 (gridcontainer, itemname, quantity) VALUES ('Equipment', 'Default', 0);
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            //Check Backup Table for if it Exists and insert Defaults
            string checkBackupTableQuery0 = @"SELECT COUNT(*) FROM BackupInventoryItems";

            using (SqliteCommand command = new(checkBackupTableQuery0, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery = @"
                        INSERT INTO BackupInventoryItems (gridcontainer, itemname, quantity) VALUES ('Consumables', 'Default', 0);
                        INSERT INTO BackupInventoryItems (gridcontainer, itemname, quantity) VALUES ('Modules', 'Default', 0);
                        INSERT INTO BackupInventoryItems (gridcontainer, itemname, quantity) VALUES ('Equipment', 'Default', 0);
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            // Create a table if it doesn't exist
            string createTableQuery1 = @"
                CREATE TABLE IF NOT EXISTS InventoryItemsSlot1 (
                    gridcontainer STRING NOT NULL,
                    itemname STRING NOT NULL,
                    quantity INTEGER NOT NULL,
                    PRIMARY KEY (gridcontainer, itemname)
                );
            ";

            using (SqliteCommand command = new(createTableQuery1, connection))
            {
                command.ExecuteNonQuery();
            }

            string checkTableQuery1 = @"SELECT COUNT(*) FROM InventoryItemsSlot1";

            using (SqliteCommand command = new(checkTableQuery1, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery = @"
                        INSERT INTO InventoryItemsSlot1 (gridcontainer, itemname, quantity) VALUES ('Consumables', 'Default', 0);
                        INSERT INTO InventoryItemsSlot1 (gridcontainer, itemname, quantity) VALUES ('Modules', 'Default', 0);
                        INSERT INTO InventoryItemsSlot1 (gridcontainer, itemname, quantity) VALUES ('Equipment', 'Default', 0);
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            // Create a table if it doesn't exist
            string createTableQuery2 = @"
                CREATE TABLE IF NOT EXISTS InventoryItemsSlot2 (
                    gridcontainer STRING NOT NULL,
                    itemname STRING NOT NULL,
                    quantity INTEGER NOT NULL,
                    PRIMARY KEY (gridcontainer, itemname)
                );
            ";

            using (SqliteCommand command = new(createTableQuery2, connection))
            {
                command.ExecuteNonQuery();
            }

            string checkTableQuery2 = @"SELECT COUNT(*) FROM InventoryItemsSlot2";

            using (SqliteCommand command = new(checkTableQuery2, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery = @"
                        INSERT INTO InventoryItemsSlot2 (gridcontainer, itemname, quantity) VALUES ('Consumables', 'Default', 0);
                        INSERT INTO InventoryItemsSlot2 (gridcontainer, itemname, quantity) VALUES ('Modules', 'Default', 0);
                        INSERT INTO InventoryItemsSlot2 (gridcontainer, itemname, quantity) VALUES ('Equipment', 'Default', 0);
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }

            // Create a table if it doesn't exist
            string createTableQuery3 = @"
                CREATE TABLE IF NOT EXISTS InventoryItemsSlot3 (
                    gridcontainer STRING NOT NULL,
                    itemname STRING NOT NULL,
                    quantity INTEGER NOT NULL,
                    PRIMARY KEY (gridcontainer, itemname)
                );
            ";

            using (SqliteCommand command = new(createTableQuery3, connection))
            {
                command.ExecuteNonQuery();
            }

            string checkTableQuery3 = @"SELECT COUNT(*) FROM InventoryItemsSlot3";

            using (SqliteCommand command = new(checkTableQuery3, connection))
            {
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                if (rowCount == 0)
                {
                    string insertDefaultsQuery = @"
                        INSERT INTO InventoryItemsSlot3 (gridcontainer, itemname, quantity) VALUES ('Consumables', 'Default', 0);
                        INSERT INTO InventoryItemsSlot3 (gridcontainer, itemname, quantity) VALUES ('Modules', 'Default', 0);
                        INSERT INTO InventoryItemsSlot3 (gridcontainer, itemname, quantity) VALUES ('Equipment', 'Default', 0);
                    ";

                    using (SqliteCommand insertCommand = new(insertDefaultsQuery, connection))
                    {
                        //execute the command
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }*/
    }
    #endregion

    #region Access SaveLoadMethods
    //Will Be Dealt With Later
    /*public void SaveGameDataDatabase(int saveSlot)
    {
        // Save game logic
        saveMaxHP = playerInventory.PlayerStats.PlayerMaxHealth;
        saveCurrentHP = playerInventory.PlayerStats.PlayerCurrentHealth;
        saveXP = playerInventory.PlayerStats.PlayerEXP;
        saveLastSaveLocation = playerInventory.PlayerStats.PlayerLastSaveLocation;

        using (SqliteConnection connection = new($"DataSource={GameDataDbPath};"))
        {
            connection.Open();
            GD.Print("gamedata Database connection opened.");

            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Cleanup query to remove existing backup data
                    string cleanupQuery = @"
                            DELETE FROM BackupPlayerStats 
                            WHERE id = @id;
                ";

                    using (SqliteCommand cleanupCommand = new(cleanupQuery, connection, transaction))
                    {
                        cleanupCommand.Parameters.AddWithValue("@id", saveSlot);
                        int cleanupRows = cleanupCommand.ExecuteNonQuery();
                        GD.Print($"{cleanupRows} rows deleted from BackupPlayerStats for player ID: {saveSlot}.");
                    }

                    // Backup current data before update
                    string backupQuery = @"
                            INSERT INTO BackupPlayerStats (id, maxhp, currenthp, xp, lastsavelocation)
                            SELECT id, maxhp, currenthp, xp, lastsavelocation
                            FROM playerstats
                            WHERE id = @id;
                ";

                    using (SqliteCommand backupCommand = new(backupQuery, connection, transaction))
                    {
                        backupCommand.Parameters.AddWithValue("@id", saveSlot);
                        int backupRows = backupCommand.ExecuteNonQuery();
                        GD.Print($"{backupRows} rows backed up for player ID: {saveSlot}.");
                    }

                    // Update query for player stats
                    string updateQuery = @"
                            UPDATE playerstats SET 
                                maxhp = @maxhp, 
                                currenthp = @currenthp, 
                                xp = @xp, 
                                lastsavelocation = @lastsavelocation 
                            WHERE id = @id;";

                    using (SqliteCommand command = new(updateQuery, connection, transaction))
                    {
                        // Define parameters for the command
                        command.Parameters.AddWithValue("@id", saveSlot);
                        command.Parameters.AddWithValue("@maxhp", saveMaxHP);
                        command.Parameters.AddWithValue("@currenthp", saveCurrentHP);
                        command.Parameters.AddWithValue("@xp", saveXP);
                        command.Parameters.AddWithValue("@lastsavelocation", saveLastSaveLocation);

                        // Execute the command
                        int updatedRows = command.ExecuteNonQuery();
                        GD.Print($"Updated {updatedRows} row(s) for player ID: {saveSlot}.");
                    }

                    // Commit the transaction
                    transaction.Commit();
                    GD.Print("Database updated successfully."
                        + "\nMaxHP: " + saveMaxHP
                        + "\nCurrentHP: " + saveCurrentHP
                        + "\nXP: " + saveXP
                        + "\nLast Save Location: " + saveLastSaveLocation);
                }
                catch (Exception ex)
                {
                    GD.PrintErr("Error saving game data: " + ex.Message);
                    transaction.Rollback();  // Roll back the transaction on error
                    throw;
                }
            }
        }
    }
    public void LoadGameDataDatabase(int loadSlot)
    {
        const int MAXRETRIES = 3; // Number of times to retry
        const int RETRYDELAYMS = 1000; // Delay between retries in milliseconds
        int retryCount = 0;
        bool loadSuccessful = false;

        while (retryCount < MAXRETRIES && !loadSuccessful)
        {
            try
            {
                using (SqliteConnection connection = new($"DataSource={GameDataDbPath};"))
                {
                    connection.Open();
                    GD.Print("gamedata.db Database connection opened.");

                    string selectQuery = @"
                        SELECT maxhp, currenthp, xp, lastsavelocation 
                        FROM playerstats
                        WHERE id = @id";

                    using (SqliteCommand command = new(selectQuery, connection))
                    {
                        // Set the @id parameter
                        command.Parameters.AddWithValue("@id", loadSlot);

                        // Execute the query and use a reader to read the result
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // If the row exists
                            {
                                // Safeguard against null or unexpected values
                                loadMaxHP = !reader.IsDBNull(0) ? reader.GetInt32(0) : 1; // Default to 1 HP if null
                                loadCurrentHP = !reader.IsDBNull(1) ? reader.GetInt32(1) : 1;
                                loadXP = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0; // Default to 0 XP if null
                                loadLastSaveLocation = !reader.IsDBNull(3) ? reader.GetString(3) : "Default"; // Fallback if null

                                // Mark the load as successful
                                loadSuccessful = true;

                                // Assign values to player stats
                                playerInventory.PlayerStats.PlayerMaxHealth = loadMaxHP;
                                playerInventory.PlayerStats.PlayerCurrentHealth = loadCurrentHP;
                                playerInventory.PlayerStats.PlayerEXP = loadXP;
                                playerInventory.PlayerStats.PlayerLastSaveLocation = loadLastSaveLocation;

                                GD.Print("gamedata.db Database loaded successfully."
                                    + "\nMaxHP: " + loadMaxHP
                                    + "\nCurrentHP: " + loadCurrentHP
                                    + "\nXP: " + loadXP
                                    + "\nLast Save Location: " + loadLastSaveLocation);
                            }
                            else
                            {
                                GD.PushWarning("Record not Found for Load Game.");
                                loadSuccessful = true; // No need to retry if record doesn't exist
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr("Error loading game data: " + ex.Message);
                retryCount++;

                if (retryCount < MAXRETRIES)
                {
                    GD.Print("Retrying load... Attempt: " + (retryCount + 1));
                    System.Threading.Thread.Sleep(RETRYDELAYMS); // Wait before retrying
                }
                else
                {
                    GD.PrintErr("Max retry attempts reached. Failed to load game data.");
                }
            }
        }

        if (!loadSuccessful)
        {
            GD.PushError("Failed to load game data after multiple attempts.");
        }
    }
    public void SaveInventoryItemsToDatabase(int saveSlot, GridContainer gridName)
    {
        string tableName = $"InventoryItemsSlot{saveSlot}";
        string gridNameKey = gridName.Name;

        string insertQuery0 = $@"
                INSERT INTO {tableName} (gridcontainer, itemname, quantity) 
                VALUES (@GridContainer, 'Default', 0);
            ";

        // Backup data query (use this for your temp backup)
        string backupQuery = $@"
                INSERT OR REPLACE INTO BackupInventoryItems (gridcontainer, itemname, quantity)
                SELECT gridcontainer, itemname, quantity
                FROM {tableName}
                WHERE gridcontainer = @GridContainer;
            ";

        // Cleanup query to remove items from BackupInventoryItems
        string cleanupQuery = $@"
                DELETE FROM BackupInventoryItems 
                WHERE gridcontainer = @GridContainer;
    ";

        using (SqliteConnection connection = new($"DataSource={InventoryItemsDbPath};"))
        {
            connection.Open();
            GD.Print("InventoryItems Database Connection Opened.");

            using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Clear existing backup data before adding new backup
                    using (SqliteCommand cleanupCommand = new(cleanupQuery, connection, transaction))
                    {
                        cleanupCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);
                        int cleanupRows = cleanupCommand.ExecuteNonQuery();
                        GD.Print($"{cleanupRows} rows deleted from BackupInventoryItems.");
                    }

                    // Backup current data before deletion
                    using (SqliteCommand backupCommand = new(backupQuery, connection, transaction))
                    {
                        backupCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);
                        int backupRows = backupCommand.ExecuteNonQuery();
                        GD.Print($"{backupRows} rows backed up.");
                    }

                    // Clear the existing entries in the table for the given save slot
                    string deleteQuery = $"DELETE FROM {tableName} WHERE gridcontainer = @GridContainer;";
                    using (SqliteCommand deleteCommand = new(deleteQuery, connection, transaction))
                    {
                        deleteCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);
                        int deleteRows = deleteCommand.ExecuteNonQuery();
                        GD.Print($"{deleteRows} rows deleted from {tableName}.");
                    }

                    // Check if InventoryItemsCollectedMap contains key
                    if (!menu.InventoryItemsCollectedMap.ContainsKey(gridName))
                    {
                        using (SqliteCommand insertCommand = new(insertQuery0, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);
                            insertCommand.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                        GD.Print("Inventory items saved to Database.");
                        return;
                    }
                    else
                    {
                        menu.InventoryItemsCollected = menu.InventoryItemsCollectedMap[gridName];
                    }

                    // Prepare the insert query for items
                    string insertQuery1 = $@"
                            INSERT INTO {tableName} (gridcontainer, itemname, quantity)
                            VALUES (@GridContainer, @ItemName, @Quantity);
                        ";

                    if (menu.InventoryItemsCollected.Count > 0)
                    {
                        foreach (KeyValuePair<string, int> item in menu.InventoryItemsCollected)
                        {
                            using (SqliteCommand insertCommand = new(insertQuery1, connection, transaction))
                            {
                                insertCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);
                                insertCommand.Parameters.AddWithValue("@ItemName", item.Key);
                                insertCommand.Parameters.AddWithValue("@Quantity", item.Value);

                                int insertRows = insertCommand.ExecuteNonQuery();
                                GD.Print($"Inserted {insertRows} row(s) for item {item.Key}.");
                            }
                        }
                    }
                    else
                    {
                        using (SqliteCommand insertCommand = new(insertQuery0, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                    GD.Print("Inventory items successfully saved to Database.");
                }
                catch (Exception ex)
                {
                    GD.PrintErr("Error saving inventory items: " + ex.Message);
                    transaction.Rollback();  // Roll back the transaction on error
                    throw;
                }
            }
        }
    }
    public void LoadInventoryItemsFromDatabase(int loadSlot, GridContainer gridName)*/
    /*{
        const int MAXRETRIES = 3; // Maximum number of retries
        const int RETRYDELAYMS = 1000; // Delay between retries in milliseconds
        int retryCount = 0;
        bool loadSuccessful = false;

        string tableName = $"InventoryItemsSlot{loadSlot}";
        string gridNameKey = gridName.Name;

        // Retry loop to handle loading failures
        while (retryCount < MAXRETRIES && !loadSuccessful)
        {
            try
            {
                // Clear the inventory before loading in the saved items
                menu.ClearInventoryGrid(gridName);

                using (SqliteConnection connection = new($"DataSource={InventoryItemsDbPath};"))
                {
                    connection.Open();
                    GD.Print("InventoryItems Database Connection Opened.");

                    // Prepare the select query
                    string selectQuery = $@"
                        SELECT itemname, quantity
                        FROM {tableName}
                        WHERE gridcontainer = @GridContainer;";

                    using (SqliteCommand selectCommand = new(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@GridContainer", gridNameKey);

                        using (SqliteDataReader reader = selectCommand.ExecuteReader())
                        {
                            bool hasItems = false;

                            while (reader.Read())
                            {
                                // Retrieve the item name and quantity from the database
                                string itemName = reader.GetString(0);
                                int quantity = reader.GetInt32(1);

                                // Add the item back into the menu's inventory
                                if (itemName != "Default")
                                {
                                    menu.AddInventoryItem(gridName, itemName, quantity);
                                    GD.Print("Item Loaded: " + itemName + ": " + quantity + " into: " + gridNameKey);
                                    hasItems = true;
                                }
                            }

                            if (!hasItems)
                            {
                                GD.Print("No items found to load for grid: " + gridNameKey);
                            }
                        }
                    }

                    GD.Print("Inventory items loaded successfully from Database.");
                    loadSuccessful = true; // Mark as successful if we get here without exceptions
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr("Error loading inventory items: " + ex.Message);
                retryCount++;

                if (retryCount < MAXRETRIES)
                {
                    GD.Print("Retrying inventory load... Attempt: " + (retryCount + 1));
                    System.Threading.Thread.Sleep(RETRYDELAYMS); // Wait before retrying
                }
                else
                {
                    GD.PrintErr("Max retry attempts reached. Failed to load inventory items.");
                }
            }
        }

        if (!loadSuccessful)
        {
            GD.PushError("Failed to load inventory items after multiple attempts.");
        }
    }*/
    #endregion
    #endregion
    #endregion
}