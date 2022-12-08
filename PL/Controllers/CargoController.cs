using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PL.Controllers
{
    public class CargoController : Controller
    {
        // GET: CargoCargaMasiva


        [HttpGet]
        public ActionResult GetAll()
        {
            ML.Cargo cargo = new ML.Cargo();
            


            ML.Result result = BL.Cargo.GetAll();

            if (result.Correct)
            {
                cargo.Cargos = result.Objects;
                
            }
            else
            {
                ViewBag.Message = "Ocurrio un error al traer la información";
            }

            return View(cargo);
        }

        [HttpGet]
        public ActionResult CargaMasiva()
        {
            //ML.Cargo cargo = new ML.Cargo()
            ML.ErrorExcel errorExcel = new ML.ErrorExcel();

            errorExcel.Errores = new List<object>();
            errorExcel.Cargo = new ML.Cargo();


            ML.Result result = BL.Cargo.GetAll();

            if (Session["RutaExcel"] != null)
            {
                errorExcel.Cargo.Cargos = result.Objects;
                errorExcel.Correct = true;
            }

            return View(errorExcel);
        }

        [HttpPost]
        public ActionResult CargaMasiva(ML.ErrorExcel errorItem)
        {
            if (Session["RutaExcel"] != null) //Que ya tiene la ruta del archivo
            {
                string direccionExcel = (string)Session["RutaExcel"];
                string CadenaConexion = System.Configuration.ConfigurationManager.AppSettings["ConexionExcel"].ToString();
                string ConnectionString = CadenaConexion + direccionExcel;


                ML.Result resultDataTable = BL.Cargo.ConvertToDataTable(direccionExcel, ConnectionString);

                if (resultDataTable.Correct)
                {
                    string ErrorMessage = " ";
                    DataTable tableCargo = (DataTable)resultDataTable.Object;//unboxing
                    foreach (DataRow row in tableCargo.Rows)
                    {
                        ML.Cargo cargo = new ML.Cargo();

                        cargo.id = row[0].ToString();
                        cargo.name = row[1].ToString();
                        cargo.company_id = row[2].ToString();
                        cargo.amount = decimal.Parse(row[3].ToString());
                        cargo.status = row[4].ToString();
                        cargo.created_at = row[5].ToString();
                        cargo.paid_at = row[6].ToString();


                        ML.Result resultCargo = BL.Cargo.Add(cargo);

                        if (!resultCargo.Correct)
                        {
                            ErrorMessage += resultCargo.ErrorMessage;
                        }
                    }

                    if (ErrorMessage == "")
                    {
                        ViewBag.Message = "Los cargos han sido cargados correctamente";
                    }
                    else
                    {
                        ViewBag.Message = "Ocurrió un error al insertar los cargos" + ErrorMessage;
                    }

                }

                //Cargar el archivo
            }
            else
            {
                //Validar el archivo
                HttpPostedFileBase file = Request.Files["ExcelCargos"];

                if (file.ContentLength > 0)//Si el usuario seleccionó un excel
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    if (extension == ".csv")
                    {
                        string direccionExcel = Server.MapPath("~/ExcelCargaMasiva/") + Path.GetFileNameWithoutExtension(file.FileName) + '-' + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

                        if (!System.IO.File.Exists(direccionExcel))
                        {
                           
                            try
                            {

                                file.SaveAs(direccionExcel);
                                Session["RutaExcel"] = direccionExcel;
                                string CadenaConexion = System.Configuration.ConfigurationManager.AppSettings["ConexionExcel"].ToString();
                                string ConnectionString = CadenaConexion + direccionExcel;

                                //.Result resultDataTable = BL.Cargo.ConvertToDataTable(direccionExcel, ConnectionString);
                                ML.Result resultDataTable = BL.Cargo.ReadPersons(direccionExcel);

                                if (resultDataTable.Correct)
                                {
                                    DataTable tableCargo = (DataTable)resultDataTable.Object;//unboxing
                                    ML.Result resultValidarExcel = BL.Cargo.ValidarExcel(tableCargo);
                                    if (!resultValidarExcel.Correct) //si hubo errores
                                    {
                                        ML.ErrorExcel error = new ML.ErrorExcel();
                                        error.Errores = resultValidarExcel.Objects;
                                        return View(error);
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Todos los registros han sido validados correctamente, puede proceder a cargarlos";
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = ex.Message;
                            }

                        }
                        else
                        {
                            ViewBag.Message = "Ya existe el nombre del archivo, por favor renombrarlo";
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Seleccione un archivo con extensión .xlsx";
                    }
                }
                else
                {
                    ViewBag.Message = "Seleccione un archivo";
                }
            }
            return PartialView("ValidationModal");
        }

        //protected void ImportCSV(object sender, EventArgs e)
        //{
        //    //Upload and save the file
        //    string csvPath = Server.MapPath("~/Files/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
        //    FileUpload1.SaveAs(csvPath);

        //    //Create a DataTable.
        //    DataTable dt = new DataTable();
        //    dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Id", typeof(int)),
        //    new DataColumn("Name", typeof(string)),
        //    new DataColumn("Technology", typeof(string)),
        //    new DataColumn("Company", typeof(string)),
        //    new DataColumn("Country",typeof(string)) });

        //    //Read the contents of CSV file.
        //    string csvData = File.ReadAllText(csvPath);

        //    //Execute a loop over the rows.
        //    foreach (string row in csvData.Split('\n'))
        //    {
        //        if (!string.IsNullOrEmpty(row))
        //        {
        //            dt.Rows.Add();
        //            int i = 0;

        //            //Execute a loop over the columns.
        //            foreach (string cell in row.Split(','))
        //            {
        //                dt.Rows[dt.Rows.Count - 1][i] = cell;
        //                i++;
        //            }
        //        }
        //    }

        //    //Bind the DataTable.
        //    GridView1.DataSource = dt;
        //    GridView1.DataBind();
        //}
    }
}