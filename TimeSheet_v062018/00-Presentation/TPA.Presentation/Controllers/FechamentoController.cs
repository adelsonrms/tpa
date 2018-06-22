using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using TPA.Domain.DomainModel;
using System.Globalization;
using TPA.ViewModel;
using TPA.ViewModel.Buscas;
using TPA.Infra.Services;
using TPA.Services.Seguranca;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller responsável por fechar o período e passar para o banco de horas as horas remanescentes
    /// </summary>
    [TPADescricaoAcaoController("Fechamento do Mês", "Controlar o fechamento do período, redefinir horas estimadas e recalcular horas trabalhadas")]
    public class FechamentoController : TPAController
    {

        #region métodos públicos de busca e fechamento


        /// <summary>
        /// get index do fechamento, listar e pesquisar
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar e Fazer buscas no fechamento", "Permitir acessar a index do fechamento, listar e pesquisar")]
        public ActionResult Index()
        {
            var fBuscaVM = (Session["fBuscaVM"] as FechamentoBuscaViewModel) ?? new FechamentoBuscaViewModel();
            if (fBuscaVM != null)
            {
                FechamentoIndexViewModel resultsAnteriores = new FechamentoIndexViewModel();
                resultsAnteriores.Busca = fBuscaVM;
                return  Index(resultsAnteriores);
            }

            CarregaMesesAnos();
            FechamentoIndexViewModel result = new FechamentoIndexViewModel();

            var usuarios = db.Usuarios
                .Include(u => u.Funcionario)
                .ToList();

            result.Usuarios.AddRange(usuarios);

            return View(result);
        }


        /// <summary>
        /// post do index do fechamento
        /// envia os parâmetros de busca
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Listar e Fazer buscas no fechamento", "Permitir acessar a index do fechamento, listar e pesquisar")]
        public ActionResult Index(FechamentoIndexViewModel model)
        {
            FechamentoBuscaViewModel fBuscaVM = model.Busca ?? (Session["fBuscaVM"] as FechamentoBuscaViewModel) ??  new FechamentoBuscaViewModel();

            CarregaMesesAnos();

            if (model == null || model.Busca == null)
            {
                MensagemParaUsuarioViewModel.MensagemAlerta("Preencher os dados do formulário corretamente. ", TempData);
                model = new FechamentoIndexViewModel();
                model.Usuarios.AddRange(db.Usuarios.ToList());

                Session["fBuscaVM"] = new FechamentoBuscaViewModel();

                return View(model);
            }

            try
            {
                model.Usuarios.Clear();
                model.Usuarios.AddRange(db.Usuarios.ToList());

                DateTime dataInicial = new DateTime(model.Busca.AnoInicial??DateTime.Today.Year, model.Busca.MesInicial ?? DateTime.Today.Month, 1);
                DateTime dataFinal = (new DateTime(model.Busca.AnoFinal ?? DateTime.Today.Year, model.Busca.MesFinal ?? DateTime.Today.Month, 1)).AddMonths(1).AddSeconds(-1);

                var referencias = db.Referencias.Where(x => DbFunctions.CreateDateTime(x.Ano, x.Mes, 1, 0, 0, 0) >= dataInicial && DbFunctions.CreateDateTime(x.Ano, x.Mes, 1, 0, 0, 0) <= dataFinal);

                if (model.Busca.IdsUsuarios != null && model.Busca.IdsUsuarios.Length > 0)
                    referencias = referencias.Where(x => model.Busca.IdsUsuarios.Contains(x.Usuario.Id));

                model.Referencias.Clear();
                model.Referencias.AddRange(referencias
                    .OrderBy(r=>r.Ano)
                    .ThenBy(r=>r.Mes)
                    .ToList());


                if(model.Acao == "Fechar")
                {
                    var lista = model
                        .Referencias
                        .Where(x => x.Fechado == false)
                        .OrderBy(r => r.Ano)
                        .ThenBy(r => r.Mes)
                        .ToList();

                    foreach (var r in lista)
                    {

                        r.SincronizaAtividades(new CalendarioServices());
                        r.Fechado = true;
                        db.SaveChanges();

                    }

                    if (lista.Count > 1)
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso(string.Format("{0} Referencias foram fechadas. ", lista.Count), TempData);
                    }
                    else if (lista.Count == 1)
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso("1 Referencia foi fechada. ", TempData);
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemAlerta("Não havia nada a ser fechado. ", TempData);
                    }
                }
                else if (model.Acao == "Recalcular")
                {
                    var lista = model
                        .Referencias
                        .OrderBy(r => r.Ano)
                        .ThenBy(r => r.Mes)
                        .ToList();

                    foreach (var r in lista)
                    {

                        //capturo o estado atual
                        //bool fechadoAtual = r.Fechado;
                        //forço reabertura e sincronizo
                        //r.Fechado = false;
                        r.SincronizaAtividades(new CalendarioServices());
                        //volto o estado anterior e salvo
                        //r.Fechado = fechadoAtual;
                        db.SaveChanges();

                    }

                    if (lista.Count > 1)
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso(string.Format("{0} Referencias foram recalculadas. ", lista.Count), TempData);
                    }
                    else if (lista.Count == 1)
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso("1 Referencia foi recalculada. ", TempData);
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemAlerta("Não havia nada a ser recalculado. ", TempData);
                    }
                }

                Session["fBuscaVM"] = model.Busca ?? new FechamentoBuscaViewModel();

                return View(model);

            }
            catch (DbEntityValidationException ex)
            {
                string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
            }

            Session["fBuscaVM"] = model.Busca ?? new FechamentoBuscaViewModel();

            return RedirectToAction("Index");
        }

        #endregion




        #region métodos públicos de edição

        /// <summary>
        /// get da página de edição do fechamento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Alterar dados do fechamento", "Permitir editar ou reabrir o período")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            Referencia referencia = await db.Referencias.FindAsync(id);


            if (referencia == null)
            {
                return HttpNotFound();
            }

            FechamentoViewModel fechamento = new FechamentoViewModel
            {
                Id = referencia.Id,
                IdUsuario = referencia.Usuario.Id,
                Ano = referencia.Ano,
                Mes = referencia.Mes,
                Fechado = referencia.Fechado,
                HorasPrevistasDouble = referencia.Previsto.TotalHours
            };



            return View(fechamento);
        }


        /// <summary>
        /// post da edição de fechamento
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Alterar dados do fechamento", "Permitir editar ou reabrir o período")]
        public async Task<ActionResult> Edit([Bind(Include = "Id, IdUsuario, HorasPrevistasString, Fechado")] FechamentoViewModel referencia)
        {
  
            if (referencia.HorasPrevistasDouble < 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher as horas previstas", TempData, ModelState);
            }

            if (ModelState.IsValid)
            {

                try
                {
                    var refe = db.Referencias.Find(referencia.Id);
                    if (refe != null)
                    {

                        refe.Previsto = referencia.HorasPrevistasTimeSpan;
                        refe.Fechado = referencia.Fechado;
                        refe.SincronizaAtividades(new CalendarioServices());

                        await db.SaveChangesAsync();
                        MensagemParaUsuarioViewModel.MensagemSucesso("Alterações salvas.", TempData);

                        return View(referencia);
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Não foi encontrada a referência deste mês para este funcionário", TempData, ModelState);
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                    MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                }

            }



            return View(referencia);
        }

        #endregion




        #region métodos públicos diversos

        /// <summary>
        /// get detalhes do fechamento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Visualizar Detalhes do fechamento", "Permitir visualizar detalhes do fechamento")]
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Id deve ser informado");
            }

            Referencia referencia = await db.Referencias.FindAsync(id);
            if(referencia == null)
            {
                return HttpNotFound();
            }

            DateTime dataInicial = new DateTime(referencia.Ano, referencia.Mes, 1);
            DateTime dataFinal = dataInicial.AddMonths(1).AddSeconds(-1);

            var busca = new 
            {
                DataInicio = TFW.TFWConvert.ToString(dataInicial),
                DataFim = TFW.TFWConvert.ToString(dataFinal),
                IdUsuario = referencia.Usuario.Id.ToString()
            };


            return RedirectToAction("Busca", "Atividades", busca);

        }


        #endregion







        #region private methods

        private void CarregaHoras(int? valor = null)
        {

            var hrs = new Dictionary<int, string>();
            for(int i=0; i<=200; i++)
            {
                hrs.Add(i, i.ToString());
            }
            SelectList horasselect = new SelectList(hrs, "Key", "Value", valor??160);
            ViewBag.Horas = horasselect;
        }




        private void CarregaMesesAnos()
        {

            var anos = new List<Tuple<int?, string>>();
            anos.Add(new Tuple<int?, string>(null, "Todos"));
            for (int i = 2015; i <= 2050; i++)
            {
                anos.Add(new Tuple<int?, string>(i, i.ToString()));
            }
            SelectList anosSelect = new SelectList(anos, "Item1", "Item2");
            ViewBag.Anos = anosSelect;
            

            var ptbr = new CultureInfo("pt-BR");
            var ptbrMeses = ptbr
                .DateTimeFormat
                .MonthNames
                .Select((monthName, index) => new
                {
                    Numero = index + 1,
                    Mes = ptbr.TextInfo.ToTitleCase(monthName)
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Mes));

            var meses = new List<Tuple<int?, string>>();
            meses.Add(new Tuple<int?, string>(null, "Todos"));
            foreach (var m in ptbrMeses)
            {
                meses.Add(new Tuple<int?, string>(m.Numero, m.Mes));                
            }
            SelectList mesesSelect = new SelectList(meses, "Item1", "Item2");
            ViewBag.Meses = mesesSelect;
        }

        #endregion


    }
}