using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

class VertexBuffer<T> : IDisposable
	where T : struct
{
	private int ID;
	private bool disposedValue;

	public VertexBuffer(T[] data, int size)
	{
		ID = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
		GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.StaticDraw);
	}

	~VertexBuffer()
	{
		Dispose(false);
	}

	public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
	public void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

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