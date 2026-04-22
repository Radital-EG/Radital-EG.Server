using Domain;
using Domain.People;
using HospitalRequestsAppCore.Interfaces;
using HospitalRequestsAppCore.Services;
using Infrastructure;
using Infrastructure.Implementations;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));

// ── Repository registrations (one per Domain entity used by this API) ─────────
builder.Services.AddScoped<IRepository<ReportingRequest>, Repository<ReportingRequest>>();
builder.Services.AddScoped<IRepository<Patient>, Repository<Patient>>();
builder.Services.AddScoped<IRepository<MedicalImage>, Repository<MedicalImage>>();

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IReportingRequestsManagementService, ReportingRequestsManagementService>();

// ── ASP.NET Core ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
