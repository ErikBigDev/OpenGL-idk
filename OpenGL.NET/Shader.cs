using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

class Shader : IDisposable
{
	private bool disposedValue;

	public Shader(string name)
	{
		ID = CreateShader(name);
		LocationCache = new Dictionary<string, int>();
	}

	~Shader()
	{
		Dispose(false);
	}

	private int ID;
	private Dictionary<string, int> LocationCache;

	public void Bind() => GL.UseProgram(ID);
	public void Unbind() => GL.UseProgram(0);
		
	private int CompileShader(ShaderType type, string src)
	{
		int id = GL.CreateShader(type);
		GL.ShaderSource(id, src);
		GL.CompileShader(id);

		int result;
		GL.GetShader(id, ShaderParameter.CompileStatus, out result);

		if (result == 0)
		{
			string infoLog;
			GL.GetShaderInfoLog(id, out infoLog);

			Console.WriteLine($"Failed to compile {type.ToString()}  {infoLog}");
			Console.WriteLine("\n" + src);

			GL.DeleteShader(id);

			return 0;
		}

		return id;
	}

	private int CreateShader(string shaderName)
	{
		string vertSrc = File.ReadAllText(Environment.CurrentDirectory + @"\..\..\res\shaders\" + shaderName + ".vert");
		string fragSrc = File.ReadAllText(Environment.CurrentDirectory + @"\..\..\res\shaders\" + shaderName + ".frag");

		int id = GL.CreateProgram();
		int vert = CompileShader(ShaderType.VertexShader, vertSrc);
		int frag = CompileShader(ShaderType.FragmentShader, fragSrc);

		GL.AttachShader(id, vert);
		GL.AttachShader(id, frag);
		GL.LinkProgram(id);
		GL.ValidateProgram(id);

		//GL.DetachShader(id, vert);
		//GL.DetachShader(id, frag);

		int result;
		GL.GetProgram(id, GetProgramParameterName.ValidateStatus, out result);

		if (result == 0)
		{
			string infoLog;
			GL.GetProgramInfoLog(id, out infoLog);

			Console.WriteLine("Failed to validate " + infoLog);

			GL.DeleteProgram(id);

			return 0;
		}

		return id;
	}

	public void Uniform4(string name, Color4 value) => GL.Uniform4(GetUniformLocation(name), value);
	public void Uniform4(string name, Quaternion value) => GL.Uniform4(GetUniformLocation(name), value);
	public void Uniform4(string name, Vector4 value) => GL.Uniform4(GetUniformLocation(name), value);

	public int GetUniformLocation(string name)
	{
		if (LocationCache.ContainsKey("name"))
			return LocationCache[name];

		int location = GL.GetUniformLocation(ID, name);

		if(location == -1)
			Console.WriteLine($"[Warning]: Uniform '{name}' does not exist!");

		LocationCache[name] = location;

		return location;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}
			GL.DeleteShader(ID);
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
