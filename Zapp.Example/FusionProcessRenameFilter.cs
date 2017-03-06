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
            var processConfigEntry = entry as FusionProcessConfigEntry;

            var metaEntry = entry as FusionMetaEntry;

            if (processEntry != null)
            {
                processEntry.Name = config.Id;
            }

            if (processConfigEntry != null)
            {
                processConfigEntry.Name = $"{config.Id}.config";
            }

            if (metaEntry != null)
            {
                metaEntry.SetInfo(FusionMetaEntry.ExecutableInfoKey, config.Id);
            }
        }
    }
}
