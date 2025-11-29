using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace CommonFrameworks.Extensions {
    public static class PlayerLoopExtensions {
        public static bool IsSameSystem(this ref PlayerLoopSystem a, in PlayerLoopSystem b) {
            return a.type == b.type && a.updateDelegate == b.updateDelegate;
        }
        
        public static bool InsertSubsystem<T>(
            this ref PlayerLoopSystem root, in PlayerLoopSystem subsystem, int index = int.MaxValue
        ) {
            if (!root.HasLoop<T>(out PlayerLoopSystem loop)) {
                return false;
            }

            if (loop.subSystemList is null) {
                loop.subSystemList = new[] { subsystem };
            } else {
                List<PlayerLoopSystem> list = new List<PlayerLoopSystem>(loop.subSystemList);
                int pos = Math.Clamp(index, 0, loop.subSystemList.Length - 1);
                list.Insert(pos, subsystem);
                loop.subSystemList = list.ToArray();
            }

            return true;
        }

        public static bool RemoveSubsystem<T>(this ref PlayerLoopSystem root, in PlayerLoopSystem subsystem) {
            if (!root.HasLoop<T>(out PlayerLoopSystem loop) || loop.subSystemList is null) {
                return false;
            }

            for (int i = 0; i < loop.subSystemList.Length; i += 1) {
                if (!loop.subSystemList[i].IsSameSystem(subsystem)) {
                    continue;
                }
                
                loop.subSystemList[i] = default;
                for (int j = i + 1; j < loop.subSystemList.Length; j += 1) {
                    loop.subSystemList[j - 1] = loop.subSystemList[j];
                }
                
                Array.Resize(ref loop.subSystemList, loop.subSystemList.Length - 1);
                return true;
            }
            
            return false;
        }

        public static bool HasLoop<T>(this ref PlayerLoopSystem root, out PlayerLoopSystem loop) {
            Stack<PlayerLoopSystem> stack = new Stack<PlayerLoopSystem>();
            stack.Push(root);
            Type target = typeof(T);
            while (stack.TryPop(out PlayerLoopSystem curr)) {
                if (curr.type == target) {
                    loop = curr;
                    return true;
                }

                if (curr.subSystemList is null) {
                    continue;
                }

                foreach (PlayerLoopSystem child in curr.subSystemList) {
                    stack.Push(child);
                }
            }
            
            loop = default;
            return false;
        }
    }
}
