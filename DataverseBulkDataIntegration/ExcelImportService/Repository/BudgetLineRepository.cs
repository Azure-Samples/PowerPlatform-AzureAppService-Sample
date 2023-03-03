// <copyright file="BudgetLineRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using System.Collections.Generic;
    using System.Text.Json;
    using Dataverse.RestClient;
    using ExcelImportService.Constants;
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;
    using Microsoft.Identity.Client;

    /// <summary>
    /// Repository implementation for Budget Header entity.
    /// </summary>
    public class BudgetLineRepository : IBudgetLineRepository
    {
        private readonly IDataverseClient dataverseClient;
        private readonly int maxNumberOfRecordInBatch;
        private readonly int maxNumberOfThreads;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetLineRepository"/> class.
        /// </summary>
        /// <param name="dataverseClient">IDataverseClient.</param>
        public BudgetLineRepository(IDataverseClient dataverseClient)
        {
            this.dataverseClient = dataverseClient;

            // Maximum amount could be 1000 as per Dataverse documentation
            this.maxNumberOfRecordInBatch = 2;

            // Maximum number of threads (currently same as number of processors)
            this.maxNumberOfThreads = 1;
        }

        /// <inheritdoc/>
        public virtual async Task<List<GetBudgetLineResponse>> GetAllAsync()
        {
            var budgetLineRecords = new List<GetBudgetLineResponse>();
            JsonArrayResponse<GetBudgetLineResponse>? response = null;
            do
            {
                response = await this.dataverseClient.ListAsync<GetBudgetLineResponse>(
                        EntityNames.BudgetLineEntityName,
                        null,
                        withAnnotations: true,
                        previousResponse: response,
                        convert: (jsonElement, _) => jsonElement.Deserialize<GetBudgetLineResponse>()!);
                budgetLineRecords.AddRange(response.ToList());
            }
            while (!string.IsNullOrEmpty(response.NextLink));
            return budgetLineRecords;
        }

        /// <inheritdoc/>
        public virtual async Task<GetBudgetLineResponse> UpsertAsync(CreateBudgetLineRequest budgetLine)
        {
            HttpResponseMessage? response = null;
            if (!budgetLine.BudgetLineId.HasValue)
            {
                response = await this.dataverseClient.PostAsync(
                                   EntityNames.BudgetLineEntityName,
                                   JsonSerializer.Serialize(budgetLine),
                                   withRepresentation: true);
            } else
            {
                response = await this.dataverseClient.PatchAsync(
                                   EntityNames.BudgetLineEntityName,
                                   JsonSerializer.Serialize(budgetLine),
                                   budgetLine.BudgetLineId ?? Guid.Empty,
                                   withRepresentation: true);
            }
            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
            {
                return new GetBudgetLineResponse();
            }
            else
            {
                return JsonSerializer.Deserialize<GetBudgetLineResponse>(body) ?? new GetBudgetLineResponse();
            }
        }

        /// <inheritdoc/>
        public virtual async Task<List<GetBudgetLineResponse>> UpsertAsync(List<CreateBudgetLineRequest> budgetLines)
        {
            var budgetLineResponses = new List<GetBudgetLineResponse>();
            var recordsYetToBeCreated = budgetLines.Count;
            int startingIndex = 0;

            do
            {
                var batch = this.dataverseClient.CreateBatchOperation();
                for (int i = startingIndex; i < startingIndex + this.maxNumberOfRecordInBatch; i++)
                {
                    if (budgetLines.ElementAtOrDefault(i) != null)
                    {
                        if (!budgetLines[i].BudgetLineId.HasValue)
                        {
                            batch.AddCreate(EntityNames.BudgetLineEntityName, JsonSerializer.Serialize(budgetLines[i]));
                        }
                        else
                        {
                            batch.AddUpdate(EntityNames.BudgetLineEntityName, budgetLines[i].BudgetLineId ?? Guid.Empty, JsonSerializer.Serialize(budgetLines[i]));
                        }
                    }
                }

                var result = await batch.ProcessAsync();
                var resultOrdered = result.ToList().OrderBy(r => r.Index).ToList();
                resultOrdered.ForEach(r =>
                {
                    budgetLineResponses.Add(new GetBudgetLineResponse()
                    {
                        BudgetLineId = r.Response?.EntityReference?.Id,
                        BudgetHeaderId = budgetLines.ElementAtOrDefault(startingIndex + r.Index)?.BudgetHeaderIdGuid,
                    });
                });

                recordsYetToBeCreated -= this.maxNumberOfRecordInBatch;
                startingIndex += this.maxNumberOfRecordInBatch;
            }
            while (recordsYetToBeCreated > 0);

            return budgetLineResponses;
        }

        /// <inheritdoc/>
        public virtual async Task<List<GetBudgetLineResponse>> UpsertAsyncParallel(List<CreateBudgetLineRequest> budgetLines)
        {
            var budgetLineResponses = new List<GetBudgetLineResponse>();
            var budgetLinesChunks = budgetLines
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / this.maxNumberOfThreads)
                .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
            await Task.Run(() =>
            {
                Parallel.ForEach(
                    budgetLinesChunks,
                    new ParallelOptions { MaxDegreeOfParallelism = this.maxNumberOfThreads },
                    async budgetLineChunk =>
                    {
                        var result = await this.UpsertAsync(budgetLineChunk);
                        budgetLineResponses = Enumerable.Concat(budgetLineResponses, result).ToList();
                    });
            });
            return budgetLineResponses;
        }
    }
}
