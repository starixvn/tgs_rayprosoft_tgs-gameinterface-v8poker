using System.Reflection;
using Tgs.GameInterface.V8Poker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services,
		Action<PolicySettingsOptions, IServiceProvider> policySettingsOptions)
		=> services
		.AddOptions<PolicySettingsOptions>()
		.Configure(policySettingsOptions)
		.Services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddHttpClient<IGameApiClient, GameApiClient>()
		.Services
		.AddHttpClient<IGameRecordApiClient, GameRecordApiClient>()
		.Services
		.AddSingleton<IHashCryptography, HashCryptography>()
		.AddSingleton<ISystemClock, SystemClock>();
}
