// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Quantum;
using Stride.Core.Quantum.References;

namespace Stride.Core.Assets.Quantum.Visitors;

/// <summary>
/// A visitor that collects all <see cref="IIdentifiable"/> objects that are visited through nodes that are not representing object references.
/// </summary>
public class IdentifiableObjectCollector : AssetGraphVisitorBase
{
    private readonly Dictionary<Guid, IIdentifiable> result = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifiableObjectCollector"/> class.
    /// </summary>
    /// <param name="propertyGraphDefinition">The <see cref="AssetPropertyGraphDefinition"/> used to analyze object references.</param>
    private IdentifiableObjectCollector(AssetPropertyGraphDefinition propertyGraphDefinition)
        : base(propertyGraphDefinition)
    {
    }

    /// <summary>
    /// Collects the <see cref="IIdentifiable"/> objects that are visited through nodes that are not representing object references.
    /// </summary>
    /// <param name="propertyGraphDefinition">The <see cref="AssetPropertyGraphDefinition"/> used to analyze object references.</param>
    /// <param name="rootNode">The root object from which to collect. If null, <see cref="AssetPropertyGraph.RootNode"/> will be used.</param>
    /// <returns>A dictionary mapping <see cref="IIdentifiable"/> object by their identifier.</returns>
    public static Dictionary<Guid, IIdentifiable> Collect(AssetPropertyGraphDefinition propertyGraphDefinition, IGraphNode rootNode)
    {
        ArgumentNullException.ThrowIfNull(propertyGraphDefinition);
        ArgumentNullException.ThrowIfNull(rootNode);
        var visitor = new IdentifiableObjectCollector(propertyGraphDefinition);
        visitor.Visit(rootNode);
        return visitor.result;
    }

    /// <inheritdoc/>
    protected override void VisitReference(IGraphNode referencer, ObjectReference reference)
    {
        // Remark: VisitReference is invoked only when IsObjectReference returned false, therefore we will visit only 'real' object here, not references to them.
        if (reference.ObjectValue is IIdentifiable value)
        {
            result[value.Id] = value;
        }
        base.VisitReference(referencer, reference);
    }
}
