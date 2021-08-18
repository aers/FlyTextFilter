﻿using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin;
using ImGuiNET;
using Lumina.Text;

namespace FlyTextFilter
{
    /// <summary>
    /// Enum of FlyTextKind values. Members suffixed with
    /// a number seem to be a duplicate, or perform duplicate behavior.
    /// </summary>
    public enum FlyTextKind
    {
        /// <summary>
        /// Val1 in serif font, Text2 in sans-serif as subtitle.
        /// Used for autos and incoming DoTs.
        /// </summary>
        AutoAttack = 0,

        /// <summary>
        /// Val1 in serif font, Text2 in sans-serif as subtitle.
        /// Does a bounce effect on appearance.
        /// </summary>
        DirectHit = 1,

        /// <summary>
        /// Val1 in larger serif font with exclamation, with Text2
        /// in sans-serif as subtitle. Does a bigger bounce effect on appearance.
        /// </summary>
        CriticalHit = 2,

        /// <summary>
        /// Val1 in even larger serif font with 2 exclamations, Text2 in
        /// sans-serif as subtitle. Does a large bounce effect on appearance.
        /// Does not scroll up or down the screen.
        /// </summary>
        CriticalDirectHit = 3,

        /// <summary>
        /// AutoAttack with sans-serif Text1 to the left of the Val1.
        /// </summary>
        NamedAttack = 4,

        /// <summary>
        /// DirectHit with sans-serif Text1 to the left of the Val1.
        /// </summary>
        NamedDirectHit = 5,

        /// <summary>
        /// CriticalHit with sans-serif Text1 to the left of the Val1.
        /// </summary>
        NamedCriticalHit = 6,

        /// <summary>
        /// CriticalDirectHit with sans-serif Text1 to the left of the Val1.
        /// </summary>
        NamedCriticalDirectHit = 7,

        /// <summary>
        /// All caps, serif MISS.
        /// </summary>
        Miss = 8,

        /// <summary>
        /// Sans-serif Text1 next to all caps serif MISS.
        /// </summary>
        NamedMiss = 9,

        /// <summary>
        /// All caps serif DODGE.
        /// </summary>
        Dodge = 10,

        /// <summary>
        /// Sans-serif Text1 next to all caps serif DODGE.
        /// </summary>
        NamedDodge = 11,

        /// <summary>
        /// Icon next to sans-serif Text1.
        /// </summary>
        NamedIcon = 12,
        NamedIcon2 = 13,

        /// <summary>
        /// Serif Val1 with all caps condensed font EXP with Text2 in sans-serif as subtitle.
        /// </summary>
        Exp = 14,

        /// <summary>
        /// Sans-serif Text1 next to serif Val1 with all caps condensed font MP with Text2 in sans-serif as subtitle.
        /// </summary>
        NamedMp = 15,

        /// <summary>
        /// Sans-serif Text1 next to serif Val1 with all caps condensed font TP with Text2 in sans-serif as subtitle.
        /// </summary>
        NamedTp = 16,

        NamedHeal = 17,
        NamedMp2 = 18,
        NamedTp2 = 19,

        /// <summary>
        /// Sans-serif Text1 next to serif Val1 with all caps condensed font EP with Text2 in sans-serif as subtitle.
        /// </summary>
        NamedEp = 20,

        /// <summary>
        /// Displays nothing.
        /// </summary>
        None = 21,

        /// <summary>
        /// All caps serif INVULNERABLE.
        /// </summary>
        Invulnerable = 22,

        /// <summary>
        /// All caps sans-serif condensed font INTERRUPTED!
        /// Does a large bounce effect on appearance.
        /// Does not scroll up or down the screen.
        /// </summary>
        Interrupted = 23,

        /// <summary>
        /// AutoAttack with no Text2.
        /// </summary>
        AutoAttackNoText = 24,
        AutoAttackNoText2 = 25,
        CriticalHit2 = 26,
        AutoAttackNoText3 = 27,
        NamedHealCriticalHit = 28,

        /// <summary>
        /// Same as NamedCriticalHit with a green (cannot change) MP in condensed font to the right of Val1.
        /// Does a jiggle effect to the right on appearance.
        /// </summary>
        NamedCriticalHitWithMp = 29,

        /// <summary>
        /// Same as NamedCriticalHit with a yellow (cannot change) TP in condensed font to the right of Val1.
        /// Does a jiggle effect to the right on appearance.
        /// </summary>
        NamedCriticalHitWithTp = 30,

        /// <summary>
        /// Same as NamedIcon with sans-serif "has no effect!" to the right.
        /// </summary>
        NamedIconHasNoEffect = 31,

