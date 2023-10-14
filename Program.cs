using conimg;
using SkiaSharp;

var path = args.Length >= 1 ? args[0] : "image.png";
var contrastRatio = args.Length >= 2 && float.TryParse(args[1], out var val) ? val : 0;

var imageBytes = File.ReadAllBytes(path);
var imageBitmap = SKBitmap.Decode(imageBytes);

var bitmapCopy = imageBitmap.Copy();

using var canvas = new SKCanvas(bitmapCopy);
using (var paint = new SKPaint())
{
    paint.ColorFilter = SKColorFilter.CreateHighContrast(
        false, SKHighContrastConfigInvertStyle.NoInvert, contrastRatio);
    canvas.DrawBitmap(imageBitmap, imageBitmap.Info.Rect, paint: paint);
}

ConsoleGraphics.Display(bitmapCopy);
