# Índice do Game Design Document

## Módulos de Sprint

| Módulo | Estado | Referência |
|---|---|---|
| 01 — Classes e atributos | Especificação experimental concluída; implementação pendente | [01_classes_e_atributos.md](01_classes_e_atributos.md) |
| 02 — Arquitetura técnica | Stack e contratos definidos; deploy/benchmarks pendentes | [02_arquitetura_tecnica.md](02_arquitetura_tecnica.md) |

Sprints posteriores à 02 ainda não possuem módulos publicados nesta pasta. Não considerar documentação como confirmação de sistemas executáveis.

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
