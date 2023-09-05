using Blazored.LocalStorage;
using Client;
using Client.Services;
using Client.Services.Interfaces;
using Client.Services.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IDataProvider, DataProvider>();
builder.Services.AddBlazoredLocalStorage();

//builder.Services.AddHttpClient<HttpClient>(client =>
//{
//    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
//}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();

// Register the Telerik services.
builder.Services.AddTelerikBlazor();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

var app = builder.Build();

await app.RunAsync();
