using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPA.ViewModel.Relatorios;

namespace TPA.Infra.Data.Repository
{
    /// <summary>
    /// essa classe serve tanto como repositório para relatórios cadastrados (futuramente) como centralizador de métodos para relatórios estáticos
    /// </summary>
    public class RelatorioRepository
    {



        #region propriedades privadas
        /// <summary>
        /// instância de TPAContext a ser passada pelo application, por causa do padrão context per request
        /// </summary>
        private TPAContext _db;
        #endregion


        #region constructor  

        /// <summary>
        /// constructor padrão
        /// </summary>
        /// <param name="context">TPAContext passado pelo application</param>
        public RelatorioRepository(TPAContext context)
        {
            this._db = context;
        }

        #endregion



        #region métodos públicos

        /// <summary>
        /// executa a procedure Relatorio_Analitico trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Analitico</returns>
        public virtual List<Relatorio_Analitico> GetRelatorio_Analitico(DateTime dtIni, DateTime dtFin)
        {

            string qry = "exec Relatorio_Analitico @dtIni, @dtFin";

            List<Relatorio_Analitico> result = new List<Relatorio_Analitico>();
            result.AddRange( _db.Database.SqlQuery<Relatorio_Analitico>(qry, 
                new SqlParameter("@dtIni", dtIni), 
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }




        /// <summary>
        /// executa a procedure Relatorio_Consolidado_Cliente_Funcionario trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Consolidado_Cliente_Funcionario</returns>
        public virtual List<Relatorio_Consolidado_Cliente_Funcionario> GetRelatorio_Consolidado_Cliente_Funcionario(DateTime dtIni, DateTime dtFin)
        {

            string qry = "exec Relatorio_Consolidado_Cliente_Funcionario @dtIni, @dtFin";

            List<Relatorio_Consolidado_Cliente_Funcionario> result = new List<Relatorio_Consolidado_Cliente_Funcionario>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Consolidado_Cliente_Funcionario>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }


        /// <summary>
        /// executa a procedure Relatorio_Consolidado_Cliente_Funcionario_Dia trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Consolidado_Cliente_Funcionario_Dia</returns>
        public virtual List<Relatorio_Consolidado_Cliente_Funcionario_Dia> GetRelatorio_Consolidado_Cliente_Funcionario_Dia(DateTime dtIni, DateTime dtFin)
        {

            string qry = "exec Relatorio_Consolidado_Cliente_Funcionario_Dia @dtIni, @dtFin";

            List<Relatorio_Consolidado_Cliente_Funcionario_Dia> result = new List<Relatorio_Consolidado_Cliente_Funcionario_Dia>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Consolidado_Cliente_Funcionario_Dia>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }

        /// <summary>
        /// executa a procedure Relatorio_Consolidado_Projeto_Funcionario trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Consolidado_Projeto_Funcionario</returns>
        public virtual List<Relatorio_Consolidado_Projeto_Funcionario> GetRelatorio_Consolidado_Projeto_Funcionario(DateTime dtIni, DateTime dtFin)
        {

            string qry = "exec Relatorio_Consolidado_Projeto_Funcionario @dtIni, @dtFin";

            List<Relatorio_Consolidado_Projeto_Funcionario> result = new List<Relatorio_Consolidado_Projeto_Funcionario>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Consolidado_Projeto_Funcionario>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }

        /// <summary>
        /// executa a procedure Relatorio_Consolidado_Projeto_Funcionario_Dia trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Consolidado_Projeto_Funcionario_Dia</returns>
        public virtual List<Relatorio_Consolidado_Projeto_Funcionario_Dia> GetRelatorio_Consolidado_Projeto_Funcionario_Dia(DateTime dtIni, DateTime dtFin)
        {

            string qry = "exec Relatorio_Consolidado_Projeto_Funcionario_Dia @dtIni, @dtFin";

            List<Relatorio_Consolidado_Projeto_Funcionario_Dia> result = new List<Relatorio_Consolidado_Projeto_Funcionario_Dia>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Consolidado_Projeto_Funcionario_Dia>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }

        /// <summary>
        /// executa a procedure Relatorio_Horas_Cliente trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Horas_Cliente</returns>
        public virtual List<Relatorio_Horas_Cliente> GetRelatorio_Horas_Cliente(DateTime dtIni, DateTime dtFin)
        {

            string qry = @"
                select 
	                cliente_principal.Nome Cliente,
	                cast(SUM(DATEDIFF(SECOND, a.inicio, a.fim) * 1.0) as double precision) Segundos
                from 
	                tpa.Atividade a 
	                left join tpa.ProjectNode cliente_principal on cliente_principal.id = a.Cliente_Id 
                where 
	                A.Inicio >= @dtIni AND a.Fim <= @dtFin
                group by 
	                cliente_principal.Nome
                order by
	                cliente_principal.Nome";

            List<Relatorio_Horas_Cliente> result = new List<Relatorio_Horas_Cliente>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Horas_Cliente>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }




        /// <summary>
        /// executa a procedure Relatorio_Horas_Funcionario trazendo os dados para criação do relatório
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Horas_Funcionario</returns>
        public virtual List<Relatorio_Horas_Funcionario> GetRRelatorio_Horas_Funcionario(DateTime dtIni, DateTime dtFin)
        {

            string qry = @"
                select 
	                coalesce(f.nome, u.login) Funcionario,
	                cast(SUM(DATEDIFF(SECOND, a.inicio, a.fim) * 1.0) as double precision) Segundos

                from 
	                tpa.Atividade a 
	                left join tpa.Funcionario f on f.Id = a.Usuario_Id
	                left join tpa.Usuario u on u.Id = a.Usuario_Id

                where 
	                A.Inicio >= @dtIni AND a.Fim <= @dtFin
                group by 
	                coalesce(f.nome, u.login)
                order by
	                coalesce(f.nome, u.login)";


            List<Relatorio_Horas_Funcionario> result = new List<Relatorio_Horas_Funcionario>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Horas_Funcionario>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }





        /// <summary>
        /// executa a procedure Relatorio_Horas_Projeto trazendo os dados para criação do relatório
        /// todo: a inserção do nodelabel segmento entre área e projeto quebrou os relatórios existentes. As 5 procedures devem ser alteradas conforme o modelo abaixo para contemplar essa alteração
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>List do modelo Relatorio_Horas_Projeto</returns>
        public virtual List<Relatorio_Horas_Projeto> GetRelatorio_Horas_Projeto(DateTime dtIni, DateTime dtFin)
        {
            string qry = @"
                select 			

                    coalesce(Projeto, Segmento, Area, Cliente, Administrativo) Projeto,
	                Cliente,
	                cast(sum(Segundos * 1.0) as double precision) Segundos

                from
                (
                    select

                        DATEDIFF(SECOND, a.inicio, a.fim) Segundos,
                        administrativo.Nome Administrativo,
                        (case 2 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Cliente,
		                (case 3 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id then n3.Nome when n4.NodeLabel_Id then n4.Nome when n5.NodeLabel_Id then n5.Nome when n6.NodeLabel_Id then n6.Nome when n7.NodeLabel_Id then n7.Nome else null end)   Area,
						(case 7 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id then n3.Nome when n4.NodeLabel_Id then n4.Nome when n5.NodeLabel_Id then n5.Nome when n6.NodeLabel_Id then n6.Nome when n7.NodeLabel_Id then n7.Nome else null end)   Segmento,
		                (case 4 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id then n3.Nome when n4.NodeLabel_Id then n4.Nome when n5.NodeLabel_Id then n5.Nome when n6.NodeLabel_Id then n6.Nome when n7.NodeLabel_Id then n7.Nome else null end)   Projeto,
		                (case 5 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id then n3.Nome when n4.NodeLabel_Id then n4.Nome when n5.NodeLabel_Id then n5.Nome when n6.NodeLabel_Id then n6.Nome when n7.NodeLabel_Id then n7.Nome else null end)   Entregaveis,
		                (case 6 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id then n3.Nome when n4.NodeLabel_Id then n4.Nome when n5.NodeLabel_Id then n5.Nome when n6.NodeLabel_Id then n6.Nome when n7.NodeLabel_Id then n7.Nome else null end)   Etapas

                from

                        tpa.Atividade a
                        left join tpa.Funcionario f on f.Id = a.Usuario_Id
                        left join tpa.Usuario u on u.Id = a.Usuario_Id
                        left join tpa.ProjectNode administrativo on administrativo.id = a.ProjectNode_Id and administrativo.NodeLabel_Id = 1
						left join tpa.ProjectNode n7 on n7.id = a.ProjectNode_Id
                        left join tpa.ProjectNode n6 on n6.id = n7.Pai_Id
                        left join tpa.ProjectNode n5 on n5.id = n6.Pai_Id
                        left join tpa.ProjectNode n4 on n4.id = n5.Pai_Id
                        left join tpa.ProjectNode n3 on n3.id = n4.Pai_Id
                        left join tpa.ProjectNode n2 on n2.id = n3.Pai_Id

                    where

                        A.Inicio >= @dtIni AND a.Fim <= @dtFin
                ) as T


                group by

                    coalesce(Projeto, Segmento, Area, Cliente, Administrativo),
	                Cliente
                order by
                    Projeto";

            List<Relatorio_Horas_Projeto> result = new List<Relatorio_Horas_Projeto>();
            result.AddRange(_db.Database.SqlQuery<Relatorio_Horas_Projeto>(qry,
                new SqlParameter("@dtIni", dtIni),
                new SqlParameter("@dtFin", dtFin)).ToList());
            return result;
        }


        #endregion


    }
}
