using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;


namespace MaksIT.Core.UI.Snapshots;

/// <summary>
/// Saves a desktop view as a 96 DPI PNG. Each app decides which views to open;
/// this only waits for a frame and writes the pixels.
/// </summary>
public static class AppViewSnapshot {
  public static async Task SettleAsync(int milliseconds) {
    if (milliseconds > 0)
      await Task.Delay(milliseconds);

    await Dispatcher.UIThread.InvokeAsync(static () => { }, DispatcherPriority.Render);
  }

  /// <summary>File name stem for a view id. Letters, digits, hyphen, and underscore are kept.</summary>
  public static string FileToken(string id) {
    var buffer = new char[id.Length];

    for (var i = 0; i < id.Length; i++) {
      var c = id[i];
      buffer[i] = char.IsAsciiLetterOrDigit(c) || c is '-' or '_' ? c : '-';
    }

    var token = new string(buffer).Trim('-');

    return token.Length == 0 ? "view" : token;
  }

  /// <summary>
  /// Writes <paramref name="control"/> to <paramref name="path"/>.
  /// <paramref name="redact"/> is in the control's pixel space and is blurred.
  /// Returns the file name, or null when the control is not on screen.
  /// </summary>
  public static string? Save(Control? control, string path, PixelRect? redact = null) {
    if (control is not { IsEffectivelyVisible: true })
      return null;

    var width = control is Window host ? host.ClientSize.Width : control.Bounds.Width;
    var height = control is Window hostWindow ? hostWindow.ClientSize.Height : control.Bounds.Height;

    if (width < 2)
      width = control.Bounds.Width;

    if (height < 2)
      height = control.Bounds.Height;

    if (width < 2 || height < 2)
      return null;

    var size = new PixelSize(
      Math.Max(1, (int)Math.Ceiling(width)),
      Math.Max(1, (int)Math.Ceiling(height)));

    using var bitmap = new RenderTargetBitmap(size, new Vector(96, 96));
    bitmap.Render(control);
    var directory = Path.GetDirectoryName(path);

    if (!string.IsNullOrEmpty(directory))
      Directory.CreateDirectory(directory);

    if (redact is { } region)
      SaveRedacted(bitmap, size, region, path);
    else
      bitmap.Save(path, new PngBitmapEncoderOptions());

    return Path.GetFileName(path);
  }

  /// <summary>Control bounds in <paramref name="relativeTo"/> pixel space, or null when it is not placed.</summary>
  public static PixelRect? BoundsIn(Control control, Visual relativeTo) {
    ArgumentNullException.ThrowIfNull(control);
    ArgumentNullException.ThrowIfNull(relativeTo);
    var origin = control.TranslatePoint(new Point(0, 0), relativeTo);

    if (origin is null || control.Bounds.Width < 2 || control.Bounds.Height < 2)
      return null;

    return new PixelRect(
      (int)Math.Floor(origin.Value.X),
      (int)Math.Floor(origin.Value.Y),
      Math.Max(1, (int)Math.Ceiling(control.Bounds.Width)),
      Math.Max(1, (int)Math.Ceiling(control.Bounds.Height)));
  }

  private static void SaveRedacted(RenderTargetBitmap bitmap, PixelSize size, PixelRect region, string path) {
    using var output = new WriteableBitmap(size, new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);

    using (var frame = output.Lock()) {
      bitmap.CopyPixels(frame);
      Redact(frame, region);
    }

    output.Save(path, new PngBitmapEncoderOptions());
  }

  private static void Redact(ILockedFramebuffer frame, PixelRect region) {
    var width = frame.Size.Width;
    var height = frame.Size.Height;
    var stride = frame.RowBytes;
    var length = stride * height;

    if (length <= 0 || frame.Address == IntPtr.Zero)
      return;

    var pixels = new byte[length];
    Marshal.Copy(frame.Address, pixels, 0, length);

    if (frame.Format.BitsPerPixel == 32 && stride >= width * 4)
      BlurRegion(pixels, width, height, stride, region);
    else
      FillRegion(pixels, stride, region, width, height);

    Marshal.Copy(pixels, 0, frame.Address, length);
  }

  private static void BlurRegion(byte[] pixels, int width, int height, int stride, PixelRect region) {
    var left = Math.Clamp(region.X, 0, Math.Max(0, width - 1));
    var top = Math.Clamp(region.Y, 0, Math.Max(0, height - 1));
    var right = Math.Clamp(region.X + region.Width, left + 1, width);
    var bottom = Math.Clamp(region.Y + region.Height, top + 1, height);

    if (right - left < 2 || bottom - top < 2)
      return;

    const int radius = 16;
    const int passes = 3;
    var scratch = new byte[pixels.Length];

    for (var pass = 0; pass < passes; pass++) {
      BlurHorizontal(pixels, scratch, stride, left, right, top, bottom, radius);
      BlurVertical(scratch, pixels, stride, left, right, top, bottom, radius);
    }
  }

  private static void BlurHorizontal(
    byte[] source,
    byte[] dest,
    int stride,
    int left,
    int right,
    int top,
    int bottom,
    int radius) {
    var span = radius * 2 + 1;

    for (var y = top; y < bottom; y++) {
      var row = y * stride;

      for (var x = left; x < right; x++) {
        var b = 0;
        var g = 0;
        var r = 0;
        var a = 0;

        for (var dx = -radius; dx <= radius; dx++) {
          var sample = row + (Math.Clamp(x + dx, left, right - 1) * 4);
          b += source[sample];
          g += source[sample + 1];
          r += source[sample + 2];
          a += source[sample + 3];
        }

        var pixel = row + (x * 4);
        dest[pixel] = (byte)(b / span);
        dest[pixel + 1] = (byte)(g / span);
        dest[pixel + 2] = (byte)(r / span);
        dest[pixel + 3] = (byte)(a / span);
      }
    }
  }

  private static void BlurVertical(
    byte[] source,
    byte[] dest,
    int stride,
    int left,
    int right,
    int top,
    int bottom,
    int radius) {
    var span = radius * 2 + 1;

    for (var y = top; y < bottom; y++) {
      for (var x = left; x < right; x++) {
        var b = 0;
        var g = 0;
        var r = 0;
        var a = 0;

        for (var dy = -radius; dy <= radius; dy++) {
          var sample = (Math.Clamp(y + dy, top, bottom - 1) * stride) + (x * 4);
          b += source[sample];
          g += source[sample + 1];
          r += source[sample + 2];
          a += source[sample + 3];
        }

        var pixel = (y * stride) + (x * 4);
        dest[pixel] = (byte)(b / span);
        dest[pixel + 1] = (byte)(g / span);
        dest[pixel + 2] = (byte)(r / span);
        dest[pixel + 3] = (byte)(a / span);
      }
    }
  }

  private static void FillRegion(byte[] pixels, int stride, PixelRect region, int width, int height) {
    var left = Math.Clamp(region.X, 0, width);
    var top = Math.Clamp(region.Y, 0, height);
    var right = Math.Clamp(region.X + region.Width, left, width);
    var bottom = Math.Clamp(region.Y + region.Height, top, height);

    for (var y = top; y < bottom; y++) {
      for (var x = left; x < right; x++) {
        var pixel = (y * stride) + (x * 4);

        if (pixel + 3 >= pixels.Length)
          return;

        pixels[pixel] = 32;
        pixels[pixel + 1] = 36;
        pixels[pixel + 2] = 40;
        pixels[pixel + 3] = 255;
      }
    }
  }
}
