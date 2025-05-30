// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Assets.Quantum.Internal;
using Stride.Core.Assets.Yaml;
using Stride.Core.Annotations;
using Stride.Core.Reflection;
using Stride.Core.Quantum;

namespace Stride.Core.Assets.Quantum.Visitors;

/// <summary>
/// A visitor that collects metadata to pass to YAML serialization.
/// </summary>
public abstract class AssetNodeMetadataCollectorBase : GraphVisitorBase
{
    private int inNonIdentifiableType;

    /// <inheritdoc/>
    protected override void VisitNode(IGraphNode node)
    {
        var assetNode = (IAssetNode)node;

        var localInNonIdentifiableType = false;
        if ((node.Descriptor as ObjectDescriptor)?.Attributes.OfType<NonIdentifiableCollectionItemsAttribute>().Any() ?? false)
        {
            localInNonIdentifiableType = true;
            inNonIdentifiableType++;
        }

        if (assetNode is IAssetMemberNode memberNode)
        {
            VisitMemberNode(memberNode, inNonIdentifiableType);
        }
        if (assetNode is IAssetObjectNode objectNode)
        {
            VisitObjectNode(objectNode, inNonIdentifiableType);
        }
        base.VisitNode(node);

        if (localInNonIdentifiableType)
            inNonIdentifiableType--;
    }

    /// <summary>
    /// Visits a node that is an <see cref="IAssetMemberNode"/>.
    /// </summary>
    /// <param name="memberNode">The node to visit.</param>
    /// <param name="inNonIdentifiableType"></param>
    protected abstract void VisitMemberNode(IAssetMemberNode memberNode, int inNonIdentifiableType);

    /// <summary>
    /// Visits a node that is an <see cref="IAssetObjectNode"/>.
    /// </summary>
    /// <param name="objectNode">The node to visit.</param>
    /// <param name="inNonIdentifiableType"></param>
    protected abstract void VisitObjectNode(IAssetObjectNode objectNode, int inNonIdentifiableType);

    /// <summary>
    /// Converts the given <see cref="GraphNodePath"/> to a <see cref="YamlAssetPath"/> that can be processed by YAML serialization.
    /// </summary>
    /// <param name="path">The path to convert.</param>
    /// <param name="inNonIdentifiableType">If greater than zero, will ignore collection item ids and write indices instead.</param>
    /// <returns>An instance of <see cref="YamlAssetPath"/> corresponding to the given <paramref name="path"/>.</returns>
    public static YamlAssetPath ConvertPath(GraphNodePath path, int inNonIdentifiableType = 0)
    {
        ArgumentNullException.ThrowIfNull(path);
        var currentNode = (IAssetNode)path.RootNode;
        var result = new YamlAssetPath();
        var i = 0;
        foreach (var item in path.Path)
        {
            switch (item.Type)
            {
                case GraphNodePath.ElementType.Member:
                {
                    var member = item.Name;
                    result.PushMember(member);
                        if (currentNode is not IObjectNode objectNode) throw new InvalidOperationException($"An IObjectNode was expected when processing the path [{path}]");
                        currentNode = (IAssetNode?)objectNode.TryGetChild(member);
                    break;
                }
                case GraphNodePath.ElementType.Target:
                {
                    if (i < path.Path.Count - 1)
                    {
                            if (currentNode is not IMemberNode targetingMemberNode) throw new InvalidOperationException($"An IMemberNode was expected when processing the path [{path}]");
                            currentNode = (IAssetNode?)targetingMemberNode.Target;
                    }
                    break;
                }
                case GraphNodePath.ElementType.Index:
                {
                    var index = item.Index;
                        if (currentNode is not AssetObjectNode objectNode) throw new InvalidOperationException($"An IObjectNode was expected when processing the path [{path}]");
                        if (inNonIdentifiableType > 0 || !CollectionItemIdHelper.HasCollectionItemIds(objectNode.Retrieve()))
                    {
                        result.PushIndex(index.Value);
                    }
                    else
                    {
                        var id = objectNode.IndexToId(index);
                        // Create a new id if we don't have any so far
                        if (id == ItemId.Empty)
                            id = ItemId.New();
                        result.PushItemId(id);
                    }
                    if (i < path.Path.Count - 1)
                    {
                        currentNode = (IAssetNode?)objectNode.IndexedTarget(index);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ++i;
        }
        return result;
    }
}
