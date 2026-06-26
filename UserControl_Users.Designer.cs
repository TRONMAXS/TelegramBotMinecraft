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
            components = new System.ComponentModel.Container();
            gb_Users = new GroupBox();
            gb_NewUsers = new GroupBox();
            tb_NameUser = new TextBox();
            tb_IdUser = new TextBox();
            btn_AddUsers = new Button();
            btn_SaveUsers = new Button();
            dgv_Users = new DataGridView();
            btn_DellUsers = new Button();
            btn_EditUsers = new Button();
            gb_PermUsers = new GroupBox();
            gb_Commands = new GroupBox();
            btn_OnAllCommands = new Button();
            clb_Commands = new CheckedListBox();
            btn_OffAllCommands = new Button();
            gb_Servers = new GroupBox();
            btn_OnAllServers = new Button();
            clb_Servers = new CheckedListBox();
            btn_OffAllServers = new Button();
            btn_SavePerm = new Button();
            errorProvider1 = new ErrorProvider(components);
            gb_Users.SuspendLayout();
            gb_NewUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_Users).BeginInit();
            gb_PermUsers.SuspendLayout();
            gb_Commands.SuspendLayout();
            gb_Servers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider1).BeginInit();
            SuspendLayout();
            // 
            // gb_Users
            // 
            gb_Users.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gb_Users.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_Users.Controls.Add(gb_NewUsers);
            gb_Users.Controls.Add(btn_SaveUsers);
            gb_Users.Controls.Add(dgv_Users);
            gb_Users.Controls.Add(btn_DellUsers);
            gb_Users.Controls.Add(btn_EditUsers);
            gb_Users.Location = new Point(848, 3);
            gb_Users.Name = "gb_Users";
            gb_Users.Size = new Size(339, 654);
            gb_Users.TabIndex = 3;
            gb_Users.TabStop = false;
            gb_Users.Text = "Пользователи, чаты, группы";
            // 
            // gb_NewUsers
            // 
            gb_NewUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gb_NewUsers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_NewUsers.Controls.Add(tb_NameUser);
            gb_NewUsers.Controls.Add(tb_IdUser);
            gb_NewUsers.Controls.Add(btn_AddUsers);
            gb_NewUsers.Location = new Point(6, 526);
            gb_NewUsers.Name = "gb_NewUsers";
            gb_NewUsers.Size = new Size(326, 122);
            gb_NewUsers.TabIndex = 10;
            gb_NewUsers.TabStop = false;
            gb_NewUsers.Text = "Новый пользователь";
            // 
            // tb_NameUser
            // 
            tb_NameUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_NameUser.Location = new Point(6, 23);
            tb_NameUser.Name = "tb_NameUser";
            tb_NameUser.PlaceholderText = "Имя";
            tb_NameUser.Size = new Size(314, 23);
            tb_NameUser.TabIndex = 4;
            // 
            // tb_IdUser
            // 
            tb_IdUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tb_IdUser.Location = new Point(6, 52);
            tb_IdUser.Name = "tb_IdUser";
            tb_IdUser.PlaceholderText = "Id";
            tb_IdUser.Size = new Size(314, 23);
            tb_IdUser.TabIndex = 5;
            // 
            // btn_AddUsers
            // 
            btn_AddUsers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_AddUsers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_AddUsers.ImeMode = ImeMode.NoControl;
            btn_AddUsers.Location = new Point(125, 93);
            btn_AddUsers.Name = "btn_AddUsers";
            btn_AddUsers.Size = new Size(75, 23);
            btn_AddUsers.TabIndex = 3;
            btn_AddUsers.Text = "Добавить";
            btn_AddUsers.UseVisualStyleBackColor = true;
            // 
            // btn_SaveUsers
            // 
            btn_SaveUsers.Anchor = AnchorStyles.Bottom;
            btn_SaveUsers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_SaveUsers.Enabled = false;
            btn_SaveUsers.ImeMode = ImeMode.NoControl;
            btn_SaveUsers.Location = new Point(130, 480);
            btn_SaveUsers.Name = "btn_SaveUsers";
            btn_SaveUsers.Size = new Size(75, 23);
            btn_SaveUsers.TabIndex = 9;
            btn_SaveUsers.Text = "Сохранить ";
            btn_SaveUsers.UseVisualStyleBackColor = true;
            // 
            // dgv_Users
            // 
            dgv_Users.AllowUserToAddRows = false;
            dgv_Users.AllowUserToDeleteRows = false;
            dgv_Users.AllowUserToResizeColumns = false;
            dgv_Users.AllowUserToResizeRows = false;
            dgv_Users.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv_Users.BackgroundColor = SystemColors.ControlLightLight;
            dgv_Users.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_Users.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv_Users.Location = new Point(6, 22);
            dgv_Users.MultiSelect = false;
            dgv_Users.Name = "dgv_Users";
            dgv_Users.ReadOnly = true;
            dgv_Users.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dgv_Users.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_Users.Size = new Size(326, 452);
            dgv_Users.TabIndex = 1;
            // 
            // btn_DellUsers
            // 
            btn_DellUsers.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_DellUsers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_DellUsers.ImeMode = ImeMode.NoControl;
            btn_DellUsers.Location = new Point(257, 480);
            btn_DellUsers.Name = "btn_DellUsers";
            btn_DellUsers.Size = new Size(75, 23);
            btn_DellUsers.TabIndex = 2;
            btn_DellUsers.Text = "Удалить";
            btn_DellUsers.UseVisualStyleBackColor = true;
            // 
            // btn_EditUsers
            // 
            btn_EditUsers.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_EditUsers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_EditUsers.ImeMode = ImeMode.NoControl;
            btn_EditUsers.Location = new Point(6, 480);
            btn_EditUsers.Name = "btn_EditUsers";
            btn_EditUsers.Size = new Size(75, 23);
            btn_EditUsers.TabIndex = 1;
            btn_EditUsers.Text = "Изменить";
            btn_EditUsers.UseVisualStyleBackColor = true;
            // 
            // gb_PermUsers
            // 
            gb_PermUsers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_PermUsers.Controls.Add(gb_Commands);
            gb_PermUsers.Controls.Add(gb_Servers);
            gb_PermUsers.Controls.Add(btn_SavePerm);
            gb_PermUsers.Enabled = false;
            gb_PermUsers.Location = new Point(3, 3);
            gb_PermUsers.Name = "gb_PermUsers";
            gb_PermUsers.Size = new Size(839, 654);
            gb_PermUsers.TabIndex = 2;
            gb_PermUsers.TabStop = false;
            gb_PermUsers.Text = "Разрешения пользователей";
            // 
            // gb_Commands
            // 
            gb_Commands.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gb_Commands.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_Commands.Controls.Add(btn_OnAllCommands);
            gb_Commands.Controls.Add(clb_Commands);
            gb_Commands.Controls.Add(btn_OffAllCommands);
            gb_Commands.Location = new Point(433, 22);
            gb_Commands.Name = "gb_Commands";
            gb_Commands.Size = new Size(400, 596);
            gb_Commands.TabIndex = 2;
            gb_Commands.TabStop = false;
            gb_Commands.Text = "Команды";
            // 
            // btn_OnAllCommands
            // 
            btn_OnAllCommands.Anchor = AnchorStyles.Bottom;
            btn_OnAllCommands.ImeMode = ImeMode.NoControl;
            btn_OnAllCommands.Location = new Point(294, 556);
            btn_OnAllCommands.Name = "btn_OnAllCommands";
            btn_OnAllCommands.Size = new Size(100, 23);
            btn_OnAllCommands.TabIndex = 6;
            btn_OnAllCommands.Text = "Включить все";
            btn_OnAllCommands.UseVisualStyleBackColor = true;
            // 
            // clb_Commands
            // 
            clb_Commands.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            clb_Commands.CheckOnClick = true;
            clb_Commands.Font = new Font("Segoe UI", 12F);
            clb_Commands.FormattingEnabled = true;
            clb_Commands.Location = new Point(6, 18);
            clb_Commands.Name = "clb_Commands";
            clb_Commands.Size = new Size(388, 532);
            clb_Commands.TabIndex = 0;
            // 
            // btn_OffAllCommands
            // 
            btn_OffAllCommands.Anchor = AnchorStyles.Bottom;
            btn_OffAllCommands.ImeMode = ImeMode.NoControl;
            btn_OffAllCommands.Location = new Point(6, 556);
            btn_OffAllCommands.Name = "btn_OffAllCommands";
            btn_OffAllCommands.Size = new Size(100, 23);
            btn_OffAllCommands.TabIndex = 5;
            btn_OffAllCommands.Text = "Отключить все";
            btn_OffAllCommands.UseVisualStyleBackColor = true;
            // 
            // gb_Servers
            // 
            gb_Servers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gb_Servers.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gb_Servers.Controls.Add(btn_OnAllServers);
            gb_Servers.Controls.Add(clb_Servers);
            gb_Servers.Controls.Add(btn_OffAllServers);
            gb_Servers.Location = new Point(6, 22);
            gb_Servers.Name = "gb_Servers";
            gb_Servers.Size = new Size(400, 596);
            gb_Servers.TabIndex = 1;
            gb_Servers.TabStop = false;
            gb_Servers.Text = "Сервера";
            // 
            // btn_OnAllServers
            // 
            btn_OnAllServers.Anchor = AnchorStyles.Bottom;
            btn_OnAllServers.ImeMode = ImeMode.NoControl;
            btn_OnAllServers.Location = new Point(294, 556);
            btn_OnAllServers.Name = "btn_OnAllServers";
            btn_OnAllServers.Size = new Size(100, 23);
            btn_OnAllServers.TabIndex = 4;
            btn_OnAllServers.Text = "Включить все";
            btn_OnAllServers.UseVisualStyleBackColor = true;
            // 
            // clb_Servers
            // 
            clb_Servers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            clb_Servers.CheckOnClick = true;
            clb_Servers.Font = new Font("Segoe UI", 12F);
            clb_Servers.FormattingEnabled = true;
            clb_Servers.Location = new Point(6, 18);
            clb_Servers.Name = "clb_Servers";
            clb_Servers.Size = new Size(388, 532);
            clb_Servers.TabIndex = 0;
            // 
            // btn_OffAllServers
            // 
            btn_OffAllServers.Anchor = AnchorStyles.Bottom;
            btn_OffAllServers.ImeMode = ImeMode.NoControl;
            btn_OffAllServers.Location = new Point(6, 556);
            btn_OffAllServers.Name = "btn_OffAllServers";
            btn_OffAllServers.Size = new Size(100, 23);
            btn_OffAllServers.TabIndex = 3;
            btn_OffAllServers.Text = "Отключить все";
            btn_OffAllServers.UseVisualStyleBackColor = true;
            // 
            // btn_SavePerm
            // 
            btn_SavePerm.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_SavePerm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btn_SavePerm.Font = new Font("Segoe UI", 9F);
            btn_SavePerm.ImeMode = ImeMode.NoControl;
            btn_SavePerm.Location = new Point(382, 624);
            btn_SavePerm.Name = "btn_SavePerm";
            btn_SavePerm.Size = new Size(75, 25);
            btn_SavePerm.TabIndex = 0;
            btn_SavePerm.Text = "Сохранить ";
            btn_SavePerm.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            errorProvider1.ContainerControl = this;
            // 
            // UserControl_Users
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(gb_Users);
            Controls.Add(gb_PermUsers);
            Name = "UserControl_Users";
            Size = new Size(1190, 660);
            gb_Users.ResumeLayout(false);
            gb_NewUsers.ResumeLayout(false);
            gb_NewUsers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv_Users).EndInit();
            gb_PermUsers.ResumeLayout(false);
            gb_Commands.ResumeLayout(false);
            gb_Servers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)errorProvider1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gb_Users;
        private TextBox tb_IdUser;
        private TextBox tb_NameUser;
        private Button btn_AddUsers;
        private Button btn_DellUsers;
        private Button btn_EditUsers;
        private GroupBox gb_PermUsers;
        private GroupBox gb_Commands;
        private CheckedListBox clb_Commands;
        private GroupBox gb_Servers;
        private CheckedListBox clb_Servers;
        private Button btn_SavePerm;
        private DataGridView dgv_Users;
        private Button btn_SaveUsers;
        private ErrorProvider errorProvider1;
        private Button btn_OffAllServers;
        private Button btn_OnAllCommands;
        private Button btn_OffAllCommands;
        private Button btn_OnAllServers;
        private GroupBox gb_NewUsers;
    }
}
