using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
    public class Program
    {
        public static void Main()
        {
            var settings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "OpenTK Spinning Cube"
            };

            using var window = new CubeWindow(GameWindowSettings.Default, settings);
            window.Run();
        }
    }

    public class CubeWindow : GameWindow
    {
        private readonly float[] _vertices =
        {
            // Positions        // Colors
            -0.5f, -0.5f, -0.5f,  1f, 0f, 0f,
             0.5f, -0.5f, -0.5f,  0f, 1f, 0f,
             0.5f,  0.5f, -0.5f,  0f, 0f, 1f,
            -0.5f,  0.5f, -0.5f,  1f, 1f, 0f,
            -0.5f, -0.5f,  0.5f,  0f, 1f, 1f,
             0.5f, -0.5f,  0.5f,  1f, 0f, 1f,
             0.5f,  0.5f,  0.5f,  1f, 1f, 1f,
            -0.5f,  0.5f,  0.5f,  0f, 0f, 0f,
        };

        private readonly uint[] _indices =
        {
            0, 1, 2, 2, 3, 0, // back
            4, 5, 6, 6, 7, 4, // front
            0, 4, 7, 7, 3, 0, // left
            1, 5, 6, 6, 2, 1, // right
            3, 2, 6, 6, 7, 3, // top
            0, 1, 5, 5, 4, 0  // bottom
        };

        private int _vao, _vbo, _ebo, _shader;
        private float _angle;

        public CubeWindow(GameWindowSettings gws, NativeWindowSettings nws)
            : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Create VAO/VBO/EBO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // Vertex attributes (position 3 floats, color 3 floats)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 2 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Compile shaders
            string vertexShaderSource = @"
                #version 330 core
                layout(location = 0) in vec3 aPos;
                layout(location = 1) in vec3 aColor;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                out vec3 ourColor;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPos, 1.0);
                    ourColor = aColor;
                }";

            string fragmentShaderSource = @"
                #version 330 core
                in vec3 ourColor;
                out vec4 FragColor;
                void main()
                {
                    FragColor = vec4(ourColor, 1.0);
                }";

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            _shader = GL.CreateProgram();
            GL.AttachShader(_shader, vertexShader);
            GL.AttachShader(_shader, fragmentShader);
            GL.LinkProgram(_shader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(_shader);

            // Transformation matrices
            _angle += (float)e.Time; // rotate with time
            var model = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angle / 2f);
            var view = Matrix4.LookAt(new Vector3(2, 2, 2), Vector3.Zero, Vector3.UnitY);
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), Size.X / (float)Size.Y, 0.1f, 100f);

            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "projection"), false, ref projection);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }
    }