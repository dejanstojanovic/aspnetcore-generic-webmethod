using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Generics.Sample.Api.Models
{
    public class SampleEvent1Model:IEventModel
    {
        public Guid Id { get; set; }
        public String Description { get; set; }
        public DateTime DateTime { get; set; }

        public SampleEvent1Model()
        {
            Id = Guid.NewGuid();
            Description = "NO DESCRIPTION";
            DateTime = DateTime.UtcNow;
        }
    }
}
