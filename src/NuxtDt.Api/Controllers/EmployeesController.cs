using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuxtDt.Api.Extensions;
using NuxtDt.Api.Models;
using NuxtDt.Api.Persistence;

namespace NuxtDt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApiContext _context;

        public EmployeesController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromForm]IDataTablesRequest request)
        {
            
            int filteredDataCount = 0;
            List<Employees> data = await _context.Employees.ToListAsync();
//            return Ok(data);
            var dataPage = data.Compute(request, out filteredDataCount);
            var response = DataTablesResponse.Create(request, data.Count, filteredDataCount, dataPage);
//            if (response is null)
//            {
//                return Ok("Response is null");
//            }
//            return Ok(JsonConvert.SerializeObject(response));

//            var icol =  request.Columns.Where(c => c.Sort != null).OrderBy(c => c.Sort.Order);

//            var order = icol.GetSort();

//            var filteredData = await _context.Employees.ToListAsync();
//
//            var dataPage = filteredData.Skip(request.Start).Take(request.Length);
//
//            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);
//
            return new DataTablesJsonResult(response, true);
        }
        
    }
}