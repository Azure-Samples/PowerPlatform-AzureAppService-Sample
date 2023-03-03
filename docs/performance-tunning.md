# Performance Tunning while inserting bulk data into Dataverse

## Introduction

The custom code that inserts bulk data into Dataverse is implemented as a Web API by using [Dataverse REST Client](https://dev.azure.com/CSECodeHub/506592%20-%20Unilever%20-%20Overheads%20Management/_git/Dataverse%20Rest%20Client). This document describes how to fine tune the performance of the data insertion process.

Let us take a look at one of the Dataverse Repository implementation [BudgetLineRepository.cs](../DataverseBulkDataIntegration/ExcelImportService/Repository/BudgetLineRepository.cs) where different approaches are used to insert bulk data into Dataverse. In the following sections, we will discuss these approaches and their performance implications, target use cases in details.

## Type 1: Inserting records one by one

The first approach is to insert records one by one. The most simple way to insert data. The target use case could be when the number of records to be inserted is very small or when the data insertion process is not performance critical.

With this approach the data insertion might a while to complete.

The following code snippet below shows how to insert records one by one.

```csharp
public virtual async Task<GetBudgetLineResponse> UpsertAsync(CreateBudgetLineRequest budgetLine)
{
    HttpResponseMessage? response = null;
    if (!budgetLine.BudgetLineId.HasValue)
    {
        /// Insert the record one by one
        response = await this.dataverseClient.PostAsync(
                            EntityNames.BudgetLineEntityName,
                            JsonSerializer.Serialize(budgetLine),
                            withRepresentation: true);
    } else
    {
        /// Update the record one by one
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
        /// Deserialize the and return the response
        return JsonSerializer.Deserialize<GetBudgetLineResponse>(body) ?? new GetBudgetLineResponse();
    }
}
```

## Type 2: Inserting records in batches

The second approach is to insert records in batches. The target use case could be when the number of records to be inserted is large and the data insertion process is performance critical.

Where the number of records in batch can be configured with [maximum value of 1000](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/webapi/execute-batch-operations-using-web-api#when-to-use-batch-requests).

With this approach the data insertion will be faster than the first approach and increasing the number of records in batch will further improve the performance.

the following code snippet below shows how to insert records in batches.

```csharp
public virtual async Task<List<GetBudgetLineResponse>> UpsertAsync(List<CreateBudgetLineRequest> budgetLines)
{
    var budgetLineResponses = new List<GetBudgetLineResponse>();
    var recordsYetToBeCreated = budgetLines.Count;
    int startingIndex = 0;

    /// Run the loop until all the records are inserted
    do
    {
        var batch = this.dataverseClient.CreateBatchOperation();
        // Create a list of records to be inserted/updated in batch with maximum number as set in maxNumberOfRecordInBatch configuration
        for (int i = startingIndex; i < startingIndex + this.maxNumberOfRecordInBatch; i++)
        {
            if (budgetLines.ElementAtOrDefault(i) != null)
            {
                if (!budgetLines[i].BudgetLineId.HasValue)
                {
                    // Add the to be created record to the batch
                    batch.AddCreate(EntityNames.BudgetLineEntityName, JsonSerializer.Serialize(budgetLines[i]));
                }
                else
                {
                    /// Add the to be updated record to the batch
                    batch.AddUpdate(EntityNames.BudgetLineEntityName, budgetLines[i].BudgetLineId ?? Guid.Empty, JsonSerializer.Serialize(budgetLines[i]));
                }
            }
        }

        /// Execute the batch
        var result = await batch.ProcessAsync();
        /// Order the result as per the order of the requested records
        var resultOrdered = result.ToList().OrderBy(r => r.Index).ToList();
        resultOrdered.ForEach(r =>
        {
            /// Add the response to the list of responses
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

    /// Return the list of responses in the same order as the requested records
    return budgetLineResponses;
}
```

## Type 3: Inserting records in batches and in parallel

The third approach is to insert records in batches and in parallel. The target use case could be when the number of records to be inserted is large and the performance outcome is not meeting expectation by using the second approach.

Where similarly as second approach the number of records in batch can be configured with maximum value of 1000.

The number of parallel batches must be configured as per number of processing cores are available in the machine/server where the custom code is running, the best practices can be found in [Parallel execution options](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.paralleloptions.maxdegreeofparallelism?view=net-7.0) and [Threading best practices](https://learn.microsoft.com/en-us/dotnet/standard/threading/managed-threading-best-practices#number-of-processors) documents.

With this approach the data insertion will be faster than the first and second approach where increasing the number of parallel batches will further improve the performance.

The following code snippet below shows how to insert records in batches and in parallel.

```csharp
public virtual async Task<List<GetBudgetLineResponse>> UpsertAsyncParallel(List<CreateBudgetLineRequest> budgetLines)
{
    var budgetLineResponses = new List<GetBudgetLineResponse>();

    /// Create chunks of requested records as per the number of parallel batches
    var budgetLinesChunks = budgetLines
        .Select((x, i) => new { Index = i, Value = x })
        .GroupBy(x => x.Index / this.maxNumberOfThreads)
        .Select(x => x.Select(v => v.Value).ToList())
    .ToList();

    /// Execute the chunks in parallel and wait for the completion of all the tasks
    await Task.Run(() =>
    {
        Parallel.ForEach(
            budgetLinesChunks,
            /// Set the limit of parallel batches as per the number of processing cores are available in the machine/server
            new ParallelOptions { MaxDegreeOfParallelism = this.maxNumberOfThreads },
            async budgetLineChunk =>
            {
                /// Execute the batch as explained in the second approach
                var result = await this.UpsertAsync(budgetLineChunk);
                /// Add the batch responses to the list of combined responses from each parallel batch
                budgetLineResponses = Enumerable.Concat(budgetLineResponses, result).ToList();
            });
    });

    /// Return the list of responses
    return budgetLineResponses;
}
```

## Conclusion

As we have seen in this article, there are different ways to insert records in Dataverse. The approach to be used depends on the target use case and the performance outcome expected.

It is recommended to test the performance of each approach and choose the best approach for the target use case rather than choosing most optimal approach.

Additionally we need to keep in mind that the performance results depends on several factors like the payload size in a request, the network latency between the Dataverse environment location and the location where the custom code is running, the number of processing cores are available in the machine/server where the custom code is running, etc.
