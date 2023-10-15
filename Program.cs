using System.Diagnostics;
using conimg;
using SkiaSharp;

Console.Clear();

var path = args.Length >= 1 ? args[0] : "image.png";
var contrastRatio = args.Length >= 2 && float.TryParse(args[1], out var val) ? val : 0;

if (path == "--video")
{
    RunVideo(contrastRatio);
}
else if (File.Exists(path))
{
    RunSingleImage(path, contrastRatio);
}

void RunVideo(float contrast)
{
    var ffmpeg = Process.Start(new ProcessStartInfo("ffmpeg",
        // Don't spam stdout
        "-hide_banner -loglevel error" +
        // Pull from first video device on linux
        " -f v4l2 -i /dev/video0" +
        // Use mjpeg to write a jpg at 10fps
        " -vf fps=fps=10 -update 1 -y frame.jpg")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
    })!;

    ffmpeg.OutputDataReceived += (_, e) => Debug.WriteLine(e.Data);
    ffmpeg.ErrorDataReceived += (_, e) => Debug.WriteLine(e.Data);

    AppDomain.CurrentDomain.ProcessExit += (_, _) => ffmpeg.Kill();

    bool loop = true;
    Console.CancelKeyPress += (_, _) => loop = false;

    try
    {
        int frameCount = 0;
        while (loop)
        {
            Thread.Sleep(10);
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write($"Frame {frameCount}");

            try
            {
                RunSingleImage("frame.jpg", contrast);
            }
            catch
            {
            }

            frameCount++;
        }
    }
    finally
    {
        ffmpeg.Kill();
    }
}

void RunSingleImage(string filePath, float contrast)
{
    var imageBytes = File.ReadAllBytes(filePath);
    var imageBitmap = SKBitmap.Decode(imageBytes);

    var bitmapCopy = imageBitmap.Copy();

    using var canvas = new SKCanvas(bitmapCopy);
    using (var paint = new SKPaint())
    {
        paint.ColorFilter = SKColorFilter.CreateHighContrast(
            false, SKHighContrastConfigInvertStyle.NoInvert, contrast);
        canvas.DrawBitmap(imageBitmap, imageBitmap.Info.Rect, paint: paint);
    }

    ConsoleGraphics.Display(bitmapCopy);
}
