using System;
using System.Collections.Generic;
using System.Globalization;

namespace Reports.Models.Task
{
    public class Task : IHaveId
    {
        private TaskState _state;
        private readonly List<Employee> _executors;

        public Task(string name, List<Employee> executors = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Task name can't be null or empty");
            Name = name;
            _executors = executors ?? new List<Employee>();
            Id = Guid.NewGuid().ToString();
            DateOfCreation = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            AddRecordAboutChange("The task is created");
        }

        public string Id { get; }

        public TaskState State
        {
            get => _state;
            set
            {
                if (value == _state) return;
                AddRecordAboutChange($"the state is changed to {value}");
                _state = value;
            }
        }

        private string Name { get; }
        private string Comments { get; set; }
        private string Changes { get; set; }
        public string LastTimeChanged { get; private set; }
        public string DateOfCreation { get; }

        public void Add(Employee executor)
        {
            if (Contains(executor))
                return;
            AddRecordAboutChange($"{executor.Name} is assigned to the task");
            _executors.Add(executor);
        }

        public void Remove(Employee executor)
        {
            if (_executors.Remove(executor))
                AddRecordAboutChange($"{executor.Name} is deleted from the task");
        }

        public bool Contains(Employee employee) => _executors.Contains(employee);

        public void Add(string author, string comment)
        {
            AddRecordAboutChange($"{author} added a comment");
            Comments += $"author: {author}, comment: {comment}\n";
        }

        public override string ToString() => $"\nName {Name}\nID {Id}\nLast time of change {LastTimeChanged}\n";

        private void AddRecordAboutChange(string message)
        {
            string now = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            Changes += $"[{now}] {message}\n";
            LastTimeChanged = now;
        }
    }
}