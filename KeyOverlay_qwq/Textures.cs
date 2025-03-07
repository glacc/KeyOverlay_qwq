// SPDX-License-Identifier: GPL-3.0-or-later

using SFML.Graphics;

namespace Glacc.KeyOverlay_qwq
{
    internal class Textures
    {
        static float CalculateFromNumAToNumB(float start, float end, float precent)
        {
            float difference = end - start;
            return start + (difference * precent);
        }

        static Texture? fadeTexture;
        public static Sprite? fade;

        public static Texture? fadeMaskTexture;

        static void InitFadeTexture(int width, int height, Color colorStartFromTop, Color colorEndToBottom, ref Texture? texture)
        {
            int stride = width * 4;
            int pixelCount = width * height;

            byte[] pixels = new byte[pixelCount * 4];
            Span<byte> pixelsSpan = pixels;

            texture?.Dispose();
            texture = new Texture((uint)width, (uint)height);

            byte rStart = colorStartFromTop.R;
            byte gStart = colorStartFromTop.G;
            byte bStart = colorStartFromTop.B;
            byte aStart = colorStartFromTop.A;

            byte rEnd = colorEndToBottom.R;
            byte gEnd = colorEndToBottom.G;
            byte bEnd = colorEndToBottom.B;
            byte aEnd = colorEndToBottom.A;

            for (int y = 0; y < height; y++)
            {
                Span<byte> pixelsCurrentLine = pixelsSpan.Slice(y * stride, stride);

                float percent = y / (float)height;

                byte rCurrentLine = (byte)CalculateFromNumAToNumB(rStart, rEnd, percent);
                byte gCurrentLine = (byte)CalculateFromNumAToNumB(gStart, gEnd, percent);
                byte bCurrentLine = (byte)CalculateFromNumAToNumB(bStart, bEnd, percent);
                byte aCurrentLine = (byte)CalculateFromNumAToNumB(aStart, aEnd, percent);

                int offsetCurrentLine = 0;

                for (int x = 0; x < width; x++)
                {
                    pixelsCurrentLine[offsetCurrentLine++] = rCurrentLine;
                    pixelsCurrentLine[offsetCurrentLine++] = gCurrentLine;
                    pixelsCurrentLine[offsetCurrentLine++] = bCurrentLine;
                    pixelsCurrentLine[offsetCurrentLine++] = aCurrentLine;
                }
            }

            texture.Update(pixels);
        }

        public static void InitTextures()
        {
            InitFadeTexture(
                AppSettings.width,
                AppSettings.fadeHeight,
                AppSettings.backgroundColour,
                new Color(AppSettings.backgroundColour.ToInteger() & 0xFFFFFF00),
                ref fadeTexture
            );
            fade?.Dispose();
            fade = new Sprite(fadeTexture);

            InitFadeTexture
            (
                100,
                AppSettings.fadeHeight,
                new Color(255, 255, 255, 0),
                new Color(255, 255, 255, 255),
                ref fadeMaskTexture
            );
        }
    }
}
