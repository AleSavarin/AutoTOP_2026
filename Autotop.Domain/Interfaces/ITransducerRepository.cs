namespace Autotop.Domain.Interfaces;

public interface ITransducerRepository
{
    Task<IReadOnlyList<TransducerCoefficients>> LoadAsync(string filePath, CancellationToken cancellationToken = default);
}