using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Utilities;
using SaintsField;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public static class AttributeUtils {
        public static AdvancedDropdownList<AttributeType> GetLeafTypes() {
            AdvancedDropdownList<AttributeType> types = new AdvancedDropdownList<AttributeType>(
                "Attributes",
                Database<AttributeDefinitionSheet>.LoadedResources
                                                  .SelectMany(sheet => sheet.GetDropdownLists())
                                                  .OrderBy(type => type.displayName)
            );
            
            return types;
        }

        public static AdvancedDropdownList<string> GetDropdownList() {
            IEnumerable<AdvancedDropdownList<string>> lists = 
                    Database<AttributeDefinitionSheet>.LoadedResources
                                                      .SelectMany(type => type.GetKeyDropdownLists())
                                                      .OrderBy(node => node.displayName);
            return new AdvancedDropdownList<string>("Attributes", lists);
        }
    }
}