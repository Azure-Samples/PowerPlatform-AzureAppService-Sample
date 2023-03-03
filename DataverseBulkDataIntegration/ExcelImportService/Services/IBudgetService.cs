// <copyright file="IBudgetService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Services
{
    /// <summary>
    /// Interface for budget service.
    /// </summary>
    public interface IBudgetService
    {
        /// <summary>
        /// Insert budget data asynchronously.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task InsertAsync(Guid id);
    }
}
