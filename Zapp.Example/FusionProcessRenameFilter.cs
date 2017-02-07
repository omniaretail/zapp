using Zapp.Config;
using Zapp.Fuse;
using Zapp.Pack;

namespace Zapp.Example
{
    public class FusionProcessRenameFilter : IFusionFilter
    {
        public void BeforeAddEntry(FusePackConfig config, IPackageEntry entry)
        {
            var processEntry = entry as FusionProcessEntry;

            if (processEntry == null)
            {
                return;
            }

            processEntry.Name = config.Id;
        }
    }
}
