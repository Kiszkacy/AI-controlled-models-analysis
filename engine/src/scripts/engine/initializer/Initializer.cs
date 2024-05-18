using Godot;

public partial class Initializer : Node
{
	public override void _Ready()
	{
		GD.Print("Initializer: Loading scene."); // TODO use NeatPrint
		GD.Print("Initializer: Loading singletons.");
		this.LoadSingletons();
		GD.Print("Initializer: Setting up engine settings.");
		this.SetupEngineSettings();
		GD.Print("Initializer: Initial load complete.");
		if ((!CommandLineReader.OpenedViaCommandLine && Config.Get().Tests.RunTests) || (CommandLineReader.OpenedViaCommandLine && Config.Get().Tests.RunTestsWhenOpenedViaCommandLine))
		{
			GD.Print("Initializer: Starting tests.");
			this.RunTests();
			GD.Print("Initializer: Tests completed.");
		}
	}

	private void LoadSingletons() // this method loads singletons that are required to be loaded in a specific order
	{
		GD.Print("Initializer: Loading config.");
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
