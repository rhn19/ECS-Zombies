using Unity.Entities;
using Unity.Mathematics;

namespace ECSZombies
{
    public struct GraveyardProperties : IComponentData
    {
        public float2 FieldDimensions;
        public int NumTombstones;
        public Entity TombstonePrefab;
        public Entity ZombiePrefab;
        public float ZombieSpawnRate;
    }

    public struct ZombieSpawnTimer: IComponentData
    {
        public float Value;
    }
}