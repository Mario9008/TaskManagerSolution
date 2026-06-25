using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json; 
using System.Text.Json.Serialization;

namespace TaskManagerLibrary
{
    public class TaskManager : Task
    {
        private List<Task> _tasks = new List<Task>();
        private int _nextId = 1;

        public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();

        public Task AddTask(string title, string description, TaskPriority priority, DateTime deadline, TaskStatus status, bool isImportant = false)
        {
            var newTask = new Task(_nextId++, title, description, priority, deadline, status, isImportant);
            _tasks.Add(newTask);
            return newTask;
        }

        public bool EditTask(int id, string title, string description, TaskPriority priority, DateTime deadline, TaskStatus status, bool isImportant)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.Title = title;
            task.Description = description;
            task.Priority = priority;
            task.Deadline = deadline;
            task.Status = status;
            task.IsImportant = isImportant;
            return true;
        }

        public bool DeleteTask(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            return _tasks.Remove(task);
        }

        public IReadOnlyList<Task> FilterByStatus(TaskStatus status)
        {
            return _tasks.Where(t => t.Status == status).ToList().AsReadOnly();
        }

        public IReadOnlyList<Task> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _tasks.AsReadOnly();

            return _tasks
                .Where(t => t.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            t.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<Task> SortByPriority()
        {
            return _tasks.OrderByDescending(t => t.Priority).ToList().AsReadOnly();
        }

        public IReadOnlyList<Task> SortByDeadline()
        {
            return _tasks.OrderBy(t => t.Deadline).ToList().AsReadOnly();
        }

        public (int Completed, int Overdue, int Total) GetStatistics()
        {
            int completed = _tasks.Count(t => t.Status == TaskStatus.Завершена);
            int overdue = _tasks.Count(t => t.Status != TaskStatus.Завершена && t.Deadline < DateTime.Now);
            int total = _tasks.Count;
            return (completed, overdue, total);
        }

        public void SaveToJson(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_tasks, options);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            var loadedTasks = JsonSerializer.Deserialize<List<Task>>(json);

            if (loadedTasks != null && loadedTasks.Any())
            {
                _tasks = loadedTasks;
                _nextId = _tasks.Max(t => t.Id) + 1;
            }
        }

        public void Clear()
        {
            _tasks.Clear();
            _nextId = 1;
        }
    }
}