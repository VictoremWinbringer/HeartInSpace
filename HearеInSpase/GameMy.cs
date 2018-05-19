using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MonoGame3DKezumieParticles
{
    public class GameMy : Game
    {
        #region Поля       
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect1;
        Particle[] particles;
        VertexPositionNormalTexture[] vertex;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        SpriteFont font;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        private short[] indices;
        Texture2D texture;
        Vector2 Size;
        bool isMoving;
        Matrix rotashion = Matrix.Identity;
        Matrix translathion = Matrix.Identity;
        MouseState mouse;
        MouseState lastMouseState;
        float cameraDistance;
        Object o = new Object();
        string s = "";
        double t = 0;
        double f = 0;
        int k = 0;
        int i = 0;
        #endregion

        public GameMy()
        {
            particles = new Particle[30000];
            Size = new Vector2(1f, 1f);
            cameraDistance = 400;
            indices = new short[particles.Length * 6];
            vertex = new VertexPositionNormalTexture[particles.Length * 4];
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            //Убераем ограничения на частоту вызова метода Update и Draw  
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
            Window.Title = "Kezumie";
            IsMouseVisible = true;
            isMoving = true;
            //Создаем матрицы вида, проекции и камеры.
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, cameraDistance), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight, 0.1f, 700);
        }

        #region Инициализация и загрузка начальных данных

        protected override void Initialize()
        {
            //Создаем буффер индексов и вершин
            graphics.GraphicsDevice.Flush();
            vertexBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), vertex.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            //Создаем вершины для наших частиц.
            CreateVertex();
            //Переносим данные в буффер для видеокарты.
            indexBuffer.SetData(indices);
            vertexBuffer.SetData(vertex);
            //Вызываем иниталайз для базового класса и всех компоненетов, если они у нас есть.
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            effect1 = Content.Load<Effect>("sd");
            font = Content.Load<SpriteFont>("font");
            using (FileStream fs = new FileStream("Content/smoke5.png", System.IO.FileMode.Open))
                texture = Texture2D.FromStream(graphics.GraphicsDevice, fs);
            effect1.Parameters["Projection"].SetValue(projectionMatrix);
            effect1.Parameters["Texture"].SetValue(texture);
            effect1.Parameters["Size"].SetValue(Size);
        }

        protected override void UnloadContent()
        {
            texture.Dispose();
        }

        #endregion

        #region Обновление данных и отображение их на экран

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            double time = gameTime.ElapsedGameTime.TotalMilliseconds;
            //Цикл для заполнения данным массива вершин
            CircleMove();
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) { Ju(); Li(); Ya(); Heart(); isMoving = true; }
            if (isMoving) isMoving = Move(gameTime);
            CameraMove(gameTime);
            base.Update(gameTime);
        }

        private void CircleMove()
        {
            ++i;
            ++k;
            if (i > 360) i = 0;
            if (k > 180) k = 0;
            for (int j = 135000; j < 135500; j++)
            {
                Random rnd = new Random(i);
                //Вычисляем позицию частицы в трехмерном пространстве
                double R = 200;
                float sin = k;
                float cos = i;
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));
                //Меняем конечную позицию частицы.
                particles[j].Position = new Vector3(x, y, z);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            ++f;
            t += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (f > 100) { s = "Fps: " + Convert.ToInt32((f / t) * 1000); f = 0; t = 0; }
            vertexBuffer.SetData(vertex);
            graphics.GraphicsDevice.Clear(Color.Black);
            effect1.Parameters["View"].SetValue(viewMatrix);
            //В Аддитив режими смешиваються только цвета, прозрачность остаетсья той же
            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            //Сообщаем видеокарте чтобы она не рисовала одну из плоскостей треугольника.
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //Линейное сжатие текстуры - она будет сжиматься под соотношение сторо нашего квадрата
            // graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //Устанавливаем чтение глубины, без этого больше 2 объектов один за другим не будет видно (остальных закроют передние) ! Обязательно.
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            //Устанавливаем для видеокарты буффер вершин и индексы для него.             
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;
            //Включаем наш шейдер
            effect1.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.Length, 0, indices.Length / 3);
            DrawString();
            base.Draw(gameTime);
        }

        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Метод для рисования буквы "Ю".
        /// </summary>
        private void Ju()
        { //0 - 50 000
            Lane l;
            l.arStart = 0;
            l.arEnd = 12000;
            l.start = new Vector2(-80, -25);
            l.middl = new Vector2(-65, -15);
            l.end = new Vector2(-50, 25);
            LaneBezier(l);
            l.arStart = 12000;
            l.arEnd = 36000;
            l.start = new Vector2(-50, -25);
            l.middl = new Vector2(-50, 15);
            l.end = new Vector2(0, 25);
            LaneBezier(l);
            l.arStart = 36000;
            l.arEnd = 48000;
            l.start = new Vector2(-50, -25);
            l.middl = new Vector2(30, -5);
            l.end = new Vector2(0, 25);
            LaneBezier(l);
            l.arStart = 48000;
            l.arEnd = 50000;
            l.start = new Vector2(-70, -5);
            l.middl = new Vector2(-50, -10);
            l.end = new Vector2(-30, 10);
            LaneBezier(l);
        }
        /// <summary>
        /// Метод для рисования буквы "л".
        /// </summary>
        void Li()
        {
            //50 00 75 000
            Lane l;
            l.arStart = 50000;
            l.arEnd = 65000;
            l.start = new Vector2(0, -10);
            l.middl = new Vector2(15, -30);
            l.end = new Vector2(40, 15);
            LaneBezier(l);
            l.arStart = 65000;
            l.arEnd = 75000;
            l.start = new Vector2(40, 15);
            l.middl = new Vector2(40, -30);
            l.end = new Vector2(50, -10);
            LaneBezier(l);
        }
        /// <summary>
        /// Метод для рисования буквы "я".
        /// </summary>
        void Ya()
        {
            // 75 000 99999
            Lane l;
            l.arStart = 75000;
            l.arEnd = 80000;
            l.start = new Vector2(50, -10);
            l.middl = new Vector2(65, -30);
            l.end = new Vector2(80, 0);
            LaneBezier(l);
            l.arStart = 80000;
            l.arEnd = 85000;
            l.start = new Vector2(80, 0);
            l.middl = new Vector2(70, 20);
            l.end = new Vector2(60, 0);
            LaneBezier(l);
            l.arStart = 85000;
            l.arEnd = 90000;
            l.start = new Vector2(60, 0);
            l.middl = new Vector2(75, -20);
            l.end = new Vector2(85, 0);
            LaneBezier(l);
            l.arStart = 90000;
            l.arEnd = 95000;
            l.start = new Vector2(80, 0);
            l.middl = new Vector2(70, -30);
            l.end = new Vector2(90, -15);
            LaneBezier(l);
        }
        /// <summary>
        /// Метод для рисования сердца
        /// </summary>
        void Heart()
        {
            Lane l;
            l.arStart = 95000;
            l.arEnd = 115000;
            l.start = new Vector2(0, 60);
            l.middl = new Vector2(280, 180);
            l.end = new Vector2(0, -150);
            LaneBezier(l);
            l.arStart = 115000;
            l.arEnd = 135000;
            l.start = new Vector2(0, 60);
            l.middl = new Vector2(-280, 180);
            l.end = new Vector2(0, -150);
            LaneBezier(l);
        }
        /// <summary>
        /// Метод для создания вершин наших частиц.
        /// </summary>
        private void CreateVertex()
        {
            //Цикл для заполнения данным массива вершин
            for (int i = 0; i < particles.Length; i++)
            {
                Random rnd = new Random(i);
                //Вычисляем позицию частицы в трехмерном пространстве
                double R = rnd.NextDouble() * 500;
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));
                //Создаем частицу с начальными данными
                particles[i] = new Particle(2, new Vector3(x, y, z));
                //Переносим данные о точках частицы в массив вершин.
                vertex[i * 4] = particles[i].Vertex[0];
                vertex[i * 4 + 1] = particles[i].Vertex[1];
                vertex[i * 4 + 2] = particles[i].Vertex[2];
                vertex[i * 4 + 3] = particles[i].Vertex[3];
                //Создаем массив индексов для вершин.
                indices[i * 6] =(short) (0 + i * 4);
                indices[i * 6 + 1] =(short) (1 + i * 4);
                indices[i * 6 + 2] =(short) (2 + i * 4);
                indices[i * 6 + 3] =(short) (0 + i * 4);
                indices[i * 6 + 4] =(short) (2 + i * 4);
                indices[i * 6 + 5] = (short)(3 + i * 4);
            }
        }
        /// <summary>
        /// Метод для нахождения координаты на кривой Безье
        /// </summary>
        /// <param name="P">Начальная точка из которой идет линия</param>
        /// <param name="P1">Серединная точка которая оттягивает линию в свою строну</param>
        /// <param name="P2">Конечная точка на которой заканчиваеться линия</param>
        /// <param name="t">Доля от общей длины линии на которой находиться наша точка. От 0 до 1 включительно</param>
        /// <returns></returns>
        double BezierMy(double P, double P1, double P2, double t)
        {
            if (t < 0 || t > 1) throw new ArgumentOutOfRangeException("t должен лежать в диапазоне от 0 до 1 включительно");
            double t0 = 1 - t;
            return Math.Pow(t0, 2) * P + 2 * t0 * t * P1 + Math.Pow(t, 2) * P2;
        }
        /// <summary>
        /// Рисуют линию безье из частиц
        /// </summary>
        /// <param name="lane">Структура содержщая данны о линии которую надо нарисовать.</param>
        void LaneBezier(Lane lane)
        {
            double step = 1d / (lane.arEnd - lane.arStart + 1);
            double t = 0;

            for (int i = lane.arStart; i < lane.arEnd + 1; i++)
            {
                Random rnd = new Random(i);
                float x = (float)BezierMy(lane.start.X, lane.middl.X, lane.end.X, t);
                float y = (float)BezierMy(lane.start.Y, lane.middl.Y, lane.end.Y, t);
                float z = 0;
                x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 5 + x;
                z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 20;
                y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 5 + y;
                particles[i].EndPosition = new Vector3(x, y, z);
                particles[i].isMoving = true;
                t += step;
            }
        }
        /// <summary>
        /// Рисует текст на мониторе.
        /// </summary>
        private void DrawString()
        {
            //Пишем на экране текст.
            spriteBatch.Begin();
            spriteBatch.DrawString(font,
                "By Victorem. " + s +
                Environment.NewLine +
                "Use arrows on keyboard and mouse wheel to move camera" +
                Environment.NewLine +
                "Press space to start animation",
                new Vector2(5, 5), Color.Red);
            spriteBatch.End();
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }
        /// <summary>
        /// Метод для передвижения времени.
        /// </summary>
        /// <param name="gameTime">Количество времени необходимое для расчета скорости передвежения</param>
        private void CameraMove(GameTime gameTime)
        {
            //Скорость поворота расчитываемая в зависимости от времени.
            float r = (float)(0.1 * gameTime.ElapsedGameTime.TotalMilliseconds);
            //Скорость поворота в радианах.
            float pi = MathHelper.ToRadians(r);
            mouse = Mouse.GetState();
            //Врашаем камеру вокруг нулевых кордианат
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotashion *= Matrix.CreateRotationY(-1 * pi);
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotashion *= Matrix.CreateRotationY(pi);
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) rotashion *= Matrix.CreateRotationX(-1 * pi);
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) rotashion *= Matrix.CreateRotationX(pi);
            if (Keyboard.GetState().IsKeyDown(Keys.A)) rotashion *= Matrix.CreateRotationZ(-1 * pi);
            if (Keyboard.GetState().IsKeyDown(Keys.D)) rotashion *= Matrix.CreateRotationZ(pi);
            //Изменяем дистанцию камеры колесиком мыши
            if (mouse.ScrollWheelValue < lastMouseState.ScrollWheelValue) cameraDistance -= 4 * r;
            if (mouse.ScrollWheelValue > lastMouseState.ScrollWheelValue) cameraDistance += 4 * r;
            //Изменяем дистанцию камеры клавиатурой
            if (Keyboard.GetState().IsKeyDown(Keys.S)) cameraDistance += r;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) cameraDistance -= r;
            //Проверяем не вышла ли камера за дозволенную дистанцию
            if (cameraDistance < 1) cameraDistance = 1;
            if (cameraDistance > 500) cameraDistance = 500;
            //Установливаем новое значение для камеры
            viewMatrix = rotashion * Matrix.CreateLookAt(new Vector3(0, 0, cameraDistance), Vector3.Zero, Vector3.Up);
            //Сохраняем текушее состояние мыши
            lastMouseState = mouse;
        }
        /// <summary>
        /// Асинхронный метод для двежения частиц.
        /// </summary>
        /// <param name="gameTime">Количество времени необходимое для расчета скорости передвежения</param>
        /// <returns></returns>
        Task MoveAsync(GameTime gameTime)
        {
            return Task.Factory.StartNew(() =>
              {
                  double time = gameTime.ElapsedGameTime.TotalMilliseconds;
                  for (int i = 0; i < particles.Length; i++)
                  {
                      lock (o)
                      {
                          if (particles[i].isMoving)
                          {
                              particles[i].Move(time);
                              vertex[i * 4] = particles[i].Vertex[0];
                              vertex[i * 4 + 1] = particles[i].Vertex[1];
                              vertex[i * 4 + 2] = particles[i].Vertex[2];
                              vertex[i * 4 + 3] = particles[i].Vertex[3];
                          }
                      }
                  }
              });
        }
        /// <summary>
        /// Метод для передвижения частицы.
        /// </summary>
        /// <param name="gameTime">Количество времени необходимое для расчета скорости передвежения</param>
        /// <returns>Есть ли еще частицы которые не завершили свое движение</returns>
        bool Move(GameTime gameTime)
        {
            int j = 0;
            double time = gameTime.ElapsedGameTime.TotalMilliseconds;
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].isMoving)
                {
                    ++j;
                    particles[i].Move(time);
                    vertex[i * 4] = particles[i].Vertex[0];
                    vertex[i * 4 + 1] = particles[i].Vertex[1];
                    vertex[i * 4 + 2] = particles[i].Vertex[2];
                    vertex[i * 4 + 3] = particles[i].Vertex[3];
                }
            }
            if (j > 50) return true;
            return false;

        }
        /// <summary>
        /// Меняет позицию частицы чиклически по поверхносте сферы.
        /// </summary>

        #endregion
    }
    /// <summary>
    /// Структура хранящая данные для рисования кривой линии.
    /// </summary>
    struct Lane
    {
        /// <summary>
        /// Позиция начальной точки кривой (x,y)
        /// </summary>
        public Vector2 start;
        /// <summary>
        /// Позиция вершины кривой.
        /// </summary>
        public Vector2 end;
        /// <summary>
        /// Позиция конечной вершины кривой.
        /// </summary>
        public Vector2 middl;
        /// <summary>
        /// Индекс первой частицы в массиве частиц с которой начнеться рисование линии.
        /// </summary>
        public int arStart;
        /// <summary>
        /// Индекс последней частицы в массиве мачастиц.
        /// </summary>
        public int arEnd;
    }
}
