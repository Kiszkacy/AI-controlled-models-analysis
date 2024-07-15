using NUnit.Framework;

public class TimerTest : TestClass<TimerTest>
{
    [Test]
    public void IsActive_ShouldBeFalseBeforeActivation()
    {
        // given
        Timer timer = new(this.MockAction);

        // then
        Assert.That(timer.IsActive, Is.False);
    }

    [Test]
    public void IsActive_ShouldBeTrueAfterActivation()
    {
        // given
        Timer timer = new(this.MockAction);

        // when
        timer.Activate(10.0);

        // then
        Assert.That(timer.IsActive, Is.True);
    }

    [Test]
    public void IsActive_ShouldBeFalseAfterTimeout()
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
    public void Action_ShouldBeCalledAfterTimeout()
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
    public void Action_ShouldNotBeCalledBeforeTimeout()
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