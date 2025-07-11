using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Dossier_Registratie.Helper
{
    public class Checksum
    {
        public static string GetSha256Checksum(string filename)
        {
            using var sha256 = SHA256.Create();
            using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            if (Path.GetExtension(filename).ToLower() == ".docx")
            {
                using var archive = new ZipArchive(fs, ZipArchiveMode.Read);
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName == "word/document.xml")
                    {
                        using var entryStream = entry.Open();
                        return ComputeChecksum(entryStream, sha256);
                    }
                }

                throw new InvalidOperationException("The .docx file does not contain a 'word/document.xml' entry.");
            }

            return ComputeChecksum(fs, sha256);
        }

        private static string ComputeChecksum(Stream stream, HashAlgorithm algorithm)
        {
            var hash = algorithm.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

    }
}
