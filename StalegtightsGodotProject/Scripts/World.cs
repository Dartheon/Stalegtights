using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class World : Node2D
{
    #region Variables
    #region Class Scripts
    private SaveLoadManager slManager;

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
    }
    #endregion

    #region Load and Unload Scene
    public void LoadSceneWorld()
    {
        //Checks each key in the dictionary
        foreach (string sceneName in slManager.ScenesLoaded.Keys.ToList())
        {
            //sets the state and node for the current scene being checked
            bool isSceneLoaded = slManager.ScenesLoaded[sceneName].SceneState;
            Node currentScene = slManager.ScenesLoaded.GetValueOrDefault(sceneName).SceneNode;

            //When the scene state is true it checks to see if it exists, if it does not then it creates a new node and adds it to the game world
            if (isSceneLoaded)
            {
                // If the scene is not already created, instance and add it to the scene tree
                if (currentScene == null)
                {
                    Node newScene = slManager.ScenesLoaded[sceneName].ScenePackedLoaded.Instantiate();
                    slManager.ScenesLoaded[sceneName].SceneNode = newScene;
                    AddChild(newScene);
                }
            }
            //if the node state is false then it checks to see if it exists, if it does exist then it removes it from the game world
            else
            {
                // As long as the Node exists, remove and free it. Otherwise nothing needs to happen
                if (currentScene != null)
                {
                    RemoveChild(currentScene);
                    currentScene.QueueFree();
                    slManager.ScenesLoaded[sceneName].SceneNode = null;
                }
            }
        }
    }
    #endregion
    #endregion
}