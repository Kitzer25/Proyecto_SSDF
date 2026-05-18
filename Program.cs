using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SistemaSSDF.Models;


var builder = WebApplication.CreateBuilder(args);

var builderconfig = builder.Configuration;

/*
 * Definicion del Context
 */
builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = builderconfig.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("ConnectionString no encontrado");

        options.UseNpgsql(
            connectionString
        );
    }
);


/*
 * Implementación JWT
 */
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builderconfig["JwtSettings:Issuer"],
            ValidAudience = builderconfig["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builderconfig["JwtSettings:SecretKey"] 
                                       ?? throw new InvalidOperationException("SecretKey no encontrado")))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TotalAccess", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("AccessClient", policy => 
        policy.RequireRole("Client"));
});


/*
 * Inyección de Dependencias Repositorio I-R
 */


/*
 * Inyección de Dependencias Servicio I-S
 */


/*
 * Registro de Perfiles AutoMapper
 */
builder.Services.AddAutoMapper(typeof(Program).Assembly);


/* Swagger */
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    /* Swagger */
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* ExceptionHandler */
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case DbUpdateException dbEx:
                context.Response.StatusCode = 400;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Error en la base de datos",
                    detail = dbEx.InnerException?.Message
                });
                break;

            case InvalidOperationException invOp:
                context.Response.StatusCode = 400;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = invOp.Message
                });
                break;
            
            default:
                context.Response.StatusCode = 500;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Error interno del servidor"
                });
                break;
        }
    });
});

/* JWT inicialización */
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers(); //Swagger
app.Run();