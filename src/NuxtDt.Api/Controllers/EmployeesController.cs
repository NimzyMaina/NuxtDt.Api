using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuxtDt.Api.Dtos;
using NuxtDt.Api.Extensions;
using NuxtDt.Api.Hubs;
using NuxtDt.Api.Models;
using NuxtDt.Api.Persistence;

namespace NuxtDt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IHubContext<EmployeesHub, IEmployeesHub> _hub;

        public EmployeesController(
            ApiContext context,
            IHubContext<EmployeesHub, IEmployeesHub> hub
            )
        {
            _context = context;
            _hub = hub;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromForm]IDataTablesRequest request)
        {
            int filteredDataCount = 0;
            List<Employees> data = await _context.Employees.ToListAsync();
            var dataPage = data.Compute(request, out filteredDataCount);
            var response = DataTablesResponse.Create(request, data.Count, filteredDataCount, dataPage);
            return new DataTablesJsonResult(response, true);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(UpdateEmployee model, int id)
        {
            // Validation failed ==> ApiController does this automatically
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
            
            var employee = await _context.Employees.FindAsync(id);

            // Employee not found
            if (employee is null)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Unable to find employee"
                });
            }
            
            // Map new values to domain model ==> for production use look into AutoMapper Library
            employee.Name = model.Name;
            employee.Age = model.Age;
            employee.Office = model.Office;
            employee.Position = model.Position;
            employee.Salary = model.Salary;
            employee.StartDate = model.StartDate;

            // Save changes to DB ==> for production look into Unit Of Work (UOW) pattern
            await _context.SaveChangesAsync();

            // Wait 4 secs so you can see live update
            await Task.Delay(4000);

            // Update all clients ==> for production look into groups & users to narrow down subscribers
            await _hub.Clients.All.EmployeeUpdated(employee);

            return Ok(new
            {
                Success = true,
                Message = "Employee Updated Successfully"
            });

        }
        
    }
}