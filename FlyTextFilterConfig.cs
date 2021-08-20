using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace FlyTextFilter
{
    public class FlyTextFilterConfig : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        
        [NonSerialized] private DalamudPluginInterface _pluginInterface;

        public List<bool> KindToggleListPlayer = new();
        public List<bool> KindToggleListOther = new();
        public HashSet<string> Blacklist = new();

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
            
            while (KindToggleListPlayer.Count < 52)
                KindToggleListPlayer.Add(true);
            while (KindToggleListOther.Count < 52)
                KindToggleListOther.Add(true);
            
        }

        public void Save()
        {
            _pluginInterface.SavePluginConfig(this);
        }
    }
}