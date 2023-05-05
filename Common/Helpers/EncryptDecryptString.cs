using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Common.Helpers;


public static class StringHelper
{
    public static async Task<string> GetRawBodyAsync(this HttpRequest request, Encoding? encoding = null)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

        var body = await reader.ReadToEndAsync().ConfigureAwait(false);

        request.Body.Position = 0;

        return body;
    }

    public static string DecryptUrl(this string encryptedText, string key)
    {
        string decrypted = encryptedText;//encryptedText.Decrypt(key); //ToDo :in future
        return decrypted;
    }

    public static string CleanFileName(this string fileName)
    {
        StringBuilder sb = new StringBuilder();
        char[] charArr = fileName.ToCharArray();
        foreach (char ch in charArr)
        {
            if (ch.ToString().Trim() != String.Empty && ch.ToString().Trim() != ".")
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
}