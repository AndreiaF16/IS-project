using System;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace IPLeiriaSmartCampusAPI.Models
{
    public class Authentication : Utilizador
    {
        private static string CONNECTION_STRING = Properties.Settings.Default.ConnectionString;

        public string Password { get; set; }

        public static string EncodingToken(string text)
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        public static string DencodingToken(string token)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(token));
        }

        public static Utilizador Login(Authentication user)
        {
            Utilizador utilizador = null;

            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                SqlCommand cmd = null;
                if (user.Email != null && user.Username == null)
                {
                    cmd = new SqlCommand("SELECT * FROM Utilizadores WHERE Email = @email AND Password = @password", conn);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                }

                if (user.Email == null && user.Username != null)
                {
                    cmd = new SqlCommand("SELECT * FROM Utilizadores WHERE Username = @username AND Password = @password", conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                }

                if (user.Email != null && user.Username != null)
                {
                    cmd = new SqlCommand("SELECT * FROM Utilizadores WHERE Username = @username AND Email = @email AND Password = @password", conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                }

                if (cmd == null)
                {
                    return null;
                }

                cmd.Parameters.AddWithValue("@password", Authentication.ComputeSha256Hash(user.Password));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        utilizador = new Utilizador
                        {
                            Id = (int)reader["Id"],
                            Username = (string)reader["Username"],
                            Email = (string)reader["Email"],
                            Nome = (string)reader["Nome"],
                            Role = (string)reader["Role"]
                        };
                    }
                }

                conn.Close();
            }

            return utilizador;
        }

        public static string ComputeSha256Hash(string text)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static Authentication GetUtilizadorById(int id)
        {
            Authentication utilizador = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM Utilizadores WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            utilizador = new Authentication()
                            {
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                Nome = (string)reader["Nome"],
                                Password = (string)reader["Password"],
                                Role = (string)reader["Role"]
                            };
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {
                return null;
            }

            return utilizador;
        }

        public static void UpdateGenericIdentity(Utilizador utilizador)
        {
            string[] roles = null;
            switch (utilizador.Role)
            {
                case "Admin":
                    roles = new string[] { "Admin", "User" };
                    break;
                case "User":
                    roles = new string[] { "User" };
                    break;
                default:
                    break;
            }

            GenericIdentity identity = new GenericIdentity(utilizador.Email);
            identity.AddClaim(new Claim(ClaimTypes.Email, utilizador.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, utilizador.Nome));
            identity.AddClaim(new Claim(ClaimTypes.Role, utilizador.Role));
            identity.AddClaim(new Claim(ClaimTypes.Sid, Convert.ToString(utilizador.Id)));
            IPrincipal principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }
    }
}