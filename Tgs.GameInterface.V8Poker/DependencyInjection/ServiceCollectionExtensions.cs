using Tgs.GameInterface.V8Poker.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterface(
		this IServiceCollection services,
		Action<PolicySettingsOptions, IServiceProvider> policySettingsOptions)
		=> services
		.AddGameInterfaceCore(policySettingsOptions);
}
