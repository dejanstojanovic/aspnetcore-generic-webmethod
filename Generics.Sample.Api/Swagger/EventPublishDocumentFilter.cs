using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Generics.Sample.Api.Extensions;

namespace Webhooks.Api.Swagger
{
    public class EventPublishDocumentFilter : IDocumentFilter
    {
        readonly IServiceScopeFactory _serviceScopeFactory;
        readonly IEnumerable<Type> _events;
        public EventPublishDocumentFilter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _events = this.GetType().GetEventTypesFromParentAssembly();
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            OpenApiMediaType getOpenApiMediaType(Type type)
            {
                // Get media type with example for the type
                return new OpenApiMediaType()
                {
                    Example = new OpenApiString(JsonConvert.SerializeObject(Activator.CreateInstance(type), Formatting.Indented)),
                    Schema = new OpenApiSchema
                    {
                        Type = "String"
                    }
                };
            }

            foreach (var @event in _events)
            {
                // Define sample payload
                swaggerDoc.Components.RequestBodies.Add(@event.FullName, new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            { "application/json-patch+json", getOpenApiMediaType(@event)},
                            { "application/json", getOpenApiMediaType(@event)},
                            { "text/json", getOpenApiMediaType(@event)},
                            { "application/*+json", getOpenApiMediaType(@event)}
                        }
                });
            }

            // Find route of the method
            var apiDescriptions = context.ApiDescriptions.Where(d => d.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase) &&
                                                                     d.RelativePath.Equals("events/{type}", StringComparison.InvariantCultureIgnoreCase) &&
                                                                     d.ParameterDescriptions.Any(p => p.Name.Equals("model", StringComparison.InvariantCultureIgnoreCase) &&
                                                                                                      p.Source.Id.Equals("Body", StringComparison.InvariantCultureIgnoreCase))
                                                                     );

            foreach (var apiDescription in apiDescriptions)
            {
                // Take into consideration version route segment if present
                var groupSegment = !String.IsNullOrWhiteSpace(apiDescription.GroupName) ? $"/{apiDescription.GroupName}" : String.Empty;
                foreach (var @event in _events)
                {
                    // Add new route
                    swaggerDoc.Paths.Add($"{groupSegment}/events/{@event.FullName}", new OpenApiPathItem()
                    {
                        Operations = new Dictionary<OperationType, OpenApiOperation>()
                        {
                            {
                                OperationType.Post, new OpenApiOperation()
                                {
                                    Tags = new List<OpenApiTag>(){
                                        new OpenApiTag() {Name="Events publishing" }
                                    },
                                    RequestBody = new OpenApiRequestBody()
                                    {
                                        Reference =  new OpenApiReference()
                                        {
                                            Type = ReferenceType.RequestBody,
                                            Id = $"components/requestBodies/{@event.FullName}",
                                            ExternalResource = ""
                                        }
                                    },
                                    Summary = $"Publishes {@event.FullName} event"
                                }
                            }
                        }
                    });
                }

                // Remove old route
                swaggerDoc.Paths.Remove($"{groupSegment}/{apiDescription.RelativePath}");
            }
        }

    }
}

