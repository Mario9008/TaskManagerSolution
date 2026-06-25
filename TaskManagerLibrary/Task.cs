using System;

namespace TaskManagerLibrary
{
    public enum TaskStatus
    {
        Новая,
        ВПроцессе,
        Завершена
    }

    public enum TaskPriority
    {
        Низкий,
        Средний,
        Высокий
    }

    public class Task
    {
 
        public int Id { get; set; }
        public string Title { get; set; }          
        public string Description { get; set; }    
        public TaskPriority Priority { get; set; } 
        public DateTime Deadline { get; set; }     
        public TaskStatus Status { get; set; }     
        public bool IsImportant { get; set; }

        public TaskPriority TaskPriority
        {
            get => default;
            set
            {
            }
        }

        public TaskStatus TaskStatus
        {
            get => default;
            set
            {
            }
        }

        public Task(int id, string title, string description, TaskPriority priority, DateTime deadline, TaskStatus status, bool isImportant = false)
        {
            Id = id;
            Title = title;
            Description = description;
            Priority = priority;
            Deadline = deadline;
            Status = status;
            IsImportant = isImportant;
        }

    }
}