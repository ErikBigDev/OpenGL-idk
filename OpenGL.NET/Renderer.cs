using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

class Renderer
{
	public Renderer()
	{

	}

	public void Draw(VertexArray va, IndexBuffer ib, Shader shader)
	{
		va.Bind();
		ib.Bind();
		shader.Bind();

		GL.DrawElements(PrimitiveType.Triangles, ib.GetCount(), DrawElementsType.UnsignedInt, IntPtr.Zero);
	}

	public void Clear() => GL.Clear(ClearBufferMask.ColorBufferBit);
}
