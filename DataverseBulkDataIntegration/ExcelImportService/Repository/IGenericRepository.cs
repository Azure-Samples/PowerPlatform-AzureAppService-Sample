// <copyright file="IGenericRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    /// <summary>
    /// Generic repository interface.
    /// </summary>
    public interface IGenericRepository
    {
        /// <summary>
        /// Get all records for an entity.
        /// </summary>
        /// <typeparam name="T">Object.</typeparam>
        /// <param name="entityName">Entity name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public Task<List<T>> GetAllAsync<T>(string entityName)
            where T : new();
    }
}
