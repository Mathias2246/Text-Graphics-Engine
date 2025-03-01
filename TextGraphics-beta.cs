/* -----------------------------------------------------------------------------------------------
 * TGE - Text Graphics Engine v1.0.1 beta-0   |   MIT License  -  Copyright (c) 2025 Mathias2246
 * https://github.com/Mathias2246/Text-Graphics-Engine/
 * -----------------------------------------------------------------------------------------------
 */

#region UsingDirectives
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static TextGraphics.ITextRenderer;
#endregion

namespace TextGraphics
{
    #region Serialization

    public interface ISerialization<T>
    {
        public abstract string Serialize();
        public static abstract T? Deserialize(string data);
    }

    public interface IInstanceSerialization<T>
    {
        public abstract string Serialize();
        public abstract void Deserialize(string data);
    }

    public class SerializationAttributes
    {
        [System.AttributeUsage(System.AttributeTargets.Field)]
        public class Ignore : Attribute { }
    }
    #endregion

    #region IntegerStructs
    /// <summary>
    /// A Rectangle that consists of two <see cref="Vector2Int"/>s
    /// </summary>
    public struct IntRect
    {
        /// <summary>
        /// The position of the top-left corner of the rectangle
        /// </summary>
        public Vector2Int position;
        /// <summary>
        /// The position of the bottom-right corner of the rectangle
        /// </summary>
        public readonly Vector2Int Corner => position + size;
        /// <summary>
        /// The size of the rectangle
        /// </summary>
        public Vector2Int size;

        /// <summary>
        /// A rectangle with integer coordinates
        /// </summary>
        /// <param name="position">The position of the top-left corner</param>
        /// <param name="size">The size of the rectangle</param>
        public IntRect(Vector2Int position, Vector2Int size)
        {
            this.position = position;
            this.size = size;
        }

        /// <summary>
        /// A rectangle with integer coordinates
        /// </summary>
        /// <param name="position">The position of the top-left corner</param>
        /// <param name="size">The size of the rectangle</param>
        public IntRect(Vector2 position, Vector2 size)
        {
            this.position = (Vector2Int)position;
            this.size = (Vector2Int)size;
        }

        /// <summary>
        /// A rectangle with integer coordinates
        /// </summary>
        /// <param name="positionX">The x-position of the top-left corner</param>
        /// <param name="positionY">The y-position of the top-left corner</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public IntRect(int positionX, int positionY, int width, int height)
        {
            position = new(positionX, positionY);
            size = new(width, height);
        }

        /// <summary>
        /// Creates a copy of the given <see cref="IntRect"/>
        /// </summary>
        /// <param name="original">The original <see cref="IntRect"/></param>
        public IntRect(IntRect original)
        {
            position = original.position;
            size = original.size;
        }

        /// <summary>
        /// Checks if a certain <see cref="Vector2Int"/> is inside the bounds of the <see cref="IntRect"/>
        /// </summary>
        /// <param name="v">The position to check</param>
        /// <returns><see langword="true"/> if, the point is inside the <see cref="IntRect"/>; otherwise <see langword="false"/></returns>
        public readonly bool IsInRect(Vector2Int v) => v.x >= position.x && v.y >= position.y && v.x < Corner.x && v.y < Corner.y;

        /// <summary>
        /// Checks if a certain <see cref="Vector2"/> is inside the bounds of the <see cref="IntRect"/>
        /// </summary>
        /// <param name="v">The position to check</param>
        /// <returns><see langword="true"/> if, the point is inside the <see cref="IntRect"/>; otherwise <see langword="false"/></returns>
        public readonly bool IsInRect(Vector2 v) => v.X >= position.x && v.Y >= position.y && v.X < Corner.x && v.Y < Corner.y;

        /// <summary>
        /// Checks if a certain point is inside the bounds of the <see cref="IntRect"/>
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <param name="y">The y position of the point</param>
        /// <returns><see langword="true"/> if, the point is inside the <see cref="IntRect"/>; otherwise <see langword="false"/></returns>
        public readonly bool IsInRect(int x, int y) => x >= position.x && y >= position.y && x < Corner.x && y < Corner.y;

        /// <summary>
        /// Checks if a certain point is inside the bounds of the <see cref="IntRect"/>
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <param name="y">The y position of the point</param>
        /// <returns><see langword="true"/> if, the point is inside the <see cref="IntRect"/>; otherwise <see langword="false"/></returns>
        public readonly bool IsInRect(float x, float y) => x >= position.x && y >= position.y && x < Corner.x && y < Corner.y;

        /// <summary>
        /// Checks if a certain point is inside the bounds of the <see cref="IntRect"/>
        /// </summary>
        /// <param name="x">The x position of the point</param>
        /// <param name="y">The y position of the point</param>
        /// <returns><see langword="true"/> if, the point is inside the <see cref="IntRect"/>; otherwise <see langword="false"/></returns>
        public readonly bool IsInRect(double x, double y) => x >= position.x && y >= position.y && x < Corner.x && y < Corner.y;

        /// <summary>
        /// Checks if a <see cref="IntRect"/> intersects with this <see cref="IntRect"/>
        /// </summary>
        /// <param name="other">The <see cref="IntRect"/> to check</param>
        /// <returns><see langword="true"/> if, the other <see cref="IntRect"/> intersects this <see cref="IntRect"/>; otherwise <see langword="false"/></returns>
        public readonly bool Intersects(IntRect other) => position.x < other.Corner.x && Corner.x >= other.position.x && position.y < other.Corner.y && Corner.y >= other.position.y;

        public override readonly string ToString() => $"from: {position} size: {size}";

        /// <summary>
        /// Creates a jagged-array of <typeparamref name="T"/>s with the size of the <see cref="IntRect"/>
        /// </summary>
        /// <typeparam name="T">The type of the grid array</typeparam>
        /// <returns>A jagged-array with the size of the current <see cref="IntRect"/></returns>
        public readonly T[][] CreateGridArray<T>()
        {
            T[][] t = new T[size.x][];
            Array.Fill(t, new T[size.y]);

            return t;
        }
    }

    /// <summary>
    /// A two-dimensional Vector that consists of two integers
    /// </summary>
    /// <param name="x">The x value of the <see cref="Vector2Int"/></param>
    /// <param name="y">The y value of the <see cref="Vector2Int"/></param>
    public struct Vector2Int(int x, int y)
    {
        public override readonly string ToString() => $"{x} {y}";

        public int x = x;
        public int y = y;

        /// <summary>
        /// The length of the <see cref="Vector2Int"/>
        /// </summary>
        public readonly float Length => MathF.Sqrt(x * x + y * y);

        /// <summary>
        /// Normalizes this <see cref="Vector2Int"/> to a length of 1
        /// </summary>
        public void Normalize()
        {
            float length = Length;
            x = (int)(x / length);
            y = (int)(y / length);
        }

        /// <summary>
        /// Calculates the Distance between this <see cref="Vector2Int"/> and a <see cref="Vector2"/>
        /// </summary>
        /// <param name="other">The other coordinate</param>
        /// <returns>The distance between the two <see cref="Vector2Int"/>s</returns>
        public readonly float Distance(Vector2 other) => Vector2.Distance(this, other);

        /// <summary>
        /// Calculates the Distance between this <see cref="Vector2Int"/> and another <see cref="Vector2Int"/>
        /// </summary>
        /// <param name="other">The other coordinate</param>
        /// <returns>The distance between the two <see cref="Vector2Int"/>s</returns>
        public readonly float Distance(Vector2Int other) => Vector2.Distance(this, other);

        /// <summary>
        /// A <see cref="Vector2Int"/> with the values 0, 0
        /// </summary>
        public static readonly Vector2Int Zero = new(0, 0);

        /// <summary>
        /// A <see cref="Vector2Int"/> with the values 1, 0
        /// </summary>
        public static readonly Vector2Int UnitX = new(1, 0);

        /// <summary>
        /// A <see cref="Vector2Int"/> with the values 0, 1
        /// </summary>
        public static readonly Vector2Int UnitY = new(0, 1);

