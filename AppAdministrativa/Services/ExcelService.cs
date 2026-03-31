using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace AppAdministrativa.Services
{
    public class ExcelService
    {
        // ══════════════════════════════════════════════════════════════════════
        // EXPORTACIÓN
        // ══════════════════════════════════════════════════════════════════════
        public void ExportarTodo(DatabaseService db, string ruta)
        {
            var wb = new XLWorkbook();

            ExportarSalones(db.GetSalones(), wb);
            ExportarProfesores(db.GetProfesores(), wb);
            ExportarMaterias(db.GetMaterias(), wb);
            ExportarHorarios(db.GetHorarios(), wb);
            ExportarProyectos(db.GetProyectos(), wb);
            ExportarMultimedia(db.GetMultimedia(), wb);
            ExportarVideos360(db.GetVideos360(), wb);
            ExportarCamaras(db.GetCamaras(), wb);
            ExportarUsuarios(db.GetUsuarios(), wb);

            wb.SaveAs(ruta);
        }

        private void EstiloEncabezado(IXLWorksheet ws, int cols)
        {
            var rng = ws.Range(1, 1, 1, cols);
            rng.Style.Font.Bold = true;
            rng.Style.Fill.BackgroundColor = XLColor.FromHtml("#678EC2");
            rng.Style.Font.FontColor = XLColor.White;
        }

        private void ExportarSalones(List<FilaAula> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Salones");
            ws.Cell(1, 1).Value = "id_salon";
            ws.Cell(1, 2).Value = "piso_salon";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].Nombre;
                ws.Cell(i + 2, 2).Value = data[i].Piso;
            }
            EstiloEncabezado(ws, 2);
            ws.Columns().AdjustToContents();
        }

        private void ExportarProfesores(List<Profesor> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Maestros");
            ws.Cell(1, 1).Value = "clave_maestro";
            ws.Cell(1, 2).Value = "nombre_maestro";
            ws.Cell(1, 3).Value = "estudios_maestro";
            ws.Cell(1, 4).Value = "investigaciones_maestro";
            ws.Cell(1, 5).Value = "semblanza_maestro";
            ws.Cell(1, 6).Value = "ruta_fotografía";
            ws.Cell(1, 7).Value = "ruta_audio";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].Clave;
                ws.Cell(i + 2, 2).Value = data[i].Nombre;
                ws.Cell(i + 2, 3).Value = data[i].Estudios;
                ws.Cell(i + 2, 4).Value = data[i].Investigaciones;
                ws.Cell(i + 2, 5).Value = data[i].Semblanza;
                ws.Cell(i + 2, 6).Value = data[i].Fotografia;
                ws.Cell(i + 2, 7).Value = data[i].Audio;
            }
            EstiloEncabezado(ws, 7);
            ws.Columns().AdjustToContents();
        }

        private void ExportarMaterias(List<Materia> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Materias");
            ws.Cell(1, 1).Value = "clave_materia";
            ws.Cell(1, 2).Value = "nombre_materia";
            ws.Cell(1, 3).Value = "companias_relacionadas";
            ws.Cell(1, 4).Value = "ruta_temario";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].ID;
                ws.Cell(i + 2, 2).Value = data[i].Nombre;
                ws.Cell(i + 2, 3).Value = data[i].Companias;
                ws.Cell(i + 2, 4).Value = data[i].RutaTemario;
            }
            EstiloEncabezado(ws, 4);
            ws.Columns().AdjustToContents();
        }

        private void ExportarHorarios(List<FilaHorario> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Clases");
            ws.Cell(1, 1).Value = "clave_curso";
            ws.Cell(1, 2).Value = "tipo_horario";
            ws.Cell(1, 3).Value = "horario_inicio";
            ws.Cell(1, 4).Value = "horario_final";
            ws.Cell(1, 5).Value = "clave_maestro";
            ws.Cell(1, 6).Value = "id_salon";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].IDClase;
                ws.Cell(i + 2, 2).Value = data[i].Tipo;
                // Guardar como entero (9, 10...) igual que el Excel original
                ws.Cell(i + 2, 3).Value = int.Parse(data[i].HoraInicio.Replace(":00", ""));
                ws.Cell(i + 2, 4).Value = int.Parse(data[i].HoraFin.Replace(":00", ""));
                ws.Cell(i + 2, 5).Value = data[i].IDProfesor;
                ws.Cell(i + 2, 6).Value = data[i].IDAula;
            }
            EstiloEncabezado(ws, 6);
            ws.Columns().AdjustToContents();
        }

        private void ExportarProyectos(List<ProyectoItem> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Proyectos");
            ws.Cell(1, 1).Value = "clave_materia";
            ws.Cell(1, 2).Value = "nombre_proyecto";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].NombreMateria;
                ws.Cell(i + 2, 2).Value = data[i].NombreProyecto;
            }
            EstiloEncabezado(ws, 2);
            ws.Columns().AdjustToContents();
        }

        private void ExportarMultimedia(List<MultimediaItem> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Multimedia_proyectos");
            ws.Cell(1, 1).Value = "nombre_proyecto";
            ws.Cell(1, 2).Value = "ruta_mult_proyecto";
            ws.Cell(1, 3).Value = "descripcion_proyecto";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].NombreProyecto;
                ws.Cell(i + 2, 2).Value = data[i].Ruta;
                ws.Cell(i + 2, 3).Value = data[i].Descripcion;
            }
            EstiloEncabezado(ws, 3);
            ws.Columns().AdjustToContents();
        }

        private void ExportarVideos360(List<Video360Item> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Videos_360");
            ws.Cell(1, 1).Value = "id_salon";
            ws.Cell(1, 2).Value = "ruta_video360";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].IDAula;
                ws.Cell(i + 2, 2).Value = data[i].RutaVideo;
            }
            EstiloEncabezado(ws, 2);
            ws.Columns().AdjustToContents();
        }

        private void ExportarCamaras(List<Camara> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Camaras");
            ws.Cell(1, 1).Value = "camara_piso";
            ws.Cell(1, 2).Value = "direccion_IP";
            ws.Cell(1, 3).Value = "ruta_guardado_camara";
            ws.Cell(1, 4).Value = "estado_camara";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].Piso;
                ws.Cell(i + 2, 2).Value = data[i].IP;
                ws.Cell(i + 2, 3).Value = data[i].Ruta;
                ws.Cell(i + 2, 4).Value = data[i].EstaConectada ? 1 : 0;
            }
            EstiloEncabezado(ws, 4);
            ws.Columns().AdjustToContents();
        }

        private void ExportarUsuarios(List<UsuarioItem> data, XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Administradores");
            ws.Cell(1, 1).Value = "usuario";
            ws.Cell(1, 2).Value = "contrasena";
            ws.Cell(1, 3).Value = "rol";
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].NombreUsuario;
                ws.Cell(i + 2, 2).Value = data[i].Password;
                ws.Cell(i + 2, 3).Value = data[i].Role;
            }
            EstiloEncabezado(ws, 3);
            ws.Columns().AdjustToContents();
        }

        // ══════════════════════════════════════════════════════════════════════
        // IMPORTACIÓN
        // ══════════════════════════════════════════════════════════════════════
        public void ImportarTodo(DatabaseService db, string ruta)
        {
            if (!File.Exists(ruta))
                throw new Exception("El archivo no existe.");

            using var wb = new XLWorkbook(ruta);

            // Limpiar y reimportar en orden correcto (FK: primero tablas padre)
            db.LimpiarBaseDeDatos();

            ImportarSalones(db, wb);       // Classroom
            ImportarProfesores(db, wb);    // Teacher
            ImportarMaterias(db, wb);      // Course
            ImportarProyectos(db, wb);     // Project (depende de Course)
            ImportarHorarios(db, wb);      // Class (depende de Classroom + Course + Teacher)
            ImportarMultimedia(db, wb);    // Multimedia (depende de Project)
            ImportarVideos360(db, wb);     // Video360 (depende de Classroom)
            ImportarCamaras(db, wb);       // Camera
            ImportarUsuarios(db, wb);      // Administrator
        }

        private void ImportarSalones(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Salones", out var ws)) return;
            var lista = new List<FilaAula>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                lista.Add(new FilaAula
                {
                    Nombre = ws.Cell(row, 1).GetString().Trim(),
                    Piso = ws.Cell(row, 2).GetString().Trim()
                });
                row++;
            }
            db.InsertarSalones(lista);
        }

        private void ImportarProfesores(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Maestros", out var ws)) return;
            var lista = new List<Profesor>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                lista.Add(new Profesor
                {
                    Clave = ws.Cell(row, 1).GetString().Trim(),
                    Nombre = ws.Cell(row, 2).GetString().Trim(),
                    Estudios = ws.Cell(row, 3).GetString(),
                    Investigaciones = ws.Cell(row, 4).GetString(),
                    Semblanza = ws.Cell(row, 5).GetString(),
                    Fotografia = ws.Cell(row, 6).GetString(),
                    Audio = ws.Cell(row, 7).GetString()
                });
                row++;
            }
            db.InsertarProfesores(lista);
        }

        private void ImportarMaterias(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Materias", out var ws)) return;
            var lista = new List<Materia>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                lista.Add(new Materia
                {
                    ID = ws.Cell(row, 1).GetString().Trim(),
                    Nombre = ws.Cell(row, 2).GetString().Trim(),
                    Companias = ws.Cell(row, 3).GetString(),
                    RutaTemario = ws.Cell(row, 4).GetString()
                });
                row++;
            }
            db.InsertarMaterias(lista);
        }

        private void ImportarProyectos(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Proyectos", out var ws)) return;
            var lista = new List<ProyectoItem>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                lista.Add(new ProyectoItem
                {
                    NombreMateria = ws.Cell(row, 1).GetString().Trim(),
                    NombreProyecto = ws.Cell(row, 2).GetString().Trim()
                });
                row++;
            }
            db.InsertarProyectos(lista);
        }

        private void ImportarHorarios(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Clases", out var ws)) return;
            var lista = new List<FilaHorario>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                string inicio = ws.Cell(row, 3).GetString().Trim();
                string fin = ws.Cell(row, 4).GetString().Trim();

                // Normalizar: acepta "9", "09", "09:00" → siempre "09:00"
                if (int.TryParse(inicio, out int h1)) inicio = $"{h1:D2}:00";
                if (int.TryParse(fin, out int h2)) fin = $"{h2:D2}:00";

                lista.Add(new FilaHorario
                {
                    IDClase = ws.Cell(row, 1).GetString().Trim(),
                    Tipo = ws.Cell(row, 2).GetString().Trim(),
                    HoraInicio = inicio,
                    HoraFin = fin,
                    IDProfesor = ws.Cell(row, 5).GetString().Trim(),
                    IDAula = ws.Cell(row, 6).GetString().Trim()
                });
                row++;
            }
            db.InsertarHorarios(lista);
        }

        private void ImportarMultimedia(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Multimedia_proyectos", out var ws)) return;
            var lista = new List<MultimediaItem>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                lista.Add(new MultimediaItem
                {
                    NombreProyecto = ws.Cell(row, 1).GetString().Trim(),
                    Ruta = ws.Cell(row, 2).GetString().Trim(),
                    Descripcion = ws.Cell(row, 3).GetString()
                });
                row++;
            }
            db.InsertarMultimedia(lista);
        }

        private void ImportarVideos360(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Videos_360", out var ws)) return;
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                string aula = ws.Cell(row, 1).GetString().Trim();
                string ruta = ws.Cell(row, 2).GetString().Trim();
                db.ActualizarRutaVideo360(aula, ruta);
                row++;
            }
        }

        private void ImportarCamaras(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Camaras", out var ws)) return;
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                if (!int.TryParse(ws.Cell(row, 1).GetString().Trim(), out int piso)) { row++; continue; }
                var c = new Camara
                {
                    Piso = piso,
                    IP = ws.Cell(row, 2).GetString().Trim(),
                    Ruta = ws.Cell(row, 3).GetString().Trim(),
                    EstaConectada = ws.Cell(row, 4).GetString().Trim() == "1"
                };
                db.ActualizarCamara(c);
                row++;
            }
        }

        private void ImportarUsuarios(DatabaseService db, XLWorkbook wb)
        {
            if (!wb.TryGetWorksheet("Administradores", out var ws)) return;
            var lista = new List<UsuarioItem>();
            int row = 2;
            while (!ws.Cell(row, 1).IsEmpty())
            {
                lista.Add(new UsuarioItem
                {
                    NombreUsuario = ws.Cell(row, 1).GetString().Trim(),
                    Password = ws.Cell(row, 2).GetString().Trim(),
                    Role = ws.Cell(row, 3).GetString().Trim()
                });
                row++;
            }
            db.InsertarUsuarios(lista);
        }
    }
}