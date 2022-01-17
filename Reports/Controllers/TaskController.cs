using System;
using System.Collections.Generic;
using System.Linq;
using Reports.Models;
using Reports.Models.Task;
using Reports.Repository;

namespace Reports.Controllers
{
    public class TaskController
    {
        private const string RepoPath = "/home/sergei/RiderProjects/Codeman-from-unknown-city/Reports/tasks.json";
        private readonly Repository<Task> _repository;
        private readonly List<Task> _tasks;

        public TaskController()
        {
            _repository = new Repository<Task>(RepoPath);
            _tasks = _repository.Load().ToList();
        }

        public void Remove(Employee executor)
        {
            _tasks.FindAll(task => task.Contains(executor)).ForEach(task => task.Remove(executor));
            UpdateData();
        }

        public Task Add(string name)
        {
            var task = new Task(name);
            _tasks.Add(task);
            UpdateData();
            return task;
        }
        
        public IEnumerable<Task> GetTasksWithLatestChanges()
        {
            IOrderedEnumerable<Task> orderedByDateTasks = from task in _tasks
                orderby Convert.ToDateTime(task.LastTimeChanged)
                select task;
            return orderedByDateTasks.Reverse().ToList();
        }

        public void ChangeExecutor(Task task, Employee oldExecutor, Employee newExecutor)
        {
            task.Remove(oldExecutor);
            oldExecutor.Tasks.Remove(task);
            if (!task.Contains(newExecutor))
                task.Add(newExecutor);
            if (!newExecutor.Tasks.Contains(task))
                newExecutor.Tasks.Add(task);
            UpdateData();
        }

        public void AssignTaskToEmployee(Task task, Employee employee)
        {
            task.Add(employee);
            employee.Tasks.Add(task);
            UpdateData();
        }

        public Task FindById(string id) => Find(task => task.Id == id);
        public IEnumerable<Task> FindByDateOfCreation(string date) => _tasks.FindAll(task => task.DateOfCreation == date);
        private Task Find(Predicate<Task> predicate) => _tasks.Find(predicate);
        public void UpdateData() => _repository.Save(_tasks);
    }
}