using UnityEngine;

namespace QueueConnect.Development
{
    /// <summary>
    /// Useful Math functions
    /// </summary>
    public static class MathFx
    {
        /// <summary>
        /// Short for 'boing-like interpolation', this method will first overshoot, then waver back and forth around the end value before coming to a rest
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float BErp(float _Start, float _End, float _InterpolationValue)
        {
            _InterpolationValue = Mathf.Clamp01(_InterpolationValue);
            _InterpolationValue = (Mathf.Sin(_InterpolationValue * Mathf.PI * (0.2f + 2.5f * _InterpolationValue * _InterpolationValue * _InterpolationValue)) * Mathf.Pow(1f - _InterpolationValue, 2.2f) + _InterpolationValue) * (1f + (1.2f * (1f - _InterpolationValue)));
            return _Start + (_End - _Start) * _InterpolationValue;
        }

        /// <summary>
        /// Short for 'boing-like interpolation', this method will first overshoot, then waver back and forth around the end value before coming to a rest
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector2 BErp(Vector2 _Start, Vector2 _End, float _InterpolationValue)
        {
            return new Vector2(BErp(_Start.x, _End.x, _InterpolationValue), BErp(_Start.y, _End.y, _InterpolationValue));
        }

        /// <summary>
        /// Short for 'boing-like interpolation', this method will first overshoot, then waver back and forth around the end value before coming to a rest
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector3 BErp(Vector3 _Start, Vector3 _End, float _InterpolationValue)
        {
            return new Vector3(BErp(_Start.x, _End.x, _InterpolationValue), BErp(_Start.y, _End.y, _InterpolationValue), BErp(_Start.z, _End.z, _InterpolationValue));
        }

        /// <summary>
        /// Returns a value between 0 and 1 that can be used to easily make bouncing GUI items (a la OS X's Dock)
        /// </summary>
        /// <param name="_Value">Value to interpolate</param>
        /// <returns></returns>
        public static float Bounce(float _Value)
        {
            return Mathf.Abs(Mathf.Sin(6.28f * (_Value + 1f) * (_Value + 1f)) * (1f - _Value));
        }

        /// <summary>
        /// Returns a value between 0 and 1 that can be used to easily make bouncing GUI items (a la OS X's Dock)
        /// </summary>
        /// <param name="_Vector">Value to interpolate</param>
        /// <returns></returns>
        public static Vector2 Bounce(Vector2 _Vector)
        {
            return new Vector2(Bounce(_Vector.x), Bounce(_Vector.y));
        }

        /// <summary>
        /// Returns a value between 0 and 1 that can be used to easily make bouncing GUI items (a la OS X's Dock)
        /// </summary>
        /// <param name="_Vector">Value to interpolate</param>
        /// <returns></returns>
        public static Vector3 Bounce(Vector3 _Vector)
        {
            return new Vector3(Bounce(_Vector.x), Bounce(_Vector.y), Bounce(_Vector.z));
        }

        /// <summary>
        /// CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360 <br/>
        /// This is useful when interpolating eulerAngles and the object crosses the 0/360 boundary <br/>
        /// The standard Lerp function causes the object to rotate in the wrong direction and looks stupid Clerp fixes that
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float CLerp(float _Start, float _End, float _InterpolationValue)
        {
            const float _MIN = 0.0f;
            const float _MAX = 360.0f;
            var _half = Mathf.Abs((_MAX - _MIN) / 2.0f);//half the distance between min and max
            float _returnValue;
            float _difference;

            if ((_End - _Start) < -_half)
            {
                _difference = ((_MAX - _Start) + _End) * _InterpolationValue;
                _returnValue = _Start + _difference;
            }
            else if ((_End - _Start) > _half)
            {
                _difference = -((_MAX - _End) + _Start) * _InterpolationValue;
                _returnValue = _Start + _difference;
            }
            else _returnValue = _Start + (_End - _Start) * _InterpolationValue;

            // Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
            return _returnValue;
        }

