select * from tpa.Usuario
select * from tpa.funcionario where id = 12

select * from tpa.Atividade where Usuario_Id = 12 and Inicio >= '01/03/2017'  and fim <= '09/03/2017'
order by inicio, fim


select cast ('2017-03-01 00:00:00.000' as datetime)

select cast ('2017-03-01T00:00:00.000' as datetime)

select cast ('20170301' as datetime)

select @@langid