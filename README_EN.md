# 🦭 SadSeal's Mega Mod (12-in-1 Modular Expansion Suite)

> 🇹🇷 **[Türkçe README için buraya tıklayın](README.md)**

<p align="center">
  <img src="About/Preview.png" alt="SadSeal's Mega Mod Banner" width="800">
</p>

<p align="center">
  <a href="#"><img src="https://img.shields.io/badge/RimWorld-1.5%20%7C%201.6%20%7C%201.7%20(Beta)-brightgreen.svg" alt="RimWorld Versions"></a>
  <a href="https://github.com/pardeike/HarmonyRimWorld"><img src="https://img.shields.io/badge/Requires-Harmony-007EC6.svg?logo=unity&logoColor=white" alt="Requires Harmony"></a>
  <a href="#"><img src="https://img.shields.io/badge/Language-C%23-239120.svg?logo=csharp&logoColor=white" alt="Language C#"></a>
  <a href="#"><img src="https://img.shields.io/badge/.NET-10.0-512BD4.svg?logo=dotnet&logoColor=white" alt=".NET 10.0"></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-orange.svg" alt="License MIT"></a>
</p>

<p align="center">
  <b>An all-in-one in-game management suite combining 12 independent core modules and a dynamic Module Manager to revolutionize your RimWorld experience under a single optimized package.</b>
</p>

---

## ⚙️ Module Manager

SadSeal's Mega Mod allows you to manage all 12 modules completely independently:

* **Individual Toggle Control:** Enable or disable any module with a single click.
* **Dynamic Harmony Filtering:** When the game launches, only Harmony patch classes for active modules are loaded into memory; disabled modules have their code and hooks completely bypassed.
* **Clean Architect Menu:** Buildings belonging to disabled modules (Deep Quarry, Autopsy Table, etc.) are automatically hidden from the Architect menu.
* **Restart Persistence:** Toggle changes are saved to disk and cleanly applied upon restarting RimWorld.
* **Batch Actions:** "Enable All" and "Disable All" quick action buttons.

---

## 🌟 12 Integrated Core Modules

### 1. 🏹 Lethal & Accurate Ranged Combat
* **Smooth Ballistic Accuracy:** Eliminates vanilla's arbitrary distance roll penalties, making skilled marksmen lethal at realistic engagement ranges.
* **4 Distinct Ballistic Curves:** `Soft Ballistic` (default), `Hyperbolic`, `Skill-Focused`, and configurable `Vanilla`.
* **Scalable Multipliers:** Independent sliders for raw bullet damage (1.00x - 2.50x), armor penetration (1.00x - 2.00x), and projectile flight speed (1.00x - 2.50x).
* **Kinetic Stagger (Stopping Power):** Enables bullets to reliably stagger incoming chargers regardless of target body size (Centipedes, Thrumbos, and melee rushers are staggered).
* **Remove 1-HP Preservation:** Disables vanilla's forced 1-HP limb/torso survival mechanics, allowing realistic fatal headshots and devastating organ strikes.
* **Preset Profiles:** `Vanilla+`, `Tactical Lethal`, `Harsh Realism`, and `Custom Profile`.

---

### 2. 🎓 Skill Learning Multiplier & Decay Control
* **Comprehensive XP Multipliers:** Scalable from 0.0x (disable learning) up to 100.0x (ultra-fast mastery).
* **Global & Per-Skill Independent Control:** Use a single global multiplier or customize each vanilla and modded skill independently.
* **Remove Daily XP Soft-Cap:** Bypasses RimWorld's 80% learning rate penalty applied after 4,000 XP per day.
* **Prevent Skill Decay:** Disables experience point loss over time for skills above level 10.
* **Colonist-Exclusive Protection:** Ensures XP multipliers only apply to player-owned colonists.
* **Mute Masterwork & Legendary Alerts:** Suppresses intrusive letter popups and sound effects for high-frequency Masterwork and Legendary crafted items.

---

### 3. 🛠️ Recipe & Crafting Tweaker
* **In-Game Live Recipe Editor:** Real-time search and filtering across all vanilla and modded recipes (`RecipeDef`).
* **Work Amount Synchronization:** Automatically synchronizes `workAmount` with the item's `WorkToMake` stat.
* **Custom Ingredient Selector:** Add, remove, or adjust ingredients from any loaded mod.
* **Recipe Blacklisting:** Disable unwanted or unbalanced recipes with a single toggle.
* **One-Click XML Patch Export:** Export all custom recipe modifications as a clean RimWorld `PatchOperation` XML file.

