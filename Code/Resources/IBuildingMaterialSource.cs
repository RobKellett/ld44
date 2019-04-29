using System;

namespace LD44.Resources
{
  public interface IBuildingMaterialSource : IResource
  {
    bool HasBuildingMaterial { get; }
    void TakeBuildingMaterial(Human human);
  }
}