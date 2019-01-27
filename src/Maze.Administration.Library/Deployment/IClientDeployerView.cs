namespace Maze.Administration.Library.Deployment
{
    public interface IClientDeployerView<in TClientDeployer> where TClientDeployer : IClientDeployer
    {
        void Initialize(TClientDeployer model);
    }
}