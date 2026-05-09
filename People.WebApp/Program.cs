using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using People.WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// kevault
var kvName = builder.Configuration["KeyVaultName"];
var mi = builder.Configuration["ManagedIdentityClientId"];
var vaultUri = new Uri($"https://{kvName}.vault.azure.net/");
var defaultAzureCredentialOptions = new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = mi
};
var defaultAzureCredential = new DefaultAzureCredential(defaultAzureCredentialOptions);
builder.Configuration.AddAzureKeyVault(vaultUri, defaultAzureCredential);

// Add services to the container.
builder.Services.AddControllersWithViews();

// PeopleDbContext
builder.Services.AddDbContext<PeopleDbContext>(options =>
{
    if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
    {
        options.UseInMemoryDatabase("People");
    }
    else
    {
        var connectionStringSecretName = builder.Configuration.GetValue<string>("SecretName");
        if (string.IsNullOrWhiteSpace(connectionStringSecretName)) throw new ArgumentNullException("Can't find name of the secret.");
        var kvSecret = builder.Configuration.GetValue<string>(connectionStringSecretName);
        options.UseSqlServer(kvSecret);
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");        
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
