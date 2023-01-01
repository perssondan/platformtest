namespace uwpPlatformer.EventArguments
{
    public struct SplashSceneEvent
    {
        public SplashSceneEvent(bool isFinished)
        {
            IsFinished = isFinished;
        }

        public bool IsFinished;
    }
}
