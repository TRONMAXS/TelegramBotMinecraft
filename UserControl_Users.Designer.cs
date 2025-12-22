namespace TelegramBotMinecraft
{
    partial class UserControl_Users
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox4 = new GroupBox();
            label = new Label();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            listBox1 = new ListBox();
            groupBox1 = new GroupBox();
            groupBox3 = new GroupBox();
            checkedListBox2 = new CheckedListBox();
            groupBox2 = new GroupBox();
            checkedListBox1 = new CheckedListBox();
            button1 = new Button();
            groupBox4.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label);
            groupBox4.Controls.Add(textBox2);
            groupBox4.Controls.Add(textBox1);
            groupBox4.Controls.Add(button4);
            groupBox4.Controls.Add(button3);
            groupBox4.Controls.Add(button2);
            groupBox4.Controls.Add(listBox1);
            groupBox4.Location = new Point(672, 0);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(297, 527);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "Пользователи";
            // 
            // label
            // 
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label.ImeMode = ImeMode.NoControl;
            label.Location = new Point(30, 389);
            label.Name = "label";
            label.Size = new Size(235, 42);
            label.TabIndex = 8;
            label.Text = "               Разрешенные \r\nпользователи, чаты, группы";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(6, 479);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Id";
            textBox2.Size = new Size(195, 23);
            textBox2.TabIndex = 5;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 450);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Имя";
            textBox1.Size = new Size(195, 23);
            textBox1.TabIndex = 4;
            // 
            // button4
            // 
            button4.ImeMode = ImeMode.NoControl;
            button4.Location = new Point(212, 466);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 3;
            button4.Text = "Добавить";
            button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.ImeMode = ImeMode.NoControl;
            button3.Location = new Point(212, 347);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "Удалить";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.ImeMode = ImeMode.NoControl;
            button2.Location = new Point(6, 347);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.Text = "Изменить";
            button2.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(6, 22);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(287, 319);
            listBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(groupBox3);
            groupBox1.Controls.Add(groupBox2);
            groupBox1.Controls.Add(button1);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(666, 527);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Настройки пользователя";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(checkedListBox2);
            groupBox3.Location = new Point(341, 22);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(315, 463);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Команды";
            // 
            // checkedListBox2
            // 
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Location = new Point(6, 18);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(303, 436);
            checkedListBox2.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(checkedListBox1);
            groupBox2.Location = new Point(6, 22);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(315, 463);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Сервера";
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(6, 18);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(303, 436);
            checkedListBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.ImeMode = ImeMode.NoControl;
            button1.Location = new Point(290, 491);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Сохранить ";
            button1.UseVisualStyleBackColor = true;
            // 
            // UserControl_Users
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox4);
            Controls.Add(groupBox1);
            Name = "UserControl_Users";
            Size = new Size(969, 527);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox4;
        private Label label;
        private TextBox textBox2;
        private TextBox textBox1;
        private Button button4;
        private Button button3;
        private Button button2;
        private ListBox listBox1;
        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private CheckedListBox checkedListBox2;
        private GroupBox groupBox2;
        private CheckedListBox checkedListBox1;
        private Button button1;
    }
}
