using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * It's the survival of the biggest!
 * Propel your chips across a frictionless table top to avoid getting eaten by bigger foes.
 * Aim for smaller oil droplets for an easy size boost.
 * Tip: merging your chips will give you a sizeable advantage.
 **/
class Player
{

    class Vector
    {
        public Vector(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public double x
        {
            get;
            set;
        }

        public double y
        {
            get;
            set;
        }

        public double length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public Vector normalize()
        {
            double len = length();
            return new Vector(x / len, y / len);
        }

        public Vector add(Vector v)
        {
            return new Vector(v.x + x, v.y + y);
        }

        public Vector sub(Vector v)
        {
            return new Vector(x - v.x, y - v.y);
        }

        public Vector mul(double d)
        {
            return new Vector(x * d, y * d);
        }

        public double scalar(Vector v)
        {
            return v.x * x + v.y * y;
        }

        public double cross(Vector v)
        {
            return x * v.y - y * v.x;
        }

        public Vector flip(Vector v)
        {
            Vector uV = v.normalize();
            Vector a = uV.mul(this.scalar(uV));
            Vector b = this.sub(a);

            return a.sub(b);
        }

        public Vector vectorProjection(Vector v)
        {
            Vector uV = v.normalize();
            return uV.mul(this.scalar(uV));
        }

        public Vector vectorRejection(Vector v)
        {
            return sub(vectorProjection(v));
        }

        public override string ToString()
        {
            return "{" + x + ", " + y + "}";
        }

        public int angleTo(Vector v)
        {
            return (int)(180 / Math.PI * Math.Acos(scalar(v) / length() / v.length()));
        }

    }

    class Entity
    {
        public Vector position
        {
            get;
            private set;
        }

        public Vector speed
        {
            get;
            private set;
        }

        public int id
        {
            get;
            private set;
        }

        public int player
        {
            get;
            private set;
        }

        public double radius
        {
            get;
            private set;
        }

        public bool alive
        {
            get;
            set;
        }

        public Entity(int id, int player)
        {
            this.id = id;
            this.player = player;
        }

        public void update(double radius, double x, double y, double vx, double vy)
        {
            this.radius = radius;
            this.position = new Vector(x, y);
            this.speed = new Vector(vx, vy);
            alive = true;
        }

        public bool isOil()
        {
            return player == -1;
        }

        public int timeUntilCollision(Entity e)
        {
            double collisionDistance = e.radius + radius;
            for (int i = 0; i < 100; i++)
            {
                Vector v = position.add(speed.mul(i)).sub(e.position.add(e.speed).mul(i));
                if (v.length() < collisionDistance)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    static void Main(string[] args)
    {
        int playerId = int.Parse(Console.ReadLine()); // your id (0 to 4)
        List<Entity> ents = new List<Entity>();

        // game loop
        while (true)
        {
            int playerChipCount = int.Parse(Console.ReadLine()); // The number of chips under your control
            int entityCount = int.Parse(Console.ReadLine()); // The total number of entities on the table, including your chips

            foreach (Entity ent in ents)
            {
                ent.alive = false;
            }
            for (int i = 0; i < entityCount; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int id = int.Parse(inputs[0]); // Unique identifier for this entity
                int player = int.Parse(inputs[1]); // The owner of this entity (-1 for neutral droplets)
                double radius = double.Parse(inputs[2]); // the radius of this entity
                double x = double.Parse(inputs[3]); // the X coordinate (0 to 799)
                double y = double.Parse(inputs[4]); // the Y coordinate (0 to 514)
                double vx = double.Parse(inputs[5]); // the speed of this entity along the X axis
                double vy = double.Parse(inputs[6]); // the speed of this entity along the Y axis

                Entity ent = null;
                if ((ent = ents.Find(e => e.id == id)) == null)
                {
                    ent = new Entity(id, player);
                    ents.Add(ent);
                    Console.Error.WriteLine("Spawned " + id + " " + player);
                }

                ent.update(radius, x, y, vx, vy);
            }

            //Remove the dead
            ents.RemoveAll(e => !e.alive);

            foreach (Entity ent in ents.FindAll(e => e.player == playerId))
            {
                //Find closest oil
                double closestOil = 10000;
                Entity oil = null;
                foreach (Entity entity in ents.FindAll(e => e.isOil()))
                {
                    double distance = ent.position.sub(entity.position).length();
                    Console.Error.WriteLine(distance);
                    if (distance < closestOil)
                    {
                        oil = entity;
                        closestOil = distance;
                    }

                }

                if (oil != null)
                {
                    Vector v = ent.position.sub(ent.position.sub(oil.position).normalize());
                    Console.Error.WriteLine(ent.position);
                    Console.Error.WriteLine(oil.position);
                    Console.Error.WriteLine(ent.position.sub(oil.position).normalize());
                    Console.Error.WriteLine(v);
                    Console.WriteLine(v.x + " " + v.y + " " + (oil.isOil() ? "oil " : "chip ") + oil.id);
                }
                else
                {
                    Console.WriteLine("WAIT");
                }
            }
        }
    }
}