        /// <summary>
        /// Similar to SinErp, except it eases in, when value is near zero, instead of easing out (and uses cosine instead of sine)
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float CosErp(float _Start, float _End, float _InterpolationValue)
        {
            return Mathf.Lerp(_Start, _End, 1.0f - Mathf.Cos(_InterpolationValue * Mathf.PI * 0.5f));
        }

        /// <summary>
        /// Similar to SinErp, except it eases in, when value is near zero, instead of easing out (and uses cosine instead of sine)
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector2 CosErp(Vector2 _Start, Vector2 _End, float _InterpolationValue)
        {
            return new Vector2(CosErp(_Start.x, _End.x, _InterpolationValue), CosErp(_Start.y, _End.y, _InterpolationValue));
        }

        /// <summary>
        /// Similar to SinErp, except it eases in, when value is near zero, instead of easing out (and uses cosine instead of sine)
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector3 CosErp(Vector3 _Start, Vector3 _End, float _InterpolationValue)
        {
            return new Vector3(CosErp(_Start.x, _End.x, _InterpolationValue), CosErp(_Start.y, _End.y, _InterpolationValue), CosErp(_Start.z, _End.z, _InterpolationValue));
        }

        /// <summary>
        /// Dampening functions, stateless-ly and frame independently interpolate towards a value
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_Smoothing">Must be in range of ("smoothing" > 0, "smoothing" < 1)</param>
        /// <param name="_Speed">Speed of the interpolation</param>
        /// <param name="_SnapEpsilon"></param>
        /// <returns></returns>
        public static float Damp(float _Start, float _End, float _Smoothing, float _Speed, float _SnapEpsilon = 0.01f)
        {
            var _val = Mathf.Lerp(_Start, _End, 1 - Mathf.Pow(_Smoothing, _Speed));
            if (Mathf.Abs(_val - _End) < _SnapEpsilon)
                _val = _End;

            return _val;
        }

        /// <summary>
        /// Dampening functions, stateless-ly and frame independently interpolate towards a value
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_Smoothing">Must be in range of ("smoothing" > 0, "smoothing" < 1)</param>
        /// <param name="_Speed">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector2 Damp(Vector2 _Start, Vector2 _End, float _Smoothing, float _Speed)
        {
            return Vector2.Lerp(_Start, _End, 1 - Mathf.Pow(_Smoothing, _Speed));
        }

        /// <summary>
        /// Dampening functions, stateless-ly and frame independently interpolate towards a value
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_Smoothing">Must be in range of ("smoothing" > 0, "smoothing" < 1)</param>
        /// <param name="_Speed">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector3 Damp(Vector3 _Start, Vector3 _End, float _Smoothing, float _Speed)
        {
            return Vector3.Lerp(_Start, _End, 1 - Mathf.Pow(_Smoothing, _Speed));
        }

        /// <summary>
        /// Dampening functions, stateless-ly and frame independently interpolate towards a value
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_Smoothing">Must be in range of ("smoothing" > 0, "smoothing" < 1)</param>
        /// <param name="_Speed">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector4 Damp(Vector4 _Start, Vector4 _End, float _Smoothing, float _Speed)
        {
            return Vector4.Lerp(_Start, _End, 1 - Mathf.Pow(_Smoothing, _Speed));
        }

        /// <summary>
        /// Dampening functions, stateless-ly and frame independently interpolate towards a value
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_Smoothing">Must be in range of ("smoothing" > 0, "smoothing" < 1)</param>
        /// <param name="_Speed">Speed of the interpolation</param>
        /// <returns></returns>
        public static Quaternion Damp(Quaternion _Start, Quaternion _End, float _Smoothing, float _Speed)
        {
            return Quaternion.Lerp(_Start, _End, 1 - Mathf.Pow(_Smoothing, _Speed));
        }

        /// <summary>
        /// Dampening functions, stateless-ly and frame independently interpolate towards a value
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_Smoothing">Must be in range of ("smoothing" > 0, "smoothing" < 1)</param>
        /// <param name="_Speed">Speed of the interpolation</param>
        /// <returns></returns>
        public static Color Damp(Color _Start, Color _End, float _Smoothing, float _Speed)
        {
            return Color.Lerp(_Start, _End, 1 - Mathf.Pow(_Smoothing, _Speed));
        }

