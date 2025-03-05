
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

        public static int fadeHeight = 200;

        public static int lightingRange = 64;

        public static byte[] backgroundColour = new byte[3];

        public static void CalculateSettings()
        {
            width = keySpacing + ((keySize + keySpacing) * 2);
        }
    }
}
