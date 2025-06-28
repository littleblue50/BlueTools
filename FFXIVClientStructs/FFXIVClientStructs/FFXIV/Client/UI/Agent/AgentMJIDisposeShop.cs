using FFXIVClientStructs.FFXIV.Client.System.String;

namespace FFXIVClientStructs.FFXIV.Client.UI.Agent;

// Client::UI::Agent::AgentMJIDisposeShop
//   Client::UI::Agent::AgentInterface
//     Component::GUI::AtkModuleInterface::AtkEventInterface
[Agent(AgentId.MJIDisposeShop)]
[GenerateInterop]
[Inherits<AgentInterface>]
[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe partial struct AgentMJIDisposeShop {
    [FieldOffset(0x28)] public AgentData* Data;

    [GenerateInterop]
    [StructLayout(LayoutKind.Explicit, Size = 0x170)]
    public unsafe partial struct AgentData {
        public const int NumCategories = 4;

        [FieldOffset(0x000)] public int SelectCountAddonHandle;
        [FieldOffset(0x004)] public int ConfirmAddonHandle;
        [FieldOffset(0x008), FixedSizeArray] internal FixedSizeArray2<uint> _currencyIconIds;
        [FieldOffset(0x010), FixedSizeArray] internal FixedSizeArray2<int> _currencyCounts;
        [FieldOffset(0x018), FixedSizeArray] internal FixedSizeArray2<int> _currencyStackSizes;
        [FieldOffset(0x020), FixedSizeArray] internal FixedSizeArray2<uint> _currencyItemIds;
        [FieldOffset(0x028)] public byte InitializationState; // 0 = none, 1 = opened, 3 = fully initialized
        [FieldOffset(0x029)] public byte CurSelectedCategory;
        [FieldOffset(0x02A)] public byte CurShipItemIndex; // in Items vector
        [FieldOffset(0x02B)] public byte CurBulkShipCheckStage; // 0 = check overcap, 1 = check material deficit after shipment, 2 = done
        [FieldOffset(0x02C)] public byte u2C; // ??? can be 0/1/2
        [FieldOffset(0x030)] public int CurBulkShiptLimit;
        [FieldOffset(0x034)] public int CurShipQuantity;
        // 0x38: substructure of size 0xA0, responsible for reading excel sheets
        [FieldOffset(0x0D8)] public StdVector<Utf8String> CategoryNames;
        [FieldOffset(0x0F0)] public StdVector<ItemData> Items;

        [FieldOffset(0x108), FixedSizeArray] internal FixedSizeArray4<StdVector<Pointer<ItemData>>> _perCategoryItems;

        [FieldOffset(0x168)] public bool DataInitialized; // set after all sheets are read and vectors are filled with data
        [FieldOffset(0x169)] public bool AddonDirty;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    public unsafe partial struct ItemData {
        [FieldOffset(0x00)] public byte ShopItemRowId; // in MJIDisposalShopItem sheet
        [FieldOffset(0x01)] public byte ItemIndex; // in Items vector
        [FieldOffset(0x02)] public bool UseIslanderCowries;
        // at 0x4, there's a copy of the excel row data (size 8)
        [FieldOffset(0x04)] public ushort CowriesPerItem;
        [FieldOffset(0x06)] public byte PouchItemRowId; // in MJIItemPouch sheet
        [FieldOffset(0x07)] public byte Currency; // 0 = seafarer cowries, 1 = islander cowries
        [FieldOffset(0x08)] public byte UICategory;
        [FieldOffset(0x09)] public byte SortOrder;
        [FieldOffset(0x0C)] public uint ItemId; // in Item sheet
        [FieldOffset(0x10)] public uint IconId;
        [FieldOffset(0x14)] public uint CountInInventory;
        [FieldOffset(0x18)] public Utf8String Name;
    }
}
