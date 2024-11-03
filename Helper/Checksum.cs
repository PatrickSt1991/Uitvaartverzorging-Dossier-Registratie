using System;
using System.IO;
using System.Security.Cryptography;

namespace Dossier_Registratie.Helper
{
    public class Checksum
    {
        public static string GetMD5Checksum(string filename)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            }

        }
    }
}
