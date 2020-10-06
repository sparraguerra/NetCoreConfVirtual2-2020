using System.Threading.Tasks;

namespace Saint.Ikki.Fx.ApplicationServices.Abstract
{
    public interface IAsyncInitialization
    {
        /// <summary>
        /// The result of the asynchronous initialization of this instance.
        /// </summary>
        Task InitializationTask { get; }
    }
}
