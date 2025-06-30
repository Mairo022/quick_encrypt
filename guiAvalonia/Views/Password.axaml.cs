using System;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using EncryptionTool.models;
using EncryptionTool.services;

namespace guiAvalonia.Views;

public partial class Password : Window
{
    readonly TextBox _passwordInput;
    readonly ToggleButton _deleteToggleButton;
    readonly ToggleButton _replaceToggleButton;

    readonly Command _command = new();
    
    public Password(CommandAction commandAction, WindowInfo parentWindow)
    {
        InitializeComponent();
        
        _passwordInput = this.FindControl<TextBox>("PasswordTextBox") 
                 ?? throw new InvalidOperationException("PasswordTextBox not found in layout");
        
        _deleteToggleButton = this.FindControl<ToggleButton>("DeleteToggleBtn") 
                              ?? throw new InvalidOperationException("DeleteToggleBtn not found in layout");
        
        _replaceToggleButton = this.FindControl<ToggleButton>("ReplaceToggleBtn") 
                               ?? throw new InvalidOperationException("ReplaceToggleBtn not found in layout");
        
        var actionButton = this.FindControl<Button>("ActionBtn") 
                               ?? throw new InvalidOperationException("ActionBtn not found in layout");

        actionButton.Content = commandAction switch
        {
            CommandAction.encrypt => "Encrypt",
            CommandAction.decrypt => "Decrypt",
            _ => throw new ArgumentException("Invalid command action")
        };

        _command.Action = commandAction;
        
        var centerX = (int) ((parentWindow.Position.X + parentWindow.Width / 2) - Width / 2);
        var centerY = (int) ((parentWindow.Position.Y + parentWindow.Height / 2)- Height / 2);
        Position = new PixelPoint(centerX, centerY);
    }

    public Password(CommandAction commandAction, WindowInfo parentWindow, GroupItem groupItem) 
        : this(commandAction, parentWindow)
    {
        _deleteToggleButton.IsChecked = groupItem.Delete;
        _replaceToggleButton.IsChecked = groupItem.Overwrite;
    }

    void ActionBtn_Click(object? sender, RoutedEventArgs e)
    {
        _command.Delete = _deleteToggleButton.IsChecked!.Value;
        _command.Overwrite = _replaceToggleButton.IsChecked!.Value;
        _command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(
            Encoding.Unicode.GetBytes(_passwordInput.Text ?? ""));
        
        Close(_command);
    }

    void CancelBtn_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}