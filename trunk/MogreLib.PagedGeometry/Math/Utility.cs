﻿#region MIT/X11 License
// This file is part of the MogreLib.PagedGeometry project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// MogreLib.PagedGeometry is a reimplementation of the PagedGeometry project for .Net/Mono
// PagedGeometry is Copyright (C) 2009 Xavier Vergu韓 Gonz醠ez <xavierverguin@hotmail.com> <xavyiy@gmail.com>

// MogreLib.PagedGeometry is Copyright (C) 2013 rains <andyhebear@hotmail.com> <andyhebear@gmail.com>
// MogreLib.PagedGeometry svn: http://mogrelibrarys.googlecode.com/svn/trunk/  Author Blog http://hi.baidu.com/andyhebear/    http://hi.baidu.com/new/rainssoft/

#endregion MIT/X11 License

using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using Intersection = Mogre.Pair<bool, float>;
using IntersectResult = Mogre.Pair<bool, float>;
using System.Diagnostics;

namespace MogreLib.Math {
    public sealed class Utility {
        public static readonly float PI = (float)System.Math.PI;
        public static readonly float TWO_PI = PI * 2.0f;
        public static readonly float HALF_PI = PI * 0.5f;

        public static readonly float RADIANS_PER_DEGREE = PI / 180.0f;
        public static readonly float DEGREES_PER_RADIAN = 180.0f / PI;

        private static Random random = new Random();

        /// <summary>
        /// Empty static constructor
        /// DO NOT DELETE.  It needs to be here because:
        /// 
        ///     # The presence of a static constructor suppresses beforeFieldInit.
        ///     # Static field variables are initialized before the static constructor is called.
        ///     # Having a static constructor is the only way to ensure that all resources are 
        ///       initialized before other static functions are called.
        /// 
        /// (from "Static Constructors Demystified" by Satya Komatineni
        ///  http://www.ondotnet.com/pub/a/dotnet/2003/07/07/staticxtor.html)
        /// </summary>
        static Utility() {
        }

        /// <summary>
        /// 
        /// </summary>
        private Utility() {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aabb"></param>
        /// <returns></returns>
        public static float BoundingRadiusFromAABB(AxisAlignedBox aabb) {
            Vector3 max = aabb.Maximum;
            Vector3 min = aabb.Minimum;

            Vector3 magnitude = max;
            magnitude.MakeCeil(-max);
            magnitude.MakeCeil(min);
            magnitude.MakeCeil(-min);

            return magnitude.Length;
        }

        /// <summary>
        ///		Converts radians to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Degree RadiansToDegrees(Radian radians) {
            return radians;
        }

        /// <summary>
        ///		Converts degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float DegreesToRadians(float  degrees) {
            return degrees * RADIANS_PER_DEGREE;
        }