        /// <summary>
        /// This method will interpolate while easing in and out at the limits
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float Hermite(float _Start, float _End, float _InterpolationValue)
        {
            return Mathf.Lerp(_Start, _End, _InterpolationValue * _InterpolationValue * (3.0f - 2.0f * _InterpolationValue));
        }

        /// <summary>
        /// This method will interpolate while easing in and out at the limits
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector2 Hermite(Vector2 _Start, Vector2 _End, float _InterpolationValue)
        {
            return new Vector2(Hermite(_Start.x, _End.x, _InterpolationValue), Hermite(_Start.y, _End.y, _InterpolationValue));
        }

        /// <summary>
        /// This method will interpolate while easing in and out at the limits
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector3 Hermite(Vector3 _Start, Vector3 _End, float _InterpolationValue)
        {
            return new Vector3(Hermite(_Start.x, _End.x, _InterpolationValue), Hermite(_Start.y, _End.y, _InterpolationValue), Hermite(_Start.z, _End.z, _InterpolationValue));
        }

        /// <summary>
        /// Linearly interpolates between a and b by t
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float Lerp(float _Start, float _End, float _InterpolationValue)
        {
            return ((1.0f - _InterpolationValue) * _Start) + (_InterpolationValue * _End);
        }

        /// <summary>
        /// Short for 'sinusoidal interpolation', this method will interpolate while easing around the end, when value is near one
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float SinErp(float _Start, float _End, float _InterpolationValue)
        {
            return Mathf.Lerp(_Start, _End, Mathf.Sin(_InterpolationValue * Mathf.PI * 0.5f));
        }

        /// <summary>
        /// Short for 'sinusoidal interpolation', this method will interpolate while easing around the end, when value is near one
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector2 SinErp(Vector2 _Start, Vector2 _End, float _InterpolationValue)
        {
            return new Vector2(Mathf.Lerp(_Start.x, _End.x, Mathf.Sin(_InterpolationValue * Mathf.PI * 0.5f)), Mathf.Lerp(_Start.y, _End.y, Mathf.Sin(_InterpolationValue * Mathf.PI * 0.5f)));
        }

        /// <summary>
        /// Short for 'sinusoidal interpolation', this method will interpolate while easing around the end, when value is near one
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static Vector3 SinErp(Vector3 _Start, Vector3 _End, float _InterpolationValue)
        {
            return new Vector3(Mathf.Lerp(_Start.x, _End.x, Mathf.Sin(_InterpolationValue * Mathf.PI * 0.5f)), Mathf.Lerp(_Start.y, _End.y, Mathf.Sin(_InterpolationValue * Mathf.PI * 0.5f)), Mathf.Lerp(_Start.z, _End.z, Mathf.Sin(_InterpolationValue * Mathf.PI * 0.5f)));
        }

        /// <summary>
        /// Interpolates between the two points
        /// </summary>
        /// <param name="_Start">Value to interpolate</param>
        /// <param name="_End">Targeted value</param>
        /// <param name="_InterpolationValue">Speed of the interpolation</param>
        /// <returns></returns>
        public static float SmoothSin(float _Start, float _End, float _InterpolationValue)
        {
            return Mathf.Lerp(_Start, _End, Mathf.Sin(_InterpolationValue * 2 * Mathf.PI + Mathf.PI * 0.5f));
        }

        /// <summary>
        /// Works like Lerp, but has ease-in and ease-out of the values
        /// </summary>
        /// <param name="_Value"></param>
        /// <param name="_MIN"></param>
        /// <param name="_MAX"></param>
        /// <returns></returns>
        public static float SmoothStep(float _Value, float _MIN, float _MAX)
        {
            _Value = Mathf.Clamp(_Value, _MIN, _MAX);
            var _v1 = (_Value - _MIN) / (_MAX - _MIN);
            var _v2 = (_Value - _MIN) / (_MAX - _MIN);
            return -2 * _v1 * _v1 * _v1 + 3 * _v2 * _v2;
        }

