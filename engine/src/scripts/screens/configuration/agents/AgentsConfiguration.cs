
using System.Collections.Generic;

using Godot;

public partial class AgentsConfiguration : Control
{
    [Export]
    public AgentBar FirstAgentBar { get; set; }
    [Export]
    public AgentBar SecondAgentBar { get; set; }
    [Export]
    public AgentBar ThirdAgentBar { get; set; }

    [Export]
    public InputHandler MaximumEnergyInput { get; set; }
    [Export]
    public InputHandler InitialEnergyInput { get; set; }
    [Export]
    public InputHandler EnergySurplusForReproductionInput { get; set; }
    [Export]
    public InputHandler ReproductionEnergyCostInput { get; set; }
    [Export]
    public InputHandler EnergyLossPerSecondInput { get; set; }
    [Export]
    public InputHandler EnergyLossPerSecondPer100UnitsOfMovementInput { get; set; }
    [Export]
    public InputHandler EnergyLossPerSecondRotationInput { get; set; }

    [Export]
    public InputHandler MaximumHealthInput { get; set; }
    [Export]
    public InputHandler InitialHealthInput { get; set; }
    [Export]
    public InputHandler HealthLossPerSecondInput { get; set; }
    [Export]
    public InputHandler HealthRegenPerSecondInput { get; set; }
    [Export]
    public InputHandler MinimumHealthToReproduceInput { get; set; }

    [Export]
    public InputHandler MaximumSpeedInput { get; set; }
    [Export]
    public InputHandler MaximumAccelerationInput { get; set; }
    [Export]
    public InputHandler MaximumDecelerationInput { get; set; }
    [Export]
    public InputHandler MaximumTurnSpeedInput { get; set; }
    [Export]
    public InputHandler SightAngleInput { get; set; }
    [Export]
    public InputHandler SightRadiusInput { get; set; }

    [Export]
    public Slider MaximumEnergySlider { get; set; }
    [Export]
    public Slider InitialEnergySlider { get; set; }
    [Export]
    public Slider EnergySurplusForReproductionSlider { get; set; }
    [Export]
    public Slider ReproductionEnergyCostSlider { get; set; }
    [Export]
    public Slider EnergyLossPerSecondSlider { get; set; }
    [Export]
    public Slider EnergyLossPerSecondPer100UnitsOfMovementSlider { get; set; }
    [Export]
    public Slider EnergyLossPerSecondRotationSlider { get; set; }
    [Export]
    public Slider MaximumHealthSlider { get; set; }
    [Export]
    public Slider InitialHealthSlider { get; set; }
    [Export]
    public Slider HealthLossPerSecondSlider { get; set; }
    [Export]
    public Slider HealthRegenPerSecondSlider { get; set; }
    [Export]
    public Slider MinimumHealthToReproduceSlider { get; set; }
    [Export]
    public Slider MaximumSpeedSlider { get; set; }
    [Export]
    public Slider MaximumAccelerationSlider { get; set; }
    [Export]
    public Slider MaximumDecelerationSlider { get; set; }
    [Export]
    public Slider MaximumTurnSpeedSlider { get; set; }
    [Export]
    public Slider SightAngleSlider { get; set; }
    [Export]
    public Slider SightRadiusSlider { get; set; }
    [Export]
    public ColorPickerButton ColorPicker { get; set; }
    [Export]
    public InputHandler NameInput { get; set; }
    [Export]
    public InputHandler SizeInput { get; set; }
    [Export]
    public Slider SizeSlider { get; set; }

    private AgentBar GetBarByIndex(int index) => index == 0 ? this.FirstAgentBar : index == 1 ? this.SecondAgentBar : this.ThirdAgentBar;

    private int currentlyActiveBarIndex => this.FirstAgentBar.Status == Status.Active ? 0 : this.SecondAgentBar.Status == Status.Active ? 1 : 2;

