using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

class VertexArray : IDisposable
{
	public VertexArray()
	{
		ID = GL.GenVertexArray();
	}

	~VertexArray()
	{
		Dispose(false);
	}

	private int ID;
	private bool disposedValue;

	public void Bind() => GL.BindVertexArray(ID);
	public void Unbind() => GL.BindVertexArray(0);

	public void AddBuffer<T>(VertexBuffer<T> vb, VertexBufferLayout layout) where T : struct
	{
		Bind();
		vb.Bind();

		List<VertexBufferElement> elements = layout.GetElements();
		int offest = 0;

		for (int i = 0; i < elements.Count; i++)
		{
			var element = elements[i];
			GL.EnableVertexAttribArray(i);
			GL.VertexAttribPointer(i, element.count, element.type, element.normalized, layout.Stride, 0);

			offest += element.count * VertexBufferElement.SizeOf(element.type);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}
			GL.DeleteVertexArray(ID);

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
