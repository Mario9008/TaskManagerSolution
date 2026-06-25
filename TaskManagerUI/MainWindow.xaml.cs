using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManagerLibrary;

namespace TaskManagerUI
{
    public partial class MainWindow : Window
    {
        private TaskManager _taskManager = new TaskManager();
        private TaskManagerLibrary.Task _selectedTask = null;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _taskManager.LoadFromJson("tasks.json");
            }
            catch { /* Если файла нет - просто начинаем с пустого списка */ }

            RefreshTaskList();
            UpdateStatistics();
        }

        private void RefreshTaskList(IEnumerable<TaskManagerLibrary.Task> tasks = null)
        {
            if (tasks == null)
                tasks = _taskManager.Tasks;

            lvTasks.ItemsSource = tasks;
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var stats = _taskManager.GetStatistics();
            txtStats.Text = $"📊 Всего: {stats.Total} | ✅ Завершено: {stats.Completed} | ⚠️ Просрочено: {stats.Overdue}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название задачи!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var priority = GetPriorityFromComboBox(cmbPriority);
            var status = GetStatusFromComboBox(cmbStatus);

            _taskManager.AddTask(
                txtTitle.Text,
                txtDescription.Text,
                priority,
                dpDeadline.SelectedDate ?? DateTime.Now,
                status,
                chkImportant.IsChecked ?? false
            );

            ClearInputs();
            RefreshTaskList();
            SaveToFile();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Выберите задачу для редактирования!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название задачи!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var priority = GetPriorityFromComboBox(cmbPriority);
            var status = GetStatusFromComboBox(cmbStatus);

            _taskManager.EditTask(
                _selectedTask.Id,
                txtTitle.Text,
                txtDescription.Text,
                priority,
                dpDeadline.SelectedDate ?? DateTime.Now,
                status,
                chkImportant.IsChecked ?? false
            );

            ClearInputs();
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            _selectedTask = null;
            RefreshTaskList();
            SaveToFile();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            _selectedTask = null;
        }

        private void LvTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvTasks.SelectedItem is TaskManagerLibrary.Task task)
            {
                _selectedTask = task;
                txtTitle.Text = task.Title;
                txtDescription.Text = task.Description;
                cmbPriority.SelectedIndex = (int)task.Priority;
                cmbStatus.SelectedIndex = (int)task.Status;
                dpDeadline.SelectedDate = task.Deadline;
                chkImportant.IsChecked = task.IsImportant;
                btnAdd.IsEnabled = false;
                btnUpdate.IsEnabled = true;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvTasks.SelectedItem is TaskManagerLibrary.Task task)
            {
                if (MessageBox.Show($"Удалить задачу '{task.Title}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _taskManager.DeleteTask(task.Id);
                    ClearInputs();
                    btnAdd.IsEnabled = true;
                    btnUpdate.IsEnabled = false;
                    _selectedTask = null;
                    RefreshTaskList();
                    SaveToFile();
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу для удаления!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnFilterAll_Click(object sender, RoutedEventArgs e) => RefreshTaskList(_taskManager.Tasks);
        private void BtnFilterNew_Click(object sender, RoutedEventArgs e) => RefreshTaskList(_taskManager.FilterByStatus(TaskManagerLibrary.TaskStatus.Новая));
        private void BtnFilterInProgress_Click(object sender, RoutedEventArgs e) => RefreshTaskList(_taskManager.FilterByStatus(TaskManagerLibrary.TaskStatus.ВПроцессе));
        private void BtnFilterCompleted_Click(object sender, RoutedEventArgs e) => RefreshTaskList(_taskManager.FilterByStatus(TaskManagerLibrary.TaskStatus.Завершена));

        private void BtnSortPriority_Click(object sender, RoutedEventArgs e) => RefreshTaskList(_taskManager.SortByPriority());
        private void BtnSortDeadline_Click(object sender, RoutedEventArgs e) => RefreshTaskList(_taskManager.SortByDeadline());

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                RefreshTaskList(_taskManager.Search(txtSearch.Text));
            }
            else
            {
                RefreshTaskList(_taskManager.Tasks);
            }
        }

        private void ClearInputs()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            cmbPriority.SelectedIndex = 1;
            cmbStatus.SelectedIndex = 0;
            dpDeadline.SelectedDate = DateTime.Now;
            chkImportant.IsChecked = false;
            lvTasks.SelectedItem = null;
        }

        private TaskManagerLibrary.TaskPriority GetPriorityFromComboBox(ComboBox cmb)
        {
            return (TaskManagerLibrary.TaskPriority)cmb.SelectedIndex;
        }

        private TaskManagerLibrary.TaskStatus GetStatusFromComboBox(ComboBox cmb)
        {
            return (TaskManagerLibrary.TaskStatus)cmb.SelectedIndex;
        }

        private void SaveToFile()
        {
            try
            {
                _taskManager.SaveToJson("tasks.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveToFile();
            base.OnClosed(e);
        }
    }
}