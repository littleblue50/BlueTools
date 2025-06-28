using ECommons.UIHelpers.AddonMasterImplementations;

namespace BlueTools.Utils;

public unsafe class GameUI
{
    public bool SkipTalk()
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.Talk>(out var talk) && talk.IsVisible)
        {
            while (talk.IsVisible) {
                talk.Click();
                EzThrottler.Throttle("TalkClick", 1000);
            }
            return true;
        }
        return false;
    }

    public bool SelectListIndex(int index)
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.SelectString>(out var addon) && addon.IsVisible && addon.Entries.Length > 0)
        {
            addon.Entries[index].Select();
            return true;
        }
        return false;
    }

    public bool SelectYesno(bool yes = true)
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.SelectYesno>(out var addon) && addon.IsVisible)
        {
            if (yes) addon.Yes();
            else addon.No();

            return true;
        }
        return false;
    }

    public bool ConfirmContentsFinder()
    {
        if (GenericHelpers.TryGetAddonMaster<AddonMaster.ContentsFinderConfirm>(out var addon) && addon.IsVisible)
        {
            try
            {
                addon.Commence();
                return true;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
}