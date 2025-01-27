namespace winforms
{
    partial class PasswordForm
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
            input_password = new TextBox();
            label_password = new Label();
            proceed_btn = new Button();
            cancel_btn = new Button();
            panel1 = new Panel();
            label_files = new Label();
            list_paths = new ListBox();
            window_bar.SuspendLayout();
            panel1.SuspendLayout();
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
            close_btn.Location = new Point(473, 0);
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
            window_bar.Size = new Size(523, 30);
            window_bar.TabIndex = 5;
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
            window_bar_title.Size = new Size(181, 17);
            window_bar_title.TabIndex = 0;
            window_bar_title.Text = "Quick Encrypt Password Form";
            window_bar_title.MouseDown += window_bar_MouseDown;
            window_bar_title.MouseMove += window_bar_MouseMove;
            window_bar_title.MouseUp += window_bar_MouseUp;
            // 
            // input_password
            // 
            input_password.Font = new Font("Segoe UI", 9F);
            input_password.Location = new Point(144, 113);
            input_password.Name = "input_password";
            input_password.PasswordChar = '*';
            input_password.Size = new Size(328, 25);
            input_password.TabIndex = 1;
            input_password.TextChanged += input_password_TextChanged;
            // 
            // label_password
            // 
            label_password.AutoSize = true;
            label_password.Font = new Font("Segoe UI", 9F);
            label_password.Location = new Point(33, 115);
            label_password.Name = "label_password";
            label_password.Size = new Size(110, 19);
            label_password.TabIndex = 99;
            label_password.Text = "Enter password: ";
            label_password.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // proceed_btn
            // 
            proceed_btn.Anchor = AnchorStyles.Right;
            proceed_btn.Location = new Point(389, 204);
            proceed_btn.Name = "proceed_btn";
            proceed_btn.Size = new Size(83, 25);
            proceed_btn.TabIndex = 100;
            proceed_btn.Text = "Encrypt";
            proceed_btn.UseVisualStyleBackColor = true;
            proceed_btn.Click += proceed_btn_Click;
            // 
            // cancel_btn
            // 
            cancel_btn.Anchor = AnchorStyles.Right;
            cancel_btn.Location = new Point(294, 204);
            cancel_btn.Name = "cancel_btn";
            cancel_btn.Size = new Size(83, 25);
            cancel_btn.TabIndex = 101;
            cancel_btn.Text = "Cancel";
            cancel_btn.UseVisualStyleBackColor = true;
            cancel_btn.Click += cancel_btn_Click;
            // 
            // panel1
            // 
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(label_files);
            panel1.Controls.Add(list_paths);
            panel1.Controls.Add(input_password);
            panel1.Controls.Add(cancel_btn);
            panel1.Controls.Add(label_password);
            panel1.Controls.Add(proceed_btn);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 30);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(30, 20, 30, 20);
            panel1.Size = new Size(523, 250);
            panel1.TabIndex = 102;
            // 
            // label_files
            // 
            label_files.AutoSize = true;
            label_files.Font = new Font("Segoe UI", 9F);
            label_files.Location = new Point(33, 23);
            label_files.Name = "label_files";
            label_files.Size = new Size(100, 19);
            label_files.TabIndex = 103;
            label_files.Text = "Selected paths:";
            // 
            // list_paths
            // 
            list_paths.DisplayMember = "as";
            list_paths.FormattingEnabled = true;
            list_paths.ItemHeight = 15;
            list_paths.Location = new Point(144, 23);
            list_paths.Name = "list_paths";
            list_paths.Size = new Size(328, 64);
            list_paths.TabIndex = 102;
            // 
            // PasswordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(523, 280);
            Controls.Add(panel1);
            Controls.Add(window_bar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "PasswordForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PasswordForm";
            window_bar.ResumeLayout(false);
            window_bar.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button close_btn;
        private Panel window_bar;
        private Label window_bar_title;
        private TextBox input_password;
        private Label label_password;
        private Button proceed_btn;
        private Button cancel_btn;
        private Panel panel1;
        private ListBox list_paths;
        private Label label_files;
    }
}