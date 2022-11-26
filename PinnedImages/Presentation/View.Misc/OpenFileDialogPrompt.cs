using Microsoft.Win32;
using System.IO;

namespace Presentation.View.Misc
{
    public record OpenFileDialogPromptParameter(string Title, string Filter);

    public class OpenFileDialogPrompt : IUserPrompt<Stream?, OpenFileDialogPromptParameter>
    {
        public Stream? Prompt(OpenFileDialogPromptParameter parameter)
        {
            var dialog = new OpenFileDialog()
            {
                Title = parameter.Title,
                Filter = parameter.Filter
            };

            bool? result = dialog.ShowDialog();

            return result.HasValue && result.Value ? dialog.OpenFile() : null;
        }
    }
}
