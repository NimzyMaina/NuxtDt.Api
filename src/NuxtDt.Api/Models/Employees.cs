using System;
using Newtonsoft.Json;

namespace NuxtDt.Api.Models
{
    public class Employees
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Office { get; set; }
        public int Age { get; set; }
        public string StartDate { get; set; }
        public string Salary { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}