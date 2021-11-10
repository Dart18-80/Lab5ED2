using System;
using System.Collections.Generic;
using System.Text;

namespace Library_SDES
{
    public interface InterfazRSA
    {
        public void CifrarRSA(string ArchivoNuevo, string ArchivoCodificado, string key);
        public void GenerarLlaves(int p, int q, string CarpetaPath);
    }
}
