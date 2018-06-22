using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;

namespace TPA.Domain.DomainModel
{

    /// <summary>
    /// usuário do sistema, o lado de negócio, não o lado do microsoft identity
    /// </summary>
    public class Usuario  : TPAEntity
    {


        #region propriedades públicas

        /// <summary>
        /// id do usuário
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// login do usuário, no caso deste sistema, o e-mail
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(80, ErrorMessage = "O login deve ter no máximo 80 caracteres")]
        public virtual string Login { get; set; }

        /// <summary>
        /// flag que diz se deve-se mandar alerta de atraso de lançamento de atividades para esse usuário
        /// </summary>
        [Display(Name = "Enviar alertas de lançamentos atrasados")]
        public virtual bool EnviarAlertaLancamento { get; set; }

        /// <summary>
        /// flag para dizer se o usuário está ativo ou não
        /// </summary>
        [Display(Name = "Usuário Ativo")]
        public virtual bool Ativo { get; set; }

        /// <summary>
        /// perfis dos quais esse usuário faz parte
        /// </summary>
        public virtual ICollection<Perfil> Perfis { get; set; }

        /// <summary>
        /// dados de funcionário complementares a esse usuário (nome, telefone, PIS, CPF etc)
        /// </summary>
        public virtual Funcionario Funcionario { get; set; }

        /// <summary>
        /// referencias (meses de trabalho) desse usuario
        /// </summary>
        public virtual ICollection<Referencia> Referencias
        {
            get; set;
        }

        /// <summary>
        /// atividades desse usuário
        /// </summary>
        public virtual ICollection<Atividade> Atividades
        {
            get; set;
        }

        /// <summary>
        /// nós de projeto atribuidos a esse usuário
        /// </summary>
        public virtual ICollection<ProjectNode> NosDoUsuario { get; set; }

        /// <summary>
        /// retorna o nome caso exista, senão retorna o login
        /// </summary>
        [NotMapped]
        public virtual string FuncionarioNomeOuLogin
        {
            get
            {
                return (this.Funcionario != null && !string.IsNullOrWhiteSpace(this.Funcionario.Nome)) ? this.Funcionario.Nome : this.Login;
            }
        }

        /// <summary>
        /// retorna o e-mail profissional caso exista, senão retorna o e-mail principal, que é o login
        /// </summary>
        [NotMapped]
        public virtual string EmailPreferencial
        {
            get
            {
                return (this.Funcionario != null && !string.IsNullOrWhiteSpace(this.Funcionario.EmailProfissional)) ? this.Funcionario.EmailProfissional : this.Login;
            }
        }

        #endregion



        #region métodos públicos

        /// <summary>
        /// obtém a referência atual baseado na data
        /// </summary>
        /// <returns>Referencia - dados do mês de trabalho</returns>
        public virtual Referencia GetReferencia()
        {
            return this.GetReferencia(DateTime.Now.Year, DateTime.Now.Month);
        }


        /// <summary>
        /// cria ou obtém a referência baseado no ano e mês dados
        /// </summary>
        /// <param name="ano">int - ano</param>
        /// <param name="mes">int - mes</param>
        /// <returns>Referencia - dados do mês de trabalho</returns>
        public virtual Referencia GetReferencia(int ano, int mes)
        {
            Referencia refe = (from r in this.Referencias
                               where r.Ano == ano && r.Mes == mes
                               select r).FirstOrDefault() ?? this.CriaReferencia(ano, mes);

            return refe;
        }



        /// <summary>
        /// usa a lista de atividades do usuário filtrando pelo ano e mês da referência dada
        /// </summary>
        /// <param name="refer">Referencia - referencia atual</param>
        /// <returns>List[Atividade] - atividades do período</returns>
        public virtual List<Atividade> GetAtividades(Referencia refer)
        {
            return this.GetAtividades(refer.Ano, refer.Mes);
        }

        /// <summary>
        /// usa a lista de atividades do usuário filtrando pelo ano e mês dados
        /// </summary>
        /// <param name="ano">int - ano</param>
        /// <param name="mes">int - mes</param>
        /// <returns>List[Atividade] - atividades do período</returns>
        public virtual List<Atividade> GetAtividades(int ano, int mes)
        {
            List<Atividade> result = new List<Atividade>();

            DateTime dt = new DateTime(ano, mes, 1);
            DateTime dtFim = dt.AddMonths(1);

            List<Atividade> atividades = (from t in this.Atividades
                                          where t.Inicio >= dt && t.Fim < dtFim
                                          select t).ToList();

            result.AddRange(atividades);
            return result;
        }



        #endregion



        #region métodos protegidos


        /// <summary>
        /// cria uma referência nova
        /// </summary>
        /// <param name="ano">int - ano</param>
        /// <param name="mes">int - mes</param>
        /// <returns>Referencia - referencia criada</returns>
        protected virtual Referencia CriaReferencia(int ano, int mes)
        {

            Referencia referencia = new Referencia
            {
                Ano = ano,
                Mes = mes,
                Usuario = this
            };

            this.Referencias.Add(referencia);

            return referencia;

        }


        #endregion
    }
}