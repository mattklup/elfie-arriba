// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Arriba.Caching;
using Arriba.Client.Serialization.Json;
using Arriba.Diagnostics.Tracing;
using Arriba.Communication;
using Arriba.Communication.Application;
using Arriba.Communication.ContentTypes;
using Arriba.Communication.Server.Application;
using Arriba.Configuration;
using Arriba.Model;
using Arriba.Model.Correctors;
using Arriba.Monitoring;
using Arriba.Observability;
using Arriba.Serialization.Json;
using Arriba.Server;
using Arriba.Server.Application;
using Arriba.Server.Authentication;
using Arriba.Server.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Arriba.Composition
{
    public static class HostExtensions
    {
        public static void AddArribaServices(this IServiceCollection services, ISecurityConfiguration config)
        {
            ArribaServices.Initialize();

            services.AddSingleton<ArribaLog>();
            services.AddSingleton<ISecurityConfiguration>(config);

            services.AddContentReadersWriters();
            services.AddJsonConverters();
            services.AddArribaManagementService();

            services.AddSingleton<ClaimsAuthenticationService>();
            services.AddSingleton<ITelemetry>((_) => new Telemetry(MonitorEventLevel.Verbose, "HTTP", null));
            services.AddSingleton<IArribaQueryServices, ArribaQueryServices>();
            services.AddSingleton<IObjectCacheFactory, MemoryCacheFactory>();
            services.AddSingleton<SecureDatabase>();
            services.AddSingleton<DatabaseFactory>();
            services.AddSingleton<ICorrector, TodayCorrector>();

            services.AddTransient<IRoutedApplication, ArribaImportApplication>();
            services.AddTransient<IRoutedApplication, ArribaQueryApplication>();
            services.AddTransient<IRoutedApplication, ArribaTableRoutesApplication>();

            services.AddApplication();
            services.AddSingleton<ApplicationServer, ComposedApplicationServer>();
        }

        private static void AddContentReadersWriters(this IServiceCollection services)
        {
            services.AddTransient<IContentReader,StringContentReader>();
            services.AddTransient<IContentReader, JsonContentReader>();
            services.AddTransient<IContentWriter, StringContentWriter>();
            services.AddTransient<IContentWriter, JsonContentWriter>();
            services.AddTransient<IContentWriter, JsonpContentWriter>();

            // Direct type requried by JsonPContentWriter
            services.AddTransient<JsonContentWriter>();
        }

        private static void AddJsonConverters(this IServiceCollection services)
        {
            services.AddTransient<JsonConverter,ColumnDetailsJsonConverter>();
            services.AddTransient<JsonConverter, DataBlockJsonConverter>();
            services.AddTransient<JsonConverter, IAggregatorJsonConverter>();
            services.AddTransient<JsonConverter, IExpressionJsonConverter>();
            services.AddTransient<JsonConverter, ValueJsonConverter>();
            services.AddTransient<JsonConverter, ByteBlockJsonConverter>();
        }

        private static void AddArribaManagementService(this IServiceCollection services)
        {
            services.AddSingleton<ArribaManagementService>();
            services.AddSingleton<IArribaManagementService>(
                sp =>
                {
                    return new ArribaManagementServiceObserver(
                        sp.GetRequiredService<ArribaLog>(),
                        sp.GetRequiredService<ArribaManagementService>());
                });
        }

        private static void AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<RoutedApplicationHandler>();
            services.AddSingleton<IApplication>(
                sp =>
                {
                    return new ArribaApplicationObserver(
                        sp.GetRequiredService<ArribaLog>(),
                        sp.GetRequiredService<RoutedApplicationHandler>());
                });
        }
    }
}
