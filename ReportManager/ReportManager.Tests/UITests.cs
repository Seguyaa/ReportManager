using System.Reflection;
using System.Windows.Forms;

namespace ReportManager.Tests
{
    [TestClass]
    public class UITests
    {
        private MainForm _form;

        [TestInitialize]
        public void SetUp()
        {
            _form = new MainForm();
        }

        [TestCleanup]
        public void TearDown()
        {
            _form?.Dispose();
        }

        [TestMethod]
        public void MainForm_Title_IsCorrect()
        {
            // Assert
            Assert.AreEqual("Управление отчетами", _form.Text);
        }

        [TestMethod]
        public void MainForm_Size_IsAppropriate()
        {
            // Assert
            Assert.IsTrue(_form.Width >= 500);
            Assert.IsTrue(_form.Height >= 350);
        }

        [TestMethod]
        public void MainForm_StartPosition_IsCenterScreen()
        {
            // Assert
            Assert.AreEqual(FormStartPosition.CenterScreen, _form.StartPosition);
        }

        [TestMethod]
        public void MainForm_MinimumSize_IsSet()
        {
            // Assert
            Assert.IsTrue(_form.MinimumSize.Width > 0);
            Assert.IsTrue(_form.MinimumSize.Height > 0);
        }

        [TestMethod]
        public void AllButtons_AreProperlyPositioned()
        {
            // Arrange
            var listBox = GetControl<ListBox>("reportsListBox", _form);
            var addButton = GetControl<Button>("addButton", _form);
            var editButton = GetControl<Button>("editButton", _form);
            var deleteButton = GetControl<Button>("deleteButton", _form);
            var viewButton = GetControl<Button>("viewButton", _form);

            // Assert - проверяем, что кнопки находятся под списком
            if (listBox != null && addButton != null)
            {
                Assert.IsTrue(addButton.Top > listBox.Bottom,
                    "Кнопка Добавить должна быть под списком отчетов");
            }

            if (addButton != null && editButton != null)
            {
                Assert.IsTrue(editButton.Left > addButton.Right ||
                    Math.Abs(editButton.Top - addButton.Top) < 5,
                    "Кнопки должны быть расположены в ряд");
            }
        }

        [TestMethod]
        public void AllControls_AreAccessible()
        {
            // Проверяем, что все основные контролы доступны
            // Используем явное указание типов вместо неявно типизированного массива
            Control[] controls = new Control[]
            {
                GetControl<ListBox>("reportsListBox", _form),
                GetControl<Button>("addButton", _form),
                GetControl<Button>("editButton", _form),
                GetControl<Button>("deleteButton", _form),
                GetControl<Button>("viewButton", _form),
                GetControl<TextBox>("searchTextBox", _form),
                GetControl<Button>("searchButton", _form)
            };

            foreach (var control in controls)
            {
                Assert.IsNotNull(control, $"Контрол не найден");
            }
        }

        /// <summary>
        /// Вспомогательный метод для получения контрола по имени
        /// </summary>
        private T GetControl<T>(string name, Form form) where T : Control
        {
            // Ищем через рефлексию
            var fieldInfo = form.GetType().GetField(name,
                BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo != null)
            {
                return (T)fieldInfo.GetValue(form);
            }

            // Если не нашли через рефлексию, ищем в коллекции Controls
            foreach (Control control in form.Controls)
            {
                if (control.Name == name && control is T)
                {
                    return (T)control;
                }
            }

            return null;
        }
    }
}