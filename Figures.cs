using System.Collections.Generic;

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

public class Vector
{
    public double VectorX { get; private set; }
    public double VectorY { get; private set; }
    public double VectorZ { get; private set; }

    public double ActualVectorX { get; private set; }
    public double ActualVectorY { get; private set; }
    public double ActualVectorZ { get; private set; }

    public Vector(double vectorX, double vectorY, double vectorZ)
    {
        VectorX = vectorX;
        VectorY = vectorY;
        VectorZ = vectorZ;
    }

    public Vector()
    {
        VectorX = 0.0;
        VectorY = 0.0;
        VectorZ = 0.0;
        ActualVectorX = 0.0;
        ActualVectorY = 0.0;
        ActualVectorZ = 0.0;
    }

    public void UpdateFactVector(double x, double y, double z)
    {
        ActualVectorX = x;
        ActualVectorY = y;
        ActualVectorZ = z;
    }

    public override string ToString()
    {
        return $"Vector ({VectorX}, {VectorY}, {VectorZ})." +
            $" FACT Vector ({ActualVectorX}, {ActualVectorY}, {ActualVectorZ})";
    }
}

public class Point : Figure
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }

    public double RealX { get; private set; }
    public double RealY { get; private set; }
    public double RealZ { get; private set; }

    public Vector Normal { get; private set; }

    public Point(double x, double y, double z, int number = -1)
    {
        X = x;
        Y = y;
        Z = z;
        Number = number;
        Normal = new Vector();
    }

    public Point(List<double> arguments, int amount = 3, int number = -1)
    {
        X = arguments[0];
        Y = arguments[1];
        Z = arguments[2];
        Number = number;
        if (amount == 6)
        {
            Normal = new Vector(arguments[3], arguments[4], arguments[5]);
        }
        else
        {
            Normal = new Vector();
        }
    }

    public override string ToString()
    {
        return $"<b><color=#ffa500ff>Точка №{Number}</color></b>\n" +
            $"Координаты точки: ({X}, {Y}, {Z}). FACT({RealX}, {RealY}, {RealZ})\n" +
            $"Нормаль точки: {Normal}";
    }

    public void UpdateCoordinates(double x, double y, double z)
    {
        RealX = x;
        RealY = y;
        RealZ = z;
    }
}

public class Circle : Figure
{
    public Point Centre { get; private set; }
    public double Radius { get; private set; }
    public double FactRadius { get; private set; }
    public Vector Normal { get; private set; }
    public double Difference { get; private set; }
    public double FactDifference { get; private set; }

    public Circle(Point centre, double radius, double factRadius, Vector normal, int number, double difference = 0, double factDifference = 0)
    {
        Centre = centre;
        Radius = radius;
        FactRadius = factRadius;
        Normal = normal;
        Number = number;
        Difference = difference;
        FactDifference = factDifference;
    }

    public override string ToString()
    {
        return $"<b><color=#800000ff>Circle №{Number}:</color></b>\n" +
            $"Центр окружности: {Centre}\n" +
            $"Radius: {Radius}. FACT Radius: {FactRadius}\n" +
            $"Нормаль окружности: {Normal}\n" +
            $"Difference: {Difference}. FACT Difference {FactDifference}";
    }
}

public class Plane : Figure
{
    public Point Point  { get; private set; }
    public Vector Normal { get; private set; }
    public double Difference { get; private set; }
    public double FactDifference { get; private set; }

    public Plane(Point start, Vector normal, int number, double difference = 0, double factDifference = 0)
    {
        Point = start;
        Normal = normal;
        Number = number;
        Difference = difference;
        FactDifference = factDifference;
    }

    public override string ToString()
    {
        return $"<b><color=#008080ff>Плоскость №{Number}:</color></b>\n" +
            $"Точка на плоскости: {Point} \n" +
            $"Нормаль плоскости: {Normal} \n" +
            $"Difference: {Difference}. FACT Difference {FactDifference}";
    }
}
