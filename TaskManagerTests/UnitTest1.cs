using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TaskManagerLibrary;

namespace TaskManagerTests
{
    [TestClass]
    public class TaskManagerTests
    {
        private TaskManager _taskManager;

        [TestInitialize]
        public void Setup()
        {
            _taskManager = new TaskManager();
        }

        [TestMethod]
        public void AddTask_ShouldIncreaseTaskCount()
        {
            int initialCount = _taskManager.Tasks.Count;

            _taskManager.AddTask("Тест", "Описание", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now.AddDays(7), TaskManagerLibrary.TaskStatus.Новая);

            Assert.AreEqual(initialCount + 1, _taskManager.Tasks.Count);
        }

        [TestMethod]
        public void AddTask_ShouldReturnTaskWithCorrectData()
        {
            var task = _taskManager.AddTask("Тест", "Описание", TaskManagerLibrary.TaskPriority.Высокий, DateTime.Now.AddDays(7), TaskManagerLibrary.TaskStatus.Новая);

            Assert.AreEqual("Тест", task.Title);
            Assert.AreEqual("Описание", task.Description);
            Assert.AreEqual(TaskManagerLibrary.TaskPriority.Высокий, task.Priority);
            Assert.AreEqual(TaskManagerLibrary.TaskStatus.Новая, task.Status);
        }

        [TestMethod]
        public void EditTask_ShouldUpdateTaskData()
        {
            var task = _taskManager.AddTask("Старое имя", "Старое описание", TaskManagerLibrary.TaskPriority.Низкий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);

            bool result = _taskManager.EditTask(task.Id, "Новое имя", "Новое описание", TaskManagerLibrary.TaskPriority.Высокий, DateTime.Now.AddDays(10), TaskManagerLibrary.TaskStatus.ВПроцессе, true);

            Assert.IsTrue(result);
            var updatedTask = _taskManager.Tasks.First(t => t.Id == task.Id);
            Assert.AreEqual("Новое имя", updatedTask.Title);
            Assert.AreEqual("Новое описание", updatedTask.Description);
            Assert.AreEqual(TaskManagerLibrary.TaskPriority.Высокий, updatedTask.Priority);
            Assert.AreEqual(TaskManagerLibrary.TaskStatus.ВПроцессе, updatedTask.Status);
            Assert.IsTrue(updatedTask.IsImportant);
        }

        [TestMethod]
        public void DeleteTask_ShouldRemoveTask()
        {
            var task = _taskManager.AddTask("Тест", "Описание", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            int countAfterAdd = _taskManager.Tasks.Count;

            bool result = _taskManager.DeleteTask(task.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(countAfterAdd - 1, _taskManager.Tasks.Count);
            Assert.IsFalse(_taskManager.Tasks.Any(t => t.Id == task.Id));
        }

        [TestMethod]
        public void FilterByStatus_ShouldReturnOnlyTasksWithSpecifiedStatus()
        {
            _taskManager.AddTask("Новая задача", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            _taskManager.AddTask("В процессе", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.ВПроцессе);
            _taskManager.AddTask("Завершена", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.Завершена);

            var filtered = _taskManager.FilterByStatus(TaskManagerLibrary.TaskStatus.ВПроцессе);

            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("В процессе", filtered[0].Title);
        }

        [TestMethod]
        public void Search_ShouldFindTasksByTitle()
        {
            _taskManager.AddTask("Важная задача", "Описание", TaskManagerLibrary.TaskPriority.Высокий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            _taskManager.AddTask("Обычная задача", "Другое", TaskManagerLibrary.TaskPriority.Низкий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            _taskManager.AddTask("Тест", "Текст", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.ВПроцессе);

            var result = _taskManager.Search("важная");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Важная задача", result[0].Title);
        }

        [TestMethod]
        public void Search_ShouldFindTasksByDescription()
        {
            _taskManager.AddTask("Задача 1", "Важное описание", TaskManagerLibrary.TaskPriority.Высокий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            _taskManager.AddTask("Задача 2", "Обычное", TaskManagerLibrary.TaskPriority.Низкий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);

            var result = _taskManager.Search("описание");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Задача 1", result[0].Title);
        }

        [TestMethod]
        public void SortByPriority_ShouldOrderTasksFromHighToLow()
        {
            _taskManager.AddTask("Низкая", "", TaskManagerLibrary.TaskPriority.Низкий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            _taskManager.AddTask("Высокая", "", TaskManagerLibrary.TaskPriority.Высокий, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);
            _taskManager.AddTask("Средняя", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая);

            var sorted = _taskManager.SortByPriority();

            Assert.AreEqual(TaskManagerLibrary.TaskPriority.Высокий, sorted[0].Priority);
            Assert.AreEqual(TaskManagerLibrary.TaskPriority.Средний, sorted[1].Priority);
            Assert.AreEqual(TaskManagerLibrary.TaskPriority.Низкий, sorted[2].Priority);
        }

        [TestMethod]
        public void GetStatistics_ShouldReturnCorrectCounts()
        {
            _taskManager.AddTask("Завершена 1", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now.AddDays(-1), TaskManagerLibrary.TaskStatus.Завершена);
            _taskManager.AddTask("Завершена 2", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now.AddDays(-2), TaskManagerLibrary.TaskStatus.Завершена);
            _taskManager.AddTask("В процессе", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now.AddDays(5), TaskManagerLibrary.TaskStatus.ВПроцессе);
            _taskManager.AddTask("Новая", "", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now.AddDays(10), TaskManagerLibrary.TaskStatus.Новая);

            var stats = _taskManager.GetStatistics();

            Assert.AreEqual(2, stats.Completed);
            Assert.AreEqual(4, stats.Total);
        }

        [TestMethod]
        public void DeleteTask_ShouldReturnFalseForNonExistentId()
        {
            bool result = _taskManager.DeleteTask(999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EditTask_ShouldReturnFalseForNonExistentId()
        {
            bool result = _taskManager.EditTask(999, "Имя", "Описание", TaskManagerLibrary.TaskPriority.Средний, DateTime.Now, TaskManagerLibrary.TaskStatus.Новая, false);

            Assert.IsFalse(result);
        }
    }
}