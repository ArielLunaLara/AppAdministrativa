using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace AppAdministrativa
{
    public class DatabaseService
    {
        private static DatabaseService? _instance;
        public static DatabaseService Instance => _instance ??= new DatabaseService();

        private readonly string _dbPath;
        public bool IsAvailable { get; private set; }

        public DatabaseService()
        {
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            _dbPath = Path.GetFullPath(Path.Combine(exeFolder, "..", "..", "..", "..", "..", "horarios.db"));
            IsAvailable = File.Exists(_dbPath);

            if (!IsAvailable)
                System.Windows.MessageBox.Show(
                    $"No se encontró horarios.db en:\n{_dbPath}\n\nVerifica la estructura de carpetas PILM.",
                    "Base de datos no encontrada",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
        }

        private SqliteConnection GetConnection()
            => new SqliteConnection($"Data Source={_dbPath}");

        // ══════════════════════════════════════════════════════════════════════
        // SALONES / CLASSROOM
        // ══════════════════════════════════════════════════════════════════════
        public List<FilaAula> GetSalones()
        {
            var lista = new List<FilaAula>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT classroom_name, floor_num FROM Classroom ORDER BY classroom_name";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new FilaAula
                {
                    Nombre = r.GetString(0),
                    Piso = r.IsDBNull(1) ? "" : r.GetInt32(1).ToString()
                });
            return lista;
        }

        public void AgregarSalon(FilaAula a)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Classroom (classroom_name, floor_num) VALUES ($n,$p)";
            cmd.Parameters.AddWithValue("$n", a.Nombre);
            cmd.Parameters.AddWithValue("$p", string.IsNullOrEmpty(a.Piso) ? DBNull.Value : int.Parse(a.Piso));
            cmd.ExecuteNonQuery();
        }

        public void EditarSalon(FilaAula a, string nombreOriginal)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Classroom SET classroom_name=$nuevo, floor_num=$p WHERE classroom_name=$original";
            cmd.Parameters.AddWithValue("$nuevo", a.Nombre);
            cmd.Parameters.AddWithValue("$p", string.IsNullOrEmpty(a.Piso) ? DBNull.Value : int.Parse(a.Piso));
            cmd.Parameters.AddWithValue("$original", nombreOriginal);
            cmd.ExecuteNonQuery();

            if (a.Nombre != nombreOriginal)
            {
                var c1 = conn.CreateCommand();
                c1.CommandText = "UPDATE Class SET id_classroom=$n WHERE id_classroom=$o";
                c1.Parameters.AddWithValue("$n", a.Nombre);
                c1.Parameters.AddWithValue("$o", nombreOriginal);
                c1.ExecuteNonQuery();

                var c2 = conn.CreateCommand();
                c2.CommandText = "UPDATE Video360 SET id_classroom=$n WHERE id_classroom=$o";
                c2.Parameters.AddWithValue("$n", a.Nombre);
                c2.Parameters.AddWithValue("$o", nombreOriginal);
                c2.ExecuteNonQuery();
            }
        }

        public void EliminarSalon(string nombre)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Classroom WHERE classroom_name=$n";
            cmd.Parameters.AddWithValue("$n", nombre);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // PROFESORES / TEACHERS
        // ══════════════════════════════════════════════════════════════════════
        public List<Profesor> GetProfesores()
        {
            var lista = new List<Profesor>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id_teacher, teacher_name, teacher_almamater,
                                        teacher_investigations, teacher_professional_bio,
                                        teacher_photo, teacher_audio
                                 FROM Teacher ORDER BY teacher_name";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Profesor
                {
                    Clave = r.GetInt32(0).ToString(),
                    Nombre = r.IsDBNull(1) ? "" : r.GetString(1),
                    Estudios = r.IsDBNull(2) ? "" : r.GetString(2),
                    Investigaciones = r.IsDBNull(3) ? "" : r.GetString(3),
                    Semblanza = r.IsDBNull(4) ? "" : r.GetString(4),
                    Fotografia = r.IsDBNull(5) ? "" : r.GetString(5),
                    Audio = r.IsDBNull(6) ? "" : r.GetString(6)
                });
            return lista;
        }

        public void AgregarProfesor(Profesor p)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Teacher (teacher_name, teacher_almamater,
                                    teacher_investigations, teacher_professional_bio,
                                    teacher_photo, teacher_audio)
                                 VALUES ($n,$e,$i,$s,$f,$a)";
            cmd.Parameters.AddWithValue("$n", p.Nombre);
            cmd.Parameters.AddWithValue("$e", p.Estudios ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$i", p.Investigaciones ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$s", p.Semblanza ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$f", p.Fotografia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$a", p.Audio ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void EditarProfesor(Profesor p)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Teacher SET teacher_name=$n, teacher_almamater=$e,
                                    teacher_investigations=$i, teacher_professional_bio=$s,
                                    teacher_photo=$f, teacher_audio=$a
                                 WHERE id_teacher=$id";
            cmd.Parameters.AddWithValue("$id", int.Parse(p.Clave));
            cmd.Parameters.AddWithValue("$n", p.Nombre);
            cmd.Parameters.AddWithValue("$e", p.Estudios ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$i", p.Investigaciones ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$s", p.Semblanza ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$f", p.Fotografia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$a", p.Audio ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

		public bool EliminarProfesor(string clave)
		{
			if (!IsAvailable) return false;
			try
			{
				using var conn = GetConnection();
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText = "DELETE FROM Teacher WHERE id_teacher=$id";
				cmd.Parameters.AddWithValue("$id", int.Parse(clave));
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		// ══════════════════════════════════════════════════════════════════════
		// MATERIAS / COURSES
		// ══════════════════════════════════════════════════════════════════════
		public List<Materia> GetMaterias()
        {
            var lista = new List<Materia>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id_course, course_name, course_related_companies, course_syllabus_rute FROM Course ORDER BY course_name";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Materia
                {
                    ID = r.GetInt32(0).ToString(),
                    Nombre = r.IsDBNull(1) ? "" : r.GetString(1),
                    Companias = r.IsDBNull(2) ? "" : r.GetString(2),
                    RutaTemario = r.IsDBNull(3) ? "" : r.GetString(3)
                });
            return lista;
        }

        public void AgregarMateria(Materia m)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Course (course_name, course_related_companies, course_syllabus_rute) VALUES ($n,$c,$r)";
            cmd.Parameters.AddWithValue("$n", m.Nombre);
            cmd.Parameters.AddWithValue("$c", m.Companias ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$r", m.RutaTemario ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void EditarMateria(Materia m)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Course SET course_name=$n, course_related_companies=$c, course_syllabus_rute=$r WHERE id_course=$id";
            cmd.Parameters.AddWithValue("$id", int.Parse(m.ID));
            cmd.Parameters.AddWithValue("$n", m.Nombre);
            cmd.Parameters.AddWithValue("$c", m.Companias ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$r", m.RutaTemario ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();

        }

        public void EliminarMateria(string id)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Course WHERE id_course=$id";
            cmd.Parameters.AddWithValue("$id", int.Parse(id));
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // HORARIOS / CLASES
        // ══════════════════════════════════════════════════════════════════════
        public List<FilaHorario> GetHorarios()
        {
            var lista = new List<FilaHorario>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id_class, schedule_type, class_start_time,
                                        class_finish_time, id_teacher, id_classroom
                                 FROM Class
                                 ORDER BY id_class, class_start_time";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new FilaHorario
                {
                    IDClase = r.GetString(0),
                    Tipo = r.IsDBNull(1) ? "" : r.GetString(1),
                    HoraInicio = $"{r.GetInt32(2):D2}:00",
                    HoraFin = $"{r.GetInt32(3):D2}:00",
                    IDProfesor = r.GetInt32(4).ToString(),
                    IDAula = r.IsDBNull(5) ? "" : r.GetString(5)
                });
            return lista;
        }

        public void AgregarHorario(FilaHorario h, string classroomName)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            if (!int.TryParse(h.IDClase.Length >= 4 ? h.IDClase[..4] : h.IDClase, out int idCourse)) return;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Class
                (id_class,id_classroom,id_course,id_teacher,schedule_type,class_start_time,class_finish_time)
                VALUES ($id,$cl,$co,$t,$tp,$hi,$hf)";
            cmd.Parameters.AddWithValue("$id", h.IDClase);
            cmd.Parameters.AddWithValue("$cl", classroomName);
            cmd.Parameters.AddWithValue("$co", idCourse);
            cmd.Parameters.AddWithValue("$t", int.Parse(h.IDProfesor));
            cmd.Parameters.AddWithValue("$tp", h.Tipo);
            cmd.Parameters.AddWithValue("$hi", int.Parse(h.HoraInicio.Replace(":00", "")));
            cmd.Parameters.AddWithValue("$hf", int.Parse(h.HoraFin.Replace(":00", "")));
            cmd.ExecuteNonQuery();
        }

        public void EditarHorario(FilaHorario h, string classroomName)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Class SET
                id_classroom=$cl, id_teacher=$t, schedule_type=$tp,
                class_start_time=$hi, class_finish_time=$hf
                WHERE id_class=$id";
            cmd.Parameters.AddWithValue("$id", h.IDClase);
            cmd.Parameters.AddWithValue("$cl", classroomName);
            cmd.Parameters.AddWithValue("$t", int.Parse(h.IDProfesor));
            cmd.Parameters.AddWithValue("$tp", h.Tipo);
            cmd.Parameters.AddWithValue("$hi", int.Parse(h.HoraInicio.Replace(":00", "")));
            cmd.Parameters.AddWithValue("$hf", int.Parse(h.HoraFin.Replace(":00", "")));
            cmd.ExecuteNonQuery();
        }

        public void EliminarHorario(string idClase)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Class WHERE id_class=$id";
            cmd.Parameters.AddWithValue("$id", idClase);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // PROYECTOS — project_name es PK, id_course es FK funcional (no visible al usuario)
        // ══════════════════════════════════════════════════════════════════════
        public List<ProyectoItem> GetProyectos()
        {
            var lista = new List<ProyectoItem>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            // JOIN con Course para mostrar el nombre de materia en lugar de la clave
            cmd.CommandText = @"SELECT p.project_name, co.course_name
                                 FROM Project p
                                 JOIN Course co ON p.id_course = co.id_course
                                 ORDER BY co.course_name, p.project_name";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new ProyectoItem
                {
                    NombreProyecto = r.GetString(0),
                    NombreMateria = r.IsDBNull(1) ? "" : r.GetString(1)
                });
            return lista;
        }

        /// <summary>Devuelve lista de nombres de materias para poblar ComboBox.</summary>
        public List<string> GetNombresMaterias()
        {
            var lista = new List<string>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT course_name FROM Course ORDER BY course_name";
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(r.GetString(0));
            return lista;
        }

        public void AgregarProyecto(ProyectoItem p)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            // Resolver id_course desde el nombre de materia elegido por el usuario
            var cmdId = conn.CreateCommand();
            cmdId.CommandText = "SELECT id_course FROM Course WHERE course_name=$n";
            cmdId.Parameters.AddWithValue("$n", p.NombreMateria);
            var idObj = cmdId.ExecuteScalar();
            if (idObj == null) return;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Project (project_name, id_course) VALUES ($pn,$ic)";
            cmd.Parameters.AddWithValue("$pn", p.NombreProyecto);
            cmd.Parameters.AddWithValue("$ic", Convert.ToInt32(idObj));
            cmd.ExecuteNonQuery();
        }

        public void EditarProyecto(ProyectoItem p, string nombreOriginal)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();

            var cmdId = conn.CreateCommand();
            cmdId.CommandText = "SELECT id_course FROM Course WHERE course_name=$n";
            cmdId.Parameters.AddWithValue("$n", p.NombreMateria);
            var idObj = cmdId.ExecuteScalar();
            if (idObj == null) return;

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Project SET project_name=$pn, id_course=$ic
                                 WHERE project_name=$original";
            cmd.Parameters.AddWithValue("$pn", p.NombreProyecto);
            cmd.Parameters.AddWithValue("$ic", Convert.ToInt32(idObj));
            cmd.Parameters.AddWithValue("$original", nombreOriginal);
            cmd.ExecuteNonQuery();

            // Propagar cambio de nombre a Multimedia si cambió
            if (p.NombreProyecto != nombreOriginal)
            {
                var cmdM = conn.CreateCommand();
                cmdM.CommandText = "UPDATE Multimedia SET project_name=$pn WHERE project_name=$o";
                cmdM.Parameters.AddWithValue("$pn", p.NombreProyecto);
                cmdM.Parameters.AddWithValue("$o", nombreOriginal);
                cmdM.ExecuteNonQuery();
            }
        }

        public void EliminarProyecto(string nombreProyecto)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Project WHERE project_name=$pn";
            cmd.Parameters.AddWithValue("$pn", nombreProyecto);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // MULTIMEDIA — PK compuesta (project_name, file_route), sin id ni capture_method
        // ══════════════════════════════════════════════════════════════════════
        public List<MultimediaItem> GetMultimedia()
        {
            var lista = new List<MultimediaItem>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT project_name, file_route, media_description
                                 FROM Multimedia
                                 ORDER BY project_name, file_route";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new MultimediaItem
                {
                    NombreProyecto = r.GetString(0),
                    Ruta = r.IsDBNull(1) ? "" : r.GetString(1),
                    Descripcion = r.IsDBNull(2) ? "" : r.GetString(2)
                });
            return lista;
        }

        /// <summary>Devuelve lista de nombres de proyectos para poblar ComboBox.</summary>
        public List<string> GetNombresProyectos()
        {
            var lista = new List<string>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT project_name FROM Project ORDER BY project_name";
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(r.GetString(0));
            return lista;
        }

        public void AgregarMultimedia(MultimediaItem m)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Multimedia (project_name, file_route, media_description)
                                 VALUES ($pn,$fr,$md)";
            cmd.Parameters.AddWithValue("$pn", m.NombreProyecto);
            cmd.Parameters.AddWithValue("$fr", m.Ruta);
            cmd.Parameters.AddWithValue("$md", m.Descripcion ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void EditarMultimedia(MultimediaItem m, string rutaOriginal)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            // La PK es (project_name, file_route) — identificamos por ambos originales
            cmd.CommandText = @"UPDATE Multimedia SET file_route=$fr, media_description=$md
                                 WHERE project_name=$pn AND file_route=$ro";
            cmd.Parameters.AddWithValue("$pn", m.NombreProyecto);
            cmd.Parameters.AddWithValue("$fr", m.Ruta);
            cmd.Parameters.AddWithValue("$md", m.Descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$ro", rutaOriginal);
            cmd.ExecuteNonQuery();
        }

        public void EliminarMultimedia(string nombreProyecto, string ruta)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Multimedia WHERE project_name=$pn AND file_route=$fr";
            cmd.Parameters.AddWithValue("$pn", nombreProyecto);
            cmd.Parameters.AddWithValue("$fr", ruta);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // VIDEOS 360
        // PK en BD: id_classroom (TEXT) — 1 registro por salón, sin id numérico
        // ══════════════════════════════════════════════════════════════════════
        public List<Video360Item> GetVideos360()
        {
            var lista = new List<Video360Item>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id_classroom, ruta_video360
                                 FROM Video360
                                 ORDER BY id_classroom";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Video360Item
                {
                    IDAula = r.GetString(0),
                    RutaVideo = r.IsDBNull(1) ? "" : r.GetString(1)
                });
            return lista;
        }

        // Actualiza únicamente la ruta; el salón ya existe (creado por excel_to_sqlite)
        public void ActualizarRutaVideo360(string idClassroom, string? ruta)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Video360
                                 SET ruta_video360 = $ruta
                                 WHERE id_classroom = $id";
            cmd.Parameters.AddWithValue("$id", idClassroom);
            cmd.Parameters.AddWithValue("$ruta", ruta ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // CÁMARAS
        // PK en BD: floor_num (INTEGER) — 1 cámara por piso, sin id_camera
        // ══════════════════════════════════════════════════════════════════════
        public List<Camara> GetCamaras()
        {
            var lista = new List<Camara>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT floor_num, IP_address, cam_capture_rute, camera_state
                                 FROM Camera
                                 ORDER BY floor_num";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Camara
                {
                    Piso = r.GetInt32(0),
                    IP = r.IsDBNull(1) ? "" : r.GetString(1),
                    Ruta = r.IsDBNull(2) ? "" : r.GetString(2),
                    EstaConectada = !r.IsDBNull(3) && r.GetInt32(3) == 1
                });
            return lista;
        }

        // Actualiza IP, ruta y estado de una cámara identificada por su piso
        public void ActualizarCamara(Camara c)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Camera
                                 SET IP_address       = $ip,
                                     cam_capture_rute = $ruta,
                                     camera_state     = $estado
                                 WHERE floor_num = $piso";
            cmd.Parameters.AddWithValue("$ip", string.IsNullOrEmpty(c.IP) ? DBNull.Value : c.IP);
            cmd.Parameters.AddWithValue("$ruta", string.IsNullOrEmpty(c.Ruta) ? DBNull.Value : c.Ruta);
            cmd.Parameters.AddWithValue("$estado", c.EstaConectada ? 1 : 0);
            cmd.Parameters.AddWithValue("$piso", c.Piso);
            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════════════════════
        // ADMINISTRADORES / USUARIOS
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Devuelve cuántos administradores hay en la BD.
        /// Se usa en el login para el fallback de primer arranque.
        /// </summary>
        public int ContarAdministradores()
        {
            if (!IsAvailable) return 0;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Administrator";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Valida credenciales. Devuelve el rol ("super" / "normal") si es correcto,
        /// null si el usuario no existe o la contraseña no coincide.
        /// </summary>
        public string? ValidarLogin(string usuario, string contrasena)
        {
            if (!IsAvailable) return null;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT role FROM Administrator
                                 WHERE user = $u AND password = $p
                                 LIMIT 1";
            cmd.Parameters.AddWithValue("$u", usuario);
            cmd.Parameters.AddWithValue("$p", contrasena);
            using var r = cmd.ExecuteReader();
            return r.Read() ? (r.IsDBNull(0) ? "normal" : r.GetString(0)) : null;
        }

        /// <summary>
        /// Lista todos los usuarios para mostrar en la tabla.
        /// La contraseña se incluye en el modelo pero se muestra enmascarada en la UI.
        /// </summary>
        public List<UsuarioItem> GetUsuarios()
        {
            var lista = new List<UsuarioItem>();
            if (!IsAvailable) return lista;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id_admin, user, password, role FROM Administrator ORDER BY user";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new UsuarioItem
                {
                    ID = r.GetInt32(0).ToString(),
                    NombreUsuario = r.IsDBNull(1) ? "" : r.GetString(1),
                    Password = r.IsDBNull(2) ? "" : r.GetString(2),
                    Role = r.IsDBNull(3) ? "normal" : r.GetString(3)
                });
            return lista;
        }

        public void AgregarUsuario(UsuarioItem u)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Administrator (user, password, role) VALUES ($u,$p,$r)";
            cmd.Parameters.AddWithValue("$u", u.NombreUsuario);
            cmd.Parameters.AddWithValue("$p", u.Password);
            cmd.Parameters.AddWithValue("$r", u.Role);
            cmd.ExecuteNonQuery();
        }

        public void EditarUsuario(UsuarioItem u)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Administrator
                                 SET user=$u, password=$p, role=$r
                                 WHERE id_admin=$id";
            cmd.Parameters.AddWithValue("$id", int.Parse(u.ID));
            cmd.Parameters.AddWithValue("$u", u.NombreUsuario);
            cmd.Parameters.AddWithValue("$p", u.Password);
            cmd.Parameters.AddWithValue("$r", u.Role);
            cmd.ExecuteNonQuery();
        }

        public void EliminarUsuario(string id)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Administrator WHERE id_admin=$id";
            cmd.Parameters.AddWithValue("$id", int.Parse(id));
            cmd.ExecuteNonQuery();
        }
        public void InsertarSalones(List<FilaAula> lista)
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            foreach (var a in lista)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT OR REPLACE INTO Classroom (classroom_name, floor_num) VALUES ($n,$p)";
                cmd.Parameters.AddWithValue("$n", a.Nombre);
                cmd.Parameters.AddWithValue("$p",
                    string.IsNullOrEmpty(a.Piso) ? DBNull.Value : int.Parse(a.Piso));

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        public void InsertarProfesores(List<Profesor> lista)
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            foreach (var p in lista)
            {
                if (!int.TryParse(p.Clave, out int idTeacher)) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO Teacher
                    (id_teacher, teacher_name, teacher_almamater, teacher_investigations,
                     teacher_professional_bio, teacher_photo, teacher_audio)
                    VALUES ($id,$n,$e,$i,$s,$f,$a)";

                cmd.Parameters.AddWithValue("$id", idTeacher);
                cmd.Parameters.AddWithValue("$n", p.Nombre);
                cmd.Parameters.AddWithValue("$e", p.Estudios ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$i", p.Investigaciones ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$s", p.Semblanza ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$f", p.Fotografia ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$a", p.Audio ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        public void InsertarMaterias(List<Materia> lista)
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            foreach (var m in lista)
            {
                if (!int.TryParse(m.ID, out int idCourse)) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO Course
                    (id_course, course_name, course_related_companies, course_syllabus_rute)
                    VALUES ($id,$n,$c,$r)";

                cmd.Parameters.AddWithValue("$id", idCourse);
                cmd.Parameters.AddWithValue("$n", m.Nombre);
                cmd.Parameters.AddWithValue("$c", m.Companias ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("$r", m.RutaTemario ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        public void InsertarHorarios(List<FilaHorario> lista)
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            foreach (var h in lista)
            {
                if (!int.TryParse(h.IDClase.Length >= 4 ? h.IDClase[..4] : h.IDClase, out int idCourse))
                    continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO Class
            (id_class, id_classroom, id_course, id_teacher,
             schedule_type, class_start_time, class_finish_time)
            VALUES ($id,$cl,$co,$t,$tp,$hi,$hf)";

                cmd.Parameters.AddWithValue("$id", h.IDClase);
                cmd.Parameters.AddWithValue("$cl", h.IDAula);
                cmd.Parameters.AddWithValue("$co", idCourse);
                cmd.Parameters.AddWithValue("$t", int.Parse(h.IDProfesor));
                cmd.Parameters.AddWithValue("$tp", h.Tipo);
                cmd.Parameters.AddWithValue("$hi", int.Parse(h.HoraInicio.Replace(":00", "")));
                cmd.Parameters.AddWithValue("$hf", int.Parse(h.HoraFin.Replace(":00", "")));

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        public void InsertarProyectos(List<ProyectoItem> lista)
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            foreach (var p in lista)
            {
                var cmdId = conn.CreateCommand();
                cmdId.CommandText = "SELECT id_course FROM Course WHERE course_name=$n";
                cmdId.Parameters.AddWithValue("$n", p.NombreMateria);
                var idObj = cmdId.ExecuteScalar();

                if (idObj == null) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT OR REPLACE INTO Project (project_name, id_course) VALUES ($pn,$ic)";
                cmd.Parameters.AddWithValue("$pn", p.NombreProyecto);
                cmd.Parameters.AddWithValue("$ic", Convert.ToInt32(idObj));

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        public void InsertarUsuarios(List<UsuarioItem> lista)
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            foreach (var u in lista)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO Administrator (user, password, role)
                            VALUES ($u,$p,$r)";

                cmd.Parameters.AddWithValue("$u", u.NombreUsuario);
                cmd.Parameters.AddWithValue("$p", u.Password);
                cmd.Parameters.AddWithValue("$r", u.Role ?? "normal");

                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        public void LimpiarBaseDeDatos()
        {
            if (!IsAvailable) return;

            using var conn = GetConnection();
            conn.Open();

            // Microsoft.Data.Sqlite no ejecuta múltiples statements en una sola llamada —
            // hay que hacer un ExecuteNonQuery por sentencia.
            void Exec(string sql)
            {
                var c = conn.CreateCommand();
                c.CommandText = sql;
                c.ExecuteNonQuery();
            }

            Exec("PRAGMA foreign_keys = OFF");

            // Primero las tablas con FK para evitar conflictos
            Exec("DELETE FROM Multimedia");
            Exec("DELETE FROM Class");
            Exec("DELETE FROM Project");
            Exec("DELETE FROM Video360");
            Exec("DELETE FROM Camera");
            Exec("DELETE FROM Administrator");
            Exec("DELETE FROM Teacher");
            Exec("DELETE FROM Course");
            Exec("DELETE FROM Classroom");

            Exec("PRAGMA foreign_keys = ON");
        }

        public void InsertarMultimedia(List<MultimediaItem> lista)
        {
            if (!IsAvailable) return;
            using var conn = GetConnection(); conn.Open();
            using var transaction = conn.BeginTransaction();
            foreach (var m in lista)
            {
                if (string.IsNullOrWhiteSpace(m.NombreProyecto) ||
                    string.IsNullOrWhiteSpace(m.Ruta)) continue;
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO Multimedia
                    (project_name, file_route, media_description)
                    VALUES ($pn,$fr,$md)";
                cmd.Parameters.AddWithValue("$pn", m.NombreProyecto);
                cmd.Parameters.AddWithValue("$fr", m.Ruta);
                cmd.Parameters.AddWithValue("$md", m.Descripcion ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }
    }

}