using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    public static class KeywordUtils {
        public static AdvancedDropdownList<string> GetDropdownList() {
            IEnumerable<AdvancedDropdownList<string>> lists =
                    Database<KeywordSheet>.LoadedResources
                                          .SelectMany(sheet => sheet.ToDropdownLists())
                                          .OrderBy(list => list.displayName);
            return new AdvancedDropdownList<string>("Keywords", lists);
        }

        public static bool HasKeyword(this GameObject obj, Keyword keyword) {
            return obj.TryGetComponentInChildren(out KeywordContainer container) && container.Contains(keyword);
        }

        public static bool HasKeywordOnGameObject(this Component comp, Keyword keyword) {
            return comp.gameObject.HasKeyword(keyword);
        }
    }
}