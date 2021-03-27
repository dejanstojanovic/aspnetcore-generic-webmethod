using Generics.Sample.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Generics.Sample.Api.Extensions;
using Newtonsoft.Json;

namespace Generics.Sample.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        readonly ISampleService _sampleService;

        public EventsController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        [HttpPost("{type}")]
        public async Task<ActionResult> PublishEvent(String type, Object model)
        {
            var eventType = this.GetType().GetEventTypesFromParentAssembly().SingleOrDefault(t => t.FullName == type);
            if (eventType == null)
                throw new ArgumentException($"{type} is not valid event type");
            var eventModel = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(model), eventType);

            var task =  (Task)_sampleService.GetType()
                            .GetMethod(nameof(ISampleService.ProcessEventModel))
                            .MakeGenericMethod(eventType).Invoke(_sampleService, new[] { eventModel });

            await task;
            return Accepted();
        }
    }
}
