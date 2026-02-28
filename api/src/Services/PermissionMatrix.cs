using WinStudentGoalTracker.Models;

namespace WinStudentGoalTracker.Services;

public readonly record struct PermissionRule(bool Mine, bool Others);

public static class PermissionMatrix
{
    private static readonly PermissionRule Allow      = new(true, true);
    private static readonly PermissionRule MineOnly   = new(true, false);
    private static readonly PermissionRule Deny       = new(false, false);

    // _rules[role][entity][action] → PermissionRule
    private static readonly Dictionary<string, Dictionary<string, Dictionary<string, PermissionRule>>> _rules = new()
    {
        // ──────────────────────────────────────────────
        // Super Admin — full access to everything
        // ──────────────────────────────────────────────
        [UserRoles.SuperAdmin] = new()
        {
            [EntityType.SchoolDistrict] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.Program] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.User] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.Student] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.Goal] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.ProgressEvent] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
        },

        // ──────────────────────────────────────────────
        // District Admin
        // ──────────────────────────────────────────────
        [UserRoles.DistrictAdmin] = new()
        {
            [EntityType.SchoolDistrict] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Program] = new()
            {
                [PermissionAction.Create] = MineOnly,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = MineOnly,
            },
            [EntityType.User] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.Student] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.Goal] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
            [EntityType.ProgressEvent] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = Allow,
            },
        },

        // ──────────────────────────────────────────────
        // Program Admin
        // ──────────────────────────────────────────────
        [UserRoles.ProgramAdmin] = new()
        {
            [EntityType.SchoolDistrict] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = Deny,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Program] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.User] = new()
            {
                [PermissionAction.Create] = MineOnly,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Student] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = MineOnly,
            },
            [EntityType.Goal] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = MineOnly,
            },
            [EntityType.ProgressEvent] = new()
            {
                [PermissionAction.Create] = Allow,
                [PermissionAction.Read]   = Allow,
                [PermissionAction.Update] = Allow,
                [PermissionAction.Delete] = MineOnly,
            },
        },

        // ──────────────────────────────────────────────
        // Teacher
        // ──────────────────────────────────────────────
        [UserRoles.Teacher] = new()
        {
            [EntityType.SchoolDistrict] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = Deny,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Program] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.User] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Student] = new()
            {
                [PermissionAction.Create] = MineOnly,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = MineOnly,
            },
            [EntityType.Goal] = new()
            {
                [PermissionAction.Create] = MineOnly,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = MineOnly,
            },
            [EntityType.ProgressEvent] = new()
            {
                [PermissionAction.Create] = MineOnly,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = MineOnly,
            },
        },

        // ──────────────────────────────────────────────
        // Paraeducator
        // ──────────────────────────────────────────────
        [UserRoles.Paraeducator] = new()
        {
            [EntityType.SchoolDistrict] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = Deny,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Program] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.User] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Student] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.Goal] = new()
            {
                [PermissionAction.Create] = Deny,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = Deny,
                [PermissionAction.Delete] = Deny,
            },
            [EntityType.ProgressEvent] = new()
            {
                [PermissionAction.Create] = MineOnly,
                [PermissionAction.Read]   = MineOnly,
                [PermissionAction.Update] = MineOnly,
                [PermissionAction.Delete] = Deny,
            },
        },
    };

    public static PermissionRule? GetRule(string role, string entity, string action)
    {
        if (_rules.TryGetValue(role, out var entities)
            && entities.TryGetValue(entity, out var actions)
            && actions.TryGetValue(action, out var rule))
        {
            return rule;
        }

        return null;
    }
}
