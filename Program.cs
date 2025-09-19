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
        private int _celShader;
        private float[] _modelVertices;
        private uint[] _modelIndices;

        private Vector3 _modelTranslation = Vector3.Zero;
        private float _panSpeed = 1.5f;
        private float _modelScale = 1.0f;

        public CubeWindow(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }
        private Vector2 _lastMousePos;
        private bool _dragging = false;

        private float _yaw = -45f;   // left-right
        private float _pitch = 30f;  // up-down

        Vector3[][] cubes = {
            new Vector3[] { // Cube 0
                new Vector3(-0.5f,-0.2f,-0.5f), new Vector3(-0.5f,-0.2f,-0.3f),
                new Vector3(-0.3f,-0.2f,-0.5f), new Vector3(-0.3f,-0.2f,-0.3f),
                new Vector3(-0.5f, 0.0f,-0.5f), new Vector3(-0.5f, 0.0f,-0.3f),
                new Vector3(-0.3f, 0.0f,-0.5f), new Vector3(-0.3f, 0.0f,-0.3f)
            },
            new Vector3[] { // Cube 1
                new Vector3(-0.5f,-0.2f, 0.5f), new Vector3(-0.5f,-0.2f, 0.3f),
                new Vector3(-0.3f,-0.2f, 0.5f), new Vector3(-0.3f,-0.2f, 0.3f),
                new Vector3(-0.5f, 0.0f, 0.5f), new Vector3(-0.5f, 0.0f, 0.3f),
                new Vector3(-0.3f, 0.0f, 0.5f), new Vector3(-0.3f, 0.0f, 0.3f)
            },
            new Vector3[] { // Cube 2
                new Vector3(0.6f,-0.2f,-0.5f), new Vector3(0.6f,-0.2f,-0.3f),
                new Vector3(0.4f,-0.2f,-0.5f), new Vector3(0.4f,-0.2f,-0.3f),
                new Vector3(0.6f, 0.0f,-0.5f), new Vector3(0.6f, 0.0f,-0.3f),
                new Vector3(0.4f, 0.0f,-0.5f), new Vector3(0.4f, 0.0f,-0.3f)
            },
            new Vector3[] { // Cube 3
                new Vector3(0.6f,-0.2f, 0.5f), new Vector3(0.6f,-0.2f, 0.3f),
                new Vector3(0.4f,-0.2f, 0.5f), new Vector3(0.4f,-0.2f, 0.3f),
                new Vector3(0.6f, 0.0f, 0.5f), new Vector3(0.6f, 0.0f, 0.3f),
                new Vector3(0.4f, 0.0f, 0.5f), new Vector3(0.4f, 0.0f, 0.3f)
            },
            new Vector3[] { // Cube 4
                new Vector3(0.7f, 0.0f, 0.7f), new Vector3(0.7f, 0.0f,-0.7f),
                new Vector3(-0.7f,0.0f, 0.7f), new Vector3(-0.7f,0.0f,-0.7f),
                new Vector3(0.7f, 0.4f, 0.7f), new Vector3(0.7f, 0.4f,-0.7f),
                new Vector3(-0.7f,0.4f, 0.7f), new Vector3(-0.7f,0.4f,-0.7f)
            },
            new Vector3[] { // Cube 5
                new Vector3(0.7f, 0.4f, 0.7f), new Vector3(0.7f, 0.4f,-0.7f),
                new Vector3(-0.7f,0.4f, 0.7f), new Vector3(-0.7f,0.4f,-0.7f),
                new Vector3(0.7f, 0.8f, 0.7f), new Vector3(0.7f, 0.8f,-0.7f),
                new Vector3(-0.7f,0.8f, 0.7f), new Vector3(-0.7f,0.8f,-0.7f)
            },
            new Vector3[] { // Cube 6
                new Vector3(0.8f, 0.3f, 0.1f), new Vector3(0.8f, 0.3f,-0.1f),
                new Vector3(0.7f, 0.3f, 0.1f), new Vector3(0.7f, 0.3f,-0.1f),
                new Vector3(0.8f, 0.4f, 0.1f), new Vector3(0.8f, 0.4f,-0.1f),
                new Vector3(0.7f, 0.4f, 0.1f), new Vector3(0.7f, 0.4f,-0.1f)
            },
            new Vector3[] { // Cube 7
                new Vector3(-0.8f,0.7f,0.05f), new Vector3(-0.8f,0.7f,-0.05f),
                new Vector3(-0.7f,0.7f,0.05f), new Vector3(-0.7f,0.7f,-0.05f),
                new Vector3(-0.8f,0.9f,0.05f), new Vector3(-0.8f,0.9f,-0.05f),
                new Vector3(-0.7f,0.9f,0.05f), new Vector3(-0.7f,0.9f,-0.05f)
            },
            new Vector3[] { // Cube 8
                new Vector3(0.5f,0.8f,-0.3f), new Vector3(0.5f,0.8f,-0.4f),
                new Vector3(0.4f,0.8f,-0.3f), new Vector3(0.4f,0.8f,-0.4f),
                new Vector3(0.5f,0.9f,-0.3f), new Vector3(0.5f,0.9f,-0.4f),
                new Vector3(0.4f,0.9f,-0.3f), new Vector3(0.4f,0.9f,-0.4f)
            },
            new Vector3[] { // Cube 9
                new Vector3(0.5f,0.8f,0.3f), new Vector3(0.5f,0.8f,0.4f),
                new Vector3(0.4f,0.8f,0.3f), new Vector3(0.4f,0.8f,0.4f),
                new Vector3(0.5f,0.9f,0.3f), new Vector3(0.5f,0.9f,0.4f),
                new Vector3(0.4f,0.9f,0.3f), new Vector3(0.4f,0.9f,0.4f)
            }
        };

        private void BuildVoxelModel()
        {
            var verts = new List<float>();
            var inds = new List<uint>();
            uint indexOffset = 0;

            // One color per cube (keep the same length as cubes)
            Vector3[] cubeColors =
            {
        new Vector3(0.9f, 0.9f, 0.9f),   // Cube 0
        new Vector3(0.9f, 0.9f, 0.9f),   // Cube 1
        new Vector3(0.9f, 0.9f, 0.9f),   // Cube 2
        new Vector3(0.9f, 0.9f, 0.9f),   // Cube 3
        new Vector3(0.9f, 0.9f, 0.9f),   // Cube 4
        new Vector3(0.42f,0.48f,0.55f),  // Cube 5
        new Vector3(0.9f, 0.9f, 0.9f),   // Cube 6
        new Vector3(0.42f,0.48f,0.55f),  // Cube 7
        new Vector3(0.42f,0.48f,0.55f),  // Cube 8
        new Vector3(0.42f,0.48f,0.55f)   // Cube 9
    };

            // face layout using canonical corner indices (see comment below)
            int[][] faceIndices = {
        new[]{0,2,3,1}, // back  (z = minZ)
        new[]{4,5,7,6}, // front (z = maxZ)
        new[]{0,1,5,4}, // left  (x = minX)
        new[]{2,6,7,3}, // right (x = maxX)
        new[]{0,4,6,2}, // bottom(y = minY)
        new[]{1,3,7,5}  // top   (y = maxY)
    };

            Vector3[] faceNormals = {
        new Vector3(0,0,-1),
        new Vector3(0,0, 1),
        new Vector3(-1,0,0),
        new Vector3( 1,0,0),
        new Vector3(0,-1,0),
        new Vector3(0, 1,0)
    };

            // For each cube, build canonical corners then emit 4 verts per face (24 verts per cube)
            for (int ci = 0; ci < cubes.Length; ci++)
            {
                var inputCorners = cubes[ci];

                // compute axis-aligned bounds from the provided 8 points
                float minX = float.MaxValue, maxX = float.MinValue;
                float minY = float.MaxValue, maxY = float.MinValue;
                float minZ = float.MaxValue, maxZ = float.MinValue;
                foreach (var p in inputCorners)
                {
                    if (p.X < minX) minX = p.X; if (p.X > maxX) maxX = p.X;
                    if (p.Y < minY) minY = p.Y; if (p.Y > maxY) maxY = p.Y;
                    if (p.Z < minZ) minZ = p.Z; if (p.Z > maxZ) maxZ = p.Z;
                }

                // canonical cube corner ordering (8 corners)
                // 0: (minX, minY, minZ)
                // 1: (minX, maxY, minZ)
                // 2: (maxX, minY, minZ)
                // 3: (maxX, maxY, minZ)
                // 4: (minX, minY, maxZ)
                // 5: (minX, maxY, maxZ)
                // 6: (maxX, minY, maxZ)
                // 7: (maxX, maxY, maxZ)
                Vector3[] corners = new Vector3[8];
                corners[0] = new Vector3(minX, minY, minZ);
                corners[1] = new Vector3(minX, maxY, minZ);
                corners[2] = new Vector3(maxX, minY, minZ);
                corners[3] = new Vector3(maxX, maxY, minZ);
                corners[4] = new Vector3(minX, minY, maxZ);
                corners[5] = new Vector3(minX, maxY, maxZ);
                corners[6] = new Vector3(maxX, minY, maxZ);
                corners[7] = new Vector3(maxX, maxY, maxZ);

                // color for this cube
                Vector3 color = cubeColors[Math.Min(ci, cubeColors.Length - 1)];

                // Emit 4 vertices per face (position, color, normal), then two triangles
                for (int f = 0; f < faceIndices.Length; f++)
                {
                    int[] fi = faceIndices[f];
                    Vector3 normal = faceNormals[f];

                    // add the face's 4 vertices (duplicate positions so faces are independent)
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 p = corners[fi[k]];
                        verts.Add(p.X); verts.Add(p.Y); verts.Add(p.Z);          // pos
                        verts.Add(color.X); verts.Add(color.Y); verts.Add(color.Z); // color
                        verts.Add(normal.X); verts.Add(normal.Y); verts.Add(normal.Z); // normal
                    }

                    // triangles (0,1,2) and (0,2,3)
                    inds.Add(indexOffset + 0);
                    inds.Add(indexOffset + 1);
                    inds.Add(indexOffset + 2);

                    inds.Add(indexOffset + 0);
                    inds.Add(indexOffset + 2);
                    inds.Add(indexOffset + 3);

                    indexOffset += 4; // 4 verts were added
                }
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
                    float ambientStrength = 0.7;
                    vec3 ambient = ambientStrength * lightColor;

                    // Diffuse with soft toon shading
                    float diff = max(dot(norm, lightDir), 0.0);
                    float levels = 3.0;
                    float diffStep = floor(diff * levels) / levels;
                    float softness = 0.5; // blend toon and smooth
                    float diffMixed = mix(diffStep, diff, softness);
                    vec3 diffuse = diffMixed * lightColor;

                    // Specular with smoothstep (softer edge)
                    vec3 viewDir = normalize(viewPos - vFragPos);
                    vec3 reflectDir = reflect(-lightDir, norm);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
                    float specStep = smoothstep(0.2, 0.8, spec);
                    vec3 specular = specStep * lightColor * 0.5;

                    // Combine
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

            GL.Enable(EnableCap.DepthTest);
        }



        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Set dark blue background
            GL.ClearColor(0.0f, 0.0f, 0.2f, 1.0f); // RGB = dark blue, Alpha = 1
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // === Common matrices ===
            var model =
            Matrix4.CreateScale(_modelScale) *
            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_yaw)) *
            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_pitch)) *
            Matrix4.CreateTranslation(_modelTranslation);


            var camPos = new Vector3(0, 2, 5); // fixed camera
            var view = Matrix4.LookAt(camPos, Vector3.Zero, Vector3.UnitY);

            var proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f),
                Size.X / (float)Size.Y,
                0.1f,
                100f
            );

            //=== Cel Shader Pass ===
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
            if (input.IsKeyDown(Keys.Up)) _modelScale += 0.0001f;
            if (input.IsKeyDown(Keys.Down))_modelScale = Math.Max(0.0001f, _modelScale - 0.0001f);
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



        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButton.Left)
            {
                _dragging = false;
            }
        }

    }
}
