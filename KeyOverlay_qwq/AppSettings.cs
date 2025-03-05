
namespace Glacc.KeyOverlay_qwq
{
    internal class AppSettings
    {
        public static int width;
        public static int height = 400;

        public static int keySize = 64;
        public static int keyFontSize = 24;
        public static int keyBorderSize = 4;
        public static int keySpacing = 24;
        public static int keyLightFadeDuration = 8;

        public static int fadeHeight = 200;

        public static int lightingRange = 64;

        public static byte[] backgroundColour = new byte[3];

        public static int keyCount = 2;

        static void CheckRequestedSectionsInConfigFile()
        {
            string[] requestedSections = ["Keys"];

            foreach (string sectionName in requestedSections)
            {
                if (!Config.config.ContainsKey(sectionName))
                    throw new Exception($"Section {sectionName} is missing.");
            }
        }
        public static void CheckKeyCount()
        {
            int numOfKey = 0;
            while (true)
            {
                string configKeyName = $"Key{numOfKey + 1}";
                if (!Config.config["Keys"].ContainsKey(configKeyName))
                    break;

                numOfKey++;
            }

            keyCount = numOfKey;
        }

        public static void UpdateSettingsFromConfig()
        {
            CheckRequestedSectionsInConfigFile();

            CheckKeyCount();

            keySize = int.Parse(Config.config["General"]["KeySize"]);
            keyBorderSize = int.Parse(Config.config["General"]["KeyBorderSize"]);
            keyFontSize = int.Parse(Config.config["General"]["KeyFontSize"]);
            keySpacing = int.Parse(Config.config["General"]["KeySpacing"]);
            keyLightFadeDuration = int.Parse(Config.config["General"]["FadeTime"]);

            lightingRange = int.Parse(Config.config["General"]["LightFade"]);

            string backgroundColourString = Config.config["Colours"]["Background"];
            string[] backgroundRGBString = backgroundColourString.Split(',');
            for (int i = 0; i < 3; i++)
                backgroundColour[i] = byte.Parse(backgroundRGBString[i]);

            height = int.Parse(Config.config["General"]["Height"]);
            width = keySpacing + ((keySize + keySpacing) * keyCount);
        }
    }
}
