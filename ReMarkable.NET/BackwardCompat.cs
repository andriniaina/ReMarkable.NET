using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReMarkable.NET.Unix.Driver.Display.Framebuffer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats.Tga;

namespace ReMarkable.NET.Graphics.BackwardCompatibility
{
    public static class ImageSharpBackwardCompatibility
    {
        public static Span<TPixel> GetPixelRowSpan<TPixel>(this Image<TPixel> image, int y) where TPixel : unmanaged, IPixel<TPixel>
        {
            return image.DangerousGetPixelRowMemory<TPixel>(y).Span;
        }
    }

}