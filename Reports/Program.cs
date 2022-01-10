using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Reports.Controllers;
using Reports.Models;
using Reports.Models.Task;

namespace Reports
{
    internal static class Program
    {
        private static Socket _listen;
        private static EmployeeController _employeeController;
        private static TaskController _taskController;
        private static readonly Dictionary<string, Func<string[], string>> Routing = new Dictionary<string, Func<string[], string>>();

        private static void Main()
        {
            _employeeController = new EmployeeController();
            _taskController = new TaskController();
            InstallRoutingTable();
            _listen = InitListenSocket();
            Socket sendSocket = InitSendSocket();
            sendSocket.Send(Encoding.ASCII.GetBytes("from server"), SocketFlags.None);
            Socket acceptedSocket = _listen.Accept();
            byte[] receiveBuffer = new byte[1024];
            string msg = "";
            while (msg != "quit")
            {
                int receiveByteCount = acceptedSocket.Receive(receiveBuffer, SocketFlags.None);
                msg = ParseBytes(receiveBuffer, receiveByteCount);
                Console.WriteLine(msg);
                string[] commands = ParseRequest(msg);
                byte[] data = TransformToBytesResponse(commands);
                sendSocket.Send(data, SocketFlags.None);
            }
            acceptedSocket.Shutdown(SocketShutdown.Both);
            acceptedSocket.Close();
        }

        private static void InstallRoutingTable()
        {
            Routing.Add("Delete", Delete);
            Routing.Add("GetAllTaskOfWorker", GetAllTaskOfWorker);
            Routing.Add("GetTaskWithThisDateTime", GetTaskWithThisDateTime);
            Routing.Add("GetTaskById", GetTaskById);
            Routing.Add("ChangeChief", ChangeChief);
            Routing.Add("GetReportsOfSubordinates", GetReportsOfSubordinates);
            Routing.Add("GetAllTasksOfSubordinates", GetAllTasksOfSubordinates);
            Routing.Add("GetWorkers", GetWorkers);
            Routing.Add("FindWorkersWithDoneReports", FindWorkersWithDoneReports);
            Routing.Add("GetTasksWithLatestChanges", GetTasksWithLatestChanges);
            Routing.Add("GetAllReports", GetAllReports);
            Routing.Add("FindWorkersWithNotDoneReports", FindWorkersWithNotDoneReports);
            Routing.Add("AddTask", AddTask);
            Routing.Add("AddTaskToReport", AddTaskToReport);
            Routing.Add("UpdateReportDescription", UpdateReportDescription);
            Routing.Add("AssignTaskToWorker", AssignTaskToWorker);
            Routing.Add("AssignReport", AssignReport);
            Routing.Add("Create", Create);
            Routing.Add("AddCommentToTask", AddCommentToTask);
            Routing.Add("ChangeStateOfTask", ChangeStateOfTask);
            Routing.Add("ChangeAssignedWorkerOfTask", ChangeAssignedWorkerOfTask);
        }

        private static byte[] TransformToBytesResponse(string[] commands) => Encoding.ASCII.GetBytes(Routing.ContainsKey(commands[0]) ? Routing[commands[0]].Invoke(commands) : "wrong input");

        private static string ParseBytes(byte[] receiveBuffer, int receiveByteCount) => Encoding.ASCII.GetString(receiveBuffer, 0, receiveByteCount);

        private static Socket InitSendSocket()
        {
            var sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var destinationIp = IPAddress.Parse(LocalIpAddress());
            var destinationEndPoint = new IPEndPoint(destinationIp, 2084);
            Console.WriteLine("\nWaiting to Connect... ");
            sendSocket.Connect(destinationEndPoint);
            Console.WriteLine("Connected... ");
            return sendSocket;
        }

        private static Socket InitListenSocket()
        {
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            const int port = 2085; 
            var localIpEndPoint = new IPEndPoint(IPAddress.Any, port);
            listenSocket.Bind(localIpEndPoint); 
            Console.WriteLine(" Server IP Address : " + LocalIpAddress());
            Console.WriteLine(" Listening on Port : " + port);
            listenSocket.Listen(4);
            return listenSocket;
        }

