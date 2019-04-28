using Godot;
using System;
using System.Collections.Generic;
using LD44.Utilities;
using LD44.Resources;

public class Human : Node2D, IFoodSource
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

  public const float SPEED = 100;
  public const float INTERACTION_RADIUS_SQUARED = 256;
  public const float MAX_THIRST = 30; // seconds
  public float Thirst = MAX_THIRST;
  public const float MAX_HUNGER = 50; // seconds
  public float Hunger = MAX_HUNGER;
  public bool Dead = false;

  public int Food = 0;
  public bool HasFood { get { return !Dead && Food > 0; } }
  public int BuildingMaterials = 0;

  private IResource _targetResource = null;
  private Vector2 _targetPosition = Vector2.Zero;
  private float _breatheSpeed;

  private enum AnimationState
  {
    Standing,
    Walking,
    Dying
  }
  private AnimationState _animationState = AnimationState.Dying;

  public override void _Ready()
  {
    var sprite = GetChild<ResourceSprite>(0);
    sprite.LoadRandomTexture("human", 3);

    _clothesSprite = sprite.GetChild<Sprite>(0);
    _animationPlayer = GetChild<AnimationPlayer>(1);

    _breatheSpeed = (float)RNG.Instance.NextDouble() / 10f + 0.95f;
    SetAnimationState(AnimationState.Standing);

    Group.Humans.Add(this);
    Group.FoodSources.Add(this);

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

    if (Thirst < MAX_THIRST / 2)
    {
      var waterSource = _targetResource as IWaterSource;
      if (waterSource == null || !waterSource.HasWater)
      {
        // Seek water
        var waterSources = new List<IWaterSource>();
        Group.CallGroup(GetTree(), Group.WaterSources, source =>
          {
            if (source.HasWater)
            {
              waterSources.Add(source);
            }
          });
        TargetClosestResource(waterSources);
        waterSource = _targetResource as IWaterSource;
      }

      if (waterSource != null && waterSource.HasWater && Position.DistanceSquaredTo(_targetPosition) <= INTERACTION_RADIUS_SQUARED)
      {
        waterSource.TakeWater(this);
        _targetResource = null;
      }
    }
    else if (Hunger < MAX_HUNGER / 2)
    {
      if (Food > 0)
      {
        // If we have food, eat
        Eat();
      }
      else
      {
        // Otherwise, seek food
        var foodSource = _targetResource as IFoodSource;
        if (foodSource == null || !foodSource.HasFood)
        {
          var foodSources = new List<IFoodSource>();
          Group.CallGroup(GetTree(), Group.FoodSources, source =>
            {
              if (!source.HasFood)
              {
                return;
              }

              var humanSource = source as Human;
              if (humanSource != null)
              {
                if (!ShouldTargetHuman<IFoodSource>(humanSource))
                {
                  return;
                }
              }

              foodSources.Add(source);
            });
          TargetClosestResource(foodSources);
          foodSource = _targetResource as IFoodSource;
        }

        if (foodSource != null && foodSource.HasFood && Position.DistanceSquaredTo(_targetPosition) <= INTERACTION_RADIUS_SQUARED)
        {
          foodSource.TakeFood(this);
          _targetResource = null;
        }
      }
    }

    if (_targetResource != null)
    {
      SetAnimationState(AnimationState.Walking);
      var velocity = (_targetPosition - Position).Normalized() * SPEED * delta;
      Position += velocity;
    }
    else
    {
      SetAnimationState(AnimationState.Standing);
    }

    if (Thirst <= 0 || Hunger <= 0)
    {
      Dead = true;
      SetAnimationState(AnimationState.Dying);
    }
  }

  private void TargetClosestResource(IEnumerable<IResource> resources)
  {
    IResource closestResource = null;
    var closestDistanceSquared = Single.PositiveInfinity;
    var closestPoint = Vector2.Zero;

    foreach (var resource in resources)
    {
      var resourcePos = resource.GetClosestPosition(Position);
      var resourceDistanceSquared = Position.DistanceSquaredTo(resourcePos);
      if (resourceDistanceSquared < closestDistanceSquared)
      {
        closestDistanceSquared = resourceDistanceSquared;
        closestPoint = resourcePos;
        closestResource = resource;
      }
    }

    _targetResource = closestResource;
    _targetPosition = closestPoint;
  }

  private void SetAnimationState(AnimationState animationState)
  {
    if (_animationState != animationState)
    {
      if (animationState == AnimationState.Standing)
      {
        _animationPlayer.Play("Stand", -1, _breatheSpeed);
      }
      else if (animationState == AnimationState.Walking)
      {
        _animationPlayer.Play("Walk");
      }
      else if (animationState == AnimationState.Dying)
      {
        _animationPlayer.Play("Die");
      }

      _animationState = animationState;
    }
  }

  public void Drink()
  {
    Thirst = MAX_THIRST;
  }

  public void Feed(int portions)
  {
    Food += portions;
    if (Food > 0 && Hunger <= MAX_HUNGER / 2)
    {
      Eat();
    }
  }

  private void Eat()
  {
    Food--;
    Hunger = MAX_HUNGER;
  }

  public void ResourceDestroyed(IResource resource, Human culprit)
  {
    if (resource == _targetResource)
    {
      _targetResource = null;
      SetAnimationState(AnimationState.Standing);
      Aggression.Adjust(_team, culprit._team, 5);
    }
  }

  public Vector2 GetClosestPosition(Vector2 yourPosition)
  {
    return Position;
  }

  public void TakeFood(Human taker)
  {
    if (Aggression.Get(_team, taker._team) == Aggression.Level.Friendly)
    {
      // Sharing is caring
      if (HasFood)
      {
        Food--;
        taker.Feed(1);
        GD.Print("Some friends shared!");
      }
    }
    else
    {
      // I have been killed
      Dead = true;
      SetAnimationState(AnimationState.Dying);
      taker.Feed(Food);
      Food = 0;
      // I shall be avenged
      Aggression.Adjust(_team, taker._team, 15);
      GD.Print("There was a MURDER!");
    }
  }

  private bool ShouldTargetHuman<T>(Human otherHuman) where T : IResource
  {
    if (otherHuman.Dead)
    {
      return false;
    }

    var isFoodSource = typeof(T) == typeof(IFoodSource);
    var aggression = Aggression.Get(_team, otherHuman._team);

    if (aggression == Aggression.Level.Neutral)
    {
      // Only if I'm starving
      return isFoodSource && Hunger <= 5;
    }

    if (aggression == Aggression.Level.Aggressive)
    {
      // Only if I'm hungry
      return isFoodSource && Hunger <= MAX_HUNGER / 2;
    }

    // If we're friendly we'll share, if we're murderous we'll kill
    return true;
  }

  public void FinalizeDeath()
  {
    this.QueueFree();
  }
}
