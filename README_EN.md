# 🦭 SadSeal's Mega Mod (Combat, Skills, Recipes, Genes, Smart Haul, Work Manager & Zero Durability Suite)

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
  <b>An all-in-one in-game management suite combining 7 integrated core modules (Combat, Skills, Recipes, Gene Synthesis, Smart Haul, Work &amp; Schedule Manager, and Zero-Tick Durability) to revolutionize your RimWorld experience under a single optimized package.</b>
</p>

---

## 🌟 Module Features

### 1. 🏹 Lethal &amp; Accurate Ranged Combat
* **Smooth Ballistic Accuracy Formula:** Eliminates vanilla's arbitrary distance roll penalties, making skilled marksmen lethal at realistic engagement ranges.
* **4 Distinct Ballistic Curves:**
  * **Soft Ballistic (Default):** Smooth square-root falloff curve. High realism with balanced gunplay.
  * **Hyperbolic:** Rational falloff curve providing rock-solid long-range consistency.
  * **Skill-Focused:** Preserves high shooter accuracy largely independent of distance.
  * **Vanilla:** Original vanilla algorithm with configurable distance falloff multipliers.
* **Scalable Multipliers:** Independent sliders for raw bullet damage (1.00x - 2.50x), armor penetration (1.00x - 2.00x), and projectile flight speed (1.00x - 2.50x).
* **Kinetic Stagger (Stopping Power):** Enables bullets to reliably stagger incoming chargers regardless of target body size (Centipedes, Thrumbos, and melee rushers are staggered).
* **1-HP Anti-Preservation:** Disables vanilla's forced 1-HP limb/torso survival mechanics, allowing realistic fatal headshots and devastating organ strikes.
* **Preset Profiles:** `Vanilla+`, `Tactical Lethal`, `Harsh Realism`, and `Custom Profile`.

---

### 2. 🎓 Skill Learning Multiplier &amp; Decay Control
* **Comprehensive XP Multipliers:** Scalable from 0.0x (disable learning) up to 100.0x (ultra-fast mastery).
* **Global &amp; Per-Skill Independent Control:** Use a single global multiplier or customize each vanilla and modded skill (Shooting, Melee, Crafting, Medicine, etc.) independently.
* **Remove Daily XP Soft-Cap:** Bypasses RimWorld's 80% learning rate penalty applied after 4,000 XP per day.
* **Prevent Skill Decay:** Disables experience point loss over time for skills above level 10.
* **Colonist-Exclusive Protection:** Ensures XP multipliers only apply to player-owned colonists, preventing raiders and enemies from gaining unfair advantages.
* **Mute Masterwork &amp; Legendary Alerts:** Suppresses intrusive letter popups and sound effects for high-frequency Masterwork and Legendary crafted items.
* **Quick Preset Buttons:** `1x`, `2x`, `5x`, `10x`, `25x`, `50x`, `100x`, and `Reset to Default`.

---

### 3. 🛠️ Recipe &amp; Crafting Tweaker
* **Live In-Game Recipe Editor:** Search, filter, and tweak all vanilla and modded recipes (`RecipeDef`) on the fly.
* **Work Amount Synchronization:** Synchronizes recipe `workAmount` with product `WorkToMake` stat in real-time.
* **Custom Ingredient Picker:** Swap, add, or alter ingredient requirements using materials from any loaded mod.
* **Disable Recipes:** Disable unwanted or unbalanced recipes with a single toggle.
* **1-Click XML Patch Exporter:** Export custom recipe tweaks into standard RimWorld `PatchOperation` XML files (`Config/SadSealMegaMod/GeneratedPatches.xml`) for easy sharing.
* **Reset to Vanilla:** Safely revert individual recipes or all tweaks to factory defaults with confirmation dialogs.

---

### 4. 🧬 Gene Synthesizer
* **Direct Synthesis at Gene Assembler:** Craft any vanilla or modded gene directly as a pure `Genepack` via the "Synthesize Gene" command.
* **Craft with Neutroamine:** Synthesize genetic material in exchange for Neutroamine (configurable cost in Mod Settings, default: 100).
* **Universal Mod Compatibility:** Dynamically queries RimWorld's `DefDatabase<GeneDef>`; automatically supports Alpha Genes, Biotech Expansion, and all third-party gene mods.
* **Rich Filtering &amp; Search:** Real-time text search, mod-based filtering, category filters (biochemical, cosmetic, archite, etc.), and complexity/metabolism/archite stats preview.
* **Sequential Colony Pipeline:** Haulers deliver Neutroamine to the assembler, and researchers perform the synthesis. Canceling safely drops all invested materials on the floor.
* **Quick Presets:** `Free` (0 cost / instant), `Cheap` (25 Neutroamine), `Standard` (100 Neutroamine), `Hardcore` (250 Neutroamine), and `Reset`.

