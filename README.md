## 🏛️ Technical Architecture

Bu proje, **Separation of Concerns (SoC)** ve **Dependency Inversion (DIP)** prensiplerine dayalı, 4 katmanlı kurumsal bir mimari üzerine inşa edilmiştir. Sistem, yüksek trafikli senaryolarda kaynak tüketimini optimize etmek için uçtan uca **non-blocking asynchronous** yapıda kurgulanmıştır.



### 🏗️ Layered Strategy
* **Presentation (Web API):** RESTful standartlarına uygun, `Middleware` tabanlı global hata yönetimi (Global Exception Handling) ve merkezi `ActionFilter` yapılarını barındıran giriş katmanıdır.
* **Business Logic (Services):** İş kurallarının izole edildiği, `Result Pattern` ile servis yanıtlarının standardize edildiği ve mülkiyet (Ownership) kontrollerinin yapıldığı çekirdek katmandır.
* **Data Access (Persistence):** `Generic Repository Pattern` ve `Unit of Work` yaklaşımlarıyla veritabanı işlemlerinin soyutlandığı katman. EfCore üzerinden `IQueryable` desteği ile veritabanı seviyesinde filtreleme performansı hedeflenmiştir.
* **Core (Entities & DTOs):** Bağımlılığı olmayan, tüm katmanlarca kullanılan Domain modelleri ve katmanlar arası veri taşıma yükünü optimize eden **DTO (Data Transfer Object)** yapılarını içerir.

---

## 🛠️ Implementation & Advanced Engineering

Projenin profesyonel derinliğini yansıtan kritik teknik detaylar şunlardır:

### ⚡ Async-First Approach
Sistemdeki tüm I/O işlemleri (Database, Role Management, Auth) `Task` tabanlı asenkron yapıda inşa edilmiştir. Bu, sunucu üzerindeki **Thread Pool** kaynaklarının verimli kullanılmasını sağlayarak ölçeklenebilirliği (**Scalability**) artırır.

### 🛡️ Global Exception & Error Handling
Uygulama genelinde `try-catch` blokları yerine merkezi bir `ErrorMiddleware` kullanılmıştır. Fırlatılan tüm `BusinessException` hataları, kullanıcı dostu ve önceden tanımlanmış `ErrorKeys` üzerinden standardize edilerek döndürülür.

### 🧪 Advanced Unit Testing Strategy
Kod kalitesi, kapsamlı bir unit test süiti ile garanti altına alınmıştır:
* **Mocking:** `Moq` kütüphanesi ile repository bağımlılıkları tamamen izole edilmiştir.
* **Async Provider Simulation:** Entity Framework Core'un asenkron metotlarını (`ToListAsync`, `CountAsync`) unit testlerde simüle edebilmek için `MockQueryable.Moq` entegrasyonu ile **IAsyncQueryProvider** implementasyonu yapılmıştır.
* **Fluent Verification:** `FluentAssertions` kullanılarak testlerin okunabilirliği ve doğrulanabilirliği artırılmıştır.



### 📊 Generic Filtering & Pagination
Büyük veri setleri ile çalışırken performansı korumak adına tüm liste istekleri `FilterDto` sınıfları üzerinden yönetilir. Filtreleme işlemleri veritabanı seviyesinde (Server-side) yapılır ve sadece ihtiyaç duyulan sayfa boyutu kadar veri belleğe alınır.
