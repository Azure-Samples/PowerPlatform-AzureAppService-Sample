// <copyright file="IBudgetLineRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Budget Line entity repository interface.
    /// </summary>
    public interface IBudgetLineRepository
    {
        /// <summary>
        /// Get all records from Budget Line entity.
        /// </summary>
        /// <returns>A <see cref="Task{List{GetBudgetLineResponse}}"/> representing the result of the asynchronous operation.</returns>
        Task<List<GetBudgetLineResponse>> GetAllAsync();

        /// <summary>
        /// Insert or update into Budget Line entity one by one.
        /// </summary>
        /// <param name="budgetLine">A <see cref="Task{CreateBudgetLineRequest}"></see>.</param>
        /// <returns>A <see cref="Task{GetBudgetLineResponse}"/> representing the result of the asynchronous operation.</placeholder></returns>
        Task<GetBudgetLineResponse> UpsertAsync(CreateBudgetLineRequest budgetLine);

        /// <summary>
        /// Insert or update into Budget Line entity in batch.
        /// </summary>
        /// <param name="budgetLines">A <see cref="Task{List{CreateBudgetLineRequest}}"></see>.</param>
        /// <returns>A <see cref="Task{List{GetBudgetLineResponse}}"/> representing the result of the asynchronous operation.</placeholder></returns>
        Task<List<GetBudgetLineResponse>> UpsertAsync(List<CreateBudgetLineRequest> budgetLines);

        /// <summary>
        /// Insert or update into Budget Line entity in batch parallelly.
        /// </summary>
        /// <param name="budgetLines">A <see cref="Task{List{CreateBudgetLineRequest}}"></see>.</param>
        /// <returns>A <see cref="Task{List{GetBudgetLineResponse}}"/> representing the result of the asynchronous operation.</placeholder></returns>
        Task<List<GetBudgetLineResponse>> UpsertAsyncParallel(List<CreateBudgetLineRequest> budgetLines);
    }
}
