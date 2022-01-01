using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGame();

    public void ShowMessage(string text)
    {
        Label message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();

        GetNode<Timer>("MessageTimer").Start();

    }

    async public void ShowGameOver()
    {
        ShowMessage("Game Over");

        Timer messageTimer = GetNode<Timer>("MessageTimer");
        await ToSignal(messageTimer, "timeout");

        Label message = GetNode<Label>("Message");
        message.Text = "Diamandi";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1), "timeout");
        GetNode<Button>("StartButton").Show();
    }

    public void UpdateScore(int score, int hiScore)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();

        GetNode<Label>("HiScoreLabel").Text = hiScore.ToString();

    }

    public void _on_StartButton_pressed()
    {
        GetNode<Button>("StartButton").Hide();
        EmitSignal("StartGame");
    }

    public void _on_MessageTimer_timeout()
    {
        GetNode<Label>("Message").Hide();
    }

}
