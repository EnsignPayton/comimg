using conimg;
using SkiaSharp;

var pngBytes = File.ReadAllBytes("image.png");
var img = SKBitmap.Decode(pngBytes);
var imgCopy = img.Copy();
var con = SKColorFilter.CreateHighContrast(false, SKHighContrastConfigInvertStyle.NoInvert, 0.2f);

using var canvas = new SKCanvas(imgCopy);
using (var paint = new SKPaint())
{
    paint.ColorFilter = con;
    canvas.DrawBitmap(img, img.Info.Rect, paint: paint);
}

ConsoleGraphics.Display(imgCopy);
