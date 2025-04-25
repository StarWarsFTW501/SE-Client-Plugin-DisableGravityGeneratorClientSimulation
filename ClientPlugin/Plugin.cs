using System;
using System.Reflection;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Layouts;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using VRage.Plugins;
using ClientPlugin.External.Config;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using System.IO;
using VRage.FileSystem;
using VRage.Utils;
using Sandbox.ModAPI;
using Sandbox.Game.Gui;

namespace ClientPlugin
{
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "DisableGravityGeneratorClientSim";
        public const string FriendlyName = "GravGen Simulation Inhibitor";

        PersistentConfig<PluginConfig> _config;
        public IPluginConfig PluginConfig => _config?.Data;

        static readonly string ConfigFileName = $"{Name}.cfg";

        public static Plugin Instance { get; private set; }
        internal SettingsGenerator SettingsGenerator { get; private set; }



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;
            SettingsGenerator = new SettingsGenerator();

            var configPathName = Path.Combine(MyFileSystem.UserDataPath, ConfigFileName);
            _config = PersistentConfig<PluginConfig>.Load(configPathName);

            MyAPIUtilities.Static.MessageEntered += ChatEntered;

            MyLog.Default.Info($"{Name}: Initializing patches...");

            // TODO: Put your one time initialization code here.
            Harmony harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }


        public void SaveConfigNow() => _config.SaveNow();



        public void Dispose()
        {
            // TODO: Save state and close resources here, called when the game exits (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.

            Instance = null;
        }

        public void Update()
        {

        }

        // ReSharper disable once UnusedMember.Global
        public void OpenConfigDialog()
        {
            Instance.SettingsGenerator.SetLayout<SettingsLayout>();
            MyGuiSandbox.AddScreen(Instance.SettingsGenerator.Dialog);
        }

        void ChatEntered(string messageText, ref bool sendToOthers)
        {
            if (messageText.Equals("/inhibitgravgens", StringComparison.OrdinalIgnoreCase))
            {
                sendToOthers = false;
                bool newValue = PluginConfig.DisableUpdate = !PluginConfig.DisableUpdate;
                MyHud.Chat.ShowMessage(FriendlyName, $"Gravity generator simulation inhibition on servers now {(newValue ? "ON" : "OFF")}");
            }
        }
    }
}