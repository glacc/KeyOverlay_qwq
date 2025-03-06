
using Glacc.UI;
using SFML.Graphics;
using SFML.Window;
using System.Globalization;

namespace Glacc.KeyOverlay_qwq
{
    internal class AppSettings
    {
        public static string fontFileName = string.Empty;

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
        public static float keyPressAlpha = 0.5f;

        public static float lightAlphaCorrection = 0.25f;
        public static int lightingRange = 64;

        public static bool showCounter = true;
        public static int counterFontSize = 16;
        public static int counterFontWidth = 10;

        public static float counterSpeed = 0.2f;

        public static int fadeHeight = 200;

        public static Color backgroundColour = Color.Black;

        public static int keyCount = 2;

        // public static int borderTop = 0;
        public static int borderLeft = 0;
        public static int borderRight = 0;
        public static int borderBottom = 0;

        public static Keyboard.Key? keyClearCounter = null;

        public static List<float> keyWidthMultipliers = new List<float>();

        public static int totalKeyWidth = 0;

        static void CheckRequestedSectionsInConfigFile()
        {
            string[] requestedSections = ["Keys", "General", "Borders"];

            foreach (string sectionName in requestedSections)
            {
                if (!Config.config.ContainsKey(sectionName))
                    throw new Exception($"Section {sectionName} is missing.");
            }
        }
        public static void CheckKeyCountAndCalculateTotalKeyWidth()
        {
            totalKeyWidth = 0;

            int numOfKey = 0;
            while (true)
            {
                string configKeyName = $"Key{numOfKey + 1}";
                if (!Config.config["Keys"].ContainsKey(configKeyName))
                    break;

                float widthMultiplier;
                if (Config.config.ContainsKey("Size"))
                {
                    if (Config.config["Size"].ContainsKey(configKeyName))
                        widthMultiplier = float.Parse(Config.config["Size"][configKeyName], CultureInfo.InvariantCulture);
                    else
                        widthMultiplier = 1.0f;
                }
                else
                    widthMultiplier = 1.0f;

                keyWidthMultipliers.Add(widthMultiplier);

                int keyWidth = (int)(keySize * widthMultiplier);

                totalKeyWidth += keyWidth + keySpacing;

                numOfKey++;
            }

            keyCount = numOfKey;
        }

        public static void UpdateSettingsFromConfig()
        {
            CheckRequestedSectionsInConfigFile();

            // framerate = int.Parse(Config.config["General"]["FPS"]);
            tickrate = int.Parse(Config.config["General"]["TickRate"]);

            fadeHeight = int.Parse(Config.config["General"]["FadeHeight"]);

            barSpeed = float.Parse(Config.config["General"]["BarSpeed"], CultureInfo.InvariantCulture);

            keySize = int.Parse(Config.config["General"]["KeySize"]);
            keyBorderSize = int.Parse(Config.config["General"]["KeyBorderSize"]);
            keySpacing = int.Parse(Config.config["General"]["KeySpacing"]);

            keyPressAlpha = float.Parse(Config.config["General"]["KeyPressAlpha"], CultureInfo.InvariantCulture);

            keyFontSize = int.Parse(Config.config["General"]["KeyFontSize"]);
            keyLightFadeDuration = int.Parse(Config.config["General"]["LightFadeTime"]);

            lightAlphaCorrection = float.Parse(Config.config["General"]["LightAlphaCorrection"], CultureInfo.InvariantCulture);
            lightingRange = int.Parse(Config.config["General"]["LightRange"]);

            counterFontSize = int.Parse(Config.config["General"]["CounterFontSize"]);
            counterFontWidth = int.Parse(Config.config["General"]["CounterFontWidth"]);
            counterSpeed = float.Parse(Config.config["General"]["CounterSpeed"], CultureInfo.InvariantCulture);

            string showCounterStr = Config.config["General"]["ShowCounter"].ToLower();
            showCounter = (showCounterStr == "true" || showCounterStr == "yes");

            fontFileName = Config.config["General"]["FontFileName"];
            Settings.LoadFont(fontFileName);

            // Background color
            byte[] backgroundColourBytes = new byte[4];
            backgroundColourBytes[3] = 255;
            string backgroundColourString = Config.config["Colours"]["Background"];
            string[] backgroundRGBString = backgroundColourString.Split(',');
            for (int i = 0; i < Math.Min(backgroundRGBString.Length, 4); i++)
                backgroundColourBytes[i] = byte.Parse(backgroundRGBString[i]);
            backgroundColour = new Color
                (
                    backgroundColourBytes[0],
                    backgroundColourBytes[1],
                    backgroundColourBytes[2],
                    backgroundColourBytes[3]
                );

            // Key count check and key size
            CheckKeyCountAndCalculateTotalKeyWidth();

            // Borders
            // borderTop = int.Parse(Config.config["Borders"]["Top"]);
            borderLeft = int.Parse(Config.config["Borders"]["Left"]);
            borderRight = int.Parse(Config.config["Borders"]["Right"]);
            borderBottom = int.Parse(Config.config["Borders"]["Bottom"]);

            // Window size
            height = int.Parse(Config.config["General"]["Height"]);
            width = keySpacing + totalKeyWidth + borderLeft + borderRight;

            if (Config.config["Keys"].ContainsKey("ClearCounter"))
                keyClearCounter = (Keyboard.Key)Enum.Parse(typeof(Keyboard.Key), Config.config["Keys"]["ClearCounter"]);
        }
    }
}
