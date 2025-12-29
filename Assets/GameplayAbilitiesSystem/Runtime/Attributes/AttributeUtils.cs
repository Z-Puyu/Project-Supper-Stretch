using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Utilities;
using SaintsField;

namespace GameplayAbilitiesSystem.Runtime.Attributes {
    public class AttributeUtils {
        public static IEnumerable<string> GetAll() {
            List<string> keywords = Database<AttributeType>.LoadedResources
                                                           .Select(type => new string(type.Id)).ToList();
            keywords.Sort();
            return keywords;
        }

        public static AdvancedDropdownList<string> GetDropdownList() {
            IEnumerable<AdvancedDropdownList<string>> lists =
                    Database<AttributeType>.LoadedResources.Select(type => type.ToAdvancedDropdownList());
            return new AdvancedDropdownList<string>("Attributes", lists);
        }
    }
}