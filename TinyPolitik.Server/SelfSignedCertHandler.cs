using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TinyPolitik.Core;

public static class SelfSignedCertHelper
{
    public static X509Certificate2 GetOrCreate(string path, string password)
    {
        if (File.Exists(path))
        {
            return X509CertificateLoader.LoadPkcs12FromFile(path, password, X509KeyStorageFlags.Exportable);
        }

        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=TinyPolitikServer", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false)
        );

        // Certificate lasts for freaking 10 years because we don't need it to expire.
        var cert = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(10));
        var pfxBytes = cert.Export(X509ContentType.Pfx, password);
        
        // Save to file
        File.WriteAllBytes(path, pfxBytes);

        // From from the byte array and return
        return X509CertificateLoader.LoadPkcs12(pfxBytes, password, X509KeyStorageFlags.Exportable);
    }
}