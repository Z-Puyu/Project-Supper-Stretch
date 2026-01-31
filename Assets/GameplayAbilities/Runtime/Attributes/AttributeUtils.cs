using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Utilities;
using SaintsField;

namespace GameplayAbilities.Attributes {
    public static class AttributeUtils {
        public static AdvancedDropdownList<string> GetLeafAttributes() {
            IEnumerable<AdvancedDropdownList<string>> lists = 
                    Database<AttributeDefinitionSheet>.LoadedResources
                                                      .SelectMany(type => type.GetKeyDropdownLists())
                                                      .OrderBy(node => node.displayName);
            return new AdvancedDropdownList<string>("Attributes", lists);
        }
    }
}