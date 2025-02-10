namespace winforms;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        window_bar = new Panel();
        window_bar_label = new Label();
        close_btn = new Button();
        panel2 = new Panel();
        selected_pathlist = new ListBox();
        selected_label = new Label();
        selected_path_label = new Label();
        delete_btn = new Button();
        encrypt_btn = new Button();
        group_delete_btn = new Button();
        group_add_btn = new Button();
        group_edit_btn = new Button();
        group_execute_btn = new Button();
        group_list = new ListView();
        columnName = new ColumnHeader();
        columnAction = new ColumnHeader();
        label_error = new Label();
        label_success = new Label();
        select_folder_btn = new Button();
        processing_dots = new PictureBox();
        decrypt_btn = new Button();
        select_file_btn = new Button();
        openFilesDialog = new OpenFileDialog();
        folderBrowserDialog = new FolderBrowserDialog();
        openFileDialog = new OpenFileDialog();
        window_bar.SuspendLayout();
        panel2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)processing_dots).BeginInit();
        SuspendLayout();
        // 
        // window_bar
        // 
        window_bar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        window_bar.BackColor = Color.FromArgb(224, 224, 224);
        window_bar.Controls.Add(window_bar_label);
        window_bar.Controls.Add(close_btn);
        window_bar.Dock = DockStyle.Top;
        window_bar.Location = new Point(0, 0);
        window_bar.Name = "window_bar";
        window_bar.Size = new Size(400, 30);
        window_bar.TabIndex = 0;
        window_bar.MouseDown += window_bar_MouseDown;
        window_bar.MouseMove += window_bar_MouseMove;
        window_bar.MouseUp += window_bar_MouseUp;
        // 
        // window_bar_label
        // 
        window_bar_label.AutoSize = true;
        window_bar_label.Font = new Font("Segoe UI", 8.830189F, FontStyle.Regular, GraphicsUnit.Point, 0);
        window_bar_label.ForeColor = Color.Black;
        window_bar_label.Location = new Point(12, 7);
        window_bar_label.Name = "window_bar_label";
        window_bar_label.Size = new Size(87, 17);
        window_bar_label.TabIndex = 0;
        window_bar_label.Text = "Quick Encrypt";
        window_bar_label.MouseDown += window_bar_MouseDown;
        window_bar_label.MouseMove += window_bar_MouseMove;
        window_bar_label.MouseUp += window_bar_MouseUp;
        // 
        // close_btn
        // 
        close_btn.BackgroundImage = Properties.Resources.Close;
        close_btn.BackgroundImageLayout = ImageLayout.Center;
        close_btn.Dock = DockStyle.Right;
        close_btn.FlatAppearance.BorderSize = 0;
        close_btn.FlatStyle = FlatStyle.Flat;
        close_btn.ForeColor = Color.FromArgb(224, 224, 224);
        close_btn.Location = new Point(350, 0);
        close_btn.Margin = new Padding(0);
        close_btn.Name = "close_btn";
        close_btn.Size = new Size(50, 30);
        close_btn.TabIndex = 0;
        close_btn.Text = " ";
        close_btn.UseVisualStyleBackColor = true;
        close_btn.MouseUp += close_btn_MouseUp;
        // 
        // panel2
        // 
        panel2.Controls.Add(selected_pathlist);
        panel2.Controls.Add(selected_label);
        panel2.Controls.Add(selected_path_label);
        panel2.Controls.Add(delete_btn);
        panel2.Controls.Add(encrypt_btn);
        panel2.Controls.Add(group_delete_btn);
        panel2.Controls.Add(group_add_btn);
        panel2.Controls.Add(group_edit_btn);
        panel2.Controls.Add(group_execute_btn);
        panel2.Controls.Add(group_list);
        panel2.Controls.Add(label_error);
        panel2.Controls.Add(label_success);
        panel2.Controls.Add(select_folder_btn);
        panel2.Controls.Add(processing_dots);
        panel2.Controls.Add(decrypt_btn);
        panel2.Controls.Add(select_file_btn);
        panel2.Dock = DockStyle.Fill;
        panel2.Location = new Point(0, 30);
        panel2.Margin = new Padding(0);
        panel2.Name = "panel2";
        panel2.Padding = new Padding(35, 40, 35, 40);
        panel2.RightToLeft = RightToLeft.No;
        panel2.Size = new Size(400, 569);
        panel2.TabIndex = 1;
        // 
        // selected_pathlist
        // 
        selected_pathlist.DisplayMember = "as";
        selected_pathlist.FormattingEnabled = true;
        selected_pathlist.HorizontalScrollbar = true;
        selected_pathlist.ItemHeight = 17;
        selected_pathlist.Location = new Point(149, 121);
        selected_pathlist.Name = "selected_pathlist";
        selected_pathlist.Size = new Size(213, 106);
        selected_pathlist.TabIndex = 104;
        selected_pathlist.Visible = false;
        // 
        // selected_label
        // 
        selected_label.Font = new Font("Segoe UI", 8.830189F, FontStyle.Bold, GraphicsUnit.Point, 0);
        selected_label.Location = new Point(149, 100);
        selected_label.Name = "selected_label";
        selected_label.Size = new Size(213, 19);
        selected_label.TabIndex = 16;
        selected_label.Text = "Selected";
        selected_label.TextAlign = ContentAlignment.MiddleLeft;
        selected_label.Visible = false;
        // 
        // selected_path_label
        // 
        selected_path_label.Location = new Point(149, 122);
        selected_path_label.Name = "selected_path_label";
        selected_path_label.Size = new Size(213, 105);
        selected_path_label.TabIndex = 15;
        selected_path_label.Text = "E:\\Downloads\\ComputerEnhance\\A First Look at Profiling Overhead.mp4";
        selected_path_label.Visible = false;
        // 
        // delete_btn
        // 
        delete_btn.Enabled = false;
        delete_btn.Location = new Point(38, 179);
        delete_btn.Name = "delete_btn";
        delete_btn.Padding = new Padding(6, 0, 6, 0);
        delete_btn.Size = new Size(90, 30);
        delete_btn.TabIndex = 14;
        delete_btn.Text = "Delete";
        delete_btn.TextAlign = ContentAlignment.MiddleLeft;
        delete_btn.UseVisualStyleBackColor = true;
        delete_btn.Click += delete_btn_Click;
        // 
        // encrypt_btn
        // 
        encrypt_btn.Enabled = false;
        encrypt_btn.Location = new Point(38, 103);
        encrypt_btn.Name = "encrypt_btn";
        encrypt_btn.Padding = new Padding(6, 0, 6, 0);
        encrypt_btn.Size = new Size(90, 30);
        encrypt_btn.TabIndex = 13;
        encrypt_btn.Text = "Encrypt";
        encrypt_btn.TextAlign = ContentAlignment.MiddleLeft;
        encrypt_btn.UseVisualStyleBackColor = true;
        encrypt_btn.Click += encrypt_btn_Click;
        // 
        // group_delete_btn
        // 
        group_delete_btn.BackgroundImage = Properties.Resources.Delete;
        group_delete_btn.BackgroundImageLayout = ImageLayout.Center;
        group_delete_btn.Enabled = false;
        group_delete_btn.Location = new Point(334, 271);
        group_delete_btn.Name = "group_delete_btn";
        group_delete_btn.Size = new Size(28, 25);
        group_delete_btn.TabIndex = 12;
        group_delete_btn.UseVisualStyleBackColor = true;
        group_delete_btn.MouseClick += group_delete_btn_MouseClick;
        // 
        // group_add_btn
        // 
        group_add_btn.BackgroundImage = Properties.Resources.Add;
        group_add_btn.BackgroundImageLayout = ImageLayout.Center;
        group_add_btn.Location = new Point(334, 335);
        group_add_btn.Name = "group_add_btn";
        group_add_btn.Size = new Size(28, 25);
        group_add_btn.TabIndex = 11;
        group_add_btn.UseVisualStyleBackColor = true;
        group_add_btn.MouseClick += group_add_btn_MouseClick;
        // 
        // group_edit_btn
        // 
        group_edit_btn.BackgroundImage = Properties.Resources.Edit;
        group_edit_btn.BackgroundImageLayout = ImageLayout.Center;
        group_edit_btn.Enabled = false;
        group_edit_btn.Location = new Point(334, 303);
        group_edit_btn.Name = "group_edit_btn";
        group_edit_btn.Size = new Size(28, 25);
        group_edit_btn.TabIndex = 10;
        group_edit_btn.UseVisualStyleBackColor = true;
        group_edit_btn.MouseClick += group_edit_btn_MouseClick;
        // 
        // group_execute_btn
        // 
        group_execute_btn.BackgroundImage = Properties.Resources.Play2;
        group_execute_btn.BackgroundImageLayout = ImageLayout.Center;
        group_execute_btn.Enabled = false;
        group_execute_btn.Location = new Point(334, 239);
        group_execute_btn.Name = "group_execute_btn";
        group_execute_btn.Size = new Size(28, 25);
        group_execute_btn.TabIndex = 8;
        group_execute_btn.UseVisualStyleBackColor = true;
        group_execute_btn.MouseClick += group_execute_btn_MouseClick;
        // 
        // group_list
        // 
        group_list.Columns.AddRange(new ColumnHeader[] { columnName, columnAction });
        group_list.FullRowSelect = true;
        group_list.Location = new Point(38, 240);
        group_list.MultiSelect = false;
        group_list.Name = "group_list";
        group_list.Size = new Size(282, 200);
        group_list.Sorting = SortOrder.Ascending;
        group_list.TabIndex = 6;
        group_list.UseCompatibleStateImageBehavior = false;
        group_list.View = View.Details;
        group_list.SelectedIndexChanged += group_list_SelectedIndexChanged;
        // 
        // columnName
        // 
        columnName.Text = "Group name";
        columnName.Width = 176;
        // 
        // columnAction
        // 
        columnAction.Text = "Action";
        columnAction.Width = 102;
        // 
        // label_error
        // 
        label_error.ForeColor = Color.Tomato;
        label_error.Location = new Point(52, 501);
        label_error.Name = "label_error";
        label_error.Size = new Size(293, 22);
        label_error.TabIndex = 5;
        label_error.Text = "Error";
        label_error.TextAlign = ContentAlignment.MiddleCenter;
        label_error.Visible = false;
        // 
        // label_success
        // 
        label_success.ForeColor = Color.MediumSeaGreen;
        label_success.Location = new Point(52, 501);
        label_success.Name = "label_success";
        label_success.Size = new Size(293, 22);
        label_success.TabIndex = 4;
        label_success.Text = "Success";
        label_success.TextAlign = ContentAlignment.MiddleCenter;
        label_success.Visible = false;
        // 
        // select_folder_btn
        // 
        select_folder_btn.Location = new Point(207, 39);
        select_folder_btn.Name = "select_folder_btn";
        select_folder_btn.Padding = new Padding(10, 0, 10, 0);
        select_folder_btn.Size = new Size(155, 34);
        select_folder_btn.TabIndex = 3;
        select_folder_btn.Text = "Select folder";
        select_folder_btn.UseVisualStyleBackColor = true;
        select_folder_btn.Click += select_folder_btn_Click;
        // 
        // processing_dots
        // 
        processing_dots.Dock = DockStyle.Bottom;
        processing_dots.ImageLocation = "C:\\Users\\Nyx\\source\\repos\\EncryptionTool\\winforms\\img\\processing.gif";
        processing_dots.Location = new Point(35, 507);
        processing_dots.Name = "processing_dots";
        processing_dots.Size = new Size(330, 22);
        processing_dots.SizeMode = PictureBoxSizeMode.CenterImage;
        processing_dots.TabIndex = 2;
        processing_dots.TabStop = false;
        processing_dots.Visible = false;
        // 
        // decrypt_btn
        // 
        decrypt_btn.Enabled = false;
        decrypt_btn.Location = new Point(38, 141);
        decrypt_btn.Margin = new Padding(0);
        decrypt_btn.Name = "decrypt_btn";
        decrypt_btn.Padding = new Padding(6, 0, 6, 0);
        decrypt_btn.Size = new Size(90, 30);
        decrypt_btn.TabIndex = 1;
        decrypt_btn.Text = "Decrypt";
        decrypt_btn.TextAlign = ContentAlignment.MiddleLeft;
        decrypt_btn.UseVisualStyleBackColor = true;
        decrypt_btn.Click += decrypt_btn_Click;
        // 
        // select_file_btn
        // 
        select_file_btn.Location = new Point(38, 39);
        select_file_btn.Name = "select_file_btn";
        select_file_btn.Padding = new Padding(10, 0, 10, 0);
        select_file_btn.Size = new Size(155, 34);
        select_file_btn.TabIndex = 0;
        select_file_btn.Text = "Select file(s)";
        select_file_btn.UseVisualStyleBackColor = true;
        select_file_btn.Click += select_file_btn_Click;
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
        // openFileDialog
        // 
        openFileDialog.Title = "Select File";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(400, 599);
        Controls.Add(panel2);
        Controls.Add(window_bar);
        Font = new Font("Segoe UI", 8.830189F, FontStyle.Regular, GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.None;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quick Encrypt";
        Load += MainForm_Load;
        window_bar.ResumeLayout(false);
        window_bar.PerformLayout();
        panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)processing_dots).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel window_bar;
    private Button close_btn;
    private Panel panel2;
    private Label window_bar_label;
    private Button select_file_btn;
    private Button decrypt_btn;
    private OpenFileDialog openFilesDialog;
    private PictureBox processing_dots;
    private Button select_folder_btn;
    private Label label_error;
    private Label label_success;
    private FolderBrowserDialog folderBrowserDialog;
    private OpenFileDialog openFileDialog;
    private ListView group_list;
    private Button group_execute_btn;
    private ColumnHeader columnName;
    private ColumnHeader columnAction;
    private Button group_edit_btn;
    private Button group_add_btn;
    private Button group_delete_btn;
    private Button encrypt_btn;
    private Button delete_btn;
    private Label selected_path_label;
    private Label selected_label;
    private ListBox selected_pathlist;
}