using Glacc.KeyOverlay_qwq.Elements;
using Glacc.UI;
using SFML.Graphics;
using SFML.Window;
using System.Globalization;

namespace Glacc.KeyOverlay_qwq
{
    internal class Program
    {
        static AppWindow? window;

        static List<Element?> elements = new List<Element?>();

        static Mutex settingsUpdateMutex = new Mutex();
        static bool settingsUpdateFlag = false;

        static RenderStates renderStatesAlphaBlend = RenderStates.Default;

        static void InitRenderStates()
        {
            renderStatesAlphaBlend.BlendMode.AlphaSrcFactor = BlendMode.Factor.One;
            renderStatesAlphaBlend.BlendMode.AlphaDstFactor = BlendMode.Factor.OneMinusSrcAlpha;
            Settings.renderStates = renderStatesAlphaBlend;
        }

        static void AddKeysElements()
        {
            int keyX = AppSettings.keySpacing + AppSettings.borderLeft;
            int keyY = AppSettings.height - AppSettings.keySpacing - AppSettings.keySize - AppSettings.borderBottom;

            int keyBottom = keyY + AppSettings.keySize;

            for (int i = 0; i < AppSettings.keyCount; i++)
            {
                string configKeyName = $"Key{i + 1}";

                // Key size

                int keyWidth;
                int keyHeight = AppSettings.keySize;

                float widthMultiplier = AppSettings.keyWidthMultipliers[i];
                keyWidth = (int)(AppSettings.keySize * widthMultiplier);

                // Create key element
                Key key = new Key(keyWidth, keyHeight, keyX, keyY);

                string keycodeString = Config.config["Keys"][configKeyName];

                // Key displayed name
                if (Config.config["Display"].ContainsKey(configKeyName))
                    key.keyText = Config.config["Display"][configKeyName];

                // Keycode
                if (!keycodeString.StartsWith("m"))
                    key.keycode = (Keyboard.Key)Enum.Parse(typeof(Keyboard.Key), keycodeString);
                else
                    key.mouseButton = (Mouse.Button)Enum.Parse(typeof(Mouse.Button), keycodeString.Substring(1));

                // Key colour
                byte[] keyRGB = new byte[3] { 255, 255, 255 };
                if (Config.config.ContainsKey("Colours"))
                {
                    if (Config.config["Colours"].ContainsKey(configKeyName))
                    {
                        string keyColourString = Config.config["Colours"][configKeyName];
                        string[] keyRGBString = keyColourString.Split(',');
                        for (int j = 0; j < 3; j++)
                            keyRGB[j] = byte.Parse(keyRGBString[j]);
                    }
                }

                key.keyColor = new Color(keyRGB[0], keyRGB[1], keyRGB[2]);

                // Position, size, etc.
                key.borderSize = AppSettings.keyBorderSize;
                key.keyTextSize = AppSettings.keyFontSize;
                key.fadeDuration = AppSettings.keyLightFadeDuration;

                // The bars and counter associated to the key.
                KeyPressBar bars = new KeyPressBar(key);

                if (Settings.font != null && AppSettings.showCounter)
                {
                    KeyCounter counter =
                        new KeyCounter
                        (
                            keyX + (key.width / 2),
                            (keyBottom + AppSettings.height) / 2,
                            key,
                            AppSettings.counterFontWidth,
                            AppSettings.counterFontSize,
                            Settings.font
                        );

                    elements.Add(counter);
                }

                elements.Add(bars);
                elements.Add(key);

                // Update position for next key
                keyX += keyWidth + AppSettings.keySpacing;
            }
        }

        static void Initialize()
        {
            Settings.bgColor = AppSettings.backgroundColour;

            Textures.InitTextures();

            elements.Clear();

            AddKeysElements();
        }

        static void CheckSettingsUpdate()
        {
            settingsUpdateMutex.WaitOne();
            bool needUpdate = settingsUpdateFlag;
            settingsUpdateFlag = false;
            settingsUpdateMutex.ReleaseMutex();

            if (!needUpdate)
                return;

            Config.configChangingMutex.WaitOne();

            AppSettings.UpdateSettingsFromConfig();

            // (Reinitialize)
            Initialize();

            Config.configChangingMutex.ReleaseMutex();

            window?.SetSize(AppSettings.width, AppSettings.height, true);

            if (window != null)
            {
                window.updateTickrate = AppSettings.tickrate;
                // window.window?.SetFramerateLimit((uint)AppSettings.framerate);
            }
        }

        static void OnConfigFileChanged(object? sender, EventArgs args)
        {
            settingsUpdateMutex.WaitOne();
            settingsUpdateFlag = true;
            settingsUpdateMutex.ReleaseMutex();
        }

        #region WindowEvents
        static void UserInit(object? sender, EventArgs args)
        {
            InitRenderStates();

            Initialize();
        }

        static void UserUpdate(object? sender, EventArgs args)
        {
            CheckSettingsUpdate();

            Utils.UpdateElements(elements);
        }

        static void UserDraw(object? sender, EventArgs args)
        {
            Utils.DrawElements(elements, window?.texture);

            if (window?.texture != null && Textures.fade != null)
                window?.texture.Draw(Textures.fade);
        }
        #endregion

        static void Main(string[] args)
        {
            Config.onConfigFileChanged += OnConfigFileChanged;

            Config.ReadConfigFile();

            AppSettings.UpdateSettingsFromConfig();

            window = new AppWindow("Key Overlay", AppSettings.width, AppSettings.height);

            window.userInit += UserInit;
            window.userUpdate += UserUpdate;
            window.userDraw += UserDraw;

            window.Run();
        }
    }
}
