# 进销存管理系统 / Inventory Management Framework

本仓库包含一个基于 **Blazor + ASP.NET Core** 的简单进销存（采购 / 销售 / 库存）开发框架。

👉 **框架代码与完整文档见 [`src/InventorySystem`](src/InventorySystem/README.md)**

## 快速开始

前置：安装 [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
cd src/InventorySystem
dotnet run
```

首次运行会自动创建 SQLite 数据库并写入演示数据，浏览器打开终端提示的地址即可使用。

## 功能一览

- 仪表盘（库存预警 / 库存成本 / 今日销售）
- 基础资料：商品、分类、供应商、客户
- 采购进货（确认入库）、销售出货（确认出库、库存校验）
- 库存流水与手工盘点调整

## 技术栈

.NET 8 · Blazor Server · EF Core · SQLite · Bootstrap 5

---

> 注：仓库根目录另存有早期的 AI Studio (React) 原型文件，进销存框架与其相互独立。
