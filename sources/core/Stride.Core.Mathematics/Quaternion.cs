// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.MathF;

namespace Stride.Core.Mathematics;

/// <summary>
/// Represents a four dimensional mathematical quaternion.
/// </summary>
[DataContract("quaternion")]
[DataStyle(DataStyle.Compact)]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct Quaternion : IEquatable<Quaternion>, ISpanFormattable
{
    /// <summary>
    /// The size of the <see cref="Stride.Core.Mathematics.Quaternion"/> type, in bytes.
    /// </summary>
    public static readonly int SizeInBytes = Unsafe.SizeOf<Quaternion>();

    /// <summary>
    /// A <see cref="Stride.Core.Mathematics.Quaternion"/> with all of its components set to zero.
    /// </summary>
    public static readonly Quaternion Zero = new();

    /// <summary>
    /// A <see cref="Stride.Core.Mathematics.Quaternion"/> with all of its components set to one.
    /// </summary>
    public static readonly Quaternion One = new(1.0f, 1.0f, 1.0f, 1.0f);

    /// <summary>
    /// The identity <see cref="Stride.Core.Mathematics.Quaternion"/> (0, 0, 0, 1).
    /// </summary>
    public static readonly Quaternion Identity = new(0.0f, 0.0f, 0.0f, 1.0f);

    /// <summary>
    /// The X component of the quaternion.
    /// </summary>
    public float X;

    /// <summary>
    /// The Y component of the quaternion.
    /// </summary>
    public float Y;

    /// <summary>
    /// The Z component of the quaternion.
    /// </summary>
    public float Z;

    /// <summary>
    /// The W component of the quaternion.
    /// </summary>
    public float W;

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.Quaternion"/> struct.
    /// </summary>
    /// <param name="value">The value that will be assigned to all components.</param>
    public Quaternion(float value)
    {
        X = value;
        Y = value;
        Z = value;
        W = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.Quaternion"/> struct.
    /// </summary>
    /// <param name="value">A vector containing the values with which to initialize the components.</param>
    public Quaternion(Vector4 value)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
        W = value.W;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.Quaternion"/> struct.
    /// </summary>
    /// <param name="value">A vector containing the values with which to initialize the X, Y, and Z components.</param>
    /// <param name="w">Initial value for the W component of the quaternion.</param>
    public Quaternion(Vector3 value, float w)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
        W = w;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.Quaternion"/> struct.
    /// </summary>
    /// <param name="value">A vector containing the values with which to initialize the X and Y components.</param>
    /// <param name="z">Initial value for the Z component of the quaternion.</param>
    /// <param name="w">Initial value for the W component of the quaternion.</param>
    public Quaternion(Vector2 value, float z, float w)
    {
        X = value.X;
        Y = value.Y;
        Z = z;
        W = w;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.Quaternion"/> struct.
    /// </summary>
    /// <param name="x">Initial value for the X component of the quaternion.</param>
    /// <param name="y">Initial value for the Y component of the quaternion.</param>
    /// <param name="z">Initial value for the Z component of the quaternion.</param>
    /// <param name="w">Initial value for the W component of the quaternion.</param>
    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stride.Core.Mathematics.Quaternion"/> struct.
    /// </summary>
    /// <param name="values">The values to assign to the X, Y, Z, and W components of the quaternion. This must be an array with four elements.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
    public Quaternion(float[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        if (values.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(values), "There must be four and only four input values for Quaternion.");

        X = values[0];
        Y = values[1];
        Z = values[2];
        W = values[3];
    }

    /// <summary>
    /// Gets a value indicating whether this instance is equivalent to the identity quaternion.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is an identity quaternion; otherwise, <c>false</c>.
    /// </value>
    public bool IsIdentity
    {
        get { return this.Equals(Identity); }
    }

    /// <summary>
    /// Gets a value indicting whether this instance is normalized.
    /// </summary>
    public bool IsNormalized
    {
        get { return MathF.Abs((X * X) + (Y * Y) + (Z * Z) + (W * W) - 1f) < MathUtil.ZeroTolerance; }
    }

    /// <summary>
    /// Gets the angle of the quaternion.
    /// </summary>
    /// <value>The quaternion's angle.</value>
    public float Angle
    {
        get
        {
            float length = (X * X) + (Y * Y) + (Z * Z);
            if (length < MathUtil.ZeroTolerance)
                return 0.0f;

            return 2.0f * MathF.Acos(W);
        }
    }

    /// <summary>
    /// Gets the axis components of the quaternion.
    /// </summary>
    /// <value>The axis components of the quaternion.</value>
    public Vector3 Axis
    {
        get
        {
            float length = (X * X) + (Y * Y) + (Z * Z);
            if (length < MathUtil.ZeroTolerance)
                return Vector3.UnitX;

            float inv = 1.0f / length;
            return new Vector3(X * inv, Y * inv, Z * inv);
        }
    }

    /// <summary>
    /// Gets yaw/pitch/roll equivalent of the quaternion
    /// </summary>
    public Vector3 YawPitchRoll
    {
        get
        {
            Vector3 yawPitchRoll;
            RotationYawPitchRoll(ref this, out yawPitchRoll.X, out yawPitchRoll.Y, out yawPitchRoll.Z);
            return yawPitchRoll;
        }
    }

    /// <summary>
    /// Gets or sets the component at the specified index.
    /// </summary>
    /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
    /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
    /// <returns>The value of the component at the specified index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return X;
                case 1: return Y;
                case 2: return Z;
                case 3: return W;
            }

            throw new ArgumentOutOfRangeException(nameof(index), "Indices for Quaternion run from 0 to 3, inclusive.");
        }

        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                case 3: W = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Quaternion run from 0 to 3, inclusive.");
            }
        }
    }

    /// <summary>
    /// Conjugates the quaternion.
    /// </summary>
    public void Conjugate()
    {
        X = -X;
        Y = -Y;
        Z = -Z;
    }

    /// <summary>
    /// Conjugates and renormalizes the quaternion.
    /// </summary>
    public void Invert()
    {
        float lengthSq = LengthSquared();
        if (lengthSq > MathUtil.ZeroTolerance)
        {
            lengthSq = 1.0f / lengthSq;

            X = -X * lengthSq;
            Y = -Y * lengthSq;
            Z = -Z * lengthSq;
            W = W * lengthSq;
        }
    }

    /// <summary>
    /// Calculates the length of the quaternion.
    /// </summary>
    /// <returns>The length of the quaternion.</returns>
    /// <remarks>
    /// <see cref="Stride.Core.Mathematics.Quaternion.LengthSquared"/> may be preferred when only the relative length is needed
    /// and speed is of the essence.
    /// </remarks>
    public readonly float Length()
    {
        return MathF.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
    }

    /// <summary>
    /// Calculates the squared length of the quaternion.
    /// </summary>
    /// <returns>The squared length of the quaternion.</returns>
    /// <remarks>
    /// This method may be preferred to <see cref="Stride.Core.Mathematics.Quaternion.Length"/> when only a relative length is needed
    /// and speed is of the essence.
    /// </remarks>
    public readonly float LengthSquared()
    {
        return (X * X) + (Y * Y) + (Z * Z) + (W * W);
    }

    /// <summary>
    /// Converts the quaternion into a unit quaternion.
    /// </summary>
    public void Normalize()
    {
        float length = Length();
        if (length > MathUtil.ZeroTolerance)
        {
            float inverse = 1.0f / length;
            X *= inverse;
            Y *= inverse;
            Z *= inverse;
            W *= inverse;
        }
    }

    /// <summary>
    /// Creates an array containing the elements of the quaternion.
    /// </summary>
    /// <returns>A four-element array containing the components of the quaternion.</returns>
    public float[] ToArray()
    {
        return [X, Y, Z, W];
    }

    /// <summary>
    /// Adds two quaternions.
    /// </summary>
    /// <param name="left">The first quaternion to add.</param>
    /// <param name="right">The second quaternion to add.</param>
    /// <param name="result">When the method completes, contains the sum of the two quaternions.</param>
    public static void Add(ref readonly Quaternion left, ref readonly Quaternion right, out Quaternion result)
    {
        result.X = left.X + right.X;
        result.Y = left.Y + right.Y;
        result.Z = left.Z + right.Z;
        result.W = left.W + right.W;
    }

    /// <summary>
    /// Adds two quaternions.
    /// </summary>
    /// <param name="left">The first quaternion to add.</param>
    /// <param name="right">The second quaternion to add.</param>
    /// <returns>The sum of the two quaternions.</returns>
    public static Quaternion Add(Quaternion left, Quaternion right)
    {
        Add(ref left, ref right, out var result);
        return result;
    }

    /// <summary>
    /// Subtracts two quaternions.
    /// </summary>
    /// <param name="left">The first quaternion to subtract.</param>
    /// <param name="right">The second quaternion to subtract.</param>
    /// <param name="result">When the method completes, contains the difference of the two quaternions.</param>
    public static void Subtract(ref readonly Quaternion left, ref readonly Quaternion right, out Quaternion result)
    {
        result.X = left.X - right.X;
        result.Y = left.Y - right.Y;
        result.Z = left.Z - right.Z;
        result.W = left.W - right.W;
    }

    /// <summary>
    /// Subtracts two quaternions.
    /// </summary>
    /// <param name="left">The first quaternion to subtract.</param>
    /// <param name="right">The second quaternion to subtract.</param>
    /// <returns>The difference of the two quaternions.</returns>
    public static Quaternion Subtract(Quaternion left, Quaternion right)
    {
        Subtract(ref left, ref right, out var result);
        return result;
    }

    /// <summary>
    /// Scales a quaternion by the given value.
    /// </summary>
    /// <param name="value">The quaternion to scale.</param>
    /// <param name="scale">The amount by which to scale the quaternion.</param>
    /// <param name="result">When the method completes, contains the scaled quaternion.</param>
    public static void Multiply(ref readonly Quaternion value, float scale, out Quaternion result)
    {
        result.X = value.X * scale;
        result.Y = value.Y * scale;
        result.Z = value.Z * scale;
        result.W = value.W * scale;
    }

    /// <summary>
    /// Scales a quaternion by the given value.
    /// </summary>
    /// <param name="value">The quaternion to scale.</param>
    /// <param name="scale">The amount by which to scale the quaternion.</param>
    /// <returns>The scaled quaternion.</returns>
    public static Quaternion Multiply(Quaternion value, float scale)
    {
        Multiply(ref value, scale, out var result);
        return result;
    }

    /// <summary>
    /// Modulates a quaternion by another.
    /// </summary>
    /// <param name="left">The first quaternion to modulate.</param>
    /// <param name="right">The second quaternion to modulate.</param>
    /// <param name="result">When the moethod completes, contains the modulated quaternion.</param>
    public static void Multiply(ref readonly Quaternion left, ref readonly Quaternion right, out Quaternion result)
    {
        float lx = left.X;
        float ly = left.Y;
        float lz = left.Z;
        float lw = left.W;
        float rx = right.X;
        float ry = right.Y;
        float rz = right.Z;
        float rw = right.W;

        result.X = (rx * lw + lx * rw + ry * lz) - (rz * ly);
        result.Y = (ry * lw + ly * rw + rz * lx) - (rx * lz);
        result.Z = (rz * lw + lz * rw + rx * ly) - (ry * lx);
        result.W = (rw * lw) - (rx * lx + ry * ly + rz * lz);
    }

    /// <summary>
    /// Modulates a quaternion by another.
    /// </summary>
    /// <param name="left">The first quaternion to modulate.</param>
    /// <param name="right">The second quaternion to modulate.</param>
    /// <returns>The modulated quaternion.</returns>
    public static Quaternion Multiply(in Quaternion left, in Quaternion right)
    {
        float lx = left.X;
        float ly = left.Y;
        float lz = left.Z;
        float lw = left.W;
        float rx = right.X;
        float ry = right.Y;
        float rz = right.Z;
        float rw = right.W;

        return new Quaternion(
            (rx * lw + lx * rw + ry * lz) - (rz * ly),
            (ry * lw + ly * rw + rz * lx) - (rx * lz),
            (rz * lw + lz * rw + rx * ly) - (ry * lx),
            (rw * lw) - (rx * lx + ry * ly + rz * lz));
    }

    /// <summary>
    /// Reverses the direction of a given quaternion.
    /// </summary>
    /// <param name="value">The quaternion to negate.</param>
    /// <param name="result">When the method completes, contains a quaternion facing in the opposite direction.</param>
    public static void Negate(ref readonly Quaternion value, out Quaternion result)
    {
        result.X = -value.X;
        result.Y = -value.Y;
        result.Z = -value.Z;
        result.W = -value.W;
    }

    /// <summary>
    /// Reverses the direction of a given quaternion.
    /// </summary>
    /// <param name="value">The quaternion to negate.</param>
    /// <returns>A quaternion facing in the opposite direction.</returns>
    public static Quaternion Negate(Quaternion value)
    {
        Negate(ref value, out var result);
        return result;
    }

    /// <summary>
    /// Returns a <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
    /// </summary>
    /// <param name="value1">A <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
    /// <param name="value2">A <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
    /// <param name="value3">A <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
    /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
    /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
    /// <param name="result">When the method completes, contains a new <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</param>
    public static void Barycentric(ref readonly Quaternion value1, ref readonly Quaternion value2, ref readonly Quaternion value3, float amount1, float amount2, out Quaternion result)
    {
        Slerp(in value1, in value2, amount1 + amount2, out var start);
        Slerp(in value1, in value3, amount1 + amount2, out var end);
        Slerp(ref start, ref end, amount2 / (amount1 + amount2), out result);
    }

    /// <summary>
    /// Returns a <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
    /// </summary>
    /// <param name="value1">A <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
    /// <param name="value2">A <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
    /// <param name="value3">A <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
    /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
    /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
    /// <returns>A new <see cref="Stride.Core.Mathematics.Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</returns>
    public static Quaternion Barycentric(Quaternion value1, Quaternion value2, Quaternion value3, float amount1, float amount2)
    {
        Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out var result);
        return result;
    }

    /// <summary>
    /// Conjugates a quaternion.
    /// </summary>
    /// <param name="value">The quaternion to conjugate.</param>
    /// <param name="result">When the method completes, contains the conjugated quaternion.</param>
    public static void Conjugate(ref readonly Quaternion value, out Quaternion result)
    {
        result.X = -value.X;
        result.Y = -value.Y;
        result.Z = -value.Z;
        result.W = value.W;
    }

    /// <summary>
    /// Conjugates a quaternion.
    /// </summary>
    /// <param name="value">The quaternion to conjugate.</param>
    /// <returns>The conjugated quaternion.</returns>
    public static Quaternion Conjugate(in Quaternion value)
    {
        return new Quaternion( -value.X, -value.Y, -value.Z, value.W );
    }

    /// <summary>
    /// Calculates the dot product of two quaternions.
    /// </summary>
    /// <param name="left">First source quaternion.</param>
    /// <param name="right">Second source quaternion.</param>
    /// <param name="result">When the method completes, contains the dot product of the two quaternions.</param>
    public static void Dot(ref readonly Quaternion left, ref readonly Quaternion right, out float result)
    {
        result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
    }

    /// <summary>
    /// Calculates the dot product of two quaternions.
    /// </summary>
    /// <param name="left">First source quaternion.</param>
    /// <param name="right">Second source quaternion.</param>
    /// <returns>The dot product of the two quaternions.</returns>
    public static float Dot(in Quaternion left, in Quaternion right)
    {
        return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
    }

    /// <summary>
    /// Returns the absolute angle in radians between <paramref name="a"/> and <paramref name="b"/>
    /// </summary>
    public static float AngleBetween(in Quaternion a, in Quaternion b)
    {
        return Acos(Min(Abs(Dot(a, b)), 1f)) * 2f;
    }

    /// <summary>
    /// Exponentiates a quaternion.
    /// </summary>
    /// <param name="value">The quaternion to exponentiate.</param>
    /// <param name="result">When the method completes, contains the exponentiated quaternion.</param>
    public static void Exponential(ref readonly Quaternion value, out Quaternion result)
    {
        float angle = MathF.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
        float sin = MathF.Sin(angle);

        if (MathF.Abs(sin) >= MathUtil.ZeroTolerance)
        {
            float coeff = sin / angle;
            result.X = coeff * value.X;
            result.Y = coeff * value.Y;
            result.Z = coeff * value.Z;
        }
        else
        {
            result = value;
        }

        result.W = MathF.Cos(angle);
    }

    /// <summary>
    /// Exponentiates a quaternion.
    /// </summary>
    /// <param name="value">The quaternion to exponentiate.</param>
    /// <returns>The exponentiated quaternion.</returns>
    public static Quaternion Exponential(Quaternion value)
    {
        Exponential(ref value, out var result);
        return result;
    }

    /// <summary>
    /// Conjugates and renormalizes the quaternion.
    /// </summary>
    /// <param name="value">The quaternion to conjugate and renormalize.</param>
    /// <param name="result">When the method completes, contains the conjugated and renormalized quaternion.</param>
    public static void Invert(ref readonly Quaternion value, out Quaternion result)
    {
        result = value;
        result.Invert();
    }

    /// <summary>
    /// Conjugates and renormalizes the quaternion.
    /// </summary>
    /// <param name="value">The quaternion to conjugate and renormalize.</param>
    /// <returns>The conjugated and renormalized quaternion.</returns>
    public static Quaternion Invert(Quaternion value)
    {
        Invert(ref value, out var result);
        return result;
    }

    /// <summary>
    /// Performs a linear interpolation between two quaternions.
    /// </summary>
    /// <param name="start">Start quaternion.</param>
    /// <param name="end">End quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the linear interpolation of the two quaternions.</param>
    /// <remarks>
    /// This method performs the linear interpolation based on the following formula.
    /// <code>start + (end - start) * amount</code>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
    /// </remarks>
    public static void Lerp(ref readonly Quaternion start, ref readonly Quaternion end, float amount, out Quaternion result)
    {
        float inverse = 1.0f - amount;

        if (Dot(start, end) >= 0.0f)
        {
            result.X = (inverse * start.X) + (amount * end.X);
            result.Y = (inverse * start.Y) + (amount * end.Y);
            result.Z = (inverse * start.Z) + (amount * end.Z);
            result.W = (inverse * start.W) + (amount * end.W);
        }
        else
        {
            result.X = (inverse * start.X) - (amount * end.X);
            result.Y = (inverse * start.Y) - (amount * end.Y);
            result.Z = (inverse * start.Z) - (amount * end.Z);
            result.W = (inverse * start.W) - (amount * end.W);
        }

        result.Normalize();
    }

    /// <summary>
    /// Performs a linear interpolation between two quaternion.
    /// </summary>
    /// <param name="start">Start quaternion.</param>
    /// <param name="end">End quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The linear interpolation of the two quaternions.</returns>
    /// <remarks>
    /// This method performs the linear interpolation based on the following formula.
    /// <code>start + (end - start) * amount</code>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
    /// </remarks>
    public static Quaternion Lerp(Quaternion start, Quaternion end, float amount)
    {
        Lerp(ref start, ref end, amount, out var result);
        return result;
    }

    /// <summary>
    /// Returns a rotation whose facing direction points towards <paramref name="forward"/>
    /// and whose up direction points as close as possible to <paramref name="up"/>.
    /// </summary>
    public static Quaternion LookRotation(in Vector3 forward, in Vector3 up)
    {
        var right = Vector3.Normalize(Vector3.Cross(up, forward));
        var orthoUp = Vector3.Cross(forward, right);
        var m = new Matrix
        {
            M11 = right.X, M12 = right.Y, M13 = right.Z,
            M21 = orthoUp.X, M22 = orthoUp.Y, M23 = orthoUp.Z,
            M31 = forward.X, M32 = forward.Y, M33 = forward.Z,
        };
        RotationMatrix(ref m, out var lQuaternion);
        return lQuaternion;
    }

    /// <summary>
    /// Calculates the natural logarithm of the specified quaternion.
    /// </summary>
    /// <param name="value">The quaternion whose logarithm will be calculated.</param>
    /// <param name="result">When the method completes, contains the natural logarithm of the quaternion.</param>
    public static void Logarithm(ref readonly Quaternion value, out Quaternion result)
    {
        if (MathF.Abs(value.W) < 1.0f)
        {
            float angle = MathF.Acos(value.W);
            float sin = MathF.Sin(angle);

            if (MathF.Abs(sin) >= MathUtil.ZeroTolerance)
            {
                float coeff = angle / sin;
                result.X = value.X * coeff;
                result.Y = value.Y * coeff;
                result.Z = value.Z * coeff;
            }
            else
            {
                result = value;
            }
        }
        else
        {
            result = value;
        }

        result.W = 0.0f;
    }

    /// <summary>
    /// Calculates the natural logarithm of the specified quaternion.
    /// </summary>
    /// <param name="value">The quaternion whose logarithm will be calculated.</param>
    /// <returns>The natural logarithm of the quaternion.</returns>
    public static Quaternion Logarithm(Quaternion value)
    {
        Logarithm(ref value, out var result);
        return result;
    }

    /// <summary>
    /// Converts the quaternion into a unit quaternion.
    /// </summary>
    /// <param name="value">The quaternion to normalize.</param>
    /// <param name="result">When the method completes, contains the normalized quaternion.</param>
    public static void Normalize(ref readonly Quaternion value, out Quaternion result)
    {
        Quaternion temp = value;
        result = temp;
        result.Normalize();
    }

    /// <summary>
    /// Converts the quaternion into a unit quaternion.
    /// </summary>
    /// <param name="value">The quaternion to normalize.</param>
    /// <returns>The normalized quaternion.</returns>
    public static Quaternion Normalize(Quaternion value)
    {
        value.Normalize();
        return value;
    }

    /// <summary>
    /// Rotates a Vector3 by the specified quaternion rotation.
    /// </summary>
    /// <param name="vector">The vector to rotate.</param>
    public readonly void Rotate(ref Vector3 vector)
    {
        var pureQuaternion = new Quaternion(vector, 0);
        pureQuaternion = Conjugate(this) * pureQuaternion * this;

        vector.X = pureQuaternion.X;
        vector.Y = pureQuaternion.Y;
        vector.Z = pureQuaternion.Z;
    }

    /// <summary>
    /// Creates a quaternion given a rotation and an axis.
    /// </summary>
    /// <param name="axis">The axis of rotation.</param>
    /// <param name="angle">The angle of rotation.</param>
    /// <param name="result">When the method completes, contains the newly created quaternion.</param>
    public static void RotationAxis(ref readonly Vector3 axis, float angle, out Quaternion result)
    {
        Vector3.Normalize(in axis, out var normalized);

        float half = angle * 0.5f;
        float sin = MathF.Sin(half);
        float cos = MathF.Cos(half);

        result.X = normalized.X * sin;
        result.Y = normalized.Y * sin;
        result.Z = normalized.Z * sin;
        result.W = cos;
    }

    /// <summary>
    /// Creates a quaternion given a rotation and an axis.
    /// </summary>
    /// <param name="axis">The axis of rotation.</param>
    /// <param name="angle">The angle of rotation.</param>
    /// <returns>The newly created quaternion.</returns>
    public static Quaternion RotationAxis(Vector3 axis, float angle)
    {
        RotationAxis(ref axis, angle, out var result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion given a rotation matrix.
    /// </summary>
    /// <param name="matrix">The rotation matrix.</param>
    /// <param name="result">When the method completes, contains the newly created quaternion.</param>
    public static void RotationMatrix(ref readonly Matrix matrix, out Quaternion result)
    {
        float sqrt;
        float half;
        float scale = matrix.M11 + matrix.M22 + matrix.M33;

        if (scale > 0.0f)
        {
            sqrt = MathF.Sqrt(scale + 1.0f);
            result.W = sqrt * 0.5f;
            sqrt = 0.5f / sqrt;

            result.X = (matrix.M23 - matrix.M32) * sqrt;
            result.Y = (matrix.M31 - matrix.M13) * sqrt;
            result.Z = (matrix.M12 - matrix.M21) * sqrt;
        }
        else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
        {
            sqrt = MathF.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
            half = 0.5f / sqrt;

            result.X = 0.5f * sqrt;
            result.Y = (matrix.M12 + matrix.M21) * half;
            result.Z = (matrix.M13 + matrix.M31) * half;
            result.W = (matrix.M23 - matrix.M32) * half;
        }
        else if (matrix.M22 > matrix.M33)
        {
            sqrt = MathF.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
            half = 0.5f / sqrt;

            result.X = (matrix.M21 + matrix.M12) * half;
            result.Y = 0.5f * sqrt;
            result.Z = (matrix.M32 + matrix.M23) * half;
            result.W = (matrix.M31 - matrix.M13) * half;
        }
        else
        {
            sqrt = MathF.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
            half = 0.5f / sqrt;

            result.X = (matrix.M31 + matrix.M13) * half;
            result.Y = (matrix.M32 + matrix.M23) * half;
            result.Z = 0.5f * sqrt;
            result.W = (matrix.M12 - matrix.M21) * half;
        }
    }

    /// <summary>
    /// Creates a quaternion given a rotation matrix.
    /// </summary>
    /// <param name="matrix">The rotation matrix.</param>
    /// <returns>The newly created quaternion.</returns>
    public static Quaternion RotationMatrix(Matrix matrix)
    {
        RotationMatrix(ref matrix, out var result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion that rotates around the x-axis.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <param name="result">When the method completes, contains the newly created quaternion.</param>
    public static void RotationX(float angle, out Quaternion result)
    {
        float halfAngle = angle * 0.5f;
        result = new Quaternion(MathF.Sin(halfAngle), 0.0f, 0.0f, MathF.Cos(halfAngle));
    }

    /// <summary>
    /// Creates a quaternion that rotates around the x-axis.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <returns>The created rotation quaternion.</returns>
    public static Quaternion RotationX(float angle)
    {
        RotationX(angle, out var result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion that rotates around the y-axis.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <param name="result">When the method completes, contains the newly created quaternion.</param>
    public static void RotationY(float angle, out Quaternion result)
    {
        float halfAngle = angle * 0.5f;
        result = new Quaternion(0.0f, MathF.Sin(halfAngle), 0.0f, MathF.Cos(halfAngle));
    }

    /// <summary>
    /// Creates a quaternion that rotates around the y-axis.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <returns>The created rotation quaternion.</returns>
    public static Quaternion RotationY(float angle)
    {
        RotationY(angle, out var result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion that rotates around the z-axis.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <param name="result">When the method completes, contains the newly created quaternion.</param>
    public static void RotationZ(float angle, out Quaternion result)
    {
        float halfAngle = angle * 0.5f;
        result = new Quaternion(0.0f, 0.0f, MathF.Sin(halfAngle), MathF.Cos(halfAngle));
    }

    /// <summary>
    /// Creates a quaternion that rotates around the z-axis.
    /// </summary>
    /// <param name="angle">Angle of rotation in radians.</param>
    /// <returns>The created rotation quaternion.</returns>
    public static Quaternion RotationZ(float angle)
    {
        RotationZ(angle, out var result);
        return result;
    }

    /// <summary>
    /// Calculate the yaw/pitch/roll rotation equivalent to the provided quaternion.
    /// </summary>
    /// <param name="rotation">The input quaternion</param>
    /// <param name="yaw">The yaw component in radians.</param>
    /// <param name="pitch">The pitch component in radians.</param>
    /// <param name="roll">The roll component in radians.</param>
    public static void RotationYawPitchRoll(ref readonly Quaternion rotation, out float yaw, out float pitch, out float roll)
    {
        // Equivalent to:
        //  Matrix rotationMatrix;
        //  Matrix.Rotation(ref cachedRotation, out rotationMatrix);
        //  rotationMatrix.Decompose(out float yaw, out float pitch, out float roll);

        var xx = rotation.X * rotation.X;
        var yy = rotation.Y * rotation.Y;
        var zz = rotation.Z * rotation.Z;
        var xy = rotation.X * rotation.Y;
        var zw = rotation.Z * rotation.W;
        var zx = rotation.Z * rotation.X;
        var yw = rotation.Y * rotation.W;
        var yz = rotation.Y * rotation.Z;
        var xw = rotation.X * rotation.W;

        var M11 = 1.0f - (2.0f * (yy + zz));
        var M12 = 2.0f * (xy + zw);
        //var M13 = 2.0f * (zx - yw);
        var M21 = 2.0f * (xy - zw);
        var M22 = 1.0f - (2.0f * (zz + xx));
        //var M23 = 2.0f * (yz + xw);
        var M31 = 2.0f * (zx + yw);
        var M32 = 2.0f * (yz - xw);
        var M33 = 1.0f - (2.0f * (yy + xx));

        /*** Refer to Matrix.Decompose(out float yaw, out float pitch, out float roll) for code and license ***/
        if (MathUtil.IsOne(Math.Abs(M32)))
        {
            if (M32 >= 0)
            {
                // Edge case where M32 == +1
                pitch = -MathUtil.PiOverTwo;
                yaw = MathF.Atan2(-M21, M11);
                roll = 0;
            }
            else
            {
                // Edge case where M32 == -1
                pitch = MathUtil.PiOverTwo;
                yaw = -MathF.Atan2(-M21, M11);
                roll = 0;
            }
        }
        else
        {
            // Common case
            pitch = MathF.Asin(-M32);
            yaw = MathF.Atan2(M31, M33);
            roll = MathF.Atan2(M12, M22);
        }
    }

    /// <summary>
    /// Creates a quaternion given a yaw, pitch, and roll value (angles in radians).
    /// </summary>
    /// <param name="yaw">The yaw of rotation in radians.</param>
    /// <param name="pitch">The pitch of rotation in radians.</param>
    /// <param name="roll">The roll of rotation in radians.</param>
    /// <param name="result">When the method completes, contains the newly created quaternion.</param>
    public static void RotationYawPitchRoll(float yaw, float pitch, float roll, out Quaternion result)
    {
        var halfRoll = roll * 0.5f;
        var halfPitch = pitch * 0.5f;
        var halfYaw = yaw * 0.5f;

        var sinRoll = MathF.Sin(halfRoll);
        var cosRoll = MathF.Cos(halfRoll);
        var sinPitch = MathF.Sin(halfPitch);
        var cosPitch = MathF.Cos(halfPitch);
        var sinYaw = MathF.Sin(halfYaw);
        var cosYaw = MathF.Cos(halfYaw);

        var cosYawPitch = cosYaw * cosPitch;
        var sinYawPitch = sinYaw * sinPitch;

        result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
        result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
        result.Z = (cosYawPitch * sinRoll) - (sinYawPitch * cosRoll);
        result.W = (cosYawPitch * cosRoll) + (sinYawPitch * sinRoll);
    }

    /// <summary>
    /// Creates a quaternion given a yaw, pitch, and roll value (angles in radians).
    /// </summary>
    /// <param name="yaw">The yaw of rotation in radians.</param>
    /// <param name="pitch">The pitch of rotation in radians.</param>
    /// <param name="roll">The roll of rotation in radians.</param>
    /// <returns>The newly created quaternion.</returns>
    public static Quaternion RotationYawPitchRoll(float yaw, float pitch, float roll)
    {
        RotationYawPitchRoll(yaw, pitch, roll, out var result);
        return result;
    }

    /// <summary>
    /// Computes a quaternion corresponding to the rotation transforming the vector <paramref name="source"/> to the vector <paramref name="target"/>.
    /// </summary>
    /// <param name="source">The source vector of the transformation.</param>
    /// <param name="target">The target vector of the transformation.</param>
    /// <returns>The resulting quaternion corresponding to the transformation of the source vector to the target vector.</returns>
    public static Quaternion BetweenDirections(Vector3 source, Vector3 target)
    {
        BetweenDirections(ref source, ref target, out var result);
        return result;
    }

    /// <summary>
    /// Computes a quaternion corresponding to the rotation transforming the vector <paramref name="source"/> to the vector <paramref name="target"/>.
    /// </summary>
    /// <param name="source">The source vector of the transformation.</param>
    /// <param name="target">The target vector of the transformation.</param>
    /// <param name="result">The resulting quaternion corresponding to the transformation of the source vector to the target vector.</param>
    public static void BetweenDirections(ref readonly Vector3 source, ref readonly Vector3 target, out Quaternion result)
    {
        var norms = MathF.Sqrt(source.LengthSquared() * target.LengthSquared());
        var real = norms + Vector3.Dot(source, target);
        if (real < MathUtil.ZeroTolerance * norms)
        {
            // If source and target are exactly opposite, rotate 180 degrees around an arbitrary orthogonal axis.
            // Axis normalisation can happen later, when we normalise the quaternion.
            result = MathF.Abs(source.X) > MathF.Abs(source.Z)
                ? new Quaternion(-source.Y, source.X, 0.0f, 0.0f)
                : new Quaternion(0.0f, -source.Z, source.Y, 0.0f);
        }
        else
        {
            // Otherwise, build quaternion the standard way.
            var axis = Vector3.Cross(source, target);
            result = new Quaternion(axis, real);
        }
        result.Normalize();
    }

    /// <summary>
    /// Interpolates between two quaternions, using spherical linear interpolation.
    /// </summary>
    /// <param name="start">Start quaternion.</param>
    /// <param name="end">End quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the spherical linear interpolation of the two quaternions.</param>
    public static void Slerp(ref readonly Quaternion start, ref readonly Quaternion end, float amount, out Quaternion result)
    {
        float opposite;
        float inverse;
        float dot = Dot(start, end);

        if (MathF.Abs(dot) > 1.0f - MathUtil.ZeroTolerance)
        {
            inverse = 1.0f - amount;
            opposite = amount * MathF.Sign(dot);
        }
        else
        {
            float acos = MathF.Acos(MathF.Abs(dot));
            float invSin = 1.0f / MathF.Sin(acos);

            inverse = MathF.Sin((1.0f - amount) * acos) * invSin;
            opposite = MathF.Sin(amount * acos) * invSin * MathF.Sign(dot);
        }

        result.X = (inverse * start.X) + (opposite * end.X);
        result.Y = (inverse * start.Y) + (opposite * end.Y);
        result.Z = (inverse * start.Z) + (opposite * end.Z);
        result.W = (inverse * start.W) + (opposite * end.W);
    }

    /// <summary>
    /// Interpolates between two quaternions, using spherical linear interpolation.
    /// </summary>
    /// <param name="start">Start quaternion.</param>
    /// <param name="end">End quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The spherical linear interpolation of the two quaternions.</returns>
    public static Quaternion Slerp(in Quaternion start, in Quaternion end, float amount)
    {
        float opposite;
        float inverse;
        float dot = Dot(start, end);

        if (Abs(dot) > 1.0f - MathUtil.ZeroTolerance)
        {
            inverse = 1.0f - amount;
            opposite = amount * Sign(dot);
        }
        else
        {
            float acos = Acos(Abs(dot));
            float invSin = 1.0f / Sin(acos);

            inverse = Sin((1.0f - amount) * acos) * invSin;
            opposite = Sin(amount * acos) * invSin * Sign(dot);
        }

        Quaternion result;
        result.X = (inverse * start.X) + (opposite * end.X);
        result.Y = (inverse * start.Y) + (opposite * end.Y);
        result.Z = (inverse * start.Z) + (opposite * end.Z);
        result.W = (inverse * start.W) + (opposite * end.W);
        return result;
    }

    /// <summary>
    /// Rotate <paramref name="current"/> towards <paramref name="target"/> by <paramref name="angle"/>.
    /// </summary>
    /// <remarks>
    /// When the angle difference between <paramref name="current"/> and <paramref name="target"/> is less than
    /// the given <paramref name="angle"/>, returns <paramref name="target"/> instead of overshooting past it.
    /// </remarks>
    public static Quaternion RotateTowards(in Quaternion current, in Quaternion target, float angle)
    {
        var maxAngle = AngleBetween(current, target);
        return maxAngle == 0f ? target : Slerp(current, target, Min(1f, angle / maxAngle));
    }

    /// <summary>
    /// Interpolates between quaternions, using spherical quadrangle interpolation.
    /// </summary>
    /// <param name="value1">First source quaternion.</param>
    /// <param name="value2">Second source quaternion.</param>
    /// <param name="value3">Thrid source quaternion.</param>
    /// <param name="value4">Fourth source quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
    /// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the quaternions.</param>
    public static void Squad(ref readonly Quaternion value1, ref readonly Quaternion value2, ref readonly Quaternion value3, ref readonly Quaternion value4, float amount, out Quaternion result)
    {
        Slerp(in value1, in value4, amount, out var start);
        Slerp(in value2, in value3, amount, out var end);
        Slerp(ref start, ref end, 2.0f * amount * (1.0f - amount), out result);
    }

    /// <summary>
    /// Interpolates between quaternions, using spherical quadrangle interpolation.
    /// </summary>
    /// <param name="value1">First source quaternion.</param>
    /// <param name="value2">Second source quaternion.</param>
    /// <param name="value3">Thrid source quaternion.</param>
    /// <param name="value4">Fourth source quaternion.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
    /// <returns>The spherical quadrangle interpolation of the quaternions.</returns>
    public static Quaternion Squad(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4, float amount)
    {
        Squad(ref value1, ref value2, ref value3, ref value4, amount, out var result);
        return result;
    }

    /// <summary>
    /// Sets up control points for spherical quadrangle interpolation.
    /// </summary>
    /// <param name="value1">First source quaternion.</param>
    /// <param name="value2">Second source quaternion.</param>
    /// <param name="value3">Third source quaternion.</param>
    /// <param name="value4">Fourth source quaternion.</param>
    /// <returns>An array of three quaternions that represent control points for spherical quadrangle interpolation.</returns>
    public static Quaternion[] SquadSetup(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4)
    {
        Quaternion q0 = (value1 + value2).LengthSquared() < (value1 - value2).LengthSquared() ? -value1 : value1;
        Quaternion q2 = (value2 + value3).LengthSquared() < (value2 - value3).LengthSquared() ? -value3 : value3;
        Quaternion q3 = (value3 + value4).LengthSquared() < (value3 - value4).LengthSquared() ? -value4 : value4;
        Quaternion q1 = value2;

        Exponential(ref q1, out var q1Exp);
        Exponential(ref q2, out var q2Exp);

        Quaternion[] results = new Quaternion[3];
        results[0] = q1 * Exponential(-0.25f * (Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)));
        results[1] = q2 * Exponential(-0.25f * (Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)));
        results[2] = q2;

        return results;
    }

    /// <summary>
    /// Adds two quaternions.
    /// </summary>
    /// <param name="left">The first quaternion to add.</param>
    /// <param name="right">The second quaternion to add.</param>
    /// <returns>The sum of the two quaternions.</returns>
    public static Quaternion operator +(in Quaternion left, in Quaternion right)
    {
        Add(in left, in right, out var result);
        return result;
    }

    /// <summary>
    /// Subtracts two quaternions.
    /// </summary>
    /// <param name="left">The first quaternion to subtract.</param>
    /// <param name="right">The second quaternion to subtract.</param>
    /// <returns>The difference of the two quaternions.</returns>
    public static Quaternion operator -(in Quaternion left, in Quaternion right)
    {
        Subtract(in left, in right, out var result);
        return result;
    }

    /// <summary>
    /// Reverses the direction of a given quaternion.
    /// </summary>
    /// <param name="value">The quaternion to negate.</param>
    /// <returns>A quaternion facing in the opposite direction.</returns>
    public static Quaternion operator -(in Quaternion value)
    {
        Negate(in value, out var result);
        return result;
    }

    /// <summary>
    /// Scales a quaternion by the given value.
    /// </summary>
    /// <param name="value">The quaternion to scale.</param>
    /// <param name="scale">The amount by which to scale the quaternion.</param>
    /// <returns>The scaled quaternion.</returns>
    public static Quaternion operator *(float scale, in Quaternion value)
    {
        Multiply(in value, scale, out var result);
        return result;
    }

    /// <summary>
    /// Scales a quaternion by the given value.
    /// </summary>
    /// <param name="value">The quaternion to scale.</param>
    /// <param name="scale">The amount by which to scale the quaternion.</param>
    /// <returns>The scaled quaternion.</returns>
    public static Quaternion operator *(in Quaternion value, float scale)
    {
        Multiply(in value, scale, out var result);
        return result;
    }

    /// <summary>
    /// Multiplies a quaternion by another.
    /// </summary>
    /// <param name="left">The first quaternion to multiply.</param>
    /// <param name="right">The second quaternion to multiply.</param>
    /// <returns>The multiplied quaternion.</returns>
    public static Quaternion operator *(in Quaternion left, in Quaternion right)
    {
        Multiply(in left, in right, out var result);
        return result;
    }

    /// <summary>
    /// Return the vector rotated by the quaternion.
    /// </summary>
    /// <remarks>
    /// Shorthand for <see cref="Rotate"/>
    /// </remarks>
    public static Vector3 operator *(in Quaternion left, in Vector3 right)
    {
        var pureQuaternion = new Quaternion(right, 0);
        pureQuaternion = Conjugate(left) * pureQuaternion * left;
        return new Vector3(pureQuaternion.X, pureQuaternion.Y, pureQuaternion.Z);
    }

    /// <summary>
    /// Tests for equality between two objects.
    /// </summary>
    /// <remarks> Comparison is not strict, a difference of <see cref="MathUtil.ZeroTolerance"/> will return as equal. </remarks>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(in Quaternion left, in Quaternion right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Tests for inequality between two objects.
    /// </summary>
    /// <remarks> Comparison is not strict, a difference of <see cref="MathUtil.ZeroTolerance"/> will return as equal. </remarks>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(in Quaternion left, in Quaternion right)
    {
        return !left.Equals(right);
    }
    
    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override readonly string ToString() => $"{this}";

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
    {
        var handler = new DefaultInterpolatedStringHandler(11, 4, formatProvider);
        handler.AppendLiteral("X:");
        handler.AppendFormatted(X, format);
        handler.AppendLiteral(" Y:");
        handler.AppendFormatted(Y, format);
        handler.AppendLiteral(" Z:");
        handler.AppendFormatted(Z, format);
        handler.AppendLiteral(" W:");
        handler.AppendFormatted(W, format);
        return handler.ToStringAndClear();
    }

    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var format1 = format.Length > 0 ? format.ToString() : null;
        var handler = new MemoryExtensions.TryWriteInterpolatedStringHandler(11, 4, destination, provider, out _);
        handler.AppendLiteral("X:");
        handler.AppendFormatted(X, format1);
        handler.AppendLiteral(" Y:");
        handler.AppendFormatted(Y, format1);
        handler.AppendLiteral(" Z:");
        handler.AppendFormatted(Z, format1);
        handler.AppendLiteral(" W:");
        handler.AppendFormatted(W, format1);
        return destination.TryWrite(ref handler, out charsWritten);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    /// <summary>
    /// Determines whether the specified <see cref="Quaternion"/> is exactly equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Quaternion"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="Quaternion"/> is exactly equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public bool EqualsStrict(Quaternion other)
    {
        return other.X == this.X && other.Y == this.Y && other.Z == this.Z && other.W == this.W;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Stride.Core.Mathematics.Quaternion"/> is within <see cref="MathUtil.ZeroTolerance"/> for equality to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Stride.Core.Mathematics.Quaternion"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="Stride.Core.Mathematics.Quaternion"/> is equal or almost equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Equals(Quaternion other)
    {
        return (Abs(other.X - X) < MathUtil.ZeroTolerance &&
            Abs(other.Y - Y) < MathUtil.ZeroTolerance &&
            Abs(other.Z - Z) < MathUtil.ZeroTolerance &&
            Abs(other.W - W) < MathUtil.ZeroTolerance);
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is within <see cref="MathUtil.ZeroTolerance"/> for equality to this instance.
    /// </summary>
    /// <param name="value">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="object"/> is equal or almost equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override readonly bool Equals(object? value)
    {
        return value is Quaternion q && Equals(q);
    }

    /// <summary>
    /// Casts from System.Numerics to Stride.Maths vectors
    /// </summary>
    /// <param name="v">Value to cast</param>
    public static implicit operator Quaternion(System.Numerics.Quaternion v)
    {
        return Unsafe.BitCast<System.Numerics.Quaternion, Quaternion>(v);
    }

    /// <summary>
    /// Casts from Stride.Maths to System.Numerics vectors
    /// </summary>
    /// <param name="v">Value to cast</param>
    public static implicit operator System.Numerics.Quaternion(Quaternion v)
    {
        return Unsafe.BitCast<Quaternion, System.Numerics.Quaternion>(v);
    }

#if SlimDX1xInterop
    /// <summary>
    /// Performs an implicit conversion from <see cref="Stride.Core.Mathematics.Quaternion"/> to <see cref="SlimDX.Quaternion"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator SlimDX.Quaternion(Quaternion value)
    {
        return new SlimDX.Quaternion(value.X, value.Y, value.Z, value.W);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="SlimDX.Quaternion"/> to <see cref="Stride.Core.Mathematics.Quaternion"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Quaternion(SlimDX.Quaternion value)
    {
        return new Quaternion(value.X, value.Y, value.Z, value.W);
    }
#endif

#if WPFInterop
    /// <summary>
    /// Performs an implicit conversion from <see cref="Stride.Core.Mathematics.Quaternion"/> to <see cref="System.Windows.Media.Media3D.Quaternion"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator System.Windows.Media.Media3D.Quaternion(Quaternion value)
    {
        return new System.Windows.Media.Media3D.Quaternion(value.X, value.Y, value.Z, value.W);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="System.Windows.Media.Media3D.Quaternion"/> to <see cref="Stride.Core.Mathematics.Quaternion"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Quaternion(System.Windows.Media.Media3D.Quaternion value)
    {
        return new Quaternion((float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
    }
#endif

#if XnaInterop
    /// <summary>
    /// Performs an implicit conversion from <see cref="Stride.Core.Mathematics.Quaternion"/> to <see cref="Microsoft.Xna.Framework.Quaternion"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Microsoft.Xna.Framework.Quaternion(Quaternion value)
    {
        return new Microsoft.Xna.Framework.Quaternion(value.X, value.Y, value.Z, value.W);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Microsoft.Xna.Framework.Quaternion"/> to <see cref="Stride.Core.Mathematics.Quaternion"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Quaternion(Microsoft.Xna.Framework.Quaternion value)
    {
        return new Quaternion(value.X, value.Y, value.Z, value.W);
    }
#endif
}
