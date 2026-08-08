using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace TelegramBotMinecraft.Core.Services
{
    public class LzmaDecompressorService
    {
        public async Task UnzipFile(string pathToFile, string targetFilePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(pathToFile))
            {
                return;
            }

            try
            {
                using (FileStream inStream = new FileStream(pathToFile, FileMode.Open))
                using (FileStream outStream = new FileStream(targetFilePath, FileMode.Create))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var decoder = new SevenZip.Compression.LZMA.Decoder();

                    byte[] properties = new byte[5];
                    inStream.ReadExactly(properties, 0, 5);
                    decoder.SetDecoderProperties(properties);

                    byte[] sizeBuffer = new byte[8];
                    inStream.Read(sizeBuffer, 0, 8);

                    long outSize = BinaryPrimitives.ReadInt64LittleEndian(sizeBuffer);

                    long inSize = inStream.Length - inStream.Position;
                    decoder.Code(inStream, outStream, inSize, outSize, null);
                }

                File.Delete(pathToFile);
            }
            catch { }
        }
    }
}
