using InventorySystem.Components;
using InventorySystem.Data;
using InventorySystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server（交互式服务端渲染）
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// EF Core + SQLite。使用 DbContextFactory 以适配 Blazor 长连接电路。
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=inventory.db";
builder.Services.AddDbContextFactory<AppDbContext>(opt => opt.UseSqlite(connectionString));

// 业务服务
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

// 初始化数据库 + 演示数据
await DbSeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
