using UnityEngine;

namespace AHG.QuizRedux
{
    public struct Colors
    {
        public static readonly Color Green = From255(46, 204, 113);
        public static readonly Color LightRed = From255(231, 76, 60);

        public static Color From255(float r, float g, float b) => From255(r, g, b, 255);
        public static Color From255(float r, float g, float b, float a) => new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        public static string ToHex(Color color) => "#" + ColorUtility.ToHtmlStringRGBA(color);
    }
}