    public override void _Ready()
    {
        this.FirstAgentBar.NameLabel.Text = "name";
        this.SecondAgentBar.NameLabel.Text = "name";
        this.ThirdAgentBar.NameLabel.Text = "name";
        this.FirstAgentBar.EnergyLabel.Text = 200.ToString();
        this.SecondAgentBar.EnergyLabel.Text = 200.ToString();
        this.ThirdAgentBar.EnergyLabel.Text = 200.ToString();

        this.FirstAgentBar.Status = Status.Active;
        this.FirstAgentBar.TrashButton.Disabled = true;
        this.FirstAgentBar.TrashButton.UpdateAfterDisabledChange();
        this.SecondAgentBar.Status = Status.Empty;
        this.ThirdAgentBar.Status = Status.Hidden;

        this.LoadBarDataIntoInputs();

        this.FirstAgentBar.DeleteButtonPressed += () => this.OnAgentBarDeleteClick(0);
        this.SecondAgentBar.DeleteButtonPressed += () => this.OnAgentBarDeleteClick(1);
        this.ThirdAgentBar.DeleteButtonPressed += () => this.OnAgentBarDeleteClick(2);
        this.FirstAgentBar.Pressed += () => this.OnAgentBarClick(0);
        this.SecondAgentBar.Pressed += () => this.OnAgentBarClick(1);
        this.ThirdAgentBar.Pressed += () => this.OnAgentBarClick(2);

        this.MaximumEnergyInput.TextSubmitted += this.OnMaximumEnergyChange;
        this.InitialEnergyInput.TextSubmitted += this.OnInitialEnergyChange;
        this.EnergySurplusForReproductionInput.TextSubmitted += this.OnEnergySurplusForReproductionChange;
        this.ReproductionEnergyCostInput.TextSubmitted += this.OnReproductionEnergyCostChange;
        this.EnergyLossPerSecondInput.TextSubmitted += this.OnEnergyLossPerSecondChange;
        this.EnergyLossPerSecondPer100UnitsOfMovementInput.TextSubmitted += this.OnEnergyLossPerSecondPer100UnitsOfMovementChange;
        this.EnergyLossPerSecondRotationInput.TextSubmitted += this.OnEnergyLossPerSecondRotationChange;

        this.MaximumHealthInput.TextSubmitted += this.OnMaximumHealthChange;
        this.InitialHealthInput.TextSubmitted += this.OnInitialHealthChange;
        this.HealthLossPerSecondInput.TextSubmitted += this.OnHealthLossPerSecondChange;
        this.HealthRegenPerSecondInput.TextSubmitted += this.OnHealthRegenPerSecondChange;
        this.MinimumHealthToReproduceInput.TextSubmitted += this.OnMinimumHealthToReproduceChange;

        this.MaximumSpeedInput.TextSubmitted += this.OnMaximumSpeedChange;
        this.MaximumAccelerationInput.TextSubmitted += this.OnMaximumAccelerationChange;
        this.MaximumDecelerationInput.TextSubmitted += this.OnMaximumDecelerationChange;
        this.MaximumTurnSpeedInput.TextSubmitted += this.OnMaximumTurnSpeedChange;
        this.SightAngleInput.TextSubmitted += this.OnSightAngleChange;
        this.SightRadiusInput.TextSubmitted += this.OnSightRadiusChange;

        this.MaximumEnergySlider.DragEnded += this.OnMaximumEnergyDrag;
        this.InitialEnergySlider.DragEnded += this.OnInitialEnergyDrag;
        this.EnergySurplusForReproductionSlider.DragEnded += this.OnEnergySurplusForReproductionDrag;
        this.ReproductionEnergyCostSlider.DragEnded += this.OnReproductionEnergyCostDrag;
        this.EnergyLossPerSecondSlider.DragEnded += this.OnEnergyLossPerSecondDrag;
        this.EnergyLossPerSecondPer100UnitsOfMovementSlider.DragEnded += this.OnEnergyLossPerSecondPer100UnitsOfMovementDrag;
        this.EnergyLossPerSecondRotationSlider.DragEnded += this.OnEnergyLossPerSecondRotationDrag;
        this.MaximumHealthSlider.DragEnded += this.OnMaximumHealthDrag;
        this.InitialHealthSlider.DragEnded += this.OnInitialHealthDrag;
        this.HealthLossPerSecondSlider.DragEnded += this.OnHealthLossPerSecondDrag;
        this.HealthRegenPerSecondSlider.DragEnded += this.OnHealthRegenPerSecondDrag;
        this.MinimumHealthToReproduceSlider.DragEnded += this.OnMinimumHealthToReproduceDrag;
        this.MaximumSpeedSlider.DragEnded += this.OnMaximumSpeedDrag;
        this.MaximumAccelerationSlider.DragEnded += this.OnMaximumAccelerationDrag;
        this.MaximumDecelerationSlider.DragEnded += this.OnMaximumDecelerationDrag;
        this.MaximumTurnSpeedSlider.DragEnded += this.OnMaximumTurnSpeedDrag;
        this.SightAngleSlider.DragEnded += this.OnSightAngleDrag;
        this.SightRadiusSlider.DragEnded += this.OnSightRadiusDrag;

        this.ColorPicker.ColorChanged += this.OnColorChange;
        this.NameInput.TextSubmitted += this.OnNameChange;
        this.SizeInput.TextSubmitted += this.OnSizeChange;
        this.SizeSlider.DragEnded += this.OnSizeDrag;
    }

