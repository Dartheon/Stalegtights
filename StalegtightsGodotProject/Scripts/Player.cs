using Godot;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    public Vector2 GetPlayerPosition()
    {
        //returns the Player's global location in the game world
        return GlobalPosition;
    }
}