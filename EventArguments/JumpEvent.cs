using uwpPlatformer.GameObjects;

namespace uwpPlatformer.EventArguments
{
    public struct JumpEvent
    {
        public JumpEvent(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public readonly GameObject GameObject;
    }
}
