using AdminGeneratorAttribute;

namespace AttributeTest;

public partial class UnitTest1 : IChainedGenerate
{
    [Setter] private bool value1;
    [Setter] private bool value2;
    [Setter] private bool value3;

    [Fact]
    public void Test1()
    {
        
    }
}