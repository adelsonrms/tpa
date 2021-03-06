USE [tecnun]
GO
/****** Object:  StoredProcedure [TPA].[Relatorio_Padrao]    Script Date: 17/11/2017 13:01:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER	 PROCEDURE [TPA].[Relatorio_Padrao]
(
	@dtIni datetime,
	@dtFin datetime
)
AS
BEGIN

-- ######################################################################################################################################################
-- # NOME			: Relatorio_Padrao
-- # PRODUTO		: Relatórios
-- # DEVELOPER		: Vitor Luiz Rubio
-- # COPYRIGHT		: Tecnun - 2017
-- # OBJETIVO		: Trazer todos os dados das atividades dos funcionários no período informado
-- # VERSAO			: 1.0
-- # DT. CRIACAO	: 2017-11-17
-- # DT. Alteracao	: 
-- #
-- # DEPENDENCIAS:
-- #	
-- # DEPENDENTES:
-- #	
-- #	
-- # HISTÓRICO
-- # 	2017-11-17	: Criação
-- #
-- #
-- # TESTES
-- #		tpa.Relatorio_Padrao '20171101', '20171130'
-- #
-- #
-- #
-- ######################################################################################################################################################


	SET NOCOUNT ON;



		select 
			coalesce(f.nome, u.login) Nome,
			cliente_principal.Nome Cliente,
			(case 3 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)   Area,
			
			coalesce
			(
				--projeto
				(case 4 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end),   
				--area
				(case 3 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)
			) Projeto,

			coalesce
			(
				--etapa
				(case 6 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end),
				--entregavel
				(case 5 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end),
				--projeto
				(case 4 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end),   
				--area
				(case 3 when n2.NodeLabel_Id then n2.Nome when n3.NodeLabel_Id  then n3.Nome  when n4.NodeLabel_Id  then n4.Nome   when n5.NodeLabel_Id  then n5.Nome  when n6.NodeLabel_Id then n6.Nome else null end)
			) Atividade,

			t.Nome Categoria,			
			convert(date, a.Inicio) Data,
			a.inicio Hora_Inicio,
			a.fim  Hora_Fim,
			cast( (a.fim - a.inicio ) as double precision) Total

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
			Nome


	SET NOCOUNT OFF;

END