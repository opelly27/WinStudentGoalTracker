using WinStudentGoalTracker.Models;

namespace WinStudentGoalTracker.Services;

public class PermissionService
{
    /// <summary>
    /// Checks whether the given role is allowed to perform the specified action
    /// on the specified entity, considering ownership.
    /// </summary>
    /// <param name="role">The user's role (use UserRoles constants)</param>
    /// <param name="entity">The entity being acted on (use EntityType constants)</param>
    /// <param name="action">The action being performed (use PermissionAction constants)</param>
    /// <param name="isMine">Whether the resource belongs to the requesting user.
    /// For Create actions this parameter is ignored.</param>
    /// <returns>True if the action is permitted, false otherwise.</returns>
    public bool IsAllowed(string role, string entity, string action, bool isMine = true)
    {
        var rule = PermissionMatrix.GetRule(role, entity, action);

        if (rule is null)
        {
            return false;
        }
            

        // Create has no ownership concept — use the Mine field as a general "can create" flag
        if (action == PermissionAction.Create)
        {
            return rule.Value.Mine;
        }
            

        return isMine ? rule.Value.Mine : rule.Value.Others;
    }
}
