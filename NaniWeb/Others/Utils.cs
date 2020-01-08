using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NaniWeb.Others
{
    public static class Utils
    {
        public static DirectoryInfo CurrentDirectory
        {
            get
            {
                var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);

                return new FileInfo(location.AbsolutePath.Replace("%20", " ")).Directory;
            }
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
            if (!File.Exists($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}{certName}"))
            {
                var certRequest = new CertificateRequest("cn=NaniWeb", RSA.Create(2048), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);

                File.WriteAllBytes($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}{certName}", certificate.Export(X509ContentType.Pfx));

                return certificate;
            }

            return new X509Certificate2($"{CurrentDirectory.FullName}{Path.DirectorySeparatorChar}{certName}");
        }
    }
}