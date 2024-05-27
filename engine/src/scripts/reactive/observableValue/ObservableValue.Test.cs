using NUnit.Framework;

public class ObservableValueTest : TestClass<ObservableValueTest>
{
    [Test]
    public void GetValue_ShouldBeAbleToReturnValueAfterInitialization()
    {
        // given
        object value = new object();
        ObservableValue<object> observableValue = new(value);
        
        // then
        Assert.That(value, Is.EqualTo(observableValue.Value));
    }
    
    [Test]
    public void SetValue_ShouldChangeInsideValue()
    {
        // given
        object initialValue = new object();
        object newValue = new object();
        ObservableValue<object> observableValue = new(initialValue);
        
        // when
        observableValue.Value = newValue;
        
        // then
        Assert.That(initialValue, Is.Not.EqualTo(observableValue.Value));
        Assert.That(newValue, Is.EqualTo(observableValue.Value));
    }
    
    [Test]
    public void SetValue_ShouldEmitValueChangedWhenSetToTheDifferentValue()
    {
        // given
        object initialValue = new object();
        object newValue = new object();
        ObservableValue<object> observableValue = new(initialValue);
        
        bool receivedValue = false;
        void OnEmitHandler(object _, ValueChanged<object> valueChanged)
        {
            receivedValue = true;
        }

        // when
        observableValue.OnChange += OnEmitHandler;
        observableValue.Value = newValue;
        
        // then
        Assert.That(receivedValue, Is.True);
    }
    
    [Test]
    public void SetValue_ShouldNotEmitValueChangedWhenSetToTheSameValue()
    {
        // given
        object value = new object();
        ObservableValue<object> observableValue = new(value);
        
        bool receivedValue = false;
        void OnEmitHandler(object _, ValueChanged<object> valueChanged)
        {
            receivedValue = true;
        }

        // when
        observableValue.OnChange += OnEmitHandler;
        observableValue.Value = value;
        
        // then
        Assert.That(receivedValue, Is.False);
    }
}