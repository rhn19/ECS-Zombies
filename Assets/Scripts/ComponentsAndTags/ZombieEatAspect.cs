using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace ECSZombies
{
    public readonly partial struct ZombieEatAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<ZombieTimer> _zombieTimer;
        private readonly RefRO<ZombieHeading> _zombieHeading;
        private readonly RefRO<ZombieEatProperties> _zombieEatProperties;

        public float EatDamagePerSecond => _zombieEatProperties.ValueRO.EatDamagePerSecond;
        public float EatAmplitude => _zombieEatProperties.ValueRO.EatAmplitude;
        public float EatFrequency => _zombieEatProperties.ValueRO.EatFrequency;
        public float ZombieHeading => _zombieHeading.ValueRO.Value;
        public float ZombieTimer
        {
            get => _zombieTimer.ValueRO.Value;
            set => _zombieTimer.ValueRW.Value = value;
        }

        public void Eat(float deltaTime, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity brainEntity)
        {
            ZombieTimer += deltaTime;
            var eatAngle = EatAmplitude * math.sin(EatFrequency * ZombieTimer);
            _transform.ValueRW.Rotation = quaternion.Euler(eatAngle, ZombieHeading, 0f);

            var eatDamage = EatDamagePerSecond * deltaTime;
            var curBrainDamage = new BrainDamageBufferElement { Value = eatDamage };
            ecb.AppendToBuffer(sortKey, brainEntity, curBrainDamage);
        }

        public bool isInEatingRange(float3 brainPosition, float brainRadiusSq)
        {
            return math.distancesq(brainPosition, _transform.ValueRO.Position) <= brainRadiusSq - 1;
        }
    }
}
