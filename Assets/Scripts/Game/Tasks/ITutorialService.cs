namespace Game.Tasks
{
    public interface ITutorialService
    {
        void Close();
        void CloseDown();
        void OpenUp(string value);
        void OpenDown(string value);
    }
}