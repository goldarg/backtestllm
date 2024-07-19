using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.Connected_Services;
using api.DataAccess;
using api.Middleware;
using api.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

/////////
///// Adds Microsoft Identity platform (Azure AD B2C) support to protect this Api
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAdB2C", options);
            options.TokenValidationParameters.NameClaimType = "name";

            // var existTokValidation = options.Events.OnTokenValidated;

            // options.Events.OnTokenValidated = async context => {
            //     try
            //     {
            //         await existTokValidation(context);
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine(ex);
            //     }

            // };

            options.Events = new JwtBearerEvents()
            {
                OnTokenValidated = async context =>
                {
                    var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IRdaUnitOfWork>();
                    var userRepository = unitOfWork.GetRepository<User>();

                    var email = context.Principal.FindFirst("preferred_username")?.Value;
                    if (email != null)
                    {
                        // Obtener roles y empresas del usuario desde la base de datos
                        var user = await userRepository.GetAll()
                            .FirstOrDefaultAsync(u => u.userName == email);

                        if (user != null)
                        {
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            // Agregar roles a los claims
                            foreach (var role in user.Roles)
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Rol.nombreRol));
                            }

                            // // Agregar empresas a los claims
                            // foreach (var company in user.Companies)
                            // {
                            //     claimsIdentity.AddClaim(new Claim("company", company.Name));
                            // }
                        }
                    }
                }
            };

            // options.Events = new JwtBearerEvents()
            // {
            //     OnAuthenticationFailed = context =>
            //     {
            //         Console.WriteLine(context.Exception);

            //         return Task.CompletedTask;
            //     }
            // };
        },
    options => { builder.Configuration.Bind("AzureAdB2C", options); });
// End of the Microsoft Identity platform block    

builder.Services.AddControllers();

//////////////

// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AccessTokenHandler>();

builder.Services.AddHttpClient("CrmHttpClient", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://www.zohoapis.com/");

    httpClient.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Bearer", AccessTokenHandler.Instance.AccessToken);
});

builder.Services.AddDbContext<RdaDbContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IRdaUnitOfWork, RdaUnitOfWork>();
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<CRMService>();

// JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
// builder.Services.AddAuthentication().AddMicrosoftIdentityWebApi(builder.Configuration, configSectionName:"AzureAdB2C", jwtBearerScheme:"test");

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
app.UseAuthorization();
app.UseRouting();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();