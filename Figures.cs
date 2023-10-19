using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataHolder
{
    public static Dictionary<int, Point> Points = new();
    public static Dictionary<int, Circle> Circles = new();
    public static Dictionary<int, Plane> Planes = new();
}

public class Figure
{
    public int Number;
}

public class Point : Figure
{
    public double X { get; private set; }
    public double Y;
    public double Z;

    public double realX;
    public double realY;
    public double realZ;

    public double normalX;
    public double normalY;
    public double normalZ;

    public Point(double x, double y, double z, int number = -1)
    {
        X = x;
        Y = y;
        Z = z;
        Number = number;
    }

    public Point(List<double> arguments, int amount = 3, int number = -1)
    {
        X = arguments[0];
        Y = arguments[1];
        Z = arguments[2];
        Number = number;
        if (amount == 6)
        {
            normalX = arguments[3];
            normalY = arguments[4];
            normalZ = arguments[5];
        }
    }

    public override string ToString()
    {
        return $"Номер в списке: {Number}\n" +
            $"Координаты точки: ({X}, {Y}, {Z})\n" +
            $"Нормаль точки: [{normalX}, {normalY}, {normalZ}]\n" +
            $"Фактические координаты точки: FACT({realX}, {realY}, {realZ})\n";
    }

    //public static Point operator +(this, Point point)
    //{
    //    return new Point(this.X + point.X, this.Y + point.Y, this.Z + point.Z);
    //}

    public void UpdateCoordinates(double x, double y, double z)
    {
        realX = x;
        realY = y;
        realZ = z;
    }
}

public class Circle : Figure
{
    public Point Centre;
    public double Radius;

    public Circle(Point centre, double radius)
    {
        Centre = centre;
        Radius = radius;
    }
}

public class Plane : Figure
{
    public Point point;

    public Plane()
    {
    }
}
