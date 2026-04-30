using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ApiUsuariosRC.Entities.Models;

namespace ApiUsuariosRC.Services.EmployeeNumbers;

public static class EmployeeNumberGenerator
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Generate(Usuario usuario, string genero, int registrationYear)
    {
        var genderInitial = BuildGenderInitial(genero);
        var nameKey = BuildNameKey(usuario.Nombres, usuario.ApellidoPaterno, usuario.ApellidoMaterno);
        var birthKey = BuildBirthKey(usuario.FechaNacimiento);
        var registrationKey = (registrationYear % 100).ToString("00", CultureInfo.InvariantCulture);
        var checksum = BuildChecksum(BuildChecksumInput(usuario, genero, registrationYear), 4);

        return $"RC-{genderInitial}-{nameKey}-{birthKey}-{registrationKey}-{checksum}";
    }

    public static string WithCollisionSuffix(string baseNumber, int collisionIndex)
    {
        if (collisionIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(collisionIndex), "El indice de colision debe ser mayor a cero.");
        }

        return $"{baseNumber}-{ToBase36(collisionIndex).PadLeft(2, '0')}";
    }

    private static string BuildNameKey(string nombres, string apellidoPaterno, string apellidoMaterno)
    {
        return string.Concat(
            TakeLetters(GetFirstToken(nombres), 2),
            TakeLetters(apellidoPaterno, 2),
            TakeLetters(apellidoMaterno, 2));
    }

    private static string BuildBirthKey(DateTime fechaNacimiento)
    {
        var dayOfYear = ToBase36(fechaNacimiento.DayOfYear).PadLeft(2, '0');
        var year = (fechaNacimiento.Year % 100).ToString("00", CultureInfo.InvariantCulture);

        return $"{dayOfYear}{year}";
    }

    private static string BuildChecksumInput(Usuario usuario, string genero, int registrationYear)
    {
        return string.Join(
            '|',
            NormalizeLetters(genero),
            NormalizeLetters(usuario.Nombres),
            NormalizeLetters(usuario.ApellidoPaterno),
            NormalizeLetters(usuario.ApellidoMaterno),
            usuario.FechaNacimiento.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
            registrationYear.ToString(CultureInfo.InvariantCulture));
    }

    private static string BuildGenderInitial(string genero)
    {
        return TakeLetters(genero, 1);
    }

    private static string BuildChecksum(string input, int length)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var value = ((uint)bytes[0] << 24)
            | ((uint)bytes[1] << 16)
            | ((uint)bytes[2] << 8)
            | bytes[3];

        var modulo = 1;
        for (var index = 0; index < length; index++)
        {
            modulo *= Alphabet.Length;
        }

        return ToBase36(value % modulo).PadLeft(length, '0');
    }

    private static string GetFirstToken(string value)
    {
        return value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault() ?? string.Empty;
    }

    private static string TakeLetters(string value, int count)
    {
        return NormalizeLetters(value).PadRight(count, 'X')[..count];
    }

    private static string NormalizeLetters(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var upper = char.ToUpperInvariant(character);
            if (upper is >= 'A' and <= 'Z')
            {
                builder.Append(upper);
            }
        }

        return builder.ToString();
    }

    private static string ToBase36(long value)
    {
        if (value == 0)
        {
            return "0";
        }

        var builder = new StringBuilder();
        var current = value;

        while (current > 0)
        {
            builder.Insert(0, Alphabet[(int)(current % Alphabet.Length)]);
            current /= Alphabet.Length;
        }

        return builder.ToString();
    }
}
