# Plano de execução da prova Unreal

Este índice transforma a planilha de trabalho fornecida em 15/10/2026 em uma sequência verificável dentro do repositório. A planilha propõe 37 tarefas em cinco etapas; suas durações, data de início de 19/10/2026, reserva de seis semanas e fator de replanejamento de 1,5 são **suposições**, não compromissos ou decisões de design. O prazo desejado é aproximadamente um ano. Recalibre a previsão com o primeiro build e com o tempo real no editor.

| Etapa | Entrega e gate | Evidência para avançar |
|---|---|---|
| 1 — Base | Projeto 5.8 compilado, convenções de `Content/Aethel`, testes de atributos, nível de teste 3D, blockout e HUD. Gate E1-10. | Log de compile/cook/package, build Windows aberta e rota curta navegável. |
| 2 — Combate e rota | Domínio de dano, recursos, ataques, inimigos, Garras, inventário, save, loot, quest, NPCs e comércio. Gate E2-11. | Loop Cidadela → Bosque → Garras → retorno, com reload e sem duplicação; regressão Godot executada. |
| 3 — Cinemática e áudio | Prólogo e conselho puláveis, áudio e introdução do chefe. Gate E3-05. | Teste de estados iguais ao assistir/pular, restauração de movimento/câmera e registro audiovisual. |
| 4 — Dungeon opcional | Bilhete de viagem, casa e porão, baús únicos e diário. Gate E4-06. | Testes de queda/recuperação e playtest; custo real por mapa medido. |
| 5 — Expansão por mapa | Ficha, blockout, inimigos, dados de quest/loot e gate por mapa. | IDs e referências válidos, testes e playtest antes do mapa seguinte. |

O [contrato de transição](unreal-transition.md), o [manifesto da primeira rota](first-route-content.json), o [contrato de persistência](unreal-persistence-contract.md) e o [índice de campanha](campaign-content-index.md) regem comportamento e conteúdo. Use o cliente Godot para comparar casos existentes, com `AGENTS.md` como guia de execução. O plano não autoriza conteúdo novo por si só.

## Estado ao incorporar a planilha

- E1-01 está **em andamento**: o `.uproject` aponta para 5.8 e o build `EchoesOfAethelEditor Win64 Development` passou em 15/10/2026 com MSVC 14.52.37025 Preview e Windows SDK 10.0.28000.0. `UnrealEditor-Cmd.exe` em modo `-nullrhi` registrou `Engine is initialized` e carregou o mapa padrão; a instância foi encerrada manualmente após `Quit` não finalizar o processo. Ainda faltam abertura visual no editor e empacotamento Windows para aceitar E1-10.
- E1-02 já está **feito no escopo dos futuros assets Unreal**: `.gitattributes` normaliza texto e configura LFS para `.uasset`, `.umap` e `SourceArt`. Confirmar `git lfs` e quota antes do primeiro binário; PNG/WAV antigos permanecem no Git comum. A proposta da planilha de mover todos os PNG/WAV para LFS não foi adotada, pois reescreveria o histórico de assets existentes.
- E1-03 tem [convenção documentada](unreal-content-conventions.md), sem criar assets. E1-04 agora tem compilação real, após corrigir os includes locais dos `.cpp`; a restauração audível do volume depois da narração continua sem teste de runtime. O build foi executado com `Build.bat EchoesOfAethelEditor Win64 Development -Project=<uproject> -WaitMutex -MaxParallelActions=2` e terminou com `Result: Succeeded` em 179,69 s. A ferramenta avisou que a versão Preview do MSVC é mais nova que a preferida, sem bloquear a compilação. A ordem exata das tarefas paralelas pode ser ajustada quando o primeiro build medir o custo.
- O gate 4 é uma rota estável, não a conclusão da campanha. As estimativas da planilha (18/26/34 semanas para as etapas 1–4 e 10/6/3 mapas totais no prazo) são cenários de capacidade, não promessa de entrega. As outras quatro classes não entram nessas contas.

Na expansão, não iniciar geração procedural, click-to-move, minimapa, as outras quatro classes, dungeons Rei Lich/Rei Orc, dublagem, conversor de saves Godot ou economia de afixos como efeito colateral de uma tarefa da primeira rota. Cada uma exige escopo e aceite próprios após a prova jogável. Revisar prioridades a cada gate e quando uma premissa medida invalidar o plano; não esperar que uma fórmula de prazo decida o design do jogo.
