using WPF.Admin.Service.Utils;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace KeyenceCollectionModule
{
    [Module(nameof(KeyenceCollectionModule))]
    public class KeyenceCollectionModule : AutoModule<KeyenceCollectionModule>
    {
        public override void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}