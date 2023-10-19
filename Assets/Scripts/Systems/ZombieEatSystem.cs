using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSZombies
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieWalkSystem))]
    public partial struct ZombieEatSystem : ISystem
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
            var brainRadius = brainScale * 0.5f + 1f;

            new EatZombieJob
            {
                DeltaTime = deltaTime,
                Ecb = ecb,
                BrainEntity = brainEntity,
                BrainRadiusSq = brainRadius * brainRadius
            }.ScheduleParallel();
        }

        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }

    [BurstCompile]
    public partial struct EatZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public Entity BrainEntity;
        public float BrainRadiusSq;

        private void Execute([EntityIndexInQuery] int sortKey, ZombieEatAspect zombieEat)
        {
            if(zombieEat.isInEatingRange(float3.zero, BrainRadiusSq))
                zombieEat.Eat(DeltaTime, Ecb, sortKey, BrainEntity);
            else
            {
                Ecb.SetComponentEnabled<ZombieEatProperties>(sortKey, zombieEat.Entity, false);
                Ecb.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieEat.Entity, true);
            }
        }
    }
}
