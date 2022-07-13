using Infrastructure.Modules.Applications.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Applications.Requests
{
    public class ApplicationUpdateStatusRequest
    {
        public ApplicationStatus? ApplicationStatus { get; set; }
    }
}
