// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using Stride.Core.Assets.Visitors;
using Stride.Core.Reflection;
using Stride.Core.Yaml;

namespace Stride.Core.Assets;

/// <summary>
/// Removes objects implementing 'IYamlProxy' from the object.
/// </summary>
public class UnloadableObjectRemover : AssetVisitorBase
{
    private List<UnloadableItem> unloadableitems;
    [ThreadStatic] private static UnloadableObjectRemover instanceTLS;

    public static IReadOnlyList<UnloadableItem> Run(object obj)
    {
        var instance = GetYamlProxyRemover();

        instance.DiscoverInternal(obj);

        // We apply changes in opposite visit order so that indices remains valid when we remove objects while iterating
        for (int index = instance.unloadableitems.Count - 1; index >= 0; index--)
        {
            var unloadableItem = instance.unloadableitems[index];
            unloadableItem.MemberPath.Apply(obj, MemberPathAction.ValueClear, null);
        }

        return instance.unloadableitems;
    }

    public static IReadOnlyList<UnloadableItem> Discover(object obj)
    {
        var instance = GetYamlProxyRemover();
        instance.DiscoverInternal(obj);
        return instance.unloadableitems;
    }

    private static UnloadableObjectRemover GetYamlProxyRemover()
    {
        var yamlProxyRemover = instanceTLS;
        if (yamlProxyRemover == null)
        {
            instanceTLS = yamlProxyRemover = new UnloadableObjectRemover();
        }
        return yamlProxyRemover;
    }

    private void DiscoverInternal(object obj)
    {
        Reset();
        unloadableitems = [];
        Visit(obj);
    }

    public override void VisitCollectionItem(IEnumerable collection, CollectionDescriptor descriptor, int index, object? item, ITypeDescriptor? itemDescriptor)
    {
        if (ProcessObject(item)) return;

        base.VisitCollectionItem(collection, descriptor, index, item, itemDescriptor);
    }

    public override void VisitArrayItem(Array array, ArrayDescriptor descriptor, int index, object? item, ITypeDescriptor? itemDescriptor)
    {
        if (ProcessObject(item)) return;

        base.VisitArrayItem(array, descriptor, index, item, itemDescriptor);
    }

    public override void VisitObjectMember(object container, ObjectDescriptor containerDescriptor, IMemberDescriptor member, object? value)
    {
        if (ProcessObject(value)) return;

        base.VisitObjectMember(container, containerDescriptor, member, value);
    }

    public override void VisitDictionaryKeyValue(object dictionary, DictionaryDescriptor descriptor, object key, ITypeDescriptor? keyDescriptor, object? value, ITypeDescriptor? valueDescriptor)
    {
        // TODO: CurrentPath is valid only for value, not key
        //if (ProcessObject(key, keyDescriptor.Type)) key = null;
        if (ProcessObject(value)) return;

        Visit(value, valueDescriptor);
        //base.VisitDictionaryKeyValue(dictionary, descriptor, key, keyDescriptor, value, valueDescriptor);
    }

    public override void VisitSetItem(IEnumerable set, SetDescriptor descriptor, object? item, ITypeDescriptor? itemDescriptor)
    {
        if (ProcessObject(item)) return;

        Visit(item, itemDescriptor);
    }

    private bool ProcessObject(object? obj)
    {
        if (obj is IUnloadable unloadable)
        {
            unloadableitems.Add(new UnloadableItem(unloadable, CurrentPath.Clone()));

            // Don't recurse inside
            return true;
        }
        return false;
    }

    public readonly struct UnloadableItem
    {
        public readonly IUnloadable UnloadableObject;
        public readonly MemberPath MemberPath;

        public UnloadableItem(IUnloadable o, MemberPath memberPath)
        {
            UnloadableObject = o;
            MemberPath = memberPath;
        }
    }
}
