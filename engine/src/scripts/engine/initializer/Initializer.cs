using System;
using Godot;

public partial class Initializer : Node
{
	public override void _Ready()
	{
		NeatPrinter.Start()
			.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
			.Print("  | LOADING SCENE")
			.NewLine()
			.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
			.Print("  | LOADING SINGLETONS")
			.End();
		this.LoadSingletons();
		NeatPrinter.Start()
			.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
			.Print("  | SETTING UP ENGINE SETTINGS")
			.End();
		this.SetupEngineSettings();
		NeatPrinter.Start()
			.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
			.Print("]  | INITIAL LOAD COMPLETE")
			.End();
		if ((!CommandLineReader.OpenedViaCommandLine && Config.Get().Tests.RunTests) || (CommandLineReader.OpenedViaCommandLine && Config.Get().Tests.RunTestsWhenOpenedViaCommandLine))
		{
			NeatPrinter.Start()
				.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
				.Print("  | STARTING TESTS")
				.End();
			this.RunTests();
			NeatPrinter.Start()
				.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
				.Print("  | TESTS COMPLETED").End();
		}
	}

	private void LoadSingletons() // this method loads singletons that are required to be loaded in a specific order
	{
		NeatPrinter.Start()
			.ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
			.Print("  | LOADING CONFIG").End();
		this.LoadConfig();
		EventManager.Get();
	}

	private void LoadConfig()
	{
		Config.Get();
		CommandLineReader.ParseCustomArguments();
	}
	
	private void SetupEngineSettings()
	{
		Engine.TimeScale = Config.Get().Data.Engine.TimeScale;
		Engine.PhysicsTicksPerSecond = Config.Get().Data.Engine.TicksPerSecond;
	}

	private void RunTests()
	{
		TestRunner.Get().Run();
	}

	public Initializer()
	{
		
	}
}
