namespace FFXIVClientStructs.FFXIV.Client.Game.Character;

// Client::Game::Character::MountContainer
//   Client::Game::Character::ContainerInterface
[GenerateInterop]
[Inherits<ContainerInterface>]
[StructLayout(LayoutKind.Explicit, Size = 0x68)]
public unsafe partial struct MountContainer {
    [FieldOffset(0x10)] public Character* MountObject;
    [FieldOffset(0x18)] public ushort MountId;
    [FieldOffset(0x1C)] public float DismountTimer;
    //1 in dismount animation, 4 = instant delete mount when dismounting (used for npcs and such)
    [FieldOffset(0x20)] public byte Flags;
    [FieldOffset(0x24), FixedSizeArray] internal FixedSizeArray7<uint> _mountedEntityIds;

    [MemberFunction("E8 ?? ?? ?? ?? 0F B7 56 66")]
    public partial void CreateAndSetupMount(short mountId, uint buddyModelTop, uint buddyModelBody, uint buddyModelLegs, byte buddyStain, byte unk6, byte unk7);

    [MemberFunction("E8 ?? ?? ?? ?? 48 8B 45 08 0F B6 90 ?? ?? ?? ??")]
    public partial void SetupMount(short mountId, uint buddyModelTop, uint buddyModelBody, uint buddyModelLegs, byte buddyStain);
}
