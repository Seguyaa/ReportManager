using System;
using System.Windows.Forms;

namespace ReportManager
{
    public partial class ReportEditForm : Form
    {
        private TextBox titleTextBox;
        private TextBox contentTextBox;
        private Button okButton;
        private Button cancelButton;
        private Label titleLabel;
        private Label contentLabel;

        public string ReportTitle { get; private set; }
        public string ReportContent { get; private set; }
        public bool IsEditMode { get; set; }

        public ReportEditForm()
        {
            InitializeComponent();
            InitializeCustomComponent();
            IsEditMode = false;
        }

        private void InitializeCustomComponent()
        {
            this.Text = "Создание отчета";
            this.Size = new System.Drawing.Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            titleLabel = new Label
            {
                Text = "Название отчета:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(360, 20)
            };

            titleTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 35),
                Size = new System.Drawing.Size(360, 20)
            };

            contentLabel = new Label
            {
                Text = "Содержание:",
                Location = new System.Drawing.Point(10, 65),
                Size = new System.Drawing.Size(360, 20)
            };

            contentTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 90),
                Size = new System.Drawing.Size(360, 170),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                AcceptsReturn = true,
                AcceptsTab = true
            };

            okButton = new Button
            {
                Text = "OK",
                Location = new System.Drawing.Point(200, 280),
                Size = new System.Drawing.Size(80, 25),
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(290, 280),
                Size = new System.Drawing.Size(80, 25),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                titleLabel, titleTextBox,
                contentLabel, contentTextBox,
                okButton, cancelButton
            });
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                MessageBox.Show("Введите название отчета!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(contentTextBox.Text))
            {
                MessageBox.Show("Введите содержание отчета!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            ReportTitle = titleTextBox.Text.Trim();
            ReportContent = contentTextBox.Text.Trim();
        }

        public void SetReportData(string title, string content)
        {
            titleTextBox.Text = title;
            contentTextBox.Text = content;
            this.Text = "Редактирование отчета";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            titleTextBox.Focus();
        }
    }
}