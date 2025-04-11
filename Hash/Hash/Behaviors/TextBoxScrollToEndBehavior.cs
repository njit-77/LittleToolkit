using Microsoft.Xaml.Behaviors;

namespace Hash.Behaviors;

public class TextBoxScrollToEndBehavior : Behavior<System.Windows.Controls.TextBox>
{
    protected override void OnAttached()
    {
        AssociatedObject.TextChanged += TextBox_TextChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.TextChanged -= TextBox_TextChanged;
    }

    private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        (sender as System.Windows.Controls.TextBox)?.ScrollToEnd();
    }
}
