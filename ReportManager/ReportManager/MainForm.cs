using ReportManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ReportManager
{
    public partial class MainForm : Form
    {
        private ReportManager reportManager;
        private ListBox reportsListBox;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button viewButton;
        private Button searchButton;
        private TextBox searchTextBox;
        private Label searchLabel;
        private Label infoLabel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponent();
            reportManager = new ReportManager();
            UpdateReportsList();
            UpdateStatusBar();
        }

        private void InitializeCustomComponent()
        {
            this.Text = "Управление отчетами";
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(500, 350);

            // Список отчетов
            reportsListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(560, 250),
                IntegralHeight = false,
                SelectionMode = SelectionMode.One
            };

            // Метка для поиска
            searchLabel = new Label
            {
                Text = "Поиск:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(50, 20)
            };

            // Поле поиска
            searchTextBox = new TextBox
            {
                Location = new System.Drawing.Point(60, 10),
                Size = new System.Drawing.Size(200, 20)
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            // Кнопка поиска
            searchButton = new Button
            {
                Text = "Найти",
                Location = new System.Drawing.Point(270, 8),
                Size = new System.Drawing.Size(75, 23)
            };
            searchButton.Click += SearchButton_Click;

            // Кнопка добавления
            addButton = new Button
            {
                Text = "Добавить",
                Location = new System.Drawing.Point(10, 300),
                Size = new System.Drawing.Size(100, 30)
            };
            addButton.Click += AddButton_Click;

            // Кнопка редактирования
            editButton = new Button
            {
                Text = "Редактировать",
                Location = new System.Drawing.Point(120, 300),
                Size = new System.Drawing.Size(100, 30)
            };
            editButton.Click += EditButton_Click;

            // Кнопка удаления
            deleteButton = new Button
            {
                Text = "Удалить",
                Location = new System.Drawing.Point(230, 300),
                Size = new System.Drawing.Size(100, 30)
            };
            deleteButton.Click += DeleteButton_Click;

            // Кнопка просмотра
            viewButton = new Button
            {
                Text = "Просмотр",
                Location = new System.Drawing.Point(340, 300),
                Size = new System.Drawing.Size(100, 30)
            };
            viewButton.Click += ViewButton_Click;

            // Информационная метка
            infoLabel = new Label
            {
                Text = "Выберите отчет для просмотра или редактирования",
                Location = new System.Drawing.Point(10, 340),
                Size = new System.Drawing.Size(560, 20),
                ForeColor = System.Drawing.Color.Gray
            };

            // Строка состояния
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Готов к работе");
            statusStrip.Items.Add(statusLabel);

            // Добавление элементов на форму
            this.Controls.AddRange(new Control[] {
                searchLabel, searchTextBox, searchButton,
                reportsListBox,
                addButton, editButton, deleteButton, viewButton,
                infoLabel,
                statusStrip
            });
        }

        private void UpdateReportsList()
        {
            reportsListBox.Items.Clear();
            foreach (var report in reportManager.Reports)
            {
                reportsListBox.Items.Add(report);
            }
            UpdateStatusBar();
        }

        private void UpdateReportsList(IEnumerable<Report> reports)
        {
            reportsListBox.Items.Clear();
            foreach (var report in reports)
            {
                reportsListBox.Items.Add(report);
            }
        }

        private void UpdateStatusBar()
        {
            int count = reportManager.Reports.Count;
            statusLabel.Text = $"Всего отчетов: {count}";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editForm = new ReportEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var report = new Report(
                        editForm.ReportTitle,
                        editForm.ReportContent,
                        DateTime.Now);

                    reportManager.AddReport(report);
                    UpdateReportsList();
                    MessageBox.Show("Отчет успешно добавлен!",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении отчета: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (reportsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет для редактирования!",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedReport = (Report)reportsListBox.SelectedItem;
            var editForm = new ReportEditForm();
            editForm.SetReportData(selectedReport.Title, selectedReport.Content);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    reportManager.UpdateReport(
                        selectedReport,
                        editForm.ReportTitle,
                        editForm.ReportContent);

                    UpdateReportsList();
                    MessageBox.Show("Отчет успешно обновлен!",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении отчета: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (reportsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет для удаления!",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedReport = (Report)reportsListBox.SelectedItem;
            var result = MessageBox.Show(
                $"Вы действительно хотите удалить отчет '{selectedReport.Title}'?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    reportManager.RemoveReport(selectedReport);
                    UpdateReportsList();
                    MessageBox.Show("Отчет успешно удален!",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении отчета: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ViewButton_Click(object sender, EventArgs e)
        {
            if (reportsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет для просмотра!",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedReport = (Report)reportsListBox.SelectedItem;
            MessageBox.Show(selectedReport.GetFullInfo(),
                "Просмотр отчета",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // Автоматический поиск при вводе (можно добавить задержку)
            PerformSearch();
        }

        private void PerformSearch()
        {
            string searchTerm = searchTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                UpdateReportsList();
            }
            else
            {
                var results = reportManager.SearchReports(searchTerm);
                UpdateReportsList(results);

                if (results.Count == 0)
                {
                    infoLabel.Text = "Отчеты не найдены";
                    infoLabel.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    infoLabel.Text = $"Найдено отчетов: {results.Count}";
                    infoLabel.ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }

            base.OnFormClosing(e);
        }
    }
}