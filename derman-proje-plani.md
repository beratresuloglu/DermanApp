```mermaid
# Derman — Afet Sonrası Yardım Eşleştirme Platformu — 1 Aylık Staj Proje Planı

**Toplam süre:** 26 Ağustos – 20 Eylül 2026 (bu hafta hazırlık + 21 günlük yoğun geliştirme)

---

## 📌 Proje Özeti

**Derman**, afet bölgesindeki ihtiyaç sahiplerini (Afetzede paneli) ve yardım edebilecek kişileri (Yardımcı paneli) konum bazlı olarak eşleştiren, mesajlaşma ve yapay zeka destekli önceliklendirme sunan, **güvenlik önceliklendirmesiyle tasarlanmış** bir web platformu.

**İsim hikayesi:** "Derman", Türkçede "çare, yardım olmak" anlamına gelen köklü bir kelime.

**Neden değerli:** 6 Şubat depreminde yaşanan en büyük sorunlardan biri kaynakların koordinesiz dağılmasıydı. Derman, bu boşluğu gerçek zamanlı eşleştirme ve AI destekli önceliklendirme ile kapatmayı hedefliyor — ama bunu yaparken kullanıcıların (özellikle savunmasız durumdaki Afetzedelerin) güvenliğini birinci öncelik olarak ele alıyor.

---

## 🛡️ Güvenlik ve Kötüye Kullanım Önlemleri (MVP'ye Entegre)

Bu proje gerçek insanların konum ve kişisel bilgilerini işlediği için, iki ana riski MVP tasarımının merkezine aldık:

### Risk 1: Konum İfşası ile Kötüye Kullanım (yağma, zorla alma)

**Önlem — Kademeli Konum Görünürlüğü:**

- Harita üzerinde talepler/teklifler **varsayılan olarak yaklaşık bir alan** (300-500m yarıçaplı bulanık daire) olarak gösterilir, tam adres gizli kalır
- Tam konum ve iletişim bilgisi, sadece **iki yönlü onay** tamamlandıktan sonra açılır (aşağıya bakın)

**Önlem — İki Yönlü Onay Mekanizması:**

- Yardımcı bir talebi "üstlenmek istiyorum" dediğinde otomatik eşleşme olmaz
- Afetzede'ye bildirim gider, Afetzede mesajlaşma üzerinden karşı tarafla iletişim kurup **onayladıktan sonra** tam konum paylaşılır
- Afetzede reddedebilir veya farklı bir Yardımcı ile devam edebilir — kontrol her zaman ihtiyaç sahibinde kalır

### Risk 2: Mesajlaşma Üzerinden Taciz

**Önlem — Şikayet / Engelleme Sistemi:**

- Her mesaj/kullanıcı profili üzerinde "Şikayet Et" ve "Engelle" butonu bulunur
- Engellenen kullanıcı, engelleyen kişiyle bir daha eşleşemez/mesajlaşamaz
- Belirli bir eşiği (örn. 3 farklı kişiden şikayet) aşan hesaplar otomatik olarak "incelemede" durumuna alınır ve yeni talep/teklif oluşturamaz — basit bir flag sistemi, karmaşık moderasyon paneli gerekmez

**Önlem — Kimlik Doğrulama Eşiği:**

- Hesap, telefon numarası doğrulaması (OTP) tamamlanmadan aktif olamaz
- Bu, anonim/sahte hesap açmayı zorlaştırır ve caydırıcı bir eşik oluşturur
- _Pratik not:_ Gerçek SMS servisi (Twilio vb.) maliyet/kurulum gerektirir; demo/staj kapsamında **e-posta ile OTP doğrulama** kullanmak da aynı güvenlik mesajını verir ve daha az zaman alır — sunumda "production'da SMS OTP'ye geçilir" diye belirtmen yeterli

### Gelecek Planı Olarak Bırakılanlar (kapsam dışı ama sunumda bahsedilecek)

- İtibar/puanlama sistemi (geçmiş yardımlaşmalara dayalı güven skoru)
- STK/kurum odaklı doğrulama katmanı (öncelikli, onaylı hesaplar)
- Moderatör/insan gözetimi paneli
- TC Kimlik/e-Devlet entegrasyonu ile güçlü kimlik doğrulama

---

## 🛠️ Teknoloji Stack'i

| Katman            | Teknoloji                                                              |
| ----------------- | ---------------------------------------------------------------------- |
| Backend           | ASP.NET Core Web API                                                   |
| Frontend          | Blazor Server (responsive)                                             |
| Veritabanı        | PostgreSQL                                                             |
| ORM               | Entity Framework Core                                                  |
| Gerçek zamanlılık | SignalR (mesajlaşma + canlı harita güncellemeleri)                     |
| Harita            | Leaflet.js (JS Interop ile)                                            |
| Auth              | ASP.NET Core Identity (rol bazlı: Afetzede / Yardımcı) + OTP doğrulama |
| Yapay Zeka        | Anthropic Claude API (`HttpClient` ile REST çağrısı)                   |
| Deployment        | Azure App Service veya Render.com                                      |

---

## 🧩 Panel Yapısı

### Afetzede Paneli

1. Kayıt/giriş + telefon/e-posta OTP doğrulama
2. Talep oluşturma (ihtiyaç, not, konum)
3. Harita: kendi konumu + bulanık gösterilen yakın teklifler + kurumlar
4. Mesajlaşma (şikayet/engelle butonlu)
5. AI Aciliyet Skoru
6. Bir Yardımcı'yı onaylayınca tam konum/iletişim açılması

### Yardımcı Paneli

1. Kayıt/giriş + OTP doğrulama
2. Teklif oluşturma (malzeme, miktar, konum)
3. Harita: kendi konumu + bulanık gösterilen yakın talepler
4. Mesajlaşma (şikayet/engelle butonlu)
5. AI Bölge Öncelik Analizi
6. Bir talebi "üstlenmek istiyorum" deme, onay bekleme

---

## 🤖 Yapay Zeka Entegrasyonu

### 1. AI Aciliyet Skoru

Talep metni Claude API'ye gönderilir, aciliyet kategorisi (Düşük/Orta/Kritik) + kısa gerekçe döner. AI karar vermez, öneri sunar — kullanıcı override edebilir.

### 2. AI Bölge Öncelik Analizi

Yardımcının yakınındaki açık talepler Claude API'ye gönderilir, model önceliklendirip kısa gerekçeli bir öneri döner.

**Ortak notlar:** API anahtarı `dotnet user-secrets` ile saklanır; AI çağrıları asenkron yapılır; JSON çıktı formatı zorunlu tutulur, parse hatasına karşı fallback değer tanımlanır.

---

## 🗓️ Bu Hafta (26–30 Ağustos) — Hazırlık Haftası

| Gün                   | Süre        | Görev                                                                                                             |
| --------------------- | ----------- | ----------------------------------------------------------------------------------------------------------------- |
| **Çrş 26 Ağu**        | ~1 saat     | MVP kapsamını (güvenlik önlemleri dahil) netleştir, README taslağı                                                |
| **Prş 27 Ağu**        | ~1-1.5 saat | Solution kurulumu, GitHub reposu, Claude API key → user-secrets                                                   |
| **Cuma 28 Ağu**       | ~1-1.5 saat | PostgreSQL, EF Core, entity taslakları (User+rol+OTP alanları, HelpRequest, HelpOffer, Message, Report, Resource) |
| **Cmt-Paz 29-30 Ağu** | —           | Meşgulsün, dokunma                                                                                                |

---

## ✅ MVP Kapsamı

**Zorunlu (Must-have):**

1. Rol bazlı kayıt/giriş + OTP doğrulama (e-posta veya SMS)
2. Afetzede: talep oluşturma
3. Yardımcı: teklif oluşturma
4. Harita: kademeli/bulanık konum gösterimi (her iki panelde)
5. **İki yönlü onay mekanizması** (tam konum sadece onay sonrası açılır)
6. Mesajlaşma (SignalR, gerçek zamanlı)
7. **Şikayet/Engelleme sistemi**
8. AI Aciliyet Skoru
9. AI Bölge Öncelik Analizi
10. Responsive tasarım

**İyi olur (Nice-to-have, zaman kalırsa):** 11. Otomatik "incelemede" flag'i sonrası e-posta bildirimi 12. Statik kurum verisini genişletme (daha fazla bölge/örnek)

**Şimdilik atla:**

- Native mobil uygulama
- İtibar/puanlama sistemi
- STK doğrulama katmanı, moderatör paneli
- Kurum verisi için canlı API entegrasyonu

---

## 📅 21 Günlük Geliştirme Planı (31 Ağustos – 20 Eylül)

### Hafta 1: Auth + Güvenlik Temeli + Panel İskeletleri (31 Ağu – 6 Eyl)

| Gün        | Odak                                                                                                  |
| ---------- | ----------------------------------------------------------------------------------------------------- |
| Pzt 31 Ağu | Solution son hali, PostgreSQL, DbContext, migration, temel entity'ler                                 |
| Sal 1 Eyl  | Identity kurulumu, rol bazlı kayıt/giriş, **OTP doğrulama akışı** (e-posta ile kod gönderme/onaylama) |
| Çrş 2 Eyl  | Afetzede paneli: talep oluşturma formu + API                                                          |
| Prş 3 Eyl  | Yardımcı paneli: teklif oluşturma formu + API                                                         |
| Cuma 4 Eyl | `Message` entity'si + temel mesaj CRUD + **Report/Block entity'leri ve temel backend mantığı**        |
| Cmt 5 Eyl  | Toparlama, ara test                                                                                   |
| Paz 6 Eyl  | **Dinlen**                                                                                            |

### Hafta 2: Harita + Konum Gizliliği + AI Aciliyet Skoru (7–13 Eyl)

| Gün         | Odak                                                                                                              |
| ----------- | ----------------------------------------------------------------------------------------------------------------- |
| Pzt 7 Eyl   | Leaflet.js entegrasyonu, kendi konum gösterimi                                                                    |
| Sal 8 Eyl   | `Resource` seed verisi + Afetzede haritası + **konum bulanıklaştırma mantığı** (yaklaşık alan hesaplama/gösterme) |
| Çrş 9 Eyl   | Yardımcı haritasında bulanık gösterilen talepler + pin popup                                                      |
| Prş 10 Eyl  | **AI Aciliyet Skoru entegrasyonu**                                                                                |
| Cuma 11 Eyl | Mesajlaşmayı SignalR'a taşıma + **mesaj ekranında şikayet/engelle butonu UI**                                     |
| Cmt 12 Eyl  | Toparlama + bug fix                                                                                               |
| Paz 13 Eyl  | Dinlen                                                                                                            |

### Hafta 3: Onay Mekanizması + AI Bölge Analizi + Cila + Sunum (14–20 Eyl)

| Gün         | Odak                                                                                                                         |
| ----------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Pzt 14 Eyl  | **AI Bölge Öncelik Analizi**                                                                                                 |
| Sal 15 Eyl  | **İki yönlü onay akışı:** Yardımcı "üstlenmek istiyorum" der → Afetzede'ye bildirim → onay sonrası tam konum/iletişim açılır |
| Çrş 16 Eyl  | **Şikayet eşiği sonrası otomatik kısıtlama** (basit flag: X şikayet → hesap "incelemede") + responsive cila                  |
| Prş 17 Eyl  | Uçtan uca test — **güvenlik senaryoları dahil:** şikayet/engelleme akışı, konum ifşa akışı, OTP doğrulama                    |
| Cuma 18 Eyl | Deployment, README, mimari diyagramı (güvenlik katmanı dahil)                                                                |
| Cmt 19 Eyl  | Sunum hazırlığı — güvenlik önlemlerini özellikle vurgulayan bir demo akışı hazırla                                           |
| Paz 20 Eyl  | Son prova, teslim                                                                                                            |

---

## 🎤 Sunumda Vurgulaman Gereken Noktalar

1. **Problem tanımı** — 6 Şubat depreminden somut örnekle aç
2. **İsim hikayesi** — "Derman"ın anlamı
3. **Güvenlik yaklaşımın** — "Bu tür platformların en büyük riski kötüye kullanım; bu yüzden konum gizliliği ve onay mekanizmasını tasarımın merkezine koydum" demek, sunumda en çok not alınacak kısımlardan biri olur
4. **AI'ın rolü** — karar vermiyor, öneri sunuyor
5. **Kapsamı bilinçli sınırladın** — native mobil app yerine responsive web, SMS OTP yerine e-posta OTP gibi kararları neden aldığını açıkla
6. **Gelecek adımları** — itibar sistemi, STK doğrulama, moderatör paneli

---

## ⚠️ Zaman Daralırsa Kesme Sırası (Güncellenmiş Öncelik)

Güvenlik önlemleri artık omurganın bir parçası — bu yüzden öncelik sırası değişti:

1. **Statik kurum verisini genişletme** — 5-10 örnek yeterli
2. **AI Bölge Öncelik Analizi'nin sofistike olması** — basit sıralama + tek cümle gerekçe yeterli
3. **Otomatik "incelemede" flag'i sonrası e-posta bildirimi** — manuel kontrol yeterli olabilir
4. **SignalR gerçek zamanlılığı** — sayfa yenileme (polling) fallback olarak kullanılabilir

**Asla feda etme:** Rol bazlı kayıt + OTP doğrulama, konum bulanıklaştırma, iki yönlü onay mekanizması, şikayet/engelleme sistemi, AI aciliyet skoru. Bunlar hem projenin güvenlik omurgası hem de sunumda en çok değer katacak kısımlar.

---

Başarılar! Report/Block entity tasarımında, konum bulanıklaştırma algoritmasında (örn. rastgele ofset ekleme) veya OTP akışında takılırsan bana dönebilirsin.
```
