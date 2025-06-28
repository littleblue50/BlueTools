namespace FFXIVClientStructs.FFXIV.Client.Game.Character;

// Client::Game::Character::CharacterSetupContainer
//   Client::Game::Character::ContainerInterface
[GenerateInterop]
[Inherits<ContainerInterface>]
[StructLayout(LayoutKind.Explicit, Size = 0x18)]
public unsafe partial struct CharacterSetupContainer {
    [MemberFunction("E8 ?? ?? ?? ?? 0F B6 9E ?? ?? ?? ?? 48 8D 8E ?? ?? ?? ??")]
    public partial ulong CopyFromCharacter(Character* source, CopyFlags flags);

    [MemberFunction("E8 ?? ?? ?? ?? 45 0F B6 86 ?? ?? ?? ?? 48 8D 8F")]
    public partial void SetupBNpc(uint bNpcBaseId, uint bNpcNameId = 0);

    [Flags]
    public enum CopyFlags : uint {
        None = 0x00,
        Mode = 0x1, // emote loop etc
        Mount = 0x2,
        ClassJob = 0x4,
        Companion = 0x20,
        WeaponHiding = 0x80,
        Target = 0x400,
        Name = 0x1000,
        LastAnimation = 0x8000,
        Position = 0x10000, // includes rotation
        UseSecondaryCharaId = 0x200000,
        Ornament = 0x400000
    }
}
