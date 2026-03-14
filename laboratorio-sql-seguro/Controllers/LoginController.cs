using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LaboratorioSQLi.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Ruta: GET /Login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Ruta: POST /Login/VulnerableLogin (Mantengo el nombre para no romper el Index.cshtml)
        [HttpPost]
        public IActionResult VulnerableLogin(string usuario, string password)
        {
            // Cadena de conexión
            string connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=db;Database=LabSQLi;User Id=sa;Password=Password_Fuerte_123!;TrustServerCertificate=True;";

            // ✅ SEGURIDAD: Usamos @user y @pass como marcadores de posición (placeholders)
            // Ya no concatenamos variables directamente en el string de la consulta.
            string sql = "SELECT NombreUsuario FROM Usuarios WHERE NombreUsuario = @user AND Password = @pass";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);

                    // ✅ PARAMETRIZACIÓN: El motor de base de datos recibe los valores por separado.
                    // Si un usuario escribe: admin' --
                    // El sistema buscará literalmente un usuario llamado "admin' --" y no ejecutará el comando.
                    command.Parameters.Add("@user", SqlDbType.VarChar).Value = usuario ?? (object)DBNull.Value;
                    command.Parameters.Add("@pass", SqlDbType.VarChar).Value = password ?? (object)DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            ViewBag.Mensaje = "¡Acceso Seguro Concedido! Bienvenido, " + reader["NombreUsuario"];
                        }
                        else
                        {
                            // En seguridad es mejor no decir si falló el usuario o la clave específicamente
                            ViewBag.Error = "Usuario o contraseña incorrectos.";
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // Error de conexión o de base de datos
                    ViewBag.Error = "Error en el servidor: " + ex.Message;
                }
            }

            // Retornamos a la misma vista Index para mostrar los resultados
            return View("Index");
        }
    }
}
