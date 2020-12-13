using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

struct VertexBufferElement
{
	public VertexBufferElement(int count, VertexAttribPointerType type, bool nomalized)
	{
		this.count = count;
		this.type = type;
		this.normalized = nomalized;
	}

	public int count;
	public VertexAttribPointerType type;
	public bool normalized;

	public static int SizeOf(VertexAttribPointerType type)
	{
		switch (type)
		{
			case VertexAttribPointerType.Byte:
				return sizeof(sbyte);
			case VertexAttribPointerType.UnsignedByte:
				return sizeof(byte);
			case VertexAttribPointerType.Short:
				return sizeof(short);
			case VertexAttribPointerType.UnsignedShort:
				return sizeof(ushort);
			case VertexAttribPointerType.Int:
				return sizeof(int);
			case VertexAttribPointerType.UnsignedInt:
				return sizeof(uint);
			case VertexAttribPointerType.Float:
				return sizeof(float);
			case VertexAttribPointerType.Double:
				return sizeof(double);
			default:
				throw new InvalidEnumArgumentException(type.ToString());
		}
	}
}

class VertexBufferLayout
{
	public VertexBufferLayout()
	{
		Elements = new List<VertexBufferElement>();
	}

	private List<VertexBufferElement> Elements;
	public int Stride { get; private set; } = 0;

	public void Push(int count, VertexAttribPointerType type)
	{
		Elements.Add(new VertexBufferElement(count, type, false));
		Stride += VertexBufferElement.SizeOf(type) * count;
	}

	public List<VertexBufferElement> GetElements() => Elements;
}
