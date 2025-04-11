using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Hash.Behaviors;

public class DropFileBehavior : Behavior<FrameworkElement>
{
    public string[]? Data
    {
        get => (string[]?)GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }

    public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(string[]),
        typeof(DropFileBehavior),
        new UIPropertyMetadata(null)
    );

    protected override void OnAttached()
    {
        AssociatedObject.AllowDrop = true;
        AssociatedObject.Drop += DropHandler;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Drop -= DropHandler;
    }

    private void DropHandler(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            Data = (string[])e.Data.GetData(DataFormats.FileDrop);
        }
    }
}
