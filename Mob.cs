using Godot;
using System;

public class Mob : RigidBody2D
{
    [Export]
    public int minSpeed = 150;

    [Export]
    public int maxSpeed = 250;

    static private Random _random = new Random();

    string mobType;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {//randomly choose an animation
        AnimatedSprite animSprite = GetNode<AnimatedSprite>("Alive");
        animSprite.Show();
        string[] mobTypes = animSprite.Frames.GetAnimationNames();

        mobType = mobTypes[_random.Next(0, mobTypes.Length)];
        animSprite.Animation = mobType;
        //store a reference to the mobType name
        //to be used later for matching death animation

    }


    public void _on_VisibilityNotifier2D_screen_exited()
    {
        //after exiting screen, mob should be destroyed

        //    int mobsCount = GetTree().GetNodesInGroup("mobs").Count;
        //    GD.Print("mobs count: "+mobsCount);
        //      just wanted to check to make sure mobs were being destroyed
        //      by getting the count of mob nodes

        QueueFree();
    }

    public void _on_Alive_hide()
    {

        HitShield();
    }

    public void HitShield()
    {
        //death on hitting shield
        //hide alive sprite
        //    AnimatedSprite animSprite = GetNode<AnimatedSprite>("Alive");
        //    animSprite.Hide();

        GetNode<AudioStreamPlayer>("mobDeath").Play();


        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        //disable collisions, so animation can play without affecting game play


        //show death sprite
        AnimatedSprite animDeathSprite = GetNode<AnimatedSprite>("Death");
        animDeathSprite.Animation = mobType; //set death animation to same name as alive animation
        animDeathSprite.Show();
        animDeathSprite.Play();

        //play animation
        //because it is not looping and also sends out a signal to queuefree
        //after animation is finished

    }

    public void _on_Death_animation_finished()
    {
        QueueFree();
    }




}
