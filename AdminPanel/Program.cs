using Microsoft.EntityFrameworkCore;
using AdminPanel.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error;
        var message = exception is System.Net.Sockets.SocketException or Npgsql.NpgsqlException
            ? "Не удалось подключиться к базе данных. Проверьте Host/Port в строке подключения и доступность БД."
            : "Произошла непредвиденная ошибка.";

        context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(message)}");
        await Task.CompletedTask;
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Exercises}/{action=Index}/{id?}");

app.Run();
