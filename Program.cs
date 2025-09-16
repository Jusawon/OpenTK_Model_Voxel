using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

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

        private int _vao, _vbo, _ebo, _shader;
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

        // Mouse controls

        private void BuildVoxelModel()
        {
            var verts = new List<float>();
            var inds = new List<uint>();
            uint offset = 0;

            // One color per cube (fill these in!)
            Vector3[] cubeColors =
            {
        new Vector3(0.9f, 0.9f, 0.9f), // Rear Right Nub
        new Vector3(0.9f, 0.9f, 0.9f), // Rear Left Nub
        new Vector3(0.9f, 0.9f, 0.9f), // Front Right Nub
        new Vector3(0.9f, 0.9f, 0.9f), // Front Left Nub
        new Vector3(0.9f, 0.9f, 0.9f), // Bottom Layer
        new Vector3(0.42f, 0.48f, 0.55f), // Top Layer
        new Vector3(0.9f, 0.9f, 0.9f), // Snout
        new Vector3(0.42f, 0.48f, 0.55f), // Tail
        new Vector3(0.42f, 0.48f, 0.55f), // Left Ear
        new Vector3(0.42f, 0.48f, 0.55f), // Right Ear
    };

            for (int cubeIndex = 0; cubeIndex < cubes.Length; cubeIndex++)
            {
                Vector3[] cubeVerts = cubes[cubeIndex];
                Vector3 color = cubeColors[cubeIndex];

                // Each cube has 8 verts
                foreach (var v in cubeVerts)
                {
                    verts.Add(v.X); verts.Add(v.Y); verts.Add(v.Z);
                    verts.Add(color.X); verts.Add(color.Y); verts.Add(color.Z);
                }

                // Cube faces (12 triangles, 36 indices)
                uint[] cubeIdx = {
            0,1,2, 2,1,3,   // bottom
            4,6,5, 5,6,7,   // top
            0,2,4, 4,2,6,   // front
            1,5,3, 3,5,7,   // back
            0,4,1, 1,4,5,   // left
            2,3,6, 6,3,7    // right
        };

                foreach (var idx in cubeIdx)
                    inds.Add(offset + idx);

                offset += 8;
            }

            _modelVertices = verts.ToArray();
            _modelIndices = inds.ToArray();
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

            // pos (3 floats)
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);

            // color (3 floats)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));

            // normal (3 floats)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));

            // attributes: position vec3, color vec3
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));


            // simple color shader
            string vs = @"
                #version 330 core
                layout(location=0) in vec3 aPos;
                layout(location=1) in vec3 aColor;
                layout(location=2) in vec3 aNormal;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                out vec3 vColor;
                out vec3 vNormal;
                out vec3 vFragPos;

                void main()
                {
                    gl_Position = projection * view * model * vec4(aPos, 1.0);
                    vFragPos = vec3(model * vec4(aPos, 1.0));   // world space position
                    vNormal = mat3(transpose(inverse(model))) * aNormal; // transformed normal
                    vColor = aColor;
                }";

            string fs = @"
                #version 330 core
                in vec3 vColor;
                in vec3 vNormal;
                in vec3 vFragPos;

                out vec4 FragColor;

                uniform vec3 lightPos;   // world space light position
                uniform vec3 viewPos;    // camera position
                uniform vec3 lightColor; // usually white (1,1,1)

                void main()
                {
                    // Normalize
                    vec3 norm = normalize(vNormal);
                    vec3 lightDir = normalize(lightPos - vFragPos);

                    // Ambient
                    float ambientStrength = 0.2;
                    vec3 ambient = ambientStrength * lightColor;

                    // Diffuse
                    float diff = max(dot(norm, lightDir), 0.0);
                    vec3 diffuse = diff * lightColor;

                    // Specular
                    float specularStrength = 0.5;
                    vec3 viewDir = normalize(viewPos - vFragPos);
                    vec3 reflectDir = reflect(-lightDir, norm);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                    vec3 specular = specularStrength * spec * lightColor;

                    vec3 result = (ambient + diffuse + specular) * vColor;
                    FragColor = vec4(result, 1.0);
                }";

            int vsId = GL.CreateShader(ShaderType.VertexShader); GL.ShaderSource(vsId, vs); GL.CompileShader(vsId);
            int fsId = GL.CreateShader(ShaderType.FragmentShader); GL.ShaderSource(fsId, fs); GL.CompileShader(fsId);

            _shader = GL.CreateProgram();
            GL.AttachShader(_shader, vsId); GL.AttachShader(_shader, fsId); GL.LinkProgram(_shader);
            GL.DeleteShader(vsId); GL.DeleteShader(fsId);

            GL.ClearColor(0.12f, 0.14f, 0.18f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace); // quads may be single-sided in any winding; disable culling so all faces visible
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

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_shader);

            // Model matrix (static placement of the voxel model)
            var model = Matrix4.CreateTranslation(_modelTranslation);

            // --- Camera orbiting around the model ---
            float yawRad = MathHelper.DegreesToRadians(_yaw);
            float pitchRad = MathHelper.DegreesToRadians(_pitch);

            Vector3 camPos = new Vector3(
                _distance * MathF.Cos(pitchRad) * MathF.Cos(yawRad),
                _distance * MathF.Sin(pitchRad),
                _distance * MathF.Cos(pitchRad) * MathF.Sin(yawRad)
            );

            var view = Matrix4.LookAt(camPos, Vector3.Zero, Vector3.UnitY);

            // Projection matrix
            var proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f),
                Size.X / (float)Size.Y,
                0.1f,
                100f
            );

            // Upload uniforms
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "projection"), false, ref proj);

            // Use your shader
            GL.UseProgram(_shader);

            // Camera position = camPos you already compute
            GL.Uniform3(GL.GetUniformLocation(_shader, "viewPos"), camPos);

            // Example light
            Vector3 lightPos = new Vector3(2f, 2f, 2f);
            Vector3 lightColor = new Vector3(1f, 1f, 1f);
            GL.Uniform3(GL.GetUniformLocation(_shader, "lightPos"), lightPos);
            GL.Uniform3(GL.GetUniformLocation(_shader, "lightColor"), lightColor);

            // Draw the voxel model
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
