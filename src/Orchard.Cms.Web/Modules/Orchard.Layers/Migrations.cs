using Orchard.ContentManagement.Metadata.Settings;
using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

namespace Orchard.Layers
{
    public class Migrations : DataMigration
    {
        IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public int Create()
        {
            _contentDefinitionManager.AlterTypeDefinition("Layer", builder => builder
                .Creatable()
                .Draftable()
                .Listable()
                .WithPart("TitlePart", part => part.WithSetting("Position", "0"))
                .WithPart("LayerPart", part => part.WithSetting("Position", "10"))
                );

            return 1;
        }
    }
}