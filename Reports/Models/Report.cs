using System;
using System.Collections.Generic;

namespace Reports.Models
{
    public class Report
    {
        public List<Task.Task> Tasks { get; }

        public bool IsSaved { get; set; }
        public string Description { get; set; }
        public string Id { get; }

        public Report(string description)
        {
            Id = Guid.NewGuid().ToString();
            IsSaved = false;
            Description =  description;
            Tasks = new List<Task.Task>();
        }

        public override string ToString() => $"\nid {Id}\n is saved {IsSaved}\ndescription {Description}\n";
    }
}