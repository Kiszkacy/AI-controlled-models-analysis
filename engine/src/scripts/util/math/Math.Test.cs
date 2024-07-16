using NUnit.Framework;
using Godot;

public class MathTest : TestClass<MathTest>
{
    [Test]
    public void RadToGodot()
    {
        // check main positive angles
        Assert.That(Math.RadToGodot(0.0f), Is.EqualTo(0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/6.0f), Is.EqualTo(-Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/4.0f), Is.EqualTo(-Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/3.0f), Is.EqualTo(-Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/2.0f), Is.EqualTo(-Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi-Mathf.Pi/4.0f), Is.EqualTo(-3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi), Is.EqualTo(-Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // check main negative angles
        Assert.That(Math.RadToGodot(-0.0f), Is.EqualTo(-0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/6.0f), Is.EqualTo(Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/4.0f), Is.EqualTo(Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/3.0f), Is.EqualTo(Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/2.0f), Is.EqualTo(Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi+Mathf.Pi/4.0f), Is.EqualTo(3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi), Is.EqualTo(-Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // check main positive angles with 2pi period
        Assert.That(Math.RadToGodot(0.0f+2*Mathf.Pi), Is.EqualTo(0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/6.0f+2*Mathf.Pi), Is.EqualTo(-Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(-Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/3.0f+2*Mathf.Pi), Is.EqualTo(-Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi/2.0f+2*Mathf.Pi), Is.EqualTo(-Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi-Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(-3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(Mathf.Pi+2*Mathf.Pi), Is.EqualTo(-Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // check main negative angles with 2pi period
        Assert.That(Math.RadToGodot(-0.0f+Mathf.Pi*2), Is.EqualTo(-0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/6.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/3.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi/2.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi+Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.RadToGodot(-Mathf.Pi+2*Mathf.Pi), Is.EqualTo(-Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
    }

    [Test]
    public void GodotToRad()
    {
        // check boundary values
        Assert.That(Math.GodotToRad(0.0f), Is.EqualTo(0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.GodotToRad(Mathf.Pi), Is.EqualTo(Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.GodotToRad(-Mathf.Pi), Is.EqualTo(Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // check positive values
        Assert.That(Math.GodotToRad(Mathf.Pi/6.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.GodotToRad(Mathf.Pi/4.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.GodotToRad(Mathf.Pi/2.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // check negative values
        Assert.That(Math.GodotToRad(-Mathf.Pi/6.0f), Is.EqualTo(Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.GodotToRad(-Mathf.Pi/4.0f), Is.EqualTo(Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.GodotToRad(-Mathf.Pi/2.0f), Is.EqualTo(Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
    }

    [Test]
    public void ClampRad()
    {
        // does not clamp values in range
        Assert.That(Math.ClampRad(0.0f), Is.EqualTo(0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/6.0f), Is.EqualTo(Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/4.0f), Is.EqualTo(Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/3.0f), Is.EqualTo(Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/2.0f), Is.EqualTo(Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi-Mathf.Pi/4.0f), Is.EqualTo(3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi), Is.EqualTo(Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+Mathf.Pi/4.0f), Is.EqualTo(Mathf.Pi+Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+Mathf.Pi/3.0f), Is.EqualTo(Mathf.Pi+Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+Mathf.Pi/2.0f), Is.EqualTo(Mathf.Pi+Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // clamps values above range
        Assert.That(Math.ClampRad(Mathf.Pi*2), Is.EqualTo(0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/6.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/3.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi/2.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi-Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+2*Mathf.Pi), Is.EqualTo(Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+Mathf.Pi/4.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi+Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+Mathf.Pi/3.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi+Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(Mathf.Pi+Mathf.Pi/2.0f+2*Mathf.Pi), Is.EqualTo(Mathf.Pi+Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        
        // clamps values below range
        Assert.That(Math.ClampRad(-0.0f), Is.EqualTo(0.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi/6.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/6.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi/4.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi/3.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi/2.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi+Mathf.Pi/4.0f), Is.EqualTo(2*Mathf.Pi-3.0f/4.0f*Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi), Is.EqualTo(2*Mathf.Pi-Mathf.Pi).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi-Mathf.Pi/4.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi-Mathf.Pi/4.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi-Mathf.Pi/3.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi-Mathf.Pi/3.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
        Assert.That(Math.ClampRad(-Mathf.Pi-Mathf.Pi/2.0f), Is.EqualTo(2*Mathf.Pi-Mathf.Pi-Mathf.Pi/2.0f).Within(Config.Get().Tests.FloatComparisonTolerance));
    }
}