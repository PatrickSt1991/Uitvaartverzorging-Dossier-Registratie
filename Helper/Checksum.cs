using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Dossier_Registratie.Helper
{
    public class Checksum
    {
        public static string GetMD5Checksum(string filename)
        {
            using (var md5 = MD5.Create())
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(filename).ToLower() == ".docx")
                {
                    using (var archive = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (entry.FullName == "word/document.xml")
                            {
                                using (var entryStream = entry.Open())
                                {
                                    return ComputeChecksum(entryStream, md5);
                                }
                            }
                        }
                    }

                    throw new InvalidOperationException("The .docx file does not contain a 'word/document.xml' entry.");
                }

                return ComputeChecksum(fs, md5);
            }
        }

        private static string ComputeChecksum(Stream stream, MD5 md5)
        {
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
