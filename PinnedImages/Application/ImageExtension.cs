using System.Text;
using System.Text.RegularExpressions;

namespace Application
{
    //https://stackoverflow.com/questions/670546/determine-if-file-is-an-image
    //answered Aug 26, 2014 at 15:19
    //Aydin's user avatar
    //Aydin
    //14.6k44 gold badges3030 silver badges42

    //With minor changes
    public static class ImageExtension
    {
        static ImageExtension()
        {
            _imageTypes = new Dictionary<string, string>
            {
                { "FFD8", "jpg" },
                { "424D", "bmp" },
                { "474946", "gif" },
                { "89504E470D0A1A0A", "png" }
            };
        }

        /// <summary>
        ///     <para> Registers a hexadecimal value used for a given image type </para>
        ///     <param name="imageType"> The type of image, example: "png" </param>
        ///     <param name="uniqueHeaderAsHex"> The type of image, example: "89504E470D0A1A0A" </param>
        /// </summary>
        public static void RegisterImageHeaderSignature(string imageType, string uniqueHeaderAsHex)
        {
            var validator = new Regex(@"^[A-F0-9]+$", RegexOptions.CultureInvariant);

            uniqueHeaderAsHex = uniqueHeaderAsHex.Replace(" ", "");

            if (string.IsNullOrWhiteSpace(imageType)) throw new ArgumentNullException(nameof(imageType));
            if (string.IsNullOrWhiteSpace(uniqueHeaderAsHex)) throw new ArgumentNullException(nameof(uniqueHeaderAsHex));
            if (uniqueHeaderAsHex.Length % 2 != 0) throw new ArgumentException("Hexadecimal value is invalid");
            if (!validator.IsMatch(uniqueHeaderAsHex)) throw new ArgumentException("Hexadecimal value is invalid");

            _imageTypes.Add(uniqueHeaderAsHex, imageType);
        }

        private static readonly Dictionary<string, string> _imageTypes;

        public static bool IsImage(this Stream stream)
        {
            return stream.IsImage(out _);
        }

        public static bool IsImage(this Stream stream, out string? imageType)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var bobTheBuilder = new StringBuilder();
            int largestByteHeader = _imageTypes.Max(img => img.Key.Length);

            for (int i = 0; i < largestByteHeader; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bobTheBuilder.Append(bit);

                string builtHex = bobTheBuilder.ToString();
                bool isImage = _imageTypes.Keys.Any(img => img == builtHex);
                if (isImage)
                {
                    imageType = _imageTypes[bobTheBuilder.ToString()];
                    return true;
                }
            }
            imageType = null;
            return false;
        }
    }
}
