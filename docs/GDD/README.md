# Índice do Game Design Document

## Módulos de Sprint

| Módulo | Estado | Referência |
|---|---|---|
| 01 — Classes e atributos | Especificação experimental concluída; implementação pendente | [01_classes_e_atributos.md](01_classes_e_atributos.md) |
| 02 — Arquitetura técnica | Stack e contratos definidos; deploy/benchmarks pendentes | [02_arquitetura_tecnica.md](02_arquitetura_tecnica.md) |
| 03 — Sistema de loot | Especificação experimental concluída; implementação pendente | [03_sistema_de_loot.md](03_sistema_de_loot.md) |
| 04 — Mapas e quests | Bíblia de produção concluída; mapas/quests pendentes | [04_mapas_e_quests.md](04_mapas_e_quests.md) |
| 05 — Combate e HUD | Regras experimentais; runtime/HUD/Eclipse pendentes | [05_combate_e_hud.md](05_combate_e_hud.md) |
| 06 — Arquitetura de software | Godot definido; exemplos não executados na engine | [06_arquitetura_software.md](06_arquitetura_software.md) |
| 07 — Roteiro e diálogos | Roteiro de produção; scripts/cenas pendentes | [07_roteiro_e_dialogos.md](07_roteiro_e_dialogos.md) |

Módulos posteriores ao 07 ainda não possuem módulos publicados nesta pasta. Não considerar documentação como confirmação de sistemas executáveis.

## Base de design existente

| Tema | Documento |
|---|---|
| Identidade, loop e invariantes de design | [Design canônico](../design/game-design.md) |
| Arte, interface, sprites e animações | [Direção de arte 16-bit](../design/art-direction.md) |
| Prólogo, NPCs e encontros do Ato I | [Prólogo e Ato I](../design/prologue-and-act-one.md) |
| Campanha, dilemas, fragmentos e revelação — spoilers | [Campanha](../design/campaign.md) |
| Raridades e consumíveis experimentais | [Baseline de loot](../design/loot-baseline.md) |
| Proposta original preservada como referência | [Proposta dos 12 mapas](../reference/proposta-original-12-mapas.md) |

## Engenharia

| Tema | Documento |
|---|---|
| Domínio, conteúdo, saves e consistência | [Arquitetura](../engineering/architecture.md) |
| Marcos e critérios de aceite | [Roadmap](../engineering/roadmap.md) |
| Publicação e dependências entre PRs | [Fluxo GitHub](../engineering/github-workflow.md) |

## Precedência

O módulo 01 define os cinco atributos, quatro ativas e duas passivas por classe e sprites jogáveis 32×32, substituindo o esboço preliminar. Para campanha e segurança dos hubs, vale o design canônico reconciliado. A proposta original é referência histórica e seu prompt textual não implementa o ARPG. Mudanças futuras precisam atualizar os documentos afetados e seus links.

O módulo 02 define a stack Flutter/Flame + FastAPI/PostgreSQL e substitui a seleção de engine pendente na arquitetura preliminar. Offline e online usam autoridades separadas; não importar livremente saves locais para economia online.

O módulo 03 substitui o baseline de quatro raridades: acrescenta Mágico, especifica taxas condicionais e fixa receita/fragmento como recompensa única por campanha.

O módulo 04 define o detalhamento de mapas/dilemas, flags e degradação do hub. O resumo de campanha e as quests secundárias permanecem em design/campaign.md; em conflitos de detalhe da campanha principal, prevalece o módulo 04.

O módulo 05 rege combate e HUD: Bastião usa vigor nas técnicas físicas; MP regenera fora de combate; multiplicador crítico pode chegar a 2×. Roteiro final específico de Vael permanece pendente em módulo posterior.

O módulo 06 substitui Flutter/Flame por Godot no cliente e acrescenta stamina universal para dash. Backend e autoridade online permanecem; exemplos originais são referência histórica, sem runtime.

O módulo 07 rege prólogo, falas dinâmicas, revelação, encenação do confronto e epílogos. A numeração evita substituir o módulo 06 de arquitetura; opções e consequências continuam canônicas no módulo 04.
