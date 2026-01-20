using System.Collections.Generic;
using CommonFrameworks.Utilities;
using UnityEngine;

namespace UI {
    public sealed class UiBook : Singleton<UiBook> {
        private LinkedList<IPage> ActivePages { get; } = new LinkedList<IPage>();
        public IPage? TopmostPage => this.ActivePages.Last?.Value;
        
        public void Open<P>(P page) where P : class, IPage {
            this.ActivePages.AddLast(page);
            page.Open();
        }

        public void PreviousPage() {
            this.TopmostPage?.Close();
            this.ActivePages.RemoveLast();
        }

        public void CloseAllPages() {
            while (this.ActivePages.Count > 0) {
                this.PreviousPage();
            }
        }
    }
}
