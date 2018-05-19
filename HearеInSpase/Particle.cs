using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame3DKezumieParticles
{/// <summary>
/// Объектное представление одной частицы.
/// </summary>
    class Particle
    {
        #region Поля        
        float Speed;
        Vector3 Velocity;
        public Vector3 Position;
        public bool isMoving { get; set; }
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public VertexPositionNormalTexture[] Vertex;
        #endregion
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="speed">Делитель ускорения нашей частицы. Чем больше тем дольше будет длиться анимация</param>
        /// <param name="position">Начальная координата нашей частицы</param>
        public Particle(float speed, Vector3 position)
        {
            Vertex = new VertexPositionNormalTexture[4];
            Speed = speed * 1000;
            StartPosition = position;
            EndPosition = position;
            Init();
        }
        /// <summary>
        /// Заполнение стартовых данных для частицы.
        /// </summary>
        public void Init()
        {
            Position = StartPosition;
            isMoving = true;
            Vertex = new VertexPositionNormalTexture[4];
            float X = StartPosition.X;
            float Y = StartPosition.Y;
            float Z = StartPosition.Z;
            ///Задаем позицию частицы, Вектор номали необходимый для того чтобы частица всега смотрела на камеру и координаты текстуры.
            Vertex[0] = new VertexPositionNormalTexture(new Vector3(X, Y, Z), new Vector3(-1, 1, 1), new Vector2(0, 0));
            Vertex[1] = new VertexPositionNormalTexture(new Vector3(X, Y, Z), new Vector3(1, 1, 1), new Vector2(1, 0));
            Vertex[2] = new VertexPositionNormalTexture(new Vector3(X, Y, Z), new Vector3(1, -1, 1), new Vector2(1, 1));
            Vertex[3] = new VertexPositionNormalTexture(new Vector3(X, Y, Z), new Vector3(-1, -1, 1), new Vector2(0, 1));
        }
        /// <summary>
        /// Передвижение частицы.
        /// </summary>
        /// <param name="time">Время, в милисекундах, необходимое для рассчета скорости передвижения</param>
        public void Move(double time)
        {
            if (this.Position.Equals(this.EndPosition))
            {
                isMoving = false;
                return;
            }
            //Вычисляем ускорение частицы
            double mult = time;
            Velocity = new Vector3((EndPosition.X - Position.X) / Speed,
           (EndPosition.Y - Position.Y) / Speed,
           (EndPosition.Z - Position.Z) / Speed);
            //Изменяем позицию частицы в соответствии с ускорением.
            Position.X += (float)((Velocity.X * mult));
            Position.Y += (float)((Velocity.Y * mult));
            Position.Z += (float)((Velocity.Z * mult));
            //Если частица приблизилась на минимальную дистанцию к своей точке остановки то переносим ее туда.
            if (MathHelper.Distance(EndPosition.X, Position.X) < 0.01) Position.X = EndPosition.X;
            if (MathHelper.Distance(EndPosition.Y, Position.Y) < 0.01) Position.Y = EndPosition.Y;
            if (MathHelper.Distance(EndPosition.Z, Position.Z) < 0.01) Position.Z = EndPosition.Z;
            //Переносим данные о местоположение в массив вершин нашей частицы.
            float X = Position.X;
            float Y = Position.Y;
            float Z = Position.Z;
            Vertex[0].Position = new Vector3(X, Y, Z);
            Vertex[1].Position = new Vector3(X, Y, Z);
            Vertex[2].Position = new Vector3(X, Y, Z);
            Vertex[3].Position = new Vector3(X, Y, Z);
        }
    }
}
