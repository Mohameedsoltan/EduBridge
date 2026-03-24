namespace EduBridge.Contracts.Idea;

public record CreateIdeaTagRequest(
    string Name,
    Guid CategoryId
);