// <copyright file="BudgetImportRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using System.Text.Json;
    using Dataverse.RestClient;
    using ExcelImportService.Constants;
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Repository implementation to get all records from Budget Import entity.
    /// </summary>
    public class BudgetImportRepository : IBudgetImportRepository
    {
        private readonly IDataverseClient dataverseClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetImportRepository"/> class.
        /// </summary>
        /// <param name="dataverseClient">IDataverseClient.</param>
        public BudgetImportRepository(IDataverseClient dataverseClient)
        {
            this.dataverseClient = dataverseClient;
        }

        /// <inheritdoc/>
        public virtual async Task<byte[]> GetAttachmentAsync(Guid id)
        {
            var files = await this.dataverseClient.ListAsync<byte[]>(
                EntityNames.BudgetImportEntityName,
                EntityColumnNames.BudgetImportEntityColumnAttachment,
                id,
                withAnnotations: true,
                convert: (jsonElement, _) =>
                {
                    return jsonElement.GetProperty("value").Deserialize<byte[]>() ?? Array.Empty<byte>();
                });
            return files.FirstOrDefault() ?? Array.Empty<byte>();
        }

        /// <inheritdoc/>
        public virtual async Task<GetBudgetImportResponse> GetByIdAsync(Guid id)
        {
            var budgetImportRecords = await this.dataverseClient.ListAsync<GetBudgetImportResponse>(
                EntityNames.BudgetImportEntityName,
                null,
                id,
                withAnnotations: true,
                convert: (jsonElement, _) => jsonElement.Deserialize<GetBudgetImportResponse>() ?? new());
            return budgetImportRecords.FirstOrDefault() ?? new();
        }

        /// <inheritdoc/>
        public virtual async Task UpdateStatusAsync(UpdateStatusRequest status, Guid id)
        {
            _ = await this.dataverseClient.PatchAsync(
                 EntityNames.BudgetImportEntityName,
                 JsonSerializer.Serialize(
                    status,
                    options: new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }),
                 id);
        }

        /// <inheritdoc/>
        public virtual async Task UploadAttachmentAsync(MemoryStream stream, Guid id)
        {
            _ = await this.dataverseClient.PatchAsync(
                EntityNames.BudgetImportEntityName,
                stream,
                id,
                $"{EntityColumnNames.BudgetImportEntityColErrorFile}?x-ms-file-name=errors.xlsx");
        }

        /// <inheritdoc/>
        public virtual async Task UpdateErrorSummaryAsync(UpdateBudgetImportErrorSummaryRequest req, Guid id)
        {
            _ = await this.dataverseClient.PatchAsync(
                EntityNames.BudgetImportEntityName,
                JsonSerializer.Serialize(
                    req,
                    options: new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }),
                id);
        }
    }
}
