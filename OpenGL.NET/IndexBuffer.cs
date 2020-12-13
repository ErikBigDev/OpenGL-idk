using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

class IndexBuffer : IDisposable
{
	private int ID;
	private int Count;
	private bool disposedValue;

	public IndexBuffer(uint[] data, int count)
	{
		Count = count;

		ID = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
		GL.BufferData(BufferTarget.ElementArrayBuffer, count * sizeof(uint), data, BufferUsageHint.StaticDraw);
	}

	~IndexBuffer()
	{
		Dispose(false);
	}

	public int GetCount() => Count;

	public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
	public void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}
			GL.DeleteBuffer(ID);

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}