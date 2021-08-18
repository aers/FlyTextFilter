using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace FlyTextFilter
{
    public class FlyTextFilterConfig : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        
        [NonSerialized] private DalamudPluginInterface _pluginInterface;

        public bool[] KindToggleArray;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
            KindToggleArray = new bool[52];
            for (int i = 0; i < 52; i++)
                KindToggleArray[i] = true;
        }

        public void Save()
        {
            _pluginInterface.SavePluginConfig(this);
        }
    }
}