    private void OnAgentBarDeleteClick(int barIndex)
    {
        int currentlyActiveBarIndex = this.currentlyActiveBarIndex;

        if (barIndex == 0 && (this.SecondAgentBar.Status == Status.Inactive || this.SecondAgentBar.Status == Status.Active))
        {
            this.CopyBarInto(1, 0);
            if (this.ThirdAgentBar.Status == Status.Inactive || this.ThirdAgentBar.Status == Status.Active)
            {
                this.CopyBarInto(2, 1);
                this.ThirdAgentBar.Status = Status.Empty;
            }
            else
            {
                this.SecondAgentBar.Status = Status.Empty;
                this.ThirdAgentBar.Status = Status.Hidden;
                this.FirstAgentBar.TrashButton.Disabled = true;
                this.FirstAgentBar.TrashButton.UpdateAfterDisabledChange();
            }
        }
        else if (barIndex == 1 && this.ThirdAgentBar.Status == Status.Empty)
        {
            this.SecondAgentBar.Status = Status.Empty;
            this.ThirdAgentBar.Status = Status.Hidden;
            this.FirstAgentBar.TrashButton.Disabled = true;
            this.FirstAgentBar.TrashButton.UpdateAfterDisabledChange();
        }
        else if (barIndex == 1 && (this.ThirdAgentBar.Status == Status.Inactive || this.ThirdAgentBar.Status == Status.Active))
        {
            this.CopyBarInto(2, 1);
            this.SecondAgentBar.Status = this.ThirdAgentBar.Status;
            this.ThirdAgentBar.Status = Status.Empty;
        }
        else if (barIndex == 2)
        {
            this.ThirdAgentBar.Status = Status.Empty;
        }

        if (currentlyActiveBarIndex == barIndex)
        {
            this.FirstAgentBar.Status = Status.Active;
        }
        else if (currentlyActiveBarIndex > barIndex)
        {
            this.GetBarByIndex(currentlyActiveBarIndex-1).Status = Status.Active;
        }

        this.LoadBarDataIntoInputs();
    }

    private void OnAgentBarClick(int barIndex)
    {
        this.GetBarByIndex(this.currentlyActiveBarIndex).Status = Status.Inactive;

        AgentBar clickedBar = this.GetBarByIndex(barIndex);
        if (clickedBar.Status == Status.Inactive)
        {
            this.GetBarByIndex(barIndex).Status = Status.Active;
            this.LoadBarDataIntoInputs();
        }
        else if (clickedBar.Status == Status.Empty)
        {
            this.FirstAgentBar.TrashButton.Disabled = false;
            this.FirstAgentBar.TrashButton.UpdateAfterDisabledChange();
            this.GetBarByIndex(barIndex).Status = Status.Active;
            this.GetBarByIndex(barIndex).ResetData();
            if (barIndex == 1)
            {
                this.ThirdAgentBar.Status = Status.Empty;
            }
            this.LoadBarDataIntoInputs();
        }
    }

    private void CopyBarInto(int barFromIndex, int barToIndex)
    {
        this.GetBarByIndex(barToIndex).NameLabel.Text = this.GetBarByIndex(barFromIndex).NameLabel.Text;
        this.GetBarByIndex(barToIndex).EnergyLabel.Text = this.GetBarByIndex(barFromIndex).EnergyLabel.Text;
        this.GetBarByIndex(barToIndex).agentData = this.GetBarByIndex(barFromIndex).agentData;
    }

