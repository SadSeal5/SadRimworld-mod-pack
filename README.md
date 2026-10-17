# 🦭 SadSeal's Mega Mod (Combat, Skills & Recipe Tweaker Suite)

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
  <b>RimWorld deneyiminizi tepeden tırnağa geliştiren 3 büyük modu tek bir optimize edilmiş süite altında birleştiren hepsi bir arada oyun içi yönetim paketi.</b>
</p>

---

## 🌟 Modül Özellikleri

### 1. 🏹 Ölümcül ve İsabetli Menzilli Çatışma (Lethal Combat)
* **Akıcı Balistik İsabet Formülü:** Vanilla'nın orantısız mesafe cezalarını kaldırarak yetenekli atıcıları mantıklı menzillerde ölümcül kılar.
* **4 Farklı Balistik Eğri:**
  * **Soft Ballistic (Varsayılan):** Pürüzsüz karekök düşüş eğrisi. Yüksek gerçekçilik ve dengeli çatışma.
  * **Hyperbolic:** Rasyonel düşüş eğrisi ile uzun menzilli kararlı isabet.
  * **Skill-Focused:** Yetenekli nişancıların mesafeden bağımsız yüksek isabetini korur.
  * **Vanilla:** Ayarlanabilir mesafe çarpanlı orijinal vanilla formülü.
* **Ölçeklenebilir Çarpanlar:** Mermi ham hasarı (1.00x - 2.50x), zırh delme oranı (1.00x - 2.00x) ve mermi uçuş hızı (1.00x - 2.50x) çarpanları.
* **Kinetik Sarsma (Stopping Power):** Mermilerin isabet ettiği hedefleri vücut boyutu sınırına takılmaksızın yavaşlatmasını sağlar (Centipede, Thrumbo ve yakın dövüş saldırganları sarsılır).
* **1-Can Koruma Sistemini Kaldırma:** Ağır isabet alan organların ve uzuvların yapay olarak 1 HP ile hayatta kalmasını sağlayan vanilla mekaniğini devre dışı bırakır.
* **Hazır Şablonlar:** `Vanilla+`, `Taktiksel Ölümcül`, `Sert Gerçekçilik` ve `Özel Profil`.

---

### 2. 🎓 Yetenek Öğrenme Çarpanı (Skill Multipliers)
* **Kapsamlı XP Çarpanları:** 0.0x (öğrenmeyi durdurma) ile 100.0x (ultra hızlı uzmanlık) arasında serbest ölçekleme.
* **Genel ve Bağımsız Yetenek Denetimi:** İster tek bir genel çarpan kullanın, ister Shooting, Melee, Crafting, Medicine gibi tüm vanilla ve modlu yetenekleri tek tek ayarlayın.
* **Günlük XP Yumuşak Tavanını Kaldırma:** RimWorld'ün günde 4.000 XP sonrasında uyguladığı %80 öğrenme hız cezasını tamamen iptal eder.
* **Yetenek Erimesini (Decay) Engelleme:** Seviye 10 üzerindeki yeteneklerin zamanla tecrübe puanı kaybetmesini önler.
* **Koloni Üyelerine Özel Koruma:** Çarpanların yalnızca oyuncu kolonistlerine uygulanmasını sağlayarak akıncıların ve düşmanların haksız avantaj kazanmasını engeller.
* **Usta & Efsanevi Bildirimlerini Susturma:** Yüksek çarpanlarda sık üretilen Masterwork/Legendary eşyaların ses ve mektup bildirim kirliliğini engeller.
* **Hızlı Şablon Butonları:** `1x`, `2x`, `5x`, `10x`, `25x`, `50x`, `100x` ve `Varsayılana Sıfırla`.

---

### 3. 🛠️ Tarif ve Üretim Düzenleyici (Recipe Tweaker)
* **Oyun İçi Canlı Tarif Editörü:** Tüm vanilla ve modlu tarifler (`RecipeDef`) arasında anında arama yapma ve filtreleme.
* **Üretim Süresi (İş Miktarı) Senkronizasyonu:** Tarifin `workAmount` değeri ile üretilen eşyanın `WorkToMake` statını eşzamanlı günceller.
* **Özel Malzeme Seçici:** Yüklü herhangi bir moddan malzemeleri tarife ekleme, çıkarma veya gerekli miktarları değiştirme.
* **Tarif Kapatma:** İstenmeyen veya dengesiz tarifleri tek tıklamayla oyunda devre dışı bırakma.
* **Tek Tıkla XML Yaması Dışa Aktarımı:** Yapılan tüm özel tarif düzenlemelerini standart RimWorld `PatchOperation` XML dosyası olarak (`Config/SadSealMegaMod/GeneratedPatches.xml`) dışa aktarabilme.
* **Orijinale Dönme:** Tekil veya tüm tarifleri onay pencereleri ile güvenle vanilla haline döndürme.

---

## 🖥️ Kullanıcı Arayüzü & Mod Ayarları

Oyun içinde **Seçenekler ➔ Mod Ayarları ➔ SadSeal Mega Mod** menüsüne girdiğinizde üst tarafta 3 ana sekme yer alır:

| Sekme | Açıklama |
|---|---|
| `[🏹 Menzilli Çatışma]` | Balistik modeller, hasar, zırh delme ve mermi hız ayarları |
| `[🎓 Yetenek Çarpanı]` | Genel ve tekil XP katsayıları, erime ve yumuşak tavan denetimi |
| `[🛠️ Tarif Düzenleyici]` | Oyun içi canlı tarif editörü, malzeme seçici ve XML dışa aktarıcı |

* Türkçe ve İngilizce tam dil desteğine sahiptir.
* Değişikliklerin çoğu oyun yeniden başlatılmadan anında yürürlüğe girer.

---

## 🧩 Bağımsız (Atomize Edilmiş) Mod Depoları

Mega mod süiti içerisindeki modülleri bağımsız olarak kullanmak isterseniz aşağıdaki ayrı depolardan da edinebilirsiniz:

* 🎓 [Rimworld-Skill-learning-multiplier](https://github.com/SadSeal5/Rimworld-Skill-learning-multiplier)
* 🏹 [rimworld-lethal-ranged-combat-1.6](https://github.com/SadSeal5/rimworld-lethal-ranged-combat-1.6)
* 🛠️ [rimworld-recipe-tweaker](https://github.com/SadSeal5/rimworld-recipe-tweaker)

---

## ⚙️ Gereksinimler & Yükleme Sırası

* **RimWorld Sürümleri:** `1.5`, `1.6` veya `1.7` (Beta)
* **Gerekli Mod:** [Harmony (brrainz.harmony)](https://github.com/pardeike/HarmonyRimWorld)

```
1. Core (RimWorld)
2. Harmony
... (Diğer Modlar)
X. SadSeal's Mega Mod
```

---

## 📁 Kurulum

1. Deponun en son sürümünü [Releases](https://github.com/SadSeal5/SadRimworld-mod-pack/releases) sayfasından indirin veya depoyu klonlayın.
2. Klasörü RimWorld mod dizininize yerleştirin:
   ```
   RimWorld/Mods/SadSealMegaMod/
   ```
3. Oyunu başlatın, Mod Yöneticisi'nde Harmony'nin altına SadSeal's Mega Mod'u ekleyin ve etkinleştirin.
