// <copyright file="BudgetService.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Services
{
    using DocumentFormat.OpenXml.Bibliography;
    using DocumentFormat.OpenXml.Spreadsheet;
    using ExcelImportService.Common;
    using ExcelImportService.Constants;
    using ExcelImportService.Controllers;
    using ExcelImportService.Models.Common;
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;
    using ExcelImportService.Repository;
    using ExcelImportService.Validations;
    using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

    /// <summary>
    /// Budget service.
    /// </summary>
    public class BudgetService : IBudgetService
    {
        private readonly ILogger<BudgetManagementController> logger;
        private readonly IBudgetHeaderRepository budgetHeaderRepository;
        private readonly IBudgetLineRepository budgetLineRepository;
        private readonly IBudgetImportRepository budgetImportRepository;
        private readonly IUserRepository userRepository;
        private readonly IGenericRepository genericRepository;
        private readonly IDataValidations dataValidations;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetService"/> class.
        /// </summary>
        /// <param name="logger">ILogger.</param>
        /// <param name="budgetHeaderRepository">IBudgetHeaderRepository.</param>
        /// <param name="budgetLineRepository">IBudgetLineRepository.</param>
        /// <param name="budgetImportRepository">IBudgetImportRepository.</param>
        /// <param name="userRepository">IUserRepository.</param>
        /// <param name="genericRepository">IGenericRepository.</param>
        /// <param name="dataValidations">IDataValidations.</param>
        public BudgetService(
            ILogger<BudgetManagementController> logger,
            IBudgetHeaderRepository budgetHeaderRepository,
            IBudgetLineRepository budgetLineRepository,
            IBudgetImportRepository budgetImportRepository,
            IUserRepository userRepository,
            IGenericRepository genericRepository,
            IDataValidations dataValidations)
        {
            this.logger = logger;
            this.budgetHeaderRepository = budgetHeaderRepository;
            this.budgetLineRepository = budgetLineRepository;
            this.budgetImportRepository = budgetImportRepository;
            this.userRepository = userRepository;
            this.genericRepository = genericRepository;
            this.dataValidations = dataValidations;
        }

        /// <inheritdoc/>
        public async Task InsertAsync(Guid id)
        {
            // Get Budget Import entity record for the input ID
            var budgetImportRecord = await this.budgetImportRepository.GetByIdAsync(id);
            if (budgetImportRecord == null)
            {
                this.logger.LogError($"No record found in Budget Import entity with ID: {id}");
                throw new ArgumentException();
            }

            // Get file attachment from Budget Import record
            var budgetFileAttachment = await this.budgetImportRepository.GetAttachmentAsync(id);
            if (budgetFileAttachment == null)
            {
                this.logger.LogError($"No file attachment found in Budget Import entity with ID: {id}");
                throw new ArgumentException();
            }

            budgetImportRecord.FileAttachment = budgetFileAttachment;

            // Set status to in progress
            var status = new UpdateStatusRequest()
            {
                StatusCode = EntityColumnNames.BudgetImportEntityStatusInprogress,
            };

            this.logger.LogInformation($"Starting to process the request and setting the status to in progress");
            await this.budgetImportRepository.UpdateStatusAsync(status, id);

            // Core business logic
            try
            {
                // Read excel file
                List<BudgetExcelRow> budgetExcelRows = Excel.ReadExcelFromStream<BudgetExcelRow>(new MemoryStream(budgetImportRecord.FileAttachment));
                this.logger.LogInformation($"Received the budget id request excel with {budgetExcelRows.Count} items in it");

                // Handle empty excel file
                if (budgetExcelRows.Count == 0)
                {
                    throw new Exception("Budget excel file uploaded is empty");
                }

                // Build cache for master data
                this.logger.LogInformation("Building cache/ retrieving existing cache for master data");

                var budgetCategories = await this.genericRepository.GetAllAsync<GetBudgetCategoryResponse>(EntityNames.BudgetCategoryEntityName);
                this.logger.LogInformation($"Retrieved {budgetCategories.Count} records from Budget Category Entity");

                var corporations = await this.genericRepository.GetAllAsync<GetCorporationResponse>(EntityNames.CorporationEntityName);
                this.logger.LogInformation($"Retrieved {corporations.Count} records from Corporation Entity");

                var departments = await this.genericRepository.GetAllAsync<GetDepartmentResponse>(EntityNames.DepartmentEntityName);
                this.logger.LogInformation($"Retrieved {departments.Count} records from Department Entity");

                // Parse excel data
                var budgetExcelRowsParsed = new List<BudgetExcelRowParsed>();
                int budgetExcelRowIndex = -1;
                bool isError = false;
                foreach (var row in budgetExcelRows)
                {
                    budgetExcelRowIndex++;
                    this.logger.LogInformation($"Started parsing the row {budgetExcelRowIndex + 1}");

                    try
                    {
                        this.logger.LogInformation("Performing referential integrity validations");
                        var budgetCategoryId = this.dataValidations.ValidateBudgetCategory(budgetCategories, row.BudgetCategory);
                        var corporationId = this.dataValidations.ValidateCorporation(corporations, row.Corporation, row.ParentCorporation);
                        var departmentId = this.dataValidations.ValidateDepartment(departments, row.Department);

                        this.logger.LogInformation("Performing total validations");
                        _ = this.dataValidations.TotalValidations(row);

                        var budgetExcelRowParsed = new BudgetExcelRowParsed()
                        {
                            ExcelRowIndex = budgetExcelRowIndex,
                            FiscalYear = int.Parse((row.FiscalYear ?? string.Empty).Where(char.IsDigit).ToArray()),
                            CorporationId = corporationId,
                            DepartmentId = departmentId,
                            BudgetCategoryId = budgetCategoryId,
                        };

                        // Create Budget Header record
                        budgetExcelRowParsed.BudgetHeaderRequest = this.BuildBudgetHeaderRecord(
                            budgetExcelRowParsed,
                            row);

                        // Create Budget Line record
                        // Budget Header ID will be null since it is not created in Dataverse yet. This will be updated post insert of budget header record.
                        budgetExcelRowParsed.BudgetLineRequest = await this.BuildBudgetLineRecordAsync(
                            budgetExcelRowParsed,
                            row);

                        budgetExcelRowParsed.IsError = false;
                        budgetExcelRowsParsed.Add(budgetExcelRowParsed);

                        this.logger.LogInformation($"Successfully parsed the row {budgetExcelRowIndex + 1}");
                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        row.IsError = true;
                        row.ErrorReason = ex.Message;
                        this.logger.LogError($"Error parsing the row {budgetExcelRowIndex + 1}");
                        this.logger.LogError(ex.Message, ex);
                        continue;
                    }
                }

                // Get existing Budget Header and Line records from Dataverse
                var budgetHeaderExisting = await this.budgetHeaderRepository.GetAllAsync();
                var budgetLineExisting = await this.budgetLineRepository.GetAllAsync();

                // Insert logic for budget header records
                var budgetHeaderRecords = budgetExcelRowsParsed.Where(h => h.IsError == false)
                    .Select(h => h.BudgetHeaderRequest ?? new CreateBudgetHeaderRequest())
                    .DistinctBy(h => new { h.FiscalYear, h.CorporationId, h.DepartmentId })
                    .ToList();
                foreach (var headerRecord in budgetHeaderRecords)
                {
                    var budgetHeaderResponse = new GetBudgetHeaderResponse();

                    if (!budgetHeaderExisting.Where(r => r.FiscalYear == headerRecord.FiscalYear && r.CorporationId == headerRecord.CorporationIdGuid && r.DepartmentId == headerRecord.DepartmentIdGuid).Any())
                    {
                        // Insert if budget header record does not exist in Dataverse and get response of record created
                        budgetHeaderResponse = await this.budgetHeaderRepository.InsertAsync(headerRecord);
                    }
                    else
                    {
                        // Skip insert if budget header already exists in Dataverse and get existing record info
                        foreach (var existingRecord in budgetHeaderExisting.Where(r => r.FiscalYear == headerRecord.FiscalYear && r.CorporationId == headerRecord.CorporationIdGuid && r.DepartmentId == headerRecord.DepartmentIdGuid))
                        {
                            budgetHeaderResponse.BudgetHeaderId = existingRecord.BudgetHeaderId;
                            budgetHeaderResponse.FiscalYear = existingRecord.FiscalYear;
                            budgetHeaderResponse.CorporationId = existingRecord.CorporationId;
                            budgetHeaderResponse.DepartmentId = existingRecord.DepartmentId;
                        }
                    }

                    budgetExcelRowsParsed.Where(r => r.FiscalYear == headerRecord.FiscalYear && r.CorporationId == headerRecord.CorporationIdGuid && r.DepartmentId == headerRecord.DepartmentIdGuid)
                            .ToList()
                            .ForEach(r => r.BudgetHeaderResponse = budgetHeaderResponse);
                }

                this.logger.LogInformation("Created budget header records");

                // Set budget header id for budget line records
                foreach (var parsedRow in budgetExcelRowsParsed)
                {
                    var budgetLineRequest = parsedRow.BudgetLineRequest ?? new CreateBudgetLineRequest();
                    var budgetHeaderResponseReceived = parsedRow.BudgetHeaderResponse ?? new GetBudgetHeaderResponse();
                    budgetLineRequest.BudgetHeaderId = budgetHeaderResponseReceived.BudgetHeaderId.ToString();
                }

                var budgetLineRecords = budgetExcelRowsParsed.Where(l => l.IsError == false)
                    .Select(l => l.BudgetLineRequest ?? new CreateBudgetLineRequest())
                    .ToList();

                foreach (var lineRecord in budgetLineRecords)
                {
                    // Check if budget line record already exists in Dataverse
                    foreach (var existingRecord in budgetLineExisting.Where(r => r.BudgetHeaderId == lineRecord.BudgetHeaderIdGuid && r.BudgetCategoryId == lineRecord.BudgetCategoryIdGuid))
                    {
                        // Set budget line id if it already exists
                        lineRecord.BudgetLineId = existingRecord.BudgetLineId;
                    }
                }

                // Batch upsert of budget line records
                var createBudgetLineResults = await this.budgetLineRepository.UpsertAsync(budgetLineRecords);
                this.logger.LogInformation("Created budget line records");

                // Update the total amount of budget headers records that needs an update
                foreach (var updateRequiredbudgetHeader in createBudgetLineResults.Select(r => r.BudgetHeaderId).Distinct())
                {
                    if (updateRequiredbudgetHeader != null)
                    {
                        await this.budgetHeaderRepository.UpdateTotalAmountAsync((Guid)updateRequiredbudgetHeader);
                    }
                }

                this.logger.LogInformation("Updated total amount in budget header records");

                // If there are any errors, upload the error excel back
                if (isError)
                {
                    this.logger.LogError("Found error while parsing the uploaded excel");
                    var errorFileColumns = budgetExcelRows[0].GetType().GetProperties().Select(p => p.Name).ToList();
                    var errorFileRows = new List<List<string?>>();
                    foreach (var item in budgetExcelRows)
                    {
                        errorFileRows.Add(item.GetType().GetProperties().Select(p =>
                        {
                            if (p.GetValue(item) != null)
                            {
                                return (p.GetValue(item) ?? string.Empty).ToString();
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }).ToList());
                    }

                    var errorFileMemoryStream = Excel.GetExcelDocMemoryStream(errorFileColumns, errorFileRows);
                    await this.budgetImportRepository.UploadAttachmentAsync(errorFileMemoryStream, id);
                    this.logger.LogError("Uploaded the error excel into record of budget id import entity");

                    await this.budgetImportRepository.UpdateErrorSummaryAsync(
                        new UpdateBudgetImportErrorSummaryRequest()
                        {
                            ErrorSummary = "There are errors while updating budgets, download the error.xlsx file for more details",
                        },
                        id);

                    status.StatusCode = EntityColumnNames.BudgetImportEntityStatusError;
                    this.logger.LogError("Making the status reason as error");
                }
                else
                {
                    status.StatusCode = EntityColumnNames.BudgetImportEntityStatusSuccess;
                }
            }
            catch (Exception ex)
            {
                // Set status to error
                status.StatusCode = EntityColumnNames.BudgetImportEntityStatusError;
                await this.budgetImportRepository.UpdateStatusAsync(status, id);
                this.logger.LogError("Updated the status of record of budget import entity to error");
                this.logger.LogError(ex.Message, ex);
                await this.budgetImportRepository.UpdateErrorSummaryAsync(
                       new UpdateBudgetImportErrorSummaryRequest()
                       {
                           ErrorSummary = ex.Message,
                       },
                       id);
                throw;
            }

            // Finllay update the status
            await this.budgetImportRepository.UpdateStatusAsync(status, id);
            this.logger.LogInformation("Completed the request and updating the status");
        }

        private CreateBudgetHeaderRequest BuildBudgetHeaderRecord(
            BudgetExcelRowParsed budgetExcelRowParsed,
            BudgetExcelRow budgetExcelRow)
        {
            return new CreateBudgetHeaderRequest()
            {
                FiscalYear = budgetExcelRowParsed.FiscalYear,
                CorporationId = budgetExcelRowParsed.CorporationId.ToString(),
                DepartmentId = budgetExcelRowParsed.DepartmentId.ToString(),
                BudgetAmount = budgetExcelRow.TotalBudgetedAmount,
                BudgetHeaderName = string.Concat(budgetExcelRow.FiscalYear, " ", budgetExcelRow.Corporation, " ", budgetExcelRow.Department),
            };
        }

        private async Task<CreateBudgetLineRequest> BuildBudgetLineRecordAsync(
            BudgetExcelRowParsed budgetExcelRowParsed,
            BudgetExcelRow budgetExcelRow)
        {
            var budgetHolder = await this.userRepository.GetByNameAsync(budgetExcelRow.BudgetHolder);
            if (budgetHolder == null)
            {
                throw new Exception($"Invalid user: {budgetExcelRow.BudgetHolder}");
            }

            return new CreateBudgetLineRequest()
            {
                BudgetHolderId = budgetHolder.SystemUserId.ToString(),
                BudgetCategoryId = budgetExcelRowParsed.BudgetCategoryId.ToString(),
                BudgetedAmountQ1 = budgetExcelRow.BudgetedAmountQ1,
                BudgetedAmountQ2 = budgetExcelRow.BudgetedAmountQ2,
                BudgetedAmountQ3 = budgetExcelRow.BudgetedAmountQ3,
                BudgetedAmountQ4 = budgetExcelRow.BudgetedAmountQ4,
                TotalBudgetedAmount = budgetExcelRow.TotalBudgetedAmount,
                BudgetLineName = string.Concat(budgetExcelRow.FiscalYear, " ", budgetExcelRow.Corporation, " ", budgetExcelRow.Department, " ", budgetExcelRow.BudgetCategory),
            };
        }
    }
}
