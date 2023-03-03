// <copyright file="IBudgetImportRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Budget Import entity repository interface.
    /// </summary>
    public interface IBudgetImportRepository
    {
        /// <summary>
        /// Get budget import attachment.
        /// </summary>
        /// <param name="id">Guid.</param>
        /// <returns>byte[].</returns>
        Task<byte[]> GetAttachmentAsync(Guid id);

        /// <summary>
        /// Get budget import record by id.
        /// </summary>
        /// <param name="id">Guid.</param>
        /// <returns>BudgetIdImport.</returns>
        Task<GetBudgetImportResponse> GetByIdAsync(Guid id);

        /// <summary>
        /// Update the status of the record.
        /// </summary>
        /// <param name="status">UpdateStatusRequest.</param>
        /// <param name="id">Guid.</param>
        /// <returns>representing the asynchronous operation.</returns>
        Task UpdateStatusAsync(UpdateStatusRequest status, Guid id);

        /// <summary>
        /// Upload the error excel of the record.
        /// </summary>
        /// <param name="stream">MemoryStream.</param>
        /// <param name="id">Guid.</param>
        /// <returns>representing the asynchronous operation.</returns>
        Task UploadAttachmentAsync(MemoryStream stream, Guid id);

        /// <summary>
        /// Update the error summary of the record.
        /// </summary>
        /// <param name="req">UpdateBudgetImportErrorSummaryRequest.</param>
        /// <param name="id">Guid.</param>
        /// <returns>representing the asynchronous operation.</returns>
        Task UpdateErrorSummaryAsync(UpdateBudgetImportErrorSummaryRequest req, Guid id);
    }
}