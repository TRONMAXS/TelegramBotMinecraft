namespace TelegramBotMinecraft
{
    partial class UserControl_Console
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
            gb_Servers = new GroupBox();
            gb_ServersInfo = new GroupBox();
            tb_StatusServer = new TextBox();
            tb_NameServer = new TextBox();
            lbl_StatusServer = new Label();
            lbl_NameServer = new Label();
            lv_Servers = new ListView();
            ServerName = new ColumnHeader();
            ServerStatus = new ColumnHeader();
            btn_OffServer = new Button();
            btn_OnServer = new Button();
            gb_ConsoleServer = new GroupBox();
            rtb_ConsoleBot = new RichTextBox();
            btn_EnterCommand = new Button();
            tb_CommandField = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            gb_Servers.SuspendLayout();
            gb_ServersInfo.SuspendLayout();
            gb_ConsoleServer.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // gb_Servers
            // 
            gb_Servers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gb_Servers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_Servers.Controls.Add(gb_ServersInfo);
            gb_Servers.Controls.Add(lv_Servers);
            gb_Servers.Controls.Add(btn_OffServer);
            gb_Servers.Controls.Add(btn_OnServer);
            gb_Servers.Location = new Point(898, 3);
            gb_Servers.Name = "gb_Servers";
            gb_Servers.Size = new Size(286, 651);
            gb_Servers.TabIndex = 6;
            gb_Servers.TabStop = false;
            gb_Servers.Text = "Сервера";
            // 
            // gb_ServersInfo
            // 
            gb_ServersInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gb_ServersInfo.Controls.Add(tb_StatusServer);
            gb_ServersInfo.Controls.Add(tb_NameServer);
            gb_ServersInfo.Controls.Add(lbl_StatusServer);
            gb_ServersInfo.Controls.Add(lbl_NameServer);
            gb_ServersInfo.Location = new Point(3, 491);
            gb_ServersInfo.Name = "gb_ServersInfo";
            gb_ServersInfo.Size = new Size(280, 154);
            gb_ServersInfo.TabIndex = 7;
            gb_ServersInfo.TabStop = false;
            gb_ServersInfo.Text = "Информация";
            // 
            // tb_StatusServer
            // 
            tb_StatusServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_StatusServer.Location = new Point(6, 93);
            tb_StatusServer.Name = "tb_StatusServer";
            tb_StatusServer.ReadOnly = true;
            tb_StatusServer.ShortcutsEnabled = false;
            tb_StatusServer.Size = new Size(268, 23);
            tb_StatusServer.TabIndex = 11;
            tb_StatusServer.TextAlign = HorizontalAlignment.Center;
            tb_StatusServer.WordWrap = false;
            // 
            // tb_NameServer
            // 
            tb_NameServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_NameServer.Location = new Point(6, 44);
            tb_NameServer.Name = "tb_NameServer";
            tb_NameServer.ReadOnly = true;
            tb_NameServer.ShortcutsEnabled = false;
            tb_NameServer.Size = new Size(268, 23);
            tb_NameServer.TabIndex = 10;
            tb_NameServer.TextAlign = HorizontalAlignment.Center;
            tb_NameServer.WordWrap = false;
            // 
            // lbl_StatusServer
            // 
            lbl_StatusServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_StatusServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_StatusServer.ImeMode = ImeMode.NoControl;
            lbl_StatusServer.Location = new Point(110, 70);
            lbl_StatusServer.Name = "lbl_StatusServer";
            lbl_StatusServer.Size = new Size(65, 20);
            lbl_StatusServer.TabIndex = 2;
            lbl_StatusServer.Text = "Статус:";
            // 
            // lbl_NameServer
            // 
            lbl_NameServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_NameServer.AutoSize = true;
            lbl_NameServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_NameServer.ImeMode = ImeMode.NoControl;
            lbl_NameServer.Location = new Point(100, 20);
            lbl_NameServer.Name = "lbl_NameServer";
            lbl_NameServer.Size = new Size(86, 21);
            lbl_NameServer.TabIndex = 1;
            lbl_NameServer.Text = "Название:";
            // 
            // lv_Servers
            // 
            lv_Servers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lv_Servers.BackColor = SystemColors.Control;
            lv_Servers.Columns.AddRange(new ColumnHeader[] { ServerName, ServerStatus });
            lv_Servers.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lv_Servers.FullRowSelect = true;
            lv_Servers.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lv_Servers.LabelWrap = false;
            lv_Servers.Location = new Point(3, 19);
            lv_Servers.MultiSelect = false;
            lv_Servers.Name = "lv_Servers";
            lv_Servers.Scrollable = false;
            lv_Servers.Size = new Size(280, 434);
            lv_Servers.TabIndex = 6;
            lv_Servers.UseCompatibleStateImageBehavior = false;
            lv_Servers.View = View.Details;
            // 
            // ServerName
            // 
            ServerName.Text = "Название";
            ServerName.Width = 216;
            // 
            // ServerStatus
            // 
            ServerStatus.Text = "Статус";
            ServerStatus.Width = 64;
            // 
            // btn_OffServer
            // 
            btn_OffServer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_OffServer.Enabled = false;
            btn_OffServer.ImeMode = ImeMode.NoControl;
            btn_OffServer.Location = new Point(207, 459);
            btn_OffServer.Name = "btn_OffServer";
            btn_OffServer.Size = new Size(73, 26);
            btn_OffServer.TabIndex = 5;
            btn_OffServer.Text = "Выкл";
            btn_OffServer.UseVisualStyleBackColor = true;
            // 
            // btn_OnServer
            // 
            btn_OnServer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_OnServer.Enabled = false;
            btn_OnServer.ImeMode = ImeMode.NoControl;
            btn_OnServer.Location = new Point(6, 459);
            btn_OnServer.Name = "btn_OnServer";
            btn_OnServer.Size = new Size(73, 26);
            btn_OnServer.TabIndex = 4;
            btn_OnServer.Text = "Вкл";
            btn_OnServer.UseVisualStyleBackColor = true;
            // 
            // gb_ConsoleServer
            // 
            gb_ConsoleServer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_ConsoleServer.Controls.Add(rtb_ConsoleBot);
            gb_ConsoleServer.Controls.Add(btn_EnterCommand);
            gb_ConsoleServer.Controls.Add(tb_CommandField);
            gb_ConsoleServer.Enabled = false;
            gb_ConsoleServer.Location = new Point(3, 3);
            gb_ConsoleServer.Name = "gb_ConsoleServer";
            gb_ConsoleServer.Size = new Size(889, 651);
            gb_ConsoleServer.TabIndex = 7;
            gb_ConsoleServer.TabStop = false;
            gb_ConsoleServer.Text = "Консоль";
            // 
            // rtb_ConsoleBot
            // 
            rtb_ConsoleBot.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtb_ConsoleBot.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            rtb_ConsoleBot.Location = new Point(3, 19);
            rtb_ConsoleBot.Name = "rtb_ConsoleBot";
            rtb_ConsoleBot.ReadOnly = true;
            rtb_ConsoleBot.Size = new Size(883, 584);
            rtb_ConsoleBot.TabIndex = 0;
            rtb_ConsoleBot.Text = "";
            // 
            // btn_EnterCommand
            // 
            btn_EnterCommand.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_EnterCommand.ImeMode = ImeMode.NoControl;
            btn_EnterCommand.Location = new Point(802, 617);
            btn_EnterCommand.Name = "btn_EnterCommand";
            btn_EnterCommand.Size = new Size(84, 23);
            btn_EnterCommand.TabIndex = 3;
            btn_EnterCommand.Text = "Ввод";
            btn_EnterCommand.UseVisualStyleBackColor = true;
            // 
            // tb_CommandField
            // 
            tb_CommandField.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_CommandField.Location = new Point(3, 615);
            tb_CommandField.Name = "tb_CommandField";
            tb_CommandField.PlaceholderText = "Поле для команд";
            tb_CommandField.Size = new Size(781, 23);
            tb_CommandField.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.4386F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.5614033F));
            tableLayoutPanel1.Controls.Add(gb_ConsoleServer, 0, 0);
            tableLayoutPanel1.Controls.Add(gb_Servers, 1, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1187, 657);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // UserControl_Console
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "UserControl_Console";
            Size = new Size(1190, 660);
            gb_Servers.ResumeLayout(false);
            gb_ServersInfo.ResumeLayout(false);
            gb_ServersInfo.PerformLayout();
            gb_ConsoleServer.ResumeLayout(false);
            gb_ConsoleServer.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gb_Servers;
        private GroupBox gb_ConsoleServer;
        private RichTextBox rtb_ConsoleBot;
        private Button btn_EnterCommand;
        private TextBox tb_CommandField;
        private Button btn_OffServer;
        private Button btn_OnServer;
        private ListView lv_Servers;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox gb_ServersInfo;
        private Label lbl_NameServer;
        private Label lbl_StatusServer;
        private TextBox tb_NameServer;
        private TextBox tb_StatusServer;
        private ColumnHeader ServerName;
        private ColumnHeader ServerStatus;
    }
}
