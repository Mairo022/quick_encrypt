using EncryptionTool.models;

namespace winforms
{
    public partial class GroupEditForm : Form
    {
        public GroupItem groupItem;
        public string groupName;

        bool moveForm;
        Point moveFormPosition = Point.Empty;

        public GroupEditForm(GroupItem? groupItem = null, string? groupName = null)
        {
            InitializeComponent();
            this.groupItem = groupItem ?? new GroupItem { Action = CommandAction.encrypt };
            this.groupName = groupName ?? "Undefined";
        }

        void GroupEditForm_Load(object sender, EventArgs e)
        {
            group_name_input.Text = groupName;
            foreach (var path in groupItem.Paths) list_paths.Items.Add(path);
            overwrite_checkbox.Checked = groupItem.Overwrite;
            delete_checkbox.Checked = groupItem.Delete;
            if (CommandAction.encrypt == groupItem.Action) action_encrypt_radio.Checked = true;
            if (CommandAction.decrypt == groupItem.Action) action_decrypt_radio.Checked = true;
        }

        // Window bar

        void window_bar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveForm = true;
                moveFormPosition = e.Location;
            }
        }

        void window_bar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                moveForm = false;
            }
        }

        void window_bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveForm)
            {
                var currentScreenPos = PointToScreen(e.Location);

                Location = new Point(
                    currentScreenPos.X - moveFormPosition.X,
                    currentScreenPos.Y - moveFormPosition.Y);
            }
        }
        void window_bar_label_MouseDown(object sender, MouseEventArgs e)
        {
            window_bar_MouseDown(sender, e);
        }

        // Group settings

        void group_name_input_TextChanged(object sender, EventArgs e)
        {
            groupName = group_name_input.Text;
        }

        void add_folder_btn_MouseClick(object sender, MouseEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                list_paths.Items.Add(folderBrowserDialog.SelectedPath);
                groupItem.AddPath(folderBrowserDialog.SelectedPath);
            }
        }

        void add_file_btn_MouseClick(object sender, MouseEventArgs e)
        {
            if (openFilesDialog.ShowDialog() == DialogResult.OK)
            {
                list_paths.Items.AddRange(openFilesDialog.FileNames);
                groupItem.AddPaths(openFilesDialog.FileNames);
            }
        }

        void delete_path_btn_MouseClick(object sender, MouseEventArgs e)
        {
            groupItem.RemovePath(list_paths.SelectedItem!.ToString()!);
            list_paths.Items.Remove(list_paths.SelectedItem!);
        }

        void action_encrypt_radio_CheckedChanged(object sender, EventArgs e)
        {
            if (action_encrypt_radio.Checked) groupItem.Action = CommandAction.encrypt;
        }

        void action_decrypt_radio_CheckedChanged(object sender, EventArgs e)
        {
            if (action_decrypt_radio.Checked) groupItem.Action = CommandAction.decrypt;
        }

        void overwrite_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            groupItem.Overwrite = overwrite_checkbox.Checked;
        }

        void delete_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            groupItem.Delete = delete_checkbox.Checked;
        }

        void list_paths_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list_paths.SelectedItem == null)
                delete_path_btn.Enabled = false;
            else delete_path_btn.Enabled = true;
        }

        // Form controls

        void proceed_btn_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        void cancel_btn_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        void close_btn_MouseUp(object sender, MouseEventArgs e)
        {
            var mousePos = close_btn.PointToClient(Cursor.Position);

            if (close_btn.ClientRectangle.Contains(mousePos)) DialogResult = DialogResult.Cancel;
        }
    }
}
