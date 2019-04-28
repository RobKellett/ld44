using System;

public static class Aggression
{
  public enum Level
  {
    Friendly,
    Neutral,
    Aggressive,
    Murderous
  }

  static int[,] aggressionTable = new int[3,3] {
    {  0, 20, 20 },
    { 20,  0, 20 },
    { 20, 20,  0 }
  };

  public static Level Get(Human.Team myTeam, Human.Team theirTeam)
  {
    if (myTeam == theirTeam)
    {
      return Level.Friendly;
    }

    var aggression = aggressionTable[(int)myTeam, (int)theirTeam];

    if (aggression <= 0)
    {
      return Level.Friendly;
    }

    if (aggression < 50)
    {
      return Level.Neutral;
    }

    if (aggression < 100)
    {
      return Level.Aggressive;
    }

    return Level.Murderous;
  }

  public static void Adjust(Human.Team myTeam, Human.Team theirTeam, int amount)
  {
    if (myTeam == theirTeam)
    {
      return;
    }

    var aggression = aggressionTable[(int)myTeam, (int)theirTeam];
    aggression += amount;
    if (aggression < 0)
    {
      aggression = 0;
    }

    if (aggression > 100)
    {
      aggression = 100;
    }

    aggressionTable[(int)myTeam, (int)theirTeam] = aggression;
  }
}