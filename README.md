
### Layered Strategy
* **Presentation (Web API):** RESTful standartlarına uygun, `Middleware` tabanlı merkezi hata yönetimi ve akıllı filtreleme yapılarını barındıran giriş katmanıdır.
* **Business Logic (Services):** İş kurallarının izole edildiği, `Result Pattern` ile servis yanıtlarının standart hale getirildiği ve mülkiyet (Ownership) kontrollerinin yapıldığı çekirdek bölümdür.
* **Data Access (Persistence):** `Generic Repository` ve `Unit of Work` yaklaşımlarıyla veritabanı işlemlerinin soyutlandığı katman. EF Core üzerinden `IQueryable` desteği ile veritabanı seviyesinde yüksek performanslı sorgulama hedeflenmiştir.
* **Core (Entities & DTOs):** Hiçbir katmana bağımlılığı olmayan; Domain modellerini ve veri taşıma yükünü optimize eden **DTO (Data Transfer Object)** yapılarını içerir.

---

## Uygulama

Projenin teknik derinliğini yansıtan kritik detaylar şunlardır:

### Async-First Approach
Sistemdeki tüm veri tabanı, rol yönetimi ve yetkilendirme işlemleri `Task` tabanlı asenkron yapıda (`async/await`) inşa edilmiştir. Bu sayede sunucu kaynakları (Thread Pool) verimli kullanılarak uygulamanın **ölçeklenebilirliği** artırılmıştır.

### Global Exception & Error Handling
Uygulama genelinde karmaşık kod blokları yerine merkezi bir `ErrorMiddleware` yapısı kurulmuştur. Tüm iş mantığı hataları `BusinessException` sınıfı üzerinden yakalanır ve önceden tanımlanmış hata anahtarları (örn: `FarmNotFound`, `InsufficientBalance`) ile istemciye tutarlı yanıtlar döner.

### Advanced Unit Testing Strategy
Kod kalitesi ve güvenilirliği, kapsamlı bir test süiti ile garanti altına alınmıştır:
* **Mocklama:** `Moq` kütüphanesi ile tüm veri tabanı bağımlılıkları izole edilerek sadece iş mantığı test edilmiştir.
* **Asenkron Sorgu Simülasyonu:** EF Core'un asenkron metotlarını (`ToListAsync`, `CountAsync`) test ortamında çalıştırabilmek için `MockQueryable.Moq` kullanılarak **IAsyncQueryProvider** entegrasyonu yapılmıştır.
* **Akıcı Doğrulama:** `FluentAssertions` kullanılarak testlerin okunabilirliği ve sonuçların doğruluğu en üst seviyeye çıkarılmıştır.



### 📊 Generic Filtering & Pagination
Sunucu tarafındaki yükü azaltmak amacıyla tüm liste istekleri `FilterDto` sınıfları üzerinden yönetilir. Filtreleme işlemleri doğrudan veritabanı seviyesinde (Server-side) yapılır ve sadece talep edilen sayfa boyutu kadar veri belleğe alınır.
