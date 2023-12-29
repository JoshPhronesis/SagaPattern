using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SagaPattern.Commons;

public static class Extensions
{
    public static IApplicationBuilder ListenForSqsEvents(this IApplicationBuilder builder, string[] events)
    {
        var serviceProvider = builder.ApplicationServices;
        var eventListener = serviceProvider.GetRequiredService<IEventListener>();
        Task.Run(() => eventListener.Listen(events, new CancellationToken()));

        return builder;
    }
    public static IServiceCollection AddCommandHandlers(this IServiceCollection collection, Type assemblyType)
    {
        if (assemblyType == null) throw new ArgumentNullException(nameof(assemblyType));
        var assembly = assemblyType.Assembly;
        var scanType = typeof(ICommandHandler<>);
        
        RegisterScanTypeWithImplementations(collection, assembly, scanType, ServiceLifetime.Scoped);

        return collection;
    }
    
    public static IServiceCollection AddEventHandlers(this IServiceCollection collection, Type assemblyType)
    {
        if (assemblyType == null) throw new ArgumentNullException(nameof(assemblyType));
        var assembly = assemblyType.Assembly;
        var scanType = typeof(IEventHandler<>);
        
        RegisterScanTypeWithImplementations(collection, assembly, scanType, ServiceLifetime.Scoped);

        return collection;
    }
    
    private static void RegisterScanTypeWithImplementations(IServiceCollection collection, Assembly assembly, Type scanType,  ServiceLifetime lifetime)
    {
        var commandHandlers = ScanTypes(assembly, scanType);

        foreach (var handler in commandHandlers)
        {
            var abstraction = handler.GetTypeInfo().ImplementedInterfaces
                .First(type => type.IsGenericType && type.GetGenericTypeDefinition() == scanType);

            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    collection.AddSingleton(abstraction, handler);
                    break;
                case ServiceLifetime.Scoped:
                    collection.AddScoped(abstraction, handler);
                    break;
                case ServiceLifetime.Transient:
                    collection.AddTransient(abstraction, handler);
                    break;
                default:
                    throw new ArgumentException("Invalid service lifetime specified.");
            }
        }
    }

    private static IEnumerable<Type> ScanTypes(Assembly assembly, Type typeToScanFor)
    {
        return assembly.GetTypes()
            .Where(type => type is
                           {
                               IsClass: true,
                               IsAbstract: false
                           } &&
                           type.GetInterfaces()
                               .Any(i=>i.IsGenericType && i.GetGenericTypeDefinition() == typeToScanFor));
    }
}