using Godot;
using System;

public class Main : Node
{
    [Export]
    public PackedScene Mob;

    [Export]
    public PackedScene Shield;

    private int _score;
    private int _hiScore;

    private Random _random = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        OS.SetWindowTitle("Diamandi");
    }

    // We'll use this later because C# doesn't support GDScript's randi().
    private float RandRange(float min, float max)
    {
        return (float)_random.NextDouble() * (max - min) + min;
    }


    public void game_over()
    {
        GetNode<Timer>("ShieldTimer").Stop();
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        GetNode<HUD>("HUD").ShowGameOver();

        //GD.Print("call Group");
        GetTree().CallGroup("mobs", "queue_free");
        GetTree().CallGroup("pickups", "queue_free");
        //destroy any existing mobs and pickups

        GetNode<AudioStreamPlayer>("Music").Stop();

    }

    public void NewGame()
    {

        GetNode<AudioStreamPlayer>("Music").Play();

        _score = 0;
        Player player = GetNode<Player>("Player");
        Position2D startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();

        HUD hud = GetNode<HUD>("HUD");
        hud.UpdateScore(_score, _hiScore);
        hud.ShowMessage("Get Ready!");

        //spawn shield at beginning of a new game
        SpawnShield();

    }

    public void _on_StartTimer_timeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    public void _on_ScoreTimer_timeout()
    {
        _score++;

        //set high score
        if (_score >= _hiScore)
        {

            _hiScore = _score;

        }

        GetNode<HUD>("HUD").UpdateScore(_score, _hiScore);

        // GD.Print(_hiScore);

    }

    public void _on_MobTimer_timeout()
    {
        // Choose a random location on Path2D.
        PathFollow2D mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.Offset = _random.Next();

        // Create a Mob instance and add it to the scene.
        RigidBody2D mobInstance = (RigidBody2D)Mob.Instance();
        AddChild(mobInstance);

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mobInstance.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mobInstance.Rotation = direction;

        // Choose the velocity.
        mobInstance.LinearVelocity = new Vector2(RandRange(150f, 250f), 0).Rotated(direction);
    }


    public void _on_ShieldTimer_timeout() { SpawnShield(); }


    public void SpawnShield()
    {
        //get random x and y within window
        //spawn shield within window with padding
        int xPos = _random.Next(50, (int)OS.GetRealWindowSize().x - 50);
        int yPos = _random.Next(50, (int)OS.GetRealWindowSize().y - 50);

        // instance shield
        Area2D shieldInstance = (Area2D)Shield.Instance();
        AddChild(shieldInstance); //add to scene
        shieldInstance.Position = new Vector2(xPos, yPos); //set position

    }

}
