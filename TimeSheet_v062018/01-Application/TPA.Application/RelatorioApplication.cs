using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using TPA.Infra.Data;
using TPA.Infra.Data.Repository;
using TPA.ViewModel.Relatorios;
using TPA.Framework;
using TPA.Infra.Services;
using System.Linq;

namespace TPA.Application
{
    /// <summary>
    /// classe de application geral para todos os relatórios,
    /// os dinâmicaos cadastrados que existirão futuramente 
    /// e os estáticos que já existem baseados em procedures
    /// </summary>
    public class RelatorioApplication
    {
        #region propriedades privadas
        /// <summary>
        /// instância de TPAContext a ser passada pelo controller, por causa do padrão context per request
        /// </summary>
        private TPAContext _db;
        #endregion


        #region constructor  

        /// <summary>
        /// constructor padrão
        /// </summary>
        /// <param name="context">TPAContext passado pelo controller</param>
        public RelatorioApplication(TPAContext context)
        {
            this._db = context;
        }

        #endregion


        #region métodos públicos



        /// <summary>
        /// relatório analítico em Excel
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List de Relatorio_Analitico - dados completos das atividades entre as datas inicial e final</returns>
        public virtual List<Relatorio_Analitico> GetRelatorio_Analitico(DateTime dtIni, DateTime dtFin)
        {

            List<Relatorio_Analitico> result = new List<Relatorio_Analitico>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Analitico(dtIni, dtFin));

            return result;
        }



        /// <summary>
        /// relatório de atividades consolidado por cliente e funcionário
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Consolidado_Cliente_Funcionario> GetRelatorio_Consolidado_Cliente_Funcionario(DateTime dtIni, DateTime dtFin)
        {
            List<Relatorio_Consolidado_Cliente_Funcionario> result = new List<Relatorio_Consolidado_Cliente_Funcionario>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Consolidado_Cliente_Funcionario(dtIni, dtFin));

