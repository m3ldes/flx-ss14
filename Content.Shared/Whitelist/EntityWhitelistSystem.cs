// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 plykiya <plykiya@protonmail.com>
// SPDX-FileCopyrightText: 2024 psykana <36602558+psykana@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 whateverusername0 <whateveremail>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Item;
using Content.Shared.Tag;

namespace Content.Shared.Whitelist;

public sealed class EntityWhitelistSystem : EntitySystem
{
    [Dependency] private readonly TagSystem _tag = default!;

    private EntityQuery<ItemComponent> _itemQuery;

    public override void Initialize()
    {
        base.Initialize();
        _itemQuery = GetEntityQuery<ItemComponent>();
    }

    /// <inheritdoc cref="IsValid(Content.Shared.Whitelist.EntityWhitelist,Robust.Shared.GameObjects.EntityUid)"/>
    public bool IsValid(EntityWhitelist list, [NotNullWhen(true)] EntityUid? uid)
    {
        return uid != null && IsValid(list, uid.Value);
    }

    /// <summary>
    /// Checks whether a given entity is allowed by a whitelist and not blocked by a blacklist.
    /// If a blacklist is provided and it matches then this returns false.
    /// If a whitelist is provided and it does not match then this returns false.
    /// If either list is null it does not get checked.
    /// </summary>
    public bool CheckBoth([NotNullWhen(true)] EntityUid? uid, EntityWhitelist? blacklist = null, EntityWhitelist? whitelist = null)
    {
        if (uid == null)
            return false;

        if (blacklist != null && IsValid(blacklist, uid))
            return false;

        return whitelist == null || IsValid(whitelist, uid);
    }

    /// <summary>
    /// Checks whether a given entity satisfies a whitelist.
    /// </summary>
    public bool IsValid(EntityWhitelist list, EntityUid uid)
    {
        if (list.Components != null)
        {
            if (list.Registrations == null)
            {
                var regs = StringsToRegs(list.Components);
                list.Registrations = new List<ComponentRegistration>();
                list.Registrations.AddRange(regs);
            }
        }

        if (list.Registrations != null && list.Registrations.Count > 0)
        {
            foreach (var reg in list.Registrations)
            {
                if (HasComp(uid, reg.Type))
                {
                    if (!list.RequireAll)
                        return true;
                }
                else if (list.RequireAll)
                    return false;
            }
        }

        if (list.Sizes != null && _itemQuery.TryComp(uid, out var itemComp))
        {
            if (list.Sizes.Contains(itemComp.Size))
                return true;
        }

        if (list.Tags != null)
        {
            return list.RequireAll
                ? _tag.HasAllTags(uid, list.Tags)
                : _tag.HasAnyTag(uid, list.Tags);
        }

        return list.RequireAll;
    }
    /// The following are a list of "helper functions" that are basically the same as each other
    /// to help make code that uses EntityWhitelist a bit more readable because at the moment
    /// it is quite clunky having to write out component.Whitelist == null ? true : _whitelist.IsValid(component.Whitelist, uid)
    /// several times in a row and makes comparisons easier to read

    /// <summary>
    /// Helper function to determine if Whitelist is not null and entity is on list
    /// </summary>
    public bool IsWhitelistPass(EntityWhitelist? whitelist, EntityUid uid)
    {
        if (whitelist == null)
            return false;

        return IsValid(whitelist, uid);
    }

    /// <summary>
    /// Helper function to determine if Whitelist is not null and entity is not on the list
    /// </summary>
    public bool IsWhitelistFail(EntityWhitelist? whitelist, EntityUid uid)
    {
        if (whitelist == null)
            return false;

        return !IsValid(whitelist, uid);
    }

    /// <summary>
    /// Helper function to determine if Whitelist is either null or the entity is on the list
    /// </summary>
    public bool IsWhitelistPassOrNull(EntityWhitelist? whitelist, EntityUid uid)
    {
        if (whitelist == null)
            return true;

        return IsValid(whitelist, uid);
    }

    /// <summary>
    /// Helper function to determine if Whitelist is either null or the entity is not on the list
    /// </summary>
    public bool IsWhitelistFailOrNull(EntityWhitelist? whitelist, EntityUid uid)
    {
        if (whitelist == null)
            return true;

        return !IsValid(whitelist, uid);
    }

    /// <summary>
    /// Helper function to determine if Blacklist is not null and entity is on list
    /// Duplicate of equivalent Whitelist function
    /// </summary>
    public bool IsBlacklistPass(EntityWhitelist? blacklist, EntityUid uid)
    {
        return IsWhitelistPass(blacklist, uid);
    }

    /// <summary>
    /// Helper function to determine if Blacklist is not null and entity is not on the list
    /// Duplicate of equivalent Whitelist function
    /// </summary>
    public bool IsBlacklistFail(EntityWhitelist? blacklist, EntityUid uid)
    {
        return IsWhitelistFail(blacklist, uid);
    }

    /// <summary>
    /// Helper function to determine if Blacklist is either null or the entity is on the list
    /// Duplicate of equivalent Whitelist function
    /// </summary>
    public bool IsBlacklistPassOrNull(EntityWhitelist? blacklist, EntityUid uid)
    {
        return IsWhitelistPassOrNull(blacklist, uid);
    }

    /// <summary>
    /// Helper function to determine if Blacklist is either null or the entity is not on the list
    /// Duplicate of equivalent Whitelist function
    /// </summary>
    public bool IsBlacklistFailOrNull(EntityWhitelist? blacklist, EntityUid uid)
    {
        return IsWhitelistFailOrNull(blacklist, uid);
    }

    private List<ComponentRegistration> StringsToRegs(string[]? input)
    {
        var list = new List<ComponentRegistration>();

        if (input == null || input.Length == 0)
            return list;

        foreach (var name in input)
        {
            var availability = Factory.GetComponentAvailability(name);
            if (Factory.TryGetRegistration(name, out var registration)
                && availability == ComponentAvailability.Available)
            {
                list.Add(registration);
            }
            else if (availability == ComponentAvailability.Unknown)
            {
                Log.Error($"StringsToRegs failed: Unknown component name {name} passed to EntityWhitelist!");
            }
        }

        return list;
    }
}