using OfficeOpenXml;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using TFW;
using TFW.Domain;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using TPA.Infra.Services;
using TPA.ViewModel;

namespace TPA.Application
{
    /// <summary>
    /// classe de controle / regras de negócio / application para lidar com importação de planilhas de timesheet
    /// Classe responsável por importar uma planilha no formato disponível para download e transformar cada linha dela em uma atividade
    /// </summary>
    public class ImportaPlanilhaApplication
    {

        #region fields privados

        private TPAContext _db;
        private AtividadeApplication _ativApp;

        #endregion



        #region construtores

        /// <summary>
        /// construtor padrão, recebe o context e passa para o objeto application
        /// </summary>
        /// <param name="ctx">TPAContext - context do entity framework</param>
        public ImportaPlanilhaApplication(TPAContext ctx)
        {
            _db = ctx;
            _ativApp = new AtividadeApplication(_db);
        }

        #endregion


        #region métodos públicos

        /// <summary>
        /// importa uma planilha do excel com atividades para um usuário
        /// </summary>
        /// <param name="idUsuario">int - id do usuário</param>
        /// <param name="pkg">ExcelPackage - package da Epplus criado com os dados da planilha como stream</param>
        /// <returns>ImportacaoPlanilhaAtividadesUsuarioViewModel - viewmodel com as atividades para serem editadas e postadas</returns>
        public virtual ImportacaoPlanilhaAtividadesUsuarioViewModel TransformarPlanilhaUsuario(int idUsuario, ExcelPackage pkg)
        {
            TFWHierarchicalList projetos = new TFWHierarchicalList();
            foreach(var pn in _db.ProjectNodes.AsNoTracking().ToList())
            {
                projetos.Add(pn.Id, pn.Pai==null? new Nullable<int>() : pn.Pai.Id, pn.Nome);
            }

            ImportacaoPlanilhaAtividadesUsuarioViewModel result = new ImportacaoPlanilhaAtividadesUsuarioViewModel();
            CalendarioServices cal = new CalendarioServices();
            result.IdUsuario = idUsuario;

            const int LINHA_INICIAL = 15;
            const int LINHA_FINAL = 79;
            const int COLUNA_PROJETO = 1;
            const int COLUNA_DATA = 2;
            const int COLUNA_ENTM = 3;
            const int COLUNA_SAIM = 4;
            const int COLUNA_ENTT = 5;
            const int COLUNA_SAIT = 6;
            const int COLUNA_HORAS = 9;
            const int COLUNA_TIPOATIVIDADE = 10;
            const int COLUNA_DESCRICAO = 11;


            var currentSheet = pkg.Workbook.Worksheets;
            var workSheet = currentSheet.First();
            var noOfCol = workSheet.Dimension.End.Column;
            var noOfRow = workSheet.Dimension.End.Row;

            for (int rowIterator = LINHA_INICIAL; rowIterator <= noOfRow || rowIterator <= LINHA_FINAL; rowIterator++)
            {
                ItemImportacaoPlanilha item = new ItemImportacaoPlanilha();
                item.IdUsuario = idUsuario;
                TimeSpan qtdHoras = ExcelToTimeSpan(workSheet.Cells[rowIterator, COLUNA_HORAS].Value);

                if ((workSheet.Cells[rowIterator, COLUNA_PROJETO].Value != null) 
                    && (qtdHoras > TimeSpan.MinValue))
                {
                    DateTime? data = ExcelToNullableDateTime(workSheet.Cells[rowIterator, COLUNA_DATA].Value);

                    //a data tem que ser não nula e dia util
                    if ((data != null) && (cal.IsDiaUtil(data.Value)))
                    {
                        item.Data = data.Value;
                        item.Descricao = workSheet.Cells[rowIterator, COLUNA_DESCRICAO].Value != null ? workSheet.Cells[rowIterator, COLUNA_DESCRICAO].Value.ToString() : "";

                        item.EntradaManha = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_ENTM].Value);
                        item.SaidaManha = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_SAIM].Value);
                        item.EntradaTarde = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_ENTT].Value);
                        item.SaidaTarde = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_SAIT].Value);

                        if (workSheet.Cells[rowIterator, COLUNA_PROJETO].Value != null)
                        {
                            string nome = workSheet.Cells[rowIterator, COLUNA_PROJETO].Value.ToString();

                            //procura por path completo
                            var proj = projetos.Where(n => n.ValorPath == nome).FirstOrDefault();
                            if (proj != null)
                            {

                                item.IdProjeto = proj.Id;
                            }
                            else
                            {
                                //procura só por nome
                                proj = projetos.Where(n => n.Valor == nome).FirstOrDefault();
                                if (proj != null)
                                {

                                    item.IdProjeto = proj.Id;
                                }
                            }
                        }

                        if (workSheet.Cells[rowIterator, COLUNA_TIPOATIVIDADE].Value != null)
                        {
                            string nome = workSheet.Cells[rowIterator, COLUNA_TIPOATIVIDADE].Value.ToString();
                            var atv = _db.TiposAtividade.Where(n => n.Nome == nome).FirstOrDefault();
                            if (atv != null)
                            {
                                item.IdTipoAtividade = atv.Id;
                            }
                        }

                        result.Itens.Add(item);
                    }

                }


            }


            return result;
        }

        /// <summary>
        /// importa uma planilha de atividades para múltiplos usuários
        /// </summary>
        /// <param name="pkg">ExcelPackage - package epplus com a planilha</param>
        /// <returns>ImportacaoPlanilhaAtividadesUsuarioViewModel - viewmodel com as atividades para serem editadas e postadas</returns>
        public virtual ImportacaoPlanilhaAtividadesUsuarioViewModel TransformarPlanilhaAdmin(ExcelPackage pkg)
        {
            TFWHierarchicalList projetos = new TFWHierarchicalList();
            foreach (var pn in _db.ProjectNodes.AsNoTracking().ToList())
            {
                projetos.Add(pn.Id, pn.Pai == null ? new Nullable<int>() : pn.Pai.Id, pn.Nome);
            }

            ImportacaoPlanilhaAtividadesUsuarioViewModel result = new ImportacaoPlanilhaAtividadesUsuarioViewModel();
            CalendarioServices cal = new CalendarioServices();

            const int LINHA_INICIAL = 15;

            const int COLUNA_USUARIO = 1;
            const int COLUNA_PROJETO = 2;
            const int COLUNA_DATA = 3;
            const int COLUNA_ENTM = 4;
            const int COLUNA_SAIM = 5;
            const int COLUNA_ENTT = 6;
            const int COLUNA_SAIT = 7;
            const int COLUNA_HORAS = 10;
            const int COLUNA_TIPOATIVIDADE = 11;
            const int COLUNA_DESCRICAO = 12;


            var currentSheet = pkg.Workbook.Worksheets;
            var workSheet = currentSheet.First();
            var noOfCol = workSheet.Dimension.End.Column;
            var noOfRow = workSheet.Dimension.End.Row;

            for (int rowIterator = LINHA_INICIAL; rowIterator <= noOfRow; rowIterator++)
            {


                TimeSpan qtdHoras = ExcelToTimeSpan(workSheet.Cells[rowIterator, COLUNA_HORAS].Value);

                if ((workSheet.Cells[rowIterator, COLUNA_USUARIO].Value != null) 
                    && (workSheet.Cells[rowIterator, COLUNA_PROJETO].Value != null) 
                    && (qtdHoras > TimeSpan.MinValue))
                {

                    string loginUsuario = workSheet.Cells[rowIterator, COLUNA_USUARIO].Value.ToString();
                    Usuario usu = _db.Usuarios.Where(x => x.Login == loginUsuario).FirstOrDefault();
                    ItemImportacaoPlanilha item = new ItemImportacaoPlanilha();
                    if (usu != null)
                        item.IdUsuario = usu.Id;

                    DateTime? data = ExcelToNullableDateTime(workSheet.Cells[rowIterator, COLUNA_DATA].Value);

                    //a data tem que ser não nula e dia util
                    if ((data != null) && (cal.IsDiaUtil(data.Value)))
                    {
                        item.Data = data.Value;
                        item.Descricao = workSheet.Cells[rowIterator, COLUNA_DESCRICAO].Value != null ? workSheet.Cells[rowIterator, COLUNA_DESCRICAO].Value.ToString() : "";

                        item.EntradaManha = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_ENTM].Value);
                        item.SaidaManha = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_SAIM].Value);
                        item.EntradaTarde = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_ENTT].Value);
                        item.SaidaTarde = ExcelToNullableTimeSpan(workSheet.Cells[rowIterator, COLUNA_SAIT].Value);

                        if (workSheet.Cells[rowIterator, COLUNA_PROJETO].Value != null)
                        {
                            string nome = workSheet.Cells[rowIterator, COLUNA_PROJETO].Value.ToString();

                            //procura por path completo
                            var proj = projetos.Where(n => n.ValorPath == nome).FirstOrDefault();
                            if (proj != null)
                            {

                                item.IdProjeto = proj.Id;
                            }
                            else
                            {
                                //procura só por nome
                                proj = projetos.Where(n => n.Valor == nome).FirstOrDefault();
                                if (proj != null)
                                {

                                    item.IdProjeto = proj.Id;
                                }
                            }
                        }

                        if (workSheet.Cells[rowIterator, COLUNA_TIPOATIVIDADE].Value != null)
                        {
                            string nome = workSheet.Cells[rowIterator, COLUNA_TIPOATIVIDADE].Value.ToString();
                            var atv = _db.TiposAtividade.Where(n => n.Nome == nome).FirstOrDefault();
                            if (atv != null)
                            {
                                item.IdTipoAtividade = atv.Id;
                            }
                        }

                        result.Itens.Add(item);
                    }

                }


            }


            return result;
        }


        /// <summary>
        /// posta os dados retirados da planilha e trabalhados no ImportacaoPlanilhaAtividadesViewModel
        /// </summary>
        /// <param name="planilha">ImportacaoPlanilhaAtividadesViewModel - viewmodel com os dados estruturados e editados para inserção no banco de dados</param>
        /// <returns>int - quantidade de atividades importadas</returns>
        public virtual async Task< int> LancarAsync(ImportacaoPlanilhaAtividadesViewModel planilha)
        {
            int result = 0;

            CalendarioServices cal = new CalendarioServices();

            AtividadeApplication app = new AtividadeApplication(this._db);

            foreach (var i in planilha.Itens)
            {
                try
                {
                    if ((i.Data != null) && (i.Data != DateTime.MinValue) && (cal.IsDiaUtil(i.Data)))
                    {
                        var projeto = await this._db.ProjectNodes.FindAsync(i.IdProjeto);
                        var tipo = await this._db.TiposAtividade.FindAsync(i.IdTipoAtividade);
                        

                        if(planilha is ImportacaoPlanilhaAtividadesUsuarioViewModel)
                        {
                            i.IdUsuario = (planilha as ImportacaoPlanilhaAtividadesUsuarioViewModel).IdUsuario;                            
                        }

                        Usuario usuario = await this._db.Usuarios.FindAsync(i.IdUsuario);

                        if (usuario == null)
                            throw new Exception("Usuário da planilha não preenchido");

                        if(i.EntradaManha != null && i.SaidaManha != null && i.EntradaTarde != null && i.SaidaTarde != null)
                        {

                            if(i.EntradaManha > i.SaidaManha)
                            {
                                throw new Exception("A entrada do primeiro horário não pode ser maior que a saída do primeiro horário");
                            }

                            if (i.EntradaTarde > i.SaidaTarde)
                            {
                                throw new Exception("A entrada do segundo horário não pode ser maior que a saída do segundo horário");
                            }

                            if (i.EntradaManha > i.SaidaTarde)
                            {
                                throw new Exception("A entrada do primeiro horário não pode ser maior que a saída do segundo horário");
                            }

                            if (i.SaidaManha > i.EntradaTarde)
                            {
                                throw new Exception("A saída do primeiro horário não pode ser maior que a entrada do segundo horário");
                            }

                        }

                        if ((i.EntradaManha != null) && (i.SaidaManha != null) && (i.SaidaManha > i.EntradaManha))
                        {

                            await app.SalvarAsync(new Atividade
                            {
                                Observacao = i.Descricao,
                                Inicio = i.Data.AddHours(i.EntradaManha.Value.Hours).AddMinutes(i.EntradaManha.Value.Minutes),
                                Fim = i.Data.AddHours(i.SaidaManha.Value.Hours).AddMinutes(i.SaidaManha.Value.Minutes),
                                ProjectNode = projeto,
                                TipoAtividade = tipo,
                                Usuario = usuario
                            }, false);

                        }

                        if ((i.EntradaTarde != null) && (i.SaidaTarde != null) && (i.SaidaTarde > i.EntradaTarde))
                        {

                            await app.SalvarAsync(new Atividade
                            {
                                Observacao = i.Descricao,
                                Inicio = i.Data.AddHours(i.EntradaTarde.Value.Hours).AddMinutes(i.EntradaTarde.Value.Minutes),
                                Fim = i.Data.AddHours(i.SaidaTarde.Value.Hours).AddMinutes(i.SaidaTarde.Value.Minutes),
                                ProjectNode = projeto,
                                TipoAtividade = tipo,
                                Usuario = usuario
                            }, false);

                        }

                    }
                }
                catch (DbEntityValidationException ex)
                {
                    string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                    throw new Exception("Problemas com a validação das entities: " + exceptionMessage, ex);
                    
                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                    throw new Exception("Erro: " + err.Message, err);
                }
                finally
                {
                    result++;
                }

            }

            return result;

        }

        #endregion



        #region metodos privados de conversao excel

        /// <summary>
        /// converte um objeto de uma célula para timespan
        /// </summary>
        /// <param name="objtimeSpan"></param>
        /// <returns></returns>
        private TimeSpan ExcelToTimeSpan(object objtimeSpan)
        {
            TimeSpan result = new TimeSpan(0);
            if ((objtimeSpan != null) && (objtimeSpan is DateTime))
            {
                result = ((DateTime)objtimeSpan).TimeOfDay;
            }
            else if ((objtimeSpan != null) && (objtimeSpan is Double))
            {
                result = DateTime.FromOADate((double)objtimeSpan).TimeOfDay;
            }
            else if ((objtimeSpan != null) && (objtimeSpan is string))
            {
                TimeSpan tmp;
                if (TimeSpan.TryParse(objtimeSpan.ToString(), out tmp))
                {
                    result = tmp;
                }
            }

            return result;
        }

        /// <summary>
        /// converte um objeto de uma célula para um timespan nulável
        /// </summary>
        /// <param name="objtimeSpan"></param>
        /// <returns></returns>
        private TimeSpan? ExcelToNullableTimeSpan(object objtimeSpan)
        {
            TimeSpan? result = null;
            if ((objtimeSpan != null) && (objtimeSpan is DateTime))
            {
                result = ((DateTime)objtimeSpan).TimeOfDay;
            }
            else if ((objtimeSpan != null) && (objtimeSpan is Double))
            {
                result = DateTime.FromOADate((double)objtimeSpan).TimeOfDay;
            }
            else if ((objtimeSpan != null) && (objtimeSpan is string) && (!string.IsNullOrWhiteSpace(objtimeSpan as string)))
            {
                TimeSpan tmp;
                if (TimeSpan.TryParse(objtimeSpan.ToString(), out tmp))
                {
                    result = tmp;
                }
            }

            return result;
        }







        /// <summary>
        /// converte um objeto em uma célula para um DateTime
        /// </summary>
        /// <param name="objtimeSpan"></param>
        /// <returns></returns>
        private DateTime ExcelToDateTime(object objtimeSpan)
        {
            DateTime result = DateTime.MinValue;
            if ((objtimeSpan != null) && (objtimeSpan is DateTime))
            {
                result = ((DateTime)objtimeSpan);
            }
            else if ((objtimeSpan != null) && (objtimeSpan is Double))
            {
                result = DateTime.FromOADate((double)objtimeSpan);
            }
            else if ((objtimeSpan != null) && (objtimeSpan is string))
            {
                try
                {
                    result = TFWConvert.ToDateTime(objtimeSpan.ToString());
                }
                catch
                {

                }
            }

            return result;
        }


        /// <summary>
        /// converte um objeto em uma célula excel para um DateTime nulável
        /// </summary>
        /// <param name="objtimeSpan"></param>
        /// <returns></returns>
        private DateTime? ExcelToNullableDateTime(object objtimeSpan)
        {
            DateTime? result = null;
            if ((objtimeSpan != null) && (objtimeSpan is DateTime))
            {
                result = ((DateTime)objtimeSpan);
            }
            else if ((objtimeSpan != null) && (objtimeSpan is Double))
            {
                result = DateTime.FromOADate((double)objtimeSpan);
            }
            else if ((objtimeSpan != null) && (objtimeSpan is string))
            {
                try
                {
                    result = TFWConvert.ToNullableDateTime(objtimeSpan.ToString());
                }
                catch
                {

                }
            }

            return result;
        }

        #endregion
    }
}