using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EncryptionTool.cmd;
using EncryptionTool.models;
using EncryptionTool.services;
using guiAvalonia.Models;
using guiAvalonia.ViewModels;

namespace guiAvalonia.Views;

public partial class MainWindow : Window
{
    Password? _passwordWindow;
    
    readonly TextBlock _messageTextBlock;
    readonly TextBlock _selectedPathInfo;
    readonly TextBlock _selectedFileInfo;
    readonly TextBlock _selectedPathTextBlock;
    readonly TextBlock _selectedFilesTextBlock;
    readonly Button _encryptButton;
    readonly Button _decryptButton;
    readonly Button _deleteButton;
    readonly Button _executeGroupButton;
    readonly Button _editGroupButton;
    readonly Button _deleteGroupButton;
    readonly ListBox _groupsListBox;
    
    readonly Lazy<ConfigService> _configService = App.ConfigService;
    readonly List<string> _selectedPaths = [];
    
    public MainWindow()
    {
        InitializeComponent();
                
        _encryptButton = this.FindControl<Button>("EncryptBtn") 
                         ?? throw new InvalidOperationException("EncryptBtn not found in layout");
        
        _decryptButton = this.FindControl<Button>("DecryptBtn") 
                         ?? throw new InvalidOperationException("DecryptBtn not found in layout");
        
        _deleteButton = this.FindControl<Button>("DeleteBtn") 
                        ?? throw new InvalidOperationException("DeleteBtn not found in layout");
        
        _selectedPathTextBlock = this.FindControl<TextBlock>("SelectedPathTextBlock") 
                                 ?? throw new InvalidOperationException("SelectedPathTextBlock not found in layout");
        
        _selectedFilesTextBlock = this.FindControl<TextBlock>("SelectedFilesTextBlock") 
                                 ?? throw new InvalidOperationException("SelectedFilesTextBlock not found in layout");
        
        _selectedPathInfo = this.FindControl<TextBlock>("SelectedPathInfo") 
                                 ?? throw new InvalidOperationException("SelectedPathInfo not found in layout");
        
        _selectedFileInfo = this.FindControl<TextBlock>("SelectedFileInfo") 
                                  ?? throw new InvalidOperationException("SelectedFileInfo not found in layout");
        
        _groupsListBox = this.FindControl<ListBox>("GroupsListBox") 
                         ?? throw new InvalidOperationException("GroupsListBox not found in layout");
        
        _messageTextBlock = this.FindControl<TextBlock>("MessageTextBlock") 
                            ?? throw new InvalidOperationException("MessageTextBlock not found in layout");
        
        _executeGroupButton = this.FindControl<Button>("ExecuteGroupBtn") 
                        ?? throw new InvalidOperationException("ExecuteGroupBtn not found in layout");
                
        _editGroupButton = this.FindControl<Button>("EditGroupBtn") 
                              ?? throw new InvalidOperationException("EditGroupBtn not found in layout");
                
        _deleteGroupButton = this.FindControl<Button>("DeleteGroupBtn") 
                              ?? throw new InvalidOperationException("DeleteGroupBtn not found in layout");
    }
    
