using EncryptionTool.cmd;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;
using System.Text;
using System.Windows.Forms;

namespace winforms;

public partial class MainForm : Form
{
    readonly ConfigService configService = new();

    bool moveForm;
    Point moveFormPosition = Point.Empty;

    public MainForm()
    {
        InitializeComponent();
    }

    void MainForm_Load(object sender, EventArgs e)
    {
        loadGroupsToGroupList();
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

    // Form controls

    void close_btn_MouseUp(object sender, MouseEventArgs e)
    {
        var mousePos = close_btn.PointToClient(Cursor.Position);

        if (close_btn.ClientRectangle.Contains(mousePos)) Close();
    }

    // Form actions

    void encrypt_file_btn_MouseClick(object sender, MouseEventArgs e)
    {
        var command = new Command { Action = CommandAction.encrypt };

        hideResultLabels();

        if (e.Button == MouseButtons.Left)
        {
            if (openFilesDialog.ShowDialog() == DialogResult.OK)
            {
                command.AddPaths(openFilesDialog.FileNames);

                var passwordForm = new PasswordForm(command.Paths);
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(passwordForm.password));

                    try
                    {
                        processing_dots.Visible = true;
                        Commands.Encrypt(command);
                        setResultLabels(true, "encrypted", command.Paths.Count <= 1 ? "file" : "files");
                    }
                    catch (Exception ex)
                    {
                        setResultLabels(false, "encrypt", command.Paths.Count <= 1 ? "file" : "files");
                    }
                    finally
                    {
                        processing_dots.Visible = false;
                    }
                }
            }
        }
    }

    void encrypt_folder_btn_MouseClick(object sender, MouseEventArgs e)
    {
        var command = new Command { Action = CommandAction.encrypt };

        hideResultLabels();

        if (e.Button == MouseButtons.Left)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                command.AddPath(folderBrowserDialog.SelectedPath);

                var passwordForm = new PasswordForm(command.Paths);
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(passwordForm.password));

                    try
                    {
                        processing_dots.Visible = true;
                        Commands.Encrypt(command);
                        setResultLabels(true, "encrypted", "folder");
                    }
                    catch (Exception ex)
                    {
                        setResultLabels(false, "encrypt", "folder");
                    }
                    finally
                    {
                        processing_dots.Visible = false;
                    }
                }
            }
        }
    }

    void decrypt_btn_MouseClick(object sender, MouseEventArgs e)
    {
        var command = new Command { Action = CommandAction.decrypt };

        hideResultLabels();

        if (e.Button == MouseButtons.Left)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                command.AddPaths(openFileDialog.FileNames);

                var passwordForm = new PasswordForm(command.Paths, command.Action);
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(passwordForm.password));

                    try
                    {
                        processing_dots.Visible = true;
                        Commands.Decrypt(command);
                        setResultLabels(true, "decrypt", "file");
                    }
                    catch (Exception)
                    {
                        setResultLabels(false, "decrypt", "file");
                    }
                    finally
                    {
                        processing_dots.Visible = false;
                    }
                }
            }
        }
    }

    // Groups

    void group_list_SelectedIndexChanged(object sender, EventArgs e)
    {
        handleGroupListSelectionChange();
    }

    void group_execute_btn_MouseClick(object sender, EventArgs e)
    {
        var selectedGroup = group_list.SelectedItems[0].Text;
        var groupItem = configService.GetGroup(selectedGroup);

        if (groupItem == null) return;

        hideResultLabels();

        var command = new Command
        {
            Action = groupItem.Action,
            Delete = groupItem.Delete,
            Overwrite = groupItem.Overwrite
        };
        command.AddPaths(groupItem.Paths);

        var passwordForm = new PasswordForm(command.Paths);
        if (passwordForm.ShowDialog() == DialogResult.OK)
        {
            command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(passwordForm.password));

            try
            {
                processing_dots.Visible = true;
                Commands.Encrypt(command);
                setResultLabels(true, $"{command.Action.ToString()}ed", command.Paths.Count <= 1 ? "path" : "paths");
            }
            catch (Exception)
            {
                setResultLabels(false, $"{command.Action.ToString()}", command.Paths.Count <= 1 ? "path" : "paths");
            }
            finally
            {
                processing_dots.Visible = false;
            }
        }
    }

    void group_delete_btn_MouseClick(object sender, MouseEventArgs e)
    {
        var selectedGroup = group_list.SelectedItems[0].Text;
        configService.DeleteGroup(selectedGroup);
        loadGroupsToGroupList();
    }

    void group_edit_btn_MouseClick(object sender, MouseEventArgs e)
    {
        var groupName = group_list.SelectedItems[0].Text;
        var groupItem = configService.GetGroup(groupName);

        if (groupItem == null) return;

        var groupItemCopy = new GroupItem
        {
            Action = groupItem.Action,
            Delete = groupItem.Delete,
            Overwrite = groupItem.Overwrite,
        };
        groupItemCopy.AddPaths(groupItem.Paths);

        var editForm = new GroupEditForm(groupItemCopy, groupName);

        if (editForm.ShowDialog() == DialogResult.OK)
        {
            var groupItemEdit = editForm.groupItem;
            var groupNameEdit = editForm.groupName;

            var groupCommand = new GroupCommand
            {
                Action = groupItemEdit.Action,
                Delete = groupItemEdit.Delete,
                Overwrite = groupItemEdit.Overwrite,
                Name = groupNameEdit
            };
            groupCommand.AddPaths(groupItemEdit.Paths);

            configService.DeleteGroup(groupName);
            configService.SaveGroup(groupCommand);

            loadGroupsToGroupList();
        }
    }

    void group_add_btn_MouseClick(object sender, MouseEventArgs e)
    {
        var editForm = new GroupEditForm();

        if (editForm.ShowDialog() == DialogResult.OK)
        {
            var groupItemEdit = editForm.groupItem;
            var groupNameEdit = editForm.groupName;

            var groupCommand = new GroupCommand
            {
                Action = groupItemEdit.Action,
                Delete = groupItemEdit.Delete,
                Overwrite = groupItemEdit.Overwrite,
                Name = groupNameEdit
            };
            groupCommand.AddPaths(groupItemEdit.Paths);

            configService.SaveGroup(groupCommand);

            loadGroupsToGroupList();
        }
    }

    // Action labels

    void setResultLabels(bool result, string action, string filetype)
    {
        if (result)
        {
            label_success.Visible = true;
            label_success.Text = $"{StringUtils.FirstLetterToUpper(filetype)} successfully {action}";
        }

        if (!result)
        {
            label_error.Visible = true;
            label_error.Text = $"Failed to {action} {filetype}";
        }
    }

    void hideResultLabels()
    {
        label_error.Visible = false;
        label_success.Visible = false;
    }

    // Utils

    void loadGroupsToGroupList()
    {
        group_list.Items.Clear();

        foreach (var kvp in configService.Groups)
        {
            var listitem = new ListViewItem(kvp.Key);
            listitem.SubItems.Add(kvp.Value.Action.ToString());
            group_list.Items.Add(listitem);
        }

        handleGroupListSelectionChange();
    }

    void handleGroupListSelectionChange()
    {
        if (group_list.SelectedItems.Count == 0)
        {
            group_execute_btn.Enabled = false;
            group_edit_btn.Enabled = false;
            group_delete_btn.Enabled = false;
            return;
        }

        group_execute_btn.Enabled = true;
        group_edit_btn.Enabled = true;
        group_delete_btn.Enabled = true;
    }
}