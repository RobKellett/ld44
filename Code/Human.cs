using Godot;
using System;

public class Human : Node2D
{
  public enum Team
  {
    Red,
    Blue,
    Green
  }

  private Sprite _clothesSprite;
  private AnimationPlayer _animationPlayer;
  private Team _team;

  public override void _Ready()
  {
    var sprite = GetChild<ResourceSprite>(0);
    sprite.LoadRandomTexture("human", 3);

    _clothesSprite = sprite.GetChild<Sprite>(0);
    _animationPlayer = GetChild<AnimationPlayer>(1);
    _animationPlayer.Play("Stand");

    SetTeam(_team);
  }

  public void SetTeam(Team team)
  {
    _team = team;

    if (_clothesSprite == null)
    {
      return;
    }

    Texture clothesTexture;

    if (team == Team.Red)
    {
      clothesTexture = (Texture)GD.Load("res://Assets/clothes-red.png");
    }
    else if (team == Team.Blue)
    {
      clothesTexture = (Texture)GD.Load("res://Assets/clothes-blue.png");
    }
    else if (team == Team.Green)
    {
      clothesTexture = (Texture)GD.Load("res://Assets/clothes-green.png");
    }
    else
    {
      return;
    }

    _clothesSprite.SetTexture(clothesTexture);
  }
}
