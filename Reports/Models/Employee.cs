using System;
using System.Collections.Generic;

namespace Reports.Models
{
    public class Employee : IHaveId
    {
        public readonly string Name;
        public Employee Chief;
        public readonly List<Employee> Subordinates;
        public readonly List<Task.Task> Tasks;
        public Report Report;

        public Employee(string name, Employee chief = null, List<Employee> subordinates = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Employee name can't be null or empty");
            Name = name;
            Chief = chief;
            Subordinates = subordinates ?? new List<Employee>();
            Id = Guid.NewGuid().ToString();
            Tasks = new List<Task.Task>();
        }

        public string Id { get; }
        
        public bool IsTeamLead => Chief == null;

        public override string ToString() => $"\nid {Id}\nname {Name}";
    }
}