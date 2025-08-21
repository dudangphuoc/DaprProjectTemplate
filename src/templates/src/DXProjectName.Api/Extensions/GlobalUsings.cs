global using DX.Application.DomainEventsDispatching.Extensions;
global using DX.Application.FailedResponseHandling.Extensions;
global using DX.Application.PipelineBehaviours;
global using DX.EventBus.Dapr.Extensions;
global using DX.Infrastructure.EFCore;
global using DX.Infrastructure.EFCore.DomainEventsDispatching;
global using DX.Infrastructure.EFCore.Exceptions;
global using DX.Infrastructure.EFCore.TransactionalEvents.Extensions;
global using DX.Infrastructure.Extensions;
global using DX.SharedKernel.Exceptions;
global using DX.SharedKernel.OpenTelemetry;
global using DXProjectName.Api.Extensions;
global using DXProjectName.Infrastructure;
global using FluentValidation;
global using HealthChecks.UI.Client;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using OpenTelemetry;
global using OpenTelemetry.Context.Propagation;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using Serilog;
global using Serilog.Enrichers.Span;
global using SharedKernel.OpenTelemetry;
global using StackExchange.Redis;
global using System.Diagnostics;
global using System.Reflection;

namespace DXProjectName.Api.Extensions;

public static class GlobalUsings
{
    public static Action<ResourceBuilder> ConfigureResource;
    public static OpenTelemetryOptions OpenTelemetryOptions;
    public static string LogExporter;
    public static string MetricsExporter = "console";
}