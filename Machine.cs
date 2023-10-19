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
        var newX = Position.X + route.X;
        var newY = Position.Y + route.Y;
        var newZ = Position.Z + route.Z;
        return new Point(
            newX >= 0 && newX <= MaxLengthCm
            ? newX
            : Position.X,
            newY >= 0 && newY <= MaxWidthCm
            ? newY
            : Position.Y,
            newZ >= 0 && newZ <= MaxHeightCm
            ? newZ
            : Position.Z);
    }

    public void Point(List<double> arguments, int number)
    {
        var rand = new System.Random();
        var point = new Point(arguments[0], arguments[1], arguments[2], number)
        {
            normalX = arguments[3],
            normalY = arguments[4],
            normalZ = arguments[5],
        };
        point.realX = point.X + Math.Round((rand.NextDouble() - 0.5) * Deviation, 4);
        point.realY = point.Y + Math.Round((rand.NextDouble() - 0.5) * Deviation, 4);
        point.realZ = point.Z + Math.Round((rand.NextDouble() - 0.5) * Deviation, 4);
        DataHolder.Points.TryAdd(point.Number, point);
        //if (DataHolder.Points.TryAdd(point.Number, point))
        //    Debug.Log("Добавлена точка в список с номером " + point.Number);
        //else
        //    Debug.Log("Не удалось добавить элемент в список элементов по данному номеру. Возможно, номер занят.");
        Debug.Log(point.ToString());
    }

    public void Circle(List<double> arguments, int number)
    {
        foreach (var item in arguments)
            Debug.Log(DataHolder.Points[(int)item]);
    }

    public void Plane(List<double> arguments, int number)
    {
        foreach (var item in arguments)
            Debug.Log(DataHolder.Points[(int)item]);
    }

    public void DevCircle(List<double> arguments, int number)
    {
        
    }

    public void DevPoint(List<double> arguments, int number)
    {
        
    }
}