        private static string ChangeAssignedWorkerOfTask(string[] commands)
        {
            if (commands[0] == null || commands[0] != "ChangeAssignedWorkerOfTask" || commands[1] == null ||
                commands[2] == null || commands[3] == null) return "wrong input";
            Employee newWorker = _employeeController.Find(commands[2]);
            if (newWorker == null)
            {
                return $"there is no such worker with id {commands[2]}";
            }
            Employee oldWorker = _employeeController.Find(commands[3]);
            if (oldWorker == null)
            {
                return $"there is no such worker with id {commands[3]}";
            }
            Task task = _taskController.FindById(commands[1]);
            if (task == null)
            {
                return "there is no such task";
            }
            _taskController.ChangeExecutor(task, oldWorker, newWorker);
            return "success";
        }

        private static string ChangeStateOfTask(string[] commands)
        {
            if (commands[0] == null ||
                commands[0] != "ChangeStateOfTask" || 
                commands[1] == null ||
                commands[2] == null ) 
                return "wrong input";
            Task task = _taskController.FindById(commands[1]);
            if (task == null)
            {
                return "there is no such task";
            }
            bool state = Enum.TryParse(commands[2], out TaskState taskState);
            if (!state)
            {
                return "there is no such type of task";
            }
            task.State = taskState;
            _taskController.UpdateData();
            return "success";
        }

        private static string AddCommentToTask(string[] commands)
        {
            if (commands[0] == null || commands[0] != "AddCommentToTask" || commands[1] == null ||
                commands[2] == null || commands[3] == null) return "wrong input";
            Task task = _taskController.FindById(commands[1]);
            if (task == null)
            {
                return "there is no such task";
            }
            Employee employee = _employeeController.Find(commands[3]);
            if (employee == null)
            {
                return "there is no such worker";
            }
            task.Add(employee.Name, commands[2]);
            _taskController.UpdateData();
            return "success";

        }

        private static string Create(string[] commands)
        {
            string err = null;
            if (commands[0] != null && 
                commands[0] == "Create" && 
                commands[1] != null && 
                commands[2] != null &&
                _employeeController.Add(commands[1], commands[2], out err))
                return "success";

            return err ?? "wrong input";
        }

        private static string AssignReport(string[] commands)
        {
            if (commands[0] == null || commands[0] != "AssignReport" || commands[1] == null || commands[2] == null)
                return "wrong input";
            Report report = _employeeController.GetAllReports().ToList().Find(r => r.Id == commands[1]);
            if (report == null)
            {
                return "there is no such report";
            }

            Employee employee = _employeeController.Find(commands[3]);
            if (employee == null)
            {
                return "there is no such worker";
            }

            employee.Report = report;
            _employeeController.UpdateData();
            return "success";

        }

        private static string AssignTaskToWorker(string[] commands)
        {
            if (commands[0] != "AssignTaskToWorker" || commands[1] == null || commands[2] == null) return "wrong input";
            Task task = _taskController.FindById(commands[1]);
            if (task == null)
            {
                return "there is no such task";
            }
            Employee employee = _employeeController.Find(commands[3]);
            if (employee == null)
            {
                return "there is no such worker";
            }
            _taskController.AssignTaskToEmployee(task, employee);
            return "success";
        }

        private static string UpdateReportDescription(string[] commands)
        {
            if (commands[0] != "UpdateReportDescription" || commands[1] == null || commands[2] == null)
                return "There is no such report";
            Report report = _employeeController.GetAllReports().ToList().Find(r => r.Id == commands[1]);
            if (report == null) return "There is no such report";
            report.Description = commands[2];
            _employeeController.UpdateData();
            return "success";
        }

        private static string AddTaskToReport(string[] commands)
        {
            if (commands[0] != "AddTaskToReport" || commands[1] == null || commands[2] == null)
                return "there is no report with such id";
            Report report = _employeeController.GetAllReports().ToList().Find(r => r.Id == commands[2]);
            if (report == null) return "there is no report with such id";
            Task task = _taskController.Add(commands[1]);
            report.Tasks.Add(task);
            _employeeController.UpdateData();
            return "success";
        }

        private static string AddTask(string[] commands)
        {
            if (commands[0] != "AddTask" || commands[1] == null || commands[2] == null) return "wrong input";
            _taskController.Add(commands[1]);
            return "success";
        }

        private static string FindWorkersWithNotDoneReports(string[] commands)
        {
            string response = "";
            if (commands[0] != "FindWorkersWithNotDoneReports") return "All reports are done";
            IEnumerable<Employee> employees = _employeeController.FindEmployeesWithNotDoneReports().ToList();
            if (!employees.Any()) return "All reports are done";
            foreach (Employee employee in employees)
            {
                string id = employee.Id;
                string name = employee.Name;
                response += "\n" + "Name" + name + "\n" + "ID" + id + "\n";
            }

            return response;

        }

