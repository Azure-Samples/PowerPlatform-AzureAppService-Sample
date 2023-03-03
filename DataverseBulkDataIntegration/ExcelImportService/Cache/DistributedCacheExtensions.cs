// <copyright file="DistributedCacheExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Cache
{
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.Extensions.Caching.Distributed;

    /// <summary>
    /// Extensions methods on IDistributedCache.
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// Gets a value with the given key.
        /// </summary>
        /// <typeparam name="TValue">The return type of cached value.</typeparam>
        /// <param name="distributedCache">An instance of <see cref="IDistributedCache"/>.</param>
        /// <param name="cacheKey">A key identifying the requested value.</param>
        /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation, containing the located value or null.</returns>
        public static async Task<TValue?> GetAsync<TValue>(this IDistributedCache distributedCache, string cacheKey, CancellationToken token = default)
        {
            byte[]? encodedData = await distributedCache.GetAsync(cacheKey, token);

            if (encodedData?.Length > 0)
            {
                string serializedData = Encoding.UTF8.GetString(encodedData);

                return JsonSerializer.Deserialize<TValue>(serializedData);
            }

            return default;
        }

        /// <summary>
        /// Sets the value of given key if key does not exist and return the value.
        /// </summary>
        /// <typeparam name="TValue">The return type of cached value.</typeparam>
        /// <param name="distributedCache">An instance of <see cref="IDistributedCache"/>.</param>
        /// <param name="cacheKey">A key identifying the requested value.</param>
        /// <param name="dataSourceProvider">A delegate that returns the value to be cached.</param>
        /// <param name="cacheOptions">Cache Options.</param>
        /// <param name="token">Optional. The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation, containing the located value or null.</returns>
        public static async Task<TValue?> GetOrSetAsync<TValue>(this IDistributedCache distributedCache, string cacheKey, Func<Task<TValue>> dataSourceProvider, DistributedCacheEntryOptions cacheOptions, CancellationToken token = default)
        {
            TValue? data = await distributedCache.GetAsync<TValue>(cacheKey, token);

            if (data != null)
            {
                return data;
            }
            else
            {
                data = await dataSourceProvider.Invoke();

                if (data != null)
                {
                    string serializedData = JsonSerializer.Serialize(data, GetJsonSerializerOptions());
                    byte[] encodedData = Encoding.UTF8.GetBytes(serializedData);

                    await distributedCache.SetAsync(cacheKey, encodedData, cacheOptions, token);
                }
            }

            return data;
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };
        }
    }
}
