using System.Threading.Tasks;

namespace Orcus.Modules.Api
{
    public interface IExecutableInterface<in TParam>
    {
        Task Execute(TParam parameter);
    }

    public interface IExecutableInterface<in TParam, in TParam2>
    {
        Task Execute(TParam parameter1, TParam2 parameter2);
    }

    public interface IExecutableInterface
    {
        Task Execute();
    }
}