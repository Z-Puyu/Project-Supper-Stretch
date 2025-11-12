using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.CommonUtilities.Databases;
using SaintsField;

namespace GameplayKeywordsSystem.Runtime {
    public static class KeywordUtils {
        public static IEnumerable<Keyword> GetAll() {
            List<Keyword> keywords = Database<KeywordSheet>.LoadedResources
                                                           .SelectMany(sheet => sheet)
                                                           .ToList();
            keywords.Sort();
            return keywords;
        }

        public static AdvancedDropdownList<string> GetDropdownList() {
            IEnumerable<AdvancedDropdownList<string>> lists =
                    Database<KeywordSheet>.LoadedResources
                                          .SelectMany(sheet => sheet.ToDropdownLists())
                                          .OrderBy(list => list.displayName);
            return new AdvancedDropdownList<string>("Keywords", lists);
        }
    }
}
