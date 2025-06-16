namespace TelegramBotMinecraft
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4));
            listBox1 = new ListBox();
            label3 = new Label();
            checkedListBox1 = new CheckedListBox();
            checkedListBox2 = new CheckedListBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.HorizontalScrollbar = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(460, 63);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new Size(274, 454);
            listBox1.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label3.Location = new Point(460, 9);
            label3.Name = "label3";
            label3.Size = new Size(235, 42);
            label3.TabIndex = 8;
            label3.Text = "               Разрешенные \r\nпользователи, чаты, группы";
            // 
            // checkedListBox1
            // 
            checkedListBox1.CheckOnClick = true;
            checkedListBox1.Font = new Font("Segoe UI", 12F);
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.HorizontalScrollbar = true;
            checkedListBox1.Location = new Point(12, 62);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(442, 196);
            checkedListBox1.TabIndex = 9;
            // 
            // checkedListBox2
            // 
            checkedListBox2.CheckOnClick = true;
            checkedListBox2.Font = new Font("Segoe UI", 12F);
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.HorizontalScrollbar = true;
            checkedListBox2.Location = new Point(12, 297);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(442, 220);
            checkedListBox2.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(190, 38);
            label1.Name = "label1";
            label1.Size = new Size(76, 21);
            label1.TabIndex = 11;
            label1.Text = "Сервера";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label2.Location = new Point(182, 273);
            label2.Name = "label2";
            label2.Size = new Size(84, 21);
            label2.TabIndex = 11;
            label2.Text = "Команды";
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(746, 537);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkedListBox2);
            Controls.Add(checkedListBox1);
            Controls.Add(label3);
            Controls.Add(listBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form4";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Настройки пользователей";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Label label3;
        private CheckedListBox checkedListBox1;
        private CheckedListBox checkedListBox2;
        private Label label1;
        private Label label2;
    }
}