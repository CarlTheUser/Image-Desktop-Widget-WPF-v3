using Data.Common.Contracts;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Presentation
{
    public class ColorsFromJsonFileQuery : IAsyncQuery<IEnumerable<Color>, FileInfo>
    {
        public async Task<IEnumerable<Color>> ExecuteAsync(FileInfo parameter, CancellationToken cancellationToken = default)
        {
            using StreamReader reader = parameter.OpenText();

            string fileContents = await reader.ReadToEndAsync();

            JArray array = JArray.Parse(fileContents);

            return from item in array
                   select (Color)ColorConverter.ConvertFromString(item.Value<string>());
        }
    }

    public class StubColorsQuery : IAsyncQuery<IEnumerable<Color>, FileInfo>
    {
        public Task<IEnumerable<Color>> ExecuteAsync(FileInfo parameter, CancellationToken token)
        {
            var colors = new Color[]
            {
                (Color)ColorConverter.ConvertFromString("#42bfff"),
                (Color)ColorConverter.ConvertFromString("#8242ff"),
                (Color)ColorConverter.ConvertFromString("#4e4e4e"),
                (Color)ColorConverter.ConvertFromString("#e9e3c8"),
                (Color)ColorConverter.ConvertFromString("#0e58c6"),
                (Color)ColorConverter.ConvertFromString("#ffc0cb"),
                (Color)ColorConverter.ConvertFromString("#fe28a2"),
                (Color)ColorConverter.ConvertFromString("#b7ef56"),
                (Color)ColorConverter.ConvertFromString("#00fa9a"),
                (Color)ColorConverter.ConvertFromString("#00ab66"),
                (Color)ColorConverter.ConvertFromString("#fe4164"),
                (Color)ColorConverter.ConvertFromString("#ff5a36"),
            };

            return Task.FromResult<IEnumerable<Color>>(colors);
        }
    }
}
