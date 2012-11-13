using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Text.RegularExpressions;

namespace Login
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

     
        protected void ButtonNewUser_Click(object sender, EventArgs e)
        {

   
            if (TextBoxUsername.Text == "" || Password.Text == "" || TextBoxUsername.Text.Length >= 7 || Password.Text.Length != 6)
            {

                Label3.Text = "Your username or password is incorrect!";
            }
            else if (checkUsername(TextBoxUsername.Text) == false)
            {

                string username = TextBoxUsername.Text;
                string password = Password.Text.ToString();


                string pass = CreateHash(password);

                string salt = pass.Split(':')[0];
                salt = salt +":" + pass.Split(':')[1];
                string hash = pass.Substring(pass.LastIndexOf(@":"));

                string connection = "Data Source=C:\\Users\\Jesper\\Documents\\Visual Studio 2012\\Projects\\Login\\Login\\DB\\Database1.sdf";

                SqlCeConnection conn = new SqlCeConnection(connection);
                SqlCeCommand insert = null;
                SqlCeTransaction sqlTransaction = null;



                string sqlInsert = @"INSERT INTO login(Name,Hash,Salt) VALUES(@Name,@Hash,@Salt)";



                conn.Open();

                sqlTransaction = conn.BeginTransaction();


                try
                {
                    insert = conn.CreateCommand();
                    insert.CommandText = sqlInsert;
                    insert.Transaction = sqlTransaction;

                    insert.Parameters.Add("@Name", SqlDbType.NVarChar);
                    insert.Parameters.Add("@Hash", SqlDbType.NVarChar);
                    insert.Parameters.Add("@Salt", SqlDbType.NVarChar);

                    insert.Parameters["@Name"].Value = username;
                    insert.Parameters["@Hash"].Value = hash;
                    insert.Parameters["@Salt"].Value = salt;

                    insert.ExecuteNonQuery();

                     sqlTransaction.Commit();

                    Label3.Text = "The user has been created";
                }
                catch
                {
                    Label3.Text = "Something is not working";
                    sqlTransaction.Rollback();

                }
                finally
                {
                    conn.Close();
                }



            }
            else
            {
                Label3.Text = "Username taken";
            }
        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {


            if (TextBoxUsernemaLogin.Text == "" || TextBoxPasswordLogin.Text == "")
            {

                Label6.Text = "Try again";

            }
            else
            {

                string username = TextBoxUsernemaLogin.Text;
                string password = TextBoxPasswordLogin.Text;

                

                string connection = "Data Source=C:\\Users\\Jesper\\Documents\\Visual Studio 2012\\Projects\\Login\\Login\\DB\\Database1.sdf";


                SqlCeConnection conn = new SqlCeConnection(connection);
                SqlCeCommand select = null;



                string sqlInsert = @"SELECT Name, Hash,Salt FROM login WHERE name = @name";



                conn.Open();





                try
                {
                    select = conn.CreateCommand();
                    select.CommandText = sqlInsert;
                    select.Parameters.Add("@Name", SqlDbType.NVarChar);

                    select.Parameters["@Name"].Value = username;


                    SqlCeDataReader reader = select.ExecuteReader();
                    string name = "";
                    string hash = "";
                    string salt = "";

                    while (reader.Read())
                    {
                        name = reader.GetString(0);
                        hash = reader.GetString(1);
                        salt = reader.GetString(2);
                    }

                    bool check = false;

                    string checkPass = salt + hash;



                    check = ValidatePassword(password, checkPass);




                  if (check == true)
                  {
                      Label6.Text = "Welcome " + name;
                  }
                  else
                  {
                      Label6.Text = "Wrong password or username";
                  }

                }
                catch
                {
                    Label3.Text = "Something is not working";


                }
                finally
                {


                    conn.Close();
                }

            }
        }

        public bool checkUsername(string username)
        {


            bool checkUName = false;

            string connection = "Data Source=C:\\Users\\Jesper\\Documents\\Visual Studio 2012\\Projects\\Login\\Login\\DB\\Database1.sdf";


            SqlCeConnection conn = new SqlCeConnection(connection);
            SqlCeCommand select = null;

            string sqlInsert = @"SELECT Name FROM login WHERE name = @name";

             conn.Open();
            
            try
            {
                select = conn.CreateCommand();
                select.CommandText = sqlInsert;
                select.Parameters.Add("@Name", SqlDbType.NVarChar);

                select.Parameters["@Name"].Value = username;


                SqlCeDataReader reader = select.ExecuteReader();
                string name = "";


                while (reader.Read())
                {
                    name = reader.GetString(0);

                }

                if (name == username)
                {
                    checkUName = true;
                }


            }
            finally
            {


                conn.Close();

                
            }

            return checkUName;
        }
    



            // The following constants may be changed without breaking existing hashes.
            public const int SALT_BYTES = 24;
            public const int HASH_BYTES = 24;
            public const int PBKDF2_ITERATIONS = 1000;

            public const int ITERATION_INDEX = 0;
            public const int SALT_INDEX = 1;
            public const int PBKDF2_INDEX = 2;

            /// <summary>
            /// Creates a salted PBKDF2 hash of the password.
            /// </summary>
            /// <param name="password">The password to hash.</param>
            /// <returns>The hash of the password.</returns>
            public static string CreateHash(string password)
            {
                // Generate a random salt
                RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
                byte[] salt = new byte[SALT_BYTES];
                csprng.GetBytes(salt);

                // Hash the password and encode the parameters
                byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTES);
                return PBKDF2_ITERATIONS + ":" +
                    Convert.ToBase64String(salt) + ":" +
                    Convert.ToBase64String(hash);
            }

            /// <summary>
            /// Validates a password given a hash of the correct one.
            /// </summary>
            /// <param name="password">The password to check.</param>
            /// <param name="goodHash">A hash of the correct password.</param>
            /// <returns>True if the password is correct. False otherwise.</returns>
            public static bool ValidatePassword(string password, string goodHash)
            {
                // Extract the parameters from the hash
                char[] delimiter = { ':' };
                string[] split = goodHash.Split(delimiter);
                int iterations = Int32.Parse(split[ITERATION_INDEX]);
                byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
                byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

                byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
                return SlowEquals(hash, testHash);
            }

            /// <summary>
            /// Compares two byte arrays in length-constant time. This comparison
            /// method is used so that password hashes cannot be extracted from
            /// on-line systems using a timing attack and then attacked off-line.
            /// </summary>
            /// <param name="a">The first byte array.</param>
            /// <param name="b">The second byte array.</param>
            /// <returns>True if both byte arrays are equal. False otherwise.</returns>
            private static bool SlowEquals(byte[] a, byte[] b)
            {
                uint diff = (uint)a.Length ^ (uint)b.Length;
                for (int i = 0; i < a.Length && i < b.Length; i++)
                    diff |= (uint)(a[i] ^ b[i]);
                return diff == 0;
            }

            /// <summary>
            /// Computes the PBKDF2-SHA1 hash of a password.
            /// </summary>
            /// <param name="password">The password to hash.</param>
            /// <param name="salt">The salt.</param>
            /// <param name="iterations">The PBKDF2 iteration count.</param>
            /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
            /// <returns>A hash of the password.</returns>
            private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
            {
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }

        }




    }


