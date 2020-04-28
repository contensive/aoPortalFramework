
using Contensive.BaseClasses;

namespace Contensive.Addons.aoPortal.Models {
    public class ContentModel : baseModel {
        //
        //====================================================================================================
        //-- const
        //------ set content name
        public const string contentName = "content";
        //------ set to tablename for the primary content (used for cache names)
        public const string contentTableName = "ccContent";
        //
        //====================================================================================================
        // -- instance properties
        public bool developerOnly { get; set; }
        //
        //====================================================================================================
        public static ContentModel create(CPBaseClass cp, int recordId) {
            return create<ContentModel>(cp, recordId);
        }
    }
}
