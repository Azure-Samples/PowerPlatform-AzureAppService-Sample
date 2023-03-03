// <copyright file="GenericRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using System.Text.Json;
    using Autofac.Features.AttributeFilters;
    using Dataverse.RestClient;
    using ExcelImportService.Cache;
    using Microsoft.Extensions.Caching.Distributed;

    /// <summary>
    /// Generic repository implementation to get all records from any entity.
    /// </summary>
    public class GenericRepository : IGenericRepository
    {
        private readonly IDataverseClient dataverseClient;
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository"/> class.
        /// </summary>
        /// <param name="dataverseClient">IDataverseClient.</param>
        /// <param name="distributedCache">IDistributedCache.</param>
        public GenericRepository(
            IDataverseClient dataverseClient,
            [KeyFilter(DistributedCacheKey.MasterData)] IDistributedCache distributedCache)
        {
            this.dataverseClient = dataverseClient;
            this.distributedCache = distributedCache;
        }

        /// <inheritdoc/>
        public virtual async Task<List<T>> GetAllAsync<T>(string entityName)
            where T : new()
        {
            return await this.distributedCache.GetOrSetAsync(
                entityName,
                async () =>
                {
                    var entityRecords = new List<T>();
                    JsonArrayResponse<T>? response = null;
                    do
                    {
                        response = await this.dataverseClient.ListAsync<T>(
                                entityName,
                                null,
                                withAnnotations: true,
                                previousResponse: response,
                                convert: (jsonElement, _) => jsonElement.Deserialize<T>()!);
                        entityRecords.AddRange(response.ToList());
                    }
                    while (!string.IsNullOrEmpty(response.NextLink));
                    return entityRecords;
                },
                new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) }) ?? new();
        }
    }
}
