namespace uwpKarate.Components
{
    public interface IGameObjectComponent
    {
        //void Update();
    }

    public interface IGameObjectComponent<T> : IGameObjectComponent
    {
        void Update(T target);
    }
}