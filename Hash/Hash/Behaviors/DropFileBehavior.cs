using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Hash.Behaviors;

public class DropFileBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(
        nameof(Files),
        typeof(string[]),
        typeof(DropFileBehavior),
        new UIPropertyMetadata(null)
    );

    public string[]? Files
    {
        get => (string[]?)GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.AllowDrop = true;
        AssociatedObject.PreviewDragOver += PreviewDragOverHandler;
        AssociatedObject.PreviewDrop += PreviewDropHandler;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.PreviewDragOver -= PreviewDragOverHandler;
        AssociatedObject.PreviewDrop -= PreviewDropHandler;
    }

    private void PreviewDragOverHandler(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private void PreviewDropHandler(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            Files = (e.Data.GetData(DataFormats.FileDrop) as string[]);
        }
    }
}
