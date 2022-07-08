using System;
using System.Collections.Generic;

namespace PlaywindTest
{
	class Program
	{
		public static void Main(string[] args)
		{
			var shapes = new List<Shape>()
			{
				new Circle(new Position(0f, 3f), 0.5f),
				new Rectangle(new Position(4f, 2f), new Size(8f, 1f)),
				new Rectangle(new Position(3f, 0f), new Size(0.2f, 3f)),
			}; //Example shapes

			var intersectValue = FindIntersections(shapes);

			foreach (var intersection in intersectValue)
			{
				Console.WriteLine($"{intersection.Key} -> ({string.Join(", ", intersection.Value)})");
			} //Formatting
		}

		public static Dictionary<int, List<int>> FindIntersections(List<Shape> shapes)
		{
			var intersectDictionary = new Dictionary<int, List<int>>();

			for (int i = 0; i < shapes.Count; i++)
			{
				for (int j = 0; j < shapes.Count; j++)
				{
					if (i == j)
					{
						continue;
					} //Skip checks on self

					var shape1 = shapes[i];
					var shape2 = shapes[j];

					if (!Intersects(shape1, shape2))
					{
						continue;
					} //No intersect

					if (!intersectDictionary.TryGetValue(shape1.id, out var list))
					{
						intersectDictionary[shape1.id] = new List<int> { shape2.id };
					} //New collection on first intersect
					else
					{
						list.Add(shape2.id);
					}
				}
			}

			return intersectDictionary;

			bool Intersects(Shape shapeA, Shape shapeB)
			{
				switch (shapeA)
				{
					case Circle a when shapeB is Circle b: return a.Intersects(b);
					case Circle a when shapeB is Rectangle b: return a.Intersects(b);
					case Rectangle a when shapeB is Circle b: return a.Intersects(b);
					case Rectangle a when shapeB is Rectangle b: return a.Intersects(b);
					default:
						throw new NotSupportedException(shapeA.GetType().ToString());
				}
			} //Should be the most efficient 
		}
	}

	public class Position
	{
		public Position(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		public float x { get; }
		public float y { get; }
	}

	public class Size
	{
		public float width { get; }
		public float height { get; }

		public Size(float width, float height)
		{
			this.width = width;
			this.height = height;
		}
	}

	public class Shape
	{
		private static int idIncrement = 1;
		public int id { get; }
		public Position position { get; }

		protected Shape(Position position)
		{
			id = idIncrement++;

			this.position = position;
		}
	} //May be inefficient 

	public class Rectangle : Shape
	{
		public Size size { get; }

		public Position[] rectCorners { get; }
		public Rectangle(Position position, Size size) : base(position)
		{
			this.size = size;

			float halfRectWidth = size.width * .5f;
			float halfRectHeight = size.height * .5f;

			rectCorners = new[]
			{
				new Position(position.x - halfRectWidth, position.y - halfRectHeight),
				new Position(position.x + halfRectWidth, position.y - halfRectHeight),
				new Position(position.x + halfRectWidth, position.y + halfRectHeight),
				new Position(position.x - halfRectWidth, position.y + halfRectHeight),
			};
		}
	}

	public class Circle : Shape
	{
		public float radius { get; }

		public Circle(Position position, float radius) : base(position)
		{
			this.radius = radius;
		}
	}

	public static class ShapeMath
	{
		public static bool Intersects(this Circle a, Circle b)
		{
			var distBetweenCenter = Distance(a.position, b.position);
			return a.radius + b.radius >= distBetweenCenter;
		}

		public static bool Intersects(this Rectangle a, Rectangle b)
		{
			var lowerLeft = a.rectCorners[0];
			var upperRight = a.rectCorners[2];

			foreach (var corner in b.rectCorners)
			{
				if (corner.x >= lowerLeft.x && corner.y <= upperRight.y)
				{
					return true;
				}
			}

			return false;
		}

		public static bool Intersects(this Rectangle a, Circle b)
		{
			foreach (var corner in a.rectCorners)
			{
				float distCenterToCorner = Distance(corner, b.position);
				if (distCenterToCorner <= b.radius)
				{
					return true;
				}
			}

			return false;
		}

		private static float Distance(Position a, Position b)
		{
			float dx = a.x - b.x;
			float dy = a.y - b.y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public static bool Intersects(this Circle a, Rectangle b) => b.Intersects(a);
	}
}