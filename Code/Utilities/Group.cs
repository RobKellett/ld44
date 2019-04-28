using System;
using Godot;
using LD44.Resources;

namespace LD44.Utilities
{
  public class Group<TGroupMember>
  {
    public string Name { get; }

    internal Group(string name)
    {
      Name = name;
    }

    public void Add<T>(T newMember) where T : Node, TGroupMember
    {
      newMember.AddToGroup(Name);
    }

    public void Call(SceneTree tree, Action<TGroupMember> action)
    {
      var nodes = tree.GetNodesInGroup(Name);
      foreach (var node in nodes)
      {
        if (node is TGroupMember)
        {
          var castedNode = (TGroupMember)node;
          action(castedNode);
        }
      }
    }
  }

  public static class Group
  {
    // To define a group:
    // public static Group<IGroupMember> GroupName = new Group<IGroupMember>("GroupName");
    public static Group<ICareAboutMapUpdates> MapUpdates = new Group<ICareAboutMapUpdates>(nameof(ICareAboutMapUpdates));
    public static Group<IWaterSource> WaterSources = new Group<IWaterSource>(nameof(IWaterSource));
    public static Group<IFoodSource> FoodSources = new Group<IFoodSource>(nameof(IFoodSource));
    public static Group<Human> Humans = new Group<Human>(nameof(Human));

    public static void CallGroup<T>(this SceneTree tree, Group<T> group, Action<T> action)
    {
      group.Call(tree, action);
    }
  }
}
