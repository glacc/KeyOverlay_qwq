using Glacc.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glacc.KeyOverlay_qwq.Elements
{
    internal class KeyCounterDigit
    {
        int m_px;
        public int px
        {
            get => m_px;
        }
        int m_py;
        public int py
        {
            get => m_py;
        }

        int m_number;
        public int number
        {
            get => m_number;
            set
            {
                AddNumberText(m_number, value);
                m_number = value;
            }
        }

        int fontSize;
        int numSpacing;
        Vector2f offsetOfNewNumber;

        bool m_moveToEnd = false;
        public bool moveToEnd
        {
            get => m_moveToEnd;
            set
            {
                m_moveToEnd = value;
                if (value)
                    AddNumberText(m_number, 0, true);
                else
                    AddNumberText(0, m_number, false);
            }
        }
        bool m_hasMovedToEnd = false;
        public bool hasMovedToEnd
        {
            get => m_hasMovedToEnd;
        }

        const float moveSpeed = 0.2f;

        Font font;

        Queue<Text> nums = new Queue<Text>();

        void AddNumberText(int oldNumber, int newNumber, bool isEmpty = false)
        {
            if (isEmpty)
            {
                newNumber = oldNumber + 1;
                if (newNumber >= 10)
                    newNumber = 0;
            }

            while (oldNumber != newNumber + 1)
            {
                if (oldNumber >= 10)
                    oldNumber = 0;

                Text newNum = new Text($"{oldNumber}", font);
                newNum.CharacterSize = (uint)fontSize;
                Utils.UpdateTextOrigins(newNum, TextAlign.Center);

                if (nums.Count == 0)
                    newNum.Position = new Vector2f(px, py);
                else
                    newNum.Position = nums.Last().Position + offsetOfNewNumber;

                nums.Enqueue(newNum);

                oldNumber++;
            }
        }

        public void Update()
        {
            if (nums.Count > 0)
            {
                float yOfLastText = nums.Last().Position.Y;
                if (MathF.Abs(yOfLastText - py) <= 0.2f)
                    m_hasMovedToEnd = true;

                float offsetY = (py - yOfLastText) * moveSpeed;

                float yToRemove = py + (numSpacing / 2);

                foreach (Text num in nums)
                    num.Position = new Vector2f(px, num.Position.Y + offsetY);

                while (nums.Count > 0)
                {
                    Text currentNum = nums.First();

                    if (currentNum.Position.Y >= yToRemove)
                        nums.Dequeue();
                    else
                        break;
                }
            }
        }

        public void Draw(RenderTarget renderTarget)
        {
            foreach (Text num in nums)
                renderTarget.Draw(num);
        }

        public KeyCounterDigit(int px, int py, int fontSize, int numSpacing, Font font, int? num = null)
        {
            m_px = px;
            m_py = py;

            this.fontSize = fontSize;
            this.numSpacing = numSpacing;

            this.font = font;

            offsetOfNewNumber = new Vector2f(0f, -numSpacing);

            if (num != null)
            {
                m_number = (int)num;

                Text newNum = new Text($"{(int)num}", font);
                newNum.CharacterSize = (uint)fontSize;
                Utils.UpdateTextOrigins(newNum, TextAlign.Center);

                newNum.Position = new Vector2f(px, py) + offsetOfNewNumber;

                nums.Enqueue(newNum);
            }
        }
    }
}
