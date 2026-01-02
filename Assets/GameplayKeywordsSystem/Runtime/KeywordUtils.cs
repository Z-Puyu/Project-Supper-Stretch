using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Extensions;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    public static class KeywordUtils {
        public static AdvancedDropdownList<string> GetTreeDropdownList(bool includeInternalNodes = false) {
            IEnumerable<AdvancedDropdownList<string>> lists =
                    Database<KeywordSheet>.LoadedResources
                                          .SelectMany(sheet => sheet.ToAdvancedDropdownLists(includeInternalNodes))
                                          .OrderBy(list => list.displayName);
            return new AdvancedDropdownList<string>("Keywords", lists);
        }

        public static DropdownList<string> GetDropdownList() {
            return new DropdownList<string>(
                Database<KeywordSheet>.LoadedResources.SelectMany(sheet => sheet.Collate()).OrderBy(pair => pair.path)
            );
        }

        public static bool HasKeyword(this GameObject obj, Keyword keyword) {
            return obj.TryGetComponentInChildren(out KeywordContainer container) && container.Contains(keyword);
        }

        public static bool HasKeywordOnGameObject(this Component comp, Keyword keyword) {
            return comp.gameObject.HasKeyword(keyword);
        }
    }
}