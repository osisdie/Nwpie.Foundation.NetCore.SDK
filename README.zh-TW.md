# Nwpie.Foundation.NetCore.SDK

[![.NET](https://img.shields.io/badge/.NET-8%20%7C%2010%20%7C%2011--preview-blue.svg)](https://dotnet.microsoft.com/download/dotnet)
[![NuGet](https://img.shields.io/nuget/v/Nwpie.Foundation.Abstractions.svg)](https://www.nuget.org/packages?q=Nwpie.Foundation)
[![netstandard2.1](https://img.shields.io/badge/netstandard-2.1-blue.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/actions/workflows/dotnet.yml/badge.svg)](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/actions/workflows/dotnet.yml)
[![Modules](https://img.shields.io/badge/modules-16-green.svg)](#modules-overview)

> [English](README.md) | **繁體中文**

**Nwpie.Foundation.NetCore.SDK** 是一套完整的模組化 .NET SDK，專為企業級應用程式開發設計。提供統一的抽象介面、實用工具與模組化元件，讓開發者可以快速建構微服務架構的應用程式。核心函式庫以 `netstandard2.1` 為目標，確保廣泛的相容性；端點與測試專案則同時支援 `net8.0` 和 `net10.0`。

## 目錄
- [特色功能](#特色功能)
- [安裝方式](#安裝方式)
- [模組總覽](#模組總覽)
- [快速開始](#快速開始)
- [開始使用](#開始使用)
- [貢獻指南](#貢獻指南)
- [授權條款](#授權條款)

---

## 特色功能
- **多目標框架 .NET 8、.NET 10 和 .NET 11 (preview)**：端點與測試專案支援 `net8.0` 和 `net10.0`，CI 同時驗證 .NET 11 preview 相容性。核心函式庫使用 `netstandard2.1`。
- **模組化 SDK**：按需使用，各模組獨立運作。
- **NuGet 套件**：透過 NuGet 安裝個別模組，輕鬆整合。
- **完整測試**：單元測試示範用法並確保可靠性。
- **豐富工具**：涵蓋日誌、組態、資料存取等常見需求。

---

## 安裝方式

透過 NuGet 安裝個別模組：

```bash
dotnet add package Nwpie.Foundation.Abstractions
dotnet add package Nwpie.Foundation.Common
dotnet add package Nwpie.Foundation.Caching.Common
dotnet add package Nwpie.Foundation.Caching.Redis
dotnet add package Nwpie.Foundation.Auth.SDK
dotnet add package Nwpie.Foundation.Http.Common
```

搜尋所有可用套件：

```bash
dotnet nuget search Nwpie.Foundation
```

---

## 模組總覽
SDK 提供以下模組，位於 `src/` 目錄下：

| 模組名稱             | 說明                                                                        |
|---------------------|-----------------------------------------------------------------------------|
| **Abstractions**    | 包含共用介面與基底類別。                                                       |
| **Auth**            | 處理驗證、授權與 Token 管理。                                                  |
| **Caching**         | 提供記憶體快取與分散式快取工具。                                                |
| **Common**          | 通用工具類別與輔助函式。                                                       |
| **Configuration**   | 簡化應用程式組態管理。                                                         |
| **DataAccess**      | 資料庫存取與 ORM 整合工具。                                                    |
| **Extensions**      | 常見 .NET 型別的擴充方法。                                                     |
| **Hosting**         | 應用程式主機與啟動設定。                                                       |
| **Http**            | 簡化 HTTP 請求、API 互動與 REST 用戶端管理。                                   |
| **Location**        | 服務探索、環境對應與主機設定。                                                  |
| **Logging**         | 進階結構化日誌工具。                                                           |
| **Measurement**     | 效能監控與指標收集工具。                                                       |
| **MessageQueue**    | 訊息佇列與事件驅動架構支援。                                                   |
| **Notification**    | Email、SMS 與推播通知服務。                                                    |
| **ServiceNode**     | 服務探索與節點管理工具。                                                       |
| **Storage**         | 檔案與 Blob 儲存操作。                                                        |

---

## 快速開始

### 快取

在 DI 容器中註冊本機或 Redis 快取：

```csharp
// 記憶體快取
services.AddLocalCache<ILocalCache>();
services.AddAsDefaultICache<ILocalCache>();

// Redis 快取（需要 Caching.Redis 模組）
services.AddRedisCache<IRedisCache>();
```

### 序列化

註冊預設 JSON 序列化器：

```csharp
services.AddDefaultSerializer<ISerializer>();
```

### 服務啟動

典型的 `Startup.cs` 設定序列化、快取與模組專屬服務：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDefaultSerializer<ISerializer>();
    services.AddLocalCache<ILocalCache>();
    services.AddAsDefaultICache<ILocalCache>();

    // 加入模組專屬服務
    services.AddConfigServer<IConfigServer>();
}
```

完整範例請參考 [samples/](samples/) 目錄（MiniSite、Serverless）。

---

## 開始使用

### 複製儲存庫

```bash
git clone https://github.com/osisdie/dotnet-nwpie-foundation-sdk.git
cd dotnet-nwpie-foundation-sdk
```

### 瀏覽模組
模組位於 `src/` 目錄下，例如：

- `src/Abstractions`
- `src/Auth`
- ...等等。

每個模組皆為獨立單元，可單獨使用或組合為完整 SDK。

### 執行單元測試

```bash
dotnet test
```

---

## 貢獻指南
歡迎社群貢獻！詳情請參閱 [CONTRIBUTING.md](CONTRIBUTING.md)。

---

## 授權條款
本專案採用 MIT 授權條款。詳見 [LICENSE](LICENSE)。

---

## 支援
如有問題或建議，歡迎在 GitHub 上開 [Issue](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/issues)。
