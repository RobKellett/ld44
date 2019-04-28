using Godot;
using System;
using LD44.Utilities;

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

  public const float MAX_THIRST = 30; // seconds
  public float Thirst = MAX_THIRST;
  public const float MAX_HUNGER = 50; // seconds
  public float Hunger = MAX_HUNGER;
  public bool Dead = false;
  public override void _Ready()
  {
    var sprite = GetChild<ResourceSprite>(0);
    sprite.LoadRandomTexture("human", 3);

    _clothesSprite = sprite.GetChild<Sprite>(0);
    _animationPlayer = GetChild<AnimationPlayer>(1);

    var breatheSpeed = (float)RNG.Instance.NextDouble() / 10f + 0.95f;
    _animationPlayer.Play("Stand", -1, breatheSpeed);

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

  public override void _Process(float delta)
  {
    if (Dead)
    {
      return;
    }

    Thirst -= delta;
    Hunger -= delta;

    if (Thirst <= 0 || Hunger <= 0)
    {
      Dead = true;
      _animationPlayer.Play("Die");
    }
  }

  public void FinalizeDeath()
  {
    this.QueueFree();
  }
}
