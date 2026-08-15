using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Uni3DGS.Starter
{
    public static class InvalidFixtureFactory
    {
        static string DirectoryPath
            => Path.Combine(Application.temporaryCachePath, "Uni3DGS-Starter-Invalid");

        public static string EmptyPly()
            => Write("Empty.ply", Array.Empty<byte>());

        public static string MalformedPly()
            => Write("Malformed.ply", Encoding.ASCII.GetBytes(
                "ply\nformat binary_little_endian 1.0\nelement vertex 1\nproperty float x\nend_header\n"));

        public static string UnsupportedExtension()
            => Write("Unsupported.xyz", Encoding.ASCII.GetBytes("intentionally unsupported"));

        public static string SpzV4()
        {
            var bytes = new byte[32];
            Buffer.BlockCopy(BitConverter.GetBytes(0x5053474Eu), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(4u), 0, bytes, 4, 4);
            bytes[13] = 12;
            bytes[15] = 6;
            return Write("v4-Unsupported.spz", bytes);
        }

        static string Write(string fileName, byte[] data)
        {
            Directory.CreateDirectory(DirectoryPath);
            string path = Path.Combine(DirectoryPath, fileName);
            File.WriteAllBytes(path, data);
            return path;
        }
    }
}
