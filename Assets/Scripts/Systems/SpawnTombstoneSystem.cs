using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace ECSZombies
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnTombstoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraveyardProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
            var graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);
            var ecb = GetEntityCommandBuffer(ref state);

            var builder = new BlobBuilder(Allocator.Temp);
            ref ZombieSpawnPoints zombieSpawnPoints = ref builder.ConstructRoot<ZombieSpawnPoints>();
            BlobBuilderArray<float3> arrayBuilder = builder.Allocate(ref zombieSpawnPoints.Value, graveyard.NumTombstones);

            float3 zombieOffset = new float3(0f, -2f, 1f);
            for(int i=0; i<graveyard.NumTombstones; i++)
            {
                var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);
                var newTombstoneTransform = graveyard.GetRandomTombstoneTransform();
                ecb.SetComponent(newTombstone, newTombstoneTransform);
                arrayBuilder[i] = new float3(newTombstoneTransform.Position + zombieOffset);
            }

            var result = builder.CreateBlobAssetReference<ZombieSpawnPoints>(Allocator.Persistent);
            graveyard.ZombieSpawnPoints = result;
            builder.Dispose();
        }

        private EntityCommandBuffer GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb;
        }
    }
}
