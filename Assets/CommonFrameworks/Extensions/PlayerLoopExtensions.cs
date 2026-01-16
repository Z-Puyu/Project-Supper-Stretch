using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace CommonFrameworks.Extensions {
    public static class PlayerLoopExtensions {
        public static bool IsSameSystem(this ref PlayerLoopSystem a, in PlayerLoopSystem b) {
            return a.type == b.type && a.updateDelegate == b.updateDelegate;
        }
        
        public static bool InsertSubsystem<T>(
            this ref PlayerLoopSystem root, in PlayerLoopSystem subsystem, int index = 0
        ) {
            if (root.type == typeof(T)) {
                if (root.subSystemList is null) {
                    root.subSystemList = new[] { subsystem };
                } else {
                    List<PlayerLoopSystem> list = new List<PlayerLoopSystem>(root.subSystemList);
                    int pos = Math.Clamp(index, 0, list.Count);
                    list.Insert(pos, subsystem);
                    root.subSystemList = list.ToArray();
                }
                
                return true;
            } 
            
            if (root.subSystemList is null) {
                return false;
            }

            for (int i = 0; i < root.subSystemList.Length; i += 1) {
                if (root.subSystemList[i].InsertSubsystem<T>(subsystem, index)) {
                    return true;
                }
            }
            
            return false;
        }

        public static bool RemoveSubsystem<T>(this ref PlayerLoopSystem root, in PlayerLoopSystem subsystem) {
            if (root.type == typeof(T) && root.subSystemList is not null) {
                for (int i = 0; i < root.subSystemList.Length; i += 1) {
                    if (!root.subSystemList[i].IsSameSystem(subsystem)) {
                        continue;
                    }
                
                    root.subSystemList[i] = default;
                    for (int j = i + 1; j < root.subSystemList.Length; j += 1) {
                        root.subSystemList[j - 1] = root.subSystemList[j];
                    }
                
                    Array.Resize(ref root.subSystemList, root.subSystemList.Length - 1);
                    return true;
                }
                
                return false;
            }

            if (root.subSystemList is null) {
                return false;
            }

            for (int i = 0; i < root.subSystemList.Length; i += 1) {
                if (root.subSystemList[i].RemoveSubsystem<T>(subsystem)) {
                    return true;
                }
            }
            
            return false;
        }
    }
}