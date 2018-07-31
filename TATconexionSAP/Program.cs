using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TATconexionSAP.Services;

namespace TATconexionSAP
{
    public class Program
    {


        static void Main(string[] args)
        {
            Modelos m = new Modelos();
            List<string> err = m.leerArchivos();
            if (err.Count > 1)
            {
                MailErrores me = new MailErrores();
                me.enviarErrores(err);
            }
            
        }
    }
}
