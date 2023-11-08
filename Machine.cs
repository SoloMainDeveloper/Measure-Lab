using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Machine
{
    public Point Position;

    public readonly double MaxHeightCm;
    public readonly double MaxWidthCm;
    public readonly double MaxLengthCm;
    public readonly double Deviation;

    public Machine(double height, double width, double length, double deviation, Point position = null)
    {
        MaxHeightCm = height;
        MaxLengthCm = length;
        MaxWidthCm = width;
        Deviation = deviation;
        Position = position ?? new Point(0, 0, MaxHeightCm);
    }

    public Point Move(Point route)
    {
        var wantedX = Position.X + route.X;
        var wantedY = Position.Y + route.Y;
        var wantedZ = Position.Z + route.Z;
        var newX = wantedX >= 0 && wantedX <= MaxLengthCm ? wantedX : Position.X;
        var newY = wantedY >= 0 && wantedY <= MaxWidthCm ? wantedY : Position.Y;
        var newZ = wantedZ >= 0 && wantedZ <= MaxHeightCm ? wantedZ : Position.Z;
        return new Point(newX, newY, newZ);
    }

    public bool Point(List<double> arguments, int number, out Point point)
    {
        var rand = new System.Random();
        point = new Point(new List<double>(arguments.GetRange(0, 6)), 6, number);
        var realX = point.X + Math.Round((rand.NextDouble() - 0.5) * Deviation, 4);
        var realY = point.Y + Math.Round((rand.NextDouble() - 0.5) * Deviation, 4);
        var realZ = point.Z + Math.Round((rand.NextDouble() - 0.5) * Deviation, 4);
        var normalRealX = point.Normal.VectorX + Math.Round((rand.NextDouble() - 0.5) * Deviation * 0.5, 4);
        var normalRealY = point.Normal.VectorY + Math.Round((rand.NextDouble() - 0.5) * Deviation * 0.5, 4);
        var normalRealZ = point.Normal.VectorZ + Math.Round((rand.NextDouble() - 0.5) * Deviation * 0.5, 4);
        point.UpdateCoordinates(realX, realY, realZ);
        point.Normal.UpdateFactVector(normalRealX, normalRealY, normalRealZ);
        return DataHolder.Points.TryAdd(point.Number, point);
    }

    public bool Circle(List<double> arguments, int number, out Circle circle)
    {
        circle = null;
        var points = new List<Point>();
        foreach (var item in arguments)
        {
            if (DataHolder.Points.TryGetValue((int)item, out Point point))
                points.Add(point);
            else
                return false;
        }
        var equation = GetNormalAndDifference(points[0], points[1], points[2]);
        var radius = GetRadiusBy3Points(points[0], points[1], points[2]);
        var centre = GetCircleCentre(points[0], points[1], points[2], number);
        circle = new Circle(centre, radius.Item1, radius.Item2, equation.Item1, number, equation.Item2, equation.Item3);
        return DataHolder.Circles.TryAdd(circle.Number, circle);
    }

    private (double, double) GetRadiusBy3Points(Point p1, Point p2, Point p3)
    {
        var length1 = GetSegmentLength(p1, p2).Item1;
        var length2 = GetSegmentLength(p2, p3).Item1;
        var length3 = GetSegmentLength(p1, p3).Item1;
        var halfPerim = (length1 + length2 + length3) / 2;
        var square = Math.Sqrt(halfPerim * (halfPerim - length1) * (halfPerim - length2) * (halfPerim - length3));
        var radius = Math.Round(length1 * length2 * length3 / (4 * square), 4);

        var realLength1 = GetSegmentLength(p1, p2).Item2;
        var realLength2 = GetSegmentLength(p2, p3).Item2;
        var realLength3 = GetSegmentLength(p1, p3).Item2;
        var realHalfPerim = (realLength1 + realLength2 + realLength3) / 2;
        var realSquare = Math.Sqrt(realHalfPerim * (realHalfPerim - realLength1) * (realHalfPerim - realLength2) * (realHalfPerim - realLength3));
        var realRadius = Math.Round(realLength1 * realLength2 * realLength3 / (4 * realSquare), 4);

        return (radius, realRadius);
    }

    private (double, double) GetSegmentLength(Point p1, Point p2)
    {
        var length = Math.Round(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2)), 4);
        var realLength = Math.Round(Math.Sqrt(Math.Pow(p1.RealX - p2.RealX, 2) + Math.Pow(p1.RealY - p2.RealY, 2) + Math.Pow(p1.RealZ - p2.RealZ, 2)), 4);
        return (length, realLength);
    }

    public bool Plane(List<double> arguments, int number, out Plane plane)
    {
        plane = null;
        var points = new List<Point>();
        foreach (var item in arguments)
        {
            if (DataHolder.Points.TryGetValue((int)item, out Point point))
                points.Add(point);
            else
                return false;
        }
        var equation = GetNormalAndDifference(points[0], points[1], points[2]);
        plane = new Plane(points[0], equation.Item1, number, equation.Item2, equation.Item3);
        return DataHolder.Planes.TryAdd(plane.Number, plane);
    }

    public string DevCircle(List<double> arguments, int number)
    {
        var devCentre = arguments[0];
        var devRadius = arguments[1];
        if (DataHolder.Circles.TryGetValue(number, out var circle))
        {
            var centre = circle.Centre;
            var diffCentre = Math.Sqrt(Math.Pow(centre.X - centre.RealX, 2) + Math.Pow(centre.Y - centre.RealY, 2) + Math.Pow(centre.Z - centre.RealZ, 2));
            var diffRadius = Math.Abs(circle.Radius - circle.FactRadius);
            return $"Отклонение центра от номинала <b>{Math.Round(diffCentre, 6)}</b> см. Норма отклонения <b>{devCentre}</b> см. В норме: <b>{diffCentre < devCentre}</b>\n" +
                $"Отклонение радиуса от номинала <b>{Math.Round(diffRadius, 6)}</b> см. Норма отклонения <b>{devRadius}</b> см. В норме: <b>{diffRadius < devRadius}</b>";
        }
        else
        {
            return "<color=red>Окружности с данным номером в списке нет!</color>";
        }
    }

    public string DevPoint(List<double> arguments, int number)
    {
        var deviation = arguments[0];
        if (DataHolder.Points.TryGetValue(number, out var point))
        {
            var distance = Math.Sqrt(Math.Pow(point.X - point.RealX, 2) + Math.Pow(point.Y - point.RealY, 2) + Math.Pow(point.Z - point.RealZ, 2));
            return $"Отклонение от номинала <b>{Math.Round(distance, 6)}</b> см. Норма отклонения <b>{deviation}</b> см. В норме: <b>{deviation > distance}</b>";
        }
        else
        {
            return "<color=red>Точки с данным номером в списке нет!</color>";
        }
    }

    public bool Projection(List<double> arguments, int number, out Point projectionPoint)
    {
        var planeNumber = arguments[0];
        var pointNumber = arguments[1];
        projectionPoint = new Point(0, 0, 0);
        if (DataHolder.Points.TryGetValue((int)pointNumber, out var point) && DataHolder.Planes.TryGetValue((int)planeNumber, out var plane))
        {
            var normal = plane.Normal;
            var paramT = -1 * (normal.VectorX * point.X + normal.VectorY * point.Y + normal.VectorZ * point.Z + plane.Difference)
                / (Math.Pow(normal.VectorX, 2) + Math.Pow(normal.VectorY, 2) + Math.Pow(normal.VectorZ, 2));
            var x = normal.VectorX * paramT + point.X;
            var y = normal.VectorY * paramT + point.Y;
            var z = normal.VectorZ * paramT + point.Z;
            var realParamT = -1 * (normal.ActualVectorX * point.RealX + normal.ActualVectorY * point.RealY + normal.ActualVectorZ * point.RealZ + plane.Difference)
                / (Math.Pow(normal.ActualVectorX, 2) + Math.Pow(normal.ActualVectorY, 2) + Math.Pow(normal.ActualVectorZ, 2));
            var realX = normal.ActualVectorX * realParamT + point.RealX;
            var realY = normal.ActualVectorY * realParamT + point.RealY;
            var realZ = normal.ActualVectorZ * realParamT + point.RealZ;
            projectionPoint = new Point(Math.Round(x, 4), Math.Round(y, 4), Math.Round(z, 4), number);
            projectionPoint.UpdateCoordinates(Math.Round(realX, 4), Math.Round(realY, 4), Math.Round(realZ, 4));
            return DataHolder.Points.TryAdd(projectionPoint.Number, projectionPoint);
        }
        else
        {
            return false;
        }
    }

    public string Comment(string comment)
    {
        return $"<color=blue>Комментарий: <b><i>{comment}</i></b></color>";
    }

    public string UnknownCommand(string input)
    {
        return $"<color=red>Неизвестная команда или неправильный ввод: <b>{input}</b></color>";
    }

    private (Vector, double, double) GetNormalAndDifference(Point start, Point p2, Point p3)
    {
        var coefX = GetDetermintantDoubleMatrix(p2.Y - start.Y, p3.Y - start.Y, p2.Z - start.Z, p3.Z - start.Z);
        var coefY = -GetDetermintantDoubleMatrix(p2.X - start.X, p3.X - start.X, p2.Z - start.Z, p3.Z - start.Z);
        var coefZ = GetDetermintantDoubleMatrix(p2.X - start.X, p3.X - start.X, p2.Y - start.Y, p3.Y - start.Y);
        var d = Math.Round(coefX * (-start.X) + coefY * (-start.Y) + coefZ * (-start.Z), 4);

        var coefX2 = GetDetermintantDoubleMatrix(p2.RealY - start.RealY, p3.RealY - start.RealY, p2.RealZ - start.RealZ, p3.RealZ - start.RealZ);
        var coefY2 = -GetDetermintantDoubleMatrix(p2.RealX - start.RealX, p3.RealX - start.RealX, p2.RealZ - start.RealZ, p3.RealZ - start.RealZ);
        var coefZ2 = GetDetermintantDoubleMatrix(p2.RealX - start.RealX, p3.RealX - start.RealX, p2.RealY - start.RealY, p3.RealY - start.RealY);
        var d2 = Math.Round(coefX2 * (-start.RealX) + coefY2 * (-start.RealY) + coefZ2 * (-start.RealZ), 4);
        var normal = new Vector(coefX, coefY, coefZ);
        normal.UpdateFactVector(Math.Round(coefX2, 4), Math.Round(coefY2, 4), Math.Round(coefZ2, 4));
        return (normal, d, d2);
    }

    private double GetDetermintantDoubleMatrix(double n1, double n2, double m1, double m2)
    {
        return n1 * m2 - m1 * n2;
    }

    private Point GetCircleCentre(Point p1, Point p2, Point p3, int number = -1)
    {
        var centre = GetCenterPoint(new double[] { p1.X, p1.Y, p1.Z },
                                    new double[] { p2.X, p2.Y, p2.Z },
                                    new double[] { p3.X, p3.Y, p3.Z });
        var actualCentre = GetCenterPoint(new double[] { p1.RealX, p1.RealY, p1.RealZ },
                                    new double[] { p2.RealX, p2.RealY, p2.RealZ },
                                    new double[] { p3.RealX, p3.RealY, p3.RealZ });
        var result = new Point(centre[0], centre[1], centre[2], number * 100 + number);
        result.UpdateCoordinates(actualCentre[0], actualCentre[1], actualCentre[2]);
        DataHolder.Points.TryAdd(result.Number, result);
        return result;
    }

    //начало заимствования
    private double[] GetCenterPoint(double[] p1, double[] p2, double[] p3)
    {
        //Находим нормальный вектор
        var normalVector = GetNormalVector(p1, p2, p3);
        var sidesSquareNorVec = normalVector[0] * normalVector[0] +
        normalVector[1] * normalVector[1] + normalVector[2] * normalVector[2];

        //Находим коэфициенты уравнений
        var coefFirstEquat = GetCoefficientEquation(p1);
        var coefSecondEquat = GetCoefficientEquation(p2);
        var coefThirdEquat = GetCoefficientEquation(p3);

        //Решаем сиcтему уравнений методом Крамера
        var fourthPoint = new double[3];
        var generalDeterminant = GetValueDeterminant(0, 1, 2, coefFirstEquat, coefSecondEquat, coefThirdEquat);
        var firstDeterminant = GetValueDeterminant(3, 1, 2, coefFirstEquat, coefSecondEquat, coefThirdEquat);
        var secondDeterminant = GetValueDeterminant(0, 3, 2, coefFirstEquat, coefSecondEquat, coefThirdEquat);
        var thirdDeterminant = GetValueDeterminant(0, 1, 3, coefFirstEquat, coefSecondEquat, coefThirdEquat);

        fourthPoint[0] = firstDeterminant / generalDeterminant;
        fourthPoint[1] = secondDeterminant / generalDeterminant;
        fourthPoint[2] = thirdDeterminant / generalDeterminant;

        //Находим центр окружности
        var newNormalVector = GetNewNormalVector(normalVector, p1, fourthPoint, sidesSquareNorVec);
        var centerPoint = new double[3];
        for (var i = 0; i < 3; i++)
        {
            centerPoint[i] = Math.Round(fourthPoint[i] - newNormalVector[i], 4);
        }

        return centerPoint;
    }

    private double[] GetNormalVector(double[] p1, double[] p2, double[] p3)
    {
        var firstVector = new double[3];
        var secondVector = new double[3];
        var normalVector = new double[3];

        for (var i = 0; i < 3; i++)
        {
            firstVector[i] = p2[i] - p1[i];
            secondVector[i] = p3[i] - p1[i];
        }
        normalVector[0] = Math.Round(firstVector[1] * secondVector[2] - firstVector[2] * secondVector[1], 4);
        normalVector[1] = Math.Round(-(firstVector[0] * secondVector[2] - firstVector[2] * secondVector[0]), 4);
        normalVector[2] = Math.Round(firstVector[0] * secondVector[1] - firstVector[1] * secondVector[0], 4);

        return normalVector;
    }

    private double[] GetCoefficientEquation(double[] point)
    {
        var coefficientsEquat = new double[4];

        for (var i = 0; i < 3; i++)
            coefficientsEquat[i] = 2 * point[i];
        coefficientsEquat[3] = point[0] * point[0] + point[1] * point[1] + point[2] * point[2];

        return coefficientsEquat;
    }

    private double GetValueDeterminant(int i, int j, int k,
    double[] coefFirstEquat, double[] coefSecondEquat, double[] coefThirdEquat)
    {
        var determinant = coefFirstEquat[i] * coefSecondEquat[j] * coefThirdEquat[k] +
        coefFirstEquat[j] * coefSecondEquat[k] * coefThirdEquat[i] +
        coefFirstEquat[k] * coefSecondEquat[i] * coefThirdEquat[j] -
        (coefFirstEquat[k] * coefSecondEquat[j] * coefThirdEquat[i] +
        coefFirstEquat[i] * coefSecondEquat[k] * coefThirdEquat[j] +
        coefFirstEquat[j] * coefSecondEquat[i] * coefThirdEquat[k]);

        return determinant;
    }

    private double[] GetNewNormalVector(double[] normalVector, double[] firstPoint,
    double[] fourthPoint, double sidesSquareNorVec)
    {
        var firstScalarProduct = 0.0;
        var secondScalarProduct = 0.0;

        for (var i = 0; i < 3; i++)
        {
            firstScalarProduct += normalVector[i] * fourthPoint[i];
            secondScalarProduct += normalVector[i] * firstPoint[i];
        }

        var k = (firstScalarProduct - secondScalarProduct) / sidesSquareNorVec;
        var newNormalVector = new double[3];

        for (var i = 0; i < 3; i++)
        {
            newNormalVector[i] = k * normalVector[i];
        }

        return newNormalVector;
    }
    //конец заимствованного кода
}
