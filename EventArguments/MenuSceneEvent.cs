namespace uwpPlatformer.EventArguments
{
    public struct MenuSceneEvent
    {
        public MenuSceneEvent(bool startGame)
        {
            StartGame = startGame;
        }

        public bool StartGame;
    }
}
