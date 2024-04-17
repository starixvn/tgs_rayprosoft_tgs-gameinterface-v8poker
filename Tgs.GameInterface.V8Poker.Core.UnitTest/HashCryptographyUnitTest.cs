namespace Tgs.GameInterface.V8Poker.Core.UnitTest;

public class HashCryptographyUnitTest
{
	[Fact]
	public void Hash()
	{
		var hashCryptography = new HashCryptography();
		var agent = 71033;
		var timestamp = 1690340035435;
		var md5Key = "628DA8F1857A6529";
		var data = $"{agent}{timestamp}{md5Key}";
		var actual = hashCryptography.GetHash(data: data);

		Assert.NotNull(actual);
		Assert.Equal("582fc71defd5d779d58996bdd79d0ab4", actual);
	}

	[Fact]
	public async void AesEncrypt_Balance()
	{
		var hashCryptography = new HashCryptography();
		var secretKey = "C6995933499B163B";
		var data = $"s=7&account=4ue0uj8cwn2v";
		var actual = await hashCryptography.GetAesEncryptAsync(
			data: data,
			secretKey: secretKey);

		Assert.NotNull(actual);
		Assert.Equal("eTRE8SrX53zbk7wbbj95Njc2oinZSQyTHDaRjDyESGM=", actual);
	}

	[Fact]
	public async void AesEncrypt_Deposit()
	{
		var hashCryptography = new HashCryptography();
		var secretKey = "C6995933499B163B";
		var data = $"s=2&account=4ue0uj8cwn2v&money=1.0&orderid=71033202307260300148304ue0uj8cwn2v";
		var actual = await hashCryptography.GetAesEncryptAsync(
			data: data,
			secretKey: secretKey);

		Assert.NotNull(actual);
		Assert.Equal("U0r5u7ylRS0Wv2jMkRtqd7rGS7llaf7XJT/qA8ELt+l9HEvGe3p02oyQGrQjp5BShM9kEh/xx9D8KtQmFzRDo3oRQPIUVno3bDgPbH2X3Ek=", actual);
	}

	[Fact]
	public async void AesEncrypt_Withdrawal()
	{
		var hashCryptography = new HashCryptography();
		var secretKey = "C6995933499B163B";
		var data = $"s=3&account=4ue0uj8cwn2v&orderid=71033202307171354537634ue0uj8cwn2v&money=1";
		var actual = await hashCryptography.GetAesEncryptAsync(
			data: data,
			secretKey: secretKey);

		Assert.NotNull(actual);
		Assert.Equal("0O/RDIqksyjYceULc6rYO7ceTmuol3T9amzImN+1w0RlRIAPwHo9cHa9oLHTYzZltkqg6hORQKGgKv8I+J98rpLeI9qXMQ9xUF8NUyqY0tY=", actual);
	}

	[Fact]
	public async void AesEncrypt_Login()
	{
		var hashCryptography = new HashCryptography();
		var secretKey = "C6995933499B163B";
		var data = $"s=0&account=4ue0uj8cwn2v&money=0&orderid=71033202307260309201174ue0uj8cwn2v&ip=127.0.0.1&lineCode=GPI_VND&KindID=0";
		var actual = await hashCryptography.GetAesEncryptAsync(
			data: data,
			secretKey: secretKey);

		Assert.NotNull(actual);
		Assert.Equal("zU8qJa06cI5nZ8K+ustPHDcUR9O2ttjmAjH7ohQagauiC72LIVNIK1BLEUZKMD/eQpfKEj9Df312jQxpzSKFucvf8M0a1z/njBAocQnTgSDHtu/SOxY7ww9OCRR1RH6OqJK6X9IWx8/bIyYoRLxltPucOtQS42xAVO2KE4wbtDQ=", actual);
	}

	[Fact]
	public async void AesEncrypt_GameRecord()
	{
		var hashCryptography = new HashCryptography();
		var secretKey = "C6995933499B163B";
		var data = $"s=6&startTime=1689573304660&endTime=1689573604660";
		var actual = await hashCryptography.GetAesEncryptAsync(
			data: data,
			secretKey: secretKey);

		Assert.NotNull(actual);
		Assert.Equal("e08+YfVSJDcJ1X6ehrKy1O3QHHO89k5jvRBzn3HCLab8/4eBWe/LW9+zGUQ8CzVO2rzcQhb/jx1COS3Cl43z6A==", actual);
	}
}
