using Godot;
using System;

public class Player : Area2D
{
    [Signal]
    public delegate void Hit();

    [Signal]
    public delegate void PickedUp();

    [Signal]
    public delegate void ShieldHit();

    [Export]
    public int Speed = 400;

    [Export]
    public float thrustVolume = -8f;

    public bool shieldOn = false;

    private Vector2 _screenSize;

    bool canThrust = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _screenSize = GetViewport().Size;
        Hide();

    }

    public override void _Process(float delta)
    {
        //show or hide shield
        //player has an animated sprite shield child node
        if (shieldOn)
        {
            GetNode<AnimatedSprite>("Shield").Show();
        }
        else
        {
            GetNode<AnimatedSprite>("Shield").Hide();
        }

        Vector2 velocity = new Vector2(); // The player's movement vector.

        if (Input.IsActionPressed("ui_right"))
        {

            velocity.x += 1;

        }

        if (Input.IsActionPressed("ui_left"))
        {
            velocity.x -= 1;
        }

        if (Input.IsActionPressed("ui_down"))
        {
            velocity.y += 1;
        }

        if (Input.IsActionPressed("ui_up"))
        {
            velocity.y -= 1;
        }

        AnimatedSprite animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

        if (velocity.Length() > 0)
        {
            if (canThrust)
            {
                GetNode<AudioStreamPlayer>("Thrust").Play();
                canThrust = false;
            }

            velocity = velocity.Normalized() * Speed;
            animatedSprite.Play();
        }
        else
        {
            animatedSprite.Animation = "idle";
            animatedSprite.Play();
        }

        //move sprite, clamped to screen
        Position += velocity * delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.x, 0, _screenSize.x),
            y: Mathf.Clamp(Position.y, 0, _screenSize.y)
        );

        //flip animations
        if (velocity.x != 0)
        {
            animatedSprite.Animation = "walk";
            animatedSprite.FlipV = false;

            animatedSprite.FlipH = velocity.x < 0;
        }
        else if (velocity.y != 0)
        {
            animatedSprite.Animation = "up";
            animatedSprite.FlipV = velocity.y > 0;
        }


    }
    public void _on_Thrust_finished()
    {
        canThrust = true;
    }


    public void _on_Player_area_entered(Area2D area)
    {
        //Collision with the ShieldPickUp which is an areas 2D
        //the mobs are PhysicsBodys

        GetNode<AudioStreamPlayer>("ShieldPickup").Play();


        ToggleShield();
        EmitSignal("PickedUp");
        area.QueueFree(); //destroy shield pickup
        GetParent().GetNode<Timer>("ShieldTimer").Stop();//stop timer

    }



    public void _on_Player_body_entered(PhysicsBody2D body)
    {//if not shielded, hit
        if (!shieldOn)
        {
            //GD.Print(body.GetNode<CollisionShape2D>("CollisionShape2D").Disabled);

            //if the collision shape is enabled register hit
            //else do nothing (it is a dying mob)
            if (!body.GetNode<CollisionShape2D>("CollisionShape2D").Disabled)
            {
                //GD.Print("hit");
                GetNode<AudioStreamPlayer>("playerDeath").Play();

                GetNode<AnimatedSprite>("AnimatedSprite").Hide();

                GetNode<AnimatedSprite>("playerDeathSprite").Show();
                GetNode<AnimatedSprite>("playerDeathSprite").SetFrame(0);
                GetNode<AnimatedSprite>("playerDeathSprite").Play();

                GetNode<AudioStreamPlayer>("Thrust").SetVolumeDb(-80); //turn off thrust volume

                EmitSignal("Hit");
                GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
            }
        }
        else
        {
            //if shielded
            //destroy mob
            //lose shield
            body.GetNode<AnimatedSprite>("Alive").Hide();

            ToggleShield();
            //start shield timer to respawn pickup
            GetParent().GetNode<Timer>("ShieldTimer").Start();
            //i need to stop timer when a shield is picked up
        }
    }

    public void _on_playerDeathSprite_animation_finished()
    {
        GetNode<AnimatedSprite>("playerDeathSprite").Hide();
        Hide();
    }


    public void Start(Vector2 pos)
    {
        Position = pos;

        Show();
        GetNode<AudioStreamPlayer>("Thrust").SetVolumeDb(thrustVolume); //reset thrust volume
        GetNode<AnimatedSprite>("AnimatedSprite").Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public void ToggleShield()
    {
        shieldOn = !shieldOn;
    }
}
