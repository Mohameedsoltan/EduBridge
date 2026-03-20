using EduBridge.Abstractions;

namespace EduBridge.Errors;

public static class TeamErrors
{
    public static readonly Error TeamNotFound = new(
        "Team.NotFound", "Team not found", StatusCodes.Status404NotFound);

    public static readonly Error NotTeamLeader = new(
        "Team.NotTeamLeader", "Only the team leader can perform this action", StatusCodes.Status403Forbidden);

    public static readonly Error AlreadyMember = new(
        "Team.AlreadyMember", "User is already a member of this team", StatusCodes.Status409Conflict);

    public static readonly Error MemberNotFound = new(
        "Team.MemberNotFound", "Member not found in this team", StatusCodes.Status404NotFound);

    public static readonly Error TeamFull = new(
        "Team.Full", "This team has reached its maximum capacity", StatusCodes.Status409Conflict);

    public static readonly Error LeaderCannotLeave = new(
        "Team.LeaderCannotLeave", "The team leader cannot leave the team", StatusCodes.Status400BadRequest);

    public static readonly Error IdeaNotFound = new(
        "Team.IdeaNotFound", "Idea not found", StatusCodes.Status404NotFound);
}