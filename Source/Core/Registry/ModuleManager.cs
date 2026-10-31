using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SadSeal.Common;
using SadSeal.Core.Contracts;

namespace SadSeal.Core.Registry
{
    public static class ModuleManager
    {
        private static readonly List<IModModule> modules = new List<IModModule>();
        private static readonly Dictionary<string, IModModule> modulesById = new Dictionary<string, IModModule>(StringComparer.OrdinalIgnoreCase);
        private static bool discovered = false;
        private static bool initialized = false;

        public static IReadOnlyList<IModModule> Modules => modules;

        public static void DiscoverModules()
        {
            if (discovered) return;
            discovered = true;

            var moduleList = new List<IModModule>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var asm in assemblies)
            {
                if (asm == null) continue;

                // Optimization: only inspect relevant assemblies
                string asmName = asm.GetName().Name ?? string.Empty;
                if (!asmName.StartsWith("SadSeal", StringComparison.OrdinalIgnoreCase) &&
                    !asmName.StartsWith("0SadSeal", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    Type[] types;
                    try
                    {
                        types = asm.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types.Where(t => t != null).ToArray();
                    }

                    foreach (var type in types)
                    {
                        if (type == null || type.IsAbstract || type.IsInterface) continue;
                        if (typeof(IModModule).IsAssignableFrom(type))
                        {
                            try
                            {
                                if (Activator.CreateInstance(type) is IModModule module)
                                {
                                    moduleList.Add(module);
                                }
                            }
                            catch (Exception ex)
                            {
                                SadLog.Error("Core", $"Failed to instantiate module {type.FullName}: {ex}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SadLog.Warning("Core", $"Error inspecting assembly {asm.FullName}: {ex.Message}");
                }
            }

            modules.Clear();
            modules.AddRange(moduleList.OrderBy(m => m.Order).ThenBy(m => m.Id, StringComparer.OrdinalIgnoreCase));

            modulesById.Clear();
            foreach (var m in modules)
            {
                modulesById[m.Id] = m;
            }

            SadLog.Message("Core", $"Discovered {modules.Count} modules: {string.Join(", ", modules.Select(m => m.Id))}");
        }

        public static T GetModule<T>() where T : class, IModModule
        {
            return modules.OfType<T>().FirstOrDefault();
        }

        public static IModModule GetModule(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return modulesById.TryGetValue(id, out var mod) ? mod : null;
        }

        public static void InitializeAll(Harmony harmony)
        {
            if (initialized) return;
            initialized = true;

            int enabledCount = 0;
            int skippedCount = 0;

            foreach (var module in modules)
            {
                if (module.IsEnabled)
                {
                    try
                    {
                        module.Initialize(harmony);
                        enabledCount++;
                    }
                    catch (Exception ex)
                    {
                        SadLog.Error("Core", $"Failed to initialize module {module.Id}: {ex}");
                    }
                }
                else
                {
                    skippedCount++;
                }
            }

            SadLog.Message("Core", $"Initialized modules: {enabledCount} active, {skippedCount} skipped.");
        }

        public static void ExposeAllData()
        {
            foreach (var module in modules)
            {
                try
                {
                    module.ExposeData();
                }
                catch (Exception ex)
                {
                    SadLog.Error("Core", $"Failed to expose data for module {module.Id}: {ex}");
                }
            }
        }

        public static void OnDefsLoadedAll()
        {
            foreach (var module in modules)
            {
                try
                {
                    module.OnDefsLoaded();
                }
                catch (Exception ex)
                {
                    SadLog.Error("Core", $"Error in OnDefsLoaded for module {module.Id}: {ex}");
                }
            }
        }
    }
}
