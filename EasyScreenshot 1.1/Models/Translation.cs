using System.Collections.Generic;
using EasyScreenshot.Resources;

namespace EasyScreenshot.Models
{
    public class Translation
    {
        public enum Languages
        {
            English = 0,
            Ukrainian,
            Russian
        }

        public static int GetIndex(Languages language)
        {
            switch (language)
            {
                case Languages.English:
                    return 0;
                case Languages.Ukrainian:
                    return 1;
                case Languages.Russian:
                    return 2;
            }

            return -1;
        }

        public enum Phrases
        {
            ReadyToMakeScreenshot,
            ClickToCameraIcon,
            ImageUrlWasCopied,
            PressToInsertImageLink,
            Ukrainian,
            English,
            Russian,
            Error,
            HotkeyNotSupported,
            AllSettingsSaved,
            Done,
            Hotkeys,
            CreateAndUpload,
            CreateAndSave,
            CreateAndUseSnippingTool,
            PathForScreenshots,
            Language,
            Save,
            Format,
            Screenshot,
            ImageWasSaved
        }

        public static string GetTranslation(Phrases phrase, Languages language)
        {
            switch (language)
            {
                case Languages.English:
                    return Dictionary_en.ResourceManager.GetString(phrase.ToString());
                case Languages.Russian:
                    return Dictionary_ru.ResourceManager.GetString(phrase.ToString());
                case Languages.Ukrainian:
                    return Dictionary_ua.ResourceManager.GetString(phrase.ToString());
                default:
                    return string.Empty;
            }
        }
    }
}
