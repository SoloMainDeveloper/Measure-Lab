using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Commands
{
    public static readonly Dictionary<string, CommandType> CommandList = new Dictionary<string, CommandType>
    {
            { "MOVE", CommandType.MOVE},
            { "POINT", CommandType.POINT},
            { "PLANE", CommandType.PLANE},
            { "CIRCLE", CommandType.CIRCLE},
            { "DEVIATION-POINT", CommandType.DEVPOINT},
            { "DEVIATION-CIRCLE", CommandType.DEVCIRCLE},
            { "POINT-BY-PROJECTION", CommandType.PROJECTION }
    };
}

public class Command
{
    public readonly CommandType Type;
    public readonly List<double> Arguments;
    public readonly int Number;

    public readonly string Comment;

    public Command(CommandType type, List<double> arguments, int number = -1)
    {
        Type = type;
        Arguments = arguments;
        Number = number;
    }

    public Command(CommandType type, string comment) //constructor for COMMENT command
    {
        Type = type;
        Number = -1;
        Comment = comment;
    }
}

public enum CommandType
{
    MOVE,
    POINT,
    PLANE,
    CIRCLE,
    DEVPOINT,
    DEVCIRCLE,
    PROJECTION,
    COMMENT,
    UNKNOWN
}