using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using TAT001.Services;
using TATconexionSAP.Entities;
using TATconexionSAP.Services;

namespace TATconexionSAP.Services
{
    public class Modelos
    {
        #region Variables Globales     
        private TAT001Entities db = new TAT001Entities();
        public string sap = "\\SAP";
        public string datasync = "\\DATA_SYNC";
        public string dataproc = "\\DATA_PROC";
        Log log = new Log();
        #endregion
        public List<string> leerArchivos()
        {
            APPSETTING lg = db.APPSETTINGs.Where(x => x.NOMBRE == "logPath" & x.ACTIVO == true).FirstOrDefault();
            log.ruta = lg.VALUE + "ConexionSAP_";
            log.escribeLog("-----------------------------------------------------------------------START");

            List<string> errores = new List<string>();
            APPSETTING sett = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") & x.ACTIVO).FirstOrDefault();//RSG 30.07.2018
            if (sett == null) { errores.Add("Falta configuración de PATH!"); }
            //var cadena = ConfigurationManager.AppSettings["url"];
            var cadena = sett.VALUE;
            List<doc> lstd = new List<doc>();
            List<string> archivos2 = new List<string>();
            try
            {
                string[] archivos = Directory.GetFiles(cadena += sap += datasync, "*.txt", SearchOption.AllDirectories);
                Console.WriteLine(archivos.Length + " _1");//RSG 30.07.2018
                log.escribeLog("Archivos en carpeta: " + archivos.Length);
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
                log.escribeLog("Archivos tipo LOG: " + archivos2.Count);
                //en este for armo un objeto para posterior manipular hacia la bd
                for (int i = 0; i < archivos2.Count; i++)
                {
                    //Leo todas las lineas del archivo
                    //string[] readText = File.ReadAllLines(archivos2[i]);
                    //foreach (var item in readText)
                    int cont = 1;
                    foreach (var item in File.ReadLines(archivos2[i]))
                    {
                        doc d = new doc();
                        string[] val = item.Split('|');
                        if (val != null)
                        {
                            if (item != "")
                                if (val.Length < 2)
                                {
                                    log.escribeLog("Archivo inválido" + archivos2[i]);
                                    errores.Add("Archivo inválido" + archivos2[i]);
                                }
                                else
                                {

                                    if (val[1] == "Error")
                                    {
                                        d.numero_TAT = val[0];
                                        d.Mensaje = val[1];
                                        //d.Cuenta_cargo = Convert.ToInt64(val[5]);
                                        //d.Cuenta_abono = Convert.ToInt64(val[6]);   
                                        log.escribeLog("(E) - NUM_DOC:" + val[0] + " -- MENSAJE: " + val[1]);
                                    }
                                    else
                                    {

                                        log.escribeLog("(S) - NUM_DOC:" + val[0] + " -- MENSAJE: " + val[1] + " -- NUM_SAP: " + val[2]);
                                        d.numero_TAT = val[0];
                                        d.Mensaje = val[1];
                                        d.Num_doc_SAP = decimal.Parse(val[2]);
                                        d.Sociedad = val[3];
                                        d.Año = int.Parse(val[4]);
                                        d.Cuenta_cargo = Convert.ToInt64(val[5]);
                                        d.Cuenta_abono = Convert.ToInt64(val[6]);
                                        try
                                        {
                                            d.blart = val[7];
                                            d.kunnr = val[8];
                                            d.desc = val[9];
                                            d.importe = decimal.Parse(val[10]);
                                            d.fechac = decimal.Parse(val[11]);
                                        }
                                        catch { }
                                    }
                                    d.pos = cont;
                                    d.file = archivos2[i];
                                    lstd.Add(d);
                                }
                        }
                        cont++;
                    }
                }
                foreach (doc d in lstd)
                {
                    if (d.pos == lstd.OrderByDescending(x => x.pos).First().pos)
                        d.last = true;
                    else
                        d.last = false;
                }
                log.escribeLog("-------------------------------");
                log.escribeLog("DOCS: " + lstd.Count + "ARCHIVOS: " + archivos2.Count);
                log.escribeLog("-------------------------------ValidarBD");
                validarBd(lstd, archivos2);
            }
            catch (Exception ex)
            {
                log.escribeLog("ERROR LOG: " + ex.Message);
                errores.Add(ex.Message);
                throw new Exception(ex.Message);
            }
            return errores;
        }

