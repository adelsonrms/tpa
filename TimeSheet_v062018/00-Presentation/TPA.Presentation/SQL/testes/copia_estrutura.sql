begin transaction

delete from tpa.referencia
delete from tpa.atividade
delete from tpa.perfilacao
delete from tpa.usuarioperfil
delete from tpa.projectnodeusuario
delete from tpa.funcionario
delete from tpa.usuario
delete from tpa.perfil
delete from tpa.acao
delete from tpa.tipoatividade
delete from tpa.projectnode
delete from tpa.nodelabel



set identity_insert tpa.nodelabel on
insert into tpa.nodelabel (id, nome) select id,nome from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].[NodeLabel]
set identity_insert tpa.nodelabel off

set identity_insert tpa.projectnode on
insert into tpa.projectnode (Id,Nome,HorasEstimadas,Pai_Id,NodeLabel_Id) select Id,Nome,HorasEstimadas,Pai_Id,NodeLabel_Id from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].projectnode
set identity_insert tpa.projectnode off




set identity_insert tpa.tipoatividade on
insert into tpa.tipoatividade (Id,Nome) select Id,Nome from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].tipoatividade
set identity_insert tpa.tipoatividade off


set identity_insert tpa.acao on
insert into tpa.acao (Id,Nome) select Id,Nome from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].acao
set identity_insert tpa.acao off


set identity_insert tpa.perfil on
insert into tpa.perfil (Id,Nome) select Id,Nome from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].perfil
set identity_insert tpa.perfil off


set identity_insert tpa.usuario on
insert into tpa.usuario (Id,login) select Id,login from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].usuario
set identity_insert tpa.usuario off

insert into tpa.funcionario (Id,Nome,Matricula,CPF,PIS,Telefone,Celular,EmailProfissional,EmailPessoal,Endereco) select Id,Nome,Matricula,CPF,PIS,Telefone,Celular,EmailProfissional,EmailPessoal,Endereco from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].funcionario
insert into tpa.projectnodeusuario (ProjectNode_Id, Usuario_Id) select ProjectNode_Id, Usuario_Id from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].projectnodeusuario
insert into tpa.usuarioperfil (Usuario_Id, Perfil_Id) select Usuario_Id, Perfil_Id from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].usuarioperfil
insert into tpa.perfilacao (Perfil_Id, Acao_Id) select Perfil_Id, Acao_Id from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].perfilacao


set identity_insert tpa.atividade on
insert into tpa.atividade (Id,Observacao,Inicio,Fim,ProjectNode_Id,TipoAtividade_Id,Usuario_Id,Cliente_Id) select Id,Observacao,Inicio,Fim,ProjectNode_Id,TipoAtividade_Id,Usuario_Id,Cliente_Id from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].atividade
set identity_insert tpa.atividade off


set identity_insert tpa.referencia on
insert into tpa.referencia (Id,Ano,Mes,Fechado,Usuario_Id,PrevistoSegundos,RealizadoSegundos,SaldoSegundos,SaldoDoMesSegundos,PrevistoCorrenteSegundos,BancoDeHorasSegundos) select Id,Ano,Mes,Fechado,Usuario_Id,PrevistoSegundos,RealizadoSegundos,SaldoSegundos,SaldoDoMesSegundos,PrevistoCorrenteSegundos,BancoDeHorasSegundos from [MSSQL04-FARM68.KINGHOST.NET].[tecnun].[TPA].referencia
set identity_insert tpa.referencia off

DBCC CHECKIDENT ('[tpa].[referencia]', RESEED)
DBCC CHECKIDENT ('[tpa].[atividade]', RESEED)
DBCC CHECKIDENT ('[tpa].[usuario]', RESEED)
DBCC CHECKIDENT ('[tpa].[perfil]', RESEED)
DBCC CHECKIDENT ('[tpa].[acao]', RESEED)
DBCC CHECKIDENT ('[tpa].[tipoatividade]', RESEED)
DBCC CHECKIDENT ('[tpa].[projectnode]', RESEED)
DBCC CHECKIDENT ('[tpa].[nodelabel]', RESEED)

--rollback
--commit