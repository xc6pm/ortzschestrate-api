namespace Ortzschestrate.Api.Models;

public record AckMessage<TMessage>(uint MessageId, TMessage Message);

public record AckMessage(uint MessageId);