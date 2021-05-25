using uwpPlatformer.GameObjects;

namespace uwpPlatformer.EventArguments
{
    public struct ActivateDustParticles
    {
        public ActivateDustParticles(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public readonly GameObject GameObject;
    }
}