        private static string GetAllReports(string[] commands)
        {
            if (commands[0] != "GetAllReports") return "there are no reports";
            IEnumerable<Report> reports = _employeeController.GetAllReports();
            return reports == null 
                ? "there are no reports" 
                : reports.Aggregate("", (current, report) => current + $"\nid {report.Id}\nis saved {report.IsSaved}\ndescription {report.Description}");
        }

        private static string GetTasksWithLatestChanges(string[] commands)
        {
            string response = "";
            if (commands[0] != "GetTasksWithLatestChanges") return "there are no tasks";
            IEnumerable<Task> tasks = _taskController.GetTasksWithLatestChanges();
            return tasks == null ? "there are no tasks" : tasks.Aggregate(response, (current, task) => current + task);
        }

        private static string FindWorkersWithDoneReports(string[] commands)
        {
            string response = "";
            if (commands[0] != "FindWorkersWithDoneReports") return "There are no workers with done reports";
            IEnumerable<Employee> employees = _employeeController.FindEmployeesWithDoneReports();
            if (employees == null) return "There are no workers with done reports";
            foreach (Employee employee in employees)
            {
                string id = employee.Id;
                string name = employee.Name;
                response += "\n" + "Name" + name + "\n" + "ID" + id + "\n";
            }

            return response;

        }

        private static string GetWorkers(string[] commands)
        {
            if (commands[0] != "GetWorkers") return "There are no workers";
            IEnumerable<Employee> workers = _employeeController.GetWorkers();
            return workers == null ? "There are no workers" : workers.Aggregate("", (current, employee) => current + employee);
        }

        private static string GetAllTasksOfSubordinates(string[] commands)
        {
            string response = "";
            if (commands[0] != "GetAllTasksOfSubordinates" || commands[1] == null)
                return $"there are no tasks of subordinates of worker {commands[1]}";
            var tasks = EmployeeController.GetTasksOfSubordinates(_employeeController.Find(commands[1])).ToList();
            return tasks.Count != 0 
                ? tasks.Aggregate(response, (current, task) => current + task) 
                : $"there are no tasks of subordinates of worker {commands[1]}";
        }

        private static string GetReportsOfSubordinates(string[] commands)
        {
            string response = "";
            if (commands[0] != "GetReportsOfSubordinates" || commands[1] == null) return "there are no reports";
            var reports = EmployeeController.GetReportsOfSubordinates(_employeeController.Find(commands[1])).ToList();
            return reports.Count <= 0 
                ? "there are no reports" 
                : reports.Aggregate(response, (current, report) => current + report);
        }

        private static string GetTaskById(string[] commands)
        {
            if (commands[0] != "GetTaskById" || commands[1] == null) return "there is no such task";
            Task task = _taskController.FindById(commands[1]);
            if (task == null) return "there is no such task";
            string response = task.ToString();
            return response;
        }

        private static string ChangeChief(string[] commands)
        {
            if (commands[0] != "ChangeChief" || commands[1] == null || commands[2] == null) return "invalid response";
            _employeeController.ChangeChief(commands[1], commands[2], out string err);
            return err ?? "success";
        }

        private static string GetTaskWithThisDateTime(string[] commands)
        {
            if (commands[0] != "GetTaskWithThisDateTime" || commands[1] == null) return "check data or there is no such task";
            string response = "";
            IEnumerable<Task> tasks = _taskController.FindByDateOfCreation(commands[1]).ToList();
            return !tasks.Any() ? "check data or there is no such task" : tasks.Aggregate(response, (current, task) => current + task);
        }

        private static string GetAllTaskOfWorker(string[] commands)
        {
            string response = "";
            if (commands[0] != "GetAllTaskOfWorker" || commands[1] == null) return "There are no tasks";
            IEnumerable<Task> tasks = _employeeController.GetAllTasks(commands[1]);
            return tasks == null ? "There are no tasks" : tasks.Aggregate(response, (current, task) => current + task);
        }

        private static string Delete(string[] commands)
        {
            if (commands[0] != "Delete" || commands[1] == null) return "there is no such worker";
            if (!_employeeController.Remove(commands[1])) return "there is no such worker";
            _taskController.Remove(_employeeController.Find(commands[1]));
            return $"workers with id {commands[1]} deleted";

        }

        private static string LocalIpAddress()
        {
            string localIp = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName()); 
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork) continue;
                localIp = ip.ToString();
                break;
            }

            return localIp;
        }

        private static string[] ParseRequest(string request)
        {
            string[] commands = request.Split(' ');
            return commands;
        }
    }
}