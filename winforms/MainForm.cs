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
    readonly List<string> selectedPaths = [];

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

    void select_file_btn_Click(object sender, EventArgs e)
    {
        if (openFilesDialog.ShowDialog() != DialogResult.OK) return;

        resetSelected();
        setActionButtonsStatus(true);
        selected_label.Visible = true;

        if (openFilesDialog.FileNames.Length > 1)
        {
            selectedPaths.AddRange(openFilesDialog.FileNames);
            selected_pathlist.Items.AddRange(openFilesDialog.FileNames);
            selected_pathlist.Visible = true;
            return;
        }

        selectedPaths.Add(openFilesDialog.FileNames[0]);
        selected_path_label.Text = openFilesDialog.FileNames[0];
        selected_path_label.Visible = true;
    }

    void select_folder_btn_Click(object sender, EventArgs e)
    {
        if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;

        resetSelected();
        setActionButtonsStatus(true);

        selectedPaths.Add(folderBrowserDialog.SelectedPath);

        selected_path_label.Text = folderBrowserDialog.SelectedPath;
        selected_path_label.Visible = true;
        selected_label.Visible = true;
    }

    void encrypt_btn_Click(object sender, EventArgs e)
    {
        var command = new Command { Action = CommandAction.encrypt };
        command.AddPaths(selectedPaths);

        hideResultLabels();

        var passwordForm = new PasswordForm(command.Paths);
        if (passwordForm.ShowDialog() == DialogResult.OK)
        {
            command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(passwordForm.password));

            try
            {
                processing_dots.Visible = true;
                Commands.Encrypt(command);
                setResultLabels(true, "encrypted", command.Paths.Count > 1 ? "paths" : "path");
            }
            catch (Exception)
            {
                setResultLabels(false, "encrypt", command.Paths.Count > 1 ? "paths" : "path");
            }
            finally
            {
                processing_dots.Visible = false;
            }
        }

        resetSelected();
        setActionButtonsStatus(false);
    }

    void decrypt_btn_Click(object sender, EventArgs e)
    {
        var command = new Command { Action = CommandAction.decrypt };
        command.AddPath(selectedPaths[0]);

        hideResultLabels();

        var passwordForm = new PasswordForm(command.Paths, command.Action);
        if (passwordForm.ShowDialog() == DialogResult.OK)
        {
            command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(passwordForm.password));

            try
            {
                processing_dots.Visible = true;
                Commands.Decrypt(command);
                setResultLabels(true, "decrypted", "file");
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

        resetSelected();
        setActionButtonsStatus(false);
    }

    void delete_btn_Click(object sender, EventArgs e)
    {
        try
        {
            var pathExists = true;
            processing_dots.Visible = true;

            if (File.Exists(selectedPaths[0])) FileUtils.OverwriteAndDeleteFile(new FileInfo(selectedPaths[0]));
            else if (Directory.Exists(selectedPaths[0])) FileUtils.OverwriteAndDeleteDirectory(new DirectoryInfo(selectedPaths[0]));
            else
            {
                pathExists = false;
                setResultLabels(false, "delete", ", path is not pathExists");
            }

            if (pathExists) setResultLabels(true, "deleted", "path");
        }
        catch (Exception)
        {
            setResultLabels(false, "delete", "");
        }
        finally
        {
            processing_dots.Visible = false;
        }
        
        resetSelected();
        setActionButtonsStatus(false);
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

    void setActionButtonsStatus(bool enabled)
    {
        encrypt_btn.Enabled = enabled;
        decrypt_btn.Enabled = enabled;
        delete_btn.Enabled = enabled;
    }

    void resetSelected()
    {
        selectedPaths.Clear();
        selected_label.Visible = false;
        selected_path_label.Visible = false;
        selected_pathlist.Visible = false;

        selected_pathlist.Items.Clear();
        selected_path_label.Text = null;
    }
}
