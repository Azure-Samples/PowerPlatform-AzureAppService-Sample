// <copyright file="GetDepartmentResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for Department entity.
    /// </summary>
    public class GetDepartmentResponse
    {
        /// <summary>
        /// gets or sets DepartmentId field.
        /// </summary>
        [JsonPropertyName("contoso_departmentid")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// gets or sets DepartmentCode field.
        /// </summary>
        [JsonPropertyName("contoso_departmentcode")]
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// gets or sets DepartmentName field.
        /// </summary>
        [JsonPropertyName("contoso_departmentname")]
        public string? DepartmentName { get; set; }
    }
}
