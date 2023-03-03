// <copyright file="IDataValidations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Validations
{
    using ExcelImportService.Models.Common;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Interface for data validations.
    /// </summary>
    public interface IDataValidations
    {
        /// <summary>
        /// Validatetotal amounts.
        /// </summary>
        /// <param name="row">The row of the excel.</param>
        /// <returns>True is validation passed, else thorws exception.</returns>
        bool TotalValidations(BudgetExcelRow row);

        /// <summary>
        /// Validate referential integrity for budget category.
        /// </summary>
        /// <param name="budgetCategoriesMasterData">Entity records.</param>
        /// <param name="budgetCategoryFromExcel">Lookup value.</param>
        /// <returns>Unique Identifier for input budget category.</returns>
        Guid ValidateBudgetCategory(
            List<GetBudgetCategoryResponse> budgetCategoriesMasterData,
            string? budgetCategoryFromExcel);

        /// <summary>
        /// Validate referential integrity for corporation.
        /// </summary>
        /// <param name="corporationMasterData">Entity records.</param>
        /// <param name="corporationFromExcel">Lookup value.</param>
        /// <param name="parentCorporationFromExcel">Lookup value for parent.</param>
        /// <returns>Unique Identifier for input corporation.</returns>
        Guid ValidateCorporation(
            List<GetCorporationResponse> corporationMasterData,
            string? corporationFromExcel,
            string? parentCorporationFromExcel);

        /// <summary>
        /// Validate referential integrity for department.
        /// </summary>
        /// <param name="departmentMasterData">Entity records.</param>
        /// <param name="departmentFromExcel">Lookup value.</param>
        /// <returns>Unique Identifier for input department.</returns>
        Guid ValidateDepartment(
            List<GetDepartmentResponse> departmentMasterData,
            string? departmentFromExcel);
    }
}
