using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics3D
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitialSetup();
            timer = new Timer();
            alpha = 0;

            FlatShading.CheckedChanged += Shading_CheckedChanged;
            GouroudShading.CheckedChanged += Shading_CheckedChanged;
            PhongShading.CheckedChanged += Shading_CheckedChanged;

            ColorIa.Click += Color_Click;
            ColorIp.Click += Color_Click;

            RenderButton.Click += RenderButton_Click;

            RotateCube.CheckedChanged += RotateCube_CheckedChanged;
            MobileObject.CheckedChanged += MobileObject_CheckedChanged;
        }

        private void MobileObject_CheckedChanged(object sender, EventArgs e)
        {
            if (!timer.Enabled && MobileObject.Checked)
            {
                timer = new Timer();
                timer.Interval = 100;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            else if(RotateCube.Checked)
            {
                return;
            }
            else
            {
                timer.Stop();
            }
        }

        private void RotateCube_CheckedChanged(object sender, EventArgs e)
        {
            if (!timer.Enabled && RotateCube.Checked)
            {
                timer = new Timer();
                timer.Interval = 100;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            else if(MobileObject.Checked)
            {
                return;
            }
            else
            {
                timer.Stop();
            }
        }

        private Matrix4x4 TranslateMatrix(float x, float y, float z)
        {
            return new Matrix4x4(
                1, 0, 0, x, 
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1
                );
        }

        private Matrix4x4 RotateZMatrix(double angle)
        {
            var cos = (float)Math.Cos(DegreeToRadian(angle));
            var sin = (float)Math.Sin(DegreeToRadian(angle));
            return new Matrix4x4(
                cos, -sin, 0, 0,
                sin, cos,  0, 0,
                0,   0,    1, 0,
                0,   0,    0, 1
                );
        }

        private void TransformModelMatrix(Matrix4x4 m)
        {
            model = Matrix4x4.Multiply(m, model);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(RotateCube.Checked) alpha = (alpha + 1) % 360;
            if(MobileObject.Checked) movingObject.Move();
            RenderScene();
        }

        private void RenderButton_Click(object sender, EventArgs e)
        {
            RenderScene();
        }

        private void Color_Click(object sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = false;
            MyDialog.ShowHelp = true;

            PictureBox box = (PictureBox)sender;
            MyDialog.Color = box.BackColor;

            if (MyDialog.ShowDialog() == DialogResult.OK)
                box.BackColor = MyDialog.Color;
        }

        private void Shading_CheckedChanged(object sender, EventArgs e)
        {
            RenderScene();
        }

        public void InitialSetup()
        {
            cameraX.Value = 0M;
            cameraY.Value = 0M;
            cameraZ.Value = 6M;

            cameraTarget = new Vector4(0, 0, 0, 1);

            targetX.Value = 0M;
            targetY.Value = 0M;
            targetZ.Value = 0M;

            sphereDivX.Value = 20;
            sphereDivY.Value = 20;
            sphereRadius.Value = 1;

            ColorIa.BackColor = Color.DarkBlue;
            ColorIp.BackColor = Color.Cyan;

            ParamKa.Value = 0.4M;

            ParamKs.Value = 0.25M;
            ParamKd.Value = 0.75M;
            ParamN.Value = 20M;

            lightX.Value = 10M;
            lightY.Value = 0M;
            lightZ.Value = 10M;

            sphereCenter = new Vector4(-1, 1, 0, 1);
            cubeStart = new Vector4(1, -1, 0, 1);

            movingObject = new MovingObject(-1, -1, 0);           
        }

        Vector4 sphereCenter;
        Vector4 cubeStart;

        Bitmap pictureBitmap;

        Matrix4x4 model;
        Matrix4x4 view;
        Matrix4x4 proj;
        Matrix4x4 modelInverted;

        IShading shading;
        PhongLight phongLight;

        Vector3 cameraPosition;
        Vector4 cameraTarget;
        MovingObject movingObject;

        double alpha;
        Timer timer;

        private double[][] zBuffer;

        private void RenderScene()
        {
            SetCamera();
            PreparePhongLight();
            PrepareShading();
            PrepareMatrixes();
            PrepareBuffer();
            DrawSolids();
        }
        private void SetCamera()
        {
            if(MoveCamera.Checked)
            {
                cameraPosition = new Vector3(movingObject.position.X, movingObject.position.Y, movingObject.position.Z);
            }
            else
            {
                cameraPosition = new Vector3((float)cameraX.Value, (float)cameraY.Value, (float)cameraZ.Value);
            }
        }
        private void PrepareShading()
        {
            if (phongLight == null) PreparePhongLight();
            if (this.PhongShading.Checked)
            {
                shading = new PhongShading(phongLight);
            }
            else if(this.GouroudShading.Checked)
            {
                shading = new GouraudShading(phongLight);
            }
            else
            {
                shading = new FlatShading(phongLight);
            }
        }
        private void PreparePhongLight()
        {
            double ka = (double)ParamKa.Value;
            double ks = (double)ParamKs.Value;
            double kd = (double)ParamKd.Value;
            int shiny = (int)ParamN.Value;
            Color Ia = ColorIa.BackColor;
            Vector4 lightPos = new Vector4((float)lightX.Value, (float)lightY.Value, (float)lightZ.Value, 1);

            Color Ip = ColorIp.BackColor;
            var lights = new List<(Vector4 pos, Color Ip)> { (lightPos, Ip), (movingObject.position, Color.White) };
            Vector3 observator = new Vector3((float)cameraX.Value, (float)cameraY.Value, (float)cameraZ.Value);
            phongLight = new PhongLight(ka,ks, kd, shiny, Ia, lights, observator);

        }
        private void PrepareMatrixes()
        {
            CreateModel();
            CreateModelInverted();
            CreateView();
            CreateProjection();
        }
        private void PrepareBuffer()
        {
            zBuffer = new double[pictureBox.Width][];
            for (int i = 0; i < pictureBox.Width; ++i)
            {
                zBuffer[i] = new double[pictureBox.Height];
                for (int j = 0; j < pictureBox.Height; ++j)
                {
                    zBuffer[i][j] = double.PositiveInfinity;
                }
            }
        }

        private void CreateModel()
        {
            model = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
                );
        }

        private void CreateModelInverted()
        {
            modelInverted = new Matrix4x4();
            var m = Matrix4x4.Transpose(model);
            Matrix4x4.Invert(m, out modelInverted);
        }

        private double DegreeToRadian(double angle)
        {
            return (Math.PI * angle / 180.0);
        }
        private void CreateView()
        {
            float camX = (float)cameraX.Value;
            float camY = (float)cameraY.Value;
            float camZ = (float)cameraZ.Value;

            float tarX = 0f;
            float tarY = 0f;
            float tarZ = 0f;

            if (FollowCamera.Checked)
            {
                tarX = movingObject.position.X;
                tarY = movingObject.position.Y;
                tarZ = movingObject.position.Z;
            }
            else
            {
                tarX = (float)targetX.Value;
                tarY = (float)targetY.Value;
                tarZ = (float)targetZ.Value;
            }
          
            Vector3 target = new Vector3(tarX, tarY, tarZ);
            Vector3 Uword = new Vector3(0, 1, 0);

            Vector3 forward = Vector3.Normalize(cameraPosition - target);
            Vector3 right = Vector3.Normalize(Vector3.Cross(Uword, forward));
            
            Vector3 up = Vector3.Cross(forward, right);

            Matrix4x4 m1 = new Matrix4x4(
                right.X, right.Y, right.Z, 0,
                up.X, up.Y, up.Z, 0,
                forward.X, forward.Y, forward.Z, 0, 
                0, 0, 0, 1
                );

            Matrix4x4 m2 = new Matrix4x4(
                1, 0, 0, -cameraPosition.X, 
                0, 1, 0, -cameraPosition.Y, 
                0, 0, 1, -cameraPosition.Z, 
                0, 0, 0, 1
                );

            view = m1 * m2;
        }
        private void CreateProjection()
        {
            float n = 0.1f;
            float f = 100f;
            float fov = (float)(Math.PI / 4);
            float a = (float)pictureBox.Width / (float)pictureBox.Height;
            float e = (float)(1.0 / Math.Tan(fov / 2.0));         

            proj = new Matrix4x4();
            proj.M11 = e;
            proj.M22 = e / a;
            proj.M33 = -(f + n) / (f - n);
            proj.M34 = -(2 * f * n) / (f - n);
            proj.M43 = -1f;         
        }

        private void CreateModelForCube()
        {
            float tx = cubeStart.X;
            float ty = cubeStart.Y;
            float tz = cubeStart.Z;

            CreateModel();
            if (alpha != 0)
            {
                TransformModelMatrix(RotateZMatrix(alpha));
            }
            TransformModelMatrix(TranslateMatrix(tx, ty, tz));

            CreateModelInverted();
        }

        private void CreateModelForCuboid()
        {
            CreateModel();

            float s = 0.25f;

            var scale = new Matrix4x4(
                s, 0, 0, 0,
                0, s, 0, 0,
                0, 0, s, 0,
                0, 0, 0, 1
                );

            TransformModelMatrix(scale);

            var tx = movingObject.position.X - s/2.0f;
            var ty = movingObject.position.Y;
            var tz = movingObject.position.Z - s/2.0f;

            var t = new Matrix4x4(
                1, 0, 0, tx,
                0, 1, 0, ty,
                0, 0, 1, tz,
                0, 0, 0, 1
                );

            TransformModelMatrix(t);
      
            CreateModelInverted();
        }

        private void CreateModelForSphere()
        {
            float tx = sphereCenter.X;
            float ty = sphereCenter.Y;
            float tz = sphereCenter.Z;

            CreateModel();
            TransformModelMatrix(TranslateMatrix(tx, ty, tz));

            CreateModelInverted();
        }

        public void DrawSolids()
        {
            pictureBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics g = Graphics.FromImage(pictureBitmap);
            Pen pen = new Pen(Brushes.Black);


            // Sphere
            int divX = (int)sphereDivX.Value;
            int divY = (int)sphereDivY.Value;
            double radius = (double)sphereRadius.Value;
            Sphere sphere = new Sphere(radius, divX, divY);

            CreateModelForSphere();

            for (int i = 0; i < sphere.vertices.Count; ++i)
            {
                TransformVertex(sphere.vertices[i]);
            }

            sphere.GenerateTriangles();

            foreach (var triangle in sphere.triangles)
            {
                ProcessTriangle(triangle, g, pen);
            }

            // Cube
            CreateModelForCube();

            Cube cube = new Cube();

            for (int i = 0; i < cube.triangles.Count; ++i)
            {
                ProcessTriangle(cube.triangles[i], g, pen);
            }

            // Cuboid
            if (!MoveCamera.Checked)
            {
                CreateModelForCuboid();

                Cuboid cuboid = new Cuboid(2);

                for (int i = 0; i < cuboid.triangles.Count; ++i)
                {
                    ProcessTriangle(cuboid.triangles[i], g, pen);
                }
            }

            pictureBox.BackgroundImage = pictureBitmap;
            g.Dispose();
        }

        private void ProcessTriangle(Triangle triangle, Graphics g, Pen pen)
        {
            TransformVertex(triangle.vertexOne);
            TransformVertex(triangle.vertexTwo);
            TransformVertex(triangle.vertexThree);

            List<MyVertex> vertices = new List<MyVertex>() {
                    triangle.vertexOne,
                    triangle.vertexTwo,
                    triangle.vertexThree
                };

            if (BackFaceCulling(triangle)) return;

            if (OnlyCurves.Checked)
            {
                g.DrawLine(pen, vertices[0].transformedCoordinates.X, vertices[0].transformedCoordinates.Y, vertices[1].transformedCoordinates.X, vertices[1].transformedCoordinates.Y);
                g.DrawLine(pen, vertices[1].transformedCoordinates.X, vertices[1].transformedCoordinates.Y, vertices[2].transformedCoordinates.X, vertices[2].transformedCoordinates.Y);
                g.DrawLine(pen, vertices[2].transformedCoordinates.X, vertices[2].transformedCoordinates.Y, vertices[0].transformedCoordinates.X, vertices[0].transformedCoordinates.Y);
            }
            else
            {
                shading.StartNewTriangle(triangle);
                FillPolygon(vertices);
            }
        }

        private Vector4 VectorByMatrixMultiply(Vector4 v, Matrix4x4 m)
        {
            var x = m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W;
            var y = m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W;
            var z = m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W;
            var w = m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W;

            return new Vector4(x, y, z, w);
        }

        private void TransformVertex(MyVertex vertex)
        {
            vertex.transformedCoordinates = TransformVector(vertex.oryginalCoordinates);
            vertex.worldCoordinates = VectorByMatrixMultiply(vertex.oryginalCoordinates, model);            
            var n = VectorByMatrixMultiply(new Vector4(vertex.normal.X, vertex.normal.Y, vertex.normal.Z, 0), modelInverted);
            var normal = Vector4.Normalize(n);
            vertex.normal = new Vector3(normal.X, normal.Y, normal.Z);

        }
        public Vector4 TransformVector(Vector4 vector)
        {
            Vector4 transformed = new Vector4(vector.X, vector.Y, vector.Z, vector.W);

            transformed = VectorByMatrixMultiply(transformed, model);
            transformed = VectorByMatrixMultiply(transformed, view);
            transformed = VectorByMatrixMultiply(transformed, proj);

            transformed.X /= transformed.W;
            transformed.Y /= transformed.W;
            transformed.Z /= transformed.W;
            transformed.W = 1;

            int H = pictureBox.Height;
            int W = pictureBox.Width;

            transformed.X = (transformed.X + 1) * W / 2.0f;
            transformed.Y = (transformed.Y + 1) * H / 2.0f;

            return transformed;
        } 

        private void FillPolygon(List<MyVertex> vertices)
        {
            List<Point> points = new List<Point>();
            foreach(var v in vertices)
            {
                points.Add(new Point((int)v.transformedCoordinates.X, (int)v.transformedCoordinates.Y));
            }

            List<(int index, int y)> ind = new List<(int index, int y)>();
            for (int i = 0; i < 3; ++i)
            {
                ind.Add((i, points[i].Y));
            }
            ind.Sort((a, b) => { return a.y.CompareTo(b.y); });

            int yMin = ind.First().y;
            int yMax = ind.Last().y;

            List<Edge> EAT = new List<Edge>();

            for (int y = yMin; y <= yMax; ++y)
            {
                for (int k = 0; k < 3; ++k)
                {
                    if (ind[k].y == y - 1)
                    {
                        int prev = k - 1 < 0 ? 2 : k - 1;
                        int next = k + 1 > 2 ? 0 : k + 1;

                        if (ind[prev].y > ind[k].y) EAT.Add(new Edge(points[ind[k].index], points[ind[prev].index]));
                        if (ind[prev].y < ind[k].y) EAT.RemoveAll((ed) => { return ed.SameEdge(points[ind[k].index], points[ind[prev].index]); });

                        if (ind[next].y > ind[k].y) EAT.Add(new Edge(points[ind[k].index], points[ind[next].index]));
                        if (ind[next].y < ind[k].y) EAT.RemoveAll((ed) => { return ed.SameEdge(points[ind[k].index], points[ind[next].index]); });
                    }
                }
                EAT.Sort();
                FillPixels(EAT, y, (vertices[0], vertices[1], vertices[2]));
                foreach (var e in EAT)
                {
                    e.updateX();
                }
            }
        }
        private void FillPixels(List<Edge> EAT, int y, (MyVertex a, MyVertex b, MyVertex c) vertices)
        {
            for (int i = 0; i < EAT.Count - 1; i += 2)
            {
                for (int x = (int)EAT[i].currX; x <= (int)EAT[i + 1].currX; ++x)
                {
                    var bary = CalculateBarycentric(x, y, (vertices.a.transformedCoordinates, vertices.b.transformedCoordinates, vertices.c.transformedCoordinates));
                    if (!InterpolateZBuffor(x, y, vertices, bary)) continue;
                    if (x >= 0 && x < pictureBox.Width && y >= 0 && y < pictureBox.Height)
                    {
                        Color finalColor = shading.GetShadingColor(bary);
                        pictureBitmap.SetPixel(x, y, finalColor);
                    }
                }
            }
        }

        private bool InterpolateZBuffor(int x, int y, (MyVertex a, MyVertex b, MyVertex c) vertices, (double alfa, double beta, double gamma) bary)
        {
            if (x < 0 || x >= pictureBox.Width || y < 0 || y >= pictureBox.Height) return false;
            var z = bary.alfa * vertices.a.transformedCoordinates.Z + bary.beta * vertices.b.transformedCoordinates.Z + bary.gamma * vertices.c.transformedCoordinates.Z;
            if (zBuffer[x][y] > z)
            {
                zBuffer[x][y] = z;
                return true;
            }
            return false;
        }
        private (double alfa, double beta, double gamma) CalculateBarycentric(int x, int y, (Vector4 a, Vector4 b, Vector4 c) vertices)
        {
            var p1 = vertices.a;
            var p2 = vertices.b;
            var p3 = vertices.c;

            double det = (p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y);

            double alfa = (p2.Y - p3.Y) * (x - p3.X) + (p3.X - p2.X) * (y - p3.Y);
            alfa /= det;

            double beta = (p3.Y - p1.Y) * (x - p3.X) + (p1.X - p3.X) * (y - p3.Y);
            beta /= det;

            double gamma = 1 - alfa - beta;

            return (alfa, beta, gamma);
        }

        private bool BackFaceCulling(Triangle triangle)
        {
            var N = triangle.GetNormalToTraingle();
            var P = new Vector3(triangle.vertexOne.worldCoordinates.X, triangle.vertexOne.worldCoordinates.Y, triangle.vertexOne.worldCoordinates.Z);
            var V = Vector3.Normalize(P - cameraPosition);
            var dot = Vector3.Dot(V, N);
            if (dot >= 0) return true;
            else return false;
        }

    }
}
