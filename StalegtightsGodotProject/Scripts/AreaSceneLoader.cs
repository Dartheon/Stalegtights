using Godot;

public partial class AreaSceneLoader : Node2D
{
    #region Variables
    #region Position
    [Export] private Vector2 startLocation; //sets the starting position of the area so it is in place when game starts
    #endregion

    #region Class Scripts
    private GameManager gameManager;
    #endregion

    #region Scenes
    //every transition has its corresponding scenes to load in it individual array
    [Export] private string[] SceneNamesToLoad;
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
        gameManager = GetNode<GameManager>("/root/GameManager");

        //Setting the start location for the area
        Position = startLocation;
    }
    #endregion

    #region Area2D Enter Check
    //Signal Area Check when a body is detected in the area
    public void OnBodyEnteredSceneLoading(Node body)
    {
        if (body != null && body.IsInGroup("Player"))
        {
            //Call the method to load scenes using the list attached to the specific area
            gameManager.ScenesManager(SceneNamesToLoad);
        }
    }
    #endregion
    #endregion
}