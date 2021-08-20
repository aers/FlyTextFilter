using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
using Lumina.Misc;
using Lumina.Text;

namespace FlyTextFilter
{
    public enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        TargetsInvalid = 0x40000000,
        TargetsNoUpdate = TargetsInvalid,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    
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

    public unsafe class FlyTextFilter : IDalamudPlugin
    {
        public string Name => "FlyTextFilter";

        private DalamudPluginInterface _interface;
        private FlyTextFilterConfig _config;

        private bool ConfigOpen = false;

        private string _addToBlacklist = "";
        private string _removeBlacklist = null;

        private byte[] origBytes = new byte[9];
        private IntPtr addScreenLogAddress = IntPtr.Zero;
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

        private delegate void AddScreenLogDelegate(
            Character* target, Character* source, int logKind, int option, int actionKind, int actionId, int val1,
            int val2, int val3, int val4);

        private Hook<AddScreenLogDelegate> addScreenLogHook;
        
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualProtect(
            IntPtr lpAddress,
            UIntPtr dwSize,
            MemoryProtection flNewProtection,
            out MemoryProtection lpflOldProtect);
        
        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _interface = pluginInterface;
            _config = pluginInterface.GetPluginConfig() as FlyTextFilterConfig ?? new FlyTextFilterConfig();
            _config.Initialize(_interface);
            var address =
                _interface.TargetModuleScanner.ScanText("48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 40 48 63 FA");
            createFlyTextHook = new Hook<CreateFlyTextDelegate>(address, CreateFlyTextDetour);
            createFlyTextHook.Enable();

            addScreenLogAddress = _interface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? BB ?? ?? ?? ?? EB 32");
            Dalamud.SafeMemory.ReadBytes(addScreenLogAddress, 9, out origBytes);
            VirtualProtect(addScreenLogAddress, new UIntPtr(9), MemoryProtection.ReadWrite, out var oldProtect);
            Dalamud.SafeMemory.WriteBytes(addScreenLogAddress,
                new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 });
            VirtualProtect(addScreenLogAddress, new UIntPtr(9), oldProtect, out oldProtect);
            addScreenLogHook = new Hook<AddScreenLogDelegate>(addScreenLogAddress, AddScreenLogDetour);
            addScreenLogHook.Enable();
            
            _interface.UiBuilder.OnOpenConfigUi += UiBuilder_OnOpenConfigUi;
            _interface.UiBuilder.OnBuildUi += UiBuilder_OnBuild;
        }

        public void Dispose()
        {
            addScreenLogHook.Dispose();
            createFlyTextHook.Dispose();
            VirtualProtect(addScreenLogAddress, new UIntPtr(9), MemoryProtection.ReadWrite, out var oldProtect);
            Dalamud.SafeMemory.WriteBytes(addScreenLogAddress,
                origBytes);
            VirtualProtect(addScreenLogAddress, new UIntPtr(9), oldProtect, out oldProtect);
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
                var text = Marshal.PtrToStringAnsi(text1);
                if (text != null)
                {
                    PluginLog.Log($"{kind} - {text}");
                    if (_config.Blacklist.Any(str => str.Contains(text)))
                        return IntPtr.Zero;
                }
            }
            else
            {
                PluginLog.Log($"{kind}");
            }

            return createFlyTextHook.Original(addonFlyText, kind, val1, val2, text2, color, icon, text1, yOffset);
        }

        private void AddScreenLogDetour(
            Character* target, Character* source, int logKind, int option, int actionKind, int actionId, int val1,
            int val2, int val3, int val4)
        {
            if (target is null) return;
            if (logKind >= 52) return;
            var localPlayer = _interface.ClientState.LocalPlayer;
            if (localPlayer is not null)
            {
                if (localPlayer.Address.ToInt64() == (long)source && !_config.KindToggleListPlayer[logKind])
                    return;
                if (localPlayer.Address.ToInt64() != (long)source && !_config.KindToggleListOther[logKind])
                    return;
            }

            addScreenLogHook.Original(target, source, logKind, option, actionKind, actionId, val1, val2, val3, val4);
        }
        
        public void UiBuilder_OnOpenConfigUi(object sender, EventArgs args) => ConfigOpen = true;

        public void UiBuilder_OnBuild()
        {
            if (!ConfigOpen) return;

            ImGui.SetNextWindowSize(new Vector2(600, 700));
            if (!ImGui.Begin(Name, ref ConfigOpen))
            {
                ImGui.End();
                return;
            }

            if (ImGui.BeginTabBar("###mainbar"))
            {
                if (ImGui.BeginTabItem("Types"))
                {
                    ImGui.Text("Use /xllog to see a log of message kinds.");

                    bool configChanged = false;

                    if (ImGui.BeginTable("table_kinds", 3))
                    {
                        ImGui.TableSetupScrollFreeze(0, 1);
                        ImGui.TableSetupColumn("Kind", ImGuiTableColumnFlags.None);
                        ImGui.TableSetupColumn("Show From Yourself", ImGuiTableColumnFlags.None);
                        ImGui.TableSetupColumn("Show From Others", ImGuiTableColumnFlags.None);
                        ImGui.TableHeadersRow();
                        
                        foreach (int i in Enum.GetValues(typeof(FlyTextKind)))
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text($"{Enum.GetName(typeof(FlyTextKind), i)}");
                            ImGui.TableSetColumnIndex(1);
                            bool temp = _config.KindToggleListPlayer[i];
                            configChanged |= ImGui.Checkbox($"###{i}player",
                                ref temp);
                            _config.KindToggleListPlayer[i] = temp;

                            ImGui.TableSetColumnIndex(2);
                        
                            temp = _config.KindToggleListOther[i];
                            configChanged |= ImGui.Checkbox($"###{i}others",
                                ref temp);
                            _config.KindToggleListOther[i] = temp;
                        }

                        ImGui.EndTable();
                    }


                    if (configChanged)
                        _config.Save();

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Name Blacklist"))
                {
                    ImGui.InputText("###addToBlacklist", ref _addToBlacklist, 100);
                    if (ImGui.Button("Add To Blacklist"))
                    {
                        _config.Blacklist.Add(_addToBlacklist);
                        _config.Save();
                    }
                    
                    ImGui.Separator();
                    
                    foreach(var blString in _config.Blacklist)
                    {
                        if (ImGui.Button($"Remove###{blString}"))
                        {
                            _removeBlacklist = blString;
                        }
                        ImGui.SameLine();
                        ImGui.Text(blString);
                    }

                    if (_removeBlacklist is not null)
                    {
                        _config.Blacklist.Remove(_removeBlacklist);
                        _removeBlacklist = null;
                        _config.Save();
                    }

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            ImGui.End();
        }
    }
}