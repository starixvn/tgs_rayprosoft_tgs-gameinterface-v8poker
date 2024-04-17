namespace Tgs.GameInterface.V8Poker.Core;

public interface IHashCryptography
{
	ValueTask<string> GetAesEncryptAsync(string data, string secretKey, CancellationToken cancellationToken = default);

	string GetHash(string data);
}
