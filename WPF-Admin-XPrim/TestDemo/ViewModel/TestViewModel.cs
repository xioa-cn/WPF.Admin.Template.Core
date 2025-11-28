using AdminGeneratorAttribute;

namespace TestDemo.ViewModel
{
    public partial class TestViewModel : IChainedGenerate
    {
        [Setter] private string value;

        public TestViewModel()
        {
            
        }
    }
}