namespace CM_Task.Domain.Exceptions;

public sealed class InsufficientStockException(Guid productId, int requested, int available)
    : DomainException($"Insufficient stock for product '{productId}': requested {requested}, available {available}.");