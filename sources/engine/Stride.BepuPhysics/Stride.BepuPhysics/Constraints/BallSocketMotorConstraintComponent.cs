// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using BepuPhysics.Constraints;
using Stride.BepuPhysics.Systems;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;

namespace Stride.BepuPhysics.Constraints;

[DataContract]
[DefaultEntityComponentProcessor(typeof(ConstraintProcessor), ExecutionMode = ExecutionMode.Runtime)]
[ComponentCategory("Physics - Bepu Constraint")]
public sealed class BallSocketMotorConstraintComponent : TwoBodyConstraintComponent<BallSocketMotor>, IMotor
{
    public BallSocketMotorConstraintComponent() => BepuConstraint = new()
    {
        Settings = new MotorSettings(1000, 10)
    };

    public Vector3 LocalOffsetB
    {
        get
        {
            return BepuConstraint.LocalOffsetB.ToStride();
        }
        set
        {
            BepuConstraint.LocalOffsetB = value.ToNumeric();
            TryUpdateDescription();
        }
    }

    public Vector3 TargetVelocityLocalA
    {
        get
        {
            return BepuConstraint.TargetVelocityLocalA.ToStride();
        }
        set
        {
            BepuConstraint.TargetVelocityLocalA = value.ToNumeric();
            TryUpdateDescription();
        }
    }

    /// <inheritdoc/>
    public float MotorDamping
    {
        get
        {
            return BepuConstraint.Settings.Damping;
        }
        set
        {
            BepuConstraint.Settings.Damping = value;
            TryUpdateDescription();
        }
    }

    /// <inheritdoc/>
    public float MotorMaximumForce
    {
        get
        {
            return BepuConstraint.Settings.MaximumForce;
        }
        set
        {
            BepuConstraint.Settings.MaximumForce = value;
            TryUpdateDescription();
        }
    }
}
