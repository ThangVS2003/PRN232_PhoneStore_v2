using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BusinessObject.Models;
using Repository.IRepository;
using Repository.Repository;
using Service.IService;
using Service.Service;
using System.Text;
using Microsoft.OpenApi.Models;

namespace PhoneStoreAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<DbPhoneStoreContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")
                    ?? throw new InvalidOperationException("Connection string 'MyCnn' not found.")));

            // Add Controllers
            builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
         options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
     });

            // Add Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            // Add Swagger with JWT support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhoneStoreAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Add Logging
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            // Register Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ISerialRepository, SerialRepository>();
            builder.Services.AddScoped<IColorRepository, ColorRepository>();
            builder.Services.AddScoped<IVersionRepository, VersionRepository>();
            builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            builder.Services.AddScoped<IFeedbackProductRepository, FeedbackProductRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IDepositHistoryRepository, DepositHistoryRepository>();
            builder.Services.AddScoped<IFeedbackOrderRepository, FeedbackOrderRepository>();
            builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();

            // Register Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ISerialService, SerialService>();
            builder.Services.AddScoped<IColorService, ColorService>();
            builder.Services.AddScoped<IVersionService, VersionService>();
            builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
            builder.Services.AddScoped<IFeedbackProductService, FeedbackProductService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IDepositHistoryService, DepositHistoryService>();
            builder.Services.AddScoped<IFeedbackOrderService, FeedbackOrderService>();
            builder.Services.AddScoped<IVoucherService, VoucherService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}