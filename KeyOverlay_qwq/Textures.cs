using SFML.Graphics;

namespace Glacc.KeyOverlay_qwq
{
    internal class Textures
    {
        static Texture? fadeTexture;
        public static Sprite? fade;
        static void InitFadeTexture()
        {
            int textureWidth = AppSettings.width;
            int textureHeight = AppSettings.fadeHeight;

            int stride = textureWidth * 4;
            int pixelCount = textureWidth * textureHeight;

            byte[] pixels = new byte[pixelCount * 4];
            Span<byte> pixelsSpan = pixels;

            fadeTexture?.Dispose();
            fadeTexture = new Texture((uint)AppSettings.width, (uint)AppSettings.fadeHeight);

            byte r = AppSettings.backgroundColour[0];
            byte g = AppSettings.backgroundColour[1];
            byte b = AppSettings.backgroundColour[2];

            for (int y = 0; y < textureHeight; y++)
            {
                Span<byte> pixelsCurrentLine = pixelsSpan.Slice(y * stride, stride);

                byte alphaCurrentLine = (byte)((1f - (y / (float)textureHeight)) * 255f);

                int offsetCurrentLine = 0;

                for (int x = 0; x < textureWidth; x++)
                {
                    pixelsCurrentLine[offsetCurrentLine++] = r;
                    pixelsCurrentLine[offsetCurrentLine++] = g;
                    pixelsCurrentLine[offsetCurrentLine++] = b;
                    pixelsCurrentLine[offsetCurrentLine++] = alphaCurrentLine;
                }
            }

            fadeTexture.Update(pixels);

            fade?.Dispose();
            fade = new Sprite(fadeTexture);
        }

        public static void InitTextures()
        {
            InitFadeTexture();
        }
    }
}