        /// <summary>
        /// Works like Lerp, but has ease-in and ease-out of the values
        /// </summary>
        /// <param name="_Vector"></param>
        /// <param name="_MIN"></param>
        /// <param name="_MAX"></param>
        /// <returns></returns>
        public static Vector2 SmoothStep(Vector2 _Vector, float _MIN, float _MAX)
        {
            return new Vector2(SmoothStep(_Vector.x, _MIN, _MAX), SmoothStep(_Vector.y, _MIN, _MAX));
        }

        /// <summary>
        /// Works like Lerp, but has ease-in and ease-out of the values
        /// </summary>
        /// <param name="_Vector"></param>
        /// <param name="_MIN"></param>
        /// <param name="_MAX"></param>
        /// <returns></returns>
        public static Vector3 SmoothStep(Vector3 _Vector, float _MIN, float _MAX)
        {
            return new Vector3(SmoothStep(_Vector.x, _MIN, _MAX), SmoothStep(_Vector.y, _MIN, _MAX), SmoothStep(_Vector.z, _MIN, _MAX));
        }

        /// <summary>
        /// Return -1, 0 or 1
        /// </summary>
        /// <param name="_Value"></param>
        /// <returns></returns>
        public static float CeilNormal(float _Value)
        {
            if (_Value > 0)
                return 1.0f;
            else if (_Value < 0)
                return -1.0f;
            return 0.0f;
        }

        /// <summary>
        /// Returns the signed value of an unsigned angle <br/>
        /// (0)-(+360) -> (-180)-(+180)
        /// </summary>
        /// <param name="_Angle">Value to convert</param>
        /// <returns></returns>
        public static float SignedAngle(float _Angle)
        {
            _Angle %= 360;

            if (_Angle > 180)
            {
                return _Angle - 360;
            }

            return _Angle;
        }

        /// <summary>
        /// Returns the unsigned value of a signed angle <br/>
        /// (-180)-(+180) -> (0)-(+360)
        /// </summary>
        /// <param name="_Angle">Value to convert</param>
        /// <returns></returns>
        public static float UnsignedAngle(float _Angle)
        {
            if (_Angle >= 0)
            {
                return _Angle;
            }

            _Angle = -_Angle % 360;

            return 360 - _Angle;
        }

        /// <summary>
        /// Test for value that is near specified float (due to floating point inprecision)
        /// </summary>
        /// <param name="_Value"></param>
        /// <param name="_About"></param>
        /// <param name="_Range"></param>
        /// <returns></returns>
        public static bool Approx(float _Value, float _About, float _Range)
        {
            return ((Mathf.Abs(_Value - _About) < _Range));
        }

        /// <summary>
        /// Test if a Vector3 is close to another Vector3 (due to floating point inprecision) <br/>
        /// Compares the square of the distance to the square of the range as this avoids calculating a square root which is much slower than squaring the range
        /// </summary>
        /// <param name="_Value"></param>
        /// <param name="_About"></param>
        /// <param name="_Range"></param>
        /// <returns></returns>
        public static bool Approx(Vector3 _Value, Vector3 _About, float _Range)
        {
            return ((_Value - _About).sqrMagnitude < _Range * _Range);
        }

        /// <summary>
        /// Will return the nearest point on a line to a point. Useful for making an object follow a track
        /// </summary>
        /// <param name="_LineStart"></param>
        /// <param name="_LineEnd"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static Vector3 NearestPointLine(Vector3 _LineStart, Vector3 _LineEnd, Vector3 _Point)
        {
            var _lineDirection = Vector3.Normalize(_LineEnd - _LineStart);
            var _closestPoint = Vector3.Dot((_Point - _LineStart), _lineDirection) / Vector3.Dot(_lineDirection, _lineDirection);
            return _LineStart + (_closestPoint * _lineDirection);
        }

