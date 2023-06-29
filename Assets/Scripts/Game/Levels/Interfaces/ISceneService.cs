namespace Game.Levels.Interfaces
{
    public interface ISceneService
    {
        void LoadLevel();
        int CurrentLevel
        {
            get;
        }
    }
}