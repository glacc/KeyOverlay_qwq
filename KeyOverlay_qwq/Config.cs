
namespace Glacc.KeyOverlay_qwq
{
    // Based on https://github.com/Friedchicken-42/KeyOverlay/blob/main/Config.cs

    internal class Config
    {
        const string configFileName = "config.ini";

        static Dictionary<string, Dictionary<string, string>> configSections = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, Dictionary<string, string>> config
        {
            get => configSections;
        }

        static FileSystemWatcher fileSystemWatcher;

        public static EventHandler<EventArgs>? onConfigFileChanged = null;

        static void ReadConfigFile()
        {
            configSections.Clear();

            string[] linesOfConfigFile = File.ReadAllLines(configFileName);

            Dictionary<string, string>? currentSection = null;
            string currentSectionName = string.Empty;
            foreach (string currentLineFull in linesOfConfigFile)
            {
                currentLineFull.Trim();

                // Remove comments
                int indexOfSemicolon = currentLineFull.IndexOf(';');
                int indexOfHash = currentLineFull.IndexOf("#");

                int indexOfStartOfComment;
                if (indexOfSemicolon >= 0 && indexOfHash >= 0)
                    indexOfStartOfComment = Math.Min(indexOfSemicolon, indexOfHash);
                else if (indexOfSemicolon >= 0)
                    indexOfStartOfComment = indexOfSemicolon;
                else if (indexOfHash >= 0)
                    indexOfStartOfComment = indexOfHash;
                else
                    indexOfStartOfComment = -1;

                string currentLine;
                if (indexOfStartOfComment >= 0)
                    currentLine = currentLineFull.Substring(0, indexOfStartOfComment);
                else
                    currentLine = currentLineFull;

                // Section
                if (currentLine.StartsWith('[') && currentLine.EndsWith(']'))
                {
                    currentSectionName = currentLine.Substring(1, currentLine.Length - 2);

                    if (!configSections.ContainsKey(currentSectionName))
                    {
                        currentSection = new Dictionary<string, string>();
                        configSections.Add(currentSectionName, currentSection);
                    }
                    else
                        currentSection = configSections[currentSectionName];

                    continue;
                }

                // Keys in section
                if (currentSection == null)
                    continue;

                string[] keyAndValue = currentLine.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (keyAndValue.Length == 2)
                {
                    string key = keyAndValue[0];
                    string value = keyAndValue[1];

                    if (!currentSection.ContainsKey(key))
                        currentSection.Add(key, value);
                    else
                        currentSection[key] = value;
                }
            }
        }

        static void OnConfigFileChangedInternal(object? sender, FileSystemEventArgs args)
        {
            // Wait until the config file actually changed
            Thread.Sleep(100);

            ReadConfigFile();

            onConfigFileChanged?.Invoke(null, EventArgs.Empty);
        }

        static Config()
        {
            fileSystemWatcher = new FileSystemWatcher(configFileName);
            fileSystemWatcher.Changed += OnConfigFileChangedInternal;
        }
    }
}
