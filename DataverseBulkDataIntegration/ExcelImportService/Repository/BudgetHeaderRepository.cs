// <copyright file="BudgetHeaderRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using System;
    using System.Text.Json;
    using Dataverse.RestClient;
    using ExcelImportService.Constants;
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Repository implementation for Budget Header entity.
    /// </summary>
    public class BudgetHeaderRepository : IBudgetHeaderRepository
    {
        private readonly IDataverseClient dataverseClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetHeaderRepository"/> class.
        /// </summary>
        /// <param name="dataverseClient">IDataverseClient.</param>
        public BudgetHeaderRepository(IDataverseClient dataverseClient)
        {
            this.dataverseClient = dataverseClient;
        }

        /// <inheritdoc/>
        public virtual async Task<List<GetBudgetHeaderResponse>> GetAllAsync()
        {
            var budgetHeaderRecords = new List<GetBudgetHeaderResponse>();
            JsonArrayResponse<GetBudgetHeaderResponse>? response = null;
            do
            {
                response = await this.dataverseClient.ListAsync<GetBudgetHeaderResponse>(
                        EntityNames.BudgetHeaderEntityName,
                        null,
                        withAnnotations: true,
                        previousResponse: response,
                        convert: (jsonElement, _) => jsonElement.Deserialize<GetBudgetHeaderResponse>()!);
                budgetHeaderRecords.AddRange(response.ToList());
            }
            while (!string.IsNullOrEmpty(response.NextLink));
            return budgetHeaderRecords;
        }

        /// <inheritdoc/>
        public virtual async Task<GetBudgetHeaderResponse> InsertAsync(CreateBudgetHeaderRequest budgetHeader)
        {
            var response = await this.dataverseClient.PostAsync(
               EntityNames.BudgetHeaderEntityName,
               JsonSerializer.Serialize(budgetHeader),
               withRepresentation: true);
            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
            {
                return new GetBudgetHeaderResponse();
            }
            else
            {
                return JsonSerializer.Deserialize<GetBudgetHeaderResponse>(body) ?? new GetBudgetHeaderResponse();
            }
        }

        /// <inheritdoc/>
        public virtual async Task UpdateTotalAmountAsync(Guid id)
        {
            var budgetLineRecords = new List<GetBudgetLineResponse>();
            JsonArrayResponse<GetBudgetLineResponse>? response = null;
            do
            {
                response = await this.dataverseClient.ListAsync<GetBudgetLineResponse>(
                    EntityNames.BudgetLineEntityName,
                    filter: $"{EntityColumnNames.BudgetLineBudgetHeader} eq '{id}'",
                    withAnnotations: true,
                    convert: (jsonElement, _) => jsonElement.Deserialize<GetBudgetLineResponse>() ?? new GetBudgetLineResponse());
                budgetLineRecords.AddRange(response.ToList());
            }
            while (!string.IsNullOrEmpty(response.NextLink));

            var totalAmounts = budgetLineRecords.Where(b => b.BudgetLineId != Guid.Empty && b.TotalBudgetedAmount != null).Sum(b => b.TotalBudgetedAmount);

            _ = await this.dataverseClient.PatchAsync(
                EntityNames.BudgetHeaderEntityName,
                JsonSerializer.Serialize(
                    new UpdateBudgetLineTotalAmountRequest() { BudgetAmount = totalAmounts },
                    options: new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }),
                id);
        }
    }
}
