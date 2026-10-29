# 🦭 SadSeal's Mega Mod (12-in-1 Modular Expansion Suite)

> 🇬🇧 **[Click here for the English README](README_EN.md)**

<p align="center">
  <img src="About/Preview.png" alt="SadSeal's Mega Mod Banner" width="800">
</p>

<p align="center">
  <a href="#"><img src="https://img.shields.io/badge/RimWorld-1.5%20%7C%201.6%20%7C%201.7%20(Beta)-brightgreen.svg" alt="RimWorld Versions"></a>
  <a href="https://github.com/pardeike/HarmonyRimWorld"><img src="https://img.shields.io/badge/Requires-Harmony-007EC6.svg?logo=unity&logoColor=white" alt="Requires Harmony"></a>
  <a href="#"><img src="https://img.shields.io/badge/Language-C%23-239120.svg?logo=csharp&logoColor=white" alt="Language C#"></a>
  <a href="#"><img src="https://img.shields.io/badge/.NET-10.0-512BD4.svg?logo=dotnet&logoColor=white" alt=".NET 10.0"></a>
  <a href="#"><img src="https://img.shields.io/badge/License-MIT-orange.svg" alt="License MIT"></a>
</p>

<p align="center">
  <b>RimWorld deneyiminizi tepeden tırnağa geliştiren 12 bağımsız ana modülü ve dinamik Modül Yöneticisini tek bir optimize edilmiş süite altında birleştiren hepsi bir arada oyun içi yönetim paketi.</b>
</p>

---

## ⚙️ Modül Yöneticisi (Module Manager)

SadSeal's Mega Mod, mod paketindeki 12 modülü tamamen bağımsız olarak yönetmenize olanak tanır:

* **Bireysel Açma/Kapama:** Dilediğiniz modülü tek tıkla devre dışı bırakabilir veya etkinleştirebilirsiniz.
* **Dinamik Harmony Filtreleme:** Oyun başlatılırken yalnızca etkin modüllerin Harmony yama sınıfları belleğe yüklenir; kapalı modüllerin kodları ve kancaları tamamen atlanır.
* **Akıllı Mimari Temizliği:** Kapatılan modüllere ait yapılar (Derin Taş Ocağı, Otopsi Masası vb.) mimari inşaat menüsünden tamamen gizlenir.
* **Yeniden Başlatma Bildirimi:** Modül ayarlarını değiştirdiğinizde, değişikliklerin oyuna temiz bir şekilde uygulanması için RimWorld'ü yeniden başlatmanız yeterlidir.
* **Toplu Eylemler:** "Tümünü Etkinleştir" ve "Tümünü Devre Dışı Bırak" hızlı butonları.

---

## 🌟 Entegre 12 Ana Modül

### 1. 🏹 Ölümcül ve İsabetli Menzilli Çatışma (Lethal Combat)
* **Akıcı Balistik İsabet Formülü:** Vanilla'nın yapay mesafe cezalarını kaldırarak yetenekli atıcıları gerçekçi menzillerde ölümcül kılar.
* **4 Farklı Balistik Eğri:** `Soft Ballistic` (varsayılan), `Hyperbolic`, `Skill-Focused` ve ayarlanabilir `Vanilla`.
* **Ölçeklenebilir Çarpanlar:** Mermi ham hasarı (1.00x - 2.50x), zırh delme oranı (1.00x - 2.00x) ve mermi uçuş hızı (1.00x - 2.50x).
* **Kinetik Sarsma (Stopping Power):** Mermilerin isabet ettiği hedefleri vücut boyutu sınırına takılmaksızın yavaşlatmasını sağlar (Centipede, Thrumbo ve yakın dövüş saldırganları sarsılır).
* **1-Can Koruma Sistemini Kaldırma:** Ağır isabet alan organların yapay olarak 1 HP ile hayatta kalmasını sağlayan vanilla mekaniğini devre dışı bırakır.
* **Hazır Şablonlar:** `Vanilla+`, `Taktiksel Ölümcül`, `Sert Gerçekçilik` ve `Özel Profil`.

