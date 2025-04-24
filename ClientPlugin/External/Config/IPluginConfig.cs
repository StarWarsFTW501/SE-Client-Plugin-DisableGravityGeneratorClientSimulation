using System.ComponentModel;

namespace ClientPlugin.External.Config
{
    public interface IPluginConfig : INotifyPropertyChanged
    {
        // Define plugin configuration items here
        bool DisableUpdate { get; set; }

    }
}
