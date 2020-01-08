using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NaniWeb.Others
{
    public static class Utils
    {
        static Utils()
        {
            InstallationId = IsInstalled() ? File.ReadAllText($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}installed.txt") : new Guid().ToString();
        }

        public static string InstallationId { get; }

        public static DirectoryInfo CurrentDirectory
        {
            get
            {
                var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);

                return new FileInfo(location.AbsolutePath.Replace("%20", " ")).Directory;
            }
        }

        public static bool IsInstalled()
        {
            return File.Exists($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}installed.txt");
        }

        public static void ResizeImage(FileInfo origin, DirectoryInfo destination, string fileName, ushort width, ushort height)
        {
            using (var image = Image.Load(origin.OpenRead()))
            {
                image.Mutate(param => param.Resize(width, height));
                image.Save($"{destination.FullName}{Path.DirectorySeparatorChar}{fileName}{origin.Extension}");
            }
        }

        public static string GenerateSlug(string source)
        {
            var output = Regex.Replace(source, @"[^A-Za-z0-9\s]", string.Empty);
            return Regex.Replace(output, @"[\s]", "-").ToLower();
        }

        public static X509Certificate2 GetCertificate(string certName)
        {
            var certKey = InstallationId;

            if (File.Exists($"{Path.DirectorySeparatorChar}etc{Path.DirectorySeparatorChar}machine-id"))
                certKey = File.ReadAllText($"{Path.DirectorySeparatorChar}etc{Path.DirectorySeparatorChar}machine-id");

            if (!File.Exists($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}{certName}"))
            {
                var keyPair = RSA.Create(1024);
                var certRequest = new CertificateRequest("cn=NaniWeb", keyPair, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);

                File.WriteAllBytes($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}{certName}", certificate.Export(X509ContentType.Pfx, certKey));

                return certificate;
            }

            return new X509Certificate2($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}{certName}", certKey);
        }
    }
}