---

### 5. 📦 Smart Haul (High-Performance Multi-Item Hauling)
* **High-Efficiency Multi-Hauling:** Colonists collect nearby compatible items until their inventory weight limit is reached, instead of making trips for single items.
* **Single-Job Batch Unloading:** Eliminates ThinkTree restarts and TPS hitches by unloading both carried and inventory-stored items in a continuous single job.
* **Affinity Cache:** Rapidly validates target stockpile compatibility for candidate items, bypassing ~95% of map-wide storage scans.
* **Personal Inventory Safety:** Only designated hauling cargo is tagged; colonist weapons, apparel, meals, and emergency medicines are strictly preserved.
* **Draft Ejection:** Drafting a colonist instantly drops gathered hauling cargo on the spot so they are immediately combat-ready.
* **Quick Presets:** `Balanced`, `Performance (Ultra TPS)`, `Maximum Capacity`, and `Reset to Default`.

---

### 6. 💼 Work &amp; Schedule Manager (Work Manager 1.6 &amp; 1.7)
* **Original Mod Attribution:** Modern community adaptation of [LordKuper](https://steamcommunity.com/sharedfiles/filedetails/?id=2029596262)'s acclaimed **Work Manager** mod for RimWorld 1.6 and 1.7 (.NET 10).
* **Modern Performance Architecture:** Prioritization passes utilize pre-allocated static buffer arrays, and schedule evaluation routines hook into hourly ticks (2,500 ticks) for high-TPS large colonies.
* **Dynamic Suitability Scoring:** Combines colonist skill levels, passions, health capacities (sight, manipulation, moving), traits, and ideology roles to determine the optimal worker for each task.
* **Specialist &amp; Emergency Work Delegation:** Emergency duties like Doctor, Firefight, and Warden receive instant priority; specialized roles like Cook, Research, and Smith are capped to the most competent workers per map.
* **Idle Worker Redirection:** Automatically flexes priorities of idle specialists toward secondary tasks such as hauling and cleaning when primary tasks are exhausted.
* **Automated Schedule Management:** Automatically schedules Night Owl colonists to night shifts, while optimizing day, sleep, and recreation blocks for the rest of the colony.
* **Direct Work Tab Integration:** Custom automation toggle columns on the Work and Schedule tabs allow per-colonist automation toggles, alongside header buttons for instant manual evaluation.
* **Quick Presets:** `Balanced`, `Performance (Ultra TPS)`, `Strict Specialization`, and `Reset to Default`.

---

### 7. 🛡️ Zero-Tick Durability (Weapons &amp; Apparel)
* **Zero-Tick Architecture:** Manipulates RimWorld's core engine `ThingDef.useHitPoints = false` flags directly rather than executing costly per-tick comp hooks. 100% TPS neutral.
* **Weapon Wearout Prevention:** All ranged and melee weapons never degrade, take damage, or wear down during combat or use.
* **Apparel &amp; Armor Protection:** Clothing and armor never deteriorate from weather (rain, snow, water), combat damage, or passage of time.
* **Mood Penalty Prevention:** Colonists never suffer negative thoughts from worn-out or frayed clothing (`tattered/frayed apparel`).
* **Instant Map-Wide Restoration:** Mod Settings button instantly restores all damaged weapons and apparel across all active maps, caravans, and inventories to 100% condition.
* **Prevent Tainted Apparel:** Optional toggle to prevent apparel from being marked as tainted (`tainted`) when enemies perish.
* **Quick Presets:** `Disable All`, `Weapons Only`, `Apparel Only`, `Immortal Everything (Default)`, and `Reset to Default`.

---

## 🖥️ User Interface &amp; Mod Settings

Navigate to **Options ➔ Mod Settings ➔ SadSeal Mega Mod** to access all modules using a dropdown navigation menu and directional arrows (`◄ [Module Name] ►`):

| Module | Description |
|---|---|
| `[🏹 Ranged Combat]` | Ballistic formulas, damage, armor penetration, and velocity multipliers |
| `[🎓 Skill Multipliers]` | Global and per-skill XP rates, decay prevention, and soft-cap removal |
| `[🛠️ Recipe Tweaker]` | Live recipe editor, custom ingredient picker, and XML patch exporter |
| `[🧬 Gene Synthesizer]` | Neutroamine costs, synthesis duration, archite rules, and presets |
| `[📦 Smart Haul]` | Multi-haul collection radius, mass threshold, and affinity cache settings |
| `[💼 Work Manager]` | Prioritization intervals, specialist limits, schedule manager, and idle redirection |
| `[🛡️ Durability]` | Weapon/apparel durability toggles, instant repair button, and tainted prevention |

* Full native localization support for both **English** and **Turkish**.
* Most setting adjustments take effect immediately without requiring a game restart.

---

## 🧩 Standalone &amp; Ecosystem Mod Repositories

If you prefer using individual modules independently or exploring other custom mods in the ecosystem, visit the dedicated repositories:

* 🎓 [Rimworld-Skill-learning-multiplier](https://github.com/SadSeal5/Rimworld-Skill-learning-multiplier) - Skill learning multipliers and decay prevention
* 🏹 [rimworld-lethal-ranged-combat-1.6](https://github.com/SadSeal5/rimworld-lethal-ranged-combat-1.6) - Accurate and lethal ballistic combat
* 🛠️ [rimworld-recipe-tweaker](https://github.com/SadSeal5/rimworld-recipe-tweaker) - Real-time in-game crafting recipe editor and XML exporter
* 🧬 [rimworld-gene-synthesizer](https://github.com/SadSeal5/rimworld-gene-synthesizer) - Pure Genepack synthesis for Neutroamine (Biotech)
* 📦 [rimworld-smart-haul](https://github.com/SadSeal5/rimworld-smart-haul) - High-TPS multi-item hauling alternative (Pick Up And Haul)
* 💼 [rimworld-work-manager](https://github.com/SadSeal5/rimworld-work-manager) - Automated colonist work and schedule management (Work Manager 1.6 &amp; 1.7)
* 🛡️ [rimworld-no-durability](https://github.com/SadSeal5/rimworld-no-durability) - Zero-tick weapon &amp; apparel durability remover
* 🧪 [Rimworld-Unified-Research-Tree](https://github.com/SadSeal5/Rimworld-Unified-Research-Tree) - Modern research canvas with DAG auto-layout, Anomaly dual research, and auto-queuing
* 🩸 [SadRimworld-Cellular-Regeneration](https://github.com/SadSeal5/SadRimworld-Cellular-Regeneration) - 4-stage prioritized cellular healing gene suite (clotting, wound closure, limb regrowth, scar cure)

---

## ⚙️ Requirements &amp; Load Order

* **RimWorld Versions:** `1.5`, `1.6`, or `1.7` (Beta)
* **Required Dependency:** [Harmony (brrainz.harmony)](https://github.com/pardeike/HarmonyRimWorld)

```
1. Core (RimWorld)
2. Harmony
... (Other Mods)
X. SadSeal's Mega Mod
```

---

## 📁 Installation

1. Download the latest release from the [Releases](https://github.com/SadSeal5/SadRimworld-mod-pack/releases) page or clone this repository.
2. Place the folder into your RimWorld mods directory:
   ```
   RimWorld/Mods/SadSealMegaMod/
   ```
3. Launch the game, enable Harmony first, then enable **SadSeal's Mega Mod** in the Mod Manager.

---

## 🙏 Credits &amp; Attribution

* **Work Manager:** Concept, design, and UI layout by [LordKuper](https://steamcommunity.com/sharedfiles/filedetails/?id=2029596262). Work Manager in this suite is a community adaptation for modern RimWorld 1.6 &amp; 1.7 (.NET 10).
* **Smart Haul:** Inspired by the *Pick Up And Haul* concept and community modding pioneers.
* **Unified Research Tree:** Built from scratch with DAG auto-layout for modern RimWorld 1.6 &amp; 1.7 (.NET 10) and Anomaly DLC, inspired by *Research Tree* and *Research Reinvented*.
* **Developer &amp; Maintainer:** Developed and maintained by SadSeal5.
