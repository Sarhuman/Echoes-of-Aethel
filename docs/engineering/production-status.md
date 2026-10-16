# Estado verificável e próximos aceites — 16/10/2026

> Atualização 16/10/2026: consulte [núcleo C# v3](runtime-core-v3.md) para os quatro kits, oito caminhos adicionais, loot e 51 encontros agora executáveis fora da engine. As limitações históricas abaixo descrevem o estágio anterior; integração Unity e validação de gameplay continuam pendentes.

> **Atualização de núcleo:** o [runtime v2](runtime-core-v2.md) acrescenta inventário/XP/recursos, migração e efeitos dos dois caminhos do Bastião; as demais classes ainda têm alocação, sem kits de combate portados.

Este índice é o ponto de entrada para a próxima entrega. A direção é Unity para Windows offline; Godot conserva a versão histórica jogável e Unreal conserva infraestrutura/blockouts. Clientes anteriores não foram removidos. Versão, manifests e projeto Unity ainda não foram inicializados/validados nesta série de PRs.

## Estado por entrega

| Área | Evidência disponível | Próximo aceite que falta |
|---|---|---|
| Campanha | 12 mapas/24 opções e propostas de interação em todos os atos | Revisão editorial, plantas em escala e implementação |
| Memória C# | Regressão de 4.096 rotas no núcleo headless | Fatos mapeados a cenário/diálogo/áudio Unity |
| Campanha + personagem | Agregado/CAS com recompensa do Bosque e ritual derivado | Recompensas restantes e catálogo de conteúdo além da primeira rota |
| Especializações | Alocação/respec/aviso persistidos e testados em C# | Outros kits/classes, conteúdo dos seus efeitos, acesso por NPC e UI |
| Ameaça regional | Faixas experimentais verificadas e projeção C# de comparação | Spawn/estatísticas/XP, percepção e HUD conectados |
| Arte de personagens | [Revisão solicitada](../design/character-visual-review.md), seis pranchas avaliadas | Variantes, comparação de silhueta, aprovação e assets de runtime |
| Mapas | Dois blockouts históricos e percursos propostos | Câmera/cápsula medidas, planta métrica, navegação, interiores e retorno |
| Combate Unity | Contratos e referência Godot | Jogador/Lobo/Cultista, colisão, feedback, dano e dash testados na engine |
| Dungeons | Dois percursos e contratos de retirada/recompensa propostos | Layout, bosses, recompensa atômica e ida/volta reais |
| Lançamento | Windows offline como alvo | Build instalada fora do editor, desempenho, acessibilidade e playtests |

“Compilado em Mono” não significa “funciona na Unity”. “Aprovado em design” não significa “implementado”. O C0 continua aberto; esta entrega não remove um gate para antecipar quantidade de mapas.

## Sequência de trabalho

1. **Integração dos PRs:** revisar a cadeia #23 → #25 → #26 → #27 → #28 → #29 e este incremento, reconciliando #22/#24 e alterações locais. Fazer diff contra a main vigente, resolver conflitos sem perder conteúdo e executar checks. Não fazer merge automático só pela ordem dos números.
2. **U0:** registrar versão real de editor/URP/input, hardware e drivers; abrir cena mínima e gerar executável Windows de inspeção. Medir abertura/compilação/RAM e frame time sem prometer FPS antes de executar. Não iniciar produção simultânea de doze cenas.
3. **C0 da primeira experiência:** fechar câmera/escala, kit inicial, conteúdo e orçamento de assets da rota piloto conforme governança vigente. Criar uma decisão explícita se a equipe quiser alterar o gate global, preservando o planejamento do restante da campanha.
4. **Percurso completo piloto:** movimento/ataque/dash, dois inimigos, loot utilizável, quest e consequência, save/reload e Garras. Interface só apresenta transações confirmadas. Aceite humano no PC antes da expansão.
5. **Segunda região:** Pântano com experiência própria, reutilizando ferramentas, domínio e produção de assets. Medir custo real de produzir/corrigir uma região; ajustar cronograma com evidência.
6. **Expansão controlada:** completar campanha, classes e dungeons gradualmente; simular economia e testar cada classe com equipamento comum. Reservar tempo contínuo de regressão, não deixar todo o QA para o final.

## Horizonte de dois anos

Prazo indicado pelo proprietário: aproximadamente outubro de 2028, contando a partir de outubro de 2026. As janelas abaixo são hipóteses de planejamento, não datas prometidas sem equipe/horas definidas.

| Janela relativa | Resultado para revisão |
|---|---|
| Meses 1–3 | Unity validada, padrão de combate e percurso curto completo |
| Meses 4–6 | Ato I representativo, piloto de especialização e custo de produção medido |
| Meses 7–12 | Expansão de campanha e introdução gradual das classes |
| Meses 13–18 | Campanha de ponta a ponta, cinco classes e duas dungeons funcionais |
| Meses 19–24 | Correção, balanceamento, otimização, acessibilidade e preparação do lançamento |

Se a velocidade medida não sustentar essas janelas, priorizar revisão do conteúdo/produção antes de comprometer estabilidade. Métricas úteis: tempo para criar um inimigo completo, tempo para fechar uma região, falhas por playtest, orçamento de frame/RAM e capacidade de completar o percurso sem suporte do desenvolvedor. Não usar quantidade de documentos ou asserções como percentual de jogo pronto.

## Definição de concluído

Cada entrega de gameplay precisa de: código/asset versionado com origem, testes relevantes executados, cena/build reproduzível, ida/volta e save/reload verificados quando aplicáveis, revisão de legibilidade/acessibilidade e observação por jogador. Registrar falhas e limitações. Todo sistema econômico ou de progresso exige validação de comando repetido, conflito, inventário cheio/recursos insuficientes e falha de gravação.

Pendências específicas desta base: kits e efeitos das outras classes, slots/afixos/loot futuros, catálogo de XP para os demais encontros, recuperação guiada, adaptação física/Unity, economia e testes Windows. A curva numérica, inventário portado, recursos, migração do protótipo e janela de recibos estão implementados no v2, sem validação de gameplay. Não há afirmação de que essas lacunas foram fechadas apenas porque seu contrato existe.