        /// <summary>
        /// Nearest point on a line, clamped to the endpoints of the line segment
        /// </summary>
        /// <param name="_LineStart"></param>
        /// <param name="_LineEnd"></param>
        /// <param name="_Point"></param>
        /// <returns></returns>
        public static Vector3 NearestPointClamped(Vector3 _LineStart, Vector3 _LineEnd, Vector3 _Point)
        {
            var _fullDirection = _LineEnd - _LineStart;
            var _lineDirection = Vector3.Normalize(_fullDirection);
            var _closestPoint = Vector3.Dot((_Point - _LineStart), _lineDirection) / Vector3.Dot(_lineDirection, _lineDirection);
            return _LineStart + (Mathf.Clamp(_closestPoint, 0.0f, Vector3.Magnitude(_fullDirection)) * _lineDirection);
        }

        /// <summary>
        /// Project a point onto a plane, not necessarily at origin <br/>
        /// Vector3.ProjectOnPlane assumes origin for the plane
        /// </summary>
        /// <param name="_Point"></param>
        /// <param name="_Plane"></param>
        /// <returns></returns>
        public static Vector3 ProjectPointOnPlane(Vector3 _Point, Plane _Plane)
        {
            return Vector3.ProjectOnPlane(_Point, _Plane.normal) + _Plane.normal * _Plane.distance;
        }

        /// <summary>
        /// Converts a point on the Canvas to World coordinates
        /// </summary>
        /// <param name="_Canvas"></param>
        /// <param name="_Camera"></param>
        /// <param name="_CanvasLocalPos"></param>
        /// <returns></returns>
        public static Vector3 CanvasToWorldPosition(RectTransform _Canvas, Camera _Camera, Vector2 _CanvasLocalPos)
        {
            var _viewportPoint = _CanvasLocalPos + _Canvas.rect.size * _Canvas.pivot;
            _viewportPoint /= _Canvas.rect.size;
            var _worldPos = _Camera.ViewportToWorldPoint(_viewportPoint, Camera.MonoOrStereoscopicEye.Mono);
            return _worldPos;
        }

        /// <summary>
        /// Converts a point on the Viewport to Canvas coordinates
        /// </summary>
        /// <param name="_Canvas"></param>
        /// <param name="_ViewportPos"></param>
        /// <returns></returns>
        public static Vector3 ViewportToCanvasPosition(RectTransform _Canvas, Vector3 _ViewportPos)
        {
            var _rect = _Canvas.rect;
            
            _ViewportPos.x *= _rect.size.x;
            _ViewportPos.y *= _rect.size.y;
            _ViewportPos.x -= _rect.size.x * _Canvas.pivot.x;
            _ViewportPos.y -= _Canvas.rect.size.y * _Canvas.pivot.y;
            return _ViewportPos;
        }

        /// <summary>
        /// Converts a point in the World to Canvas coordinates
        /// </summary>
        /// <param name="_Canvas"></param>
        /// <param name="_Camera"></param>
        /// <param name="_WorldPos"></param>
        /// <returns></returns>
        public static Vector3 WorldToCanvasPosition(RectTransform _Canvas, Camera _Camera, Vector3 _WorldPos)
        {
            var _pos = _Camera.WorldToViewportPoint(_WorldPos);
            _pos = ViewportToCanvasPosition(_Canvas, _pos);
            return _pos;
        }

        public static float DecibelToLinear(float _Decibel)
        {
            var _factor = _Decibel * Mathf.Pow(10.0f, _Decibel / 20.0f);

            return _factor;
        }

        public static float LinearToDecibel(float _Linear)
        {
            float _decibel;

            if (_Linear != 0)
                _decibel = 20.0f * Mathf.Log10(_Linear);
            else
                _decibel = -144.0f;

            return _decibel;
        }

        public static Bounds NegativeBounds()
        {
            var _negativeBounds = new Bounds();
            _negativeBounds.min = Vector3.positiveInfinity;
            _negativeBounds.max = Vector3.negativeInfinity;
            return _negativeBounds;
        }
    }
}