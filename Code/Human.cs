using Godot;
using System;
using System.Collections.Generic;
using LD44.Utilities;
using LD44.Resources;

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

  public const float SPEED = 100;
  public const float INTERACTION_RADIUS_SQUARED = 256;
  public const float MAX_THIRST = 30; // seconds
  public float Thirst = MAX_THIRST;
  public const float MAX_HUNGER = 50; // seconds
  public float Hunger = MAX_HUNGER;
  public bool Dead = false;

  public int Food = 0;
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
        Food--;
        Hunger = MAX_HUNGER;
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
              if (source.HasFood)
              {
                foodSources.Add(source);
              }
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
  }

  public void ResourceDestroyed(IResource resource, Human culprit)
  {
    if (resource == _targetResource)
    {
      _targetResource = null;
      SetAnimationState(AnimationState.Standing);
    }
  }

  public void FinalizeDeath()
  {
    this.QueueFree();
  }
}
