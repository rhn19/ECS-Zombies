using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace ECSZombies
{

    public class GraveyardAuthoring : MonoBehaviour
    {
        public float2 FieldDimensions;
        public int NumTombstones;
        public GameObject TombstonePrefab;
        public GameObject ZombiePrefab;
        public float ZombieSpawnRate;
        public uint RandomSeed;
    }

    public class GraveyardBaker : Baker<GraveyardAuthoring>
    {
        public override void Bake(GraveyardAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GraveyardProperties
            {
                FieldDimensions = authoring.FieldDimensions,
                NumTombstones = authoring.NumTombstones,
                TombstonePrefab = GetEntity(authoring.TombstonePrefab, TransformUsageFlags.None),
                ZombiePrefab = GetEntity(authoring.ZombiePrefab, TransformUsageFlags.None),
                ZombieSpawnRate = authoring.ZombieSpawnRate
            });

            AddComponent(entity, new GraveyardRandom
            {
                Value = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
            });

            AddComponent(entity, new ZombieSpawnPoints());

            AddComponent(entity, new ZombieSpawnTimer());

            AddComponent(entity, new SpawnPointsBlob());
        }
    }
}