// <copyright file="GetUserResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for User entity.
    /// </summary>
    public class GetUserResponse
    {
        /// <summary>
        /// gets or sets FullName field.
        /// </summary>
        [JsonPropertyName("fullname")]
        public string? FullName { get; set; }

        /// <summary>
        /// gets or sets SystemUserId field.
        /// </summary>
        [JsonPropertyName("systemuserid")]
        public Guid SystemUserId { get; set; }
    }
}
