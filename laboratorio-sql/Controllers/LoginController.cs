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

        // 1. Muestra el formulario de login (GET)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // 2. Procesa el intento de login (POST)
        [HttpPost]
        public IActionResult VulnerableLogin(string usuario, string password)
        {
            // CONFIGURACIÓN DE CONEXIÓN
            string connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=db;Database=LabSQLi;User Id=sa;Password=Password_Fuerte_123!;TrustServerCertificate=True;";

            // CONSULTA VULNERABLE (Concatenación directa de strings)
            // Esta es la línea que permite el ataque ' OR 1=1 --
            string sql = "SELECT NombreUsuario FROM LabSQLi.dbo.Usuarios WHERE NombreUsuario = '" + usuario + "' AND Password = '" + password + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Si la consulta devuelve al menos una fila, el login es "exitoso"
                    if (dt.Rows.Count > 0)
                    {
                        string nombreEncontrado = dt.Rows[0]["NombreUsuario"].ToString();
                        ViewBag.Mensaje = "¡Acceso Concedido! Bienvenido, " + nombreEncontrado;
                        return View("Index"); // En un entorno real aquí irías al Dashboard
                    }
                    else
                    {
                        ViewBag.Error = "Usuario o contraseña incorrectos.";
                    }
                }
                catch (System.Exception ex)
                {
                    // Esto te ayudará a ver si la tabla o la DB tienen errores
                    ViewBag.Error = "Error de Base de Datos: " + ex.Message;
                    ViewBag.DebugQuery = "Query intentada: " + sql;
                }
            }

            return View("Index");
        }
    }
}
