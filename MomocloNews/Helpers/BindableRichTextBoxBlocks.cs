using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Helpers
{
    public class BindableRichTextBoxBlocks : FrameworkElement
    {
        public static string GetBlocks(RichTextBox element)
        {
            if (element != null)
                return element.GetValue(BlocksProperty) as string;
            return string.Empty;
        }

        public static void SetBlocks(RichTextBox element, string value)
        {
            if (element != null)
                element.SetValue(BlocksProperty, value);
        }

        public static readonly DependencyProperty BlocksProperty =
            DependencyProperty.RegisterAttached(
                "Blocks",
                typeof(List<Block>),
                typeof(RichTextBox),
                new PropertyMetadata(null, OnBlocksPropertyChanged));

        private static void OnBlocksPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var rtb = obj as RichTextBox;
            if (rtb != null)
            {
                rtb.Blocks.Clear();

                var blocks = e.NewValue as List<Block>;
                if (blocks != null)
                {
                    blocks.ForEach(block => rtb.Blocks.Add((block)));
                }
            }
        }
    }
}
