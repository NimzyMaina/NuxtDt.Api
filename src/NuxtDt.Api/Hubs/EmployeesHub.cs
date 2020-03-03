using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace NuxtDt.Api.Hubs
{
    public class EmployeesHub : Hub<IEmployeesHub>
    {
        private readonly ILogger<EmployeesHub> _logger;

        public EmployeesHub(ILogger<EmployeesHub> logger)
        {
            _logger = logger;
        }
    }
}