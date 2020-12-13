using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using OpenTK.Graphics.OpenGL4;

class Texture : IDisposable
{
	public Texture(string name)
	{
		ID = GL.GenTexture();
		GL.BindTexture(TextureTarget.Texture2D, ID);

		image = Image.Load<Rgba32>(Environment.CurrentDirectory + @"..\..\res\textures\" + name);
		
		Width = image.Width;
		Height = image.Height;

		image.Mutate(x => x.Flip(FlipMode.Horizontal));
		Rgba32[] tempPixels = image.GetPixelRowSpan(0).ToArray();

		foreach (Rgba32 p in tempPixels)
		{
			pixels.Add(p.R);
			pixels.Add(p.G);
			pixels.Add(p.B);
			pixels.Add(p.A);
		}

		GL.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
		GL.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
		GL.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
		GL.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
	}

	~Texture()
	{
		Dispose(disposing: false);
	}

	int ID;
	string FilePath;

	Image<Rgba32> image;
	List<byte> pixels = new List<byte>();
	private bool disposedValue;

	public int Width { get; private set; } = 0;
	public int Height { get; private set; } = 0;

	public void Bind(int slot = 0)
	{
		GL.ActiveTexture(TextureUnit.Texture0 + slot);
		GL.BindTexture(TextureTarget.Texture2D, ID);
	}
	public void Unbind() => GL.BindTexture(TextureTarget.Texture2D, 0);

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}
			GL.DeleteTexture(ID);

			disposedValue = true;
		}
	}



	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
