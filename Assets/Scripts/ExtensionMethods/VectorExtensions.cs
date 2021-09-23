using QueueConnect.Development;
using UnityEngine;

namespace QueueConnect.ExtensionMethods
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Get a copy of this vector with the given X value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_X">This Vectors X value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed value</returns>
        public static Vector2 WithX(this Vector2 _Vector, float _X, bool _Add = false)
        {
            _Vector.x = _Add == false ? _X : _Vector.x + _X;
            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Y value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Y">This Vectors Y value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed value</returns>
        public static Vector2 WithY(this Vector2 _Vector, float _Y, bool _Add = false)
        {
            _Vector.y = _Add == false ? _Y : _Vector.y + _Y;
            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X and Y value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_X">This Vectors X value</param>
        /// <param name="_Y">This Vectors Y value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed values</returns>
        public static Vector2 WithXY(this Vector2 _Vector, float _X, float _Y, bool _Add = false)
        {
            _Vector.x = _Add == false ? _X : _Vector.x + _X;
            _Vector.y = _Add == false ? _Y : _Vector.y + _Y;
            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_X">This Vectors X-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed X-Rotation</returns>
        public static Vector2 WithRotX(this Vector2 _Vector, Transform _Transform, float _X, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(MathFx.UnsignedAngle(_Add == false ? _X : _rotation.x + _X), _rotation.y, _rotation.z);
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Y rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_Y">This Vectors Y-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed Y-Rotation</returns>
        public static Vector2 WithRotY(this Vector2 _Vector, Transform _Transform, float _Y, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(_rotation.x, MathFx.UnsignedAngle(_Add == false ? _Y : _rotation.y + _Y), _rotation.z);
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X and Y rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_X">This Vectors X-Rotation</param>
        /// <param name="_Y">This Vectors Y-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed X and Y-Rotation</returns>
        public static Vector2 WithRotXY(this Vector2 _Vector, Transform _Transform, float _X, float _Y, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(MathFx.UnsignedAngle(_Add == false ? _X : _rotation.x + _X), MathFx.UnsignedAngle(_Add == false ? _Y : _rotation.y + _Y), _rotation.z);
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_X">This Vectors X value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed value</returns>
        public static Vector3 WithX(this Vector3 _Vector, float _X, bool _Add = false)
        {
            _Vector.x = _Add == false ? _X : _Vector.x + _X;
            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Y value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Y">This Vectors Y value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed value</returns>
        public static Vector3 WithY(this Vector3 _Vector, float _Y, bool _Add = false)
        {
            _Vector.y = _Add == false ? _Y : _Vector.y + _Y;
            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Z value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Z">This Vectors Z value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed value</returns>
        public static Vector3 WithZ(this Vector3 _Vector, float _Z, bool _Add = false)
        {
            _Vector.z = _Add == false ? _Z : _Vector.z + _Z;
            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X and Y value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_X">This Vectors X value</param>
        /// <param name="_Y">This Vectors Y value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed values</returns>
        public static Vector3 WithXY(this Vector3 _Vector, float _X, float _Y, bool _Add = false)
        {
            _Vector.x = _Add == false ? _X : _Vector.x + _X;
            _Vector.y = _Add == false ? _Y : _Vector.y + _Y;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X and Z value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_X">This Vectors X value</param>
        /// <param name="_Z">This Vectors Z value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed values</returns>
        public static Vector3 WithXZ(this Vector3 _Vector, float _X, float _Z, bool _Add = false)
        {
            _Vector.x = _Add == false ? _X : _Vector.x + _X;
            _Vector.z = _Add == false ? _Z : _Vector.z + _Z;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Y and Z value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Y">This Vectors Y value</param>
        /// <param name="_Z">This Vectors Z value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed values</returns>
        public static Vector3 WithYZ(this Vector3 _Vector, float _Y, float _Z, bool _Add = false)
        {
            _Vector.y = _Add == false ? _Y : _Vector.y + _Y;
            _Vector.z = _Add == false ? _Z : _Vector.z + _Z;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X, Y and Z value
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_X">This Vectors X value</param>
        /// <param name="_Y">This Vectors Y value</param>
        /// <param name="_Z">This Vectors Z value</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed values</returns>
        public static Vector3 WithXYZ(this Vector3 _Vector, float _X, float _Y, float _Z, bool _Add = false)
        {
            _Vector.x = _Add == false ? _X : _Vector.x + _X;
            _Vector.y = _Add == false ? _Y : _Vector.y + _Y;
            _Vector.z = _Add == false ? _Z : _Vector.z + _Z;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_X">This Vectors X-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed X-Rotation</returns>
        public static Vector3 WithRotX(this Vector3 _Vector, Transform _Transform, float _X, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(MathFx.UnsignedAngle(_Add == false ? _X : _rotation.x + _X), _rotation.y, _rotation.z);
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Y rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_Y">This Vectors Y-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed Y-Rotation</returns>
        public static Vector3 WithRotY(this Vector3 _Vector, Transform _Transform, float _Y, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(_rotation.x, MathFx.UnsignedAngle(_Add == false ? _Y : _rotation.y + _Y), _rotation.z);
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Z rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_Z">This Vectors Z-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed Z-Rotation</returns>
        public static Vector3 WithRotZ(this Vector3 _Vector, Transform _Transform, float _Z, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(_rotation.x, _rotation.y, MathFx.UnsignedAngle(_Add == false ? _Z : _rotation.z + _Z));
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X and Y rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_X">This Vectors X-Rotation</param>
        /// <param name="_Y">This Vectors Y-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed X and Y-Rotation</returns>
        public static Vector3 WithRotXY(this Vector3 _Vector, Transform _Transform, float _X, float _Y, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(MathFx.UnsignedAngle(_Add == false ? _X : _rotation.x + _X), MathFx.UnsignedAngle(_Add == false ? _Y : _rotation.y + _Y), _rotation.z);
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X and Z rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_X">This Vectors X-Rotation</param>
        /// <param name="_Z">This Vectors Z-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed X and Z-Rotation</returns>
        public static Vector3 WithRotXZ(this Vector3 _Vector, Transform _Transform, float _X, float _Z, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(MathFx.UnsignedAngle(_Add == false ? _X : _rotation.x + _X), _rotation.y, MathFx.UnsignedAngle(_Add == false ? _Z : _rotation.z + _Z));
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given Y and Z rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_Y">This Vectors Y-Rotation</param>
        /// <param name="_Z">This Vectors Z-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed Y and Z-Rotation</returns>
        public static Vector3 WithRotYZ(this Vector3 _Vector, Transform _Transform, float _Y, float _Z, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(_rotation.x, MathFx.UnsignedAngle(_Add == false ? _Y : _rotation.y + _Y), MathFx.UnsignedAngle(_Add == false ? _Z : _rotation.z + _Z));
            _Transform.rotation = _rotation;

            return _Vector;
        }

        /// <summary>
        /// Get a copy of this vector with the given X, Y and Z rotation
        /// </summary>
        /// <param name="_Vector">This Vector</param>
        /// <param name="_Transform">The vectors Transform component</param>
        /// <param name="_X">This Vectors X-Rotation</param>
        /// <param name="_Y">This Vectors Y-Rotation</param>
        /// <param name="_Z">This Vectors Z-Rotation</param>
        /// <param name="_Add">If "true", adds the passed value to the Vectors existing value instead of overwriting it</param>
        /// <returns>Returns this Vector with the passed X, Y and Z-Rotation</returns>
        public static Vector3 WithRotXYZ(this Vector3 _Vector, Transform _Transform, float _X, float _Y, float _Z, bool _Add = false)
        {
            var _rotation = _Transform.rotation;
            
            _rotation = Quaternion.Euler(MathFx.UnsignedAngle(_Add == false ? _X : _rotation.x + _X), MathFx.UnsignedAngle(_Add == false ? _Y : _rotation.y + _Y), MathFx.UnsignedAngle(_Add == false ? _Z : _rotation.z + _Z));
            _Transform.rotation = _rotation;

            return _Vector;
        }
    }
}