    async void PickFileBtn_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Pick File",
            AllowMultiple = true
        });
        
        if (files.Count == 0) return;

        _selectedPaths.Clear();
        _selectedFilesTextBlock.Text = "";
        _selectedPathTextBlock.Text = Path.GetDirectoryName(Uri.UnescapeDataString(files[0].Path.AbsolutePath));
        _selectedPathInfo.Opacity = 1;
        _selectedFileInfo.Opacity = 1;
        _messageTextBlock.Text = "";

        for (var i = 0; i < files.Count; i++)
        {
            var storageFile = files[i];
            
            _selectedFilesTextBlock.Text += i != files.Count - 1 
                ? $"{storageFile.Name}, " 
                : storageFile.Name;
            
            _selectedPaths.Add(Uri.UnescapeDataString(storageFile.Path.AbsolutePath));
        }
        
        SetActionButtonsState(true);
    }
    
    async void PickFolderBtn_Click(object? sender, RoutedEventArgs e)
    {
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Pick Folder",
            AllowMultiple = false
        });
        
        if (folder.Count == 0) return;
        
        var unescapedPath = Uri.UnescapeDataString(folder[0].Path.AbsolutePath);

        _selectedPathInfo.Opacity = 1;
        _selectedFileInfo.Opacity = 0;
        _selectedPathTextBlock.Text = Path.GetDirectoryName(unescapedPath);
        _selectedFilesTextBlock.Text = "";
        _messageTextBlock.Text = "";
        
        _selectedPaths.Clear();
        _selectedPaths.Add(unescapedPath);

        SetActionButtonsState(true);
    }

    public async void EncryptBtn_Click(object sender, RoutedEventArgs e)
    {
        _messageTextBlock.Text = "";
        
        _passwordWindow = new Password(CommandAction.encrypt, new WindowInfo(Position, Width, Height));
        var command = await _passwordWindow.ShowDialog<Command?>(this);

        if (command == null) return;
        
        try
        {
            command.AddPaths(_selectedPaths);
            Commands.Encrypt(command);
            
            _messageTextBlock.Text = _selectedPaths.Count > 1 
                ? "Files encrypted"
                : "File encrypted";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _messageTextBlock.Text = "Failed to encrypt";
        }
    }
    
    public async void DecryptBtn_Click(object sender, RoutedEventArgs e)
    {
        _messageTextBlock.Text = "";
        
        _passwordWindow = new Password(CommandAction.decrypt, new WindowInfo(Position, Width, Height));
        var command = await _passwordWindow.ShowDialog<Command?>(this);

        if (command == null) return;

        try
        {
            command.AddPaths(_selectedPaths);
            Commands.Decrypt(command);
            
            _messageTextBlock.Text = "File decrypted";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _messageTextBlock.Text = "Failed to decrypt";
        }
    }
    
    public void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var command = new Command { Action = CommandAction.delete };
            command.AddPaths(_selectedPaths);
            Commands.Delete(command);
            
            _messageTextBlock.Text = _selectedPaths.Count > 1 
                ? "Files deleted"
                : "File deleted";
            
            _selectedPathInfo.Opacity = 0;
            _selectedFileInfo.Opacity = 0;
            _selectedPathTextBlock.Text = "";
            _selectedFilesTextBlock.Text = "";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _messageTextBlock.Text = "Failed to delete";
        }
    }

    async void GroupItem_ExecuteClick(object? sender, RoutedEventArgs e)
    {
        if (_groupsListBox.Selection.SelectedItem is not GroupsListItem selection) return;
        if (!_configService.Value.Groups.TryGetValue(selection.Name, out var group)) return;
        
        _passwordWindow = new Password(group.Action, new WindowInfo(Position, Width, Height), group);
        var command = await _passwordWindow.ShowDialog<Command?>(this);
        
        if (command == null) return;

        try
        {
            command.AddPaths(group.Paths);

            if (CommandAction.encrypt == command.Action) 
                Commands.Encrypt(command);
            
            if (CommandAction.decrypt == command.Action) 
                Commands.Decrypt(command);
            
            _messageTextBlock.Text = $"{selection.Name} {command.Action.ToString().ToLower()}ed";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _messageTextBlock.Text = $"Failed to {command.Action.ToString().ToLower()} {selection.Name}";
        }
    }

    async void GroupItemAdd_Click(object? sender, RoutedEventArgs e)
    {
        _messageTextBlock.Text = "";
        
        var editWindow = new GroupEdit();
        var group = await editWindow.ShowDialog<Dictionary<string, GroupItem>?>(this);

        if (group == null) return;
        
        if (DataContext is MainWindowViewModel vm)
        {
            _configService.Value.AddGroup(group);
            vm.UpdateGroups();
        }
    }

    async void GroupItem_EditClick(object? sender, RoutedEventArgs e)
    {
        if (_groupsListBox.Selection.SelectedItem is not GroupsListItem selection) return;
        if (!_configService.Value.Groups.TryGetValue(selection.Name, out var group)) return;
        
        _messageTextBlock.Text = "";
        
        var editWindow = new GroupEdit(group, selection.Name);
        var groupNew = await editWindow.ShowDialog<Dictionary<string, GroupItem>?>(this);

        if (groupNew == null || DataContext is not MainWindowViewModel vm) return;
        
        _configService.Value.DeleteGroup(selection.Name);
        _configService.Value.AddGroup(groupNew);
        vm.UpdateGroups();
    }

    void GroupItem_DeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_groupsListBox.Selection.SelectedItem is GroupsListItem selection) 
            _configService.Value.DeleteGroup(selection.Name);
        
        if (DataContext is MainWindowViewModel vm) vm.UpdateGroups();
    }

    void GroupsList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;

        _messageTextBlock.Text = "";
        SetGroupButtonsState(listBox.SelectedIndex != -1);
    }

    void SetGroupButtonsState(bool isEnabled)
    {
        _executeGroupButton.IsEnabled = isEnabled;
        _editGroupButton.IsEnabled = isEnabled;
        _deleteGroupButton.IsEnabled = isEnabled;
    }
    
    void SetActionButtonsState(bool isEnabled)
    {
        _encryptButton.IsEnabled = isEnabled;
        _decryptButton.IsEnabled = isEnabled;
        _deleteButton.IsEnabled = isEnabled;
    }
}

public readonly struct WindowInfo(PixelPoint position, double width, double height)
{
    public PixelPoint Position { get; } = position;
    public double Width { get; } = width;
    public double Height { get; } = height;
}