        /// <summary>
        /// Same as NamedIcon but Text1 is slightly faded. Used for buff expiration.
        /// </summary>
        NamedIconFaded = 32,
        NamedIconFaded2 = 33,

        /// <summary>
        /// Text1 in sans-serif font.
        /// </summary>
        Named = 34,

        /// <summary>
        /// Same as NamedIcon with sans-serif "(fully resisted)" to the right.
        /// </summary>
        NamedIconFullyResisted = 35,

        /// <summary>
        /// All caps serif 'INCAPACITATED!'.
        /// </summary>
        Incapacitated = 36,

        /// <summary>
        /// Text1 with sans-serif "(fully resisted)" to the right.
        /// </summary>
        NamedFullyResisted = 37,

        /// <summary>
        /// Text1 with sans-serif "has no effect!" to the right.
        /// </summary>
        NamedHasNoEffect = 38,

        NamedAttack3 = 39,
        NamedMp3 = 40,
        NamedTp3 = 41,

        /// <summary>
        /// Same as NamedIcon with serif "INVULNERABLE!" beneath the Text1.
        /// </summary>
        NamedIconInvulnerable = 42,

        /// <summary>
        /// All caps serif RESIST.
        /// </summary>
        Resist = 43,

        /// <summary>
        /// Same as NamedIcon but places the given icon in the item icon outline.
        /// </summary>
        NamedIconWithItemOutline = 44,

        AutoAttackNoText4 = 45,
        CriticalHit3 = 46,

        /// <summary>
        /// All caps serif REFLECT.
        /// </summary>
        Reflect = 47,

        /// <summary>
        /// All caps serif REFLECTED.
        /// </summary>
        Reflected = 48,

        DirectHit2 = 49,
        CriticalHit5 = 50,
        CriticalDirectHit2 = 51
    }

    public class FlyTextFilter : IDalamudPlugin
    {
        public string Name => "FlyTextFilter";

        private DalamudPluginInterface _interface;
        private FlyTextFilterConfig _config;

        private bool ConfigOpen = false;
        
        private delegate IntPtr CreateFlyTextDelegate(
            IntPtr addonFlyText,
            FlyTextKind kind,
            int val1,
            int val2,
            IntPtr text2,
            uint color,
            uint icon,
            IntPtr text1,
            float yOffset);

        private Hook<CreateFlyTextDelegate> createFlyTextHook;
        
        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _interface = pluginInterface;
            _config = new FlyTextFilterConfig();
            _config.Initialize(_interface);
            var address =
                _interface.TargetModuleScanner.ScanText("48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 40 48 63 FA");
            createFlyTextHook = new Hook<CreateFlyTextDelegate>(address, CreateFlyTextDetour);
            createFlyTextHook.Enable();
            
            _interface.UiBuilder.OnOpenConfigUi += UiBuilder_OnOpenConfigUi;
            _interface.UiBuilder.OnBuildUi += UiBuilder_OnBuild;
        }

        public void Dispose()
        {
            createFlyTextHook.Dispose();
            _interface.UiBuilder.OnOpenConfigUi -= UiBuilder_OnOpenConfigUi;
            _interface.UiBuilder.OnBuildUi -= UiBuilder_OnBuild;
        }

        public IntPtr CreateFlyTextDetour(IntPtr addonFlyText,
            FlyTextKind kind,
            int val1,
            int val2,
            IntPtr text2,
            uint color,
            uint icon,
            IntPtr text1,
            float yOffset)
        {
            if (text1 != IntPtr.Zero)
            {
                PluginLog.Log($"{kind} - {Marshal.PtrToStringAnsi(text1)}");
            }
            else
            {
                PluginLog.Log($"{kind}");
            }

            if ((int)kind < 52 && !_config.KindToggleArray[(int)kind])
                return IntPtr.Zero;
                
            return createFlyTextHook.Original(addonFlyText, kind, val1, val2, text2, color, icon, text1, yOffset);
        }
        
        public void UiBuilder_OnOpenConfigUi(object sender, EventArgs args) => ConfigOpen = true;

        public void UiBuilder_OnBuild()
        {
            if (!ConfigOpen) return;

            ImGui.SetNextWindowSize(new Vector2(300, 400));
            if (!ImGui.Begin(Name, ref ConfigOpen))
            {
                ImGui.End();
                return;
            }
            
            ImGui.Text("Use /xllog to see a log of message kinds.");

            bool configChanged = false;
            
            foreach(int i in Enum.GetValues(typeof(FlyTextKind)))
            {
                configChanged |= ImGui.Checkbox($"{Enum.GetName(typeof(FlyTextKind), i)}",
                    ref _config.KindToggleArray[i]);
            }

            if (configChanged)
                _config.Save();

            ImGui.End();
        }
    }
}