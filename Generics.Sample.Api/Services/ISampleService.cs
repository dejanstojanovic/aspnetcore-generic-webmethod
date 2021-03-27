using Generics.Sample.Api.Models;
using System.Threading.Tasks;

namespace Generics.Sample.Api.Services
{
    public interface ISampleService
    {
        Task ProcessEventModel<T>(T model) where T : class, IEventModel;
    }
}
