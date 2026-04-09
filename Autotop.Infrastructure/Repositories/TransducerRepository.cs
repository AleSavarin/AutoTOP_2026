using Autotop.Domain.Interfaces;
using Autotop.Domain.Models;
using System.Globalization;

namespace Autotop.Infrastructure.Repositories;

public class TransducerRepository : ITransducerRepository
{
    public async Task<IReadOnlyList<TransducerCoefficients>> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        var culture = CultureInfo.GetCultureInfo("es-AR");
        var result = new List<TransducerCoefficients>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var parts = line.Split(';');
            if (parts.Length != 5) continue;

            var name = parts[0].Trim();
            var a = double.Parse(parts[1].Trim(), culture);
            var b = double.Parse(parts[2].Trim(), culture);
            var c = double.Parse(parts[3].Trim(), culture);
            var d = double.Parse(parts[4].Trim(), culture);

            result.Add(new TransducerCoefficients(name, a, b, c, d));
        }

        return result.AsReadOnly();
    }
}