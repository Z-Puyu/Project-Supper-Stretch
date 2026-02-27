// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using System.Text;
// using CommonFrameworks.Utilities;
// using GameplayAbilities.Abilities;
// using GameplayKeywords;
// using SaintsField;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace GameplayAbilities.Debugger {
//     [DisallowMultipleComponent, RequireComponent(typeof(UIDocument))]
//     internal sealed class AbilitySystemDebugger : Singleton<AbilitySystemDebugger> {
//         [field: SerializeField, Required] private AbilitySystem? ObservedSystem { get; set; }
//         
//         [NotNull] private UIDocument? UiDocument { get; set; }
//         [NotNull] private Label? SystemNameLabel { get; set; }
//         [NotNull] private Label? ListOfActiveAbilities { get; set; }
//         [NotNull] private Label? ListOfUsableAbilities { get; set; }
//         [NotNull] private Label? ListOfKeywords { get; set; }
//
//         protected override void Awake() {
//             base.Awake();
//             this.UiDocument = this.GetComponent<UIDocument>();
//             VisualElement root = this.UiDocument.rootVisualElement;
//             this.SystemNameLabel = root.Q<Label>("SystemNameLabel");
//             this.ListOfActiveAbilities = root.Q<Label>("ListOfActiveAbilities");
//             this.ListOfUsableAbilities = root.Q<Label>("ListOfUsableAbilities");
//             this.ListOfKeywords = root.Q<Label>("ListOfKeywords");
//         }
//
//         protected override void Start() {
//             base.Start();
//             if (!this.ObservedSystem) {
//                 this.ObservedSystem = Object.FindAnyObjectByType<AbilitySystem>();
//             }
//
//             if (!this.ObservedSystem) {
//                 return;
//             }
//
//             this.SystemNameLabel.text = this.ObservedSystem.Owner.name;
//             this.ListOfActiveAbilities.text = string.Empty;
//             this.ListOfUsableAbilities.text = string.Empty;
//             this.ListOfKeywords.text = string.Empty;
//             
//             KeywordContainer container = this.ObservedSystem.Root.GetOrAdd<KeywordContainer>();
//             this.ObservedSystem.OnAbilityStarted += this.OnAbilitySystemChanged;
//             this.ObservedSystem.OnAbilityStopped += this.OnAbilitySystemChanged;
//             this.ObservedSystem.OnAbilityRevoked += this.OnAbilitySystemChanged;
//             this.ObservedSystem.OnAbilityGranted += this.OnAbilitySystemChanged;
//             
//             StringBuilder active = new StringBuilder();
//             StringBuilder usable = new StringBuilder();
//             foreach (Ability ability in this.ObservedSystem) {
//                 if (this.ObservedSystem.IsRunningAbility(ability)) {
//                     active.AppendLine(ability.name);
//                 } else {
//                     usable.AppendLine(ability.name);
//                 }
//             }
//             
//             this.ListOfActiveAbilities.text = active.ToString();
//             this.ListOfUsableAbilities.text = usable.ToString();
//             
//             StringBuilder keywords = new StringBuilder();
//             foreach (Keyword keyword in this.ObservedSystem.Root.GetOrAdd<KeywordContainer>().OrderBy(k => k)) {
//                 keywords.AppendLine(keyword);
//             }
//             
//             this.ListOfKeywords.text = keywords.ToString();
//         }
//         
//         private void OnAbilitySystemChanged(Ability _) {
//             if (!this.ObservedSystem) {
//                 return;
//             }
//             
//             StringBuilder active = new StringBuilder();
//             StringBuilder usable = new StringBuilder();
//             foreach (Ability ability in this.ObservedSystem) {
//                 if (this.ObservedSystem.IsRunningAbility(ability)) {
//                     active.AppendLine(ability.name);
//                 } else {
//                     usable.AppendLine(ability.name);
//                 }
//             }
//             
//             this.ListOfActiveAbilities.text = active.ToString();
//             this.ListOfUsableAbilities.text = usable.ToString();
//         }
//
//         private void OnKeywordsChanged(Keyword _) {
//             if (!this.ObservedSystem) {
//                 return;
//             }
//             
//             StringBuilder keywords = new StringBuilder();
//             foreach (Keyword keyword in this.ObservedSystem.Root.GetOrAdd<KeywordContainer>().OrderBy(k => k)) {
//                 keywords.AppendLine(keyword);
//             }
//             
//             this.ListOfKeywords.text = keywords.ToString();
//         }
//     }
// }
