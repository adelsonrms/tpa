USE [tecnun]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Consolidado_Cliente_Funcionario]    Script Date: 21/11/2017 11:01:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER	 PROCEDURE [dbo].[Relatorio_Consolidado_Cliente_Funcionario]
(
	@dtIni datetime,
	@dtFin datetime
)
AS
BEGIN

-- ######################################################################################################################################################
-- # NOME			: Relatorio_Consolidado_Cliente_Funcionario
-- # PRODUTO		: Relatórios
-- # DEVELOPER		: Vitor Luiz Rubio
-- # COPYRIGHT		: Tecnun - 2017
-- # OBJETIVO		: Trazer todos os dados das atividades dos funcionários no período informado com a quantidade de horas consolidada por cliente / funcionário
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
-- #		Relatorio_Consolidado_Cliente_Funcionario '20170701', '20170731'
-- #
-- #
-- #
-- ######################################################################################################################################################


	SET NOCOUNT ON;

		select 
			coalesce(f.nome, u.login) Funcionario,
			cliente_principal.Nome Cliente,
			cast(SUM(DATEDIFF(MINUTE, '00:00:00.000', (a.fim - a.inicio ) ))/60 as varchar) + ':' + cast(SUM(DATEDIFF(MINUTE, '00:00:00.000', (a.fim - a.inicio ) ))%60 as varchar) as Horas

		from 
			tpa.Atividade a 
			left join tpa.Funcionario f on f.Id = a.Usuario_Id
			left join tpa.Usuario u on u.Id = a.Usuario_Id
			left join tpa.TipoAtividade t on t.Id = a.TipoAtividade_Id
			left join tpa.ProjectNode cliente_principal on cliente_principal.id = a.Cliente_Id 
	
	
		where 
			A.Inicio >= @dtIni AND a.Fim <= @dtFin

		group by
			coalesce(f.nome, u.login),
			cliente_principal.Nome 


		order by 
			Funcionario,
			cliente


	SET NOCOUNT OFF;

END