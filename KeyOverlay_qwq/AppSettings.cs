
using SFML.Graphics;
using System.Globalization;

namespace Glacc.KeyOverlay_qwq
{
    internal class AppSettings
    {
        public static int width;
        public static int height = 400;

        // public static int framerate = 60;
        public static int tickrate = 60;

        public static float barSpeed = 1000f;

        public static int keySize = 64;
        public static int keyBorderSize = 4;
        public static int keySpacing = 24;

        public static int keyFontSize = 24;
        public static int keyLightFadeDuration = 8;

        public static int fadeHeight = 200;

        public static int lightingRange = 64;

        public static Color backgroundColour = Color.Black;

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

            // framerate = int.Parse(Config.config["General"]["FPS"]);
            tickrate = int.Parse(Config.config["General"]["TickRate"]);

            fadeHeight = int.Parse(Config.config["General"]["FadeHeight"]);

            barSpeed = float.Parse(Config.config["General"]["BarSpeed"], CultureInfo.InvariantCulture);

            keySize = int.Parse(Config.config["General"]["KeySize"]);
            keyBorderSize = int.Parse(Config.config["General"]["KeyBorderSize"]);
            keySpacing = int.Parse(Config.config["General"]["KeySpacing"]);

            keyFontSize = int.Parse(Config.config["General"]["KeyFontSize"]);
            keyLightFadeDuration = int.Parse(Config.config["General"]["LightFadeTime"]);

            lightingRange = int.Parse(Config.config["General"]["LightRange"]);

            byte[] backgroundColourBytes = new byte[3];
            string backgroundColourString = Config.config["Colours"]["Background"];
            string[] backgroundRGBString = backgroundColourString.Split(',');
            for (int i = 0; i < 3; i++)
                backgroundColourBytes[i] = byte.Parse(backgroundRGBString[i]);
            backgroundColour = new Color
                (
                    backgroundColourBytes[0],
                    backgroundColourBytes[1],
                    backgroundColourBytes[2]
                );

            height = int.Parse(Config.config["General"]["Height"]);
            width = keySpacing + ((keySize + keySpacing) * keyCount);
        }
    }
}
