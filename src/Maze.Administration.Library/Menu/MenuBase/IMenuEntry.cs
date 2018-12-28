namespace Maze.Administration.Library.Menu.MenuBase
{
    public interface IMenuEntry<in TCommandEntry> where TCommandEntry : IVisibleMenuItem { }
}