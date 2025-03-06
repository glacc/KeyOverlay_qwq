using Glacc.KeyOverlay_qwq.Elements;
using Glacc.UI;
using SFML.Graphics;
using SFML.Window;

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
            int keyX = AppSettings.keySpacing;
            int keyY = AppSettings.height - AppSettings.keySpacing - AppSettings.keySize;
            for (int i = 0; i < AppSettings.keyCount; i++)
            {
                Key key = new Key(AppSettings.keySize, AppSettings.keySize, keyX, keyY);

                string configKeyName = $"Key{i + 1}";

                string keycodeString = Config.config["Keys"][configKeyName];

                // Key displayed name
                if (Config.config["Display"].ContainsKey(configKeyName))
                    key.keyText = Config.config["Display"][configKeyName];

                // Keycode
                if (!keycodeString.StartsWith("m"))
                    key.keycode = (Keyboard.Key)Enum.Parse(typeof(Keyboard.Key), keycodeString);
                else
                    key.mouseButton = (Mouse.Button)Enum.Parse(typeof(Mouse.Button), keycodeString.Substring(1));

                string keyColourString = Config.config["Colours"][configKeyName];

                // Key colour
                byte[] keyRGB = new byte[3];
                string[] keyRGBString = keyColourString.Split(',');
                for (int j = 0; j < 3; j++)
                    keyRGB[j] = byte.Parse(keyRGBString[j]);

                key.keyColor = new Color(keyRGB[0], keyRGB[1], keyRGB[2]);

                // Position, size, etc.
                key.borderSize = AppSettings.keyBorderSize;
                key.keyTextSize = AppSettings.keyFontSize;
                key.fadeDuration = AppSettings.keyLightFadeDuration;

                // Update position for next key
                keyX += AppSettings.keySize + AppSettings.keySpacing;

                // The bars and counter associated to the key.
                KeyPressBar bars = new KeyPressBar(key);

                if (Settings.font != null)
                {
                    KeyCounter counter = new KeyCounter(keyX + (key.width / 2), keyY + key.height + (AppSettings.keySpacing / 2), key, 8, 12, Settings.font);
                    elements.Add(counter);
                }

                elements.Add(bars);
                elements.Add(key);
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
            // InitRenderStates();

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