            return result;
        }


        /// <summary>
        /// relatório diário consolidado por cliente e funcionário
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Consolidado_Cliente_Funcionario_Dia> GetRelatorio_Consolidado_Cliente_Funcionario_Dia(DateTime dtIni, DateTime dtFin)
        {
            List<Relatorio_Consolidado_Cliente_Funcionario_Dia> result = new List<Relatorio_Consolidado_Cliente_Funcionario_Dia>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Consolidado_Cliente_Funcionario_Dia(dtIni, dtFin));

            return result;
        }


        /// <summary>
        /// relatório consolidado por projeto e funcionário
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Consolidado_Projeto_Funcionario> GetRelatorio_Consolidado_Projeto_Funcionario(DateTime dtIni, DateTime dtFin)
        {

            List<Relatorio_Consolidado_Projeto_Funcionario> result = new List<Relatorio_Consolidado_Projeto_Funcionario>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Consolidado_Projeto_Funcionario(dtIni, dtFin));

            return result;
        }


        /// <summary>
        /// relatório diário consolidado por projeto e funcionário
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Consolidado_Projeto_Funcionario_Dia> GetRelatorio_Consolidado_Projeto_Funcionario_Dia(DateTime dtIni, DateTime dtFin)
        {
            List<Relatorio_Consolidado_Projeto_Funcionario_Dia> result = new List<Relatorio_Consolidado_Projeto_Funcionario_Dia>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Consolidado_Projeto_Funcionario_Dia(dtIni, dtFin));

            return result;
        }


        /// <summary>
        /// relatório consolidado de horas por cliente
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Horas_Cliente> GetRelatorio_Horas_Cliente(DateTime dtIni, DateTime dtFin)
        {
            List<Relatorio_Horas_Cliente> result = new List<Relatorio_Horas_Cliente>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Horas_Cliente(dtIni, dtFin));

            return result;
        }



        /// <summary>
        /// relatório consolidado de horas por projeto
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Horas_Projeto> GetRelatorio_Horas_Projeto(DateTime dtIni, DateTime dtFin)
        {
            List<Relatorio_Horas_Projeto> result = new List<Relatorio_Horas_Projeto>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRelatorio_Horas_Projeto(dtIni, dtFin));

            return result;
        }


        /// <summary>
        /// relatório consolidado de horas por funcionário
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns></returns>
        public virtual List<Relatorio_Horas_Funcionario> GetRelatorio_Horas_Funcionario(DateTime dtIni, DateTime dtFin)
        {

            List<Relatorio_Horas_Funcionario> result = new List<Relatorio_Horas_Funcionario>();

            var rep = new RelatorioRepository(this._db);
            result.AddRange(rep.GetRRelatorio_Horas_Funcionario(dtIni, dtFin));

            return result;
        }















        /// <summary>
        /// usa os métodos públicos para trazer os dados e os privados abaixo para colocar os dados em uma planilha de excel
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="pathCompletoModelo">string - caminho do modelo em branco</param>
        /// <returns>string - caminho no servidor da planilha de excel construída conforme o modelo, com todos esses dados sentro</returns>
        public virtual string GeraRelatorioPadrao(DateTime dtIni, DateTime dtFin, string pathCompletoModelo)
        {
            string pathDirModelo = Path.GetDirectoryName(pathCompletoModelo);
            string pathTemp = Path.Combine(pathDirModelo, "temp");
            string fileTemp = Path.Combine(pathTemp, Guid.NewGuid().ToString() + ".xlsx");


            if (!Directory.Exists(pathTemp))
            {
                Directory.CreateDirectory(pathTemp);
            }

            if (File.Exists(pathCompletoModelo))
            {
                try
                {
                    File.Copy(pathCompletoModelo, fileTemp, true);
                }
                catch (Exception err)
                {
                    throw new Exception(string.Format("Erro ao copiar o arquivo {0} para {1}", pathCompletoModelo, fileTemp), err);
                }
            }
            else
            {
                throw new Exception("Não existe o arquivo " + pathCompletoModelo);
            }


            if (!File.Exists(fileTemp))
            {
                throw new Exception("Não foi possível criar o arquivo temporário " + fileTemp);
            }

            FileInfo info = new FileInfo(fileTemp);
            ExcelPackage pkg = new ExcelPackage(info);

            ConstroiRelatorio_Analitico(dtIni, dtFin, pkg.Workbook.Worksheets["Dados"]);
            ConstroiRelatorio_Consolidado_Cliente_Funcionario(dtIni, dtFin, pkg.Workbook.Worksheets["Funcionario_Cliente"]);
            ConstroiRelatorio_Consolidado_Cliente_Funcionario_Dia(dtIni, dtFin, pkg.Workbook.Worksheets["Funcionario_Cliente_Dia"]);
            ConstroiRelatorio_Consolidado_Projeto_Funcionario(dtIni, dtFin, pkg.Workbook.Worksheets["Funcionario_Projeto"]);
            ConstroiRelatorio_Consolidado_Projeto_Funcionario_Dia(dtIni, dtFin, pkg.Workbook.Worksheets["Funcionario_Projeto_Dia"]);

            ConstroiRelatorio_Horas_Cliente(dtIni, dtFin, pkg.Workbook.Worksheets["Horas_Cliente"]);
            ConstroiRelatorio_Horas_Funcionario(dtIni, dtFin, pkg.Workbook.Worksheets["Horas_Funcionario"]);
            ConstroiRelatorio_Horas_Projeto(dtIni, dtFin, pkg.Workbook.Worksheets["Horas_Projeto"]);

            pkg.Save();

            return fileTemp;
        }

        #endregion





        #region métodos privados

        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Analitico
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Analitico(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Analitico(dtIni, dtFin);

            int linha = 6;

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Inicio.Date;
                    ws.Cells[linha, 2].Value = d.Funcionario;
                    ws.Cells[linha, 3].Value = d.Tipo_Atividade;
                    ws.Cells[linha, 4].Value = d.Observacao;
                    ws.Cells[linha, 5].Value = d.Inicio;
                    ws.Cells[linha, 6].Value = d.Fim;
                    ws.Cells[linha, 7].Value = d.Horas;
                    ws.Cells[linha, 8].Value = d.Administrativo;
                    ws.Cells[linha, 9].Value = d.Cliente_Raiz;
                    ws.Cells[linha, 10].Value = d.Cliente;
                    ws.Cells[linha, 11].Value = d.Area;
                    ws.Cells[linha, 12].Value = d.Projeto;
                    ws.Cells[linha, 13].Value = d.Entregaveis;
                    ws.Cells[linha, 14].Value = d.Etapas;

                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }



        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Consolidado_Cliente_Funcionario
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Consolidado_Cliente_Funcionario(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Consolidado_Cliente_Funcionario(dtIni, dtFin);

            int linha = 6;

            ws.Column(3).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Funcionario;
                    ws.Cells[linha, 2].Value = d.Cliente;                    
                    ws.Cells[linha, 3].Value = d.HorasTimeSpan;


                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }


        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Consolidado_Cliente_Funcionario_Dia
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Consolidado_Cliente_Funcionario_Dia(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Consolidado_Cliente_Funcionario_Dia(dtIni, dtFin);

            int linha = 6;

            ws.Column(4).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Funcionario;
                    ws.Cells[linha, 2].Value = d.Cliente;
                    ws.Cells[linha, 3].Value = d.Dia;
                    ws.Cells[linha, 4].Value = d.HorasTimeSpan;

                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }



        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Consolidado_Projeto_Funcionario
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Consolidado_Projeto_Funcionario(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Consolidado_Projeto_Funcionario(dtIni, dtFin);

            int linha = 6;

            ws.Column(5).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Funcionario;
                    ws.Cells[linha, 2].Value = d.Cliente;
                    ws.Cells[linha, 3].Value = d.Area;
                    ws.Cells[linha, 4].Value = d.Projeto;
                    ws.Cells[linha, 5].Value = d.HorasTimeSpan;

                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }



        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Consolidado_Projeto_Funcionario_Dia
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Consolidado_Projeto_Funcionario_Dia(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Consolidado_Projeto_Funcionario_Dia(dtIni, dtFin);

            int linha = 6;

            ws.Column(1).Style.Numberformat.Format = "dd/MM/yyyy";
            ws.Column(6).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Dia;
                    ws.Cells[linha, 2].Value = d.Funcionario;
                    ws.Cells[linha, 3].Value = d.Cliente;
                    ws.Cells[linha, 4].Value = d.Area;
                    ws.Cells[linha, 5].Value = d.Projeto;
                    ws.Cells[linha, 6].Value = d.HorasTimeSpan;

                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }


        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Horas_Cliente
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Horas_Cliente(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Horas_Cliente(dtIni, dtFin);

            int linha = 6;


            ws.Column(2).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Cliente;
                    ws.Cells[linha, 2].Value = d.Horas;

                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }

        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Horas_Funcionario
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Horas_Funcionario(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Horas_Funcionario(dtIni, dtFin);

            int linha = 6;

            ws.Column(2).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Funcionario;
                    ws.Cells[linha, 2].Value = d.Horas;


                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }

        /// <summary>
        /// copia para a ExcelWorksheet os dados de GetRelatorio_Horas_Projeto
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <param name="ws">ExcelWorksheet - worksheet do pacote epplus para onde vão os dados</param>
        private void ConstroiRelatorio_Horas_Projeto(DateTime dtIni, DateTime dtFin, ExcelWorksheet ws)
        {
            var dados = this.GetRelatorio_Horas_Projeto(dtIni, dtFin);

            int linha = 6;

            ws.Column(2).Style.Numberformat.Format = "[hh]:mm";

            foreach (var d in dados)
            {
                try
                {
                    ws.Cells[linha, 1].Value = d.Projeto;
                    ws.Cells[linha, 2].Value = d.Horas;

                    linha++;
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                }
            }
        }

        #endregion


    }
}
