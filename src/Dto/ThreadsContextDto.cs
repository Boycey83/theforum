namespace theforum.Dto;

public record ThreadsContextDto(
    string LoggedInAsUsername,
    int PageNumber,
    int ThreadCount,
    IEnumerable<ThreadDto> Threads
);
