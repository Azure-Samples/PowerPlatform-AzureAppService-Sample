// <copyright file="BudgetManagementController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Controllers
{
    using System.Net;
    using ExcelImportService.Services;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Budget Management Controller.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class BudgetManagementController : ControllerBase
    {
        private readonly ILogger<BudgetManagementController> logger;
        private readonly IBudgetService budgetService;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetManagementController"/> class.
        /// </summary>
        /// <param name="logger">ILogger.</param>
        /// <param name="budgetService">IBudgetService.</param>
        /// <param name="telemetryClient">TelemetryClient.</param>
        public BudgetManagementController(ILogger<BudgetManagementController> logger, IBudgetService budgetService, TelemetryClient telemetryClient)
        {
            this.logger = logger;
            this.budgetService = budgetService;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Upload budget data from excel.
        /// </summary>
        /// <param name="id">Guid.</param>
        /// <exception cref="ArgumentNullException">Id is null.</exception>
        /// <returns>A <see cref="Task"/> of budget upload operation.</returns>
        [HttpPost(Name = "Upload")]
        public async Task<IActionResult> Post(Guid id)
        {
            if (id == Guid.Empty)
            {
                this.logger.LogError("ID can not be null");
                return this.BadRequest("ID can not be null");
            }

            this.telemetryClient.TrackEvent(
                "Budget upload request",
                new Dictionary<string, string>() { { "requestId", id.ToString() } });
            /* Use kusto query below in Application Insights events:
             *      customEvents
             *      | where name == "Budget upload request"
             */
            using (this.logger.BeginScope(new Dictionary<string, object> { ["requestId"] = id.ToString() }))
            {
                /* Use kusto query below in Application Insights Logs:
                 *      traces
                 *      | extend requestId = customDimensions.requestId
                 *      | where requestId == "<fill the id of the budget import entity record>"
                 */
                this.logger.LogInformation($"Received request to upload budget id's");
                try
                {
                    await this.budgetService.InsertAsync(id);
                }
                catch (Exception ex)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                    {
                        Content = new StringContent(ex.Message),
                        ReasonPhrase = $"Budget upload failed due to following error: {ex.Message} ",
                    };
                    return this.BadRequest(resp);
                }

                this.logger.LogInformation($"Completed the request of uploading budget");
            }

            return await Task.FromResult(this.Ok());
        }
    }
}