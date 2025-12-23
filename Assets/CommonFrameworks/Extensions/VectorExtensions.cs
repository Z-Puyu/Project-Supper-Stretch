using UnityEngine;

namespace CommonFrameworks.Extensions;

public static class VectorExtensions {
    public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null) {
        return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
    }

    public static Vector2 With(this Vector2 v, float? x = null, float? y = null) {
        return new Vector2(x ?? v.x, y ?? v.y);
    }

    public static Vector3Int With(this Vector3Int v, int? x = null, int? y = null, int? z = null) {
        return new Vector3Int(x ?? v.x, y ?? v.y, z ?? v.z);
    }
        
    public static Vector2Int With(this Vector2Int v, int? x = null, int? y = null) {
        return new Vector2Int(x ?? v.x, y ?? v.y);
    }
        
    public static Vector4 With(this Vector4 v, float? x = null, float? y = null, float? z = null, float? w = null) {
        return new Vector4(x ?? v.x, y ?? v.y, z ?? v.z, w ?? v.w);
    }
        
    public static Vector3 Offset(this Vector3 v, float x = 0, float y = 0, float z = 0) {
        return new Vector3(v.x + x, v.y + y, v.z + z);
    }
        
    public static Vector2 Offset(this Vector2 v, float x = 0, float y = 0) {
        return new Vector2(v.x + x, v.y + y);
    }
        
    public static Vector3Int Offset(this Vector3Int v, int x = 0, int y = 0, int z = 0) {
        return new Vector3Int(v.x + x, v.y + y, v.z + z);
    }
        
    public static Vector2Int Offset(this Vector2Int v, int x = 0, int y = 0) {
        return new Vector2Int(v.x + x, v.y + y);
    }
        
    public static Vector4 Offset(this Vector4 v, float x = 0, float y = 0, float z = 0, float w = 0) {
        return new Vector4(v.x + x, v.y + y, v.z + z, v.w + w);
    }
        
    public static Vector3 Scale(this Vector3 v, float x = 1, float y = 1, float z = 1) {
        return new Vector3(v.x * x, v.y * y, v.z * z);
    }
        
    public static Vector2 Scale(this Vector2 v, float x = 1, float y = 1) {
        return new Vector2(v.x * x, v.y * y);
    }
        
    public static Vector3 Scale(this Vector3Int v, float x = 1, float y = 1, float z = 1) {
        return new Vector3(v.x * x, v.y * y, v.z * z);
    }
        
    public static Vector2 Scale(this Vector2Int v, float x = 1, float y = 1) {
        return new Vector2(v.x * x, v.y * y);
    }
        
    public static Vector4 Scale(this Vector4 v, float x = 1, float y = 1, float z = 1, float w = 1) {
        return new Vector4(v.x * x, v.y * y, v.z * z, v.w * w);
    }
}