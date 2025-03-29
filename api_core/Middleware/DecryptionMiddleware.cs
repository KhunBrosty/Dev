using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class EncryptionMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string Key = "AyoZNg948mTYJ6uJSlzxcbonCboWYhKL";
    private static readonly string IV = "Z163xZu5gheONuff";

    public EncryptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // 🟢 Decrypt Incoming Request
        if (context.Request.Method == HttpMethods.Post && context.Request.ContentType == "application/json; charset=utf-8")
        {
            using var reader = new StreamReader(context.Request.Body);
            string encryptedRequestBody = await reader.ReadToEndAsync();

            var jsonDoc = JsonDocument.Parse(encryptedRequestBody);
            jsonDoc.RootElement.TryGetProperty("data", out JsonElement encryptedDataElement);
            //if (!jsonDoc.RootElement.TryGetProperty("data", out JsonElement encryptedDataElement))
            //{
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    await context.Response.WriteAsync("Missing 'data' field in request");
            //    return;
            //}

            string encryptedData = encryptedDataElement.GetString()!;
            string decryptedJson = DecryptAES(encryptedData);

            var byteArray = Encoding.UTF8.GetBytes(decryptedJson);
            context.Request.Body = new MemoryStream(byteArray);
            context.Request.ContentLength = byteArray.Length;
        }

        // 🟢 Call Next Middleware (Controller)
        var originalBodyStream = context.Response.Body;
        using var newBodyStream = new MemoryStream();
        context.Response.Body = newBodyStream;

        await _next(context);

        // 🟢 Encrypt Response Body
        newBodyStream.Seek(0, SeekOrigin.Begin);
        using var readerResponse = new StreamReader(newBodyStream);
        string responseBody = await readerResponse.ReadToEndAsync();

        string encryptedResponse = EncryptAES(responseBody);
        var encryptedJson = JsonSerializer.Serialize(new { data = encryptedResponse });

        var responseBytes = Encoding.UTF8.GetBytes(encryptedJson);
        context.Response.Body = originalBodyStream;
        context.Response.ContentLength = responseBytes.Length;
        await context.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
    }

    private static string EncryptAES(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(IV);

        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = ivBytes;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(encryptedBytes);
    }

    private static string DecryptAES(string encryptedText)
    {
        byte[] cipherBytes = Convert.FromBase64String(encryptedText);
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(Key);
        aes.IV = Encoding.UTF8.GetBytes(IV);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
