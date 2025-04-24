using ClientPlugin.Settings.Elements;
using Sandbox.Game.Multiplayer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClientPlugin.External.Config
{
    public class PluginConfig : IPluginConfig
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void SetValue<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;

            OnPropertyChanged(propName);
        }

        void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;

            propertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        // Settings dialog title
        public static readonly string Title = "Gravity Generator Update Inhibitor Configuration";

        bool _disableUpdate = true;

        [Checkbox("Toggle Plugin", "Switch for the plugin's inhibition of gravity generator updates")]
        public bool DisableUpdate
        {
            get => _disableUpdate;
            set
            {
                SetValue(ref _disableUpdate, value);
                if (!Sync.IsServer)
                    MyPatchUtilities.ForceFieldUpdates();
            }
        }
        [Button("Confirm", "Confirm the changes")]
        public void ConfirmButton()
        {
            Plugin.Instance.SaveConfigNow();
            Plugin.Instance.SettingsGenerator.Dialog.CloseScreen();
        }
    }
}
