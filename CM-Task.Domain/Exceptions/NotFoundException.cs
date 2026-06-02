namespace CM_Task.Domain.Exceptions;

public sealed class NotFoundException(string resourceName, object id)
    : DomainException($"{resourceName} with id '{id}' was not found");