using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace ECSZombies
{
    public readonly partial struct ZombieRiseAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;

        private readonly RefRO<ZombieRiseRate> _zombieRiseRate;
        public float ZombieRiseRate => _zombieRiseRate.ValueRO.Value;

        public void Rise(float deltaTime)
        {
            _transform.ValueRW.Position += math.up() * ZombieRiseRate * deltaTime;
        }

        public bool isAboveGround => _transform.ValueRW.Position.y >= 0f;
        public void SetAtGroundLevel()
        {
            var position = _transform.ValueRO.Position;
            position.y = 0f;
            _transform.ValueRW.Position = position;
        }
    }
}
