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
    //private readonly float[] cube_vertices =
    //{
    //    //the points of the cube
    //    // Positions          // Colors
    //    -0.5f, -0.5f, -0.5f,  1f, 0f, 0f, 
    //     0.5f, -0.5f, -0.5f,  0f, 0f, 0f,
    //     0.5f,  0.5f, -0.5f,  0f, 0f, 1f,
    //    -0.5f,  0.5f, -0.5f,  1f, 1f, 0f,
    //    -0.5f, -0.5f,  0.5f,  0f, 0f, 1f,
    //     0.5f, -0.5f,  0.5f,  1f, 0f, 1f,
    //     0.5f,  0.5f,  0.5f,  1f, 1f, 1f,
    //    -0.5f,  0.5f,  0.5f,  0f, 0f, 0f,
    //};



    //private readonly uint[] cube_indices =
    //{

    //    //so basically the face of the cubes
    //    0, 1, 2, 2, 3, 0, // back
    //    4, 5, 6, 6, 7, 4, // front
    //    0, 4, 7, 7, 3, 0, // left
    //    1, 5, 6, 6, 2, 1, // right
    //    3, 2, 6, 6, 7, 3, // top
    //    0, 1, 5, 5, 4, 0  // bottom
    //};

    //private readonly float[] prism_vertices =
    //{
    //    //for prism
    //    // Positions              // Colors
    //    // Base triangle (z = -0.5)
    //    -0.5f, -0.5f, -0.5f,      1f, 0f, 0f, // 0
    //     0.5f, -0.5f, -0.5f,      0f, 1f, 0f, // 1
    //     0.0f,  0.5f, -0.5f,      0f, 0f, 1f, // 2

    //    // Top triangle (z = +0.5)
    //    -0.5f, -0.5f,  0.5f,      1f, 1f, 0f, // 3
    //     0.5f, -0.5f,  0.5f,      0f, 1f, 1f, // 4
    //     0.0f,  0.5f,  0.5f,      1f, 0f, 1f  // 5

    //};

    //    private readonly uint[] prism_indices =
    //{
    //    // Base
    //    0, 1, 2,
    //    // Top
    //    3, 5, 4,

    //    // Sides
    //    0, 1, 4, 4, 3, 0, // side 1
    //    1, 2, 5, 5, 4, 1, // side 2
    //    2, 0, 3, 3, 5, 2  // side 3
    //};

    //    private readonly float[] pyramid_vertices =
    //{
    //    // Positions              // Colors
    //    // Base (z = -0.5)
    //    -0.5f, -0.5f, -0.5f,      1f, 0f, 0f, // 0
    //     0.5f, -0.5f, -0.5f,      0f, 1f, 0f, // 1
    //     0.5f,  0.5f, -0.5f,      0f, 0f, 1f, // 2
    //    -0.5f,  0.5f, -0.5f,      1f, 1f, 0f, // 3

    //    // Apex (z = +0.5)
    //     0.0f,  0.0f,  0.5f,      1f, 0f, 1f  // 4
    //};

    //    private readonly uint[] pyramid_indices =
    //    {
    //    // Base (two triangles)
    //    0, 1, 2,
    //    2, 3, 0,

    //    // Sides
    //    0, 1, 4,
    //    1, 2, 4,
    //    2, 3, 4,
    //    3, 0, 4
    //};

    // 88 vertices total (11 blocks × 8 verts per block)
    private readonly float[] model_vertices =
    {
    // -------- Block 1 --------
    -0.5f,-0.5f,-0.2f,   1f,0f,0f,
    -0.5f,-0.3f,-0.2f,   1f,0f,0f,
    -0.3f,-0.5f,-0.2f,   1f,0f,0f,
    -0.3f,-0.3f,-0.2f,   1f,0f,0f,
    -0.5f,-0.5f, 0.0f,   1f,0f,0f,
    -0.5f,-0.3f, 0.0f,   1f,0f,0f,
    -0.3f,-0.5f, 0.0f,   1f,0f,0f,
    -0.3f,-0.3f, 0.0f,   1f,0f,0f,

    // -------- Block 2 --------
    -0.5f, 0.5f,-0.2f,   0f,1f,0f,
    -0.5f, 0.3f,-0.2f,   0f,1f,0f,
    -0.3f, 0.5f,-0.2f,   0f,1f,0f,
    -0.3f, 0.3f,-0.2f,   0f,1f,0f,
    -0.5f, 0.5f, 0.0f,   0f,1f,0f,
    -0.5f, 0.3f, 0.0f,   0f,1f,0f,
    -0.3f, 0.5f, 0.0f,   0f,1f,0f,
    -0.3f, 0.3f, 0.0f,   0f,1f,0f,

    // -------- Block 3 --------
     0.6f,-0.5f,-0.2f,   0f,0f,1f,
     0.6f,-0.3f,-0.2f,   0f,0f,1f,
     0.4f,-0.5f,-0.2f,   0f,0f,1f,
     0.4f,-0.3f,-0.2f,   0f,0f,1f,
     0.6f,-0.5f, 0.0f,   0f,0f,1f,
     0.6f,-0.3f, 0.0f,   0f,0f,1f,
     0.4f,-0.5f, 0.0f,   0f,0f,1f,
     0.4f,-0.3f, 0.0f,   0f,0f,1f,

    // -------- Block 4 --------
     0.6f, 0.5f,-0.2f,   1f,1f,0f,
     0.6f, 0.3f,-0.2f,   1f,1f,0f,
     0.4f, 0.5f,-0.2f,   1f,1f,0f,
     0.4f, 0.3f,-0.2f,   1f,1f,0f,
     0.6f, 0.5f, 0.0f,   1f,1f,0f,
     0.6f, 0.3f, 0.0f,   1f,1f,0f,
     0.4f, 0.5f, 0.0f,   1f,1f,0f,
     0.4f, 0.3f, 0.0f,   1f,1f,0f,

    // -------- Block 5 --------
     0.7f, 0.7f, 0.0f,   0f,1f,1f,
     0.7f,-0.7f, 0.0f,   0f,1f,1f,
    -0.7f, 0.7f, 0.0f,   0f,1f,1f,
    -0.7f,-0.7f, 0.0f,   0f,1f,1f,
     0.7f, 0.7f, 0.4f,   0f,1f,1f,
     0.7f,-0.7f, 0.4f,   0f,1f,1f,
    -0.7f, 0.7f, 0.4f,   0f,1f,1f,
    -0.7f,-0.7f, 0.4f,   0f,1f,1f,

    // -------- Block 6 --------
     0.7f, 0.7f, 0.4f,   1f,0f,1f,
     0.7f,-0.7f, 0.4f,   1f,0f,1f,
    -0.7f, 0.7f, 0.4f,   1f,0f,1f,
    -0.7f,-0.7f, 0.4f,   1f,0f,1f,
     0.7f, 0.7f, 0.8f,   1f,0f,1f,
     0.7f,-0.7f, 0.8f,   1f,0f,1f,
    -0.7f, 0.7f, 0.8f,   1f,0f,1f,
    -0.7f,-0.7f, 0.8f,   1f,0f,1f,

    // -------- Block 7 --------
     0.8f, 0.1f, 0.3f,   1f,1f,1f,
     0.8f,-0.1f, 0.3f,   1f,1f,1f,
     0.7f, 0.1f, 0.3f,   1f,1f,1f,
     0.7f,-0.1f, 0.3f,   1f,1f,1f,
     0.8f, 0.1f, 0.4f,   1f,1f,1f,
     0.8f,-0.1f, 0.4f,   1f,1f,1f,
     0.7f, 0.1f, 0.4f,   1f,1f,1f,
     0.7f,-0.1f, 0.4f,   1f,1f,1f,

    // -------- Block 8 --------
    -0.8f, 0.05f,0.7f,   1f,0.5f,0f,
    -0.8f,-0.05f,0.7f,   1f,0.5f,0f,
    -0.7f, 0.05f,0.7f,   1f,0.5f,0f,
    -0.7f,-0.05f,0.7f,   1f,0.5f,0f,
    -0.8f, 0.05f,0.9f,   1f,0.5f,0f,
    -0.8f,-0.05f,0.9f,   1f,0.5f,0f,
    -0.7f, 0.05f,0.9f,   1f,0.5f,0f,
    -0.7f,-0.05f,0.9f,   1f,0.5f,0f,

    // -------- Block 9 --------
     0.5f,-0.3f,0.8f,   0f,0.5f,1f,
     0.5f,-0.4f,0.8f,   0f,0.5f,1f,
     0.4f,-0.3f,0.8f,   0f,0.5f,1f,
     0.4f,-0.4f,0.8f,   0f,0.5f,1f,
     0.5f,-0.3f,0.9f,   0f,0.5f,1f,
     0.5f,-0.4f,0.9f,   0f,0.5f,1f,
     0.4f,-0.3f,0.9f,   0f,0.5f,1f,
     0.4f,-0.4f,0.9f,   0f,0.5f,1f,

    // -------- Block 10 --------
     0.5f, 0.3f,0.8f,   0.5f,1f,0.5f,
     0.5f, 0.4f,0.8f,   0.5f,1f,0.5f,
     0.4f, 0.3f,0.8f,   0.5f,1f,0.5f,
     0.4f, 0.4f,0.8f,   0.5f,1f,0.5f,
     0.5f, 0.3f,0.9f,   0.5f,1f,0.5f,
     0.5f, 0.4f,0.9f,   0.5f,1f,0.5f,
     0.4f, 0.3f,0.9f,   0.5f,1f,0.5f,
     0.4f, 0.4f,0.9f,   0.5f,1f,0.5f,
};

    // Indices: 11 blocks × 36 = 396
    private readonly uint[] model_indices =
    {
    // Block 1
    0,1,2, 2,3,1, 4,5,6, 6,7,5, 0,1,5, 5,4,0, 2,3,7, 7,6,2, 1,3,7, 7,5,1, 0,2,6, 6,4,0,

    // Block 2
    8,9,10, 10,11,9, 12,13,14, 14,15,13, 8,9,13, 13,12,8, 10,11,15, 15,14,10, 9,11,15, 15,13,9, 8,10,14, 14,12,8,

    // Block 3
    16,17,18, 18,19,17, 20,21,22, 22,23,21, 16,17,21, 21,20,16, 18,19,23, 23,22,18, 17,19,23, 23,21,17, 16,18,22, 22,20,16,

    // Block 4
    24,25,26, 26,27,25, 28,29,30, 30,31,29, 24,25,29, 29,28,24, 26,27,31, 31,30,26, 25,27,31, 31,29,25, 24,26,30, 30,28,24,

    // Block 5
    32,33,34, 34,35,33, 36,37,38, 38,39,37, 32,33,37, 37,36,32, 34,35,39, 39,38,34, 33,35,39, 39,37,33, 32,34,38, 38,36,32,

    // Block 6
    40,41,42, 42,43,41, 44,45,46, 46,47,45, 40,41,45, 45,44,40, 42,43,47, 47,46,42, 41,43,47, 47,45,41, 40,42,46, 46,44,40,

    // Block 7
    48,49,50, 50,51,49, 52,53,54, 54,55,53, 48,49,53, 53,52,48, 50,51,55, 55,54,50, 49,51,55, 55,53,49, 48,50,54, 54,52,48,

    // Block 8
    56,57,58, 58,59,57, 60,61,62, 62,63,61, 56,57,61, 61,60,56, 58,59,63, 63,62,58, 57,59,63, 63,61,57, 56,58,62, 62,60,56,

    // Block 9
    64,65,66, 66,67,65, 68,69,70, 70,71,69, 64,65,69, 69,68,64, 66,67,71, 71,70,66, 65,67,71, 71,69,65, 64,66,70, 70,68,64,

    // Block 10
    72,73,74, 74,75,73, 76,77,78, 78,79,77, 72,73,77, 77,76,72, 74,75,79, 79,78,74, 73,75,79, 79,77,73, 72,74,78, 78,76,72,
    };


    private int _vao, _vbo, _ebo, _shader;
    private float _angle;

    private Vector3 _cubePosition = Vector3.Zero; 
    private float _speed = 2.0f;                  

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
        GL.BufferData(BufferTarget.ArrayBuffer, model_vertices.Length * sizeof(float), model_vertices, BufferUsageHint.StaticDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, model_indices.Length * sizeof(uint), model_indices, BufferUsageHint.StaticDraw);

        //_vbo = GL.GenBuffer();
        //GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        //GL.BufferData(BufferTarget.ArrayBuffer, prism_vertices.Length * sizeof(float), prism_vertices, BufferUsageHint.StaticDraw);

        //_ebo = GL.GenBuffer();
        //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        //GL.BufferData(BufferTarget.ElementArrayBuffer, prism_indices.Length * sizeof(uint), prism_indices, BufferUsageHint.StaticDraw);

        //_vbo = GL.GenBuffer();
        //GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        //GL.BufferData(BufferTarget.ArrayBuffer, pyramid_vertices.Length * sizeof(float), pyramid_vertices, BufferUsageHint.StaticDraw);

        //_ebo = GL.GenBuffer();
        //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        //GL.BufferData(BufferTarget.ElementArrayBuffer, pyramid_indices.Length * sizeof(uint), pyramid_indices, BufferUsageHint.StaticDraw);

        // Vertex attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
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

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        var input = KeyboardState;
        float delta = (float)e.Time;

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            _cubePosition.Z -= _speed * delta;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            _cubePosition.Z += _speed * delta;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            _cubePosition.X -= _speed * delta;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            _cubePosition.X += _speed * delta;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(_shader);

        //Rotation + Translation
        _angle += (float)e.Time;
        var rotation = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angle / 2f);
        var translation = Matrix4.CreateTranslation(_cubePosition);
        var model = rotation * translation;

        var view = Matrix4.LookAt(new Vector3(2, 2, 2), Vector3.Zero, Vector3.UnitY);
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), Size.X / (float)Size.Y, 0.1f, 100f);

        GL.UniformMatrix4(GL.GetUniformLocation(_shader, "model"), false, ref model);
        GL.UniformMatrix4(GL.GetUniformLocation(_shader, "view"), false, ref view);
        GL.UniformMatrix4(GL.GetUniformLocation(_shader, "projection"), false, ref projection);

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, model_indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();


        // This here is for making it just stand
        //base.OnRenderFrame(e);

        //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        //GL.UseProgram(_shader);

        //// Just translation (or Matrix4.Identity if you don’t want movement at all)
        //var model = Matrix4.CreateTranslation(_cubePosition);

        //var view = Matrix4.LookAt(new Vector3(2, 2, 2), Vector3.Zero, Vector3.UnitY);
        //var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), Size.X / (float)Size.Y, 0.1f, 100f);

        //GL.UniformMatrix4(GL.GetUniformLocation(_shader, "model"), false, ref model);
        //GL.UniformMatrix4(GL.GetUniformLocation(_shader, "view"), false, ref view);
        //GL.UniformMatrix4(GL.GetUniformLocation(_shader, "projection"), false, ref projection);

        //GL.BindVertexArray(_vao);
        //GL.DrawElements(PrimitiveType.Triangles, model_indices.Length, DrawElementsType.UnsignedInt, 0);

        //SwapBuffers();
    }
}
