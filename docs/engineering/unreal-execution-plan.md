# Plano de execução da prova Unreal

> **Mudança de ordem solicitada em 15/10/2026:** a produção da parte jogável Unreal fica **adiada até a pré-produção cobrir os doze mapas, cinco protagonistas, elenco, bestiário e contexto de campanha**. O [atlas de mapas](../design/campaign-world-atlas-3d.md) e o [índice de personagens/criaturas](../design/campaign-cast-and-bestiary-3d.md) iniciam essa cobertura. As etapas de gameplay abaixo permanecem como plano posterior; suas datas e gates não autorizam começar a cena jogável agora. O build/teste da base C++ já feito continua válido.

O [registro de conteúdo C0](campaign-content-ledger.md) acompanha quests, hubs, comércio, loot, secundárias, cenas e lacunas com sua origem. Ele distingue contrato aprovado, proposta e asset ainda ausente.

As fichas de [hubs e histórias secundárias](../design/hubs-and-side-stories-3d.md) e de [comércio por classe/região](../design/regional-commerce-3d.md) são propostas para revisão no C0. Os serviços básicos existentes permanecem acessíveis; equipamento e preços novos exigem catálogo e simulação.

O [catálogo de habilidades](../design/ability-production-catalog-3d.md) cobre os 30 nós das cinco classes para preparar animação, VFX, SFX, ícones e IDs; não inicia implementação de combate.

As [decisões D01–D08 de direção ARPG 3D](diablo-direction-decisions.md) respondem à planilha de comparação com Diablo II sem reabrir a migração de engine. Em especial, grade + baú são um contrato novo da Unreal; o inventário Godot de 60 posições continua referência e exige migração explícita se os saves forem importados.

A planilha de acompanhamento **`plano-codex-echoes-of-aethel-v2(2).xlsx` substitui as cópias anteriores**. Ela acrescenta as tarefas C0-01 a C0-11 antes de E1-06, semanas reais, a aba Arte e riscos R1–R7. O arquivo foi fornecido fora do repositório; este documento registra a ordem e o estado do trabalho, sem tratar as fórmulas e durações estimadas como validação de produção. As [18 pranchas de personagens](../media/concepts/model-sheets/README.md) avançam C0-06, mas estão apenas **geradas**, não aprovadas; não fecham as fichas nem o gate.

R1 aponta uma dependência circular: o atlas precisa de medidas de cápsula, câmera e largura antes dos blockouts, enquanto o nível navegável E1-06 espera C0. Adotar para C0-07 apenas uma arena **descartável de medição**, sem loop ou entrega jogável, resolve a medição sem antecipar E1-06. R2 pede um piloto de arte C0-08 com o Bastião, do conceito à importação, para medir horas reais por ator; ele também não abre gameplay. R3–R6 permanecem riscos de estimativa, toolchain Preview, prazo e verificação, a acompanhar nos gates. Os dois pilotos ainda não foram executados.

R7 trata do peso das pranchas. Os 18 PNGs medidos somam **48.794.687 bytes** e a regra LFS cobre somente `docs/media/concepts/model-sheets/*.png`, sem migrar imagens antigas. Cada prancha possui `*.source.txt` com proveniência. Git LFS e autenticação foram confirmados pelo upload completo dos 18 objetos em 15/10/2026; a quota restante não foi medida.

## Gate C0 — conteúdo antes de gameplay

| Entrega | Evidência requerida |
|---|---|
| Campanha inteira | Doze fichas de zona revisadas com rotas de entrada/retorno, interiores, escolha, encontro, ambiente móvel e limites; mapas de hubs e dungeons opcionais separados dos doze |
| Elenco | Cinco protagonistas, NPCs recorrentes, 33 entradas de inimigos comuns, oito subchefes e quatro bosses com fichas de silhueta, função e dependências de animação/áudio |
| Narrativa e economia | Cenas/diálogos e estados de consequência auditados contra GDD 04/07/08; loot, vendedores, recipientes e recompensas catalogados com IDs definitivos e regras de persistência |
| Produção visual | Conceitos autorais e lista de modelos/rigs/materiais por zona e ator, com origem/licença; plantas/blockouts **não jogáveis** e orçamento técnico medido quando viável |
| Revisão | Lacunas e propostas `NOVO/EXPERIMENTAL` decididas editorialmente; nenhuma ficha confundida com asset ou mapa 3D implementado |

Somente depois de C0 fechado iniciar E1-06 em diante (nível de teste, blockout navegável, HUD e loop). A implementação antecipada do projeto base e dos testes de atributos não obriga continuar a vertical jogável. A ordem antiga da planilha era capacidade estimada, não uma restrição contra a direção atual do projeto.

Este índice acompanha a planilha v2 fornecida em 15/10/2026 como sequência verificável dentro do repositório. A versão atual acrescenta 11 tarefas C0 às etapas existentes; suas durações, reserva e fator de replanejamento de 1,5 são **suposições**, não compromissos ou decisões de design. O início registrado é 14/10/2026, conforme os primeiros commits Unreal. O prazo desejado é aproximadamente um ano; a própria previsão central da v2 aponta apenas quatro mapas nesse horizonte, portanto os doze mapas exigem replanejamento de escopo, capacidade ou prazo nos gates. O primeiro build já foi medido; recalibrar as etapas restantes pelo tempo real no editor e no piloto de arte.

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
- E1-05 tem [suíte Automation C++](../../client/unreal/Source/EchoesOfAethel/Private/Tests/AethelPlayerStatsTest.cpp) e [script de linha de comando](../../tools/run_tests_unreal.ps1). Em 15/10/2026, o script retornou código 0 com `Aethel.Domain.PlayerStats: 1 teste executado, Success`. Ele compara HP/MP nos níveis 1 e 2 e rejeição de atributos inválidos, sem renderização.
- O gate 4 é uma rota estável, não a conclusão da campanha. As estimativas da v1 (18/26/34 semanas para as etapas 1–4 e 10/6/3 mapas) foram substituídas pela v2 após incluir C0; não usar esses números antigos para prometer entrega. As outras quatro classes não entram na rota jogável inicial.

Na expansão, não iniciar geração procedural, click-to-move, minimapa, as outras quatro classes, dungeons Rei Lich/Rei Orc, dublagem, conversor de saves Godot ou economia de afixos como efeito colateral de uma tarefa da primeira rota. Cada uma exige escopo e aceite próprios após a prova jogável. Revisar prioridades a cada gate e quando uma premissa medida invalidar o plano; não esperar que uma fórmula de prazo decida o design do jogo.

