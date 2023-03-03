// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dataverse.RestClient;
using ExcelImportService.Modules;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(b =>
    {
        _ = b.RegisterModule(new BudgetManagementModule()
        {
            DataverseSpClientId = builder.Configuration["DataverseServicePrincipal:ClientId"],
            DataverseSpClientSecret = builder.Configuration["DataverseServicePrincipal:ClientSecret"],
            DataverseSpTenantId = builder.Configuration["DataverseServicePrincipal:TenantId"],
            DataverseSpAuthDelegationUrl = builder.Configuration["DataverseServicePrincipal:AuthDelegationUrl"],
        });
    });

// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddHttpClient<IDataverseClient, DataverseClient>((httpClient, sp) =>
{
    return new DataverseClient(httpClient, new DataverseClientOptions()
    {
        DataverseBaseUrl = builder.Configuration["DataverseServicePrincipal:BaseUrl"] ?? string.Empty,
    });
}).AddHttpMessageHandler<ConfidentialClientAuthDelegatingHandler>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
