using ML;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;

namespace BL
{
    public class Cargo
    {
        public static ML.Result Add(ML.Cargo cargo)
        {
            ML.Result result = new ML.Result();

            try
            {
                using (DL.NextTechnologiesEntities context = new DL.NextTechnologiesEntities())
                {
                    var restulQuery = context.CargoAdd(cargo.id, cargo.name, cargo.company_id, decimal.Parse(cargo.amount), cargo.status, cargo.created_at, cargo.paid_at);


                    if (restulQuery >= 1)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se insertó el registro";
                    }

                    result.Correct = true;

                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public static ML.Result GetAll()
        {
            ML.Result result = new ML.Result();

            try
            {
                using (DL.NextTechnologiesEntities context = new DL.NextTechnologiesEntities())
                {
                    var query = context.CargoGetAll().ToList();

                    result.Objects = new List<object>();

                    if (query != null)
                    {
                        foreach (var obj in query)
                        {
                            ML.Cargo cargo = new ML.Cargo();

                            cargo.id = obj.id;
                            cargo.name = obj.company_name;
                            cargo.company_id = obj.company_id;
                            cargo.amount = decimal.Parse(cargo.amount).ToString();
                            cargo.status = obj.status;
                            cargo.created_at = obj.created_at.ToString("dd/MM/yyyy");
                            cargo.paid_at = obj.updated_at.ToString("dd/MM/yyyy");


                            result.Objects.Add(cargo);
                        }

                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se encontraron registros.";
                    }

                }
            }
            catch (Exception Ex)
            {
                result.Correct = false;
                result.ErrorMessage = Ex.Message;
            }

            return result;
        }


        public static Result ConvertToDataTable(string strFilePath, string connString)
        {
            Result result = new Result();

            try
            {
                using (OleDbConnection context = new OleDbConnection(connString))
                {
                    string query = "SELECT * FROM [$data_prueba_tecnica]";
                    using (OleDbCommand cmd = new OleDbCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Connection = context;


                        OleDbDataAdapter da = new OleDbDataAdapter();
                        da.SelectCommand = cmd;
                        DataTable tableCargo = new DataTable();

                        da.Fill(tableCargo);

                        result.Object = tableCargo;

                        if (tableCargo.Rows.Count > 1)
                        {
                            result.Correct = true;
                        }
                        else
                        {
                            result.Correct = false;
                            result.ErrorMessage = "No existen registros en el excel";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;

            }

            return result;

        }

        public static Result ValidarExcel(DataTable tableEmpleado)
        {
            Result result = new Result();

            try
            {
                result.Objects = new List<object>();
                //DataTable  //Rows //Columns
                foreach (DataRow row in tableEmpleado.Rows)
                {
                    ErrorExcel error = new ErrorExcel();

                    if (row[0].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el id de cargo. ";
                    }
                    if (row[1].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el RFC del cargo. ";
                    }
                    if (row[2].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el Nombre del cargo. ";
                    }
                    if (row[3].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el Apellido Paterno del cargo. ";
                    }
                    if (row[4].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el Apellido Materno del cargo. ";
                    }
                    if (row[5].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el Email del cargo. ";
                    }
                    if (row[6].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el Télefono del cargo. ";
                    }
                    if (row[7].ToString() == "")
                    {
                        error.Message += "Por favor ingrese la Fecha de nacimiento del cargo. ";
                    }
                    if (row[8].ToString() == "")
                    {
                        error.Message += "Por favor ingrese el NSS del cargo. ";
                    }
                    if (row[9].ToString() == "")
                    {
                        error.Message += "Por favor ingrese la Fecha de ingreso del cargo. ";
                    }

                    if (error.Message != null)
                    {
                        result.Objects.Add(error);
                    }
                    result.Correct = true;
                }
            }
            catch (Exception ex)
            {
                result.Correct = false;
                result.ErrorMessage = ex.Message;

            }

            return result;

        }

        public static ML.Result ReadPersons(string direccionExcel)
        {
            ML.Result result = new ML.Result();
            result.Objects = new List<object>();
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var reader = new StreamReader(direccionExcel))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<ML.Cargo>();

                foreach (var item in records)
                {
                    result.Objects.Add(item);

                }
            }

            return result;
        }
    }
}
