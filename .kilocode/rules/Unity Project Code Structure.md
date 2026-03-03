# Unity Project Code Structure.md

This file defines rules for the Unity project code structure.

## Guidelines

- Always divide code into assemblies categorised by game feature or system.
- Logic code should never be dependent on UI code.
- Avoid dependencies between game system assemblies unless absolutely necessary.
- Create a dedicated assembly containing scripts that handle interactions between game systems from different assemblies.
- Use UI Toolkit for both runtime UI and editor scripting. Do not use the old Unity UI or UGUI.
- Assume that `SerializeReference` has support for custom editor tooling to create instances of concrete subtypes.
