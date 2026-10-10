# SadSeal's Mega Mod (Combat, Skills & Recipe Tweaker Suite)

**RimWorld 1.5 & 1.6 Uyumlu Birleşik Mega Mod**

SadSeal's Mega Mod, RimWorld topluluğu için geliştirilmiş 3 temel modu tek bir çatı altında birleştiren, yüksek performanslı ve tam optimize edilmiş bir süitedir:
1. **Lethal & Accurate Ranged Combat** (Ölümcül ve İsabetli Menzilli Çatışma)
2. **Skill Learning Multiplier & Decay Control** (Yetenek Öğrenme Çarpanı ve Erime Denetimi)
3. **Recipe & Crafting Tweaker** (Oyun İçi Tarif ve Üretim Düzenleyici)

---

## 🌟 Modül Özellikleri

### 1. 🏹 Ölümcül ve İsabetli Menzilli Çatışma (Lethal Combat)
- **Akıcı Balistik İsabet Formülü:** Vanilla'nın aşırı mesafeli atış cezalarını ortadan kaldırarak yetenekli atıcıları mantıklı menzillerde ölümcül kılar.
- **4 İsabet Eğrisi:** Yumuşak Balistik (Soft Ballistic), Hiperbolik (Hyperbolic), Yetenek Odaklı (Skill-Focused) ve Vanilla eğrisi.
- **Ölçeklenebilir Çarpanlar:** Mermi ham hasarı, zırh delme oranı (AP) ve mermi uçuş hızı çarpanları.
- **Kinetik Sarsma (Stopping Power):** Mermilerin hedef vücut büyüklüğü sınırına takılmaksızın hedefleri sarsıp yavaşlatmasını sağlar.
- **1-Can Koruma Sistemini Kaldırma:** Ağır darbe alan organların yapay olarak 1 Can ile kurtulmasını sağlayan vanilla mekaniğini devre dışı bırakır.
- **Hazır Şablonlar:** Vanilla+, Taktiksel Ölümcül (Tactical Lethal), Sert Gerçekçilik (Harsh Realism) ve Özel Profil.

### 2. 🎓 Yetenek Öğrenme Çarpanı (Skill Multipliers)
- **Kapsamlı XP Çarpanları:** 0.0x ile 100.0x arasında serbest ölçekleme.
- **Genel ve Tekil Yetenek Desteği:** Tek bir genel çarpan kullanabilir veya vanilla ve tüm modlu yetenekleri bağımsız olarak ayarlayabilirsiniz.
- **Günlük XP Yumuşak Tavanını Kaldırma:** 4.000 XP/gün sonrasında uygulanan %80 öğrenme hız cezasını iptal eder.
- **Yetenek Erimesini (Decay) Engelleme:** Seviye 10 ve üzeri yeteneklerin zamanla tecrübe kaybetmesini önler.
- **Hızlı Şablon Butonları:** 1x, 2x, 5x, 10x, 25x, 50x, 100x ve Varsayılana Sıfırla.

### 3. 🛠️ Tarif ve Üretim Düzenleyici (Recipe Tweaker)
- **Oyun İçi Tarif Editörü:** Sol listeden tüm modlar ve vanilla tarifleri arasında anında arama yapma ve seçme.
- **Üretim Süresi (Work Amount) Ayarlama:** Tarifin hem `workAmount` değerini hem de üretilen eşyanın `WorkToMake` statını senkronize günceller.
- **Malzeme Düzenleme:** Herhangi bir yüklü moddan eşyaları malzeme olarak ekleme, çıkarma ve miktarlarını değiştirme.
- **Tarif Kapatma:** İstenmeyen tarifleri oyunda devre dışı bırakma.
- **XML Yaması Dışa Aktarımı:** Yapılan tüm değişiklikleri tek tuşla `Config/SadSealMegaMod/GeneratedPatches.xml` dosyasına standart RimWorld XML yaması olarak dışa aktarabilme.

---

## 🖥️ Kullanıcı Arayüzü & Mod Ayarları
- Oyun içinde **Seçenekler -> Mod Ayarları -> SadSeal Mega Mod** menüsüne girdiğinizde üst tarafta **3 Sekme** yer alır:
  - `[🏹 Menzilli Çatışma]`
  - `[🎓 Yetenek Çarpanı]`
  - `[🛠️ Tarif Düzenleyici]`
- Pencereler arasında geçiş yapmak için tek bir tıklama yeterlidir.
- Türkçe ve İngilizce tam dil desteğine sahiptir.

---

## 📁 Klasör Yapısı

```
SadSealMegaMod/
├── About/
│   ├── About.xml           # Mod kimliği, bağımlılıklar ve açıklamalar
│   └── Preview.png         # Oyun içi mod menüsü afişi (800x450)
├── Assemblies/
│   └── SadSealMegaMod.dll  # Birleşik derlenmiş C# assembly kütüphanesi
├── Languages/
│   ├── English/Keyed/
│   │   └── MegaMod_Keys.xml
│   └── Turkish/Keyed/
│       └── MegaMod_Keys.xml
└── Source/
    ├── MegaMod.cs          # Mod giriş noktası ve sekmeli UI yöneticisi
    ├── MegaModSettings.cs  # Birleşik ayar serileştirme (IExposable)
    ├── SadSealMegaMod.csproj
    ├── Combat/             # Balistik, isabet ve hasar yamaları + UI
    ├── Skills/             # XP çarpanları, erime iptali + UI
    └── Recipes/            # Tarif motoru, Live Patcher, XML Exporter + UI
```

---

## ⚙️ Gereksinimler
- **RimWorld:** `1.5` veya `1.6`
- **Harmony:** `brrainz.harmony` (Mod yükleme sırasında Harmony'nin Mega Mod'dan önce yüklenmesi gerekir).
