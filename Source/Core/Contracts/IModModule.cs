using HarmonyLib;
using UnityEngine;

namespace SadSeal.Core.Contracts
{
    public interface IModModule
    {
        string Id { get; }
        string DisplayNameKey { get; }
        string DescriptionKey { get; }
        int Order { get; }
        bool IsEnabled { get; set; }
        bool DefaultEnabled { get; }
        string ScribeTag { get; }
        void Initialize(Harmony harmony);
        void OnDefsLoaded();
        void ExposeData();
        void DrawSettings(Rect inRect);
    }
}
