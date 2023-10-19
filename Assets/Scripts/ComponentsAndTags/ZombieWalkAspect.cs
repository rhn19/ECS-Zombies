using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace ECSZombies
{
    public readonly partial struct ZombieWalkAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<ZombieWalkProperties> _zombieWalkProperties;
        private readonly RefRW<ZombieTimer> _zombieTimer;
        private readonly RefRO<ZombieHeading> _zombieHeading;

        public float WalkSpeed => _zombieWalkProperties.ValueRO.WalkSpeed;
        public float WalkAmplitude => _zombieWalkProperties.ValueRO.WalkAmplitude;
        public float WalkFrequency => _zombieWalkProperties.ValueRO.WalkFrequency;
        public float ZombieHeading => _zombieHeading.ValueRO.Value;
        public float ZombieTimer
        {
            get => _zombieTimer.ValueRO.Value;
            set => _zombieTimer.ValueRW.Value = value;
        }

        public void Walk(float deltaTime)
        {
            ZombieTimer += deltaTime;
            _transform.ValueRW.Position += _transform.ValueRO.Forward() * WalkSpeed * deltaTime;

            var swayAngle = WalkAmplitude * math.sin(ZombieTimer * WalkFrequency);
            _transform.ValueRW.Rotation = quaternion.Euler(0f, ZombieHeading, swayAngle);
        }

        public bool IsInStoppingRange(float3 brainPosition, float brainRadiusSq)
        {
            return math.distancesq(brainPosition, _transform.ValueRO.Position) <= brainRadiusSq;
        }
    }
}
