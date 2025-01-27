namespace winforms
{
    partial class GroupEditForm
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
            close_btn = new Button();
            window_bar = new Panel();
            window_bar_title = new Label();
            list_paths = new ListBox();
            panel1 = new Panel();
            group_name_label = new Label();
            group_name_input = new TextBox();
            cancel_btn = new Button();
            proceed_btn = new Button();
            groupbox_action = new GroupBox();
            action_encrypt_radio = new RadioButton();
            action_decrypt_radio = new RadioButton();
            delete_checkbox = new CheckBox();
            overwrite_checkbox = new CheckBox();
            delete_path_btn = new Button();
            add_file_btn = new Button();
            add_folder_btn = new Button();
            openFilesDialog = new OpenFileDialog();
            folderBrowserDialog = new FolderBrowserDialog();
            window_bar.SuspendLayout();
            panel1.SuspendLayout();
            groupbox_action.SuspendLayout();
            SuspendLayout();
            // 
            // close_btn
            // 
            close_btn.BackgroundImage = Properties.Resources.Close;
            close_btn.BackgroundImageLayout = ImageLayout.Center;
            close_btn.Dock = DockStyle.Right;
            close_btn.FlatAppearance.BorderSize = 0;
            close_btn.FlatStyle = FlatStyle.Flat;
            close_btn.ForeColor = Color.FromArgb(224, 224, 224);
            close_btn.Location = new Point(500, 0);
            close_btn.Margin = new Padding(0);
            close_btn.Name = "close_btn";
            close_btn.Size = new Size(50, 30);
            close_btn.TabIndex = 4;
            close_btn.Text = " ";
            close_btn.UseVisualStyleBackColor = true;
            close_btn.MouseUp += close_btn_MouseUp;
            // 
            // window_bar
            // 
            window_bar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            window_bar.BackColor = Color.FromArgb(224, 224, 224);
            window_bar.Controls.Add(window_bar_title);
            window_bar.Controls.Add(close_btn);
            window_bar.Dock = DockStyle.Top;
            window_bar.Location = new Point(0, 0);
            window_bar.Name = "window_bar";
            window_bar.Size = new Size(550, 30);
            window_bar.TabIndex = 6;
            window_bar.MouseDown += window_bar_MouseDown;
            window_bar.MouseMove += window_bar_MouseMove;
            window_bar.MouseUp += window_bar_MouseUp;
            // 
            // window_bar_title
            // 
            window_bar_title.AutoSize = true;
            window_bar_title.Font = new Font("Segoe UI", 8.830189F, FontStyle.Regular, GraphicsUnit.Point, 0);
            window_bar_title.ForeColor = Color.Black;
            window_bar_title.Location = new Point(12, 7);
            window_bar_title.Name = "window_bar_title";
            window_bar_title.Size = new Size(167, 17);
            window_bar_title.TabIndex = 0;
            window_bar_title.Text = "Quick Encrypt Group Editor";
            window_bar_title.MouseDown += window_bar_label_MouseDown;
            // 
            // list_paths
            // 
            list_paths.DisplayMember = "as";
            list_paths.FormattingEnabled = true;
            list_paths.HorizontalScrollbar = true;
            list_paths.ItemHeight = 15;
            list_paths.Location = new Point(29, 51);
            list_paths.Name = "list_paths";
            list_paths.Size = new Size(488, 109);
            list_paths.TabIndex = 103;
            list_paths.SelectedIndexChanged += list_paths_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(group_name_label);
            panel1.Controls.Add(group_name_input);
            panel1.Controls.Add(cancel_btn);
            panel1.Controls.Add(proceed_btn);
            panel1.Controls.Add(groupbox_action);
            panel1.Controls.Add(delete_checkbox);
            panel1.Controls.Add(overwrite_checkbox);
            panel1.Controls.Add(delete_path_btn);
            panel1.Controls.Add(add_file_btn);
            panel1.Controls.Add(add_folder_btn);
            panel1.Controls.Add(list_paths);
            panel1.Location = new Point(-1, 27);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(30, 20, 30, 20);
            panel1.Size = new Size(550, 313);
            panel1.TabIndex = 104;
            // 
            // group_name_label
            // 
            group_name_label.AutoSize = true;
            group_name_label.Location = new Point(26, 21);
            group_name_label.Name = "group_name_label";
            group_name_label.Size = new Size(50, 16);
            group_name_label.TabIndex = 115;
            group_name_label.Text = "Name: ";
            // 
            // group_name_input
            // 
            group_name_input.Font = new Font("Microsoft Sans Serif", 9F);
            group_name_input.Location = new Point(76, 18);
            group_name_input.Name = "group_name_input";
            group_name_input.Size = new Size(146, 22);
            group_name_input.TabIndex = 114;
            group_name_input.TextChanged += group_name_input_TextChanged;
            // 
            // cancel_btn
            // 
            cancel_btn.Location = new Point(336, 260);
            cancel_btn.Name = "cancel_btn";
            cancel_btn.Size = new Size(83, 25);
            cancel_btn.TabIndex = 113;
            cancel_btn.Text = "Cancel";
            cancel_btn.UseVisualStyleBackColor = true;
            cancel_btn.MouseClick += cancel_btn_MouseClick;
            // 
            // proceed_btn
            // 
            proceed_btn.Location = new Point(434, 260);
            proceed_btn.Name = "proceed_btn";
            proceed_btn.Size = new Size(83, 25);
            proceed_btn.TabIndex = 112;
            proceed_btn.Text = "Save";
            proceed_btn.UseVisualStyleBackColor = true;
            proceed_btn.MouseClick += proceed_btn_MouseClick;
            // 
            // groupbox_action
            // 
            groupbox_action.BackColor = Color.Transparent;
            groupbox_action.Controls.Add(action_encrypt_radio);
            groupbox_action.Controls.Add(action_decrypt_radio);
            groupbox_action.Location = new Point(29, 201);
            groupbox_action.Name = "groupbox_action";
            groupbox_action.Size = new Size(206, 50);
            groupbox_action.TabIndex = 111;
            groupbox_action.TabStop = false;
            groupbox_action.Text = "Action";
            // 
            // action_encrypt_radio
            // 
            action_encrypt_radio.AutoSize = true;
            action_encrypt_radio.Checked = true;
            action_encrypt_radio.Location = new Point(15, 20);
            action_encrypt_radio.Name = "action_encrypt_radio";
            action_encrypt_radio.Size = new Size(70, 20);
            action_encrypt_radio.TabIndex = 109;
            action_encrypt_radio.TabStop = true;
            action_encrypt_radio.Text = "Encrypt";
            action_encrypt_radio.UseVisualStyleBackColor = true;
            action_encrypt_radio.CheckedChanged += action_encrypt_radio_CheckedChanged;
            // 
            // action_decrypt_radio
            // 
            action_decrypt_radio.AutoSize = true;
            action_decrypt_radio.Location = new Point(109, 20);
            action_decrypt_radio.Name = "action_decrypt_radio";
            action_decrypt_radio.Size = new Size(72, 20);
            action_decrypt_radio.TabIndex = 110;
            action_decrypt_radio.Text = "Decrypt";
            action_decrypt_radio.UseMnemonic = false;
            action_decrypt_radio.UseVisualStyleBackColor = true;
            action_decrypt_radio.CheckedChanged += action_decrypt_radio_CheckedChanged;
            // 
            // delete_checkbox
            // 
            delete_checkbox.AutoSize = true;
            delete_checkbox.Location = new Point(242, 230);
            delete_checkbox.Name = "delete_checkbox";
            delete_checkbox.Size = new Size(113, 20);
            delete_checkbox.TabIndex = 108;
            delete_checkbox.Text = "Delete original";
            delete_checkbox.UseVisualStyleBackColor = true;
            delete_checkbox.CheckedChanged += delete_checkbox_CheckedChanged;
            // 
            // overwrite_checkbox
            // 
            overwrite_checkbox.AutoSize = true;
            overwrite_checkbox.Location = new Point(242, 208);
            overwrite_checkbox.Name = "overwrite_checkbox";
            overwrite_checkbox.Size = new Size(82, 20);
            overwrite_checkbox.TabIndex = 107;
            overwrite_checkbox.Text = "Overwrite";
            overwrite_checkbox.UseVisualStyleBackColor = true;
            overwrite_checkbox.CheckedChanged += overwrite_checkbox_CheckedChanged;
            // 
            // delete_path_btn
            // 
            delete_path_btn.Enabled = false;
            delete_path_btn.Location = new Point(240, 169);
            delete_path_btn.Name = "delete_path_btn";
            delete_path_btn.Padding = new Padding(10, 0, 10, 0);
            delete_path_btn.Size = new Size(100, 25);
            delete_path_btn.TabIndex = 106;
            delete_path_btn.Text = "Delete";
            delete_path_btn.UseVisualStyleBackColor = true;
            delete_path_btn.MouseClick += delete_path_btn_MouseClick;
            // 
            // add_file_btn
            // 
            add_file_btn.Location = new Point(134, 169);
            add_file_btn.Name = "add_file_btn";
            add_file_btn.Padding = new Padding(10, 0, 10, 0);
            add_file_btn.Size = new Size(100, 25);
            add_file_btn.TabIndex = 105;
            add_file_btn.Text = "Add file";
            add_file_btn.UseVisualStyleBackColor = true;
            add_file_btn.MouseClick += add_file_btn_MouseClick;
            // 
            // add_folder_btn
            // 
            add_folder_btn.Location = new Point(28, 169);
            add_folder_btn.Name = "add_folder_btn";
            add_folder_btn.Padding = new Padding(10, 0, 10, 0);
            add_folder_btn.Size = new Size(100, 25);
            add_folder_btn.TabIndex = 104;
            add_folder_btn.Text = "Add folder";
            add_folder_btn.UseVisualStyleBackColor = true;
            add_folder_btn.MouseClick += add_folder_btn_MouseClick;
            // 
            // openFilesDialog
            // 
            openFilesDialog.Multiselect = true;
            openFilesDialog.Title = "Select File(s)";
            // 
            // folderBrowserDialog
            // 
            folderBrowserDialog.AddToRecent = false;
            // 
            // GroupEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(550, 340);
            Controls.Add(panel1);
            Controls.Add(window_bar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "GroupEditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "GroupEditForm";
            Load += GroupEditForm_Load;
            window_bar.ResumeLayout(false);
            window_bar.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            groupbox_action.ResumeLayout(false);
            groupbox_action.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button close_btn;
        private Panel window_bar;
        private Label window_bar_title;
        private ListBox list_paths;
        private Panel panel1;
        private Button add_folder_btn;
        private Button add_file_btn;
        private Button delete_path_btn;
        private RadioButton action_encrypt_radio;
        private CheckBox delete_checkbox;
        private CheckBox overwrite_checkbox;
        private GroupBox groupbox_action;
        private RadioButton action_decrypt_radio;
        private Button cancel_btn;
        private Button proceed_btn;
        private OpenFileDialog openFilesDialog;
        private FolderBrowserDialog folderBrowserDialog;
        private TextBox group_name_input;
        private Label group_name_label;
    }
}