using Unity.Entities;
using Unity.Burst;

namespace ECSZombies
{
    [BurstCompile]
    [UpdateAfter(typeof(SpawnZombieSystem))]
    public partial struct ZombieRiseSystem : ISystem
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

            new RiseZombieJob
            {
                DeltaTime = deltaTime,
                Ecb = ecb
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
    public partial struct RiseZombieJob: IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;
        private void Execute([EntityIndexInQuery]int sortKey, ZombieRiseAspect zombieRise)
        {
            if (!zombieRise.isAboveGround)
                zombieRise.Rise(DeltaTime);
            else
            {
                zombieRise.SetAtGroundLevel();
                Ecb.RemoveComponent<ZombieRiseRate>(sortKey, zombieRise.Entity);
                Ecb.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieRise.Entity, true);
            }                
        }
    }
}
