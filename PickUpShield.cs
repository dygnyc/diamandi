using Godot;
using System;

public class PickUpShield : Area2D
{

    public void PickedUp()
    {
        QueueFree();
    }
 
}
