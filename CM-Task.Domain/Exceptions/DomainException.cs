namespace CM_Task.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);