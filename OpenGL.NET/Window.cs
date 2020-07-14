using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

class Window : GameWindow
{
	public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
	{
		
	}

	float[] vertex;

	int vbo;
	int vao;

	int shader;

	protected override void OnLoad(EventArgs e)
	{
		vertex = new float[6] {
			-0.5f, -0.5f,
			 0.0f,  0.5f,
			 0.5f, -0.5f
		};

		vao = GL.GenVertexArray();
		GL.BindVertexArray(vao);

		vbo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex, BufferUsageHint.StaticDraw);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);

		GL.EnableVertexAttribArray(0);

		shader = CreateShader("Standard");
		GL.UseProgram(shader);

		
		base.OnLoad(e);
	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		GL.ClearColor(Color.FromArgb(100, 200, 230));
		GL.Clear(ClearBufferMask.ColorBufferBit);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

		SwapBuffers();
		base.OnRenderFrame(e);
	}
	static int CompileShader(uint type, string src)
	{
		int id = GL.CreateShader((ShaderType)type);
		if(id == 0)
			Console.WriteLine("000000000000");
		GL.ShaderSource(id, src);
		GL.CompileShader(id);

		int result;
		GL.GetShader(id, ShaderParameter.CompileStatus, out result);

		if(result == 0)
		{
			int lenght;
			GL.GetShader(id, ShaderParameter.InfoLogLength, out lenght);
			//GL.GetShader(id, ShaderParameterName.ShaderSourceLength, out lenght);

			string infoLog;
			GL.GetShaderInfoLog(id, out infoLog);
			//GL.GetShaderSource(id, lenght, out _, sb);

			Console.WriteLine($"Failed to compile {type.ToString()}  {infoLog}");
			Console.WriteLine("\n" + src);

			GL.DeleteShader(id);

			return 0;
		}

		return id;
	}

	static int CreateShader(string shaderName)
	{
		string vertSrc = File.ReadAllText(Environment.CurrentDirectory + @"\..\..\res\" + shaderName + ".vert");
		string fragSrc = File.ReadAllText(Environment.CurrentDirectory + @"\..\..\res\" + shaderName + ".frag");

		int id = GL.CreateProgram();
		int vert = CompileShader((uint)ShaderType.VertexShader, vertSrc);
		int frag = CompileShader((uint)ShaderType.FragmentShader, fragSrc);

		GL.AttachShader(id, vert);
		GL.AttachShader(id, frag);
		GL.LinkProgram(id);
		GL.ValidateProgram(id);

		//GL.DetachShader(id, vert);
		//GL.DetachShader(id, frag);

		int result;
		GL.GetProgram(id, GetProgramParameterName.ValidateStatus, out result);

		if(result == 0)
		{
			int lenght;
			GL.GetProgram(id, GetProgramParameterName.InfoLogLength, out lenght);

			string infoLog;
			GL.GetProgramInfoLog(id, out infoLog);

			Console.WriteLine("Failed to validate " + infoLog);

			GL.DeleteProgram(id);

			return 0;
		}

		return id;
	}

	private static void GLDebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
	{
		string strMessage;
		unsafe
		{
			strMessage = Encoding.ASCII.GetString((byte*)message.ToPointer(), length);
		}

		Console.WriteLine($"[{ (type == DebugType.DebugTypeError ? "**GL ERROR**" : type.ToString())}; {severity.ToString()}]: {strMessage}; From: {source.ToString()}");
		Debugger.Break();
	}

}
