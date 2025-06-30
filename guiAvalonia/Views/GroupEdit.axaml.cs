using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EncryptionTool.models;
using guiAvalonia.ViewModels;

namespace guiAvalonia.Views;

public partial class GroupEdit : Window
{
    readonly GroupItem _groupItem;

    int _selectedIndex;
        
    public GroupEdit()
    {
        InitializeComponent();
        DataContext = new GroupEditViewModel();
        _groupItem = new GroupItem();
    }
    
    public GroupEdit(GroupItem groupItem, string name) : this()
    {
        InitializeComponent();
        _groupItem = groupItem;

        if (DataContext is not GroupEditViewModel vm) return;

        vm.Text = name;
        vm.IsDeleteButtonChecked = groupItem.Delete;
        vm.IsReplaceButtonChecked = groupItem.Overwrite;
        vm.IsEncryptSelected = groupItem.Action == CommandAction.encrypt;
        vm.IsDecryptSelected = groupItem.Action == CommandAction.decrypt;
        
        foreach (var path in groupItem.Paths)
            vm.Paths.Add(path);
    }
    
    async void AddFileBtn_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Pick File",
            AllowMultiple = true,
        });
        
        if (files.Count == 0 || DataContext is not GroupEditViewModel vm) return;

        foreach (var file in files)
        {
            vm.Paths.Add(Uri.UnescapeDataString(file.Path.AbsolutePath));
        }
    }

    async void AddFolderBtn_Click(object? sender, RoutedEventArgs e)
    {
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Pick Folder",
            AllowMultiple = false
        });
        
        if (folder.Count == 0 || DataContext is not GroupEditViewModel vm) return;
        
        vm.Paths.Add(Uri.UnescapeDataString(folder[0].Path.AbsolutePath));
    }
    
    void RemoveBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not GroupEditViewModel vm) return;
        
        vm.Paths.RemoveAt(_selectedIndex);
    }

    void SaveBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not GroupEditViewModel vm) return;
        
        var action = vm.IsEncryptSelected ? CommandAction.encrypt : CommandAction.decrypt;
        var deleteOriginal = vm.IsDeleteButtonChecked;
        var replaceOriginal = vm.IsReplaceButtonChecked;
        var paths = vm.Paths;
        var name = vm.Text;

        if (string.IsNullOrEmpty(name)) return;
        
        _groupItem.ClearPaths();
        _groupItem.AddPaths(paths);
        _groupItem.Delete = deleteOriginal;
        _groupItem.Overwrite = replaceOriginal;
        _groupItem.Action = action;

        var output = new Dictionary<string, GroupItem> { { name, _groupItem } };

        Close(output);
    }

    void CancelBtn_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    void PathsList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox || DataContext is not GroupEditViewModel vm) return;
        
        var selectionIndex = listBox.SelectedIndex;
        Console.WriteLine(selectionIndex);

        if (selectionIndex == -1)
        {
            vm.IsRemoveButtonEnabled = false;
            return;
        }

        vm.IsRemoveButtonEnabled = true;
        _selectedIndex= selectionIndex;
    }
}
