using System;
using HarmonyLib;
using SadSeal.Core.Contracts;
using UnityEngine;
using Verse;

namespace SadSeal.Core.Modules
{
    public abstract class ModModule : IModModule
    {
        public abstract string Id { get; }
        public virtual string DisplayNameKey => $"MegaMod_Tab{Id}";
        public virtual string DescriptionKey => $"MegaMod_Module{Id}_Desc";
        public virtual int Order => 100;
        public virtual bool DefaultEnabled => true;
        public virtual bool IsEnabled { get; set; } = true;
        public virtual string ScribeTag => Id.ToLowerInvariant();

        public virtual void Initialize(Harmony harmony)
        {
            harmony.PatchAll(GetType().Assembly);
        }

        public virtual void OnDefsLoaded() { }

        public abstract void ExposeData();

        public abstract void DrawSettings(Rect inRect);
    }

    public abstract class ModModule<TSettings> : ModModule where TSettings : class, IExposable, new()
    {
        private static TSettings modSettings;

        public static TSettings ModSettings
        {
            get
            {
                if (modSettings == null)
                {
                    modSettings = new TSettings();
                }
                return modSettings;
            }
            internal set => modSettings = value;
        }

        public static TSettings Settings => ModSettings;

        public TSettings CurrentSettings => ModSettings;

        public override void ExposeData()
        {
            if (modSettings == null)
            {
                modSettings = new TSettings();
            }

            TSettings s = modSettings;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Deep.Look(ref s, ScribeTag);
                if (s == null && !string.Equals(ScribeTag, Id.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
                {
                    Scribe_Deep.Look(ref s, Id.ToLowerInvariant());
                }
            }
            else
            {
                Scribe_Deep.Look(ref s, ScribeTag);
            }

            if (s != null)
            {
                modSettings = s;
            }
            else if (modSettings == null)
            {
                modSettings = new TSettings();
            }
        }
    }
}
