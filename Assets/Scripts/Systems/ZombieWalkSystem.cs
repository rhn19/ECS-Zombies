using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSZombies
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieRiseSystem))]
    public partial struct ZombieWalkSystem : ISystem
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

            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            var brainRadius = brainScale * 0.5f + 0.5f;

            new WalkZombieJob
            {
                DeltaTime = deltaTime,
                Ecb = ecb,
                BrainRadiusSq = brainRadius * brainRadius
            }.ScheduleParallel();
        }

        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }

    [BurstCompile]
    public partial struct WalkZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public float BrainRadiusSq;
        private void Execute([EntityIndexInQuery] int sortKey, ZombieWalkAspect zombieWalk)
        {
            zombieWalk.Walk(DeltaTime);
            if (zombieWalk.IsInStoppingRange(float3.zero, BrainRadiusSq))
            {
                Ecb.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieWalk.Entity, false);
                Ecb.SetComponentEnabled<ZombieEatProperties>(sortKey, zombieWalk.Entity, true);
            }
        }
    }
}
