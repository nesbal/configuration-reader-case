# Configuration Reader Case

.NET 8 ile geliştirilmiş dinamik konfigürasyon yönetimi case projesidir.

Proje iki ana parçadan oluşur:

- `ConfigurationReader`: Uygulamaların kendi konfigürasyonlarını okuyabilmesi için class library.
- `ConfigurationAdmin`: Konfigürasyon kayıtlarını listelemek, eklemek, güncellemek ve filtrelemek için web arayüzü.

## Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core MVC
- MongoDB
- Docker Compose
- xUnit

## Projeyi Docker Compose ile Çalıştırma

Repo ana dizininde:

```bash
docker compose up --build
```

Web arayüzü:

```text
http://localhost:8080/Configurations
```

İlk çalıştırmada MongoDB otomatik olarak örnek case kayıtlarıyla seed edilir.

Seed dosyası:

```text
mongo-init/seed.js
```

Fresh kurulum testi için mevcut volume silinerek yeniden başlatılabilir:

```bash
docker compose down -v
docker compose up --build
```

## Örnek MongoDB Kayıtları

| Name | Type | Value | IsActive | ApplicationName |
|---|---|---|---|---|
| SiteName | string | soty.io | true | SERVICE-A |
| IsBasketEnabled | bool | 1 | true | SERVICE-B |
| MaxItemCount | int | 50 | false | SERVICE-A |

## ConfigurationReader Kullanımı

```csharp
var configurationReader = new ConfigurationReader(
    "SERVICE-A",
    "mongodb://localhost:27017/ConfigurationDb",
    5000
);

var siteName = configurationReader.GetValue<string>("SiteName");
```

## Testleri Çalıştırma

```bash
dotnet test
```

## Case Kapsamı

Bu projede aşağıdaki case beklentileri karşılanmıştır:

- Konfigürasyon kayıtları storage üzerinde tutulur.
- Her servis yalnızca kendi `ApplicationName` değerine ait kayıtları okuyabilir.
- Sadece `IsActive = true` kayıtlar döner.
- `string`, `int`, `double`, `bool` tip dönüşümleri desteklenir.
- Belirli aralıklarla storage kontrol edilir.
- Storage erişilemezse son başarılı cache ile devam edilir.
- Web arayüzünde kayıt listeleme, ekleme, güncelleme ve client-side name filtreleme vardır.
- Docker Compose ile MongoDB ve web uygulaması birlikte çalışır.
- İlk çalıştırmada MongoDB örnek kayıtlarla seed edilir.
- Unit testler eklenmiştir.
