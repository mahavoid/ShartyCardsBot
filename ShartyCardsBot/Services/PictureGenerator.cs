using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InputFiles;

namespace ShartyCardsBot.Services
{
    public class PictureGenerator : IPictureGenerator
    {
        public InputOnlineFile Generate(PictureGenerationOptions pgo)
        {
            using (Image<Rgba32> img1 = Image.Load<Rgba32>("source_images/flowers/bouquet_PNG61.png")) // load up source images
            using (Image<Rgba32> img2 = Image.Load<Rgba32>("source_images/animals/shark_PNG18829.png"))
            using (Image<Rgba32> outputImage = new Image<Rgba32>(300, 200)) // create output image of the correct dimensions
            {
                var _rnd  = new Random(DateTime.Now.Millisecond + 1);
                img1.Mutate(o => o.Rotate(Convert.ToSingle(_rnd.NextDouble() * 360)));
                img2.Mutate(o => o.Rotate(Convert.ToSingle(_rnd.NextDouble() * 360)));

                img1.Mutate(o => o.Resize(new Size(150, 100)));
                img2.Mutate(o => o.Resize(new Size(150, 100)));

                // take the 2 source images and draw them onto the image
                Point target1 = new Point(
                        _rnd.Next(outputImage.Width - img1.Width),
                        _rnd.Next(outputImage.Height - img1.Height));

                Point target2 = new Point(
                        _rnd.Next(outputImage.Width - img2.Width),
                        _rnd.Next(outputImage.Height - img2.Height));

                outputImage.Mutate(o => o
                    .DrawImage(img1, target1, 1f) // draw the first one top left
                    .DrawImage(img2, target2, 1f) // draw the second next to it
                );

                var text = "С ДНЁМ ДНЯ";

                //outputImage.Mutate(o => o.DrawText(text,)

                MemoryStream ms = new MemoryStream();
                outputImage.Save(ms, new PngEncoder());

                if (ms.CanSeek) ms.Seek(0, SeekOrigin.Begin);

                InputOnlineFile iof = new InputOnlineFile(ms);

                return iof;
            }
        }
    }
}
