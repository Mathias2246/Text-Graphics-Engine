
using System.Text.Json;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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

    public class Serialization
    {
        [System.AttributeUsage(System.AttributeTargets.Field)]
        public class Ignore : Attribute { }
    }
    #endregion

    #region IntegerStructs
    /// <summary>
    /// A Rectangle consisting of <see langword="int"/>s
    /// </summary>
    /// <param name="position">The position of the top-left corner</param>
    public struct IntRect(Vector2Int position, Vector2Int size)
    {
        public Vector2Int position = position;
        /// <summary>
        /// The position of the bottom-right corner of the rectangle
        /// </summary>
        public readonly Vector2Int Corner => position + size;
        public Vector2Int size = size;

        public readonly bool IsInRect(Vector2Int v) => v.x >= position.x && v.y >= position.y && v.x < Corner.x && v.y < Corner.y;
        public readonly bool IsInRect(Vector2 v) => v[0] >= position.x && v[1] >= position.y && v[0] < Corner.x && v[1] < Corner.y;

        public override readonly string ToString() => $"from: {position} size: {size}";
    }

    /// <summary>
    /// A two-dimensional Vector that consists of two <see langword="int"/>s
    /// </summary>
    /// <param name="x">The x value of the <see cref="Vector2Int"/></param>
    /// <param name="y">The y value of the <see cref="Vector2Int"/></param>
    public struct Vector2Int(int x, int y)
    {

        public override readonly string ToString() => $"{x} {y}";

        public int x = x;
        public int y = y;

        public static readonly Vector2Int Zero = new(0, 0);

        public static implicit operator Vector2(Vector2Int v) => new(v.x, v.y);
        public static explicit operator Vector2Int(Vector2 v) => new((int)v.X, (int)v.Y);

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2) => new(v1.x + v2.x, v1.y + v2.y);
        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2) => new(v1.x - v2.x, v1.y - v2.y);
        public static Vector2Int operator *(Vector2Int v1, Vector2Int v2) => new(v1.x * v2.x, v1.y * v2.y);
        public static Vector2Int operator /(Vector2Int v1, Vector2Int v2) => new(v1.x / v2.x, v1.y / v2.y);

    }

    /// <summary>
    /// A Integer Range
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
    }

    #endregion

    #region ExtensionClasses
    /// <summary>
    /// Contains <see cref="Vector"/> extension methods
    /// </summary>
    public static class VectorExt
    {
        public static Vector2 RotateAboutOrigin(this Vector2 point, Vector2 origin, float angle) => Vector2.Transform(point - origin, Matrix3x2.CreateRotation(angle.DegToRad())) + origin;

        public static Vector2Int FloorToVector2Int(this Vector2 vector) => new((int)MathF.Floor(vector.X), (int)MathF.Floor(vector.Y));
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
        /// <returns>Converted angle as radians</returns>
        public static float DegToRad(this float angle) => dtrf * angle;
    }

    #endregion

    #region TextRendering
    /// <summary>
    /// Colored text character
    /// </summary>
    /// <remarks>
    /// The default constructor
    /// </remarks>
    /// <param name="character">The character to show</param>
    /// <param name="fgcolor">The foreground color of the character</param>
    /// <param name="bgcolor">The background color of the character</param>
    public partial class TextColor(char character, TextColor.RGBA fgcolor, TextColor.RGBA bgcolor)
    {
        public char c = character;
        [JsonIgnore]
        public char Character { get => c; set { c = value; _character = (byte)value; } }
        [JsonIgnore]
        private byte _character = (byte)character;

        public RGBA fgColor = fgcolor;
        public RGBA bgColor = bgcolor;

        [JsonIgnore]
        public byte[] Encoded => [27, 91, 51, 56, 59, 50, 59, _bc[fgColor.red][0], _bc[fgColor.red][1], _bc[fgColor.red][2], 59, _bc[fgColor.green][0], _bc[fgColor.green][1], _bc[fgColor.green][2], 59, _bc[fgColor.blue][0], _bc[fgColor.blue][1], _bc[fgColor.blue][2], 109, 27, 91, 52, 56, 59, 50, 59, _bc[bgColor.red][0], _bc[bgColor.red][1], _bc[bgColor.red][2], 59, _bc[bgColor.green][0], _bc[bgColor.green][1], _bc[bgColor.green][2], 59, _bc[bgColor.blue][0], _bc[bgColor.blue][1], _bc[bgColor.blue][2], 109, _character, 27, 91, 48, 109];


        public override string ToString() => $"\x1B[38;2;{fgColor.red};{fgColor.green};{fgColor.blue}m\x1B[48;2;{bgColor.red};{bgColor.green};{bgColor.blue}m{Character}\u001b[0m";

        private static readonly byte[][] _bc = [[48, 48, 48,], [48, 48, 49,], [48, 48, 50,], [48, 48, 51,], [48, 48, 52,], [48, 48, 53,], [48, 48, 54,], [48, 48, 55,], [48, 48, 56,], [48, 48, 57,], [48, 49, 48,], [48, 49, 49,], [48, 49, 50,], [48, 49, 51,], [48, 49, 52,], [48, 49, 53,], [48, 49, 54,], [48, 49, 55,], [48, 49, 56,], [48, 49, 57,], [48, 50, 48,], [48, 50, 49,], [48, 50, 50,], [48, 50, 51,], [48, 50, 52,], [48, 50, 53,], [48, 50, 54,], [48, 50, 55,], [48, 50, 56,], [48, 50, 57,], [48, 51, 48,], [48, 51, 49,], [48, 51, 50,], [48, 51, 51,], [48, 51, 52,], [48, 51, 53,], [48, 51, 54,], [48, 51, 55,], [48, 51, 56,], [48, 51, 57,], [48, 52, 48,], [48, 52, 49,], [48, 52, 50,], [48, 52, 51,], [48, 52, 52,], [48, 52, 53,], [48, 52, 54,], [48, 52, 55,], [48, 52, 56,], [48, 52, 57,], [48, 53, 48,], [48, 53, 49,], [48, 53, 50,], [48, 53, 51,], [48, 53, 52,], [48, 53, 53,], [48, 53, 54,], [48, 53, 55,], [48, 53, 56,], [48, 53, 57,], [48, 54, 48,], [48, 54, 49,], [48, 54, 50,], [48, 54, 51,], [48, 54, 52,], [48, 54, 53,], [48, 54, 54,], [48, 54, 55,], [48, 54, 56,], [48, 54, 57,], [48, 55, 48,], [48, 55, 49,], [48, 55, 50,], [48, 55, 51,], [48, 55, 52,], [48, 55, 53,], [48, 55, 54,], [48, 55, 55,], [48, 55, 56,], [48, 55, 57,], [48, 56, 48,], [48, 56, 49,], [48, 56, 50,], [48, 56, 51,], [48, 56, 52,], [48, 56, 53,], [48, 56, 54,], [48, 56, 55,], [48, 56, 56,], [48, 56, 57,], [48, 57, 48,], [48, 57, 49,], [48, 57, 50,], [48, 57, 51,], [48, 57, 52,], [48, 57, 53,], [48, 57, 54,], [48, 57, 55,], [48, 57, 56,], [48, 57, 57,], [49, 48, 48,], [49, 48, 49,], [49, 48, 50,], [49, 48, 51,], [49, 48, 52,], [49, 48, 53,], [49, 48, 54,], [49, 48, 55,], [49, 48, 56,], [49, 48, 57,], [49, 49, 48,], [49, 49, 49,], [49, 49, 50,], [49, 49, 51,], [49, 49, 52,], [49, 49, 53,], [49, 49, 54,], [49, 49, 55,], [49, 49, 56,], [49, 49, 57,], [49, 50, 48,], [49, 50, 49,], [49, 50, 50,], [49, 50, 51,], [49, 50, 52,], [49, 50, 53,], [49, 50, 54,], [49, 50, 55,], [49, 50, 56,], [49, 50, 57,], [49, 51, 48,], [49, 51, 49,], [49, 51, 50,], [49, 51, 51,], [49, 51, 52,], [49, 51, 53,], [49, 51, 54,], [49, 51, 55,], [49, 51, 56,], [49, 51, 57,], [49, 52, 48,], [49, 52, 49,], [49, 52, 50,], [49, 52, 51,], [49, 52, 52,], [49, 52, 53,], [49, 52, 54,], [49, 52, 55,], [49, 52, 56,], [49, 52, 57,], [49, 53, 48,], [49, 53, 49,], [49, 53, 50,], [49, 53, 51,], [49, 53, 52,], [49, 53, 53,], [49, 53, 54,], [49, 53, 55,], [49, 53, 56,], [49, 53, 57,], [49, 54, 48,], [49, 54, 49,], [49, 54, 50,], [49, 54, 51,], [49, 54, 52,], [49, 54, 53,], [49, 54, 54,], [49, 54, 55,], [49, 54, 56,], [49, 54, 57,], [49, 55, 48,], [49, 55, 49,], [49, 55, 50,], [49, 55, 51,], [49, 55, 52,], [49, 55, 53,], [49, 55, 54,], [49, 55, 55,], [49, 55, 56,], [49, 55, 57,], [49, 56, 48,], [49, 56, 49,], [49, 56, 50,], [49, 56, 51,], [49, 56, 52,], [49, 56, 53,], [49, 56, 54,], [49, 56, 55,], [49, 56, 56,], [49, 56, 57,], [49, 57, 48,], [49, 57, 49,], [49, 57, 50,], [49, 57, 51,], [49, 57, 52,], [49, 57, 53,], [49, 57, 54,], [49, 57, 55,], [49, 57, 56,], [49, 57, 57,], [50, 48, 48,], [50, 48, 49,], [50, 48, 50,], [50, 48, 51,], [50, 48, 52,], [50, 48, 53,], [50, 48, 54,], [50, 48, 55,], [50, 48, 56,], [50, 48, 57,], [50, 49, 48,], [50, 49, 49,], [50, 49, 50,], [50, 49, 51,], [50, 49, 52,], [50, 49, 53,], [50, 49, 54,], [50, 49, 55,], [50, 49, 56,], [50, 49, 57,], [50, 50, 48,], [50, 50, 49,], [50, 50, 50,], [50, 50, 51,], [50, 50, 52,], [50, 50, 53,], [50, 50, 54,], [50, 50, 55,], [50, 50, 56,], [50, 50, 57,], [50, 51, 48,], [50, 51, 49,], [50, 51, 50,], [50, 51, 51,], [50, 51, 52,], [50, 51, 53,], [50, 51, 54,], [50, 51, 55,], [50, 51, 56,], [50, 51, 57,], [50, 52, 48,], [50, 52, 49,], [50, 52, 50,], [50, 52, 51,], [50, 52, 52,], [50, 52, 53,], [50, 52, 54,], [50, 52, 55,], [50, 52, 56,], [50, 52, 57,], [50, 53, 48,], [50, 53, 49,], [50, 53, 50,], [50, 53, 51,], [50, 53, 52,], [50, 53, 53,],];


        /// <summary>
        /// A RGB-Color
        /// </summary>
        public struct RGB(byte red, byte green, byte blue)
        {
            public byte red = red;
            public byte green = green;
            public byte blue = blue;

            public static explicit operator RGB(RGBA rgba) => new(rgba.red, rgba.green, rgba.blue);
            public static implicit operator RGBA(RGB rgb) => new(rgb.red, rgb.green, rgb.blue, 255);
        }

        /// <summary>
        /// A RGBA-Color with transparency
        /// </summary>
        public struct RGBA(byte red, byte green, byte blue, byte alpha)
        {
            public byte red = red;
            public byte green = green;
            public byte blue = blue;
            public byte alpha = alpha;

            public override readonly string ToString() => $"{red}; {green}; {blue}; {alpha}";
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
        /// <param name="rgb">The <see cref="RGB"/> colors</param>
        /// <param name="background">If <see langword="true"/>, the background color will be changed instead of the foreground color</param>
        /// <returns>A Color Code from the given <see cref="RGB"/> values</returns>
        public static string FromColor(RGB rgb, bool background = false) => !background ? $"\x1B[38;2;{rgb.red};{rgb.green};{rgb.blue}m" : $"←[48;2;{rgb.red};{rgb.green};{rgb.blue}m";

        /// <summary>
        /// Creates a Color Code with the given <see cref="RGBA"/> colors<br/>
        /// The alpha component will be ignored
        /// </summary>
        /// <param name="rgb">The <see cref="RGBA"/> colors</param>
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
        /// Blends to <see cref="RGBA"/> colors
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
            if (outAlpha == 0) return new RGBA(0, 0, 0, 0);

            // Compute the resulting color components
            float invOutAlpha = 1 / outAlpha;
            byte outR = (byte)((foreground.red * alphaF + background.red * alphaB * (1 - alphaF)) * invOutAlpha);
            byte outG = (byte)((foreground.green * alphaF + background.green * alphaB * (1 - alphaF)) * invOutAlpha);
            byte outB = (byte)((foreground.blue * alphaF + background.blue * alphaB * (1 - alphaF)) * invOutAlpha);
            byte outA = (byte)(outAlpha * 255);

            return new RGBA(outR, outG, outB, outA);
        }

        /// <summary>
        /// A space with a white foreground and black background
        /// </summary>
        public static readonly TextColor Empty = new(' ', new(255, 255, 255, 255), new(0, 0, 0, 255));

        public const string EmptyString = "\u001b[38;2;255;255;255m\u001b[48;2;0;0;0m \u001b[0m";

        public static readonly byte[] encodedEmptyString = Empty.Encoded;

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
            private readonly Dictionary<Vector2Int, TextColor> strings = [];

            private static readonly List<TextGrid> textGrids = [];

            private static readonly Dictionary<string, int> IdIndex = [];

            /// <summary>
            /// An empty <see cref="TextGrid"/>.<br/>
            /// Should be used when no grid is needed
            /// </summary>
            public static readonly TextGrid Empty = new("empty", Array.Empty<TextColor[]>());

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
            /// Creates a new <see cref="TextGrid"/> instance from this <see cref="TextGrid"/>
            /// </summary>
            /// <param name="cloneId">The new id of the clone</param>
            public void Clone(string cloneId) { new TextGrid(cloneId, strings); }

            /// <summary>
            /// The global id of this <see cref="TextGrid"/>
            /// </summary>
            public readonly string Id;

            /// <summary>
            /// Gets the grid Dictionary of this <see cref="TextGrid"/>
            /// </summary>
            public Dictionary<Vector2Int, TextColor> Strings { get => strings; }

            /// <summary>
            /// Creates a new <see cref="TextGrid"/> instance from a jagged-array of <see cref="TextColor"/>s
            /// </summary>
            /// <param name="id">The global id of this grid</param>
            /// <param name="textGrid">A jagged-array of <see cref="TextColor"/>s</param>
            public TextGrid(string id, TextColor[][] textGrid)
            {
                for (Vector2Int v = new(0, 0); v.y < textGrid.Length; v.y++)
                {
                    for (v.x = 0; v.x < textGrid[v.y].Length; v.x++)
                    {
                        strings[v] = textGrid[v.y][v.x];
                    }
                }
                this.Id = id;
                Index = textGrids.Count;
                IdIndex[id] = Index;
                textGrids.Add(this);
            }

            /// <summary>
            /// Creates a <see cref="TextGrid"/> from a grid dictionary
            /// </summary>
            /// <param name="strings">A text grid dictionary</param>
            public TextGrid(string id, Dictionary<Vector2Int, TextColor> strings)
            {
                this.strings = strings;

                Index = textGrids.Count;
                Id = id;
                IdIndex[id] = Index;
                textGrids.Add(this);
            }
        }

        public class Transform : ISerialization<Transform>
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
                if (IsRotationLocked) ma = Matrix3x2.CreateRotation(this.GlobalRotation.DegToRad());
            }

            /// <summary>
            /// A new Transform with the given parameters
            /// </summary>
            /// <param name="position">The position</param>
            /// <param name="rotationPivot">The pivot point that the object rotates around</param>
            /// <param name="rotation">The rotation of the object in degrees</param>
            /// <param name="parent">The parent of this transform</param>
            /// <param name="rotationLocked">If <see langword="true"/>, the rotation matrix will be locked to the current angle</param>
            public Transform(Vector2Int position, Vector2 rotationPivot, float rotation = 0, Transform? parent = null, bool rotationLocked = false)
            {
                Position = position;
                Parent = parent;
                Rotation = rotation;
                RotationPivot = rotationPivot;
                IsRotationLocked = rotationLocked;
                if (IsRotationLocked) ma = Matrix3x2.CreateRotation(this.GlobalRotation.DegToRad());
            }

            [JsonIgnore]
            private bool rotationLocked;

            [JsonRequired]
            /// <summary>
            /// If set to <see langword="true"/>, the rotation matrix will be locked to the current angle</br>
            /// otherwise the rotation matrix will be updated
            /// <summary/>
            public bool IsRotationLocked
            {
                get => rotationLocked; set
                {
                    if (value) ma = Matrix3x2.CreateRotation(this.GlobalRotation.DegToRad());
                    rotationLocked = value;
                }
            }

            public Vector2 Position;
            public Transform? Parent;
            public float Rotation;
            [Obsolete("The Scaling system is not implemented yet")] public Vector2Int Scale;
            public Vector2 RotationPivot;

            private Matrix3x2 ma;

            /// <summary>
            /// The global coordinates relative to the parent
            /// </summary>
            [JsonIgnore]
            public Vector2Int GlobalIntCoordinates { get => Parent != null ? (Vector2Int)(Parent.GlobalCoordinates + Position) : (Vector2Int)Position; }

            [JsonIgnore]
            public Vector2 GlobalCoordinates { get => Parent != null ? Vector2.Transform(Position, RotationMatrix) + Parent.GlobalCoordinates : Position; }

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
            /// Transforms a <see cref="TextGrid"/>
            /// </summary>
            /// <param name="grid">The <see cref="TextGrid"/> that should be transformed</param>
            public void TransformToOutput(int gridIndex, ITextRenderer.TextOutput output, Vector2Int offset)
            {
                Vector2 piv = GlobalRotationPivot;
                Matrix3x2 rot = RotationMatrix;

                var outputGrid = output.Grid;

                Vector2Int o = GlobalIntCoordinates + offset;

                foreach (var kvp in TextGrid.Get(gridIndex).Strings)
                {
                    Vector2Int a = (Vector2Int)(Vector2.Transform(kvp.Key - piv, rot) + piv) + o;

                    TextColor t = kvp.Value;
                    if (kvp.Value.bgColor.alpha != 255)
                    {
                        t.bgColor = outputGrid.TryGetValue(kvp.Key, out TextColor? value)
                            ? TextColor.AlphaBlend(kvp.Value.bgColor, value.bgColor)
                            : TextColor.AlphaBlend(kvp.Value.bgColor, TextColor.Empty.bgColor);
                    }
                    if (kvp.Value.fgColor.alpha != 255)
                    {
                        t.fgColor = outputGrid.TryGetValue(kvp.Key, out TextColor? value)
                            ? TextColor.AlphaBlend(kvp.Value.fgColor, value.bgColor)
                            : TextColor.AlphaBlend(kvp.Value.fgColor, TextColor.Empty.bgColor);
                    }

                    output[a] = t;
                }
            }

            public string Serialize() => JsonSerializer.Serialize(this);

            public static Transform? Deserialize(string data) => JsonSerializer.Deserialize<Transform>(data);
        }

        public bool IsHidden = false;
        public Transform Transformation;

        public void LoadGrid(string textGridId) => GridIndex = TextGrid.GetIndex(textGridId);
        public int GridIndex { get; set; }

        public readonly int Layer;

        /// <summary>
        /// The unique <see cref="Guid"/> of this <see cref="RenderObject"/>
        /// </summary>
        public readonly Guid GUID = Guid.NewGuid();

        /// <summary>
        /// A constructor that will automatically add the new <see cref="RenderObject"/> to the given <see cref="ITextRenderer.RenderLayers"/>
        /// </summary>
        /// <param name="renderLayers">A <see langword="ref"/> to a <see cref="ITextRenderer.RenderLayers"/> object that this <see cref="RenderObject"/> will be added to</param>
        /// <param name="transform">The <see cref="Transform"/> this text will be transformed by</param>
        /// <param name="text">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="isHidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        public RenderObject(ITextRenderer.RenderLayers renderLayers, Transform transform, string textGridId, int layer, bool isHidden = false)
        {
            Transformation = transform;
            GridIndex = TextGrid.GetIndex(textGridId);
            Layer = layer;
            IsHidden = isHidden;
            renderLayers.AddRenderObject(this);
        }

        /// <summary>
        /// A constructor that will create a new <see cref="RenderObject"/> without adding it to a <see cref="ITextRenderer.RenderLayers"/>
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> this text will be transformed by</param>
        /// <param name="text">The <see cref="TextGrid"/> that is getting rendered</param>
        /// <param name="layer">The layer number this <see cref="RenderObject"/> should be added to</param>
        /// <param name="hidden">If <see langword="true"/>, the <see cref="RenderObject"/> will be hidden</param>
        public RenderObject(Transform transform, string textGridId, int layer, bool hidden = false)
        {
            Transformation = transform;
            GridIndex = TextGrid.GetIndex(textGridId);
            Layer = layer;
            IsHidden = hidden;
        }

    }

    /// <summary>
    /// Used to create renderers that render to <see cref="TextOutput"/>s
    /// </summary>
    public interface ITextRenderer
    {
        /// <summary>
        /// A way to organize the layers that <see cref="RenderObject"/>s are rendered to
        /// It also allows you to hide certain layers
        /// </summary>
        public class RenderLayers
        {
            private readonly SortedDictionary<int, HashSet<RenderObject>> HiddenLayers = [];

            private readonly HashSet<int> hiddenLayerKeys = [];

            private readonly SortedDictionary<int, HashSet<RenderObject>> ShownLayers = [];

            /// <summary>
            /// Gets the <see cref="IEnumerator{T}"/> for Enumerating through all shown layers
            /// </summary>
            /// <returns><see cref="IEnumerator{T}"/> of all shown layers</returns>
            public IEnumerator<KeyValuePair<int, HashSet<RenderObject>>> GetEnumerator() => ShownLayers.GetEnumerator();

            /// <summary>
            /// Checks if a layer exists or not
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <returns><see langword="true"/> if the layer exists; otherwise <see langword="false"/></returns>
            public bool DoesLayerExist(int layer) => HiddenLayers.ContainsKey(layer) || ShownLayers.ContainsKey(layer);

            /// <summary>
            /// Checks if a layer is hidden
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <returns><see langword="true"/> if the layer is hidden; otherwise <see langword="false"/></returns>
            public bool IsLayerHidden(int layer) => hiddenLayerKeys.Contains(layer);

            /// <summary>
            /// Creates a new layer
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <param name="hidden">If <see langword="true"/>, the layer will be creates as a hidden layer</param>
            /// <returns><see langword="true"/> if the new layer was created; otherwise <see langword="false"/></returns>
            public bool CreateLayer(int layer, bool hidden = false)
            {
                if (hidden && !HiddenLayers.ContainsKey(layer)) { HiddenLayers[layer] = []; return true; }
                else if (!ShownLayers.ContainsKey(layer)) { ShownLayers[layer] = []; return true; }
                return false;
            }

            /// <summary>
            /// Adds a <see cref="RenderObject"/> to the given layer <br/>
            /// The <see cref="RenderObject"/>s are rendered in the order of when they where added to the layer
            /// </summary>
            /// <param name="obj">The <see cref="RenderObject"/> to add</param>
            public void AddRenderObject(RenderObject obj)
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

            /// <summary>
            /// Removes a certain <see cref="RenderObject"/> from the given layer
            /// </summary>
            /// <param name="obj">The <see cref="RenderObject"/> that should be removed</param>
            /// <param name="layer">The layer number</param>
            /// <returns><see langword="true"/> if the <see cref="RenderObject"/> was successfully removed from the layer; otherwise <see langword="false"/></returns>
            public bool RemoveRenderObject(RenderObject obj)
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

            /// <summary>
            /// Sets if a layer is hidden or not
            /// </summary>
            /// <param name="layer">The layer number</param>
            /// <param name="hide">If true, the layer should be hidden; otherwise the layer is shown</param>
            public void SetHiddenLayer(int layer, bool hide = true)
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
        }

        /// <summary>
        /// Stores the currently rendered frame as text<br/>
        /// A <see cref="TextOutput"/> is like a viewport and<br/>
        /// allows editing text at a certain coordinate
        /// </summary>
        public class TextOutput(IntRect rect)
        {
            private readonly Dictionary<Vector2Int, TextColor> grid = [];

            public Dictionary<Vector2Int, TextColor> Grid => grid;

            public IntRect rect = rect;

            /// <summary>
            /// Sets or gets the given <see cref="TextColor"/> at a certain coordinate
            /// </summary>
            /// <param name="at">A coordinate inside the grid</param>
            /// <returns>The <see cref="TextColor"/> at the given coordinates or <see cref="TextColor.Empty"/> if not found</returns>
            public virtual TextColor this[Vector2Int at]
            {
                get => rect.IsInRect(at) ? grid[at] : TextColor.Empty;
                set
                {
                    if (rect.IsInRect(at)) grid[at] = value;
                }
            }

            /// <summary>
            /// Clears the grid
            /// </summary>
            public virtual void Clear() => grid.Clear();

            /// <summary>
            /// Copys the contents of a <see cref="RenderObject.TextGrid"/> to the grid<br/>
            /// Also blends transparent colors using <see cref="TextColor.AlphaBlend(TextColor.RGBA, TextColor.RGBA)"/>
            /// </summary>
            /// <param name="textGrid">The <see cref="RenderObject.TextGrid"/> that is copied</param>
            public virtual void CopyTextGrid(RenderObject.TextGrid textGrid)
            {
                foreach (var kvp in textGrid.Strings)
                {
                    if (!rect.IsInRect(kvp.Key)) continue;
                    TextColor t = kvp.Value;
                    if (kvp.Value.bgColor.alpha != 255)
                    {
                        t.bgColor = !grid.TryGetValue(kvp.Key, out TextColor? value)
                            ? TextColor.AlphaBlend(kvp.Value.bgColor, TextColor.Empty.bgColor)
                            : TextColor.AlphaBlend(kvp.Value.bgColor, value.bgColor);
                    }
                    if (kvp.Value.fgColor.alpha != 255)
                    {
                        t.fgColor = !grid.TryGetValue(kvp.Key, out TextColor? value)
                            ? TextColor.AlphaBlend(kvp.Value.fgColor, TextColor.Empty.bgColor)
                            : TextColor.AlphaBlend(kvp.Value.fgColor, value.bgColor);
                    }
                    grid[kvp.Key] = t;
                }
            }

            /// <summary>
            /// Copys the contents of a grid dictionary to the grid<br/>
            /// Also blends transparent colors using <see cref="TextColor.AlphaBlend(TextColor.RGBA, TextColor.RGBA)"/>
            /// </summary>
            /// <param name="textGrid">The grid dictionary that is copied</param>
            public virtual void CopyTextGrid(Dictionary<Vector2Int, TextColor> textGrid)
            {
                foreach (var kvp in textGrid)
                {

                    if (!rect.IsInRect(kvp.Key)) continue;
                    TextColor t = kvp.Value;
                    if (kvp.Value.bgColor.alpha != 255)
                    {
                        t.bgColor = !grid.TryGetValue(kvp.Key, out TextColor? value)
                            ? TextColor.AlphaBlend(kvp.Value.bgColor, TextColor.Empty.bgColor)
                            : TextColor.AlphaBlend(kvp.Value.bgColor, value.bgColor);
                    }
                    if (kvp.Value.fgColor.alpha != 255)
                    {
                        t.fgColor = !grid.TryGetValue(kvp.Key, out TextColor? value)
                            ? TextColor.AlphaBlend(kvp.Value.fgColor, TextColor.Empty.bgColor)
                            : TextColor.AlphaBlend(kvp.Value.fgColor, value.bgColor);
                    }
                    grid[kvp.Key] = t;
                }
            }

            readonly StringBuilder resultBuilder = new();

            public override string ToString()
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
            }

            private readonly byte[] result = new byte[rect.size.y * rect.size.x * 43 + rect.size.y];

            public byte[] AsArray()
            {
                int index = 0;
                Vector2Int c = rect.Corner;
                for (Vector2Int v = rect.position; v.y < c.y; v.y++)
                {
                    for (v.x = rect.position.x; v.x < c.x; v.x++)
                    {
                        if (grid.TryGetValue(v, out TextColor? value))
                        {
                            value.Encoded.CopyTo(result, index);
                            index += 43;
                        }
                        else
                        {
                            TextColor.encodedEmptyString.CopyTo(result, index);
                            index += 43;
                        }
                    }
                    result[index] = 10;
                    index++;
                }
                return result;
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
    public class ConsoleRenderer : ITextRenderer
    {

        public static class FastConsole
        {
            public readonly static BufferedStream str;

            static FastConsole()
            {
                enc = Console.OutputEncoding;

                str = new BufferedStream(Console.OpenStandardOutput(), 0x100);
            }

            public static void Clear()
            {
                str.Flush();
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

        public ITextRenderer.RenderLayers Layers = new();

        public void AddRenderObject(RenderObject obj) => Layers.AddRenderObject(obj);

        public virtual void AfterRender(ITextRenderer.TextOutput output)
        {
            Console.SetCursorPosition(0, 0);
            FastConsole.Write(output.AsArray());

            Clear(output);
        }

        public Vector2Int Offset = Vector2Int.Zero;

        public virtual void Render(ITextRenderer.TextOutput output)
        {
            foreach (var layer in Layers)
            {
                foreach (var obj in layer.Value)
                {
                    if (obj.IsHidden) continue;
                    obj.Transformation.TransformToOutput(obj.GridIndex, output, Offset);
                }
            }
            AfterRender(output);
        }

        public virtual void Clear(ITextRenderer.TextOutput output)
        {
            output.Clear();
        }
    }
    #endregion
}
