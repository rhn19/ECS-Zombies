using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace ECSZombies
{
    [BurstCompile]
    public partial struct SpawnZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecb = GetEntityCommandBuffer(ref state);

            state.CompleteDependency();
            new SpawnZombieJob
            {
                DeltaTime = deltaTime,
                Ecb = ecb
            }.Run();
        }

        private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }
    }

    [BurstCompile]
    public partial struct SpawnZombieJob: IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer Ecb;
        private void Execute(GraveyardAspect graveyard)
        {
            graveyard.ZombieSpawnTimer -= DeltaTime;
            if (!graveyard.TimeToSpawnZombie) return;
            if (!graveyard.ZombieSpawnPoints.IsCreated) return;
            if (graveyard.ZombieSpawnPoints.Value.Value.Length == 0) return;

            graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;
            var newZombie = Ecb.Instantiate(graveyard.ZombiePrefab);
            var newZombieTransform = graveyard.GetZombieSpawnPoint();
            Ecb.SetComponent(newZombie, newZombieTransform);

            var heading = MathHelpers.GetHeading(newZombieTransform.Position, graveyard.Position);
            Ecb.SetComponent(newZombie, new ZombieHeading { Value = heading });
        }
    }
}
