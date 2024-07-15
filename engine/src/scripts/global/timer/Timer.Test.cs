using NUnit.Framework;

public class TimerTest : TestClass<TimerTest>
{
    [Test]
    public void IsInactive_BeforeActivation()
    {
        // given
        Timer timer = new(this.MockAction);

        // then
        Assert.That(timer.IsActive, Is.False);
    }

    [Test]
    public void IsActive_AfterActivation()
    {
        // given
        Timer timer = new(this.MockAction);

        // when
        timer.Activate(10.0);

        // then
        Assert.That(timer.IsActive, Is.True);
    }

    [Test]
    public void IsInactive_AfterTimeout()
    {
        // given
        Timer timer = new(this.MockAction);

        // when
        timer.Activate(10.0);
        TestUtility.SimulateProcess(timer, 10.0);

        // then
        Assert.That(timer.IsActive, Is.False);
    }

    [Test]
    public void ActionCalled_AfterTimeout()
    {
        // given
        bool actionCalled = false;
        void MockLocalAction()
        {
            actionCalled = true;
        }
        Timer timer = new(MockLocalAction);

        // when
        timer.Activate(10.0);
        TestUtility.SimulateProcess(timer, 10.0);

        // then
        Assert.That(actionCalled, Is.True);
    }

    [Test]
    public void ActionNotCalled_BeforeTimeout()
    {
        // given
        bool actionCalled = false;
        void MockLocalAction()
        {
            actionCalled = true;
        }
        Timer timer = new(MockLocalAction);

        // when
        timer.Activate(10.0);
        TestUtility.SimulateProcess(timer, 5.0);

        // then
        Assert.That(actionCalled, Is.False);
    }

    private void MockAction() { }
}