// <copyright file="BudgetExcelRowParsed.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Common
{
    using ExcelImportService.Models.Requests;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Model for parsed row from excel.
    /// </summary>
    public class BudgetExcelRowParsed
    {
        /// <summary>
        /// Gets or sets ExcelRowIndex field.
        /// </summary>
        public int ExcelRowIndex { get; set; }

        /// <summary>
        /// Gets or sets IsError field in case of import error.
        /// </summary>
        public bool? IsError { get; set; }

        /// <summary>
        /// Gets or sets FiscalYear field.
        /// </summary>
        public int FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets CorporationId field.
        /// </summary>
        public Guid CorporationId { get; set; }

        /// <summary>
        /// Gets or sets DepartmentId field.
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets BudgetCategoryId field.
        /// </summary>
        public Guid BudgetCategoryId { get; set; }

        /// <summary>
        /// Gets or sets Budget Header response.
        /// </summary>
        public GetBudgetHeaderResponse? BudgetHeaderResponse { get; set; }

        /// <summary>
        /// Gets or sets Budget Header request.
        /// </summary>
        public CreateBudgetHeaderRequest? BudgetHeaderRequest { get; set; }

        /// <summary>
        /// Gets or sets Budget Line response.
        /// </summary>
        public GetBudgetLineResponse? BudgetLineResponse { get; set; }

        /// <summary>
        /// Gets or sets Budget Line request.
        /// </summary>
        public CreateBudgetLineRequest? BudgetLineRequest { get; set; }
    }
}
