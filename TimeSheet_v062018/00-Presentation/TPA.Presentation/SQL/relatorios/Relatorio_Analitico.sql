USE [tecnun]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Analitico]    Script Date: 21/11/2017 11:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER	 PROCEDURE [dbo].[Relatorio_Analitico]
(
	@dtIni datetime,
	@dtFin datetime
)
AS
BEGIN

-- ######################################################################################################################################################
-- # NOME			: Relatorio_Analitico
-- # PRODUTO		: Relatórios
-- # DEVELOPER		: Vitor Luiz Rubio
-- # COPYRIGHT		: Tecnun - 2017
-- # OBJETIVO		: Trazer todos os dados das atividades dos funcionários no período informado
-- # VERSAO			: 1.0
-- # DT. CRIACAO	: 2017-08-23
-- # DT. Alteracao	: 
-- #
-- # DEPENDENCIAS:
-- #	
-- # DEPENDENTES:
-- #	
-- #	
-- # HISTÓRICO
-- # 	2017-08-23	: Criação
-- #
-- #
-- # TESTES
-- #		Relatorio_Analitico '2017-07-01', '2017-07-31'
-- #
-- #
-- #
-- ######################################################################################################################################################


	SET NOCOUNT ON;



		select 
			coalesce(f.nome, u.login) Funcionario,
			t.Nome Tipo_Atividade,

			a.Observacao,
			a.Inicio,
			a.Fim,


			cast((a.fim - a.inicio ) as time) Horas,

			case when administrativo.Nome is not null then 'Sim' else 'Não' end Administrativo,
			cliente_principal.Nome Cliente_Raiz,
	
			(case 2 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Cliente,
			(case 3 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Area,
			(case 4 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Projeto,
			(case 5 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Entregaveis,
			(case 6 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Etapas

		from 
			tpa.Atividade a 
			left join tpa.Funcionario f on f.Id = a.Usuario_Id
			left join tpa.Usuario u on u.Id = a.Usuario_Id
			left join tpa.TipoAtividade t on t.Id = a.TipoAtividade_Id
			left join tpa.ProjectNode administrativo on administrativo.id = a.ProjectNode_Id and administrativo.NodeLabel_Id = 1
			left join tpa.ProjectNode cliente_principal on cliente_principal.id = a.Cliente_Id 
	
			left join tpa.ProjectNode n6 on n6.id = a.ProjectNode_Id 

			left join tpa.ProjectNode n5 on n5.id = n6.Pai_Id 
			left join tpa.ProjectNode n4 on n4.id = n5.Pai_Id 
			left join tpa.ProjectNode n3 on n3.id = n4.Pai_Id 
			left join tpa.ProjectNode n2 on n2.id = n3.Pai_Id 

		where 
			A.Inicio >= @dtIni AND a.Fim <= @dtFin

		order by 
			a.Inicio,
			Funcionario


	SET NOCOUNT OFF;

END