using System.Diagnostics;

using OpenTelemetry.Context.Propagation;

namespace Ecomm.Shared.Infrastructure.Messaging;

internal static class MessagingDiagnostics
{
    public static readonly string ActivitySourceName = $"{AppDomain.CurrentDomain.FriendlyName}.Messaging";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    public static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
}
