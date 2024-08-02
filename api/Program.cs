using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.Connected_Services;
using api.DataAccess;
using api.Logic;
using api.Middleware;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AccessTokenHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(
    "CrmHttpClient",
    httpClient =>
    {
        httpClient.BaseAddress = new Uri("https://www.zohoapis.com/");

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                AccessTokenHandler.Instance.AccessToken
            );
    }
);

builder.Services.AddDbContext<RdaDbContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IRdaUnitOfWork, RdaUnitOfWork>();
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<VehiculosLogica>();
builder.Services.AddScoped<CRMService>();
builder.Services.AddScoped<IUserIdentityService, UserIdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IContratoService, ContratoService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<IClaimsProvider, HttpContextClaimsProvider>();
builder.Services.AddScoped<IActividadUsuarioService, ActividadUsuarioService>();

///// Adds Microsoft Identity platform (Azure AD B2C) support to protect this Api
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAdB2C");

builder
    .Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure(
        (options) =>
        {
            options.Events ??= new JwtBearerEvents();
            var onTokenValidated = options.Events.OnTokenValidated;
            options.Events.OnTokenValidated = async context =>
            {
                await onTokenValidated(context);
                var unitOfWork =
                    context.HttpContext.RequestServices.GetRequiredService<IRdaUnitOfWork>();
                var userRepository = unitOfWork.GetRepository<User>();

                var email = context.Principal.FindFirst("preferred_username")?.Value;
                if (email != null)
                {
                    // Obtener roles y empresas del usuario desde la base de datos
                    var user = await userRepository
                        .GetAll()
                        .FirstOrDefaultAsync(u => u.userName == email);

                    if (user != null)
                    {
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        // Agregar roles a los claims
                        foreach (var role in user.Roles)
                            claimsIdentity.AddClaim(
                                new Claim(claimsIdentity.RoleClaimType, role.Rol.nombreRol)
                            );

                        foreach (var empresa in user.EmpresasAsignaciones)
                        {
                            claimsIdentity.AddClaim(new Claim("empresas", empresa.Empresa.idCRM));
                        }

                        // // Agregar empresas a los claims
                        // foreach (var company in user.Companies)
                        // {
                        //     claimsIdentity.AddClaim(new Claim("company", company.Name));
                        // }
                    }
                }
            };
        }
    );

// End of the Microsoft Identity platform block


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
