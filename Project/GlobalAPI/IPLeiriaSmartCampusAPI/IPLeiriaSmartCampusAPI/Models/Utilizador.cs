using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IPLeiriaSmartCampusAPI.Models
{
    public class Utilizador
    {
        private static string CONNECTION_STRING = Properties.Settings.Default.ConnectionString;

        public int Id { get; set; }
	    public string Username{ get; set; }
        public string Email { get; set; }
	    public string Nome { get; set; }
        public string Role { get; set; }

        public static List<Utilizador> GetAllUtilizadores()
        {
            List<Utilizador> utilizadores = null;

            try
            {
                utilizadores = new List<Utilizador>();
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM Utilizadores", conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Utilizador utilizador = new Utilizador()
                            {
                                Id = (int)reader["Id"],
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                Nome = (string)reader["Nome"],
                                Role = (string)reader["Role"]
                            };

                            utilizadores.Add(utilizador);
                        }
                    }
                    conn.Close();
                }

                return utilizadores;
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public static Utilizador GetUtilizadorById(int id)
        {
            Utilizador utilizador = null;
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
                            utilizador = new Utilizador()
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
            } 
            catch
            {
                return null;
            }

            return utilizador;
        }

        public static Utilizador UpdateUtilizadorById(Authentication user, int id, string identityRole)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    Authentication currentUser = Authentication.GetUtilizadorById(id);
                    if(currentUser == null)
                    {
                        return null;
                    }

                    SqlCommand cmd = new SqlCommand("UPDATE Utilizadores SET Nome = @nome, Username = @username, Email = @email, Password = @password, Role = @role WHERE Id = @id", conn);

                    if (user.Nome == null)
                    {
                        cmd.Parameters.AddWithValue("@nome", currentUser.Nome);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@nome", user.Nome);
                    }

                    if (user.Username == null)
                    {
                        cmd.Parameters.AddWithValue("@username", currentUser.Username);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                    }

                    if (user.Email == null)
                    {
                        cmd.Parameters.AddWithValue("@email", currentUser.Email);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@email", user.Email);
                    }

                    if (user.Password == null || user.Password.Length == 64)
                    {
                        cmd.Parameters.AddWithValue("@password", currentUser.Password);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@password", Authentication.ComputeSha256Hash(user.Password));
                    }

                    if (identityRole == "Admin" && user.Role != null)
                    {
                        cmd.Parameters.AddWithValue("@role", user.Role);
                    }
                    else
                    { 
                        cmd.Parameters.AddWithValue("@role", currentUser.Role);
                    }

                    cmd.Parameters.AddWithValue("@id", id);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == -1)
                    {
                        return null;
                    }

                    return GetUtilizadorByEmail(user.Email);
                }
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public static Utilizador GetUtilizadorByUsername(string username)
        {
            Utilizador utilizador = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM Utilizadores WHERE Username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            utilizador = new Utilizador()
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
            }
            catch
            {
                return null;
            }

            return utilizador;
        }

        public static Utilizador GetUtilizadorByEmail(string email)
        {
            Utilizador utilizador = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM Utilizadores WHERE Email = @email", conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            utilizador = new Utilizador()
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
            }
            catch
            {
                return null;
            }

            return utilizador;
        }

        public static Utilizador CreateNewUser(Authentication user)
        {
            Utilizador utilizador = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO Utilizadores (Nome, Username, Email, Password, Role) " +
                                                        "VALUES (@nome, @username, @email, @password, @role)", conn);
                    cmd.Parameters.AddWithValue("@nome", user.Nome);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@password", Authentication.ComputeSha256Hash(user.Password));
                    cmd.Parameters.AddWithValue("@role", "User");

                    int rows = cmd.ExecuteNonQuery();

                    if(rows == -1)
                    {
                        return null;
                    }

                    return GetUtilizadorByEmail(user.Email);
                }
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}
