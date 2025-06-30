using System.Collections.ObjectModel;

namespace guiAvalonia.ViewModels;

public class GroupEditViewModel : ViewModelBase
{
    public ObservableCollection<string> Paths { get; set; } = [];
    
    string _text;
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }
    
    bool _isDeleteButtonChecked;

    public bool IsDeleteButtonChecked
    {
        get => _isDeleteButtonChecked;
        set => SetProperty(ref _isDeleteButtonChecked, value);
    }
    
    bool _isReplaceButtonChecked;

    public bool IsReplaceButtonChecked
    {
        get => _isReplaceButtonChecked;
        set => SetProperty(ref _isReplaceButtonChecked, value);
    }

    bool _isRemoveButtonEnabled;

    public bool IsRemoveButtonEnabled
    {
        get => _isRemoveButtonEnabled;
        set => SetProperty(ref _isRemoveButtonEnabled, value);
    }

    bool _isEncryptSelected = true;

    public bool IsEncryptSelected
    {
        get => _isEncryptSelected;
        set => SetProperty(ref _isEncryptSelected, value);
    }
    
    bool _isDecryptSelected;

    public bool IsDecryptSelected
    {
        get => _isDecryptSelected;
        set => SetProperty(ref _isDecryptSelected, value);
    }
}