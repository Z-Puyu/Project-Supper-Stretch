using UnityEngine;

namespace Project.External.FastScriptReload.Samples.Basic.Scripts
{
    public class FunctionLibrary: MonoBehaviour
    {
        public delegate Vector3 Function(float u, float v, float t);

        public enum FunctionName
        {
            Wave,
            MultiWave,
            Ripple,
            Sphere,
            Torus
        }

        private static readonly Function[] functions = { FunctionLibrary.Wave, FunctionLibrary.MultiWave, FunctionLibrary.Ripple, FunctionLibrary.Sphere, FunctionLibrary.Torus };

        public static Function GetFunction(FunctionName name)
        {
            return FunctionLibrary.functions[(int)name];
        }

        public static Vector3 Wave(float u, float v, float t)
        {
            Vector3 p;
            p.x = u;
            p.y = Mathf.Sin(Mathf.PI * (u + v + t));
            p.z = v;
            return p;
        }

        public static Vector3 MultiWave(float u, float v, float t)
        {
            Vector3 p;
            p.x = u;
            p.y = Mathf.Sin(Mathf.PI * (u + 0.5f * t));
            p.y += 0.5f * Mathf.Sin(2f * Mathf.PI * (v + t));
            p.y += Mathf.Sin(Mathf.PI * (u + v + 0.25f * t));
            p.y *= 1f / 2.5f;
            p.z = v;
            return p;
        }

        public static Vector3 Ripple(float u, float v, float t)
        {
            var d = Mathf.Sqrt(u * u + v * v);
            Vector3 p;
            p.x = u;
            p.y = Mathf.Sin(Mathf.PI * (4f * d - t));
            p.y /= 1f + 10f * d;   
            p.z = v;
            return p;
        }

        public static Vector3 Sphere(float u, float v, float t)
        {
            var r = 0.9f + 0.1f * Mathf.Sin(Mathf.PI * (6f * u + 4f * v + t));
            var s = r * Mathf.Cos(0.5f * Mathf.PI * v);
            Vector3 p;
            p.x = s * Mathf.Sin(Mathf.PI * u);
            p.y = r * Mathf.Sin(0.5f * Mathf.PI * v);
            p.z = s * Mathf.Cos(Mathf.PI * u) * 10;
            return p;
        }

        public static Vector3 Torus(float u, float v, float t)
        {
            var r1 = 0.7f + 0.1f * Mathf.Sin(Mathf.PI * (6f * u + 0.5f * t));
            var r2 = 0.15f + 0.05f * Mathf.Sin(Mathf.PI * (8f * u + 4f * v + 2f * t));
            var s = r1 + r2 * Mathf.Cos(Mathf.PI * v);
            Vector3 p;
            p.x = s * Mathf.Sin(Mathf.PI * u);
            p.y = r2 * Mathf.Sin(Mathf.PI * v);
            p.z = s * Mathf.Cos(Mathf.PI * u);
            return p;
        }
    }
}