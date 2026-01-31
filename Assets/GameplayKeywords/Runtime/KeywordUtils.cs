using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayKeywords {
    public static class KeywordUtils {
        public static AdvancedDropdownList<string> Fetch<S>() where S : GeneralKeywordSheet {
            IEnumerable<AdvancedDropdownList<string>> lists = Database<S>.LoadedResources
                                                                         .SelectMany(sheet => sheet.Collate(true))
                                                                         .OrderBy(list => list.displayName);
            return new AdvancedDropdownList<string>("Keywords", lists);
        }

        public static AdvancedDropdownList<string> FetchLeaves<S>() where S : GeneralKeywordSheet {
            IEnumerable<AdvancedDropdownList<string>> lists = Database<S>.LoadedResources
                                                                         .SelectMany(sheet => sheet.Collate())
                                                                         .OrderBy(list => list.displayName);
            return new AdvancedDropdownList<string>("Keywords", lists);
        }

        public static bool HasKeyword(this GameObject obj, Keyword keyword) {
            return obj.TryGetComponentInChildren(out KeywordContainer container) && container.HasTag(keyword);
        }

        public static bool HasKeywordOnGameObject(this Component comp, Keyword keyword) {
            return comp.gameObject.HasKeyword(keyword);
        }
    }
}