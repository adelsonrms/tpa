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