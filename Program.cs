using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Firebase.Storage;



var builder = WebApplication.CreateBuilder(args);




//register services
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<TemplateService>();
builder.Services.AddScoped<AppService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<FacebookService>();
builder.Services.AddScoped<GoogleService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<ServiceTypesService>();
builder.Services.AddScoped<StatusService>();
builder.Services.AddScoped<ImageService>();



//Firebase
var firebaseApp = FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase-service-account.json")
});
// Read the bucket name from configuration
var bucketName = builder.Configuration["Authentication:Firebase:Bucket"];

// Register FirebaseStorage
// Register FirebaseStorage with an Auth Token
builder.Services.AddSingleton(provider =>
{
    var googleCredential = GoogleCredential.FromFile("firebase-service-account.json");
    return new FirebaseStorage(bucketName, new FirebaseStorageOptions
    {
        AuthTokenAsyncFactory = async () =>
        {
            var accessToken = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return accessToken; // Pass access token for authenticated requests
        },
        ThrowOnCancel = true
    });
});

//Handle Cyclesd
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
    
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

//Add authentication services
var jwtSettings = builder.Configuration.GetSection("Authentication:JwtSettings") ?? throw new Exception("Jwt settings not found");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };

});
builder.Services.AddAuthorization();

// Add CORS policy to allow all origins
builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin() // Allow requests from any origin
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

// handler for the main route ("/api)
var handler = () => "Welcome to the car wash API. Go to '/swagger/index.html' to see all the routes and learn more about the API. Enjoy!";
app.MapGet("/api", handler);

//Run the application
app.Run();
