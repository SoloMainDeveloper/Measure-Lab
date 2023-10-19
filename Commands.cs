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
            { "DEVPOINT", CommandType.DEVPOINT},
            { "DEVCIRCLE", CommandType.DEVCIRCLE}
    };
}

//public interface ICommand
//{
//    void DoCommand();
//}

public class Command
{
    public readonly CommandType Type;
    public readonly List<double> Arguments;
    public readonly int Number;

    public Command(CommandType type, List<double> arguments, int number)
    {
        Type = type;
        Arguments = arguments;
        Number = number;
    }
}
//public class MoveCommand : ICommand
//{
//    public void DoCommand()
//    {
//        throw new System.NotImplementedException();
//    }
//}

public enum CommandType
{
    MOVE,
    POINT,
    PLANE,
    CIRCLE,
    DEVPOINT,
    DEVCIRCLE,
    UNKNOWN
}