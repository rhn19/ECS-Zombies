using Unity.Entities;

namespace ECSZombies
{
    public struct BrainHealth : IComponentData
    {
        public float Value;
        public float Max;
    }
}
