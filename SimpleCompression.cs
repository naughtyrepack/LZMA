using System;
using SevenZip.Compression.LZMA;
using System.IO;

namespace LZMA
{
    public static class SimpleCompression
    {
        public static void CompressFile(string inputFile, string outputFile)
        {
            Encoder encoder = new Encoder();
            MemoryStream properties = new MemoryStream();
            encoder.WriteCoderProperties(properties);

            using (FileStream input = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                output.Write(properties.ToArray(), 0, 5);
                long inputFileSize = input.Length;
                byte[] fileLengthBytes = BitConverter.GetBytes(inputFileSize);
                output.Write(fileLengthBytes, 0, 8);
                encoder.Code(input, output, inputFileSize, -1, null);
            }
        }

        public static void DecompressFile(string compressedFile, string outputFile)
        {
            using (FileStream input = new FileStream(compressedFile, FileMode.Open, FileAccess.Read))
            using (FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                byte[] properties = new byte[5];
                input.Read(properties, 0, 5);
                byte[] fileLengthBytes = new byte[8];
                input.Read(fileLengthBytes, 0, 8);
                long uncompressedSize = BitConverter.ToInt64(fileLengthBytes, 0);
                Decoder decoder = new Decoder();
                decoder.SetDecoderProperties(properties);
                decoder.Code(input, output, input.Length - input.Position, uncompressedSize, null);
            }
        }
    }
}