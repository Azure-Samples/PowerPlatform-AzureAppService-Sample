// <copyright file="EntityColumnNames.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Constants
{
    /// <summary>
    /// Constants for entity columns.
    /// </summary>
    public class EntityColumnNames
    {
        /// <summary>
        /// Budget Line entity "Budget Header" column name in Dataverse.
        /// </summary>
        internal const string BudgetLineBudgetHeader = "_contoso_budgetheader_value";

        /// <summary>
        /// Budget Import entity "Attachment" column name in Dataverse.
        /// </summary>
        internal const string BudgetImportEntityColumnAttachment = "contoso_attachment";

        /// <summary>
        /// Budget Import entity "Error File" column name in Dataverse.
        /// </summary>
        internal const string BudgetImportEntityColErrorFile = "contoso_errorfile";

        /// <summary>
        /// BudgetImport entity status In Progress.
        /// </summary>
        internal const int BudgetImportEntityStatusInprogress = 330650002;

        /// <summary>
        /// BudgetImport entity status Success.
        /// </summary>
        internal const int BudgetImportEntityStatusSuccess = 330650003;

        /// <summary>
        /// BudgetImport entity status Error.
        /// </summary>
        internal const int BudgetImportEntityStatusError = 330650004;
    }
}