---

### 2. 🎓 Yetenek Öğrenme Çarpanı (Skill Multipliers)
* **Kapsamlı XP Çarpanları:** 0.0x (öğrenmeyi durdurma) ile 100.0x (ultra hızlı uzmanlık) arasında serbest ölçekleme.
* **Genel ve Bağımsız Denetim:** İster tek bir genel çarpan kullanın, ister tüm vanilla ve modlu yetenekleri tek tek ayarlayın.
* **Günlük XP Yumuşak Tavanını Kaldırma:** RimWorld'ün günde 4.000 XP sonrasında uyguladığı %80 öğrenme hız cezasını tamamen iptal eder.
* **Yetenek Erimesini (Decay) Engelleme:** Seviye 10 üzerindeki yeteneklerin zamanla tecrübe puanı kaybetmesini önler.
* **Koloni Üyelerine Özel Koruma:** Çarpanların yalnızca oyuncu kolonistlerine uygulanmasını sağlar.
* **Usta & Efsanevi Bildirimlerini Susturma:** Yüksek çarpanlarda sık üretilen Masterwork/Legendary eşyaların bildirim kirliliğini engeller.

---

### 3. 🛠️ Tarif ve Üretim Düzenleyici (Recipe Tweaker)
* **Oyun İçi Canlı Tarif Editörü:** Tüm vanilla ve modlu tarifler (`RecipeDef`) arasında anında arama yapma ve filtreleme.
* **Üretim Süresi Senkronizasyonu:** Tarifin `workAmount` değeri ile üretilen eşyanın `WorkToMake` statını eşzamanlı günceller.
* **Özel Malzeme Seçici:** Yüklü herhangi bir moddan malzemeleri tarife ekleme, çıkarma veya gerekli miktarları değiştirme.
* **Tarif Kapatma:** İstenmeyen veya dengesiz tarifleri tek tıklamayla oyunda devre dışı bırakma.
* **Tek Tıkla XML Yaması Dışa Aktarımı:** Yapılan tüm özel tarif düzenlemelerini standart RimWorld `PatchOperation` XML dosyası olarak dışa aktarabilme.

---

### 4. 🧬 Gen Sentezleyici (Gene Synthesizer)
* **Gene Assembler Üzerinden Doğrudan Sentez:** Gene Assembler binasında "Gen Sentezle" komutu ile dilediğiniz geni seçip saf bir `Genepack` olarak üretin.
* **Neutroamine ile Üretim:** Varsayılan 100 Neutroamine karşılığında genetik sentezleme (mod ayarlarından serbestçe değiştirilebilir).
* **Bütün Modlu Genlerle Tam Uyum:** Alpha Genes, Biotech Expansion ve diğer tüm gen modlarını otomatik olarak listeler.
* **Gelişmiş Arama ve Filtreleme:** Arayüzde anlık arama, mod bazında filtreleme, kategori (biyokimyasal, kozmetik, archite vb.) filtreleme ve istatistik önizlemesi.

---

### 5. 📦 Akıllı Taşıma (Smart Haul - Pick Up And Haul)
* **Yüksek Performanslı Çoklu Taşıma:** Kolonistler tek bir eşya taşımak yerine envanter kapasiteleri dolana kadar yakındaki eşyaları da toplar.
* **Kesintisiz Toplu Boşaltma:** Eldeki ve çantadaki tüm eşyaları aynı iş akışı içinde sırayla depoya yerleştirir.
* **Stok Alanı Kümeleme (Affinity Cache):** Yakındaki aday eşyaların hedef depoya kabul durumunu anında kontrol ederek harita çapı ağır stok taramalarını %95 oranında baypas eder.
* **Kişisel Eşya Güvenliği:** Yalnızca taşıma amacıyla toplanan yükler etiketlenir; kolonistin yemeği, ilacı ve teçhizatı asla depoya boşaltılmaz.
* **Askere Alınma (Draft) Koruması:** Kolonist savaşa çağrıldığında taşınan yükler anında yere bırakılır.

