using SadSeal.Core.Modules;
using SadSeal.MegaMod.Skills.UI;
using UnityEngine;

namespace SadSeal.MegaMod.Skills
{
    public class SkillsModule : ModModule<SkillSettings>
    {
        public static SkillsModule Instance { get; private set; }

        public override string Id => "Skills";
        public override string DisplayNameKey => "MegaMod_TabSkills";
        public override string DescriptionKey => "MegaMod_ModuleSkills_Desc";
        public override int Order => 2;

        public static new SkillSettings ModSettings => Instance?.CurrentSettings ?? ModModule<SkillSettings>.ModSettings;
        public static new SkillSettings Settings => ModSettings;

        public SkillsModule()
        {
            Instance = this;
        }

        public override void DrawSettings(Rect inRect)
        {
            SkillSettingsDrawer.DrawSettings(inRect, Settings);
        }
    }
}
