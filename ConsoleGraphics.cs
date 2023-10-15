using System.Diagnostics;
using System.Numerics;
using SkiaSharp;

namespace conimg;

public static class ConsoleGraphics
{
    private const char Block = '█';
    private static readonly ConsoleColor[] _consoleColors = Enum.GetValues<ConsoleColor>();

    private static readonly Vector3[] _consoleColorVectors = {
        Vector3.Zero,
        new(0, 0, 0.5451f),
        new(0, 0.3922f, 0),
        new(0, 0.5451f, 0.5451f),
        new(0.5451f, 0, 0),
        new(0.5451f, 0, 0.5451f),
        new(0.5451f, 0.5f, 0),
        new(0.5f, 0.5f, 0.5f),
        new(0.25f, 0.25f, 0.25f),
        new(0, 0, 1),
        new(0, 1, 0),
        new(0, 1, 1),
        new(1, 0, 0),
        new(1, 0, 1),
        new(1, 1, 0),
        new(1, 1, 1),
    };

    public static void Display(SKBitmap bitmap)
    {
        var originalColor = Console.ForegroundColor;
        var iWidth = bitmap.Width;
        var iHeight = bitmap.Height;
        var cWidth = Console.WindowWidth;
        var cHeight = Console.WindowHeight;

        var sw = Stopwatch.StartNew();

        for (int cY = 0; cY < cHeight - 1; cY++)
        {
            Console.SetCursorPosition(0, cY);
            var iY = (int)((double)iHeight * cY / cHeight);

            for (int cX = 0; cX < cWidth; cX++)
            {
                var iX = (int)((double)iWidth * cX / cWidth);

                var pixel = bitmap.GetPixel(iX, iY);
                var color = MapColor(pixel);

                Console.ForegroundColor = color;
                Console.Write(color is ConsoleColor.Black ? ' ' : Block);
            }
        }

        sw.Stop();
        Console.ForegroundColor = originalColor;
        Debug.WriteLine($"Completed in {sw.ElapsedTicks:N0} ticks");
    }

    private static Vector3 GetVector(SKColor pixel) =>
        new(Magnitude(pixel.Red), Magnitude(pixel.Green), Magnitude(pixel.Blue));

    private static float Magnitude(byte value) => value / 256f;

    private static int GetNearestColorIndex(Vector3 value)
    {
        var targetIndex = 0;
        var targetDistance = Vector3.Distance(_consoleColorVectors[0], value);

        for (int i = 1; i < _consoleColorVectors.Length; i++)
        {
            var currentVector = _consoleColorVectors[i];
            var currentDistance = Vector3.Distance(currentVector, value);
            if (currentDistance < targetDistance)
            {
                targetIndex = i;
                targetDistance = currentDistance;
            }
        }

        return targetIndex;
    }

    private static ConsoleColor MapColor(SKColor pixel)
    {
        var pixelVector = GetVector(pixel);
        var colorIndex = GetNearestColorIndex(pixelVector);
        return _consoleColors[colorIndex];
    }
}
