using Godot;
using System;

public class Player : Area2D
{
    [Signal]
    public delegate void Hit();

    [Signal]
    public delegate void PickedUp();

    [Export]
    public int Speed = 400;

    public bool shieldOn = false;

    private Vector2 _screenSize;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _screenSize = GetViewport().Size;
        Hide();
        
    }

    public override void _Process(float delta)
    {
        //show or hide shield
        if (shieldOn)
        {
            GetNode<Sprite>("Shield").Show();
        }
        else
        {
            GetNode<Sprite>("Shield").Hide();
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
        } else if(velocity.y != 0)
        {
            animatedSprite.Animation = "up";
            animatedSprite.FlipV = velocity.y > 0;
        }


    }

    public void _on_Player_area_entered(Area2D area)
    {
        ToggleShield();
        EmitSignal("PickedUp");
    }



    public void _on_Player_body_entered(PhysicsBody2D body)
    {//if not shielded, hit
        if (!shieldOn)
        {

            Hide();
            EmitSignal("Hit");
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        } else //if shielded
                //destroy mob
               //lose shield
        {
            body.QueueFree();  
            ToggleShield();
        }
    }

    public void Start(Vector2 pos)
    {
        Position = pos;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public void ToggleShield()
    {
        shieldOn = !shieldOn;
    }
}
