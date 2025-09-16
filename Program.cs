using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace VoxelQuadsViewer
{
    class Program
    {
        static void Main()
        {
            var settings = new NativeWindowSettings()
            {
                Size = new Vector2i(1000, 700),
                Title = "Voxel Model"
            };

            using var win = new CubeWindow(GameWindowSettings.Default, settings);
            win.Run();
        }
    }

    class CubeWindow : GameWindow
    {
        private int _vao, _vbo, _ebo;
        private int _celShader, _outlineShader;
        private float[] _modelVertices;
        private uint[] _modelIndices;

        private Vector3 _modelTranslation = Vector3.Zero;
        private float _panSpeed = 1.5f;

        public CubeWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }
        private Vector2 _lastMousePos;
        private bool _dragging = false;
        private bool _firstMove = true;

        private float _yaw = -45f;   // left-right
        private float _pitch = 30f;  // up-down
        private float _distance = 5f; // zoom

        Vector3[][] cubes = {
            new Vector3[] { // Cube 0
                new Vector3(-0.5f,-0.5f,-0.2f), new Vector3(-0.5f,-0.3f,-0.2f),
                new Vector3(-0.3f,-0.5f,-0.2f), new Vector3(-0.3f,-0.3f,-0.2f),
                new Vector3(-0.5f,-0.5f, 0.0f), new Vector3(-0.5f,-0.3f, 0.0f),
                new Vector3(-0.3f,-0.5f, 0.0f), new Vector3(-0.3f,-0.3f, 0.0f)
            },
            new Vector3[] { // Cube 1
                new Vector3(-0.5f, 0.5f,-0.2f), new Vector3(-0.5f, 0.3f,-0.2f),
                new Vector3(-0.3f, 0.5f,-0.2f), new Vector3(-0.3f, 0.3f,-0.2f),
                new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(-0.5f, 0.3f, 0.0f),
                new Vector3(-0.3f, 0.5f, 0.0f), new Vector3(-0.3f, 0.3f, 0.0f)
            },
            new Vector3[] { // Cube 2
                new Vector3(0.6f,-0.5f,-0.2f), new Vector3(0.6f,-0.3f,-0.2f),
                new Vector3(0.4f,-0.5f,-0.2f), new Vector3(0.4f,-0.3f,-0.2f),
                new Vector3(0.6f,-0.5f, 0.0f), new Vector3(0.6f,-0.3f, 0.0f),
                new Vector3(0.4f,-0.5f, 0.0f), new Vector3(0.4f,-0.3f, 0.0f)
            },
            new Vector3[] { // Cube 3
                new Vector3(0.6f, 0.5f,-0.2f), new Vector3(0.6f, 0.3f,-0.2f),
                new Vector3(0.4f, 0.5f,-0.2f), new Vector3(0.4f, 0.3f,-0.2f),
                new Vector3(0.6f, 0.5f, 0.0f), new Vector3(0.6f, 0.3f, 0.0f),
                new Vector3(0.4f, 0.5f, 0.0f), new Vector3(0.4f, 0.3f, 0.0f)
            },
            new Vector3[] { // Cube 4
                new Vector3(0.7f, 0.7f, 0.0f), new Vector3(0.7f,-0.7f, 0.0f),
                new Vector3(-0.7f,0.7f, 0.0f), new Vector3(-0.7f,-0.7f, 0.0f),
                new Vector3(0.7f, 0.7f, 0.4f), new Vector3(0.7f,-0.7f, 0.4f),
                new Vector3(-0.7f,0.7f, 0.4f), new Vector3(-0.7f,-0.7f, 0.4f)
            },
            new Vector3[] { // Cube 5
                new Vector3(0.7f, 0.7f, 0.4f), new Vector3(0.7f,-0.7f, 0.4f),
                new Vector3(-0.7f,0.7f, 0.4f), new Vector3(-0.7f,-0.7f, 0.4f),
                new Vector3(0.7f, 0.7f, 0.8f), new Vector3(0.7f,-0.7f, 0.8f),
                new Vector3(-0.7f,0.7f, 0.8f), new Vector3(-0.7f,-0.7f, 0.8f)
            },
            new Vector3[] { // Cube 6
                new Vector3(0.8f, 0.1f, 0.3f), new Vector3(0.8f,-0.1f, 0.3f),
                new Vector3(0.7f, 0.1f, 0.3f), new Vector3(0.7f,-0.1f, 0.3f),
                new Vector3(0.8f, 0.1f, 0.4f), new Vector3(0.8f,-0.1f, 0.4f),
                new Vector3(0.7f, 0.1f, 0.4f), new Vector3(0.7f,-0.1f, 0.4f)
            },
            new Vector3[] { // Cube 7
                new Vector3(-0.8f,0.05f,0.7f), new Vector3(-0.8f,-0.05f,0.7f),
                new Vector3(-0.7f,0.05f,0.7f), new Vector3(-0.7f,-0.05f,0.7f),
                new Vector3(-0.8f,0.05f,0.9f), new Vector3(-0.8f,-0.05f,0.9f),
                new Vector3(-0.7f,0.05f,0.9f), new Vector3(-0.7f,-0.05f,0.9f)
            },
            new Vector3[] { // Cube 8
                new Vector3(0.5f,-0.3f,0.8f), new Vector3(0.5f,-0.4f,0.8f),
                new Vector3(0.4f,-0.3f,0.8f), new Vector3(0.4f,-0.4f,0.8f),
                new Vector3(0.5f,-0.3f,0.9f), new Vector3(0.5f,-0.4f,0.9f),
                new Vector3(0.4f,-0.3f,0.9f), new Vector3(0.4f,-0.4f,0.9f)
            },
            new Vector3[] { // Cube 9
                new Vector3(0.5f, 0.3f,0.8f), new Vector3(0.5f, 0.4f,0.8f),
                new Vector3(0.4f, 0.3f,0.8f), new Vector3(0.4f, 0.4f,0.8f),
                new Vector3(0.5f, 0.3f,0.9f), new Vector3(0.5f, 0.4f,0.9f),
                new Vector3(0.4f, 0.3f,0.9f), new Vector3(0.4f, 0.4f,0.9f)
            }
        };

        private void BuildVoxelModel()
        {
            var verts = new List<float>();
            var inds = new List<uint>();
            uint offset = 0;

            Vector3[] cubeColors =
            {
        new Vector3(0.9f, 0.9f, 0.9f),   // Rear Right Nub
        new Vector3(0.9f, 0.9f, 0.9f),   // Rear Left Nub
        new Vector3(0.9f, 0.9f, 0.9f),   // Front Right Nub
        new Vector3(0.9f, 0.9f, 0.9f),   // Front Left Nub
        new Vector3(0.9f, 0.9f, 0.9f),   // Bottom Layer
        new Vector3(0.42f, 0.48f, 0.55f),// Top Layer
        new Vector3(0.9f, 0.9f, 0.9f),   // Snout
        new Vector3(0.42f, 0.48f, 0.55f),// Tail
        new Vector3(0.42f, 0.48f, 0.55f),// Left Ear
        new Vector3(0.42f, 0.48f, 0.55f) // Right Ear
    };

            // Cube faces: 4 vertex indices per face + normal
            (int[], Vector3)[] faces = {
        (new[]{0,1,3,2}, new Vector3(0,0,-1)), // back
        (new[]{4,5,7,6}, new Vector3(0,0,1)),  // front
        (new[]{0,2,6,4}, new Vector3(0,-1,0)), // bottom
        (new[]{1,3,7,5}, new Vector3(0,1,0)),  // top
        (new[]{0,4,5,1}, new Vector3(-1,0,0)), // left
        (new[]{2,3,7,6}, new Vector3(1,0,0)),  // right
    };

            for (int ci = 0; ci < cubes.Length; ci++)
            {
                var cubeVerts = cubes[ci];
                Vector3 color = cubeColors[Math.Min(ci, cubeColors.Length - 1)];

                // Add all 8 cube vertices only once
                foreach (var v in cubeVerts)
                {
                    verts.Add(v.X); verts.Add(v.Y); verts.Add(v.Z);
                    verts.Add(color.X); verts.Add(color.Y); verts.Add(color.Z);
                    verts.Add(0f); verts.Add(0f); verts.Add(0f); // placeholder normal
                }

                // Add indices per face and assign normals per vertex
                foreach (var (faceIdx, normal) in faces)
                {
                    // Assign normal to each vertex in the face
                    foreach (int fi in faceIdx)
                    {
                        int vertStart = (int)(offset + fi) * 9;
                        verts[vertStart + 6] = normal.X;
                        verts[vertStart + 7] = normal.Y;
                        verts[vertStart + 8] = normal.Z;
                    }

                    // Triangulate face
                    inds.Add(offset + (uint)faceIdx[0]);
                    inds.Add(offset + (uint)faceIdx[1]);
                    inds.Add(offset + (uint)faceIdx[2]);

                    inds.Add(offset + (uint)faceIdx[0]);
                    inds.Add(offset + (uint)faceIdx[2]);
                    inds.Add(offset + (uint)faceIdx[3]);
                }

                offset += (uint)cubeVerts.Length; // 8 vertices per cube
            }

            _modelVertices = verts.ToArray();
            _modelIndices = inds.ToArray();
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }



        protected override void OnLoad()
        {
            base.OnLoad();

            BuildVoxelModel();

            // create and upload buffers
            _vao = GL.GenVertexArray(); GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _modelVertices.Length * sizeof(float), _modelVertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _modelIndices.Length * sizeof(uint), _modelIndices, BufferUsageHint.StaticDraw);

            // Each vertex = 9 floats (3 pos + 3 col + 3 normal)
            int stride = 9 * sizeof(float);

            // position attribute (layout=0 in vertex shader)
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);

            // color attribute (layout=1)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));

            // normal attribute (layout=2)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));


            // === Cel Shader ===
            string celVS = @"
                #version 330 core
                layout(location=0) in vec3 aPos;
                layout(location=1) in vec3 aColor;
                layout(location=2) in vec3 aNormal;

                out vec3 vColor;
                out vec3 vNormal;
                out vec3 vFragPos;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                void main()
                {
                    vColor = aColor;
                    vNormal = mat3(transpose(inverse(model))) * aNormal;
                    vFragPos = vec3(model * vec4(aPos, 1.0));
                    gl_Position = projection * view * vec4(vFragPos, 1.0);
                }";

            string celFS = @"
                #version 330 core
                in vec3 vColor;
                in vec3 vNormal;
                in vec3 vFragPos;
                out vec4 FragColor;

                uniform vec3 lightPos;
                uniform vec3 viewPos;
                uniform vec3 lightColor;

                void main()
                {
                    vec3 norm = normalize(vNormal);
                    vec3 lightDir = normalize(lightPos - vFragPos);

                    // Ambient
                    float ambientStrength = 0.6;
                    vec3 ambient = ambientStrength * lightColor;

                    // Diffuse (quantized)
                    float diff = max(dot(norm, lightDir), 0.0);
                    float levels = 3.0;
                    float diffStep = floor(diff * levels) / levels;
                    vec3 diffuse = diffStep * lightColor + 0.3 * lightColor;


                    // Specular (hard)
                    vec3 viewDir = normalize(viewPos - vFragPos);
                    vec3 reflectDir = reflect(-lightDir, norm);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
                    float specStep = step(0.5, spec);
                    vec3 specular = specStep * lightColor * 0.5;

                    vec3 result = (ambient + diffuse + specular) * vColor;
                    FragColor = vec4(result, 1.0);
                }";

            _celShader = GL.CreateProgram();
            int celVSId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(celVSId, celVS);
            GL.CompileShader(celVSId);
            int celFSId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(celFSId, celFS);
            GL.CompileShader(celFSId);
            GL.AttachShader(_celShader, celVSId);
            GL.AttachShader(_celShader, celFSId);
            GL.LinkProgram(_celShader);
            GL.DeleteShader(celVSId);
            GL.DeleteShader(celFSId);

            // === Outline Shader ===
            string outlineVS = @"
                #version 330 core
                layout(location=0) in vec3 aPos;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                void main()
                {
                    gl_Position = projection * view * model * vec4(aPos, 1.0);
                }";

            string outlineFS = @"
                #version 330 core
                out vec4 FragColor;
                uniform vec4 outlineColor;
                void main()
                {
                    FragColor = outlineColor;
                }";

            _outlineShader = GL.CreateProgram();
            int oVS = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(oVS, outlineVS);
            GL.CompileShader(oVS);
            int oFS = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(oFS, outlineFS);
            GL.CompileShader(oFS);
            GL.AttachShader(_outlineShader, oVS);
            GL.AttachShader(_outlineShader, oFS);
            GL.LinkProgram(_outlineShader);
            GL.DeleteShader(oVS);
            GL.DeleteShader(oFS);

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var input = KeyboardState;
            float dt = (float)e.Time;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W)) _modelTranslation.Z -= _panSpeed * dt;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S)) _modelTranslation.Z += _panSpeed * dt;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A)) _modelTranslation.X -= _panSpeed * dt;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D)) _modelTranslation.X += _panSpeed * dt;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q)) _modelTranslation.Y -= _panSpeed * dt;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E)) _modelTranslation.Y += _panSpeed * dt;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Set dark blue background
            GL.ClearColor(0.0f, 0.0f, 0.2f, 1.0f); // RGB = dark blue, Alpha = 1
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // === Common matrices ===
            var model = Matrix4.CreateTranslation(_modelTranslation);
            var outlineModel = Matrix4.CreateScale(1.05f) * model;

            // Camera orbit math
            float yawRad = MathHelper.DegreesToRadians(_yaw);
            float pitchRad = MathHelper.DegreesToRadians(_pitch);

            Vector3 camPos = new Vector3(
                _distance * MathF.Cos(pitchRad) * MathF.Cos(yawRad),
                _distance * MathF.Sin(pitchRad),
                _distance * MathF.Cos(pitchRad) * MathF.Sin(yawRad)
            );

            var view = Matrix4.LookAt(camPos, Vector3.Zero, Vector3.UnitY);
            var proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f),
                Size.X / (float)Size.Y,
                0.1f,
                100f
            );

            // === Outline Pass ===
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.UseProgram(_outlineShader);
            GL.UniformMatrix4(GL.GetUniformLocation(_outlineShader, "model"), false, ref outlineModel);
            GL.UniformMatrix4(GL.GetUniformLocation(_outlineShader, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(_outlineShader, "projection"), false, ref proj);
            GL.Uniform4(GL.GetUniformLocation(_outlineShader, "outlineColor"), 0f, 0f, 0f, 1f);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _modelIndices.Length, DrawElementsType.UnsignedInt, 0);

            // === Cel Shader Pass ===
            GL.CullFace(CullFaceMode.Back);

            GL.UseProgram(_celShader);
            GL.UniformMatrix4(GL.GetUniformLocation(_celShader, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(_celShader, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(_celShader, "projection"), false, ref proj);

            // Light + camera
            GL.Uniform3(GL.GetUniformLocation(_celShader, "lightPos"), new Vector3(2f, 2f, 2f));
            GL.Uniform3(GL.GetUniformLocation(_celShader, "lightColor"), new Vector3(1f, 1f, 1f));
            GL.Uniform3(GL.GetUniformLocation(_celShader, "viewPos"), camPos);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _modelIndices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }



        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragging)
            {
                var delta = new Vector2(e.X, e.Y) - _lastMousePos;
                _lastMousePos = new Vector2(e.X, e.Y);

                // Adjust sensitivity
                _yaw += delta.X * 0.3f;
                _pitch -= delta.Y * 0.3f;

                // Clamp pitch to avoid flipping upside down
                _pitch = Math.Clamp(_pitch, -89f, 89f);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                _dragging = true;
                // Record current mouse position
                var state = MouseState; // Get current state from window
                _lastMousePos = new Vector2(state.X, state.Y);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButton.Left)
            {
                _dragging = false;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _distance -= e.OffsetY * 0.25f;
            _distance = Math.Clamp(_distance, 1f, 10f);
        }

    }
}
