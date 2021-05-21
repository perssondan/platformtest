namespace uwpPlatformer.Utilities
{
    public class GameStateManager
    {
        public GameState GameState { get; private set; }

        public bool TryChangeState(GameStrateTriggers gameStrateTriggers)
        {
            switch (gameStrateTriggers)
            {
                case GameStrateTriggers.GameStarted:
                    GameState = GameState.GameRunning;
                    break;
                case GameStrateTriggers.GameOver:
                    GameState = GameState.GameOver;
                    break;
                case GameStrateTriggers.BackToStart:
                    GameState = GameState.StartScreen;
                    break;
            }

            return true;
        }

        public void GameOver()
        {
            GameState = GameState.GameOver;
        }
    }

    public enum GameState
    {
        StartScreen,
        GameRunning,
        GameOver
    }

    public enum GameStrateTriggers
    {
        GameStarted,
        GameOver,
        BackToStart
    }
}