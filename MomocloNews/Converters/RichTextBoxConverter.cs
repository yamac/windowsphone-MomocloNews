using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;

namespace MomocloNews.Converters
{
    public class StringToRichTextBoxBlocksConverter : IValueConverter
    {
        private Regex regex =
            new Regex(
                @"(((http|https|ftp|news|file)+\:\/\/)[_.a-z0-9-]+\.[a-z0-9\/_:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])",
                RegexOptions.IgnoreCase
            );

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var blocks = new List<Block>();
            if (value != null)
            {
                var paragraph = new Paragraph();
                var originalString = value as string;
                int index = 0;
                var match = regex.Match(originalString);
                while (match.Success)
                {
                    paragraph.Inlines.Add(new Run() { Text = originalString.Substring(index, match.Index - index) });
                    var hyperlink = new Hyperlink();
                    hyperlink.Inlines.Add(new Run() { Text = match.Value });
                    hyperlink.NavigateUri = new Uri(match.Value, UriKind.Absolute);
                    hyperlink.TargetName = "_blank";
                    paragraph.Inlines.Add(hyperlink);
                    index += match.Index + match.Length;
                    match = match.NextMatch();
                }
                if (index < originalString.Length)
                {
                    paragraph.Inlines.Add(new Run() { Text = originalString.Substring(index) });
                }
                blocks.Add(paragraph);
            }
            return blocks;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