        /// <summary>
        ///		Converts radians to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float RadiansToDegrees(float  radians) {
            return radians * DEGREES_PER_RADIAN;
        }

        /// <summary>
        ///     Compares float values for equality, taking into consideration
        ///     that floating point values should never be directly compared using
        ///     the '==' operator.  2 floats could be conceptually equal, but vary by a 
        ///     float.Epsilon which would fail in a direct comparison.  To circumvent that,
        ///     a tolerance value is used to see if the difference between the 2 floats
        ///     is less than the desired amount of accuracy.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool RealEqual(float  a, float b, float tolerance) {
            return (System.Math.Abs(b - a) <= tolerance);
        }

        /// <summary>
        ///     Compares float values for equality, taking into consideration
        ///     that floating point values should never be directly compared using
        ///     the '==' operator.  2 floats could be conceptually equal, but vary by a 
        ///     float.Epsilon which would fail in a direct comparison.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool RealEqual(float  a, float b) {
            return (System.Math.Abs(b - a) <= float.Epsilon);
        }

        public static float ParseReal(string value) {
            return float.Parse(value, new System.Globalization.CultureInfo("en-US"));
        }

        /// <summary>
        ///     Returns the sign of a float number.
        /// The result will be -1 for a negative number, 0 for zero and 1 for positive number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int Sign(float  number) {
            return System.Math.Sign(number);
        }

        /// <summary>
        ///	Returns the sine of the specified angle.
        /// </summary>
        public static float Sin(Radian angle) {
            return (float)System.Math.Sin(angle.ValueRadians);
        }

        /// <summary>
        ///	Returns the angle whose cosine is the specified number.
        /// </summary>
        public static Radian ASin(float  angle) {
            return new Radian((float)System.Math.Asin(angle));
        }

        /// <summary>
        ///	Returns the cosine of the specified angle.
        /// </summary>
        public static float Cos(Radian angle) {
            return (float)System.Math.Cos(angle.ValueRadians);
        }

        /// <summary>
        ///	Returns the angle whose cosine is the specified number.
        /// </summary>
        public static Radian ACos(float  angle) {

            // HACK: Ok, this needs to be looked at.  The decimal precision of float values can sometimes be 
            // *slightly* off from what is loaded from .skeleton files.  In some scenarios when we end up having 
            // a cos value calculated above that is just over 1 (i.e. 1.000000012), which the ACos of is Nan, thus 
            // completly throwing off node transformations and rotations associated with an animation.
            if (angle > 1) {
                angle = 1.0f;
            }

            return (Radian)System.Math.Acos(angle);
        }

        /// <summary>
        /// Returns the tangent of the specified angle.
        /// </summary>
        public static float Tan(Radian value) {
            return (float)System.Math.Tan(value.ValueRadians);
        }
        public static float Tan(float value) {
            return (float)System.Math.Tan(value);
        }
        /// <summary>
        /// Return the angle whos tangent is the specified number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Atan(float  value) {
            return (float)System.Math.Atan(value);
        }

        /// <summary>
        /// Returns the angle whose tangent is the quotient of the two specified numbers.
        /// </summary>
        public static float ATan(float  y, float x) {
            return (float)System.Math.Atan2(y, x);
        }

        public static float ATan2(float  y, float x) {
            return (float)System.Math.Atan2(y, x);
        }

        /// <summary>
        ///		Returns the square root of a number.
        /// </summary>
        /// <remarks>This is one of the more expensive math operations.  Avoid when possible.</remarks>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float Sqrt(float  number) {
            return (float)System.Math.Sqrt(number);
        }

        /// <summary>
        ///    Inverse square root.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float InvSqrt(float  number) {
            return 1 / Sqrt(number);
        }

        /// <summary>
        ///     Raise a number to a power.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The number x raised to power y</returns>
        public static float Pow(float  x, float y) {
            return (float)System.Math.Pow(x, y);
        }
        public static float Pow(double x, double y) { 
            return (float)System.Math.Pow(x, y);
        }
        /// <summary>
        ///		Returns the absolute value of the supplied number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float Abs(float  number) {
            return (float)System.Math.Abs(number);
        }

        /// <summary>
        /// Returns the maximum of the two supplied values.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static float Max(float  lhs, float rhs) {
            return lhs > rhs ? lhs : rhs;
        }

        /// <summary>
        /// Returns the maximum of the two supplied values.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static long Max(long lhs, long rhs) {
            return lhs > rhs ? lhs : rhs;
        }

        /// <summary>
        ///     Finds the first maximum value in the array and returns the index of it.
        /// </summary>
        /// <param name="values">Array of values containing one value at least.</param>
        /// <returns></returns>
        public static int Max(float[] values) {
            Debug.Assert(values != null && values.Length > 0);

            int maxIndex = 0;
            float max = values[0];
            for (int i = 1; i < values.Length; i++)
                if (values[i] > max) {
                    max = values[i];
                    maxIndex = i;
                }

            return maxIndex;
        }

        /// <summary>
        /// Returns the minumum of the two supplied values.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static float Min(float  lhs, float rhs) {
            return lhs < rhs ? lhs : rhs;
        }

        /// <summary>
        /// Returns the minumum of the two supplied values.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static long Min(long lhs, long rhs) {
            return lhs < rhs ? lhs : rhs;
        }

        /// <summary>
        ///     Finds the first minimum value in the array and returns the index of it.
        /// </summary>
        /// <param name="values">Array of values containing one value at least.</param>
        /// <returns></returns>
        public static int Min(float[] values) {
            Debug.Assert(values != null && values.Length > 0);

            int minIndex = 0;
            float min = values[0];
            for (int i = 1; i < values.Length; i++)
                if (values[i] < min) {
                    min = values[i];
                    minIndex = i;
                }

            return minIndex;
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to the specified value.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float Ceiling(float  number) {
            return (float)System.Math.Ceiling(number);
        }

        /// <summary>
        ///    Returns a random value between the specified min and max values.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>A random value in the range [min,max].</returns>
        public static float RangeRandom(float  min, float max) {
            return (max - min) * UnitRandom() + min;
        }

        /// <summary>
        ///    
        /// </summary>
        /// <returns></returns>
        public static float UnitRandom() {
            return (float)random.Next(Int32.MaxValue) / (float)Int32.MaxValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static float SymmetricRandom() {
            return (float)(2.0f * UnitRandom() - 1.0f);
        }

        /// <summary>
        ///     Builds a reflection matrix for the specified plane.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Matrix4 BuildReflectionMatrix(Plane plane) {
            Vector3 normal = plane.normal;

            return new Matrix4(
                -2.0f * normal.x * normal.x + 1.0f, -2.0f * normal.x * normal.y, -2.0f * normal.x * normal.z, -2.0f * normal.x * plane.d,
                -2.0f * normal.y * normal.x, -2.0f * normal.y * normal.y + 1.0f, -2.0f * normal.y * normal.z, -2.0f * normal.y * plane.d,
                -2.0f * normal.z * normal.x, -2.0f * normal.z * normal.y, -2.0f * normal.z * normal.z + 1.0f, -2.0f * normal.z * plane.d,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        ///		Calculate a face normal, including the w component which is the offset from the origin.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector4 CalculateFaceNormal(Vector3 v1, Vector3 v2, Vector3 v3) {
            Vector3 normal = CalculateBasicFaceNormal(v1, v2, v3);

            // Now set up the w (distance of tri from origin
            return new Vector4(normal.x, normal.y, normal.z, -(normal.DotProduct(v1)));
        }

        /// <summary>
        ///		Calculate a face normal, no w-information.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 CalculateBasicFaceNormal(Vector3 v1, Vector3 v2, Vector3 v3) {
            Vector3 normal = (v2 - v1).CrossProduct(v3 - v1);
            //normal.Normalize();

            return normal.NormalisedCopy;
        }
        /// <summary>
        ///		Calculate a face normal, no w-information.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 CalculateBasicFaceNormalWithoutNormalize(Vector3 v1, Vector3 v2, Vector3 v3) {
            Vector3 normal = (v2 - v1).CrossProduct(v3 - v1);
            return normal;
        }
        /// <summary>
        ///    Calculates the tangent space vector for a given set of positions / texture coords.
        /// </summary>
        /// <remarks>
        ///    Adapted from bump mapping tutorials at:
        ///    http://www.paulsprojects.net/tutorials/simplebump/simplebump.html
        ///    author : paul.baker@univ.ox.ac.uk
        /// </remarks>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <param name="position3"></param>
        /// <param name="u1"></param>
        /// <param name="v1"></param>
        /// <param name="u2"></param>
        /// <param name="v2"></param>
        /// <param name="u3"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 CalculateTangentSpaceVector(
            Vector3 position1, Vector3 position2, Vector3 position3, float u1, float v1, float u2, float v2, float u3, float v3) {

            // side0 is the vector along one side of the triangle of vertices passed in, 
            // and side1 is the vector along another side. Taking the cross product of these returns the normal.
            Vector3 side0 = position1 - position2;
            Vector3 side1 = position3 - position1;
            // Calculate face normal
            Vector3 normal = side1.CrossProduct(side0);
            //normal.Normalize();
            normal = normal.NormalisedCopy;

            // Now we use a formula to calculate the tangent. 
            float deltaV0 = v1 - v2;
            float deltaV1 = v3 - v1;
            Vector3 tangent = deltaV1 * side0 - deltaV0 * side1;
            //tangent.Normalize();
            tangent = tangent.NormalisedCopy;

            // Calculate binormal
            float deltaU0 = u1 - u2;
            float deltaU1 = u3 - u1;
            Vector3 binormal = deltaU1 * side0 - deltaU0 * side1;
            //binormal.Normalise();
            binormal = binormal.NormalisedCopy;

            // Now, we take the cross product of the tangents to get a vector which 
            // should point in the same direction as our normal calculated above. 
            // If it points in the opposite direction (the dot product between the normals is less than zero), 
            // then we need to reverse the s and t tangents. 
            // This is because the triangle has been mirrored when going from tangent space to object space.
            // reverse tangents if necessary.
            Vector3 tangentCross = tangent.CrossProduct(binormal);
            if (tangentCross.DotProduct(normal) < 0.0f) {
                tangent = -tangent;
                binormal = -binormal;
            }

            return tangent;
        }
        /// <summary>
        ///		Checks wether a given point is inside a triangle, in a
        ///		2-dimensional (Cartesian) space.
        /// </summary>
        /// <remarks>
        ///		The vertices of the triangle must be given in either
        ///		trigonometrical (anticlockwise) or inverse trigonometrical
        ///		(clockwise) order.
        /// </remarks>
        /// <param name="px">
        ///    The X-coordinate of the point.
        /// </param>
        /// <param name="py">
        ///    The Y-coordinate of the point.
        /// </param>
        /// <param name="ax">
        ///    The X-coordinate of the triangle's first vertex.
        /// </param>
        /// <param name="ay">
        ///    The Y-coordinate of the triangle's first vertex.
        /// </param>
        /// <param name="bx">
        ///    The X-coordinate of the triangle's second vertex.
        /// </param>
        /// <param name="by">
        ///    The Y-coordinate of the triangle's second vertex.
        /// </param>
        /// <param name="cx">
        ///    The X-coordinate of the triangle's third vertex.
        /// </param>
        /// <param name="cy">
        ///    The Y-coordinate of the triangle's third vertex.
        /// </param>
        /// <returns>
        ///    <list type="bullet">
        ///        <item>
        ///            <description><b>true</b> - the point resides in the triangle.</description>
        ///        </item>
        ///        <item>
        ///            <description><b>false</b> - the point is outside the triangle</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public static bool PointInTri2D(float  px, float py, float ax, float ay, float bx, float by, float cx, float cy) {
            float v1x, v2x, v1y, v2y;
            bool bClockwise;

            v1x = bx - ax;
            v1y = by - ay;

            v2x = px - bx;
            v2y = py - by;

            bClockwise = (v1x * v2y - v1y * v2x >= 0.0);

            v1x = cx - bx;
            v1y = cy - by;

            v2x = px - cx;
            v2y = py - cy;

            if ((v1x * v2y - v1y * v2x >= 0.0) != bClockwise)
                return false;

            v1x = ax - cx;
            v1y = ay - cy;

            v2x = px - ax;
            v2y = py - ay;

            if ((v1x * v2y - v1y * v2x >= 0.0) != bClockwise)
                return false;

            return true;
        }

        /// <summary>
        ///    Method delegate with a simple signature. 
        ///    Used to measure execution time of a method for instance.
        /// </summary>
        public delegate void SimpleMethodDelegate();

        /// <summary>
        ///     Measure the execution time of a method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns>The elapsed time in seconds.</returns>
        public static float Measure(SimpleMethodDelegate method) {
            long start = System.Diagnostics.Stopwatch.GetTimestamp();

            method();

            double elapsed = (double)(System.Diagnostics.Stopwatch.GetTimestamp() - start);
            double freq = (double)System.Diagnostics.Stopwatch.Frequency;

            return (float)(elapsed / freq);
        }


        #region Intersection Methods

        /// <summary>
        ///    Tests an intersection between a ray and a box.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="box"></param>
        /// <returns>A Pair object containing whether the intersection occurred, and the distance between the 2 objects.</returns>
        public static IntersectResult Intersects(Ray ray, AxisAlignedBox box) {
            return Mogre.Math.Intersects(ray, box);
            //Contract.RequiresNotNull(ray, "ray");
            //Contract.RequiresNotNull(box, "box");

            //if (box.IsNull) {
            //    return new IntersectResult(false, 0);
            //}

            //if (box.IsInfinite) {
            //    return new IntersectResult(true, 0);
            //}

            //float lowt = 0.0f;
            //float t;
            //bool hit = false;
            //Vector3 hitPoint;
            //Vector3 min = box.Minimum;
            //Vector3 max = box.Maximum;

            //// check origin inside first
            //if (ray.origin > min && ray.origin < max) {
            //    return new IntersectResult(true, 0.0f);
            //}

            //// check each face in turn, only check closest 3

            //// Min X
            //if (ray.origin.x <= min.x && ray.direction.x > 0) {
            //    t = (min.x - ray.origin.x) / ray.direction.x;

            //    if (t >= 0) {
            //        // substitue t back into ray and check bounds and distance
            //        hitPoint = ray.origin + ray.direction * t;

            //        if (hitPoint.y >= min.y && hitPoint.y <= max.y &&
            //            hitPoint.z >= min.z && hitPoint.z <= max.z &&
            //            (!hit || t < lowt)) {

            //            hit = true;
            //            lowt = t;
            //        }
            //    }
            //}

            //// Max X
            //if (ray.origin.x >= max.x && ray.direction.x < 0) {
            //    t = (max.x - ray.origin.x) / ray.direction.x;

            //    if (t >= 0) {
            //        // substitue t back into ray and check bounds and distance
            //        hitPoint = ray.origin + ray.direction * t;

            //        if (hitPoint.y >= min.y && hitPoint.y <= max.y &&
            //            hitPoint.z >= min.z && hitPoint.z <= max.z &&
            //            (!hit || t < lowt)) {

            //            hit = true;
            //            lowt = t;
            //        }
            //    }
            //}

            //// Min Y
            //if (ray.origin.y <= min.y && ray.direction.y > 0) {
            //    t = (min.y - ray.origin.y) / ray.direction.y;

            //    if (t >= 0) {
            //        // substitue t back into ray and check bounds and distance
            //        hitPoint = ray.origin + ray.direction * t;

            //        if (hitPoint.x >= min.x && hitPoint.x <= max.x &&
            //            hitPoint.z >= min.z && hitPoint.z <= max.z &&
            //            (!hit || t < lowt)) {

            //            hit = true;
            //            lowt = t;
            //        }
            //    }
            //}

            //// Max Y
            //if (ray.origin.y >= max.y && ray.direction.y < 0) {
            //    t = (max.y - ray.origin.y) / ray.direction.y;

            //    if (t >= 0) {
            //        // substitue t back into ray and check bounds and distance
            //        hitPoint = ray.origin + ray.direction * t;

            //        if (hitPoint.x >= min.x && hitPoint.x <= max.x &&
            //            hitPoint.z >= min.z && hitPoint.z <= max.z &&
            //            (!hit || t < lowt)) {

            //            hit = true;
            //            lowt = t;
            //        }
            //    }
            //}

            //// Min Z
            //if (ray.origin.z <= min.z && ray.direction.z > 0) {
            //    t = (min.z - ray.origin.z) / ray.direction.z;

            //    if (t >= 0) {
            //        // substitue t back into ray and check bounds and distance
            //        hitPoint = ray.origin + ray.direction * t;

            //        if (hitPoint.x >= min.x && hitPoint.x <= max.x &&
            //            hitPoint.y >= min.y && hitPoint.y <= max.y &&
            //            (!hit || t < lowt)) {

            //            hit = true;
            //            lowt = t;
            //        }
            //    }
            //}

            //// Max Z
            //if (ray.origin.z >= max.z && ray.direction.z < 0) {
            //    t = (max.z - ray.origin.z) / ray.direction.z;

            //    if (t >= 0) {
            //        // substitue t back into ray and check bounds and distance
            //        hitPoint = ray.origin + ray.direction * t;

            //        if (hitPoint.x >= min.x && hitPoint.x <= max.x &&
            //            hitPoint.y >= min.y && hitPoint.y <= max.y &&
            //            (!hit || t < lowt)) {

            //            hit = true;
            //            lowt = t;
            //        }
            //    }
            //}

            //return new IntersectResult(hit, lowt);
        }

        public static IntersectResult Intersects(Ray ray, Vector3 a,
            Vector3 b, Vector3 c, Vector3 normal, bool positiveSide, bool negativeSide) {
            // Calculate intersection with plane.
            float t;
            {
                float denom = normal.DotProduct(ray.Direction);
                // Check intersect side
                if (denom > +float.Epsilon) {
                    if (!negativeSide)
                        return new IntersectResult(false, 0);
                }
                else if (denom < -float.Epsilon) {
                    if (!positiveSide)
                        return new IntersectResult(false, 0);
                }
                else {
                    // Parallel or triangle area is close to zero when
                    // the plane normal not normalised.
                    return new IntersectResult(false, 0);
                }

                t = normal.DotProduct(a - ray.Origin) / denom;
                if (t < 0) {
                    return new IntersectResult(false, 0);
                }
            }

            // Calculate the largest area projection plane in X, Y or Z.
            int i0, i1;
            {
                float n0 = Math.Utility.Abs(normal[0]);
                float n1 = Math.Utility.Abs(normal[1]);
                float n2 = Math.Utility.Abs(normal[2]);

                i0 = 1;
                i1 = 2;
                if (n1 > n2) {
                    if (n1 > n0)
                        i0 = 0;
                }
                else {
                    if (n2 > n0)
                        i1 = 0;
                }

            }

            // Check the intersection point is inside the triangle.
            {
                float u1 = b[i0] - a[i0];
                float v1 = b[i1] - a[i1];
                float u2 = c[i0] - a[i0];
                float v2 = c[i1] - a[i1];
                float u0 = t * ray.Direction[i0] + ray.Origin[i0] - a[i0];
                float v0 = t * ray.Direction[i1] + ray.Origin[i1] - a[i1];

                float alpha = u0 * v2 - u2 * v0;
                float beta = u1 * v0 - u0 * v1;
                float area = u1 * v2 - u2 * v1;

                // epsilon to avoid float precision error
                float EPSILON = 1e-3f;

                float tolerance = -EPSILON * area;

                if (area > 0) {
                    if (alpha < tolerance || beta < tolerance || alpha + beta > area - tolerance)
                        return new IntersectResult(false, 0);
                }
                else {
                    if (alpha > tolerance || beta > tolerance || alpha + beta < area - tolerance)
                        return new IntersectResult(false, 0);
                }

            }

            return new IntersectResult(true, t);
        }

        public static IntersectResult Intersects(Ray ray, Vector3 a,
            Vector3 b, Vector3 c, bool positiveSide, bool negativeSide) {
            Vector3 normal = CalculateBasicFaceNormalWithoutNormalize(a, b, c);
            return Intersects(ray, a, b, c, normal, positiveSide, negativeSide);
        }

        /// <summary>
        ///    Tests an intersection between two boxes.
        /// </summary>
        /// <param name="boxA">
        ///    The primary box.
        /// </param>
        /// <param name="boxB">
        ///    The box to test intersection with boxA.
        /// </param>
        /// <returns>
        ///    <list type="bullet">
        ///        <item>
        ///            <description>None - There was no intersection between the 2 boxes.</description>
        ///        </item>
        ///        <item>
        ///            <description>Contained - boxA is fully within boxB.</description>
        ///         </item>
        ///        <item>
        ///            <description>Contains - boxB is fully within boxA.</description>
        ///         </item>
        ///        <item>
        ///            <description>Partial - boxA is partially intersecting with boxB.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// Submitted by: romout
        public static Intersection Intersects(AxisAlignedBox boxA, AxisAlignedBox boxB) {
            //return Mogre.Math.Intersects(boxA, boxB);
            //Contract.RequiresNotNull(boxA, "boxA");
            //Contract.RequiresNotNull(boxB, "boxB");

            // grab the max and mix vectors for both boxes for comparison
            Vector3 minA = boxA.Minimum;
            Vector3 maxA = boxA.Maximum;
            Vector3 minB = boxB.Minimum;
            Vector3 maxB = boxB.Maximum;
            Intersection par = new Pair<bool, float>();
            if ((minB.x < minA.x) &&
                (maxB.x > maxA.x) &&
                (minB.y < minA.y) &&
                (maxB.y > maxA.y) &&
                (minB.z < minA.z) &&
                (maxB.z > maxA.z)) {

                // boxA is within boxB
                par.first = true;
                return par;
            }

            if ((minB.x > minA.x) &&
                (maxB.x < maxA.x) &&
                (minB.y > minA.y) &&
                (maxB.y < maxA.y) &&
                (minB.z > minA.z) &&
                (maxB.z < maxA.z)) {

                // boxB is within boxA
                par.first = true;
                return par;
            }

            if ((minB.x > maxA.x) ||
                (minB.y > maxA.y) ||
                (minB.z > maxA.z) ||
                (maxB.x < minA.x) ||
                (maxB.y < minA.y) ||
                (maxB.z < minA.z)) {

                // not interesting at all
                return par;
            }

            // if we got this far, they are partially intersecting
            return par;
        }


        public static IntersectResult Intersects(Ray ray, Sphere sphere) {
            return Intersects(ray, sphere, false);
        }

        /// <summary>
        ///		Ray/Sphere intersection test.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="sphere"></param>
        /// <param name="discardInside"></param>
        /// <returns>Struct that contains a bool (hit?) and distance.</returns>
        public static IntersectResult Intersects(Ray ray, Sphere sphere, bool discardInside) {
            return Mogre.Math.Intersects(ray, sphere, discardInside);
            //Contract.RequiresNotNull(ray, "ray");
            //Contract.RequiresNotNull(sphere, "sphere");

            //Vector3 rayDir = ray.Direction;
            ////Adjust ray origin relative to sphere center
            //Vector3 rayOrig = ray.Origin - sphere.Center;
            //float radius = sphere.Radius;

            //// check origin inside first
            //if ((rayOrig.LengthSquared <= radius * radius) && discardInside) {
            //    return new IntersectResult(true, 0);
            //}

            //// mmm...sweet quadratics
            //// Build coeffs which can be used with std quadratic solver
            //// ie t = (-b +/- sqrt(b*b* + 4ac)) / 2a
            //float a = rayDir.DotProduct(rayDir);
            //float b = 2 * rayOrig.DotProduct(rayDir);
            //float c = rayOrig.DotProduct(rayOrig) - (radius * radius);

            //// calc determinant
            //float d = (b * b) - (4 * a * c);

            //if (d < 0) {
            //    // no intersection
            //    return new IntersectResult(false, 0);
            //}
            //else {
            //    // BTW, if d=0 there is one intersection, if d > 0 there are 2
            //    // But we only want the closest one, so that's ok, just use the 
            //    // '-' version of the solver
            //    float t = (-b - Utility.Sqrt(d)) / (2 * a);

            //    if (t < 0) {
            //        t = (-b + Utility.Sqrt(d)) / (2 * a);
            //    }

            //    return new IntersectResult(true, t);
            //}
        }

        ///// <summary>
        /////		Ray/Plane intersection test.
        ///// </summary>
        ///// <param name="ray"></param>
        ///// <param name="plane"></param>
        ///// <returns>Struct that contains a bool (hit?) and distance.</returns>
        //public static IntersectResult Intersects(Ray ray, Plane plane) {
            //Mogre.Pair<bool,float>par= Mogre.Math.Intersects(ray, plane);
            //IntersectResult ir = new IntersectResult(par.first, par.second);
            //return ir;
            //Contract.RequiresNotNull(ray, "ray");

            //float denom = plane.normal.DotProduct(ray.Direction);

            //if (Utility.Abs(denom) < float.Epsilon) {
            //    // Parellel
            //    return new IntersectResult(false, 0);
            //}
            //else {
            //    float nom = plane.normal.DotProduct(ray.Origin) + plane.d;
            //    float t = -(nom / denom);
            //    return new IntersectResult(t >= 0, t);
            //}
        //}

        /// <summary>
        ///		Sphere/Box intersection test.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Sphere sphere, AxisAlignedBox box) {
            return Mogre.Math.Intersects(sphere, box);
            //Contract.RequiresNotNull(sphere, "sphere");
            //Contract.RequiresNotNull(box, "box");

            //if (box.IsNull)
            //    return false;

            //// Use splitting planes
            //Vector3 center = sphere.Center;
            //float radius = sphere.Radius;
            //Vector3 min = box.Minimum;
            //Vector3 max = box.Maximum;

            //// just test facing planes, early fail if sphere is totally outside
            //if (center.x < min.x &&
            //    min.x - center.x > radius) {
            //    return false;
            //}
            //if (center.x > max.x &&
            //    center.x - max.x > radius) {
            //    return false;
            //}

            //if (center.y < min.y &&
            //    min.y - center.y > radius) {
            //    return false;
            //}
            //if (center.y > max.y &&
            //    center.y - max.y > radius) {
            //    return false;
            //}

            //if (center.z < min.z &&
            //    min.z - center.z > radius) {
            //    return false;
            //}
            //if (center.z > max.z &&
            //    center.z - max.z > radius) {
            //    return false;
            //}

            //// Must intersect
            //return true;
        }

        /// <summary>
        ///		Plane/Box intersection test.
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Plane plane, AxisAlignedBox box) {
            return Mogre.Math.Intersects(plane, box);
            //Contract.RequiresNotNull(box, "box");

            //if (box.IsNull)
            //    return false;

            //// Get corners of the box
            //Vector3[] corners = box.Corners;

            //// Test which side of the plane the corners are
            //// Intersection occurs when at least one corner is on the 
            //// opposite side to another
            //PlaneSide lastSide = plane.GetSide(corners[0]);

            //for (int corner = 1; corner < 8; corner++) {
            //    if (plane.GetSide(corners[corner]) != lastSide) {
            //        return true;
            //    }
            //}

            //return false;
        }

        /// <summary>
        ///		Sphere/Plane intersection test.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="plane"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Sphere sphere, Plane plane) {
            return Mogre.Math.Intersects(sphere, plane);
            //Contract.RequiresNotNull(sphere, "sphere");

            //return Utility.Abs(plane.normal.DotProduct(sphere.Center)) <= sphere.Radius;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="box"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static Tuple<bool, float, float> Intersect(Ray ray, AxisAlignedBox box) {
            if (box.IsNull)
                return new Tuple<bool, float, float>(false, float.NaN, float.NaN);

            if (box.IsInfinite) {
                return new Tuple<bool, float, float>(true, float.NaN, float.PositiveInfinity);
            }

            Vector3 min = box.Minimum;
            Vector3 max = box.Maximum;
            Vector3 rayorig = ray.Origin;
            Vector3 rayDir = ray.Direction;

            Vector3 absDir = Vector3.ZERO;
            absDir[0] = Abs(rayDir[0]);
            absDir[1] = Abs(rayDir[1]);
            absDir[2] = Abs(rayDir[2]);

            // Sort the axis, ensure check minimise floating error axis first
            int imax = 0, imid = 1, imin = 2;
            if (absDir[0] < absDir[2]) {
                imax = 2;
                imin = 0;
            }
            if (absDir[1] < absDir[imin]) {
                imid = imin;
                imin = 1;
            }
            else if (absDir[1] > absDir[imax]) {
                imid = imax;
                imax = 1;
            }

            float start = 0, end = float.PositiveInfinity;
            // Check each axis in turn

            if (!CalcAxis(imax, rayDir, rayorig, min, max, ref end, ref start))
                return new Tuple<bool, float, float>(false, float.NaN, float.NaN);

            if (absDir[imid] < float.Epsilon) {
                // Parallel with middle and minimise axis, check bounds only
                if (rayorig[imid] < min[imid] || rayorig[imid] > max[imid] ||
                    rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
                    return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
            }
            else {
                if (!CalcAxis(imid, rayDir, rayorig, min, max, ref end, ref start))
                    return new Tuple<bool, float, float>(false, float.NaN, float.NaN);

                if (absDir[imin] < float.Epsilon) {
                    // Parallel with minimise axis, check bounds only
                    if (rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
                        return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
                }
                else {
                    if (!CalcAxis(imin, rayDir, rayorig, min, max, ref end, ref start))
                        return new Tuple<bool, float, float>(false, float.NaN, float.NaN);
                }
            }
            return new Tuple<bool, float, float>(true, start, end);
        }

        private static bool CalcAxis(int i, Vector3 raydir, Vector3 rayorig, Vector3 min, Vector3 max, ref float end, ref float start) {
            float denom = 1 / raydir[i];
            float newstart = (min[i] - rayorig[i]) * denom;
            float newend = (max[i] - rayorig[i]) * denom;
            if (newstart > newend)
                Swap<float>(ref newstart, ref newend);
            if (newstart > end || newend < start)
                return false;
            if (newstart > start)
                start = newstart;
            if (newend < end)
                end = newend;

            return true;
        }

        ///// <summary>
        /////    Ray/PlaneBoundedVolume intersection test.
        ///// </summary>
        ///// <param name="ray"></param>
        ///// <param name="volume"></param>
        ///// <returns>Struct that contains a bool (hit?) and distance.</returns>
        //public static IntersectResult Intersects(Ray ray, PlaneBoundedVolume volume) {
        //    Contract.RequiresNotNull(ray, "ray");
        //    Contract.RequiresNotNull(volume, "volume");

        //    PlaneList planes = volume.planes;

        //    float maxExtDist = 0.0f;
        //    float minIntDist = float.PositiveInfinity;

        //    float dist, denom, nom;

        //    for (int i = 0; i < planes.Count; i++) {
        //        Plane plane = (Plane)planes[i];

        //        denom = plane.normal.DotProduct(ray.Direction);
        //        if (Utility.Abs(denom) < float.Epsilon) {
        //            // Parallel
        //            if (plane.GetSide(ray.Origin) == volume.outside)
        //                return new IntersectResult(false, 0);

        //            continue;
        //        }

        //        nom = plane.normal.DotProduct(ray.Origin) + plane.D;
        //        dist = -(nom / denom);

        //        if (volume.outside == PlaneSide.Negative)
        //            nom = -nom;

        //        if (dist > 0.0f) {
        //            if (nom > 0.0f) {
        //                if (maxExtDist < dist)
        //                    maxExtDist = dist;
        //            }
        //            else {
        //                if (minIntDist > dist)
        //                    minIntDist = dist;
        //            }
        //        }
        //        else {
        //            //Ray points away from plane
        //            if (volume.outside == PlaneSide.Negative)
        //                denom = -denom;

        //            if (denom > 0.0f)
        //                return new IntersectResult(false, 0);
        //        }
        //    }

        //    if (maxExtDist > minIntDist)
        //        return new IntersectResult(false, 0);

        //    return new IntersectResult(true, maxExtDist);
        //}

        #endregion Intersection Methods

        /// <summary>
        /// Swaps two values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public static void Swap<T>(ref T v1, ref T v2) {
            T temp = v1;
            v1 = v2;
            v2 = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static T Clamp<T>(T value, T max, T min)
                 where T : System.IComparable<T> {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        public static T Max<T>(T value, T max)
         where T : System.IComparable<T> {
            T result = value;
            if (value.CompareTo(max) < 0)
                result = max;
            return result;
        }

        public static T Min<T>(T value, T min)
                 where T : System.IComparable<T> {
            T result = value;
            if (value.CompareTo(min) > 0)
                result = min;
            return result;
        }

        public static float Sqr(float  number) {
            return number * number;
        }

        public static int Log2(int x) {
            if (x <= 65536) {
                if (x <= 256) {
                    if (x <= 16) {
                        if (x <= 4) {
                            if (x <= 2) {
                                if (x <= 1) {
                                    return 0;
                                }
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                            return 3;
                        return 4;
                    }
                    if (x <= 64) {
                        if (x <= 32)
                            return 5;
                        return 6;
                    }
                    if (x <= 128)
                        return 7;
                    return 8;
                }
                if (x <= 4096) {
                    if (x <= 1024) {
                        if (x <= 512)
                            return 9;
                        return 10;
                    }
                    if (x <= 2048)
                        return 11;
                    return 12;
                }
                if (x <= 16384) {
                    if (x <= 8192)
                        return 13;
                    return 14;
                }
                if (x <= 32768)
                    return 15;
                return 16;
            }
            if (x <= 16777216) {
                if (x <= 1048576) {
                    if (x <= 262144) {
                        if (x <= 131072)
                            return 17;
                        return 18;
                    }
                    if (x <= 524288)
                        return 19;
                    return 20;
                }
                if (x <= 4194304) {
                    if (x <= 2097152)
                        return 21;
                    return 22;
                }
                if (x <= 8388608)
                    return 23;
                return 24;
            }
            if (x <= 268435456) {
                if (x <= 67108864) {
                    if (x <= 33554432)
                        return 25;
                    return 26;
                }
                if (x <= 134217728)
                    return 27;
                return 28;
            }
            if (x <= 1073741824) {
                if (x <= 536870912)
                    return 29;
                return 30;
            }
            //	since int is unsigned it can never be higher than 2,147,483,647
            //	if( x <= 2147483648 )
            //		return	31;	
            //	return	32;	
            return 31;
        }

        /// <summary>
        /// Generates a value based on the Gaussian (normal) distribution function
        /// with the given offset and scale parameters.
        /// </summary>
        /// <returns></returns>
        public static float GaussianDistribution(float  x, float offset, float scale) {
            float nom = (float)System.Math.Exp(-Utility.Sqr(x - offset) / (2 * Utility.Sqr(scale)));
            float denom = scale * Utility.Sqrt(2 * Utility.PI);

            return nom / denom;
        }
    }

    #region Return result structures

    ///// <summary>
    /////		Simple struct to allow returning a complex intersection result.
    ///// </summary>
    //public struct IntersectResult {
    //    #region Fields

    //    /// <summary>
    //    ///		Did the intersection test result in a hit?
    //    /// </summary>
    //    public bool Hit;

    //    /// <summary>
    //    ///		If Hit was true, this will hold a query specific distance value.
    //    ///		i.e. for a Ray-Box test, the distance will be the distance from the start point
    //    ///		of the ray to the point of intersection.
    //    /// </summary>
    //    public float Distance;

    //    #endregion Fields

    //    /// <summary>
    //    ///		Constructor.
    //    /// </summary>
    //    /// <param name="hit"></param>
    //    /// <param name="distance"></param>
    //    public IntersectResult(bool hit, float distance) {
    //        this.Hit = hit;
    //        this.Distance = distance;
    //    }
    //}

    #endregion Return result structures
}
