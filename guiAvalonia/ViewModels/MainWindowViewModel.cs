using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;
using guiAvalonia.Models;

namespace guiAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    readonly Lazy<ConfigService> _configService = App.ConfigService;
    IReadOnlyDictionary<string, GroupItem> Groups { get; }
    
    public ObservableCollection<GroupsListItem> GroupsListItems { get; set; } = [];
    

    public MainWindowViewModel()
    {
        Groups = _configService.Value.Groups;

        SetGroupsToGroupsListItems();
    }

    void SetGroupsToGroupsListItems()
    {
        List<GroupsListItem> tempGroupsItems = [];
        
        foreach (var (name, value) in Groups)
        {
            var actions =  StringUtils.FirstLetterToUpper(value.Action.ToString());
            
            if (value.Overwrite) actions += ", Replace";
            if (value.Delete) actions += ", Delete";
            
            tempGroupsItems.Add(new GroupsListItem(name, actions));
        }

        tempGroupsItems = tempGroupsItems.OrderBy(item => item.Name).ToList();

        GroupsListItems.Clear();

        foreach (var groupItem in tempGroupsItems)
            GroupsListItems.Add(groupItem);
    }

    public void UpdateGroups()
    {
        SetGroupsToGroupsListItems();
    }
}