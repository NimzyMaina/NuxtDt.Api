using System.Threading.Tasks;
using NuxtDt.Api.Models;

namespace NuxtDt.Api.Hubs
{
    public interface IEmployeesHub
    {
        Task EmployeeUpdated(Employees employee);
    }
}