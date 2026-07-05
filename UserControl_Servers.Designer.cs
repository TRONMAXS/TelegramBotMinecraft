namespace TelegramBotMinecraft
{
    partial class UserControl_Servers
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
            btn_CancleServer = new Button();
            btn_SaveServer = new Button();
            gb_SettingsServer = new GroupBox();
            groupBox1 = new GroupBox();
            button3 = new Button();
            comboBox2 = new ComboBox();
            label2 = new Label();
            comboBox1 = new ComboBox();
            button1 = new Button();
            button2 = new Button();
            label3 = new Label();
            btn_UpdateServer = new Button();
            gb_RconSettings = new GroupBox();
            lbl_RconSettings = new Label();
            btn_OnOffRconSettings = new CheckBox();
            lbl_RconPassword = new Label();
            tb_RconPassword = new TextBox();
            lbl_RconPort = new Label();
            tb_RconPort = new TextBox();
            gb_StartSettings = new GroupBox();
            button5 = new Button();
            tb_IdProcessServer = new TextBox();
            lbl_IdProcessServer = new Label();
            button6 = new Button();
            lbl_JavaArgsServer = new Label();
            tb_JavaArgsServer = new TextBox();
            lbl_PathToServer = new Label();
            tb_PathToServer = new TextBox();
            gb_MainSettings = new GroupBox();
            lbl_NameServer = new Label();
            tb_NameServer = new TextBox();
            lbl_IpAndPortServer = new Label();
            tb_IpAndPortServer = new TextBox();
            gb_Servers = new GroupBox();
            btn_DellServer = new Button();
            btn_AddServer = new Button();
            lb_Servers = new ListBox();
            gb_SettingsServer.SuspendLayout();
            groupBox1.SuspendLayout();
            gb_RconSettings.SuspendLayout();
            gb_StartSettings.SuspendLayout();
            gb_MainSettings.SuspendLayout();
            gb_Servers.SuspendLayout();
            SuspendLayout();
            // 
            // btn_CancleServer
            // 
            btn_CancleServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_CancleServer.Enabled = false;
            btn_CancleServer.ImeMode = ImeMode.NoControl;
            btn_CancleServer.Location = new Point(87, 625);
            btn_CancleServer.Name = "btn_CancleServer";
            btn_CancleServer.Size = new Size(75, 23);
            btn_CancleServer.TabIndex = 15;
            btn_CancleServer.Text = "Отменить";
            btn_CancleServer.UseVisualStyleBackColor = true;
            // 
            // btn_SaveServer
            // 
            btn_SaveServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_SaveServer.Enabled = false;
            btn_SaveServer.ImeMode = ImeMode.NoControl;
            btn_SaveServer.Location = new Point(6, 625);
            btn_SaveServer.Name = "btn_SaveServer";
            btn_SaveServer.Size = new Size(75, 23);
            btn_SaveServer.TabIndex = 14;
            btn_SaveServer.Text = "Сохранить";
            btn_SaveServer.UseVisualStyleBackColor = true;
            // 
            // gb_SettingsServer
            // 
            gb_SettingsServer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_SettingsServer.Controls.Add(groupBox1);
            gb_SettingsServer.Controls.Add(btn_UpdateServer);
            gb_SettingsServer.Controls.Add(btn_SaveServer);
            gb_SettingsServer.Controls.Add(btn_CancleServer);
            gb_SettingsServer.Controls.Add(gb_RconSettings);
            gb_SettingsServer.Controls.Add(gb_StartSettings);
            gb_SettingsServer.Controls.Add(gb_MainSettings);
            gb_SettingsServer.Enabled = false;
            gb_SettingsServer.Location = new Point(3, 3);
            gb_SettingsServer.Name = "gb_SettingsServer";
            gb_SettingsServer.Size = new Size(864, 654);
            gb_SettingsServer.TabIndex = 12;
            gb_SettingsServer.TabStop = false;
            gb_SettingsServer.Text = "Настройки сервера";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(comboBox2);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(5, 264);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(852, 86);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Java";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.Enabled = false;
            button3.ImeMode = ImeMode.NoControl;
            button3.Location = new Point(511, 52);
            button3.Name = "button3";
            button3.Size = new Size(84, 23);
            button3.TabIndex = 17;
            button3.Text = "Управление";
            button3.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(218, 17);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(288, 23);
            comboBox2.TabIndex = 17;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label2.ImeMode = ImeMode.NoControl;
            label2.Location = new Point(6, 19);
            label2.Name = "label2";
            label2.Size = new Size(145, 21);
            label2.TabIndex = 16;
            label2.Text = "Версия Minecraft:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(218, 52);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(287, 23);
            comboBox1.TabIndex = 13;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.ImeMode = ImeMode.NoControl;
            button1.Location = new Point(1423, 25);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 10;
            button1.Text = "Выбрать";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.ImeMode = ImeMode.NoControl;
            button2.Location = new Point(1423, 56);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 6;
            button2.Text = "Настроить";
            button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label3.ImeMode = ImeMode.NoControl;
            label3.Location = new Point(6, 54);
            label3.Name = "label3";
            label3.Size = new Size(104, 21);
            label3.TabIndex = 0;
            label3.Text = "Версия Java:";
            // 
            // btn_UpdateServer
            // 
            btn_UpdateServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_UpdateServer.Enabled = false;
            btn_UpdateServer.ImeMode = ImeMode.NoControl;
            btn_UpdateServer.Location = new Point(6, 596);
            btn_UpdateServer.Name = "btn_UpdateServer";
            btn_UpdateServer.Size = new Size(75, 23);
            btn_UpdateServer.TabIndex = 16;
            btn_UpdateServer.Text = "Обновить";
            btn_UpdateServer.UseVisualStyleBackColor = true;
            // 
            // gb_RconSettings
            // 
            gb_RconSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gb_RconSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_RconSettings.Controls.Add(lbl_RconSettings);
            gb_RconSettings.Controls.Add(btn_OnOffRconSettings);
            gb_RconSettings.Controls.Add(lbl_RconPassword);
            gb_RconSettings.Controls.Add(tb_RconPassword);
            gb_RconSettings.Controls.Add(lbl_RconPort);
            gb_RconSettings.Controls.Add(tb_RconPort);
            gb_RconSettings.Location = new Point(6, 396);
            gb_RconSettings.Name = "gb_RconSettings";
            gb_RconSettings.Size = new Size(852, 104);
            gb_RconSettings.TabIndex = 11;
            gb_RconSettings.TabStop = false;
            // 
            // lbl_RconSettings
            // 
            lbl_RconSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_RconSettings.AutoSize = true;
            lbl_RconSettings.Location = new Point(6, 2);
            lbl_RconSettings.Name = "lbl_RconSettings";
            lbl_RconSettings.Size = new Size(34, 15);
            lbl_RconSettings.TabIndex = 17;
            lbl_RconSettings.Text = "Rcon";
            // 
            // btn_OnOffRconSettings
            // 
            btn_OnOffRconSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_OnOffRconSettings.AutoSize = true;
            btn_OnOffRconSettings.ImeMode = ImeMode.NoControl;
            btn_OnOffRconSettings.Location = new Point(46, 3);
            btn_OnOffRconSettings.Name = "btn_OnOffRconSettings";
            btn_OnOffRconSettings.Size = new Size(15, 14);
            btn_OnOffRconSettings.TabIndex = 7;
            btn_OnOffRconSettings.TabStop = false;
            btn_OnOffRconSettings.UseVisualStyleBackColor = true;
            // 
            // lbl_RconPassword
            // 
            lbl_RconPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_RconPassword.AutoSize = true;
            lbl_RconPassword.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_RconPassword.ImeMode = ImeMode.NoControl;
            lbl_RconPassword.Location = new Point(6, 52);
            lbl_RconPassword.Name = "lbl_RconPassword";
            lbl_RconPassword.Size = new Size(71, 21);
            lbl_RconPassword.TabIndex = 4;
            lbl_RconPassword.Text = "Пароль:";
            // 
            // tb_RconPassword
            // 
            tb_RconPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tb_RconPassword.Location = new Point(217, 52);
            tb_RconPassword.Name = "tb_RconPassword";
            tb_RconPassword.PlaceholderText = "12345";
            tb_RconPassword.Size = new Size(288, 23);
            tb_RconPassword.TabIndex = 5;
            // 
            // lbl_RconPort
            // 
            lbl_RconPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_RconPort.AutoSize = true;
            lbl_RconPort.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_RconPort.ImeMode = ImeMode.NoControl;
            lbl_RconPort.Location = new Point(6, 20);
            lbl_RconPort.Name = "lbl_RconPort";
            lbl_RconPort.Size = new Size(53, 21);
            lbl_RconPort.TabIndex = 2;
            lbl_RconPort.Text = "Порт:";
            // 
            // tb_RconPort
            // 
            tb_RconPort.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tb_RconPort.Location = new Point(217, 22);
            tb_RconPort.Name = "tb_RconPort";
            tb_RconPort.PlaceholderText = "25575";
            tb_RconPort.Size = new Size(288, 23);
            tb_RconPort.TabIndex = 3;
            // 
            // gb_StartSettings
            // 
            gb_StartSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gb_StartSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_StartSettings.Controls.Add(button5);
            gb_StartSettings.Controls.Add(tb_IdProcessServer);
            gb_StartSettings.Controls.Add(lbl_IdProcessServer);
            gb_StartSettings.Controls.Add(button6);
            gb_StartSettings.Controls.Add(lbl_JavaArgsServer);
            gb_StartSettings.Controls.Add(tb_JavaArgsServer);
            gb_StartSettings.Controls.Add(lbl_PathToServer);
            gb_StartSettings.Controls.Add(tb_PathToServer);
            gb_StartSettings.Location = new Point(5, 122);
            gb_StartSettings.Name = "gb_StartSettings";
            gb_StartSettings.Size = new Size(852, 136);
            gb_StartSettings.TabIndex = 11;
            gb_StartSettings.TabStop = false;
            gb_StartSettings.Text = "Запуск";
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button5.ImeMode = ImeMode.NoControl;
            button5.Location = new Point(771, 25);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 10;
            button5.Text = "Выбрать";
            button5.UseVisualStyleBackColor = true;
            // 
            // tb_IdProcessServer
            // 
            tb_IdProcessServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tb_IdProcessServer.Location = new Point(163, 93);
            tb_IdProcessServer.Name = "tb_IdProcessServer";
            tb_IdProcessServer.ReadOnly = true;
            tb_IdProcessServer.ShortcutsEnabled = false;
            tb_IdProcessServer.Size = new Size(288, 23);
            tb_IdProcessServer.TabIndex = 9;
            tb_IdProcessServer.WordWrap = false;
            // 
            // lbl_IdProcessServer
            // 
            lbl_IdProcessServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_IdProcessServer.AutoSize = true;
            lbl_IdProcessServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_IdProcessServer.ImeMode = ImeMode.NoControl;
            lbl_IdProcessServer.Location = new Point(6, 91);
            lbl_IdProcessServer.Name = "lbl_IdProcessServer";
            lbl_IdProcessServer.Size = new Size(109, 21);
            lbl_IdProcessServer.TabIndex = 8;
            lbl_IdProcessServer.Text = "ID Процесса:";
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button6.ImeMode = ImeMode.NoControl;
            button6.Location = new Point(771, 56);
            button6.Name = "button6";
            button6.Size = new Size(75, 23);
            button6.TabIndex = 6;
            button6.Text = "Настроить";
            button6.UseVisualStyleBackColor = true;
            // 
            // lbl_JavaArgsServer
            // 
            lbl_JavaArgsServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_JavaArgsServer.AutoSize = true;
            lbl_JavaArgsServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_JavaArgsServer.ImeMode = ImeMode.NoControl;
            lbl_JavaArgsServer.Location = new Point(6, 54);
            lbl_JavaArgsServer.Name = "lbl_JavaArgsServer";
            lbl_JavaArgsServer.Size = new Size(132, 21);
            lbl_JavaArgsServer.TabIndex = 2;
            lbl_JavaArgsServer.Text = "Аргументы Java:";
            // 
            // tb_JavaArgsServer
            // 
            tb_JavaArgsServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_JavaArgsServer.Location = new Point(163, 56);
            tb_JavaArgsServer.Name = "tb_JavaArgsServer";
            tb_JavaArgsServer.PlaceholderText = "-Xmx4096M -Xms1024M -jar server.jar nogui";
            tb_JavaArgsServer.Size = new Size(602, 23);
            tb_JavaArgsServer.TabIndex = 3;
            // 
            // lbl_PathToServer
            // 
            lbl_PathToServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_PathToServer.AutoSize = true;
            lbl_PathToServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_PathToServer.ImeMode = ImeMode.NoControl;
            lbl_PathToServer.Location = new Point(6, 23);
            lbl_PathToServer.Name = "lbl_PathToServer";
            lbl_PathToServer.Size = new Size(128, 21);
            lbl_PathToServer.TabIndex = 0;
            lbl_PathToServer.Text = "Путь к серверу:";
            // 
            // tb_PathToServer
            // 
            tb_PathToServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_PathToServer.Location = new Point(163, 25);
            tb_PathToServer.Name = "tb_PathToServer";
            tb_PathToServer.PlaceholderText = "C:\\Minecraft\\Server\\";
            tb_PathToServer.Size = new Size(602, 23);
            tb_PathToServer.TabIndex = 1;
            // 
            // gb_MainSettings
            // 
            gb_MainSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gb_MainSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_MainSettings.Controls.Add(lbl_NameServer);
            gb_MainSettings.Controls.Add(tb_NameServer);
            gb_MainSettings.Controls.Add(lbl_IpAndPortServer);
            gb_MainSettings.Controls.Add(tb_IpAndPortServer);
            gb_MainSettings.Location = new Point(5, 25);
            gb_MainSettings.Name = "gb_MainSettings";
            gb_MainSettings.Size = new Size(852, 91);
            gb_MainSettings.TabIndex = 10;
            gb_MainSettings.TabStop = false;
            gb_MainSettings.Text = "Основное";
            // 
            // lbl_NameServer
            // 
            lbl_NameServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_NameServer.AutoSize = true;
            lbl_NameServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_NameServer.ImeMode = ImeMode.NoControl;
            lbl_NameServer.Location = new Point(6, 19);
            lbl_NameServer.Name = "lbl_NameServer";
            lbl_NameServer.Size = new Size(86, 21);
            lbl_NameServer.TabIndex = 0;
            lbl_NameServer.Text = "Название:";
            // 
            // tb_NameServer
            // 
            tb_NameServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tb_NameServer.Location = new Point(218, 21);
            tb_NameServer.Name = "tb_NameServer";
            tb_NameServer.PlaceholderText = "Survival - 1.20.1";
            tb_NameServer.Size = new Size(287, 23);
            tb_NameServer.TabIndex = 1;
            // 
            // lbl_IpAndPortServer
            // 
            lbl_IpAndPortServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbl_IpAndPortServer.AutoSize = true;
            lbl_IpAndPortServer.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lbl_IpAndPortServer.ImeMode = ImeMode.NoControl;
            lbl_IpAndPortServer.Location = new Point(6, 52);
            lbl_IpAndPortServer.Name = "lbl_IpAndPortServer";
            lbl_IpAndPortServer.Size = new Size(70, 21);
            lbl_IpAndPortServer.TabIndex = 0;
            lbl_IpAndPortServer.Text = "IP : Port:";
            // 
            // tb_IpAndPortServer
            // 
            tb_IpAndPortServer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            tb_IpAndPortServer.Location = new Point(218, 52);
            tb_IpAndPortServer.Name = "tb_IpAndPortServer";
            tb_IpAndPortServer.PlaceholderText = "127.0.0.1:25565";
            tb_IpAndPortServer.Size = new Size(287, 23);
            tb_IpAndPortServer.TabIndex = 1;
            // 
            // gb_Servers
            // 
            gb_Servers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gb_Servers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_Servers.Controls.Add(btn_DellServer);
            gb_Servers.Controls.Add(btn_AddServer);
            gb_Servers.Controls.Add(lb_Servers);
            gb_Servers.Location = new Point(873, 3);
            gb_Servers.Name = "gb_Servers";
            gb_Servers.Size = new Size(314, 654);
            gb_Servers.TabIndex = 13;
            gb_Servers.TabStop = false;
            gb_Servers.Text = "Сервера";
            // 
            // btn_DellServer
            // 
            btn_DellServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_DellServer.ImeMode = ImeMode.NoControl;
            btn_DellServer.Location = new Point(233, 596);
            btn_DellServer.Name = "btn_DellServer";
            btn_DellServer.Size = new Size(75, 23);
            btn_DellServer.TabIndex = 4;
            btn_DellServer.Text = "Удалить";
            btn_DellServer.UseVisualStyleBackColor = true;
            // 
            // btn_AddServer
            // 
            btn_AddServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_AddServer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_AddServer.ImeMode = ImeMode.NoControl;
            btn_AddServer.Location = new Point(7, 596);
            btn_AddServer.Name = "btn_AddServer";
            btn_AddServer.Size = new Size(75, 23);
            btn_AddServer.TabIndex = 3;
            btn_AddServer.Text = "Добавить";
            btn_AddServer.UseVisualStyleBackColor = true;
            // 
            // lb_Servers
            // 
            lb_Servers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lb_Servers.CausesValidation = false;
            lb_Servers.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lb_Servers.FormattingEnabled = true;
            lb_Servers.Location = new Point(7, 18);
            lb_Servers.Name = "lb_Servers";
            lb_Servers.Size = new Size(301, 550);
            lb_Servers.TabIndex = 1;
            // 
            // UserControl_Servers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(gb_SettingsServer);
            Controls.Add(gb_Servers);
            Name = "UserControl_Servers";
            Size = new Size(1190, 660);
            gb_SettingsServer.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            gb_RconSettings.ResumeLayout(false);
            gb_RconSettings.PerformLayout();
            gb_StartSettings.ResumeLayout(false);
            gb_StartSettings.PerformLayout();
            gb_MainSettings.ResumeLayout(false);
            gb_MainSettings.PerformLayout();
            gb_Servers.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btn_CancleServer;
        private Button btn_SaveServer;
        private GroupBox gb_SettingsServer;
        private GroupBox gb_RconSettings;
        private CheckBox btn_OnOffRconSettings;
        private Label lbl_RconPassword;
        private TextBox tb_RconPassword;
        private Label lbl_RconPort;
        private TextBox tb_RconPort;
        private GroupBox gb_StartSettings;
        private TextBox tb_IdProcessServer;
        private Label lbl_IdProcessServer;
        private Button button6;
        private Button button7;
        private Label lbl_JavaArgsServer;
        private TextBox tb_JavaArgsServer;
        private Label lbl_PathToServer;
        private TextBox tb_PathToServer;
        private GroupBox gb_MainSettings;
        private Label lbl_NameServer;
        private TextBox tb_NameServer;
        private Label lbl_IpAndPortServer;
        private TextBox tb_IpAndPortServer;
        private GroupBox gb_Servers;
        private Button btn_DellServer;
        private Button btn_AddServer;
        private ListBox lb_Servers;
        private Button button5;
        private Button btn_UpdateServer;
        private Label lbl_RconSettings;
        private GroupBox groupBox1;
        private Button button1;
        private Button button2;
        private Label label3;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label label2;
        private Button button3;
    }
}
