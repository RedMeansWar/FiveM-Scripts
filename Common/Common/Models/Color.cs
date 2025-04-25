namespace Common.Models
{
    public class Color
    {
        public int R, G, B, A;

        public Color() => R = G = B = A = 255;

        public Color(int InColor, int InAlpha = 255)
        {
            R = G = B = Extensions.Clamp(InColor, 0, 255);
            A = Extensions.Clamp(InAlpha, 0, 255);
        }

        public Color(int InR, int InG, int InB, int InA = 255)
        {
            R = Extensions.Clamp(InR, 0, 255);
            G = Extensions.Clamp(InG, 0, 255);
            B = Extensions.Clamp(InB, 0, 255);
            A = Extensions.Clamp(InA, 0, 255);
        }

        public Color(float InColor, float InAlpha = 1.0f)
        {
            R = G = B = Extensions.Clamp((int)(InColor * 255), 0, 255);
            A = Extensions.Clamp((int)(InAlpha * 255), 0, 255);
        }

        public Color(float InR, float InG, float InB, float InA = 1.0f)
        {
            R = Extensions.Clamp((int)(InR * 255), 0, 255);
            G = Extensions.Clamp((int)(InG * 255), 0, 255);
            B = Extensions.Clamp((int)(InB * 255), 0, 255);
            A = Extensions.Clamp((int)(InA * 255), 0, 255);
        }

        public Color(float[] InColor, float InAlpha = 1.0f)
        {
            R = Extensions.Clamp((int)(InColor[0] * 255), 0, 255);
            G = Extensions.Clamp((int)(InColor[1] * 255), 0, 255);
            B = Extensions.Clamp((int)(InColor[2] * 255), 0, 255);
            A = Extensions.Clamp((int)(InAlpha * 255), 0, 255);
        }

        public Color(float[] InColor)
        {
            R = Extensions.Clamp((int)(InColor[0] * 255), 0, 255);
            G = Extensions.Clamp((int)(InColor[1] * 255), 0, 255);
            B = Extensions.Clamp((int)(InColor[2] * 255), 0, 255);
            A = Extensions.Clamp((int)(InColor[3] * 255), 0, 255);
        }

        public Color(Color InColor)
        {
            R = InColor.R;
            G = InColor.G;
            B = InColor.B;
            A = InColor.A;
        }

        public Color(Color InColor, int InAlpha)
        {
            R = InColor.R;
            G = InColor.G;
            B = InColor.B;
            A = Extensions.Clamp(InAlpha, 0, 255);
        }

        public Color(Color InColor, float InAlpha)
        {
            R = InColor.R;
            G = InColor.G;
            B = InColor.B;
            A = Extensions.Clamp((int)(InAlpha * 255), 0, 255);
        }

        public Color(string InColor)
        {
            if (InColor.Length == 3)
            {
                R = CharToHex(InColor[0]) * 16;
                G = CharToHex(InColor[1]) * 16;
                B = CharToHex(InColor[2]) * 16;
                A = 255;
            }
            else if (InColor.Length == 4)
            {
                R = CharToHex(InColor[0]) * 16;
                G = CharToHex(InColor[1]) * 16;
                B = CharToHex(InColor[2]) * 16;
                A = CharToHex(InColor[3]) * 16;
            }
            else if (InColor.Length == 6)
            {
                R = (CharToHex(InColor[0]) * 16) + CharToHex(InColor[1]);
                G = (CharToHex(InColor[2]) * 16) + CharToHex(InColor[3]);
                B = (CharToHex(InColor[4]) * 16) + CharToHex(InColor[5]);
                A = 255;
            }
            else if (InColor.Length == 8)
            {
                R = (CharToHex(InColor[0]) * 16) + CharToHex(InColor[1]);
                G = (CharToHex(InColor[2]) * 16) + CharToHex(InColor[3]);
                B = (CharToHex(InColor[4]) * 16) + CharToHex(InColor[5]);
                A = (CharToHex(InColor[6]) * 16) + CharToHex(InColor[7]);
            }
        }

        public int Red() => R;
        public int Green() => G;
        public int Blue() => B;
        public int Alpha() => A;
        public override string ToString() => $"R:{R}, G:{G}, B:{B}, A:{A}";

        private int CharToHex(char c)
        {
            switch (c)
            {
                case '0':
                case '2':
                case '4':
                case '6':
                case '8':
                    return 0;
                case '1':
                case '3':
                case '5':
                case '7':
                case '9':
                    return 1;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
            }
            return 0;
        }
    }
}
