using Unity.Entities;
using Unity.Mathematics;

namespace ECSZombies
{
    public struct GraveyardRandom : IComponentData
    {
        public Random Value;
    }
}