using PostHog.Features;

namespace PostHog.FeatureManagement;

internal static class FeatureManagementExtensions
{
    public static async Task<LocalEvaluator?> GetLocalEvaluatorAsync(this IPostHogClient posthog, CancellationToken cancellationToken)
    {
        // Get the local evaluator from the PostHog client so that we can return a list of the feature flags.
        // This makes me feel dirty, but I don't yet want to add a method to `IPostHogClient` for this just
        // yet because I'll have to support it and I want time to think about it. For example, should it be
        // IAsyncEnumerable or IReadOnlyList? Should it be a method on `IPostHogClient` or a separate interface?
        Func<CancellationToken, Task<LocalEvaluator?>> getAsync = posthog is PostHogClient posthogClient
#pragma warning disable CS0618 // Type or member is obsolete: It's for our internal use only, so we're good.
            ? posthogClient.GetLocalEvaluatorAsync
#pragma warning restore CS0618 // Type or member is obsolete
            : cancelToken =>
            {
                var method = typeof(PostHogClient).GetMethod("GetLocalEvaluatorAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, [typeof(CancellationToken)]);
                return method?.Invoke(posthog, [cancelToken]) as Task<LocalEvaluator?>
                       ?? Task.FromResult<LocalEvaluator?>(null);
            };
        return await getAsync(cancellationToken);
    }
}