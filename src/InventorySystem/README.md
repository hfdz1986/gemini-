# 进销存管理系统（Blazor + ASP.NET Core）

一个**简单、可扩展**的进销存（采购 / 销售 / 库存）开发框架，基于 .NET 8 Blazor Server + EF Core + SQLite，开箱即用，零外部依赖。

## 技术栈

| 层 | 技术 |
|----|------|
| UI | Blazor Server（InteractiveServer 交互式渲染）+ Bootstrap 5 |
| 应用 | ASP.NET Core 8 |
| 数据访问 | EF Core 8 + `IDbContextFactory`（适配 Blazor 长连接电路） |
| 数据库 | SQLite（首次运行自动建库 + 演示数据） |

## 功能模块

- **仪表盘**：商品数、库存预警、库存成本、今日销售额
- **基础资料**：商品、商品分类、供应商、客户（标准 CRUD）
- **采购进货**：录入采购单 → 确认入库（自动增加库存并生成流水）
- **销售出货**：录入销售单 → 确认出库（库存不足自动拦截并回滚）
- **库存流水**：完整出入库记录、按商品筛选、手工盘点调整

## 运行

前置：安装 [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
cd src/InventorySystem
dotnet restore
dotnet run
```

浏览器打开终端提示的地址（如 `https://localhost:5001`）。首次启动会自动创建 `inventory.db` 并写入演示数据。

## 项目结构

```
src/InventorySystem/
├── Models/            领域模型（BaseEntity + 各实体 + 枚举）
├── Data/              AppDbContext、DbSeeder（建库与种子数据）
├── Services/          业务服务
│   ├── CrudService.cs        通用 CRUD 基类（框架核心）
│   ├── MasterDataServices.cs 商品/分类/供应商/客户服务
│   ├── InventoryService.cs   库存过账（保证数量与流水一致）
│   ├── PurchaseService.cs    采购单 + 确认入库（事务）
│   ├── SalesService.cs       销售单 + 确认出库（事务、库存校验）
│   └── DashboardService.cs   仪表盘汇总
├── Components/        Blazor 组件
│   ├── Layout/        侧边导航 + 主布局
│   └── Pages/         各功能页面
├── wwwroot/app.css    样式
└── Program.cs         启动与依赖注入
```

## 设计要点（框架可扩展性）

### 1. 通用 CRUD 基类
新增一个主数据模块（如「仓库」「品牌」）只需三步：

```csharp
// 1) 模型继承 BaseEntity
public class Warehouse : BaseEntity { public string Name { get; set; } = ""; }

// 2) DbContext 增加 DbSet
public DbSet<Warehouse> Warehouses => Set<Warehouse>();

// 3) 继承通用服务即获得全部 CRUD
public class WarehouseService : CrudService<Warehouse>
{
    public WarehouseService(IDbContextFactory<AppDbContext> f) : base(f) { }
}
```
在 `Program.cs` 注册服务、复制一个页面即可。

### 2. 库存一致性
所有库存变化都经过 `InventoryService.Apply()`，它在同一个事务里
**同时**更新 `Product.StockQuantity` 与写入 `StockMovement` 流水，
保证「实时库存 = 流水累加」永不漂移；出库时库存不足会抛异常使事务回滚。

### 3. 单据状态机
单据为 `草稿 → 已确认` 流转：草稿不影响库存、可删除；确认后过账库存、不可删除，
便于后续扩展审核、红冲等业务。
```
