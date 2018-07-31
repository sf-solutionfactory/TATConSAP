using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using TAT001.Services;
using TATconexionSAP.Entities;

namespace TATconexionSAP.Services
{
    public class Modelos
    {
        #region Variables Globales     
        private TAT001Entities db = new TAT001Entities();
        public string sap = "\\SAP";
        public string datasync = "\\DATA_SYNC";
        public string dataproc = "\\DATA_PROC";
        #endregion
        public void leerArchivos()
        {
            APPSETTING sett = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") & x.ACTIVO).FirstOrDefault();//RSG 30.07.2018
            if (sett == null)
                return;
            //var cadena = ConfigurationManager.AppSettings["url"];
            var cadena = sett.VALUE;
            List<doc> lstd = new List<doc>();
            List<string> archivos2 = new List<string>();
            try
            {
                string[] archivos = Directory.GetFiles(cadena += sap += datasync, "*.txt", SearchOption.AllDirectories);
                Console.WriteLine(archivos.Length + " _1");//RSG 30.07.2018
                //en este for sabre cuales archivos usar
                for (int i = 0; i < archivos.Length; i++)
                {
                    //separo por carpetas
                    string[] varx = archivos[i].Split('\\');
                    //separo especificamente el nombre para saber si es BUDG O LOG
                    string[] varNA = varx[varx.Length - 1].Split('_');
                    if (varNA[1] == "LOG")
                    {
                        archivos2.Add(archivos[i]);
                    }
                }
                Console.WriteLine(archivos2.Count + " _2");//RSG 30.07.2018
                //en este for armo un objeto para posterior manipular hacia la bd
                for (int i = 0; i < archivos2.Count; i++)
                {
                    //Leo todas las lineas del archivo
                    string[] readText = File.ReadAllLines(archivos2[i]);
                    foreach (var item in readText)
                    {
                        doc d = new doc();
                        string[] val = item.Split('|');
                        if (val != null)
                        {
                            if (val[1] == "Error")
                            {
                                d.numero_TAT = val[0];
                                d.Mensaje = val[1];
                                d.Cuenta_cargo = Convert.ToInt64(val[5]);
                                d.Cuenta_abono = Convert.ToInt64(val[6]);
                            }
                            else
                            {
                                d.numero_TAT = val[0];
                                d.Mensaje = val[1];
                                d.Num_doc_SAP = int.Parse(val[2]);
                                d.Sociedad = val[3];
                                d.Año = int.Parse(val[4]);
                                d.Cuenta_cargo = Convert.ToInt64(val[5]);
                                d.Cuenta_abono = Convert.ToInt64(val[6]);
                            }
                            lstd.Add(d);
                        }
                    }
                }
                validarBd(lstd, archivos2);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void validarBd(List<doc> lstd, List<string> archivos)
        {
            int x = 0;
            for (int i = 0; i < lstd.Count; i++)
            {
                decimal de = Convert.ToDecimal(lstd[i].numero_TAT);
                //Corroboro que exista la informacion
                var dA = db.DOCUMENTOes.Where(y => y.NUM_DOC == de).FirstOrDefault();
                //si encuentra una coincidencia
                if (dA != null)
                {
                    //para el estatus E/X
                    if (lstd[i].Mensaje == string.Empty)
                    {
                        dA.ESTATUS_SAP = "X";
                    }
                    else if (lstd[i].Mensaje != string.Empty)
                    {
                        if (lstd[i].Mensaje.Equals("Error"))
                        {
                            dA.ESTATUS_SAP = "E";
                        }
                        if (lstd[i].Mensaje.Equals("Success"))
                        {
                            dA.ESTATUS_SAP = "X";
                        }
                    }
                    try
                    {
                        //Hacemos el update en BD
                        dA.DOCUMENTO_SAP = lstd[i].Num_doc_SAP.ToString();
                        db.Entry(dA).State = EntityState.Modified;
                        x = x + db.SaveChanges();
                        //Agregamos en la tabla los valores
                        DOCUMENTOSAP ds = new DOCUMENTOSAP();
                        ds.NUM_DOC = int.Parse(lstd[i].numero_TAT);
                        ds.BUKRS = lstd[i].Sociedad;
                        ds.EJERCICIO = lstd[i].Año;
                        ds.CUENTA_A = lstd[i].Cuenta_abono.ToString();
                        ds.CUENTA_C = lstd[i].Cuenta_cargo.ToString();
                        try
                        {
                            db.DOCUMENTOSAPs.Add(ds);
                            db.SaveChanges();
                            moverArchivo(archivos[i]);
                        }
                        catch
                        {
                            ds = db.DOCUMENTOSAPs.Where(a => a.NUM_DOC == ds.NUM_DOC).FirstOrDefault();
                            ds.BUKRS = lstd[i].Sociedad;
                            ds.EJERCICIO = lstd[i].Año;
                            ds.CUENTA_A = lstd[i].Cuenta_abono.ToString();
                            ds.CUENTA_C = lstd[i].Cuenta_cargo.ToString();
                            db.Entry(ds).State = EntityState.Modified;
                            db.SaveChanges();
                            moverArchivo(archivos[i]);
                        }


                    }
                    catch (Exception varEx)
                    {
                        var ex = varEx.ToString();
                    }

                    if (dA.DOCUMENTO_REF != null)
                    {
                        if (dA.DOCUMENTO_REF > 0)
                        {
                            List<DOCUMENTO> rela = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == dA.DOCUMENTO_REF).ToList();
                            DOCUMENTO parcial = rela.Where(a => a.TSOL_ID == "RP").FirstOrDefault();
                            if (parcial != null)
                            {
                                bool contabilizados = true;
                                foreach (DOCUMENTO rel in rela)
                                {
                                    if (rel.TSOL_ID == "RP")
                                        if (rel.ESTATUS_SAP == "X")
                                            contabilizados = false;
                                }

                                if (contabilizados)
                                {
                                    FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC == parcial.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();
                                    if (f != null)
                                    {
                                        f.ESTATUS = "A";
                                        f.FECHAM = DateTime.Now;
                                        ProcesaFlujo p = new ProcesaFlujo();
                                        string res = p.procesa(f, "");

                                        if (res == "0" | res == "")
                                        {
                                            FLUJO f1 = db.FLUJOes.Where(a => a.NUM_DOC == parcial.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();

                                            f.ESTATUS = "A";
                                            f.FECHAM = DateTime.Now;
                                            res = p.procesa(f, "");
                                        }

                                        //if (res == "0" | res == "")
                                    }
                                }

                            }
                        }
                    }
                }
            }
            try
            {
                //if (x == lstd.Count)
                //{
                //    moverArchivos(archivos);
                //}
            }
            catch (Exception varEx)
            {
                var ex = varEx.ToString();
                throw new Exception(ex);
            }
        }

        public void moverArchivos(List<string> archivo)
        {
            for (int i = 0; i < archivo.Count; i++)
            {
                try
                {
                    var from = Path.Combine(archivo[i]);
                    var arc2 = archivo[i].Replace(datasync, dataproc);
                    var to = Path.Combine(arc2);

                    File.Move(from, to); // Try to move
                }
                catch (IOException ex)
                {
                    // Console.WriteLine(ex); // Write error
                    throw new Exception(ex.Message);
                }
            }
        }

        public void moverArchivo(string archivo)
        {
            try
            {
                var from = Path.Combine(archivo);
                var arc2 = archivo.Replace(datasync, dataproc);
                var to = Path.Combine(arc2);

                File.Move(from, to); // Try to move
            }
            catch (IOException ex)
            {
                // Console.WriteLine(ex); // Write error
                throw new Exception(ex.Message);
            }
        }
    }
}