        public static implicit operator Vector2(Vector2Int v) => new(v.x, v.y);
        public static explicit operator Vector2Int(Vector2 v) => new((int)v.X, (int)v.Y);

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2) => new(v1.x + v2.x, v1.y + v2.y);
        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2) => new(v1.x - v2.x, v1.y - v2.y);
        public static Vector2Int operator *(Vector2Int v1, Vector2Int v2) => new(v1.x * v2.x, v1.y * v2.y);
        public static Vector2Int operator /(Vector2Int v1, Vector2Int v2) => new(v1.x / v2.x, v1.y / v2.y);

        public static Vector2Int operator +(Vector2Int v1, int v2) => new(v1.x + v2, v1.y + v2);
        public static Vector2Int operator -(Vector2Int v1, int v2) => new(v1.x - v2, v1.y - v2);
        public static Vector2Int operator *(Vector2Int v1, int v2) => new(v1.x * v2, v1.y * v2);
        public static Vector2Int operator /(Vector2Int v1, int v2) => new(v1.x / v2, v1.y / v2);
    }

    /// <summary>
    /// A range out of two integers
    /// </summary>
    public struct IntRange
    {
        /// <summary>
        /// The start value of the <see cref="IntRange"/>
        /// </summary>
        public int Start;
        /// <summary>
        /// The end value of the <see cref="IntRange"/>
        /// </summary>
        public int End;
        /// <summary>
        /// Returns <see langword="true"/> if the given value is inside the range
        /// </summary>
        /// <returns><see langword="true"/> if x is inside the range</returns>
        public readonly bool InRange(int x) => Start <= x && x <= End;
        /// <summary>
        /// Returns <see langword="true"/> if the given value is inside the range
        /// </summary>
        /// <returns><see langword="true"/> if x is inside the range</returns>
        public readonly bool InRange(float x) => Start <= x && x <= End;
        /// <summary>
        /// Returns <see langword="true"/> if the given value is inside the range
        /// </summary>
        /// <returns><see langword="true"/> if x is inside the range</returns>
        public readonly bool InRange(double x) => Start <= x && x <= End;
        /// <summary>
        /// Returns <see langword="true"/> if the given value is inside the range
        /// </summary>
        /// <returns><see langword="true"/> if x is inside the range</returns>
        public readonly bool InRange(long x) => Start <= x && x <= End;
        /// <summary>
        /// Returns <see langword="true"/> if the given value is inside the range
        /// </summary>
        /// <returns><see langword="true"/> if x is inside the range</returns>
        public readonly bool InRange(short x) => Start <= x && x <= End;
        /// <summary>
        /// Returns <see langword="true"/> if the given value is inside the range
        /// </summary>
        /// <returns><see langword="true"/> if x is inside the range</returns>
        public readonly bool InRange(byte x) => Start <= x && x <= End;

        /// <summary>
        /// Checks if this <see cref="IntRange"/> intersects with another <see cref="IntRange"/>
        /// </summary>
        /// <param name="other">The <see cref="IntRange"/> to check</param>
        /// <returns><see langword="true"/> if, the two <see cref="IntRange"/>s intersect; otherwise <see langword="false"/></returns>
        public readonly bool Intersects(IntRange other) => Start < other.End && End >= other.Start;
    }

    #endregion

    #region ExtensionClasses
    /// <summary>
    /// Contains <see cref="Vector"/> extension methods
    /// </summary>
    public static class VectorExt
    {
        /// <summary>
        /// Rotates a <see cref="Vector2"/> around the origin
        /// </summary>
        /// <param name="point">The point to rotate</param>
        /// <param name="origin">The origin to rotate around</param>
        /// <param name="angle">The angle to rotate</param>
        /// <returns></returns>
        public static Vector2 RotateAboutOrigin(this Vector2 point, Vector2 origin, float angle) => Vector2.Transform(point - origin, Matrix3x2.CreateRotation(angle.DegToRad())) + origin;

        /// <summary>
        /// Uses <see cref="MathF.Floor(float)"/> to round the <see cref="Vector2"/> values to a <see cref="Vector2Int"/>
        /// </summary>
        /// <param name="vector">The original <see cref="Vector2"/></param>
        /// <returns>The rounded <see cref="Vector2Int"/></returns>
        public static Vector2Int FloorToVector2Int(this Vector2 vector) => new((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));

        /// <summary>
        /// Uses <see cref="MathF.Ceiling(float)"/> to round the <see cref="Vector2"/> values to a <see cref="Vector2Int"/>
        /// </summary>
        /// <param name="vector">The original <see cref="Vector2"/></param>
        /// <returns>The rounded <see cref="Vector2Int"/></returns>
        public static Vector2Int CeilingToVector2Int(this Vector2 vector) => new((int)MathF.Ceiling(vector.X), (int)MathF.Ceiling(vector.Y));
    }

    /// <summary>
    /// Contains <see langword="float"/> extension methods
    /// </summary>
    public static class FloatExt
    {
        private const float dtrf = MathF.PI / 180;

        /// <summary>
        /// Converts Degrees to Radians
        /// </summary>
        /// <param name="angle">The angle to convert</param>
        /// <returns>The angle as radians</returns>
        public static float DegToRad(this float angle) => dtrf * angle;
    }

    #endregion

    #region TextRendering
    /// <summary>
    /// Colored text character
    /// </summary>
    public partial class TextColor
    {
        private char c;

        /// <summary>
        /// Gets/Sets the character of this <see cref="TextColor"/>
        /// </summary>
        [JsonIgnore]
        public char Character { get => c; set { c = value; _character = (byte)value; } }
        [JsonIgnore]
        private byte _character;

        /// <summary>
        /// The foreground color of the character
        /// </summary>
        public RGBA fgColor;
        /// <summary>
        /// The background color of the character
        /// </summary>
        public RGBA bgColor;

        /// <summary>
        /// The encoded version of this <see cref="TextColor"/> using the default encoding from the windows console
        /// </summary>
        [JsonIgnore]
        public byte[] Encoded => [27, 91, 51, 56, 59, 50, 59, _bc[fgColor.red][0], _bc[fgColor.red][1], _bc[fgColor.red][2], 59, _bc[fgColor.green][0], _bc[fgColor.green][1], _bc[fgColor.green][2], 59, _bc[fgColor.blue][0], _bc[fgColor.blue][1], _bc[fgColor.blue][2], 109, 27, 91, 52, 56, 59, 50, 59, _bc[bgColor.red][0], _bc[bgColor.red][1], _bc[bgColor.red][2], 59, _bc[bgColor.green][0], _bc[bgColor.green][1], _bc[bgColor.green][2], 59, _bc[bgColor.blue][0], _bc[bgColor.blue][1], _bc[bgColor.blue][2], 109, _character, 27, 91, 48, 109];

        public override string ToString() => $"\x1B[38;2;{fgColor.red};{fgColor.green};{fgColor.blue}m\x1B[48;2;{bgColor.red};{bgColor.green};{bgColor.blue}m{Character}\u001b[0m";

        private static readonly byte[][] _bc = [[48, 48, 48,], [48, 48, 49,], [48, 48, 50,], [48, 48, 51,], [48, 48, 52,], [48, 48, 53,], [48, 48, 54,], [48, 48, 55,], [48, 48, 56,], [48, 48, 57,], [48, 49, 48,], [48, 49, 49,], [48, 49, 50,], [48, 49, 51,], [48, 49, 52,], [48, 49, 53,], [48, 49, 54,], [48, 49, 55,], [48, 49, 56,], [48, 49, 57,], [48, 50, 48,], [48, 50, 49,], [48, 50, 50,], [48, 50, 51,], [48, 50, 52,], [48, 50, 53,], [48, 50, 54,], [48, 50, 55,], [48, 50, 56,], [48, 50, 57,], [48, 51, 48,], [48, 51, 49,], [48, 51, 50,], [48, 51, 51,], [48, 51, 52,], [48, 51, 53,], [48, 51, 54,], [48, 51, 55,], [48, 51, 56,], [48, 51, 57,], [48, 52, 48,], [48, 52, 49,], [48, 52, 50,], [48, 52, 51,], [48, 52, 52,], [48, 52, 53,], [48, 52, 54,], [48, 52, 55,], [48, 52, 56,], [48, 52, 57,], [48, 53, 48,], [48, 53, 49,], [48, 53, 50,], [48, 53, 51,], [48, 53, 52,], [48, 53, 53,], [48, 53, 54,], [48, 53, 55,], [48, 53, 56,], [48, 53, 57,], [48, 54, 48,], [48, 54, 49,], [48, 54, 50,], [48, 54, 51,], [48, 54, 52,], [48, 54, 53,], [48, 54, 54,], [48, 54, 55,], [48, 54, 56,], [48, 54, 57,], [48, 55, 48,], [48, 55, 49,], [48, 55, 50,], [48, 55, 51,], [48, 55, 52,], [48, 55, 53,], [48, 55, 54,], [48, 55, 55,], [48, 55, 56,], [48, 55, 57,], [48, 56, 48,], [48, 56, 49,], [48, 56, 50,], [48, 56, 51,], [48, 56, 52,], [48, 56, 53,], [48, 56, 54,], [48, 56, 55,], [48, 56, 56,], [48, 56, 57,], [48, 57, 48,], [48, 57, 49,], [48, 57, 50,], [48, 57, 51,], [48, 57, 52,], [48, 57, 53,], [48, 57, 54,], [48, 57, 55,], [48, 57, 56,], [48, 57, 57,], [49, 48, 48,], [49, 48, 49,], [49, 48, 50,], [49, 48, 51,], [49, 48, 52,], [49, 48, 53,], [49, 48, 54,], [49, 48, 55,], [49, 48, 56,], [49, 48, 57,], [49, 49, 48,], [49, 49, 49,], [49, 49, 50,], [49, 49, 51,], [49, 49, 52,], [49, 49, 53,], [49, 49, 54,], [49, 49, 55,], [49, 49, 56,], [49, 49, 57,], [49, 50, 48,], [49, 50, 49,], [49, 50, 50,], [49, 50, 51,], [49, 50, 52,], [49, 50, 53,], [49, 50, 54,], [49, 50, 55,], [49, 50, 56,], [49, 50, 57,], [49, 51, 48,], [49, 51, 49,], [49, 51, 50,], [49, 51, 51,], [49, 51, 52,], [49, 51, 53,], [49, 51, 54,], [49, 51, 55,], [49, 51, 56,], [49, 51, 57,], [49, 52, 48,], [49, 52, 49,], [49, 52, 50,], [49, 52, 51,], [49, 52, 52,], [49, 52, 53,], [49, 52, 54,], [49, 52, 55,], [49, 52, 56,], [49, 52, 57,], [49, 53, 48,], [49, 53, 49,], [49, 53, 50,], [49, 53, 51,], [49, 53, 52,], [49, 53, 53,], [49, 53, 54,], [49, 53, 55,], [49, 53, 56,], [49, 53, 57,], [49, 54, 48,], [49, 54, 49,], [49, 54, 50,], [49, 54, 51,], [49, 54, 52,], [49, 54, 53,], [49, 54, 54,], [49, 54, 55,], [49, 54, 56,], [49, 54, 57,], [49, 55, 48,], [49, 55, 49,], [49, 55, 50,], [49, 55, 51,], [49, 55, 52,], [49, 55, 53,], [49, 55, 54,], [49, 55, 55,], [49, 55, 56,], [49, 55, 57,], [49, 56, 48,], [49, 56, 49,], [49, 56, 50,], [49, 56, 51,], [49, 56, 52,], [49, 56, 53,], [49, 56, 54,], [49, 56, 55,], [49, 56, 56,], [49, 56, 57,], [49, 57, 48,], [49, 57, 49,], [49, 57, 50,], [49, 57, 51,], [49, 57, 52,], [49, 57, 53,], [49, 57, 54,], [49, 57, 55,], [49, 57, 56,], [49, 57, 57,], [50, 48, 48,], [50, 48, 49,], [50, 48, 50,], [50, 48, 51,], [50, 48, 52,], [50, 48, 53,], [50, 48, 54,], [50, 48, 55,], [50, 48, 56,], [50, 48, 57,], [50, 49, 48,], [50, 49, 49,], [50, 49, 50,], [50, 49, 51,], [50, 49, 52,], [50, 49, 53,], [50, 49, 54,], [50, 49, 55,], [50, 49, 56,], [50, 49, 57,], [50, 50, 48,], [50, 50, 49,], [50, 50, 50,], [50, 50, 51,], [50, 50, 52,], [50, 50, 53,], [50, 50, 54,], [50, 50, 55,], [50, 50, 56,], [50, 50, 57,], [50, 51, 48,], [50, 51, 49,], [50, 51, 50,], [50, 51, 51,], [50, 51, 52,], [50, 51, 53,], [50, 51, 54,], [50, 51, 55,], [50, 51, 56,], [50, 51, 57,], [50, 52, 48,], [50, 52, 49,], [50, 52, 50,], [50, 52, 51,], [50, 52, 52,], [50, 52, 53,], [50, 52, 54,], [50, 52, 55,], [50, 52, 56,], [50, 52, 57,], [50, 53, 48,], [50, 53, 49,], [50, 53, 50,], [50, 53, 51,], [50, 53, 52,], [50, 53, 53,],];


        /// <summary>
        /// A RGB-Color
        /// </summary>
        public struct RGB
        {
            public byte red;
            public byte green;
            public byte blue;

            public RGB(byte red, byte green, byte blue)
            {
                this.red = red;
                this.green = green;
                this.blue = blue;
            }

            /// <summary>
            /// Creates a <see cref="RGB"/> color from three <see langword="float"/> values between 0 and 1
            ///<br/>The values will be multiplied by 255 to get the <see langword="byte"/> values
            /// </summary>
            /// <param name="red">The red color from 0 to 1</param>
            /// <param name="green">The green color from 0 to 1</param>
            /// <param name="blue">The blue color from 0 to 1</param>
            public RGB(float red, float green, float blue)
            {
                this.red = (byte)(red * 255);
                this.green = (byte)(green * 255);
                this.blue = (byte)(blue * 255);
            }

            /// <summary>
            /// Creates a <see cref="RGB"/> color from a hexadecimal RGB <see cref="string"/>
            /// </summary>
            /// <param name="hexRgbColor">Hexadecimal <see cref="string"/> with the format '#RRGGBB'</param>
            public RGB(string hexRgbColor) => this = (RGB)hexRgbColor;

            /// <summary>
            /// Returns the hexadecimal <see cref="string"/> representation of this <see cref="RGB"/> color with the format '#RRGGBB'
            /// </summary>
            /// <returns><see cref="string"/> representation of this <see cref="RGB"/> color with the format '#RRGGBB'</returns>
            public override readonly string ToString() => $"#{red:X2}{green:X2}{blue:X2}";

            /// <summary>
            /// Turn a hexadecimal <see cref="string"/> with the format '#RRGGBB' to a <see cref="RGB"/> color
            /// </summary>
            /// <param name="hexRgbColor">Hexadecimal <see cref="string"/> with the format '#RRGGBB'</param>
            public static implicit operator RGB(string hexRgbColor) => new(
                byte.Parse(hexRgbColor.Substring(1, 2), NumberStyles.HexNumber),
                byte.Parse(hexRgbColor.Substring(3, 2), NumberStyles.HexNumber),
                byte.Parse(hexRgbColor.Substring(5, 2), NumberStyles.HexNumber));

            public static explicit operator RGB(RGBA rgba) => new(rgba.red, rgba.green, rgba.blue);
            public static implicit operator RGBA(RGB rgb) => new(rgb.red, rgb.green, rgb.blue, 255);
        }

        /// <summary>
        /// A RGBA-32-Color
        /// </summary>
        public struct RGBA
        {
            public byte red;
            public byte green;
            public byte blue;
            /// <summary>
            /// The opacity of the color
            /// </summary>
            public byte alpha;

            /// <summary>
            /// Creates a <see cref="RGBA"/> color from four <see langword="byte"/> values
            /// </summary>
            /// <param name="red">The red color from 0 to 255</param>
            /// <param name="green">The green color from 0 to 255</param>
            /// <param name="blue">The blue color from 0 to 255</param>
            /// <param name="alpha">The alpha value from 0 to 255</param>
            public RGBA(byte red, byte green, byte blue, byte alpha)
            {
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.alpha = alpha;
            }

            /// <summary>
            /// Creates a <see cref="RGBA"/> color from four <see langword="float"/> values between 0 and 1
            ///<br/>The values will be multiplied by 255 to get the <see langword="byte"/> values
            /// </summary>
            /// <param name="red">The red color from 0 to 1</param>
            /// <param name="green">The green color from 0 to 1</param>
            /// <param name="blue">The blue color from 0 to 1</param>
            /// <param name="alpha">The alpha value from 0 to 1</param>
            public RGBA(float red, float green, float blue, float alpha)
            {
                this.red = (byte)(red * 255);
                this.green = (byte)(green * 255);
                this.blue = (byte)(blue * 255);
                this.alpha = (byte)(alpha * 255);
            }

            /// <summary>
            /// Creates a <see cref="RGBA"/> color from a hexadecimal RGBA-32 <see cref="string"/>
            /// </summary>
            /// <param name="hexRgbaColor">Hexadecimal <see cref="string"/> with the format '#RRGGBBAA'</param>
            public RGBA(string hexRgbaColor) => this = (RGBA)hexRgbaColor;

            /// <summary>
            /// Turn a hexadecimal <see cref="string"/> with the format '#RRGGBBAA' to a <see cref="RGBA"/> color
            /// </summary>
            /// <param name="hexRgbaColor">Hexadecimal <see cref="string"/> with the format '#RRGGBBAA'</param>
            public static implicit operator RGBA(string hexRgbaColor) => new(
                byte.Parse(hexRgbaColor.Substring(1, 2), NumberStyles.HexNumber),
                byte.Parse(hexRgbaColor.Substring(3, 2), NumberStyles.HexNumber),
                byte.Parse(hexRgbaColor.Substring(5, 2), NumberStyles.HexNumber),
                byte.Parse(hexRgbaColor.Substring(7, 2), NumberStyles.HexNumber));

            /// <summary>
            /// Returns the hexadecimal <see cref="string"/> representation of this <see cref="RGBA"/> color with the format '#RRGGBBAA'
            /// </summary>
            /// <returns><see cref="string"/> representation of this <see cref="RGBA"/> color with the format '#RRGGBBAA'</returns>
            public override readonly string ToString() => $"#{red:X2}{green:X2}{blue:X2}{alpha:X2}";
        }

        /// <summary>
        /// Creates a Color Code with the given RGB colors
        /// </summary>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        /// <param name="background">If <see langword="true"/>, the background color will be changed instead of the foreground color</param>
        /// <returns>A Color Code from the given RGB values</returns>
        public static string FromColor(byte red, byte green, byte blue, bool background = false) => !background ? $"\x1B[38;2;{red};{green};{blue}m" : $"\x1B[48;2;{red};{green};{blue}m";

        /// <summary>
        /// Creates a Color Code with the given <see cref="RGB"/> colors
        /// </summary>
        /// <param name="rgb">The <see cref="RGB"/> color</param>
        /// <param name="background">If <see langword="true"/>, the background color will be changed instead of the foreground color</param>
        /// <returns>A Color Code from the given <see cref="RGB"/> values</returns>
        public static string FromColor(RGB rgb, bool background = false) => !background ? $"\x1B[38;2;{rgb.red};{rgb.green};{rgb.blue}m" : $"←[48;2;{rgb.red};{rgb.green};{rgb.blue}m";

        /// <summary>
        /// Creates a Color Code with the given <see cref="RGBA"/> colors<br/>
        /// The alpha component will be ignored
        /// </summary>
        /// <param name="rgb">The <see cref="RGBA"/> color</param>
        /// <param name="background">If <see langword="true"/>, the background color will be changed instead of the foreground color</param>
        /// <returns>A Color Code from the given <see cref="RGBA"/> values</returns>
        public static string FromColor(RGBA rgb, bool background = false) => !background ? $"\x1B[38;2;{rgb.red};{rgb.green};{rgb.blue}m" : $"←[48;2;{rgb.red};{rgb.green};{rgb.blue}m";

        [GeneratedRegex(@"←\[48;2;(?<red>\d+);(?<green>\d+);(?<blue>\d+)m")]
        private static partial Regex BgColorSeqRegex();

        /*[GeneratedRegex(@"←\[38;2;(?<red>\d+);(?<green>\d+);(?<blue>\d+)m")]
        private static partial Regex FgColorSeqRegex();*/

        /// <summary>
        /// Trys to get the current background color inside the given string
        /// </summary>
        /// <param name="text">The <see langword="string"/> to search in</param>
        /// <returns>A <see cref="RGBA"/> color with the found RGB values and an alpha value of 255</returns>
        public static RGBA GetBackgroundColorFromSequence(string text)
        {
            var r = BgColorSeqRegex().Match(text);
            return r.Success
                ? new RGBA(
                    byte.Parse(r.Groups["red"].Value),
                    byte.Parse(r.Groups["green"].Value),
                    byte.Parse(r.Groups["blue"].Value),
                    255)
                : new RGBA(0, 0, 0, 255);
        }

        /// <summary>
        /// Blends two <see cref="RGBA"/> colors<br/>
        /// Should be used for blending colors with alpha values
        /// </summary>
        /// <param name="foreground">The foreground color</param>
        /// <param name="background">The background color</param>
        /// <returns>A new blended color</returns>
        public static RGBA AlphaBlend(RGBA foreground, RGBA background)
        {
            // If fully opaque, return the foreground color
            if (foreground.alpha == 255) return foreground;
            // If fully transparent, return the background color
            if (foreground.alpha == 0) return background;

            // Normalize alpha values to [0, 1] range
            float alphaF = foreground.alpha / 255f;
            float alphaB = background.alpha / 255f;

            // Calculate the resulting alpha
            float outAlpha = alphaF + alphaB * (1 - alphaF);

            // If fully transparent, return a transparent color
            //if (outAlpha == 0) return new RGBA(0, 0, 0, 0);

            // Compute the resulting color components
            float invOutAlpha = 1 / outAlpha;
            byte outR = (byte)((foreground.red * alphaF + background.red * alphaB * (1 - alphaF)) * invOutAlpha);
            byte outG = (byte)((foreground.green * alphaF + background.green * alphaB * (1 - alphaF)) * invOutAlpha);
            byte outB = (byte)((foreground.blue * alphaF + background.blue * alphaB * (1 - alphaF)) * invOutAlpha);
            byte outA = (byte)(outAlpha * 255);
            return new RGBA(outR, outG, outB, outA);
        }

        /// <summary>
        /// Multiplies two <see cref="RGBA"/> colors<br/>
        /// Should be a faster alternative to <see cref="AlphaBlend(RGBA, RGBA)"/> but also less accurate
        /// </summary>
        /// <param name="foreground">The foreground color</param>
        /// <param name="background">The background color</param>
        /// <returns>A new multiplied color</returns>
        public static RGBA AlphaMultiply(RGBA foreground, RGBA background)
        {
            // If fully opaque, return the foreground color
            if (foreground.alpha == 255) return foreground;
            // If fully transparent, return the background color
            if (foreground.alpha == 0) return background;

            // Normalize alpha values to [0, 1] range
            float alphaF = foreground.alpha / 255f;
            float alphaB = background.alpha / 255f;

            return new RGBA(
                (byte)(foreground.red * alphaF * background.red * alphaB),
                (byte)(foreground.green * alphaF * background.green * alphaB),
                (byte)(foreground.blue * alphaF * background.blue * alphaB),
                (byte)(alphaF * alphaB * 255));
        }

        /// <summary>
        /// A space with a white foreground and black background
        /// </summary>
        public static readonly TextColor Empty = new(' ', new(255, 255, 255, 255), new(0, 0, 0, 255));

        /// <summary>
        /// A space with a white foreground and black background
        /// </summary>
        public const string EmptyString = "\u001b[38;2;255;255;255m\u001b[48;2;0;0;0m \u001b[0m";

        /// <summary>
        /// An encoded space with a white foreground and black background
        /// </summary>
        public static readonly byte[] encodedEmptyString = Empty.Encoded;

        /// <summary>
        /// Sets the background color of the console to black
        /// </summary>
        public const string blackBg = "\x1b[48;2;0;0;0m";

        public const string reset = "\x1b[0m";
        public const string black = "\u001b[30m";
        public const string light_red = "\u001b[91m";
        public const string red = "\u001b[31m";
        public const string green = "\u001b[32m";
        public const string white = "\u001b[97m";
        public const string underlined = "\u001b[4m";
        public const string bold = "\u001b[1m";
        public const string no_underline = "\u001b[24m";
        public const string light_blue = "\u001b[94m";
        public const string blue = "\u001b[34m";
        public const string cyan = "\u001b[36m";
        public const string gray = "\u001b[37m";
        public const string yellow = "\u001b[93m";
        public const string magenta = "\u001b[95m";

        /// <param name="character">The character to show</param>
        /// <param name="fgcolor">The foreground color of the character</param>
        /// <param name="bgcolor">The background color of the character</param>
        public TextColor(char character, RGBA fgcolor, RGBA bgcolor)
        {
            c = character;
            _character = (byte)character;
            fgColor = fgcolor;
            bgColor = bgcolor;
        }

        /// <param name="character">The character to show</param>
        /// <param name="fgcolor">The foreground color of the character</param>
        /// <param name="bgcolor">The background color of the character</param>
        public TextColor(byte character, RGBA fgcolor, RGBA bgcolor)
        {
            _character = character;
            c = (char)character;
            fgColor = fgcolor;
            bgColor = bgcolor;
        }

        /// <summary>
        /// Creates a copy of the given <see cref="TextColor"/>
        /// </summary>
        /// <param name="original">The original <see cref="TextColor"/></param>
        public TextColor(TextColor original)
        {
            c = original.c;
            _character = original._character;
            fgColor = original.fgColor;
            bgColor = original.bgColor;
        }
    }


    /// <summary>
    /// A Object that contains all of the necessary data and methods
    /// to render a grid of text
    /// </summary>
    public class RenderObject
    {
        /// <summary>
        /// A grid of strings that is used for rendering.<br/>
        /// Acts like a 2D Image in normal rendering
        /// </summary>
        public class TextGrid
        {
            /// <summary>
            /// A grid of <see cref="TextColor"/>"s that is used for rendering<br/>
            /// If a row or column is <see langword="null"/>, it will be ignored
            /// </summary>
            public TextColor?[]?[] Grid;

            private static readonly Dictionary<int, TextGrid> textGrids = [];

            private static readonly Dictionary<string, int> IdIndex = [];

            /// <summary>
            /// Disposes a <see cref="TextGrid"/> by its id
            /// </summary>
            /// <param name="gridId">The id of the grid</param>
            public static void DisposeGrid(string gridId)
            {
                if (IdIndex.Remove<string, int>(gridId, out int val)) textGrids.Remove(val);
            }

            /// <summary>
            /// Disposes a <see cref="TextGrid"/>
            /// </summary>
            public void DisposeGrid()
            {
                IdIndex.Remove(Id); 
                textGrids.Remove(Index);
#pragma warning disable CS8625
                Grid = null;
#pragma warning restore CS8625
            }

            /// <summary>
            /// An empty <see cref="TextGrid"/>.<br/>
            /// Should be used when no grid is needed
            /// </summary>
            public static readonly TextGrid Empty = new("empty", Array.Empty<TextColor[]>(), 0, 0);

            /// <summary>
            /// The global index of this <see cref="TextGrid"/>
            /// </summary>
            public readonly int Index;

            /// <summary>
            /// Gets the global index of a <see cref="TextGrid"/> by its id
            /// </summary>
            /// <param name="id">The id of the grid</param>
            /// <returns>Index of the <see cref="TextGrid"/></returns>
            public static int GetIndex(string id) => IdIndex[id];

            /// <summary>
            /// Returns the global <see cref="TextGrid"/> instance by its index
            /// </summary>
            /// <param name="index">The index of the <see cref="TextGrid"/></param>
            /// <returns>The global <see cref="TextGrid"/> instance</returns>
            public static TextGrid Get(int index) => textGrids[index];

            /// <summary>
            /// Returns the global <see cref="TextGrid"/> instance by its id
            /// </summary>
            /// <param name="id">The id of the <see cref="TextGrid"/></param>
            /// <returns>The global <see cref="TextGrid"/> instance</returns>
            public static TextGrid Get(string id) => textGrids[GetIndex(id)];

            /// <summary>
            /// Creates a new <see cref="TextGrid"/> instance from this <see cref="TextGrid"/>
            /// </summary>
            /// <param name="cloneId">The new id of the clone</param>
            public TextGrid Clone(string cloneId) => new TextGrid(cloneId, Grid, Size);

            /// <summary>
            /// The global id of this <see cref="TextGrid"/>
            /// </summary>
            public readonly string Id;

            /// <summary>
            /// Creates a new <see cref="TextGrid"/> instance from a jagged-array of <see cref="TextColor"/>s
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGrid">A jagged-array of <see cref="TextColor"/>s</param>
            /// <param name="rect">The size of the grid</param>
            public TextGrid(string id, TextColor?[]?[] textGrid, IntRect rect)
            {
                Size = rect.size;
                Grid = textGrid;
                this.Id = id;
                Index = GetHashCode();
                IdIndex[id] = Index;
                textGrids.Add(Index, this);
            }

            /// <summary>
            /// Creates a new <see cref="TextGrid"/> instance from a jagged-array of <see cref="TextColor"/>s
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGrid">A jagged-array of <see cref="TextColor"/>s</param>
            /// <param name="size">The size of the grid</param>
            public TextGrid(string id, TextColor?[]?[] textGrid, Vector2Int size)
            {
                Size = size;
                Grid = textGrid;
                this.Id = id;
                Index = GetHashCode();
                IdIndex[id] = Index;
                textGrids.Add(Index, this);
            }

            /// <summary>
            /// Creates a new <see cref="TextGrid"/> instance from a jagged-array of <see cref="TextColor"/>s
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGrid">A jagged-array of <see cref="TextColor"/>s</param>
            /// <param name="width">The width of the grid</param>
            /// <param name="height">The height of the grid</param>
            public TextGrid(string id, TextColor?[]?[] textGrid, int width, int height)
            {
                Size = new(width, height);
                Grid = textGrid;
                this.Id = id;
                Index = GetHashCode();
                IdIndex[id] = Index;
                textGrids.Add(Index, this);
            }

            /// <summary>
            /// The size of the grid
            /// </summary>
            public readonly Vector2Int Size;

            /// <summary>
            /// Creates a <see cref="TextGrid"/> from a grid dictionary
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGridDict">A text grid dictionary</param>
            /// <param name="rect">The size of the grid</param>
            public TextGrid(string id, Dictionary<Vector2Int, TextColor> textGridDict, IntRect rect)
            {
                Size = rect.size;
                Grid = new TextColor?[]?[rect.size.x];
                for (int x = 0; x < rect.size.x; x++)
                {
                    Grid[x] = new TextColor?[rect.size.y];
                    for (int y = 0; y < rect.size.y; y++)
                    {
                        Vector2Int c = new(x, y);
#pragma warning disable CS8602
                        if (textGridDict.TryGetValue(c, out TextColor? v)) Grid[x][y] = v;
#pragma warning restore CS8602
                    }
                }

                Index = GetHashCode();
                Id = id;
                IdIndex[id] = Index;
                textGrids.Add(Index, this);
            }

            /// <summary>
            /// Creates a <see cref="TextGrid"/> from a grid dictionary
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGridDict">A text grid dictionary</param>
            /// <param name="size">The size of the grid</param>
            public TextGrid(string id, Dictionary<Vector2Int, TextColor> textGridDict, Vector2Int size)
            {
                Size = size;
                Grid = new TextColor?[]?[size.x];
                for (int x = 0; x < size.x; x++)
                {
                    Grid[x] = new TextColor?[size.y];
                    for (int y = 0; y < size.y; y++)
                    {
                        Vector2Int c = new(x, y);
#pragma warning disable CS8602
                        if (textGridDict.TryGetValue(c, out TextColor? v)) Grid[x][y] = v;
#pragma warning restore CS8602
                    }
                }

                Index = GetHashCode();
                Id = id;
                IdIndex[id] = Index;
                textGrids.Add(Index, this);
            }

            /// <summary>
            /// Creates a <see cref="TextGrid"/> from a grid dictionary
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGridDict">A text grid dictionary</param>
            /// <param name="width">The width of the grid</param>
            /// <param name="height">The height of the grid</param>
            public TextGrid(string id, Dictionary<Vector2Int, TextColor> textGridDict, int width, int height)
            {
                Size = new(width, height);
                Grid = new TextColor?[]?[Size.x];
                for (int x = 0; x < Size.x; x++)
                {
                    Grid[x] = new TextColor?[Size.y];
                    for (int y = 0; y < Size.y; y++)
                    {
                        Vector2Int c = new(x, y);
#pragma warning disable CS8602
                        if (textGridDict.TryGetValue(c, out TextColor? v)) Grid[x][y] = v;
#pragma warning restore CS8602
                    }
                }

                Index = GetHashCode();
                Id = id;
                IdIndex[id] = Index;
                textGrids.Add(Index, this);
            }
        }


        /// <summary>
        /// A <see cref="Transform"/> that can be used to transform a <see cref="TextGrid"/>
        /// </summary>
        public class Transform// : ISerialization<Transform>
        {
            /// <summary>
            /// Creates a clone of the given <see cref="Transform"/>
            /// </summary>
            /// <param name="textTransform">The original <see cref="Transform"/></param>
            /// <param name="rotationLocked">If <see langword="true"/>, the rotation matrix will be locked to the current angle</param>
            public Transform(Transform textTransform, bool rotationLocked = false)
            {
                Position = textTransform.Position;
                Parent = textTransform.Parent;
                Rotation = textTransform.Rotation;
                RotationPivot = textTransform.RotationPivot;
                IsRotationLocked = rotationLocked;
            }

            /// <summary>
            /// A new Transform with the given parameters
            /// </summary>
            /// <param name="position">The position</param>
            /// <param name="rotationPivot">The pivot point that the object rotates around</param>
            /// <param name="rotation">The rotation of the object in degrees</param>
            /// <param name="parent">The optional parent of this transform</param>
            /// <param name="rotationLocked">If <see langword="true"/>, the rotation matrix will be locked to the current angle</param>
            public Transform(Vector2Int position, Vector2 rotationPivot, float rotation = 0, Transform? parent = null, bool rotationLocked = false)
            {
                Position = position;
                Parent = parent;
                Rotation = rotation;
                RotationPivot = rotationPivot;
                IsRotationLocked = rotationLocked;
            }

            /// <summary>
            /// A new Transform with the given parameters
            /// </summary>
            /// <param name="position">The position</param>
            /// <param name="rotationPivot">The pivot point that the object rotates around</param>
            /// <param name="rotation">The rotation of the object in degrees</param>
            /// <param name="parent">The optional parent of this transform</param>
            /// <param name="rotationLocked">If <see langword="true"/>, the rotation matrix will be locked to the current angle</param>
            public Transform(Vector2 position, Vector2 rotationPivot, float rotation = 0, Transform? parent = null, bool rotationLocked = false)
            {
                Position = position;
                Parent = parent;
                Rotation = rotation;
                RotationPivot = rotationPivot;
                IsRotationLocked = rotationLocked;
            }

            [JsonIgnore]
            private bool rotationLocked;

            /// <summary>
            /// If set to <see langword="true"/>, the rotation matrix will be locked to the current angle<br/>
            /// otherwise the rotation matrix will be updated
            /// </summary>
            [JsonRequired]
            public bool IsRotationLocked
            {
                get => rotationLocked; set
                {
                    if (value) ma = Matrix3x2.CreateRotation(this.GlobalRotation.DegToRad());
                    rotationLocked = value;
                }
            }

            /// <summary>
            /// Current Local Position
            /// </summary>
            public Vector2 Position;

            /// <summary>
            /// Current Parent Transform
            /// </summary>
            public Transform? Parent;

            /// <summary>
            /// Current Local Rotation
            /// </summary>
            public float Rotation;
            [Obsolete("The Scaling system is not implemented yet")][JsonIgnore] public Vector2Int Scale;

            /// <summary>
            /// Current Local Rotation Pivot
            /// </summary>
            public Vector2 RotationPivot;

            private Matrix3x2 ma;

            /// <summary>
            /// The global integer-coordinates relative to the parent
            /// </summary>
            [JsonIgnore]
            public Vector2Int GlobalIntCoordinates { get => Parent != null ? (Vector2Int)(GlobalCoordinates) : (Vector2Int)Position; }

            /// <summary>
            /// The global coordinates relative to the parent
            /// </summary>
            [JsonIgnore]
            public Vector2 GlobalCoordinates { get => Parent != null ? Vector2.Transform(Position, RotationMatrix) + Parent.GlobalCoordinates : Position; }

            /// <summary>
            /// The global rotation matrix of this transform relative to the parent
            /// </summary>
            [JsonIgnore]
            public virtual Matrix3x2 RotationMatrix => IsRotationLocked ? ma : Matrix3x2.CreateRotation(this.GlobalRotation.DegToRad());

            /// <summary>
            /// The global rotation of this transform relative to parent
            /// </summary>
            [JsonIgnore]
            public float GlobalRotation { get => Parent != null ? Parent.GlobalRotation + Rotation : Rotation; }

            /// <summary>
            /// The scale this text is transfromed to
            /// </summary>
            [Obsolete("The Scaling system is not implemented yet")]
            [JsonIgnore]
            public Vector2Int GlobalScale { get => Parent != null ? Parent.GlobalScale + Scale : Scale; }

            /// <summary>
            /// The global pivot point of this transform relative to the parent
            /// </summary>
            [JsonIgnore]
            public Vector2 GlobalRotationPivot { get => Parent != null ? Vector2.Transform(Parent.GlobalRotationPivot, RotationMatrix) + RotationPivot : RotationPivot; }

            /// <summary>
            /// Transforms a <see cref="TextGrid"/><br/>
            /// Uses the Standart Blend method and allows using a custom alpha factor
            /// </summary>
            /// <param name="gridIndex">The global id of the <see cref="TextGrid"/> that should be transformed</param>
            /// <param name="offset">The offset of the transformation</param>
            /// <param name="output">The <see cref="ITextRenderer.TextOutput"/> that the transformed <see cref="TextGrid"/> will be rendered to</param>
            /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
            public void TransformToOutput(int gridIndex, ITextRenderer.TextOutput output, Vector2Int offset, float alphaFactor = 1.0f)
            {
                Vector2 piv = GlobalRotationPivot;
                Matrix3x2 rot = RotationMatrix;

                Vector2Int o = GlobalIntCoordinates + offset;

                TextGrid g = TextGrid.Get(gridIndex);
                TextColor? tc;
                TextColor?[]? column;

                bool outOfBounds = false;

                for (Vector2Int gp = Vector2Int.Zero; gp.x < g.Grid.Length; gp.x++)
                {
                    if (outOfBounds) break;

                    column = g.Grid[gp.x];
                    if (column == null) continue;

                    for (gp.y = 0; gp.y < column.Length; gp.y++)
                    {
                        tc = column[gp.y];
                        if (tc == null) continue;

                        Vector2Int newPos = (Vector2Int)(Vector2.Transform(gp - piv, rot) + piv) + o;

                        if (newPos.x < 0 || newPos.y < 0) continue;
                        else if (newPos.x >= output.rect.size.x) { outOfBounds = true; break; }
                        else if (newPos.y >= output.rect.size.y) break;

                        tc = new(tc);

                        tc.bgColor.alpha = (byte)(alphaFactor * tc.bgColor.alpha);
                        tc.fgColor.alpha = (byte)(alphaFactor * tc.fgColor.alpha);

                        tc.bgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value
                            ? TextColor.AlphaBlend(tc.bgColor, value.bgColor)
                            : TextColor.AlphaBlend(tc.bgColor, TextColor.Empty.bgColor);

                        tc.fgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value2
                            ? TextColor.AlphaBlend(tc.fgColor, value2.bgColor)
                            : TextColor.AlphaBlend(tc.fgColor, TextColor.Empty.bgColor);

                        if (output.Grid.Grid[newPos.x] == null) output.Grid.Grid[newPos.x] = new TextColor?[output.Grid.Size.y];
#pragma warning disable CS8602
                        output.Grid.Grid[newPos.x][newPos.y] = tc;
#pragma warning restore CS8602
                    }
                }

            }

            /// <summary>
            /// Transforms a <see cref="TextGrid"/>
            /// </summary>
            /// <param name="gridIndex">The global id of the <see cref="TextGrid"/> that should be transformed</param>
            /// <param name="offset">The offset of the transformation</param>
            /// <param name="output">The <see cref="TextOutput"/> that the transformed <see cref="TextGrid"/> will be rendered to</param>
            public void TransformToOutput(int gridIndex, ITextRenderer.TextOutput output, Vector2Int offset)
            {
                Vector2 piv = GlobalRotationPivot;
                Matrix3x2 rot = RotationMatrix;

                Vector2Int o = GlobalIntCoordinates + offset;

                TextGrid g = TextGrid.Get(gridIndex);
                TextColor? tc;
                TextColor?[]? column;

                bool outOfBounds = false;

                for (Vector2Int gp = Vector2Int.Zero; gp.x < g.Grid.Length; gp.x++)
                {
                    if (outOfBounds) break;

                    column = g.Grid[gp.x];
                    if (column == null) continue;

                    for (gp.y = 0; gp.y < column.Length; gp.y++)
                    {
                        tc = column[gp.y];
                        if (tc == null) continue;

                        Vector2Int newPos = (Vector2Int)(Vector2.Transform(gp - piv, rot) + piv) + o;

                        if (newPos.x < 0 || newPos.y < 0) continue;
                        else if (newPos.x >= output.rect.size.x) { outOfBounds = true; break; }
                        else if (newPos.y >= output.rect.size.y) break;

                        tc = new(tc);

                        tc.bgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value
                            ? TextColor.AlphaBlend(tc.bgColor, value.bgColor)
                            : TextColor.AlphaBlend(tc.bgColor, TextColor.Empty.bgColor);

                        tc.fgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value2
                            ? TextColor.AlphaBlend(tc.fgColor, value2.bgColor)
                            : TextColor.AlphaBlend(tc.fgColor, TextColor.Empty.bgColor);

                        if (output.Grid.Grid[newPos.x] == null) output.Grid.Grid[newPos.x] = new TextColor?[output.Grid.Size.y];
#pragma warning disable CS8602
                        output.Grid.Grid[newPos.x][newPos.y] = tc;
#pragma warning restore CS8602
                    }
                }
            }

            /// <summary>
            /// Transforms a <see cref="TextGrid"/> without alpha blending<br/>
            /// Transparent pixels will have no transparency
            /// </summary>
            /// <param name="gridIndex">The global id of the <see cref="TextGrid"/> that should be transformed</param>
            /// <param name="offset">The offset of the transformation</param>
            /// <param name="output">The <see cref="TextOutput"/> that the transformed <see cref="TextGrid"/> will be rendered to</param>
            public void TransformToOutputNoBlending(int gridIndex, ITextRenderer.TextOutput output, Vector2Int offset)
            {
                Vector2 piv = GlobalRotationPivot;
                Matrix3x2 rot = RotationMatrix;

                Vector2Int o = GlobalIntCoordinates + offset;

                TextGrid g = TextGrid.Get(gridIndex);
                TextColor? tc;
                TextColor?[]? column;

                bool outOfBounds = false;

                for (Vector2Int gp = Vector2Int.Zero; gp.x < g.Grid.Length; gp.x++)
                {
                    if (outOfBounds) break;

                    column = g.Grid[gp.x];
                    if (column == null) continue;

                    for (gp.y = 0; gp.y < column.Length; gp.y++)
                    {
                        tc = column[gp.y];
                        if (tc == null) continue;

                        Vector2Int newPos = (Vector2Int)(Vector2.Transform(gp - piv, rot) + piv) + o;

                        if (newPos.x < 0 || newPos.y < 0) continue;
                        else if (newPos.x >= output.rect.size.x) { outOfBounds = true; break; }
                        else if (newPos.y >= output.rect.size.y) break;

                        if (output.Grid.Grid[newPos.x] == null) output.Grid.Grid[newPos.x] = new TextColor?[output.Grid.Size.y];
#pragma warning disable CS8602
                        output.Grid.Grid[newPos.x][newPos.y] = tc;
#pragma warning restore CS8602
                    }
                }
            }

            /// <summary>
            /// Transforms a <see cref="TextGrid"/><br/>
            /// Uses the Multiply-Blending method
            /// </summary>
            /// <param name="gridIndex">The global id of the <see cref="TextGrid"/> that should be transformed</param>
            /// <param name="offset">The offset of the transformation</param>
            /// <param name="output">The <see cref="TextOutput"/> that the transformed <see cref="TextGrid"/> will be rendered to</param>
            public void TransformToOutputMultiplyBlending(int gridIndex, ITextRenderer.TextOutput output, Vector2Int offset)
            {
                Vector2 piv = GlobalRotationPivot;
                Matrix3x2 rot = RotationMatrix;

                Vector2Int o = GlobalIntCoordinates + offset;

                TextGrid g = TextGrid.Get(gridIndex);
                TextColor? tc;
                TextColor?[]? column;

                bool outOfBounds = false;

                for (Vector2Int gp = Vector2Int.Zero; gp.x < g.Grid.Length; gp.x++)
                {
                    if (outOfBounds) break;

                    column = g.Grid[gp.x];
                    if (column == null) continue;

                    for (gp.y = 0; gp.y < column.Length; gp.y++)
                    {
                        tc = column[gp.y];
                        if (tc == null) continue;

                        Vector2Int newPos = (Vector2Int)(Vector2.Transform(gp - piv, rot) + piv) + o;

                        if (newPos.x < 0 || newPos.y < 0) continue;
                        else if (newPos.x >= output.rect.size.x) { outOfBounds = true; break; }
                        else if (newPos.y >= output.rect.size.y) break;

                        tc = new(tc);

                        tc.bgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value
                            ? TextColor.AlphaMultiply(tc.bgColor, value.bgColor)
                            : TextColor.AlphaMultiply(tc.bgColor, TextColor.Empty.bgColor);

                        tc.fgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value2
                            ? TextColor.AlphaMultiply(tc.fgColor, value2.bgColor)
                            : TextColor.AlphaMultiply(tc.fgColor, TextColor.Empty.bgColor);

                        if (output.Grid.Grid[newPos.x] == null) output.Grid.Grid[newPos.x] = new TextColor?[output.Grid.Size.y];
#pragma warning disable CS8602
                        output.Grid.Grid[newPos.x][newPos.y] = tc;
#pragma warning restore CS8602
                    }
                }
            }

            /// <summary>
            /// Transforms a <see cref="TextGrid"/><br/>
            /// Uses the Multiply-Blending method and allows using a custom alpha factor
            /// </summary>
            /// <param name="gridIndex">The global id of the <see cref="TextGrid"/> that should be transformed</param>
            /// <param name="offset">The offset of the transformation</param>
            /// <param name="output">The <see cref="TextOutput"/> that the transformed <see cref="TextGrid"/> will be rendered to</param>
            /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
            public void TransformToOutputMultiplyBlending(int gridIndex, ITextRenderer.TextOutput output, Vector2Int offset, float alphaFactor = 1.0f)
            {
                Vector2 piv = GlobalRotationPivot;
                Matrix3x2 rot = RotationMatrix;

                Vector2Int o = GlobalIntCoordinates + offset;

                TextGrid g = TextGrid.Get(gridIndex);
                TextColor? tc;
                TextColor?[]? column;

                bool outOfBounds = false;

                for (Vector2Int gp = Vector2Int.Zero; gp.x < g.Grid.Length; gp.x++)
                {
                    if (outOfBounds) break;

                    column = g.Grid[gp.x];
                    if (column == null) continue;

                    for (gp.y = 0; gp.y < column.Length; gp.y++)
                    {
                        tc = column[gp.y];
                        if (tc == null) continue;

                        Vector2Int newPos = (Vector2Int)(Vector2.Transform(gp - piv, rot) + piv) + o;

                        if (newPos.x < 0 || newPos.y < 0) continue;
                        else if (newPos.x >= output.rect.size.x) { outOfBounds = true; break; }
                        else if (newPos.y >= output.rect.size.y) break;



                        tc = new(tc);

                        tc.bgColor.alpha = (byte)(alphaFactor * tc.bgColor.alpha);
                        tc.fgColor.alpha = (byte)(alphaFactor * tc.fgColor.alpha);

                        tc.bgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value
                            ? TextColor.AlphaMultiply(tc.bgColor, value.bgColor)
                            : TextColor.AlphaMultiply(tc.bgColor, TextColor.Empty.bgColor);

                        tc.fgColor = output.Grid.Grid[newPos.x]?[newPos.y] is TextColor value2
                            ? TextColor.AlphaMultiply(tc.fgColor, value2.bgColor)
                            : TextColor.AlphaMultiply(tc.fgColor, TextColor.Empty.bgColor);

                        if (output.Grid.Grid[newPos.x] == null) output.Grid.Grid[newPos.x] = new TextColor?[output.Grid.Size.y];
#pragma warning disable CS8602
                        output.Grid.Grid[newPos.x][newPos.y] = tc;
#pragma warning restore CS8602
                    }
                }
            }

            private Transform() { }

            /*public string Serialize() => JsonSerializer.Serialize(this);

            public static Transform? Deserialize(string data)
            {
                return JsonSerializer.Deserialize<Transform>(data);
            }*/
        }


        /// <summary>
        /// If set to <see langword="true"/>, the <see cref="RenderObject"/> will be hidden
        /// </summary>
        public bool IsHidden = false;

        /// <summary>
        /// The Transformation of this <see cref="RenderObject"/>
        /// </summary>
        public Transform Transformation;

        /// <summary>
        /// Changes the current <see cref="TextGrid"/> to the one with the given id
        /// </summary>
        /// <param name="textGridId"></param>
        public void LoadGrid(string textGridId) => GridIndex = TextGrid.GetIndex(textGridId);

        /// <summary>
        /// The index of the <see cref="TextGrid"/> that is getting rendered
        /// </summary>
        public int GridIndex;

        /// <summary>
        /// The layer number this <see cref="RenderObject"/> should be added to
        /// </summary>
        public readonly int Layer;

        /// <summary>
        /// The unique <see cref="Guid"/> of this <see cref="RenderObject"/>
        /// </summary>
        public readonly Guid GUID = Guid.NewGuid();

        /// <summary>
        /// The factor that the all of the copied pixel alpha values are multiplied with
        /// </summary>
        public float AlphaFactor = 1.0f;

        /// <summary>
        /// A constructor that will automatically add the new <see cref="RenderObject"/> to the given <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="renderLayers">A <see cref="IRenderLayers"/> object that this <see cref="RenderObject"/> will be added to</param>
        /// <param name="transform">The <see cref="Transform"/> this text will be transformed by</param>
        /// <param name="textGridId">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="isHidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
        public RenderObject(IRenderLayers renderLayers, Transform transform, string textGridId, int layer, bool isHidden = false, float alphaFactor = 1)
        {
            Transformation = transform;
            GridIndex = TextGrid.GetIndex(textGridId);
            Layer = layer;
            IsHidden = isHidden;
            AlphaFactor = alphaFactor;
            renderLayers.AddRenderObject(this);
        }

        /// <summary>
        /// A constructor that will create a new <see cref="RenderObject"/> without adding it to a <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> this text will be transformed by</param>
        /// <param name="textGridId">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="hidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
        public RenderObject(Transform transform, string textGridId, int layer, bool hidden = false, float alphaFactor = 1)
        {
            Transformation = transform;
            GridIndex = TextGrid.GetIndex(textGridId);
            Layer = layer;
            IsHidden = hidden;
            AlphaFactor = alphaFactor;
        }

        /// <summary>
        /// A constructor that will create a new <see cref="RenderObject"/> without adding it to a <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> this text will be transformed by</param>
        /// <param name="textGridIndex">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="hidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
        public RenderObject(Transform transform, int textGridIndex, int layer, bool hidden = false, float alphaFactor = 1)
        {
            Transformation = transform;
            GridIndex = textGridIndex;
            Layer = layer;
            IsHidden = hidden;
            AlphaFactor = alphaFactor;
        }

        /// <summary>
        /// A constructor that will automatically add the new <see cref="RenderObject"/> to the given <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="renderLayers">A <see cref="IRenderLayers"/> object that this <see cref="RenderObject"/> will be added to</param>
        /// <param name="transform">The <see cref="Transform"/> this text will be transformed by</param>
        /// <param name="textGridIndex">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="hidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
        public RenderObject(IRenderLayers renderLayers, Transform transform, int textGridIndex, int layer, bool hidden = false, float alphaFactor = 1)
        {
            Transformation = transform;
            GridIndex = textGridIndex;
            Layer = layer;
            IsHidden = hidden;
            AlphaFactor = alphaFactor;
            renderLayers.AddRenderObject(this);
        }

        /// <summary>
        /// A constructor that will create a new <see cref="RenderObject"/> without adding it to a <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="globalPosition">The global position this text will be transformed to</param>
        /// <param name="rotation">The rotation of the object in degrees</param>
        /// <param name="rotationLocked">If <see langword="true"/>, the rotation matrix will be locked to the current angle</param>
        /// <param name="rotationPivot">The pivot point that the object rotates around</param>
        /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
        /// <param name="textGridIndex">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="hidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        public RenderObject(Vector2 globalPosition, Vector2 rotationPivot, float rotation, int textGridIndex, int layer, bool hidden = false, float alphaFactor = 1, bool rotationLocked = false)
        {
            Transformation = new Transform(position:globalPosition, rotationPivot:rotationPivot, rotation: rotation, rotationLocked:rotationLocked);
            GridIndex = textGridIndex;
            Layer = layer;
            IsHidden = hidden;
            AlphaFactor = alphaFactor;
        }

        /// <summary>
        /// A constructor that will automatically add the new <see cref="RenderObject"/> to the given <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="renderLayers">A <see cref="IRenderLayers"/> object that this <see cref="RenderObject"/> will be added to</param>
        /// <param name="globalPosition">The global position this text will be transformed to</param>
        /// <param name="rotation">The rotation of the object in degrees</param>
        /// <param name="rotationLocked">If <see langword="true"/>, the rotation matrix will be locked to the current angle</param>
        /// <param name="rotationPivot">The pivot point that the object rotates around</param>
        /// <param name="alphaFactor">The factor that the all of the copied pixel alpha values are multiplied with</param>
        /// <param name="textGridIndex">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="hidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        public RenderObject(IRenderLayers renderLayers, Vector2 globalPosition, Vector2 rotationPivot, float rotation, int textGridIndex, int layer, bool hidden = false, float alphaFactor = 1, bool rotationLocked = false)
        {
            Transformation = new Transform(position: globalPosition, rotationPivot: rotationPivot, rotation: rotation, rotationLocked: rotationLocked);
            GridIndex = textGridIndex;
            Layer = layer;
            IsHidden = hidden;
            AlphaFactor = alphaFactor;
            renderLayers.AddRenderObject(this);
        }

        /// <summary>
        /// Creates a copy of the given <see cref="RenderObject"/>
        /// </summary>
        /// <param name="original">The original <see cref="RenderObject"/></param>
        /// <param name="newLayer">Allows you to create the copy on a different layer if not null</param>
        public RenderObject(RenderObject original, int? newLayer = null) 
        { 
            Transformation = original.Transformation;
            GridIndex = original.GridIndex;
            Layer = newLayer ?? original.Layer;
            IsHidden = original.IsHidden;
            AlphaFactor = original.AlphaFactor;
        }

        /// <summary>
        /// Creates a copy of the given <see cref="RenderObject"/> on the given <see cref="ITextRenderer.IRenderLayers"/>
        /// </summary>
        /// <param name="renderLayers">A <see cref="IRenderLayers"/> object that this <see cref="RenderObject"/> will be added to</param>
        /// <param name="original">The original <see cref="RenderObject"/></param>
        /// <param name="newLayer">Allows you to create the copy on a different layer if not null</param>
        public RenderObject(IRenderLayers renderLayers, RenderObject original, int? newLayer = null)
        {
            Transformation = original.Transformation;
            GridIndex = original.GridIndex;
            Layer = newLayer ?? original.Layer;
            IsHidden = original.IsHidden;
            AlphaFactor = original.AlphaFactor;
            renderLayers.AddRenderObject(this);
        }
    }


    /// <summary>
    /// Used to create renderers that render to <see cref="TextOutput"/>s
    /// </summary>
    public interface ITextRenderer
    {
        /// <summary>
        /// A <see langword="interface"/> that manages the different layers that <see cref="RenderObject"/>s are rendered to
        /// </summary>
        public interface IRenderLayers
        {
            /// <summary>
            /// Gets the <see cref="IEnumerator{T}"/> for Enumerating through all shown layers
            /// </summary>
            /// <returns><see cref="IEnumerator{T}"/> of all shown layers</returns>
            public abstract IEnumerator<KeyValuePair<int, HashSet<RenderObject>>> GetEnumerator();

            /// <summary>
            /// Checks if a layer exists or not
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <returns><see langword="true"/> if the layer exists; otherwise <see langword="false"/></returns>
            public abstract bool DoesLayerExist(int layer);

            /// <summary>
            /// Checks if a layer is hidden
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <returns><see langword="true"/> if the layer is hidden; otherwise <see langword="false"/></returns>
            public abstract bool IsLayerHidden(int layer);

            /// <summary>
            /// Creates a new layer that can be used to sort <see cref="RenderObject"/>s
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <param name="hidden">If <see langword="true"/>, the layer will be creates as a hidden layer</param>
            /// <returns><see langword="true"/> if the new layer was created; otherwise <see langword="false"/></returns>
            public abstract bool CreateLayer(int layer, bool hidden = false);

            /// <summary>
            /// Adds a <see cref="RenderObject"/> to the given layer <br/>
            /// The <see cref="RenderObject"/>s are rendered in the order of when they where added to the layer
            /// </summary>
            /// <param name="obj">The <see cref="RenderObject"/> to add</param>
            public abstract void AddRenderObject(RenderObject obj);

            /// <summary>
            /// Removes a certain <see cref="RenderObject"/> from the given layer
            /// </summary>
            /// <param name="obj">The <see cref="RenderObject"/> that should be removed</param>
            /// <returns><see langword="true"/> if the <see cref="RenderObject"/> was successfully removed from the layer; otherwise <see langword="false"/></returns>
            public abstract bool RemoveRenderObject(RenderObject obj);

            /// <summary>
            /// Sets if a layer is hidden or not
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <param name="hide">If true, the layer should be hidden; otherwise the layer is shown</param>
            public abstract void SetHiddenLayer(int layer, bool hide = true);

            /// <summary>
            /// Moves a <see cref="RenderObject"/> to a different layer by creating a new <see cref="RenderObject"/> with the same properties but a different layer
            /// </summary>
            /// <param name="obj">The <see cref="RenderObject"/> that should be moved</param>
            /// <param name="newLayer">The new layer</param>
            public abstract void MoveRenderObject(RenderObject obj, int newLayer);
        }


        /// <summary>
        /// A way to organize the layers that <see cref="RenderObject"/>s are rendered to<br/>
        /// It also allows you to hide certain layers
        /// </summary>
        public class RenderLayers : IRenderLayers
        {
            private readonly SortedDictionary<int, HashSet<RenderObject>> HiddenLayers = [];

            private readonly HashSet<int> hiddenLayerKeys = [];

            private readonly SortedDictionary<int, HashSet<RenderObject>> ShownLayers = [];

            public IEnumerator<KeyValuePair<int, HashSet<RenderObject>>> GetEnumerator() => ShownLayers.GetEnumerator();

            public bool DoesLayerExist(int layer) => HiddenLayers.ContainsKey(layer) || ShownLayers.ContainsKey(layer);

            public bool IsLayerHidden(int layer) => hiddenLayerKeys.Contains(layer);

            public virtual bool CreateLayer(int layer, bool hidden = false)
            {
                if (hidden && !HiddenLayers.ContainsKey(layer)) { HiddenLayers[layer] = []; return true; }
                else if (!ShownLayers.ContainsKey(layer)) { ShownLayers[layer] = []; return true; }
                return false;
            }

            public virtual void AddRenderObject(RenderObject obj)
            {
                if (IsLayerHidden(obj.Layer))
                {
                    if (!HiddenLayers.ContainsKey(obj.Layer)) HiddenLayers[obj.Layer] = [];
                    HiddenLayers[obj.Layer].Add(obj);
                }
                else
                {
                    if (!ShownLayers.ContainsKey(obj.Layer)) ShownLayers[obj.Layer] = [];
                    ShownLayers[obj.Layer].Add(obj);
                }
            }

            public virtual bool RemoveRenderObject(RenderObject obj)
            {
                if (IsLayerHidden(obj.Layer) && HiddenLayers.TryGetValue(obj.Layer, out HashSet<RenderObject>? value))
                {
                    return value.Remove(obj);
                }
                else if (ShownLayers.TryGetValue(obj.Layer, out HashSet<RenderObject>? value2))
                {
                    return value2.Remove(obj);
                }
                return false;
            }

            public virtual void SetHiddenLayer(int layer, bool hide = true)
            {
                switch (hide)
                {
                    case true:
                        if (HiddenLayers.ContainsKey(layer) || !ShownLayers.TryGetValue(layer, out HashSet<RenderObject>? obj)) return;
                        hiddenLayerKeys.Add(layer);
                        HiddenLayers[layer] = obj;
                        ShownLayers.Remove(layer);
                        break;
                    case false:
                        if (ShownLayers.ContainsKey(layer) || !HiddenLayers.TryGetValue(layer, out HashSet<RenderObject>? obj2)) return;
                        hiddenLayerKeys.Remove(layer);
                        ShownLayers[layer] = obj2;
                        HiddenLayers.Remove(layer);
                        break;
                }
            }

            public void MoveRenderObject(RenderObject obj, int newLayer)
            {
                RemoveRenderObject(obj);

                _ = new RenderObject(this, obj, newLayer);
            }
        }


        /// <summary>
        /// A wrapper for <see cref="RenderLayers"/> that allows concurrent access to the different layers
        /// </summary>
        public class ConcurrentRenderLayers : IRenderLayers
        {
            private readonly RenderLayers layers = new();

            public IEnumerator<KeyValuePair<int, HashSet<RenderObject>>> GetEnumerator() => layers.GetEnumerator();

            /// <summary>
            /// A lock that should be used when accessing a layer or adding/removing <see cref="RenderObject"/>s or Rendering all the layers<br/>
            /// You have to use this lock every time you change something about the layers or else an Exception can be thrown
            /// </summary>
            public readonly Lock RenderLock = new();

            public ConcurrentRenderLayers() { }

            public bool DoesLayerExist(int layer) => layers.DoesLayerExist(layer);

            public bool IsLayerHidden(int layer) => layers.IsLayerHidden(layer);

            public void SetHiddenLayer(int layer, bool hide = true)
            {
                lock (RenderLock) { layers.SetHiddenLayer(layer, hide); }
            }

            public bool CreateLayer(int layer, bool hidden = false)
            {
                lock (RenderLock) { return layers.CreateLayer(layer, hidden); }
            }

            public void AddRenderObject(RenderObject obj)
            {
                lock (RenderLock) { layers.AddRenderObject(obj); }
            }

            public bool RemoveRenderObject(RenderObject obj)
            {
                lock (RenderLock) { return layers.RemoveRenderObject(obj); }
            }

            public void MoveRenderObject(RenderObject obj, int newLayer)
            {
                lock (RenderLock) { layers.MoveRenderObject(obj, newLayer); }
            }
        }


        /// <summary>
        /// The way the alpha values of the pixels are blended<br/>
        /// </summary>
        public enum AlphaBlendMode
        {
            /// <summary>
            /// (Best Quality) The standart alpha blending method
            /// </summary>
            BLEND,
            /// <summary>
            /// (Fastest) Skips the alpha blending and just copies the pixel colors
            /// </summary>
            NONE,
            /// <summary>
            /// (Faster) A faster way of blending the alpha values that is less accurate
            /// </summary>
            MULTIPLY
        }


        /// <summary>
        /// Stores the currently rendered frame<br/>
        /// A <see cref="TextOutput"/> is like a viewport and<br/>
        /// allows editing text at a certain coordinate
        /// </summary>
        public class TextOutput
        {
            /// <summary>
            /// The output <see cref="RenderObject.TextGrid"/> that is being displayed
            /// </summary>
            public readonly RenderObject.TextGrid Grid;

            /// <summary>
            /// The viewport of the <see cref="TextOutput"/>
            /// </summary>
            public IntRect rect;

            /// <summary>
            /// Sets or gets the given <see cref="TextColor"/> at a certain coordinate
            /// </summary>
            /// <param name="at">A coordinate inside the grid</param>
            /// <returns>The <see cref="TextColor"/> at the given coordinates or <see cref="TextColor.Empty"/> if not found</returns>
            public virtual TextColor this[Vector2Int at]
            {
                get => rect.IsInRect(at) ? Grid.Grid[at.x]?[at.y] ?? TextColor.Empty : TextColor.Empty;
            }

            /// <summary>
            /// Clears the entire output <see cref="RenderObject.TextGrid"/>
            /// </summary>
            public virtual void Clear() => Array.Clear(Grid.Grid);

            readonly StringBuilder resultBuilder = new();

            /*public override string ToString()
            {
                resultBuilder.Clear();
                for (Vector2Int v = rect.position; v.y < rect.Corner.y; v.y++)
                {
                    for (v.x = rect.position.x; v.x < rect.Corner.x; v.x++)
                    {
                        if (!grid.TryGetValue(v, out TextColor? value)) resultBuilder.Append(TextColor.EmptyString);
                        else
                            resultBuilder.Append(value);
                    }
                    resultBuilder.AppendLine();
                }
                return resultBuilder.ToString();
            }*/

            private readonly byte[] result;

            private readonly byte[] emptyLine;

            /// <summary>
            /// Creates a new <see cref="TextOutput"/> with the given viewport <see cref="IntRect"/>
            /// </summary>
            /// <param name="rect">The viewport of this <see cref="TextOutput"/></param>
            public TextOutput(IntRect rect)
            {
                Grid = new($"TextOut-{Random.Shared.Next()}", rect.CreateGridArray<TextColor>(), rect);
                this.rect = rect;

                result = new byte[rect.size.y * rect.size.x * 43 + rect.size.y + 1];

                emptyLine = new byte[rect.size.x * 43 + 1];
                for (int i = 0; i < rect.size.x; i++)
                {
                    TextColor.encodedEmptyString.CopyTo(emptyLine, i * 43);
                }
                emptyLine[^1] = 10;
            }

            /// <summary>
            /// Converts the current frame grid to an encoded byte array
            /// </summary>
            /// <returns>A byte array that uses the default windows console encoding</returns>
            public Span<byte> AsSpan()
            {
                int index = 0;
                Vector2Int corner = rect.Corner;
                TextColor? textColor;
                Span<byte> resultSpan = new Span<byte>(result);
                Span<byte> emptyStringSpan = TextColor.encodedEmptyString.AsSpan();

                for (Vector2Int v = rect.position; v.y < corner.y; v.y++)
                {
                    for (v.x = rect.position.x; v.x < corner.x; v.x++)
                    {
                        textColor = Grid.Grid[v.x]?[v.y];

                        if (textColor != null)
                        {
                            textColor.Encoded.CopyTo(resultSpan.Slice(index, 43));
                        }
                        else
                        {
                            emptyStringSpan.CopyTo(resultSpan.Slice(index, 43));
                        }

                        index += 43;
                    }
                    resultSpan[index] = 10;
                    index++;
                }
                return resultSpan;
            }
        }

        /// <summary>
        /// Renders text to a <see cref="TextOutput"/>
        /// </summary>
        /// <param name="output"></param>
        public abstract void Render(TextOutput output);

        /// <summary>
        /// Executed after <see cref="ITextRenderer.Render(TextOutput)"/> was executed<br/>
        /// Allows editing the same <see cref="TextOutput"/> instance of the <see cref="ITextRenderer.Render(TextOutput)"/> method
        /// </summary>
        public abstract void AfterRender(TextOutput output);

        /// <summary>
        /// Used to clear the given <see cref="TextOutput"/>
        /// </summary>
        public abstract void Clear(TextOutput output);
    }
    #endregion

    #region Renderers
    /// <summary>
    /// Renders Text to the windows Console
    /// </summary>
    public class ConsoleRenderer : ITextRenderer
    {
        private AlphaBlendMode _blendMode = AlphaBlendMode.BLEND;

        /// <summary>
        /// The current alpha blending mode of the renderer
        /// </summary>
        public AlphaBlendMode BlendMode { get => _blendMode; set => _blendMode = value; }

        /// <summary>
        /// A custom Console that allows much faster writing
        /// </summary>
        public static class FastConsole
        {
            public readonly static BufferedStream str;

            static FastConsole()
            {
                enc = Console.OutputEncoding;

                str = new BufferedStream(Console.OpenStandardOutput(), 0x100);
            }

            public static readonly Encoding enc;

            public static void WriteLine(string s) => Write(s + "\r\n");

            public static void Write(string s)
            {
                Span<byte> rgb = stackalloc byte[s.Length];
                enc.GetBytes(s, rgb);
                str.Write(rgb);
            }

            public static void Write(Span<byte> s)
            {
                str.Write(s);
            }

            public static void Write(char[] s)
            {
                var rgb = new byte[s.Length << 1];
                enc.GetBytes(s, 0, s.Length, rgb, 0);
                str.Write(rgb, 0, rgb.Length);
            }

            public static void Flush() => str.Flush();
        };

        /// <summary>
        /// The layers that are rendered
        /// </summary>
        public ITextRenderer.IRenderLayers Layers;

        public ConsoleRenderer(ITextRenderer.IRenderLayers layers)
        {
            Layers = layers;
        }

        /// <summary>
        /// Adds a <see cref="RenderObject"/> to the <see cref="ITextRenderer.RenderLayers"/>
        /// </summary>
        /// <param name="obj">The <see cref="RenderObject"/> to add</param>
        public void AddRenderObject(RenderObject obj) => Layers.AddRenderObject(obj);

        /// <summary>
        /// Executed after rendering the frame using <see cref="Render(ITextRenderer.TextOutput)"/><br/>
        /// Also writes the current frame to the console
        /// </summary>
        /// <param name="output">The current frame output</param>
        public virtual void AfterRender(ITextRenderer.TextOutput output)
        {
            Console.SetCursorPosition(0, 0);
            FastConsole.Write(output.AsSpan());

            Clear(output);
        }

        /// <summary>
        /// The offset of the rendered frame
        /// </summary>
        public Vector2Int Offset = Vector2Int.Zero;


        /// <summary>
        /// Renders a new frame to the <see cref="ITextRenderer.TextOutput"/>
        /// </summary>
        /// <param name="output">The current frame output</param>
        public virtual void Render(ITextRenderer.TextOutput output)
        {
            foreach (var layer in Layers)
            {
                foreach (var obj in layer.Value)
                {
                    if (obj.IsHidden) continue;
                    if (obj.AlphaFactor != 1) obj.Transformation.TransformToOutput(obj.GridIndex, output, Offset, obj.AlphaFactor);
                    else obj.Transformation.TransformToOutput(obj.GridIndex, output, Offset);
                }
            }
            AfterRender(output);
        }

        /// <summary>
        /// Executed after using the <see cref="AfterRender(ITextRenderer.TextOutput)"/> method<br/>
        /// Should be used to clear the <see cref="ITextRenderer.TextOutput"/>
        /// </summary>
        /// <param name="output">The current frame output</param>
        public virtual void Clear(ITextRenderer.TextOutput output)
        {
            output.Clear();
        }
    }


    /// <summary>
    /// Renders Text to the windows Console<br/>
    /// And supports concurrent access
    /// </summary>
    public class ConcurrentConsoleRenderer : ITextRenderer
    {
        private AlphaBlendMode _blendMode = AlphaBlendMode.BLEND;

        /// <summary>
        /// The current alpha blending mode of the renderer
        /// </summary>
        public AlphaBlendMode BlendMode
        {
            get => _blendMode; set
            {
                lock (Layers.RenderLock)
                {
                    _blendMode = value;
                }
            }
        }

        /// <summary>
        /// The layers that are rendered
        /// </summary>
        public ITextRenderer.ConcurrentRenderLayers Layers = new();

        /// <summary>
        /// Adds a <see cref="RenderObject"/> to the <see cref="ITextRenderer.RenderLayers"/>
        /// </summary>
        /// <param name="obj">The <see cref="RenderObject"/> to add</param>
        public void AddRenderObject(RenderObject obj) => Layers.AddRenderObject(obj);

        /// <summary>
        /// Executed after rendering the frame using <see cref="Render(ITextRenderer.TextOutput)"/><br/>
        /// Also writes the current frame to the console
        /// </summary>
        /// <param name="output">The current frame output</param>
        public virtual void AfterRender(ITextRenderer.TextOutput output)
        {
            Console.SetCursorPosition(0, 0);
            ConsoleRenderer.FastConsole.Write(output.AsSpan());

            Clear(output);
        }

        /// <summary>
        /// The offset of the rendered frame
        /// </summary>
        public Vector2Int Offset = Vector2Int.Zero;

        /// <summary>
        /// Renders a new frame to the <see cref="ITextRenderer.TextOutput"/>
        /// </summary>
        /// <param name="output">The current frame output</param>
        public virtual void Render(ITextRenderer.TextOutput output)
        {
            lock (Layers.RenderLock)
            {
                switch (BlendMode)
                {
                    case AlphaBlendMode.BLEND:
                        foreach (var layer in Layers)
                        {
                            foreach (var obj in layer.Value)
                            {
                                if (obj.IsHidden) continue;

                                if (obj.AlphaFactor != 1f) obj.Transformation.TransformToOutput(obj.GridIndex, output, Offset, obj.AlphaFactor);
                                else obj.Transformation.TransformToOutput(obj.GridIndex, output, Offset);
                            }
                        }
                        break;
                    case AlphaBlendMode.NONE:
                        foreach (var layer in Layers)
                        {
                            foreach (var obj in layer.Value)
                            {
                                if (obj.IsHidden) continue;

                                obj.Transformation.TransformToOutputNoBlending(obj.GridIndex, output, Offset);
                            }
                        }
                        break;
                    case AlphaBlendMode.MULTIPLY:
                        foreach (var layer in Layers)
                        {
                            foreach (var obj in layer.Value)
                            {
                                if (obj.IsHidden) continue;

                                if (obj.AlphaFactor != 1f) obj.Transformation.TransformToOutputMultiplyBlending(obj.GridIndex, output, Offset, obj.AlphaFactor);
                                else obj.Transformation.TransformToOutputMultiplyBlending(obj.GridIndex, output, Offset);
                            }
                        }
                        break;
                }
                AfterRender(output);
            }
            
        }

        /// <summary>
        /// Executed after using the <see cref="AfterRender(ITextRenderer.TextOutput)"/> method<br/>
        /// Should be used to clear the <see cref="ITextRenderer.TextOutput"/>
        /// </summary>
        /// <param name="output">The current frame output</param>
        public virtual void Clear(ITextRenderer.TextOutput output)
        {
            output.Clear();
        }
    }
    #endregion
}
