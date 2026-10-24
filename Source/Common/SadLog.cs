using Verse;

namespace SadSeal.Common
{
    public static class SadLog
    {
        public static void Message(string module, string text) => Log.Message($"[SadSeal {module}] {text}");
        public static void Warning(string module, string text) => Log.Warning($"[SadSeal {module}] {text}");
        public static void Error(string module, string text) => Log.Error($"[SadSeal {module}] {text}");
    }
}
