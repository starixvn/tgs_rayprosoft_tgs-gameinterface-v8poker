using System.Security.Cryptography;
using System.Text;

namespace Tgs.GameInterface.V8Poker.Core;

internal class HashCryptography : IHashCryptography
{
	public async ValueTask<string> GetAesEncryptAsync(string data, string secretKey, CancellationToken cancellationToken = default)

	{
		byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
		byte[] dataBytes = Encoding.UTF8.GetBytes(data);
		using var aes = Aes.Create();
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = keyBytes;
		aes.GenerateIV();

		byte[] encrypted;
		using var cryptoTransform = aes.CreateEncryptor();
		using var memoryStream = new MemoryStream();
		using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
		await cryptoStream.WriteAsync(
			buffer: dataBytes,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		await cryptoStream.FlushFinalBlockAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		encrypted = memoryStream.ToArray();

		var base64Str = Convert.ToBase64String(encrypted, 0, encrypted.Length);

		return base64Str;
	}

	public string GetHash(string data)

	{
		var dataBytes = Encoding.UTF8.GetBytes(data);

		using var md5 = MD5.Create();
		using var memoryStream = new MemoryStream(dataBytes);
		var bytes = md5.ComputeHash(inputStream: memoryStream);
		var stringBuilder = new StringBuilder();
		foreach (var b in bytes)
		{
			_ = stringBuilder.Append(b.ToString("x").PadLeft(2, '0'));
		}

		return stringBuilder.ToString();
	}
}
