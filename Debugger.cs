using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UI;



public static class Debugger
{
    private static int commandNumber = 0;

    public static void RunCode(InputField output, string inputCode, int type)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        var machine = new Machine(100, 150, 200, 0.001);
        var parsedCode = RemoveUselessSymbolsAndParse(inputCode);
        var result = String.Empty;
        if (type == 0)
        {
            commandNumber = 0;
        }
        for (var i = commandNumber; i < parsedCode.Length; i++)
        {
            if (i == 0)
            {
                DataHolder.Points.Clear();
                DataHolder.Planes.Clear();
                DataHolder.Circles.Clear();
                output.text = "";
            }
            try
            {
                var command = GetCommand(parsedCode[i]);
                switch (command.Type)
                {
                    case CommandType.MOVE:
                        machine.Position = machine.Move(new Point(command.Arguments));
                        result += $"<b><color=purple>LOCATION</color></b>: {machine.Position}";
                        break;
                    case CommandType.POINT:
                        if (machine.Point(command.Arguments, command.Number, out Point point))
                            result += point.ToString() + "\n\n";
                        else
                            result += "<color=red>Не удалось добавить точку</color>\n\n";
                        break;
                    case CommandType.CIRCLE:
                        if (machine.Circle(command.Arguments, command.Number, out Circle circle))
                            result += circle.ToString() + "\n\n";
                        else
                            result += "<color=red>Не удалось выполнить операцию <b>Circle</b></color>\n\n";
                        break;
                    case CommandType.PLANE:
                        if (machine.Plane(command.Arguments, command.Number, out Plane plane))
                            result += plane.ToString() + "\n\n";
                        else
                            result += "<color=red>Не удалось выполнить операцию <b>Plane</b></color>\n\n";
                        break;
                    case CommandType.DEVPOINT:
                        result += machine.DevPoint(command.Arguments, command.Number) + "\n\n";
                        break;
                    case CommandType.DEVCIRCLE:
                        result += machine.DevCircle(command.Arguments, command.Number) + "\n\n";
                        break;
                    case CommandType.PROJECTION:
                        if (machine.Projection(command.Arguments, command.Number, out var projectionPoint))
                            result += "Проекция точки:\n" + projectionPoint + "\n\n";
                        else
                            result += "<color=red>Не удалось найти проекцию точки. Проверьте правильность ввода данных.</color>\n\n";
                        break;
                    case CommandType.COMMENT:
                        result += machine.Comment(command.Comment) + "\n\n";
                        break;
                    case CommandType.UNKNOWN:
                        result += machine.UnknownCommand(command.Comment) + "\n\n";
                        break;
                }
            }
            catch
            {
                result += $"<color=red>Не удалось выполнить операцию <b>{parsedCode[i]}</b></color>\n\n";
            }
            
            if (type != 0)
            {
                commandNumber += 1;
                output.text += result;
                if (i + 1 == parsedCode.Length)
                    commandNumber = 0;
                break;
            }
            
        }
        if (type == 0)
        {
            output.text = result;
            commandNumber = 0;
        }
    }

    public static void ReloadCommandNumber() => commandNumber = 0;

    private static string[] RemoveUselessSymbolsAndParse(string inputCode)
    {
        var input = inputCode.Split(";").SkipLast(1).ToArray();
        for (var i = 0; i < input.Length; i++)
        {
            input[i] = input[i].Trim().Replace("\n", "").Replace("\t", "");
            if (input[i][..2] != "//")
            {
                input[i] = input[i].Replace(" ", "");
            }
            //str.Trim()
            //   .Replace("\n", "")
            //   .Replace("\t", "");
            //if (str[..2] != "//")
            //{
            //    Debug.Log(1);
            //    str.Replace(" ", "");
            //}
            //Debug.Log(input[i]);
        }
        return input;
        //return input.ToArray();
            //inputCode
            //.Replace("\n", "")
            //.Replace("\t", "")
            //.Replace(" ", "")
            //.Split(";")
            //.SkipLast(1)
            //.ToArray();
    }
        

    private static Command GetCommand(string input)
    {
        var indexOpenBracket = input.IndexOf("(");
        var indexUnderline = input.IndexOf("_");
        //Debug.Log(indexOpenBracket + " " + indexUnderline);
        if (indexOpenBracket != -1 || indexUnderline != -1)
        {
            var index = GetRightIndex(indexOpenBracket, indexUnderline);
            var commandName = input[..index].Trim();
            if (Commands.CommandList.ContainsKey(commandName))
            {
                var commandType = Commands.CommandList[commandName];
                var number = GetNumber(input, index);
                var arguments = TryGetArguments(commandType, input);
                return new Command(commandType, arguments, number);
            }
        }
        else if (input.Trim()[..2] == "//")
        {
            return new Command(CommandType.COMMENT, input.Trim()[2..]);
        }
        return new Command(CommandType.UNKNOWN, input);
    }

    private static List<double> TryGetArguments(CommandType type, string input)
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
        //Debug.Log(input);
        return type switch
        {
            CommandType.MOVE or CommandType.POINT or CommandType.DEVPOINT => input
                                .Substring(start + 1, end - start - 1)
                                .Split(',')
                                .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                                .ToList(),
            CommandType.PLANE or CommandType.CIRCLE  or CommandType.PROJECTION or CommandType.DEVCIRCLE => input
                                .Substring(start + 1, end - start - 1)
                                .Replace("POINT_", "")
                                .Replace("PLANE_", "")
                                .Split(",")
                                .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                                .ToList(),
            CommandType.UNKNOWN => null,
            _ => null,
        };
    }

    private static int GetRightIndex(int index1, int index2)
    {
        if (index1 < 0)
            return index2;
        else if (index2 < 0)
            return index1;
        return Math.Min(index1, index2);
    }

    private static int GetNumber(string input, int index)
    {
        var number = new StringBuilder();
        for (var i = index + 1; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]))
                number.Append(input[i]);
            else
                break;
        }
        return number.Length > 0 ? int.Parse(number.ToString()) : -1;
    }
}
