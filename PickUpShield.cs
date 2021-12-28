using Godot;
using System;

public class PickUpShield : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";


    public void PickedUp()
    {
        QueueFree();
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
