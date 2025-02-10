

# Text Graphics Engine
A Rendering-Engine writen in C# that allows you to render text in your console.
But it is not just writing text! You are able to transfrom grids of text however you want.
In the console you can even use colors that support transparency!

![Example Image inside the Console](https://github.com/Mathias2246/Text-Graphics-Engine/blob/master/example.png)

*Why did i do this? I don't know...*

***
## Setup
All you have to do is put the *'TextGraphics.cs'* file into your project.
That's it.

## Example
TGE use *'RenderObjects'* for representing what to render at a certain location.
Each RenderObject can be added to RenderLayer.
A layer is prioritized by its id so the contents of layer 3 are overwriting all lower layers.
But all layers above layer 3 overwrite will overwrite it.

In TGE Transforms are able to move and rotate text grids.
Transforms can also have a parent Transform *(Optional)*.
If a Transform has a Parent all values are going to be relative to the position and rotation
of the parent and its parents.

Here is a very simple example on how to use TGE with a single RenderObject


    using TextGraphics;
    
    internal class Example
    {
	    public static ConsoleRenderer Renderer { get; set; } = new ConsoleRenderer();

	    public static ITextRenderer.TextOutput TextOutput { get; set; } = new ITextRenderer.TextOutput(
	        new IntRect(
	            new Vector2Int(0, 0),
	            new Vector2Int(Console.WindowWidth - 2, Console.WindowHeight - 2)
	            )
	        );

	    public static void Main(string[] args)
	    {
	        // Creates a new text grid with two red '#' characters side by side
	        new RenderObject.TextGrid("testGrid", 
	            [
	                [new TextColor('#', new TextColor.RGBA(255,0,0,255), new TextColor.RGBA(0,0,0,255)), new TextColor('#', new TextColor.RGBA(255,0,0,255), new TextColor.RGBA(0,0,0,255))],
	            ]
	        );

	        // Creates a new render object at the position 4, 4
	        // with the text grid and adds it to Layer 1 of the Renderer
	        RenderObject renderObject = new(
	            renderLayers: Renderer.Layers, // The layers instance to add the object to
	            transform: new RenderObject.Transform(
	                new Vector2Int(4, 4),
	                new Vector2Int(0, 0),
	                0.0f
	                ), // The Transform of the object
	            textGridId: "testGrid", // The id of the text grid to render
	            layer: 1, // The layer to render the object on
	            isHidden: false // Optional
	            );

	        // Starts a simple rendering loop
	        while (true)
	        {
	            Renderer.Render(TextOutput);
	        }
	    }
    }


***
## Loading Images as Grids
Maybe you want to load a png or bitmap into TGE so i wrote a simple solution using SixLabors.ImageSharp (https://github.com/SixLabors/ImageSharp)

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    
    namespace TextGraphics
    {
        public static class ImgToGrid
        {
	        // Used to Convert ImageSharps Rgba32 colors to TGE RGBA colors
            public static TextColor.RGBA ToTgeRgba(this Rgba32 color) => new(color.R, color.G, color.B, color.A);
    
            public static RenderObject.TextGrid TextGridFromImg(string path, string gridId)
            {
                string p = Path.GetFileName(path);
    
                Dictionary<Vector2Int, TextColor> tg = [];
                using (var image = Image.Load<Rgba32>(path)) {
                    for (int i = 0; i < image.Height; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
                        {
                            TextColor.RGBA c = image[j, i].ToTgeRgba();
                            // Pixels with an alpha value of 0 are ignored
                            if (c.alpha == 0) continue;
                            tg[new(j, i)] = new(' ', new(255, 255, 255, 255), c);
                        }
                    }
                }
                RenderObject.TextGrid t = new RenderObject.TextGrid(gridId, tg);
                return t;
            }
        }
    }


***


## ***This Project isn't finished.***

There is still plenty of room for improvement. And thats why i need your help!
So please give me suggestions on how to improve or fix things.
***
*Sorry for my bad English :(*