    private void OnMaximumEnergyChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumEnergy = args.Value.ToFloat();
        this.MaximumEnergySlider.Value = activeBar.agentData.MaximumEnergy;
        activeBar.EnergyLabel.Text = args.Value;
    }

    private void OnInitialEnergyChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.InitialEnergy = args.Value.ToFloat();
        this.InitialEnergySlider.Value = activeBar.agentData.InitialEnergy;
    }

    private void OnEnergySurplusForReproductionChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergySurplusForReproduction = args.Value.ToFloat() - activeBar.agentData.ReproductionEnergyCost;
        this.EnergySurplusForReproductionSlider.Value = activeBar.agentData.EnergySurplusForReproduction;
    }

    private void OnReproductionEnergyCostChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.ReproductionEnergyCost = args.Value.ToFloat();
        this.ReproductionEnergyCostSlider.Value = activeBar.agentData.ReproductionEnergyCost;
    }

    private void OnEnergyLossPerSecondChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergyLossPerSecond = args.Value.ToFloat();
        this.EnergyLossPerSecondSlider.Value = activeBar.agentData.EnergyLossPerSecond;
    }

    private void OnEnergyLossPerSecondPer100UnitsOfMovementChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergyLossPerSecondPer100UnitsOfMovement = args.Value.ToFloat();
        this.EnergyLossPerSecondPer100UnitsOfMovementSlider.Value = activeBar.agentData.EnergyLossPerSecondPer100UnitsOfMovement;
    }

    private void OnEnergyLossPerSecondRotationChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergyLossPerSecondRotation = args.Value.ToFloat();
        this.EnergyLossPerSecondRotationSlider.Value = activeBar.agentData.EnergyLossPerSecondRotation;
    }

    private void OnMaximumHealthChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumHealth = args.Value.ToFloat();
        this.MaximumHealthSlider.Value = activeBar.agentData.MaximumHealth;
    }

    private void OnInitialHealthChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.InitialHealth = args.Value.ToFloat();
        this.InitialHealthSlider.Value = activeBar.agentData.InitialHealth;
    }

    private void OnHealthLossPerSecondChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.HealthLossPerSecond = args.Value.ToFloat();
        this.HealthLossPerSecondSlider.Value = activeBar.agentData.HealthLossPerSecond;
    }

    private void OnHealthRegenPerSecondChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.HealthRegenPerSecond = args.Value.ToFloat();
        this.HealthRegenPerSecondSlider.Value = activeBar.agentData.HealthRegenPerSecond;
    }

    private void OnMinimumHealthToReproduceChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MinimumHealthToReproduce = args.Value.ToFloat();
        this.MinimumHealthToReproduceSlider.Value = activeBar.agentData.MinimumHealthToReproduce;
    }

    private void OnMaximumSpeedChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumSpeed = args.Value.ToFloat();
        this.MaximumSpeedSlider.Value = activeBar.agentData.MaximumSpeed;
    }

    private void OnMaximumAccelerationChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumAcceleration = args.Value.ToFloat();
        this.MaximumAccelerationSlider.Value = activeBar.agentData.MaximumAcceleration;
    }

    private void OnMaximumDecelerationChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumDeceleration = args.Value.ToFloat();
        this.MaximumDecelerationSlider.Value = activeBar.agentData.MaximumDeceleration;
    }

    private void OnMaximumTurnSpeedChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumTurnSpeed = Mathf.DegToRad(args.Value.ToFloat());
        this.MaximumTurnSpeedSlider.Value = Mathf.RadToDeg(activeBar.agentData.MaximumTurnSpeed);
    }

    private void OnSightAngleChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.SightAngle = Mathf.DegToRad(args.Value.ToFloat());
        this.SightAngleSlider.Value = Mathf.RadToDeg(activeBar.agentData.SightAngle);
    }

    private void OnSightRadiusChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.SightRadius = args.Value.ToFloat();
        this.SightRadiusSlider.Value = activeBar.agentData.SightRadius;
    }

    private void OnMaximumEnergyDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MaximumEnergyInput.SetTextTo(this.MaximumEnergySlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumEnergy = (float)this.MaximumEnergySlider.Value;
        activeBar.EnergyLabel.Text = this.MaximumEnergySlider.Value.ToString();
    }

    private void OnInitialEnergyDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.InitialEnergyInput.SetTextTo(this.InitialEnergySlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.InitialEnergy = (float)this.InitialEnergySlider.Value;
    }

    private void OnEnergySurplusForReproductionDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.EnergySurplusForReproductionInput.SetTextTo(this.EnergySurplusForReproductionSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergySurplusForReproduction = (float)this.EnergySurplusForReproductionSlider.Value;
    }

    private void OnReproductionEnergyCostDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.ReproductionEnergyCostInput.SetTextTo(this.ReproductionEnergyCostSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.ReproductionEnergyCost = (float)this.ReproductionEnergyCostSlider.Value;
    }

    private void OnEnergyLossPerSecondDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.EnergyLossPerSecondInput.SetTextTo(this.EnergyLossPerSecondSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergyLossPerSecond = (float)this.EnergyLossPerSecondSlider.Value;
    }

    private void OnEnergyLossPerSecondPer100UnitsOfMovementDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.EnergyLossPerSecondPer100UnitsOfMovementInput.SetTextTo(this.EnergyLossPerSecondPer100UnitsOfMovementSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergyLossPerSecondPer100UnitsOfMovement = (float)this.EnergyLossPerSecondPer100UnitsOfMovementSlider.Value;
    }

    private void OnEnergyLossPerSecondRotationDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.EnergyLossPerSecondRotationInput.SetTextTo(this.EnergyLossPerSecondRotationSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.EnergyLossPerSecondRotation = (float)this.EnergyLossPerSecondRotationSlider.Value;
    }

    private void OnMaximumHealthDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MaximumHealthInput.SetTextTo(this.MaximumHealthSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumHealth = (float)this.MaximumHealthSlider.Value;
    }

    private void OnInitialHealthDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.InitialHealthInput.SetTextTo(this.InitialHealthSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.InitialHealth = (float)this.InitialHealthSlider.Value;
    }

    private void OnHealthLossPerSecondDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.HealthLossPerSecondInput.SetTextTo(this.HealthLossPerSecondSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.HealthLossPerSecond = (float)this.HealthLossPerSecondSlider.Value;
    }

    private void OnHealthRegenPerSecondDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.HealthRegenPerSecondInput.SetTextTo(this.HealthRegenPerSecondSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.HealthRegenPerSecond = (float)this.HealthRegenPerSecondSlider.Value;
    }

    private void OnMinimumHealthToReproduceDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MinimumHealthToReproduceInput.SetTextTo(this.MinimumHealthToReproduceSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MinimumHealthToReproduce = (float)this.MinimumHealthToReproduceSlider.Value;
    }

    private void OnMaximumSpeedDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MaximumSpeedInput.SetTextTo(this.MaximumSpeedSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumSpeed = (float)this.MaximumSpeedSlider.Value;
    }

    private void OnMaximumAccelerationDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MaximumAccelerationInput.SetTextTo(this.MaximumAccelerationSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumAcceleration = (float)this.MaximumAccelerationSlider.Value;
    }

    private void OnMaximumDecelerationDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MaximumDecelerationInput.SetTextTo(this.MaximumDecelerationSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumDeceleration = (float)this.MaximumDecelerationSlider.Value;
    }

    private void OnMaximumTurnSpeedDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.MaximumTurnSpeedInput.SetTextTo(this.MaximumTurnSpeedSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.MaximumTurnSpeed = Mathf.DegToRad((float)this.MaximumTurnSpeedSlider.Value);
    }

    private void OnSightAngleDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.SightAngleInput.SetTextTo(this.SightAngleSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.SightAngle = Mathf.DegToRad((float)this.SightAngleSlider.Value);
    }

    private void OnSightRadiusDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.SightRadiusInput.SetTextTo(this.SightRadiusSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.SightRadius = (float)this.SightRadiusSlider.Value;
    }

    private void OnColorChange(Color color)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.Color = color;
        activeBar.Sprite.SelfModulate = color;
    }

    private void OnNameChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.NameLabel.Text = args.Value;
    }

    private void OnSizeChange(object _, BaseEventArgs<string> args)
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.SizeMultiplier = args.Value.ToFloat();
        this.SizeSlider.Value = activeBar.agentData.SizeMultiplier;
        activeBar.Sprite.Scale = new Vector2(args.Value.ToFloat(), args.Value.ToFloat());
    }

    private void OnSizeDrag(bool wasChanged)
    {
        if (!wasChanged)
        {
            return;
        }
        this.SizeInput.SetTextTo(this.SizeSlider.Value.ToString());

        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        activeBar.agentData.SizeMultiplier = (float)this.SizeSlider.Value;
        activeBar.Sprite.Scale = new Vector2((float)this.SizeSlider.Value, (float)this.SizeSlider.Value);
    }

    public List<AgentTemplate> GetAgentTemplates()
    {
        List<AgentTemplate> result = new();
        if (this.FirstAgentBar.Status == Status.Active)
        {
            result.Add(this.FirstAgentBar.agentData);
        }
        else if (this.SecondAgentBar.Status == Status.Active)
        {
            result.Add(this.SecondAgentBar.agentData);
        }
        else if (this.ThirdAgentBar.Status == Status.Active)
        {
            result.Add(this.ThirdAgentBar.agentData);
        }

        return result;
    }

    public void LoadBarDataIntoInputs()
    {
        AgentBar activeBar = this.GetBarByIndex(this.currentlyActiveBarIndex);
        AgentTemplate data = activeBar.agentData;

        this.MaximumEnergyInput.SetTextTo(data.MaximumEnergy.ToString());
        this.MaximumEnergySlider.Value = data.MaximumEnergy;

        this.InitialEnergyInput.SetTextTo(data.InitialEnergy.ToString());
        this.InitialEnergySlider.Value = data.InitialEnergy;

        this.EnergySurplusForReproductionInput.SetTextTo((data.ReproductionEnergyCost + data.EnergySurplusForReproduction).ToString());
        this.EnergySurplusForReproductionSlider.Value = data.EnergySurplusForReproduction + data.ReproductionEnergyCost;

        this.ReproductionEnergyCostInput.SetTextTo(data.ReproductionEnergyCost.ToString());
        this.ReproductionEnergyCostSlider.Value = data.ReproductionEnergyCost;

        this.EnergyLossPerSecondInput.SetTextTo(data.EnergyLossPerSecond.ToString());
        this.EnergyLossPerSecondSlider.Value = data.EnergyLossPerSecond;

        this.EnergyLossPerSecondPer100UnitsOfMovementInput.SetTextTo(data.EnergyLossPerSecondPer100UnitsOfMovement.ToString());
        this.EnergyLossPerSecondPer100UnitsOfMovementSlider.Value = data.EnergyLossPerSecondPer100UnitsOfMovement;

        this.EnergyLossPerSecondRotationInput.SetTextTo(data.EnergyLossPerSecondRotation.ToString());
        this.EnergyLossPerSecondRotationSlider.Value = data.EnergyLossPerSecondRotation;

        this.MaximumHealthInput.SetTextTo(data.MaximumHealth.ToString());
        this.MaximumHealthSlider.Value = data.MaximumHealth;

        this.InitialHealthInput.SetTextTo(data.InitialHealth.ToString());
        this.InitialHealthSlider.Value = data.InitialHealth;

        this.HealthLossPerSecondInput.SetTextTo(data.HealthLossPerSecond.ToString());
        this.HealthLossPerSecondSlider.Value = data.HealthLossPerSecond;

        this.HealthRegenPerSecondInput.SetTextTo(data.HealthRegenPerSecond.ToString());
        this.HealthRegenPerSecondSlider.Value = data.HealthRegenPerSecond;

        this.MinimumHealthToReproduceInput.SetTextTo(data.MinimumHealthToReproduce.ToString());
        this.MinimumHealthToReproduceSlider.Value = data.MinimumHealthToReproduce;

        this.MaximumSpeedInput.SetTextTo(data.MaximumSpeed.ToString());
        this.MaximumSpeedSlider.Value = data.MaximumSpeed;

        this.MaximumAccelerationInput.SetTextTo(data.MaximumAcceleration.ToString());
        this.MaximumAccelerationSlider.Value = data.MaximumAcceleration;

        this.MaximumDecelerationInput.SetTextTo(data.MaximumDeceleration.ToString());
        this.MaximumDecelerationSlider.Value = data.MaximumDeceleration;

        this.MaximumTurnSpeedInput.SetTextTo(Mathf.RadToDeg(data.MaximumTurnSpeed).ToString());
        this.MaximumTurnSpeedSlider.Value = Mathf.RadToDeg(data.MaximumTurnSpeed);

        this.SightAngleInput.SetTextTo(Mathf.RadToDeg(data.SightAngle).ToString());
        this.SightAngleSlider.Value = Mathf.RadToDeg(data.SightAngle);

        this.SightRadiusInput.SetTextTo(data.SightRadius.ToString());
        this.SightRadiusSlider.Value = data.SightRadius;

        this.ColorPicker.Color = data.Color;
        this.NameInput.SetTextTo(activeBar.NameLabel.Text);

        this.SizeInput.SetTextTo(data.SizeMultiplier.ToString());
        this.SizeSlider.Value = data.SizeMultiplier;
    }

    // this.MaximumHealthInput.SetTextTo(data.MaximumHealth.ToString());
}