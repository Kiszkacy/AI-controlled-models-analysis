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
            .Print($"  | SET SEED TO {Config.Instance.Environment.Seed}")
            .End();
        RandomGenerator.SetSeed(Config.Instance.Environment.Seed);

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
            .Print("  | ENGINE SETUP")
            .End();
        this.SetupEngine();

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
            .Print("  | INITIAL LOAD COMPLETE")
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
                .Print("  | TESTS COMPLETED")
                .End();
        }

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
            .Print("  | GENERATING ENVIRONMENT")
            .End();
        this.GenerateEnvironment();

        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
            .Print("  | ENVIRONMENT SETUP")
            .End();
        this.SetupEnvironment();
    }

    private void LoadSingletons() // this method loads singletons that are required to be loaded in a specific order
    {
        NeatPrinter.Start()
            .ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
            .Print("  | LOADING CONFIG")
            .End();
        this.LoadConfig();
        EventManager.Get();
    }

    private void LoadConfig()
    {
        Config.Get();
        CommandLineReader.ParseCustomArguments();
    }

    private void SetupEngine()
    {
        Engine.TimeScale = Config.Get().Data.Engine.TimeScale;
        Engine.PhysicsTicksPerSecond = Config.Get().Data.Engine.TicksPerSecond;
    }

    private void RunTests()
    {
        TestRunner.Get().Run();
    }

    private void GenerateEnvironment() // TODO temporary, remove me later
    {
        EnvironmentGenerator environmentGenerator = EnvironmentGeneratorBuilder.Start.SetAllToDefault().End();
        EnvironmentTemplate environmentTemplate = environmentGenerator.Generate();
        EntityManager.Instance.Initialize(environmentTemplate.GenerationSettings
                .Size); // IMPORTANT: EntityManager must initialize before environment instantiate
        Node parent = this.GetParent<Node>();
        Environment environment = ((Environment)(parent.GetNode("Environment")));
        environment.Initialize(environmentTemplate);

        ((Node2D)(parent.GetNode("Camera"))).GlobalPosition = environment.Size / 2.0f;
    }

    private void SetupEnvironment() // TODO remove me too
    {
        AgentSightRayCastManager.Instance.Initialize(this.GetParent(), true);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("reload.scene"))
        {
            NeatPrinter.Start()
                .ColorPrint(ConsoleColor.Blue, "[INITIALIZER]")
                .Print("  | RELOADING SCENE")
                .End();
            Reloader.Get().Reload(this.GetParent<Node>());
            GetTree().ReloadCurrentScene();
        }
    }

    public Initializer()
    {

    }
}
