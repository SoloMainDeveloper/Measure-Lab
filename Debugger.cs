using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;


public static class Debugger
{
    public static void DebugCode(string inputCode)
    {
        var machine = new Machine(100, 150, 200, 0.001);
        var parsedCode = RemoveUselessSymbolsAndParse(inputCode);
        for (var i = 0; i < parsedCode.Length; i++)
        {
            var command = GetCommand(parsedCode[i]);
            switch (command.Type)
            {
                case CommandType.MOVE:
                    machine.Position = machine.Move(new Point(command.Arguments));
                    Debug.Log(machine.Position);
                    break;
                case CommandType.POINT:
                    machine.Point(command.Arguments, command.Number);
                    break;
                case CommandType.CIRCLE:
                    machine.Circle(command.Arguments, command.Number);
                    break;
                case CommandType.PLANE:
                    machine.Plane(command.Arguments, command.Number);
                    break;
                case CommandType.DEVPOINT:
                    break;
                case CommandType.DEVCIRCLE:
                    break;
                case CommandType.UNKNOWN:
                    break;
            }
        }
    }

    private static string[] RemoveUselessSymbolsAndParse(string inputCode) => inputCode
            .Replace("\n", "")
            .Replace("\t", "")
            .Replace(" ", "")
            .Split(";")
            .SkipLast(1)
            .ToArray();

    private static Command GetCommand(string input)
    {
        var indexOpenBracket = input.IndexOf("(");
        var indexUnderline = input.IndexOf("_");
        if (indexOpenBracket == -1)
        {
            return null;
        }
        var commandName = input.Substring(0, indexUnderline >=0 ? Math.Min(indexOpenBracket, indexUnderline) : indexOpenBracket).Trim();
        if (Commands.CommandList.ContainsKey(commandName))
        {
            var commandType = Commands.CommandList[commandName];
            var number = indexUnderline >= 0 ? int.Parse(input.Substring(indexUnderline + 1, indexOpenBracket - indexUnderline - 1)) : -1; //todo
            var arguments = GetArguments(commandType, input);
            return new Command(commandType, arguments, number);
        }
        else
        {
            return null;
        }
    }

    private static List<double> GetArguments(CommandType type, string input)
    {   
        var indexOpenBracket = input.IndexOf("(");
        var indexEndBracket = input.LastIndexOf(")");
        var indexOpenSquareBracket = input.IndexOf("[");
        var indexEndSquareBracket = input.IndexOf("]");
        var arguments = SubstringArguments(input, indexOpenBracket, indexEndBracket, type);
        if (type == CommandType.POINT)
        {
            var extraArgs = SubstringArguments(input, indexOpenSquareBracket, indexEndSquareBracket, type);
            arguments.AddRange(extraArgs);
        }
        return arguments;
    }

    private static List<double> SubstringArguments(string input, int start, int end, CommandType type)
    {
        Debug.Log(input);
        //Debug.Log(start);
        //Debug.Log(end);
        return type switch
        {
            CommandType.MOVE or CommandType.POINT => input
                                .Substring(start + 1, end - start - 1)
                                .Split(',')
                                .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                                .ToList(),
            CommandType.PLANE or CommandType.CIRCLE => input
                                .Substring(start + 1, end - start - 1)
                                .Replace("POINT_", "")
                                .Split(",")
                                .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                                .ToList(),
            CommandType.DEVPOINT => null,
            CommandType.DEVCIRCLE => null,
            CommandType.UNKNOWN => null,
            _ => null,
        };
    }
}
