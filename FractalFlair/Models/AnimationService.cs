using AnimatedGif;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FractalFlair.Models;

public class AnimationService
{
  public async Task<BitmapImage> AnimateBitmapsAsync(Bitmap[] bitmaps, string filePath)
  {
    var first = bitmaps.First();
    var last = bitmaps.Last();
    var reverseList = bitmaps.Where(b => b != first && b != last).Reverse().ToArray();

    using (var gif = AnimatedGif.AnimatedGif.Create(filePath, 110))
    {
      foreach (var bitmap in bitmaps)
        await gif.AddFrameAsync(bitmap, -1, GifQuality.Bit8);

      foreach (var bitmap in reverseList)
        await gif.AddFrameAsync(bitmap, -1, GifQuality.Bit8);
    }

    //using (var gif = File.OpenWrite(filePath))
    //{
    //  using var encoder = new GifEncoder(gif);
    //  encoder.FrameDelay = TimeSpan.FromMilliseconds(110d);
    //  foreach (var bitmap in bitmaps)
    //    encoder.AddFrame(bitmap);

    //  foreach (var bitmap in reverseList)
    //    encoder.AddFrame(bitmap);
    //}

    var bitmapImage = new BitmapImage();
    bitmapImage.BeginInit();
    bitmapImage.UriSource = new Uri(filePath);
    bitmapImage.EndInit();
    return bitmapImage;
  }

  public BitmapImage BitmapImageFromBytes(byte[] bytes)
  {
    var bitmapImage = new BitmapImage();
    using var stream = new MemoryStream(bytes);
    bitmapImage.BeginInit();
    bitmapImage.StreamSource = stream;
    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
    bitmapImage.EndInit();
    bitmapImage.Freeze();

    return bitmapImage;
  }
}