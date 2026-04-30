using ApiUsuariosRC.Entities.Models;
using ApiUsuariosRC.Services.EmployeeNumbers;

namespace ApiUsuariosRC.UnitTests;

public class EmployeeNumberGeneratorTests
{
    [Fact]
    public void Generate_ReturnsDeterministicNumberWithRcPrefixAndGenderInitial()
    {
        var usuario = new Usuario
        {
            Nombres = "Eduardo Daniel",
            ApellidoPaterno = "Villanueva",
            ApellidoMaterno = "Amaro",
            FechaNacimiento = new DateTime(1996, 11, 16)
        };

        var first = EmployeeNumberGenerator.Generate(usuario, "Masculino", 2026);
        var second = EmployeeNumberGenerator.Generate(usuario, "Masculino", 2026);

        Assert.Equal(first, second);
        Assert.StartsWith("RC-M-EDVIAM-8X96-26-", first);
        Assert.Equal(24, first.Length);
    }

    [Fact]
    public void Generate_NormalizesAccentsAndPadsMissingLetters()
    {
        var usuario = new Usuario
        {
            Nombres = "Ana",
            ApellidoPaterno = "Nunez",
            ApellidoMaterno = "O",
            FechaNacimiento = new DateTime(1999, 12, 9)
        };

        var numeroEmpleado = EmployeeNumberGenerator.Generate(usuario, "Femenino", 2026);

        Assert.Contains("-F-ANNUOX-", numeroEmpleado);
    }

    [Fact]
    public void WithCollisionSuffix_AppendsBase36Suffix()
    {
        var result = EmployeeNumberGenerator.WithCollisionSuffix("RC-M-TEST-001", 36);

        Assert.Equal("RC-M-TEST-001-10", result);
    }
}
