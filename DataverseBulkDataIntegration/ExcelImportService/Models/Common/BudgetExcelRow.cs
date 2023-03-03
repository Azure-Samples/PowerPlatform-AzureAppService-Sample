// <copyright file="BudgetExcelRow.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
//
namespace ExcelImportService.Models.Common
{
    /// <summary>
    /// Model for importing rows from excel.
    /// </summary>
    public class BudgetExcelRow
    {
        /// <summary>
        /// Gets or sets IsError field in case of import error.
        /// </summary>
        public bool? IsError { get; set; }

        /// <summary>
        /// Gets or sets ErrorReason field in case of import error.
        /// </summary>
        public string? ErrorReason { get; set; }

        /// <summary>
        /// Gets or sets FiscalYear field.
        /// </summary>
        public string? FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets ParentCorporation field.
        /// </summary>
        public string? ParentCorporation { get; set; }

        /// <summary>
        /// Gets or sets Corporation field.
        /// </summary>
        public string? Corporation { get; set; }

        /// <summary>
        /// Gets or sets Department field.
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Gets or sets BudgetCategory field.
        /// </summary>
        public string? BudgetCategory { get; set; }

        /// <summary>
        /// Gets or sets BudgetHolder field.
        /// </summary>
        public string? BudgetHolder { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ1 field.
        /// </summary>
        public float? BudgetedAmountQ1 { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ2 field.
        /// </summary>
        public float? BudgetedAmountQ2 { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ3 field.
        /// </summary>
        public float? BudgetedAmountQ3 { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ4 field.
        /// </summary>
        public float? BudgetedAmountQ4 { get; set; }

        /// <summary>
        /// Gets or sets TotalBudgetedAmount field.
        /// </summary>
        public float? TotalBudgetedAmount { get; set; }
    }
}