        public void validarBd(List<doc> lstd, List<string> archivos)
        {
            int x = 0;
            for (int i = 0; i < lstd.Count; i++)
            {
                decimal de = Convert.ToDecimal(lstd[i].numero_TAT);
                log.escribeLog("NUM_DOC: " + de);
                //Corroboro que exista la informacion
                var dA = db.DOCUMENTOes.Where(y => y.NUM_DOC == de).FirstOrDefault();
                //si encuentra una coincidencia
                if (dA != null)
                {
                    log.escribeLog("NUM_DOC existe: " + dA.NUM_DOC);
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
                        if (dA.ESTATUS_WF == "P") dA.ESTATUS_WF = "A";
                        db.Entry(dA).State = EntityState.Modified;
                        x = x + db.SaveChanges();
                        //Agregamos en la tabla los valores
                        DOCUMENTOSAP ds = new DOCUMENTOSAP();
                        ds.NUM_DOC = decimal.Parse(lstd[i].numero_TAT);
                        ds.BUKRS = lstd[i].Sociedad;
                        ds.EJERCICIO = lstd[i].Año;
                        ds.CUENTA_A = lstd[i].Cuenta_abono.ToString();
                        ds.CUENTA_C = lstd[i].Cuenta_cargo.ToString();
                        ds.BLART = lstd[i].blart;
                        ds.KUNNR = lstd[i].kunnr;
                        ds.DESCR = lstd[i].desc;
                        ds.IMPORTE = lstd[i].importe;
                        try
                        {
                            db.DOCUMENTOSAPs.Add(ds);
                            db.SaveChanges();
                            log.escribeLog("Actualiza doc -- " + dA.NUM_DOC);
                            if (lstd[i].last)
                                moverArchivo(lstd[i].file); log.escribeLog("Mueve archivo -- " + lstd[i].file);
                            //moverArchivo(archivos[i]);
                        }
                        catch
                        {
                            if (dA.ESTATUS_WF == "P") dA.ESTATUS_WF = "A";
                            DOCUMENTOSAP ds1 = db.DOCUMENTOSAPs.Find(ds.NUM_DOC);
                            ds1.BUKRS = lstd[i].Sociedad;
                            ds1.EJERCICIO = lstd[i].Año;
                            ds1.CUENTA_A = lstd[i].Cuenta_abono.ToString();
                            ds1.CUENTA_C = lstd[i].Cuenta_cargo.ToString();
                            ds.BLART = lstd[i].blart;
                            ds.KUNNR = lstd[i].kunnr;
                            ds.DESCR = lstd[i].desc;
                            ds.IMPORTE = lstd[i].importe;
                            if(lstd[i].fechac.ToString().Length == 8)
                            {
                                ds.FECHAC = new DateTime(int.Parse(lstd[i].fechac.ToString().Substring(0, 4)), int.Parse(lstd[i].fechac.ToString().Substring(4, 2)), int.Parse(lstd[i].fechac.ToString().Substring(6, 2)));
                            }
                            db.Entry(ds1).State = EntityState.Modified;

                            db.SaveChanges();
                            log.escribeLog("Actualiza doc -- " + dA.NUM_DOC);
                            if (lstd[i].last)
                                moverArchivo(lstd[i].file); log.escribeLog("Mueve archivo -- " + lstd[i].file);
                            //moverArchivo(archivos[i]);
                        }


                    }
                    catch (Exception varEx)
                    {
                        log.escribeLog("Error LOG -- " + varEx.ToString());
                        var ex = varEx.ToString();
                    }

                    if (dA.DOCUMENTO_REF != null)
                    {
                        if (dA.DOCUMENTO_REF > 0)
                        {
                            log.escribeLog("Es relacionado -- NUM_DOC: " + dA.NUM_DOC+" - NUM_PADRE: "+ dA.DOCUMENTO_REF);
                            List<DOCUMENTO> rela = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == dA.DOCUMENTO_REF).ToList();
                            DOCUMENTO parcial = rela.Where(a => a.TSOL_ID == "RP").FirstOrDefault();
                            if (parcial != null)
                            {
                                log.escribeLog("Es parcial -- NUM_DOC: " + dA.NUM_DOC + " - NUM_PADRE: " + dA.DOCUMENTO_REF);
                                bool contabilizados = true;
                                foreach (DOCUMENTO rel in rela)
                                {
                                    if (rel.TSOL_ID == "RP")
                                        if (rel.ESTATUS_SAP == "X")
                                            contabilizados = false;
                                }

                                if (contabilizados)
                                {
                                    log.escribeLog("Estan contabilizados -- NUM_DOC: " + dA.NUM_DOC + " - NUM_PADRE: " + dA.DOCUMENTO_REF);
                                    FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC == parcial.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();
                                    if (f != null)
                                    {
                                        f.ESTATUS = "A";
                                        f.FECHAM = DateTime.Now;
                                        ProcesaFlujo p = new ProcesaFlujo();
                                        string res = p.procesa(f, "");
                                        log.escribeLog("Procesa Flujo 1 -- NUM_DOC: " + parcial.NUM_DOC + " - RES: " + res);
                                        
                                        if (res == "0" | res == "")
                                        {
                                            FLUJO f1 = db.FLUJOes.Where(a => a.NUM_DOC == parcial.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();

                                            f.ESTATUS = "A";
                                            f.FECHAM = DateTime.Now;
                                            res = p.procesa(f, "");
                                            log.escribeLog("Procesa Flujo 2 -- NUM_DOC: " + parcial.NUM_DOC + " - RES: " + res);
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
                log.escribeLog("Error LOG -- " + varEx.ToString());
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
                    log.escribeLog("Error LOG -- " + ex.Message.ToString());
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
                log.escribeLog("Error LOG -- " + ex.Message.ToString());
                throw new Exception(ex.Message);
            }
        }
    }
}
