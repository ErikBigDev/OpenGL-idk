using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

class Window : GameWindow
{
	public Window(int width, int height, string title) 
		: base(width, height, GraphicsMode.Default, title, GameWindowFlags.Default, DisplayDevice.Default, 4, 3, GraphicsContextFlags.Debug & GraphicsContextFlags.ForwardCompatible & GraphicsContextFlags.Embedded)
	{
		GL.DebugMessageCallback(GLDebugProc, IntPtr.Zero);
		Context.ErrorChecking = true;
	}

	float[] vertex;
	uint[] index;

	VertexArray vao;
	VertexBuffer<float> vbo;
	IndexBuffer ibo;

	Shader shader;

	Renderer renderer;

	protected override void OnLoad(EventArgs e)
	{
		vertex = new float[8] {
			-0.5f, -0.5f,
			 0.5f, -0.5f,
			 0.5f,	0.5f,
			-0.5f,  0.5f
		};

		index = new uint[6]
		{
			0, 1, 2,
			2, 3, 0
		};

		vbo = new VertexBuffer<float>(vertex, vertex.Length * sizeof(float));

		vao = new VertexArray();

		VertexBufferLayout layout = new VertexBufferLayout();
		layout.Push(2, VertexAttribPointerType.Float);

		vao.AddBuffer(vbo, layout);

		ibo = new IndexBuffer(index, index.Length);

		shader = new Shader("Standard");
		shader.Bind();

		shader.Uniform4("u_Color", Color4.Teal);

		renderer = new Renderer();

		base.OnLoad(e);
	}

	float r = 0.0f;

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		renderer.Clear();

		r += 0.03f;

		shader.Uniform4("u_Color", new Color4(Math.Abs(((int)r % 2) - (r - (int)r)), Color4.Teal.G, Color4.Teal.B, 1.0f));
		renderer.Draw(vao, ibo, shader);

		SwapBuffers();
		base.OnRenderFrame(e);
	}

	protected override void OnUnload(EventArgs e)
	{
		ibo.Dispose();
		vbo.Dispose();
		vao.Dispose();
		shader.Dispose();

		base.OnUnload(e);
	}

	public static void GLDebugProc(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
	{
		string strMessage = Marshal.PtrToStringAnsi(message);


		Console.WriteLine($"[{ (type == DebugType.DebugTypeError ? "**GL ERROR**" : type.ToString())}; {severity.ToString()}]: {strMessage}; From: {source.ToString()}");
		Debugger.Break();
	}
}
