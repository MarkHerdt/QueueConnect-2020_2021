using UnityEngine;

namespace Ganymed.Utils.ExtensionMethods
{
    public static class VectorExtensions
    {
        #region --- [VECTOR2] ---

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static float RandomValue(this Vector2 vector)
        {
            return UnityEngine.Random.Range(vector.x, vector.y);
        }
        
        public static float RandomValue(this Vector2Int vector)
        {
            return UnityEngine.Random.Range(vector.x, vector.y);
        }

        #endregion
    
        //--------------------------------------------------------------------------------------------------------------
    
        #region --- [VECTOR3] ---

        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
    
        #region --- [VECTOR4] ---

        public static Vector4 Abs(this Vector4 vector)
        {
            return new Vector4(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
        }

        #endregion

    }
}
