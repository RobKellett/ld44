using System;
using Godot;

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

    public static void CallGroup<T>(this SceneTree tree, Group<T> group, Action<T> action)
    {
      group.Call(tree, action);
    }
  }
}
