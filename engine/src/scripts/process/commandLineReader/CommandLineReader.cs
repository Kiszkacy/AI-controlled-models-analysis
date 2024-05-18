using System;
using System.Linq;
using Godot;

public class CommandLineReader
{
	private static readonly string argumentPrefix = "--"; 
	private static readonly string valueSeperator = "="; 
	private static readonly string[] valueArguments = { "pipe-name" };
	
	public static bool OpenedViaCommandLine => OS.GetCmdlineArgs().Length > 1;

	public static void ParseCustomArguments()
	{
		string[] userArguments = OS.GetCmdlineUserArgs();
		
		for (int index = 0; index < userArguments.Length; index++)
		{
			string argumentName;
			string? argumentValue = null;
			string argument = userArguments[index];
			if (argument.Contains(valueSeperator))
			{
				string[] splitted = argument.Split(valueSeperator);
				(argumentName, argumentValue) = (splitted[0], splitted[1]);
				
				if (!argumentName.StartsWith(argumentPrefix))
				{
					NeatPrinter.Start().ColorPrint(ConsoleColor.Red, $"Commandline parse error: argument='{argumentName}' name does not start with '{argumentPrefix}'.").End();
					continue;
				}
				argumentName = argumentName.Remove(0, argumentPrefix.Length);
				
				if (!valueArguments.Contains(argumentName))
				{
					NeatPrinter.Start().ColorPrint(ConsoleColor.Red, $"Commandline parse error: argument='{argumentName}' name does not require any values but value='{argumentValue}' was provided.").End();
					continue;
				} 
			}
			else
			{
				argumentName = userArguments[index];
				if (!argumentName.StartsWith(argumentPrefix))
				{
					NeatPrinter.Start().ColorPrint(ConsoleColor.Red, $"Commandline parse error: argument='{argumentName}' name does not start with '{argumentPrefix}'.").End();
					continue;
				} 
				argumentName = argumentName.Remove(0, argumentPrefix.Length);
				
				if (valueArguments.Contains(argumentName))
				{
					argumentValue = userArguments[index + 1];
					index++;
				}
			}

			UpdateConfig(argumentName, argumentValue);
		}
	}

	public static void UpdateConfig(string argumentName, string? argumentValue)
	{
		NeatPrinter.Start().Print($"Commandline argument: name: '{argumentName}', value: '{argumentValue}'.").End();
		switch (argumentName)
		{
			case "pipe-name":
				NeatPrinter.Start().Print($"Commandline argument: applied '{argumentValue}' to 'Config.Data.Pipe.Name'.").End();
				Config.Get().Data.Pipe.Name = argumentValue;
				break;
		}
	}
}
