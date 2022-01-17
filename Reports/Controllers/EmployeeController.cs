using System;
using System.Collections.Generic;
using System.Linq;
using Reports.Models;
using Reports.Models.Task;
using Reports.Repository;

namespace Reports.Controllers
{
    public class EmployeeController
    {
        private const string NoChief = "-1";
        private const string RepoPath = "/home/sergei/RiderProjects/Codeman-from-unknown-city/Reports/employess.json";
        private readonly Repository<Employee> _repository;
        private readonly List<Employee> _employees;

        public EmployeeController()
        {
            _repository = new Repository<Employee>(RepoPath);
            _employees = _repository.Load().ToList();
        }

        public bool Remove(string id)
        {
            Employee employee = Find(id);
            if (employee == null)
                return false;
            employee.Chief?.Subordinates.Remove(employee);
            _employees.Remove(employee);
            UpdateData();
            return true;
        }

        public bool Add(string name, string chiefId, out string err)
        {
            err = null;
            Employee employee;
            if (chiefId == NoChief)
            {
                employee = new Employee(name);
                Employee currTeamLead = FindTeamLead();
                if (currTeamLead != null)
                {
                    employee.Subordinates.Add(currTeamLead);
                    currTeamLead.Chief = employee;
                }
                _employees.Add(employee);
                UpdateData();
                return true;
            }
            Employee chief = Find(chiefId);
            if (chief == null)
            {
                err = "Invalid chief id";
                return false;
            }
            employee = new Employee(name, chief);
            chief.Subordinates.Add(employee);
            _employees.Add(employee);
            UpdateData();
            return true;
        }

        public bool ChangeChief(string employeeId, string newChiefId, out string err)
        { 
            err = null;
            Employee employee = Find(employeeId);
            if (employee == null)
            {
                err = "Invalid employee id";
                return false;
            }
            Employee newChief = Find(newChiefId);
            if (newChief == null)
            {
                err = "Invalid chief id";
                return false;
            }

            employee.Chief?.Subordinates.Remove(employee);
            employee.Chief = newChief;
            newChief.Subordinates.Add(employee);
            UpdateData();
            return true;
        }

        public IEnumerable<Task> GetAllTasks(string employeeId) => Find(employeeId)?.Tasks;

        public IEnumerable<Report> GetAllReports()
        {
            var reports = new List<Report>();
            _employees.ForEach(employee => reports.Add(employee.Report));
            return reports;
        }

        public IEnumerable<Employee> FindEmployeesWithDoneReports() => _employees.FindAll(employee => employee.Report.IsSaved);
        public IEnumerable<Employee> FindEmployeesWithNotDoneReports() => _employees.FindAll(employee => !employee.Report.IsSaved);
        public IEnumerable<Employee> GetWorkers() => _employees.FindAll(employee => !employee.IsTeamLead);

        public static IEnumerable<Task> GetTasksOfSubordinates(Employee employee)
        {
            var tasks = new List<Task>();
            foreach (Employee employeeSubordinate in employee.Subordinates)
                tasks.AddRange(GetTasksOfSubordinates(employeeSubordinate));
            return tasks;
        }

        public static IEnumerable<Report> GetReportsOfSubordinates(Employee employee)
        {
            var reports = new List<Report>();
            foreach (Employee employeeSubordinate in employee.Subordinates)
                reports.AddRange(GetReportsOfSubordinates(employeeSubordinate));
            return reports;
        }

        public Employee Find(string id) => Find(employee => employee.Id == id);
        private Employee FindTeamLead() => Find(employee => employee.IsTeamLead);
        private Employee Find(Predicate<Employee> match) => _employees.Find(match);
        public void UpdateData() => _repository.Save(_employees);
    }
}