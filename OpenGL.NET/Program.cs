using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GLFW;
using Khronos;
using OpenGL;

class Program
{
	static void Main(string[] args)
	{
		Glfw.Init();

		Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
		Glfw.WindowHint(Hint.ContextVersionMajor, 4);
		Glfw.WindowHint(Hint.ContextVersionMinor, 3);
		Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
		Glfw.WindowHint(Hint.Doublebuffer, true);
		Glfw.WindowHint(Hint.Decorated, true);
		Glfw.WindowHint(Hint.OpenglForwardCompatible, true);

		Gl.DebugMessageCallback(GLDebugProc, IntPtr.Zero);

		Window window = Glfw.CreateWindow(900, 500, "Title", GLFW.Monitor.None, Window.None);
		Gl.Initialize();
		Glfw.MakeContextCurrent(window);

		float[] vertex = new float[6] {
			-0.5f, -0.5f,
			 0.0f,  0.5f,
			 0.5f, -0.5f
		};

		uint vb = Gl.GenBuffer();
		Gl.BindBuffer(BufferTarget.ArrayBuffer, vb);
		Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(vertex.Length * sizeof(float)), vertex, BufferUsage.StaticDraw);
		Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, false, sizeof(float) * 2, 0);

		Gl.EnableVertexAttribArray(0);

		uint shader = CreateShader("Standard");
		Gl.UseProgram(shader);

		while (!Glfw.WindowShouldClose(window))
		{
			//Gl.ClearColor(0.0f, (float)(new Random().NextDouble()), 1.0f, 1.0f);
			Gl.Clear(ClearBufferMask.ColorBufferBit);

			Gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

			Glfw.SwapBuffers(window);
			Glfw.PollEvents();
		}

		Glfw.DestroyWindow(window);
		Glfw.Terminate();
	}

	static uint CompileShader(ShaderType type, string src)
	{
		string[] str = new string[] { src };
		uint id = Gl.CreateShader(type);
		Gl.ShaderSource(id, str);
		Gl.CompileShader(id);

		int result;
		Gl.GetShader(id, ShaderParameterName.CompileStatus, out result);

		if(result == Gl.FALSE)
		{
			int lenght;
			Gl.GetShader(id, ShaderParameterName.InfoLogLength, out lenght);

			StringBuilder sb = new StringBuilder(lenght);
			Gl.GetShaderInfoLog(id, lenght, out _, sb);

			Console.WriteLine("Failed to compile " + type.ToString() + " " + sb.ToString());

			Gl.DeleteShader(id);

			return 0;
		}

		return id;
	}

	static uint CreateShader(string shaderName)
	{
		string vertSrc = File.ReadAllText(Environment.CurrentDirectory + @"\..\..\res\" + shaderName + ".vert");
		string fragSrc = File.ReadAllText(Environment.CurrentDirectory + @"\..\..\res\" + shaderName + ".frag");

		uint id = Gl.CreateProgram();
		uint vert = CompileShader(ShaderType.VertexShader, vertSrc);
		uint frag = CompileShader(ShaderType.FragmentShader, fragSrc);

		Gl.AttachShader(id, vert);
		Gl.AttachShader(id, frag);
		Gl.LinkProgram(id);
		Gl.ValidateProgram(id);

		//Gl.DetachShader(id, vert);
		//Gl.DetachShader(id, frag);

		int result;
		Gl.GetProgram(id, ProgramProperty.ValidateStatus, out result);

		if(result == Gl.FALSE)
		{
			int lenght;
			Gl.GetProgram(id, ProgramProperty.InfoLogLength, out lenght);

			StringBuilder sb = new StringBuilder(lenght);
			Gl.GetProgramInfoLog(id, lenght, out _, sb);

			Console.WriteLine("Failed to validate " + sb.ToString());

			Gl.DeleteProgram(id);

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
