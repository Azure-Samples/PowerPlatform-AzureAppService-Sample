// <copyright file="IBudgetHeaderRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Budget Header entity repository interface.
    /// </summary>
    public interface IBudgetHeaderRepository
    {
        /// <summary>
        /// Get all records from Budget Header entity.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GetBudgetHeaderResponse>> GetAllAsync();

        /// <summary>
        /// Insert into Budget Header entity.
        /// </summary>
        /// <param name="budgetHeader">CreateBudgetHeaderRequest.</param>
        /// <returns>GetBudgetHeaderResponse.</returns>
        Task<GetBudgetHeaderResponse> InsertAsync(CreateBudgetHeaderRequest budgetHeader);

        /// <summary>
        /// Update the total amount for a record.
        /// </summary>
        /// <param name="id">Guid.</param>
        /// <returns>Representing the result of the asynchronous operation.</returns>
        Task UpdateTotalAmountAsync(Guid id);
    }
}
