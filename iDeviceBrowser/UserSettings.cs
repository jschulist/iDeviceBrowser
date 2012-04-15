using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace iDeviceBrowser
{
    public class UserSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("false")]
        public bool IsNotFirstLoad
        {
            get
            {
                return (bool)this["IsNotFirstLoad"];
            }
            set
            {
                this["IsNotFirstLoad"] = value;
            }
        }

        [UserScopedSetting()]
        public List<Favorite> Favorites
        {
            get
            {
                return (List<Favorite>)this["Favorites"];
            }
            set
            {
                this["Favorites"] = value;
            }
        }

        public void Setup()
        {
            if (!this.IsNotFirstLoad)
            {
                this.Favorites = new List<Favorite>()
                {
                    new Favorite("Ringtones", "/Library/Ringtones"),
                    new Favorite("UI Sounds", "/System/Library/Audio/UISounds"),
                    new Favorite("SpringBoard Image && Settings", "/System/Library/CoreServices/SpringBoard.app"),
                    new Favorite("WinterBoard Themes", "/Library/Themes"),
                    new Favorite("SummerBoard Themes", "/var/mobile/Library/Summerboard/Themes"),
                    new Favorite("Camera Roll", "/var/mobile/Media/DCIM"),
                    new Favorite("Fonts", "/System/Library/Fonts"),
                    new Favorite("Installous Downloads", "/private/var/mobile/Documents/Installous/Downloads")
                };
                this.IsNotFirstLoad = true;
                this.Save();
            }
        }
    }
}