---

### 4. 🧬 Gene Synthesizer
* **Craft Any Gene at the Gene Assembler:** Order specific genes (Vanilla + all modded genes) directly from the Gene Assembler as pure `Genepack` items.
* **Neutroamine Cost:** Synthesize genetic material using Neutroamine (default 100, fully configurable).
* **Universal Mod Compatibility:** Automatically scans `DefDatabase<GeneDef>` for Alpha Genes, Biotech Expansion, and all custom gene mods.
* **Advanced Search & Filtering:** Filter by mod, category (biochemical, cosmetic, archite), and preview metabolic and complexity stats.

---

### 5. 📦 Smart Haul (Pick Up And Haul)
* **High-Performance Multi-Hauling:** Colonists carry multiple items up to their personal inventory capacity rather than hauling single items.
* **Single-Job Batch Unload:** Unloads inventory items sequentially into stockpiles within the same job workflow to conserve TPS.
* **Affinity Cache:** Caches stockpile compatibility checks to bypass 95% of heavy map-wide item searches.
* **Gear & Equipment Safety:** Only items gathered specifically for hauling are unloaded; personal food, medicine, and sidearms are safe.
* **Draft Emergency Drop:** Immediately drops carried loads when drafted for military readiness.

---

### 6. 💼 Work & Schedule Manager
* **Original Adaptation:** Community adaptation and performance overhaul of [LordKuper](https://steamcommunity.com/sharedfiles/filedetails/?id=2029596262)'s **Work Manager** for RimWorld 1.6 & 1.7 (.NET 10).
* **Dynamic Suitability Scoring:** Combines skills, passions, health capacities, traits, and ideology roles to determine optimal work assignments.
* **Emergency & Specialist Distribution:** Prioritizes critical emergency duties (Doctor, Firefight, Warden) while assigning top specialists to complex crafts (Cook, Research, Smith).
* **Smart Sleep & Schedule Optimization:** Automatically places Night Owls on night shifts and configures optimal work/rest cycles for others.
* **Work Tab Direct Integration:** Dedicated automation toggle columns and one-click refresh buttons in the vanilla Work and Schedule tabs.

---

### 7. 🛡️ Zero-Tick Weapon & Apparel Durability
* **Zero-Tick Engine Architecture:** Operates directly at the engine level (`ThingDef.useHitPoints = false`) without periodic tick overhead for maximum TPS.
* **Prevent Gear Wear:** Weapons and armor never degrade in combat or from regular usage.
* **Weather & Environmental Protection:** Items left outdoors, in rain, or in snow never rot or deteriorate.
* **Mood Penalty Removal:** Colonists never suffer negative mood debuffs from wearing tattered or frayed apparel.
* **One-Click Repair:** Instantly restore all map and caravan items to 100% condition.
* **Optional Tainted Removal:** Option to disable the tainted apparel penalty upon death.

---

### 8. ✨ Quality of Life (QoL) Suite
* **Don't Block Doors:** Prevents haulers and drop pods from leaving items in doorways, preventing stuck-open security breaches.
* **Filth Vanishes Over Time:** Outdoor blood, mud, and dirt naturally decay and vanish over time, keeping maps clean and saving memory.
* **Immortal Trees:** Prevents mature trees from dying of old age to preserve forests and timber reserves.
* **Smooth Walls Before Floors:** Prioritizes smoothing rough rock walls before smoothing floor tiles.
* **No Death Acidifier:** Prevents Imperial soldiers and pirates from dissolving their weapons and armor upon death.
* **Raid Strategy Toggles:** Selectively disable frustrating raid arrival strategies (Sappers, Breachers, Sieges).

---

### 9. 🩺 Medical & Autopsy Suite
* **Post-Mortem Autopsy:** Dissect fresh humanlike corpses to salvage natural organs (heart, kidney, lung, liver), bionics, and implants.
* **Dedicated Surgical Autopsy Table:** Heavy stainless steel dissection table with drainage conduits, lamp, and instrument tray (`SadMedical_AutopsyTable`).
* **Research Tree Prerequisite:** Requires researching **Post-Mortem Autopsy (`SadMedical_AutopsyResearch`)** before construction (`DrugProduction` prerequisite).
* **Field Medicine:** Allows doctors to tend wounded colonists directly on the battlefield using inventory medicine.
* **Smart Medicine Choice:** Intelligently allocates medicine based on injury severity to prevent wasting Glitterworld medicine on minor bruises.

---

### 10. ✋ Allow Tool & Utility Commands
* **Unforbid All:** Unlocks all forbidden items across the map in a single click with fogged/ancient danger exclusion.
* **Haul Urgently:** High-priority hauling designation seamlessly integrated with Smart Haul.
* **Select Similar:** Quickly selects all matching items on screen (e.g., all stone chunks or weapons).
* **Finish Off:** Mercy-kill downed enemies in a single swift blow without drawn-out melee brawls.

---

### 11. ⏩ Smart Speed & Time Flow
* **4x Super Speed:** Unlocks 4x game speed without requiring Developer Mode via the `4` hotkey or UI button.
* **Bypass Forced Event Slowdowns:** Eliminates the forced 1x speed lock during combat, fires, and raids.
* **Smart Sleep Fast-Forward:** Automatically accelerates time when all colonists are asleep and restores normal speed upon awakening or alerts.

---

### 12. ⛏️ Deep Quarry & Resource Extractor
* **Excavated 5x5 Ground Pit:** Deep mining pit facility for stone chunks, raw metals, and compact machinery (`Sad_DeepQuarry`).
* **Custom 2D Art:** High-detail excavated pit textures with timber framing, rock ledges, ore bucket, and hoist winch.
* **Excavation Recipes:** Quarry stone chunks, extract deep mineral veins (steel, plasteel, uranium, gold, silver), and salvage components.
* **Research Requirement:** Requires vanilla **Deep Drilling (`DeepDrilling`)** research before construction.
* **Depletion & Multipliers:** Configurable infinite quarry mode or 1,000 excavation limit with adjustable mining speed scaling.

---

## 🖥️ In-Game Settings Window

Access the configuration suite via **Options ➔ Mod Settings ➔ SadSeal's Mega Mod**:

| Tab | Description |
|---|---|
| `[⚙️ Modules]` | Toggle individual modules on/off and batch enable/disable (Requires restart) |
| `[🏹 Ranged Combat]` | Ballistics, damage, armor penetration, projectile speed, and stopping power |
| `[🎓 Skill Multipliers]` | Global and individual XP multipliers, decay toggles, and daily soft-cap bypass |
| `[🛠️ Recipe Tweaker]` | Live recipe search, ingredient editor, work amount sync, and XML patch exporter |
| `[🧬 Gene Synthesizer]` | Neutroamine costs, production work time, archite rules, and presets |
| `[📦 Smart Haul]` | Multi-item hauling, search radius, mass limits, and stockpile affinity caching |
| `[💼 Work Manager]` | Assignment intervals, specialist targets, auto-scheduling, and idle redirection |
| `[🛡️ Durability]` | Weapon and apparel durability remover, instant repair, and tainted toggles |
| `[✨ QoL Suite]` | Door blocking, filth decay, tree immortality, smooth wall priority, and raid options |
| `[🩺 Medical & Autopsy]` | Autopsy success scaling, field medicine thresholds, and mood penalty toggles |
| `[✋ Allow Tool]` | Urgent hauling priorities and finish-off commands |
| `[⏩ Time Flow]` | 4x speed toggle, forced slowdown bypass, and smart sleep acceleration |
| `[⛏️ Deep Quarry]` | Mining speed multipliers, infinite quarry toggle, and mineral drop weights |

* Full English and Turkish localization included.

---

## ⚙️ Requirements & Load Order

* **RimWorld Versions:** `1.5`, `1.6`, or `1.7` (Beta / CoreCLR .NET 10)
* **Required Dependency:** [Harmony (brrainz.harmony)](https://github.com/pardeike/HarmonyRimWorld)

```
1. Core (RimWorld)
2. Harmony
... (Other Mods)
X. SadSeal's Mega Mod
```

---

## 📁 Installation

1. Download `SadSealMegaMod-v1.5.0.zip` from the [Releases](https://github.com/SadSeal5/SadRimworld-mod-pack/releases) page.
2. Extract the archive into your RimWorld mods directory:
   ```
   RimWorld/Mods/SadSealMegaMod/
   ```
3. Launch RimWorld, navigate to the Mod Manager, place SadSeal's Mega Mod below Harmony, and activate it.

---

## 🙏 Credits & Attribution

* **Work Manager:** Concept and original design by [LordKuper](https://steamcommunity.com/sharedfiles/filedetails/?id=2029596262).
* **Smart Haul:** Concept inspired by *Pick Up And Haul*.
* **Developed & Maintained by:** SadSeal5.
