using Generics.Sample.Api.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Generics.Sample.Api.Services
{
    public class SampleService : ISampleService
    {
        readonly ILogger<SampleService> _logger;
        public SampleService(ILogger<SampleService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessEventModel<T>(T model) where T:class, IEventModel
        {
            _logger.LogInformation($"Processed model of type {typeof(T).FullName}");
            await Task.CompletedTask;
        }
    }
}