---

### 6. 💼 Akıllı İş & Zaman Yöneticisi (Work & Schedule Manager)
* **Orijinal Mod Uyarlaması:** [LordKuper](https://steamcommunity.com/sharedfiles/filedetails/?id=2029596262)'ın sevilen **Work Manager** modunun RimWorld 1.6 ve 1.7 (.NET 10) için optimize edilmiş topluluk uyarlamasıdır.
* **Dinamik Uygunluk Puanlaması:** Yetenekler, tutkular, sağlık durumları, özellikler ve rolleri birleştirerek her iş türü için en yetkin kişileri belirler.
* **Uzman & Acil Durum İş Dağıtımı:** Doktorluk, Yangın ve Gardiyanlık gibi kritik görevleri acil önceliklendirir; Aşçılık, Araştırma ve Zanaatkarlık alanlarına harita başına en yetkin uzmanları adar.
* **Akıllı Çalışma & Uyku Takvimi:** Gece Kuşu kolonistleri otomatik gece vardiyasına, diğerlerini en verimli gündüz/uyku/eğlence döngüsüne alır.
* **İş Sekmesi Entegrasyonu:** Koloni İş (Work) ve Takvim (Schedule) pencerelerine eklenen otomasyon sütunları ve tek tıkla yenileme kontrolleri.

---

### 7. 🛡️ Sıfır-Tick Silah ve Kıyafet Dayanıklılık Kaldırıcı (Zero-Tick Durability)
* **Sıfır-Tick Motor Mimarisi:** Periyodik tick veya döngüsel kod çalıştırmaksızın doğrudan RimWorld motor düzeyinde `ThingDef.useHitPoints = false` bayrağını yönetir. %100 TPS dostudur.
* **Silah ve Teçhizat Aşınmasını Önleme:** Tüm menzilli ve yakın dövüş silahları çatışmada asla hasar almaz ve aşınmaz.
* **Kıyafet ve Zırh Bozulmasını Engelleme:** Giysiler dış ortam koşullarında (yağmur, kar, su), çatışmada veya zamanla hasar almaz.
* **Ruh Hali Koruması:** Kolonistler aşınmış veya parçalanmış giysi nedeniyle negatif ruh hali cezası almaz.
* **Tek Tıkla Tüm Eşyaları Onarma:** Haritadaki ve kervanlardaki tüm yıpranmış eşyaları anında %100 kondisyona getirme seçeneği.
* **Ceset Kıyafeti (Tainted) Engelleme:** İsteğe bağlı olarak ceset giysisi etiketini iptal etme.

---

### 8. ✨ Yaşam Kalitesi Paketi (QoL Suite)
* **Kapı Önü Engel Koruması:** Kolonistlerin ve taşıyıcıların kapı açıklıklarına eşya bırakmasını engelleyerek kapıların açık kilitlenmesini ve savunma açıklarını önler.
* **Zamanla Kaybolan Zemin Kirleri:** Açık arazideki kan, çamur ve pisliklerin belirli aralıklarla doğada kendiliğinden yok olmasını sağlar (harita temizliği ve TPS tasarrufu).
* **Ölümsüz Ağaçlar:** Yaşlılık süresi dolan ağaçların ölmesini engelleyerek orman örtüsünü ve odun kaynaklarını korur.
* **Zemin Öncesi Duvar Düzeltme:** İnşaatçıların zeminleri cilalamadan önce pürüzlü kaya duvarlarını düzeltmesine öncelik verir.
* **Ölüm Asitleştirici İptali:** İmparatorluk askerleri veya korsanlar öldüğünde teçhizatlarının erimesini önleyerek ganimeti korur.
* **Akıncı Taktik Filtreleri:** Sapper, Breach ve Kuşatma gibi can sıkıcı akıncı stratejilerini isteğe bağlı devre dışı bırakabilme.

---

### 9. 🩺 Medikal & Kadavra Otopsisi (Medical & Autopsy Suite)
* **Kadavra Otopsisi (Post-Mortem Autopsy):** Taze insanımsı kadavralardan doğal organları (kalp, böbrek, akciğer, karaciğer vb.), biyonik protezleri ve implantları kurtarma imkanı.
* **Özel Cerrahi Otopsi Masası:** Paslanmaz çelikten üretilen, sıvı tahliye kanalları, cerrahi lamba ve alet tepsisine sahip özel ameliyat masası (`SadMedical_AutopsyTable`).
* **Özel Araştırma Ağacı:** Masayı inşa edebilmek için sıfırdan tasarlanan **Kadavra Otopsisi (`SadMedical_AutopsyResearch`)** araştırması şarttır (`DrugProduction` önkoşullu).
* **Sahada Tedavi (Field Medicine):** Doktorların yaralı kolonistleri acil durumlarda hemen bulundukları yerde veya hastanın kendi çantasındaki ilaçlarla tedavi edebilmesini sağlar.
* **Akıllı İlaç Seçimi:** Basit sıyrıklar için Glitterworld veya Endüstriyel ilaçların israf edilmesini önler, hayati tehlikeye göre en uygun ilacı seçer.

---

### 10. ✋ Allow Tool ve Harita Komutları (Allow Tool & Utility Commands)
* **Tüm Yasakları Kaldır (Unforbid All):** Haritadaki tüm kilitli eşyaları tek tıkla açar; sisli/keşfedilmemiş antik tehlike alanlarını güvenle filtreler.
* **Acil Taşıma (Haul Urgently):** Seçilen eşyaları en yüksek öncelikle taşınmak üzere etiketler. Smart Haul ile kusursuz entegre çalışır.
* **Benzerini Seç (Select Similar):** Harita ekranında seçilen bir eşyanın (örneğin tüm kaya parçaları veya mermiler) ekrandaki tüm benzerlerini tek tıkla seçer.
* **İnfaz Et (Finish Off):** Can çekişen, yere serilmiş düşmanları acı çekmeden tek vuruşta infaz etme emri verir.

---

### 11. ⏩ Zaman ve 4x Hız Akışı (Smart Speed & Time Flow)
* **Geliştirici Modsuz 4x Hız:** Geliştirici modu (Dev Mode) açmaya gerek kalmadan klavyedeki `4` tuşu veya arayüz butonuyla 4x süper hıza geçiş.
* **Zorunlu Yavaşlamayı Kaldırma:** Çatışmalarda, yangınlarda veya akınlarda oyunun hızı zorla 1x'e düşürmesini engelleyerek oyuncunun hız kontrolünü korur.
* **Akıllı Koloni Uykusu:** Haritadaki tüm kolonistler uykuya daldığında hızı otomatik artırır; bir tehlike çıktığında veya birisi uyandığında normal hıza döner.

---

### 12. ⛏️ Derin Taş Ocağı (Deep Quarry & Resource Extractor)
* **Özel 5x5 Zemin Hafriyat Ocağı:** Koloninizin maden ve taş ihtiyacını karşılayan zemin kazı tesisi (`Sad_DeepQuarry`).
* **Özel 2D Sanat ve Doku:** Ahşap istinat kirişleri, basamaklı derin kaya katmanları, metal kova ve makara vinç mekanizması içeren özel spritelar.
* **Üretim Tarifleri:** Taş parçaları çıkarma, derin maden damarlarından çelik/plasteel/uranyum/altın/gümüş kazma ve kompakt makine bileşenleri kurtarma.
* **Araştırma Şartı:** İnşa edilebilmesi için vanilla **Derin Sondaj (`DeepDrilling`)** araştırması gereklidir.
* **Tükenme ve Çarpan Ayarları:** Sınırsız madencilik seçeneği veya 1.000 kazı sınırı ile ayarlanabilir madencilik hızı çarpanları.

---

## 🖥️ Kullanıcı Arayüzü & Mod Ayarları

Oyun içinde **Seçenekler ➔ Mod Ayarları ➔ SadSeal's Mega Mod** menüsüne girdiğinizde üst tarafta açılır menü (dropdown) ve ok butonları (`◄ [Modül Adı] ►`) ile tüm sekmeler arasında akıcı geçiş yapabilirsiniz:

| Sekme | Açıklama |
|---|---|
| `[⚙️ Modüller]` | 12 modülü tek tek açıp kapatma ve toplu eylem kontrolleri (Yeniden başlatma gerektirir) |
| `[🏹 Menzilli Çatışma]` | Balistik modeller, hasar, zırh delme ve mermi hız ayarları |
| `[🎓 Yetenek Çarpanı]` | Genel ve tekil XP katsayıları, erime ve yumuşak tavan denetimi |
| `[🛠️ Tarif Düzenleyici]` | Oyun içi canlı tarif editörü, malzeme seçici ve XML dışa aktarıcı |
| `[🧬 Gen Sentezleyici]` | Neutroamine maliyeti, üretim süresi, archite kuralları ve hazır şablonlar |
| `[📦 Akıllı Taşıma]` | Çoklu eşya toplama, arama yarıçapı, ağırlık eşiği ve stok kümeleme ayarları |
| `[💼 İş Yöneticisi]` | Öncelik hesaplama sıklığı, uzman hedefleri, takvim yönetimi ve boşta kalma kontrolleri |
| `[🛡️ Dayanıklılık]` | Silah ve kıyafet dayanıklılık iptali, tek tıkla onarma ve tainted kıyafet engelleme |
| `[✨ QoL Paketi]` | Kapı engelleme, çürüyen kirler, ölümsüz ağaçlar, duvar önceliği ve akıncı ayarları |
| `[🩺 Medikal & Otopsi]` | Otopsi başarı çarpanları, sahada tedavi eşikleri ve ruh hali cezası ayarları |
| `[✋ Allow Tool]` | Acil taşıma öncelikleri ve infaz emri kontrolleri |
| `[⏩ Zaman Akışı]` | 4x hız kilidi, zorunlu yavaşlama iptali ve akıllı uyku hızlandırma |
| `[⛏️ Taş Ocağı]` | Kazı hızı çarpanı, sınırsız ocak modu ve maden ağırlık dağılımları |

* Türkçe ve İngilizce tam dil desteğine sahiptir.

---

## ⚙️ Gereksinimler & Yükleme Sırası

* **RimWorld Sürümleri:** `1.5`, `1.6` veya `1.7` (Beta / CoreCLR .NET 10)
* **Gerekli Mod:** [Harmony (brrainz.harmony)](https://github.com/pardeike/HarmonyRimWorld)

```
1. Core (RimWorld)
2. Harmony
... (Diğer Modlar)
X. SadSeal's Mega Mod
```

---

## 📁 Kurulum

1. Deponun en son sürümünü [Releases](https://github.com/SadSeal5/SadRimworld-mod-pack/releases) sayfasından `SadSealMegaMod-v1.5.0.zip` olarak indirin.
2. Arşiv içeriğini RimWorld mod dizininize çıkartın:
   ```
   RimWorld/Mods/SadSealMegaMod/
   ```
3. Oyunu başlatın, Mod Yöneticisi'nde Harmony'nin altına SadSeal's Mega Mod'u ekleyin ve etkinleştirin.

---

## 🙏 Teşekkürler & Krediler (Credits & Attribution)

* **Work Manager:** Fikir, tasarım ve arayüz konsepti için [LordKuper](https://steamcommunity.com/sharedfiles/filedetails/?id=2029596262)'a teşekkür ederiz.
* **Smart Haul:** *Pick Up And Haul* konsepti ve orijinal mod fikirleri için topluluk geliştiricilerine teşekkürler.
* **Geliştirici & Paket:** SadSeal5 tarafından geliştirilip sürdürülmektedir.
