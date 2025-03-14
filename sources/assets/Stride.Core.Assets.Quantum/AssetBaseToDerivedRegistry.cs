﻿// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Quantum;

namespace Stride.Core.Assets.Quantum;

internal class AssetBaseToDerivedRegistry : IBaseToDerivedRegistry
{
    private readonly AssetPropertyGraph propertyGraph;
    private readonly Dictionary<IAssetNode, IAssetNode> baseToDerived = [];

    public AssetBaseToDerivedRegistry(AssetPropertyGraph propertyGraph)
    {
        this.propertyGraph = propertyGraph;
    }

    public void RegisterBaseToDerived(IAssetNode? baseNode, IAssetNode derivedNode)
    {
        var baseValue = baseNode?.Retrieve();
        if (baseValue == null)
            return;

        if (baseValue is IIdentifiable)
        {
            baseToDerived[baseNode!] = derivedNode;
            var baseMemberNode = baseNode as IAssetMemberNode;
            if (baseMemberNode?.Target is not null && !propertyGraph.Definition.IsMemberTargetObjectReference(baseMemberNode, baseValue))
            {
                baseToDerived[baseMemberNode.Target] = ((IAssetMemberNode)derivedNode).Target;
            }
        }

        var derivedObjectNode = derivedNode as IObjectNode;
        var baseObjectNode = baseNode as IObjectNode;
        if (derivedObjectNode?.ItemReferences is not null && baseObjectNode?.ItemReferences is not null)
        {
            foreach (var reference in derivedObjectNode.ItemReferences)
            {
                var target = propertyGraph.baseLinker.FindTargetReference(derivedNode, baseNode, reference);
                if (target == null)
                    continue;

                baseValue = target.TargetNode?.Retrieve();
                if (!propertyGraph.Definition.IsTargetItemObjectReference(baseObjectNode, target.Index, baseNode.Retrieve(target.Index)))
                {
                    if (baseValue is IIdentifiable)
                    {
                        baseToDerived[(IAssetNode)target.TargetNode] = (IAssetNode)derivedObjectNode.IndexedTarget(reference.Index);
                    }
                }
            }
        }
    }

    public IIdentifiable? ResolveFromBase(object? baseObjectReference, IAssetNode derivedReferencerNode)
    {
        ArgumentNullException.ThrowIfNull(derivedReferencerNode);
        if (baseObjectReference == null)
            return null;

        var baseNode = (IAssetNode)propertyGraph.Container.NodeContainer.GetNode(baseObjectReference);
        baseToDerived.TryGetValue(baseNode, out var derivedNode);
        return derivedNode?.Retrieve() as IIdentifiable;
    }
}
