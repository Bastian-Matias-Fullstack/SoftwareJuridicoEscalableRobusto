

using Dominio.Entidades;

namespace Tests.Domain.Entities
{
    public class UsuarioTests
    {
        [Fact]
        public void CrearUsuario_DatosValidos_CreaUsuarioCorrectamente()
        {
            // Arrange
            var nombre = "Juan Pérez";
            var email = "juan.perez@example.com";
            var passwordHash = "hash-valido";

            // Act
            var usuario = new Usuario(nombre, email, passwordHash);

            // Assert
            Assert.Equal(nombre, usuario.Nombre);
            Assert.Equal(email, usuario.Email);
            Assert.Equal(passwordHash, usuario.PasswordHash);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("juan@")]
        [InlineData("@mail.com")]
        [InlineData("")]
        public void CrearUsuario_EmailInvalido_LanzaFormatException(string emailInvalido)
        {
            // Arrange
            var nombre = "Juan Pérez";
            var passwordHash = "hash";

            // Act & Assert
            Assert.Throws<FormatException>(() =>
                new Usuario(nombre, emailInvalido, passwordHash));
        }
    }
}
