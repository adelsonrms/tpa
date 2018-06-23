USE [tecnun]
GO
/****** Object:  StoredProcedure [SyncDB].[usp_SyncDB_Local_LinkedServer]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SyncDB].[usp_SyncDB_Local_LinkedServer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [SyncDB].[usp_SyncDB_Local_LinkedServer]
GO
/****** Object:  StoredProcedure [DML].[DropObject]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DML].[DropObject]') AND type in (N'P', N'PC'))
DROP PROCEDURE [DML].[DropObject]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Fechamento]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Relatorio_Fechamento]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Relatorio_Fechamento]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Consolidado_Projeto_Funcionario_Dia]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Relatorio_Consolidado_Projeto_Funcionario_Dia]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Relatorio_Consolidado_Projeto_Funcionario_Dia]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Consolidado_Projeto_Funcionario]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Relatorio_Consolidado_Projeto_Funcionario]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Relatorio_Consolidado_Projeto_Funcionario]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Consolidado_Cliente_Funcionario_Dia]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Relatorio_Consolidado_Cliente_Funcionario_Dia]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Relatorio_Consolidado_Cliente_Funcionario_Dia]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Consolidado_Cliente_Funcionario]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Relatorio_Consolidado_Cliente_Funcionario]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Relatorio_Consolidado_Cliente_Funcionario]
GO
/****** Object:  StoredProcedure [dbo].[Relatorio_Analitico]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Relatorio_Analitico]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Relatorio_Analitico]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.UsuarioPerfil_TPA.Usuario_Usuario_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[UsuarioPerfil]'))
ALTER TABLE [TPA].[UsuarioPerfil] DROP CONSTRAINT [FK_TPA.UsuarioPerfil_TPA.Usuario_Usuario_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.UsuarioPerfil_TPA.Perfil_Perfil_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[UsuarioPerfil]'))
ALTER TABLE [TPA].[UsuarioPerfil] DROP CONSTRAINT [FK_TPA.UsuarioPerfil_TPA.Perfil_Perfil_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.Referencia_TPA.Usuario_Usuario_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[Referencia]'))
ALTER TABLE [TPA].[Referencia] DROP CONSTRAINT [FK_TPA.Referencia_TPA.Usuario_Usuario_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.ProjectNodeUsuario_TPA.Usuario_Usuario_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[ProjectNodeUsuario]'))
ALTER TABLE [TPA].[ProjectNodeUsuario] DROP CONSTRAINT [FK_TPA.ProjectNodeUsuario_TPA.Usuario_Usuario_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.ProjectNodeUsuario_TPA.ProjectNode_ProjectNode_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[ProjectNodeUsuario]'))
ALTER TABLE [TPA].[ProjectNodeUsuario] DROP CONSTRAINT [FK_TPA.ProjectNodeUsuario_TPA.ProjectNode_ProjectNode_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.ProjectNode_TPA.ProjectNode_Pai_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[ProjectNode]'))
ALTER TABLE [TPA].[ProjectNode] DROP CONSTRAINT [FK_TPA.ProjectNode_TPA.ProjectNode_Pai_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.ProjectNode_TPA.NodeLabel_NodeLabel_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[ProjectNode]'))
ALTER TABLE [TPA].[ProjectNode] DROP CONSTRAINT [FK_TPA.ProjectNode_TPA.NodeLabel_NodeLabel_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.PerfilAcao_TPA.Perfil_Perfil_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[PerfilAcao]'))
ALTER TABLE [TPA].[PerfilAcao] DROP CONSTRAINT [FK_TPA.PerfilAcao_TPA.Perfil_Perfil_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.PerfilAcao_TPA.Acao_Acao_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[PerfilAcao]'))
ALTER TABLE [TPA].[PerfilAcao] DROP CONSTRAINT [FK_TPA.PerfilAcao_TPA.Acao_Acao_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.Funcionario_TPA.Usuario_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[Funcionario]'))
ALTER TABLE [TPA].[Funcionario] DROP CONSTRAINT [FK_TPA.Funcionario_TPA.Usuario_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.Atividade_TPA.Usuario_Usuario_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[Atividade]'))
ALTER TABLE [TPA].[Atividade] DROP CONSTRAINT [FK_TPA.Atividade_TPA.Usuario_Usuario_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.Atividade_TPA.TipoAtividade_TipoAtividade_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[Atividade]'))
ALTER TABLE [TPA].[Atividade] DROP CONSTRAINT [FK_TPA.Atividade_TPA.TipoAtividade_TipoAtividade_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.Atividade_TPA.ProjectNode_ProjectNode_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[Atividade]'))
ALTER TABLE [TPA].[Atividade] DROP CONSTRAINT [FK_TPA.Atividade_TPA.ProjectNode_ProjectNode_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.Atividade_TPA.ProjectNode_Cliente_Id]') AND parent_object_id = OBJECT_ID(N'[TPA].[Atividade]'))
ALTER TABLE [TPA].[Atividade] DROP CONSTRAINT [FK_TPA.Atividade_TPA.ProjectNode_Cliente_Id]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.AspNetUserRoles_TPA.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[TPA].[AspNetUserRoles]'))
ALTER TABLE [TPA].[AspNetUserRoles] DROP CONSTRAINT [FK_TPA.AspNetUserRoles_TPA.AspNetUsers_UserId]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.AspNetUserRoles_TPA.AspNetRoles_RoleId]') AND parent_object_id = OBJECT_ID(N'[TPA].[AspNetUserRoles]'))
ALTER TABLE [TPA].[AspNetUserRoles] DROP CONSTRAINT [FK_TPA.AspNetUserRoles_TPA.AspNetRoles_RoleId]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.AspNetUserLogins_TPA.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[TPA].[AspNetUserLogins]'))
ALTER TABLE [TPA].[AspNetUserLogins] DROP CONSTRAINT [FK_TPA.AspNetUserLogins_TPA.AspNetUsers_UserId]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[TPA].[FK_TPA.AspNetUserClaims_TPA.AspNetUsers_UserId]') AND parent_object_id = OBJECT_ID(N'[TPA].[AspNetUserClaims]'))
ALTER TABLE [TPA].[AspNetUserClaims] DROP CONSTRAINT [FK_TPA.AspNetUserClaims_TPA.AspNetUsers_UserId]
GO
/****** Object:  View [TPA].[StatusLancamentosTeste]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[StatusLancamentosTeste]'))
DROP VIEW [TPA].[StatusLancamentosTeste]
GO
/****** Object:  View [SyncDB].[vw_Tabelas]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[SyncDB].[vw_Tabelas]'))
DROP VIEW [SyncDB].[vw_Tabelas]
GO
/****** Object:  View [SyncDB].[uv_ReportTabelas]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[SyncDB].[uv_ReportTabelas]'))
DROP VIEW [SyncDB].[uv_ReportTabelas]
GO
/****** Object:  View [dbo].[HorasFuncionarios]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[HorasFuncionarios]'))
DROP VIEW [dbo].[HorasFuncionarios]
GO
/****** Object:  View [SyncDB].[vw_Tabelas_DBLinkedServer]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[SyncDB].[vw_Tabelas_DBLinkedServer]'))
DROP VIEW [SyncDB].[vw_Tabelas_DBLinkedServer]
GO
/****** Object:  View [Server].[uv_TabelasBDHomologacao]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Server].[uv_TabelasBDHomologacao]'))
DROP VIEW [Server].[uv_TabelasBDHomologacao]
GO
/****** Object:  View [Manutencao].[uv_ReportTabelas]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Manutencao].[uv_ReportTabelas]'))
DROP VIEW [Manutencao].[uv_ReportTabelas]
GO
/****** Object:  View [TPA].[StatusLancamentos]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[StatusLancamentos]'))
DROP VIEW [TPA].[StatusLancamentos]
GO
/****** Object:  View [TPA].[PBI_FatoAtividades]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[PBI_FatoAtividades]'))
DROP VIEW [TPA].[PBI_FatoAtividades]
GO
/****** Object:  View [TPA].[HierarquiaDeProjeto]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[HierarquiaDeProjeto]'))
DROP VIEW [TPA].[HierarquiaDeProjeto]
GO
/****** Object:  View [TPA].[Entregaveis]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[Entregaveis]'))
DROP VIEW [TPA].[Entregaveis]
GO
/****** Object:  View [TPA].[RelacaoDeAtividades]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[RelacaoDeAtividades]'))
DROP VIEW [TPA].[RelacaoDeAtividades]
GO
/****** Object:  View [dbo].[Minhas_Atividades]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Minhas_Atividades]'))
DROP VIEW [dbo].[Minhas_Atividades]
GO
/****** Object:  View [TPA].[Projetos]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[Projetos]'))
DROP VIEW [TPA].[Projetos]
GO
/****** Object:  View [TPA].[Segmentos]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[Segmentos]'))
DROP VIEW [TPA].[Segmentos]
GO
/****** Object:  View [TPA].[Clientes]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[Clientes]'))
DROP VIEW [TPA].[Clientes]
GO
/****** Object:  View [TPA].[Areas]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[TPA].[Areas]'))
DROP VIEW [TPA].[Areas]
GO
/****** Object:  View [dbo].[DDL_Tabelas]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[DDL_Tabelas]'))
DROP VIEW [dbo].[DDL_Tabelas]
GO
/****** Object:  View [SyncDB].[vw_Tabelas_DBLocalServer]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[SyncDB].[vw_Tabelas_DBLocalServer]'))
DROP VIEW [SyncDB].[vw_Tabelas_DBLocalServer]
GO
/****** Object:  Table [TPA].[UsuarioPerfil]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[UsuarioPerfil]') AND type in (N'U'))
DROP TABLE [TPA].[UsuarioPerfil]
GO
/****** Object:  Table [TPA].[Usuario]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Usuario]') AND type in (N'U'))
DROP TABLE [TPA].[Usuario]
GO
/****** Object:  Table [TPA].[UserTokenCache]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[UserTokenCache]') AND type in (N'U'))
DROP TABLE [TPA].[UserTokenCache]
GO
/****** Object:  Table [TPA].[TipoAtividade]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[TipoAtividade]') AND type in (N'U'))
DROP TABLE [TPA].[TipoAtividade]
GO
/****** Object:  Table [TPA].[TemplateEmail]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[TemplateEmail]') AND type in (N'U'))
DROP TABLE [TPA].[TemplateEmail]
GO
/****** Object:  Table [TPA].[Referencia]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Referencia]') AND type in (N'U'))
DROP TABLE [TPA].[Referencia]
GO
/****** Object:  Table [TPA].[ProjectNodeUsuario]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[ProjectNodeUsuario]') AND type in (N'U'))
DROP TABLE [TPA].[ProjectNodeUsuario]
GO
/****** Object:  Table [TPA].[ProjectNode]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[ProjectNode]') AND type in (N'U'))
DROP TABLE [TPA].[ProjectNode]
GO
/****** Object:  Table [TPA].[PerfilAcao]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[PerfilAcao]') AND type in (N'U'))
DROP TABLE [TPA].[PerfilAcao]
GO
/****** Object:  Table [TPA].[Perfil]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Perfil]') AND type in (N'U'))
DROP TABLE [TPA].[Perfil]
GO
/****** Object:  Table [TPA].[NodeLabel]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[NodeLabel]') AND type in (N'U'))
DROP TABLE [TPA].[NodeLabel]
GO
/****** Object:  Table [TPA].[LogEmailsEnviados]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[LogEmailsEnviados]') AND type in (N'U'))
DROP TABLE [TPA].[LogEmailsEnviados]
GO
/****** Object:  Table [TPA].[Funcionario]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Funcionario]') AND type in (N'U'))
DROP TABLE [TPA].[Funcionario]
GO
/****** Object:  Table [TPA].[Feriado]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Feriado]') AND type in (N'U'))
DROP TABLE [TPA].[Feriado]
GO
/****** Object:  Table [TPA].[EndeceroUrl]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[EndeceroUrl]') AND type in (N'U'))
DROP TABLE [TPA].[EndeceroUrl]
GO
/****** Object:  Table [TPA].[Atividade]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Atividade]') AND type in (N'U'))
DROP TABLE [TPA].[Atividade]
GO
/****** Object:  Table [TPA].[AtestadoAnexo]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[AtestadoAnexo]') AND type in (N'U'))
DROP TABLE [TPA].[AtestadoAnexo]
GO
/****** Object:  Table [TPA].[AspNetUsers]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[AspNetUsers]') AND type in (N'U'))
DROP TABLE [TPA].[AspNetUsers]
GO
/****** Object:  Table [TPA].[AspNetUserRoles]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[AspNetUserRoles]') AND type in (N'U'))
DROP TABLE [TPA].[AspNetUserRoles]
GO
/****** Object:  Table [TPA].[AspNetUserLogins]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[AspNetUserLogins]') AND type in (N'U'))
DROP TABLE [TPA].[AspNetUserLogins]
GO
/****** Object:  Table [TPA].[AspNetUserClaims]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[AspNetUserClaims]') AND type in (N'U'))
DROP TABLE [TPA].[AspNetUserClaims]
GO
/****** Object:  Table [TPA].[AspNetRoles]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[AspNetRoles]') AND type in (N'U'))
DROP TABLE [TPA].[AspNetRoles]
GO
/****** Object:  Table [TPA].[Acao]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[Acao]') AND type in (N'U'))
DROP TABLE [TPA].[Acao]
GO
/****** Object:  Table [TPA].[__MigrationHistory]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TPA].[__MigrationHistory]') AND type in (N'U'))
DROP TABLE [TPA].[__MigrationHistory]
GO
/****** Object:  Table [SyncDB].[TB_TabelasBDHomologacao]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SyncDB].[TB_TabelasBDHomologacao]') AND type in (N'U'))
DROP TABLE [SyncDB].[TB_TabelasBDHomologacao]
GO
/****** Object:  Table [HangFire].[State]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[State]') AND type in (N'U'))
DROP TABLE [HangFire].[State]
GO
/****** Object:  Table [HangFire].[Set]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Set]') AND type in (N'U'))
DROP TABLE [HangFire].[Set]
GO
/****** Object:  Table [HangFire].[Server]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Server]') AND type in (N'U'))
DROP TABLE [HangFire].[Server]
GO
/****** Object:  Table [HangFire].[Schema]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Schema]') AND type in (N'U'))
DROP TABLE [HangFire].[Schema]
GO
/****** Object:  Table [HangFire].[List]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[List]') AND type in (N'U'))
DROP TABLE [HangFire].[List]
GO
/****** Object:  Table [HangFire].[JobQueue]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[JobQueue]') AND type in (N'U'))
DROP TABLE [HangFire].[JobQueue]
GO
/****** Object:  Table [HangFire].[JobParameter]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[JobParameter]') AND type in (N'U'))
DROP TABLE [HangFire].[JobParameter]
GO
/****** Object:  Table [HangFire].[Job]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Job]') AND type in (N'U'))
DROP TABLE [HangFire].[Job]
GO
/****** Object:  Table [HangFire].[Hash]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Hash]') AND type in (N'U'))
DROP TABLE [HangFire].[Hash]
GO
/****** Object:  Table [HangFire].[Counter]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Counter]') AND type in (N'U'))
DROP TABLE [HangFire].[Counter]
GO
/****** Object:  Table [HangFire].[AggregatedCounter]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[AggregatedCounter]') AND type in (N'U'))
DROP TABLE [HangFire].[AggregatedCounter]
GO
/****** Object:  Table [dbo].[OrdemTabelas]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrdemTabelas]') AND type in (N'U'))
DROP TABLE [dbo].[OrdemTabelas]
GO
/****** Object:  UserDefinedFunction [SyncDB].[GetRecordCount]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SyncDB].[GetRecordCount]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [SyncDB].[GetRecordCount]
GO
/****** Object:  UserDefinedFunction [DML].[ObjectExists]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DML].[ObjectExists]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DML].[ObjectExists]
GO
/****** Object:  UserDefinedFunction [DML].[GetRecordCount]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DML].[GetRecordCount]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DML].[GetRecordCount]
GO
/****** Object:  UserDefinedFunction [DML].[CamposSincronizacao]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DML].[CamposSincronizacao]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DML].[CamposSincronizacao]
GO
/****** Object:  Schema [TPA]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'TPA')
DROP SCHEMA [TPA]
GO
/****** Object:  Schema [SyncDB]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'SyncDB')
DROP SCHEMA [SyncDB]
GO
/****** Object:  Schema [Server]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'Server')
DROP SCHEMA [Server]
GO
/****** Object:  Schema [Manutencao]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'Manutencao')
DROP SCHEMA [Manutencao]
GO
/****** Object:  Schema [HangFire]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'HangFire')
DROP SCHEMA [HangFire]
GO
/****** Object:  Schema [DML]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'DML')
DROP SCHEMA [DML]
GO
/****** Object:  User [tecnun]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'tecnun')
DROP USER [tecnun]
GO
USE [master]
GO
/****** Object:  Database [tecnun]    Script Date: 22/06/2018 12:25:32 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'tecnun')
DROP DATABASE [tecnun]
GO
