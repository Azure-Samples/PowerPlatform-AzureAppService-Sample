// <copyright file="BudgetManagementModule.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Modules
{
    using System.Reflection;
    using Autofac;
    using Dataverse.RestClient;
    using ExcelImportService.Cache;
    using ExcelImportService.Services;
    using ExcelImportService.Validations;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Identity.Client;
    using Microsoft.Identity.Web;

    /// <summary>
    /// Base Autofac BudgetManagementModule.
    /// </summary>
    public class BudgetManagementModule : Autofac.Module
    {
        /// <summary>
        /// Gets or sets dataverse service principal client id.
        /// </summary>
        public string? DataverseSpClientId { get; set; }

        /// <summary>
        /// Gets or sets dataverse service principal client secret.
        /// </summary>
        public string? DataverseSpClientSecret { get; set; }

        /// <summary>
        /// Gets or sets dataverse service principal tenant id.
        /// </summary>
        public string? DataverseSpTenantId { get; set; }

        /// <summary>
        /// Gets or sets dataverse service principal auth delegation url.
        /// </summary>
        public string? DataverseSpAuthDelegationUrl { get; set; }

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(this.DataverseSpClientId)
                .WithClientSecret(this.DataverseSpClientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{this.DataverseSpTenantId}"))
                .Build();
            app.AddInMemoryTokenCache();

            builder.RegisterInstance(app);

            builder.RegisterType<ConfidentialClientAuthDelegatingHandler>()
                .WithParameter((p, _) => p.Name?.Equals("scopes") == true, (_, c) => new List<string>() { this.DataverseSpAuthDelegationUrl ?? string.Empty });
            this.AddCache(builder);
            builder.RegisterType<BudgetService>().As<IBudgetService>();
            builder.RegisterType<DataValidations>().As<IDataValidations>();
            builder.RegisterAssemblyTypes(typeof(BudgetManagementModule).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            base.Load(builder);
        }

        private void AddCache(ContainerBuilder builder)
        {
            foreach (var val in Enum.GetValues<DistributedCacheKey>())
            {
                builder.RegisterType<MemoryDistributedCache>().Keyed<IDistributedCache>(val).SingleInstance();
            }
        }
    }
}
