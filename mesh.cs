using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
	// mesh and loader based on work by JTalton; http://www.opentk.com/node/642

	public class Mesh
	{
		// data members
		Matrix4 M, V, P;
		Matrix4 transform, rotation, scale;
		const float PI = 3.1415926535f;

		static float r = 0.0f;

		public ObjVertex[] vertices;            // vertex positions, model space
		public ObjTriangle[] triangles;         // triangles (3 vertex indices)
		public ObjQuad[] quads;                 // quads (4 vertex indices)
		int vertexBufferId;                     // vertex buffer
		int triangleBufferId;                   // triangle buffer
		int quadBufferId;                       // quad buffer

		// constructor
		public Mesh(string fileName)
		{
			MeshLoader loader = new MeshLoader();
			loader.Load(this, fileName);

		}
		public void MVP()
		{
			/*
			M = transform * rotation * scale;
			float angle90degrees = PI / 2;
			V =  Matrix4.CreateTranslation(new Vector3(0, 14.5f, 0)) * Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), angle90degrees);
			Matrix4 vertex = P * V * M * vertex;*/

			//We use an Identity matrix for the model
			Matrix4 _model = Matrix4.Identity;

			Matrix4 _projection = Matrix4.CreatePerspectiveFieldOfView(1.0f, 4.0f / 3.0f, 0.1f, 100.0f);

			//Camera will be as 4,3,3 looking at origin, and set up direction to 1
			Matrix4 _view = Matrix4.LookAt(new Vector3(4, 3, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			//combine into our model view projection
			Matrix4 _model_view_projection = _model * _view * _projection;
		}

		// initialization; called during first render
		public void Prepare(Shader shader)
		{
			if (vertexBufferId == 0)
			{

				// generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
				GL.GenBuffers(1, out vertexBufferId);
				GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(ObjVertex))), vertices, BufferUsageHint.StaticDraw);

				// generate triangle index array
				GL.GenBuffers(1, out triangleBufferId);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
				GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf(typeof(ObjTriangle))), triangles, BufferUsageHint.StaticDraw);

				// generate quad index array
				GL.GenBuffers(1, out quadBufferId);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
				GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf(typeof(ObjQuad))), quads, BufferUsageHint.StaticDraw);
			}
		}

		// render the mesh using the supplied shader and matrix
		public void Render(Shader shader, Matrix4 transform, Texture texture)
		{
			// set model view matrix
			GL.Frustum(-1.0f, 1.0f, -1.0f, 1.0f, 1.0f, 15.0f);
			GL.Translate(0, 0, -2);
			GL.Rotate(r, 0, 1, 0);

			// on first run, prepare buffers
			Prepare(shader);

			// safety dance
			GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

			// enable texture
			int texLoc = GL.GetUniformLocation(shader.programID, "pixels");
			GL.Uniform1(texLoc, 0);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture.id);

			// enable shader
			GL.UseProgram(shader.programID);

			// pass transform to vertex shader
			GL.UniformMatrix4(shader.uniform_mview, false, ref transform);

			// enable position, normal and uv attributes
			GL.EnableVertexAttribArray(shader.attribute_vpos);
			GL.EnableVertexAttribArray(shader.attribute_vnrm);
			GL.EnableVertexAttribArray(shader.attribute_vuvs);

			// bind interleaved vertex data
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
			GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf(typeof(ObjVertex)), IntPtr.Zero);

			// link vertex attributes to shader parameters 
			GL.VertexAttribPointer(shader.attribute_vuvs, 2, VertexAttribPointerType.Float, false, 32, 0);
			GL.VertexAttribPointer(shader.attribute_vnrm, 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
			GL.VertexAttribPointer(shader.attribute_vpos, 3, VertexAttribPointerType.Float, false, 32, 5 * 4);

			// bind triangle index data and render
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
			GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length * 3);

			// bind quad index data and render
			if (quads.Length > 0)
			{
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
				GL.DrawArrays(PrimitiveType.Quads, 0, quads.Length * 4);
			}

			// restore previous OpenGL state
			GL.UseProgram(0);
			GL.PopClientAttrib();
			r += 0.1f;
		}

		// layout of a single vertex
		[StructLayout(LayoutKind.Sequential)]
		public struct ObjVertex
		{
			public Vector2 TexCoord;
			public Vector3 Normal;
			public Vector3 Vertex;
		}

		// layout of a single triangle
		[StructLayout(LayoutKind.Sequential)]
		public struct ObjTriangle
		{
			public int Index0, Index1, Index2;
		}

		// layout of a single quad
		[StructLayout(LayoutKind.Sequential)]
		public struct ObjQuad
		{
			public int Index0, Index1, Index2, Index3;
		}
	}
}