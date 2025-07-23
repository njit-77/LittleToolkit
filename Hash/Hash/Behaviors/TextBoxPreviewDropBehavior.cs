using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Hash.Behaviors;

public class TextBoxPreviewDropBehavior : Behavior<System.Windows.Controls.TextBox>
{
    public string[]? Data
    {
        get => (string[]?)GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }

    public static readonly DependencyProperty FilesProperty = DependencyProperty.Register(
        nameof(Data),
        typeof(string[]),
        typeof(TextBoxPreviewDropBehavior),
        new UIPropertyMetadata(null)
    );

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.AllowDrop = true;
        AssociatedObject.PreviewDragOver += PreviewDragOverHandler;
        AssociatedObject.PreviewDrop += PreviewDropHandler;
        AssociatedObject.PreviewDragEnter += PreviewDragEnterHandler;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.AllowDrop = false;
        AssociatedObject.PreviewDragOver -= PreviewDragOverHandler;
        AssociatedObject.PreviewDrop -= PreviewDropHandler;
        AssociatedObject.PreviewDragEnter -= PreviewDragEnterHandler;
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
            Data = (string[])e.Data.GetData(DataFormats.FileDrop);
        }
    }

    private void PreviewDragEnterHandler(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
    }
}
