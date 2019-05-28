using System;
using System.Windows;

namespace PerfectCode.TVShowManager.UserInterface
{
    public static class DialogClosedBehavior
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogClosedBehavior),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }
    }
}
