using DunGen.Project.External.DunGen.Code.DungeonFlowGraph;

namespace DunGen.Editor.Project.External.DunGen.Code.Editor.Validation
{
	public interface IValidationRule
	{
		void Validate(DungeonFlow flow, DungeonValidator validator);
	}
}
