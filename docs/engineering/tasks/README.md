# Cartões de tarefa

> Registro histórico: T00–T12 e T01A foram implementados em conjunto na branch `codex/playable-first-slice` por autorização explícita do usuário para tornar o jogo jogável. CI e export local passaram; o playtest humano continua pendente. Novos trabalhos devem começar pelo estado real do código e por [project-structure.md](../project-structure.md).

Backlog ordenado para a primeira fatia jogável (Bastião · Cidadela · Bosque). **Um cartão por branch/PR**, na ordem das dependências. Regras gerais em [`AGENTS.md`](../../../AGENTS.md); mapa de pastas em [`project-structure.md`](../project-structure.md); decisões em [`decisions-first-slice.md`](../decisions-first-slice.md); números em [`experimental-values.md`](../experimental-values.md).

| ID | Cartão | Depende de | Entrega verificável |
|---|---|---|---|
| T00 | [Catálogo de conteúdo](T00-content-catalog.md) | — | IDs únicos e referências válidas |
| T01A | [Fórmula de dano, atributos e regeneração](T01A-damage-and-stats.md) | T00 | Dano do GDD reproduzido; HP 172 / MP 52 |
| T01 | [Habilidades do Bastião e guarda](T01-bastion-skills-guard.md) | T00, T01A | Arena com todas as ações utilizáveis e custos validados |
| T02 | [Lobo e Cultista](T02-wolf-cultist.md) | T00, T01A | Encontros próprios com antecipações |
| T03 | [Mundo, zonas e checkpoint](T03-world-zones-checkpoint.md) | T00, T02 | Sair, lutar, morrer e voltar sem perder decisões |
| T04 | [Boss Garras de Musgo](T04-moss-claws-boss.md) | T02, T03 | Antecipações evitáveis e retry |
| T05 | [Inventário, peso e equipamento](T05-inventory-weight.md) | T00, T01A | Bolsa cheia não perde item de quest |
| T06 | [Save v2 e migração](T06-save-v2-migration.md) | T05 | v1→v2 sem perda; versão futura recusada |
| T07 | [Loot e XP atômicos](T07-loot-xp-atomic.md) | T05, T06, T02 | Sem duplicação nem perda |
| T08 | [Quest A Caça ao Espião](T08-quest-spy-hunt.md) | T03, T06 | Duas rotas e consequência única |
| T09 | [Diálogos e NPCs](T09-dialogue-npcs.md) | T08 | NPCs essenciais conversáveis |
| T10 | [HUD, menus e áudio mínimo](T10-hud-menus-audio.md) | T01, T05 | Loop jogável sem console |
| T11 | [Degradação do hub](T11-hub-degradation.md) | T08, T09 | Mudança observável, hub seguro |
| T12 | [CI, export e playtest](T12-ci-export-playtest.md) | T01–T11 | Executável Windows e CI verde |

Ordem sugerida de execução: T00 → T01A → T01 → T05 → T02 → T03 → T06 → T07 → T04 → T08 → T09 → T10 → T11 → T12.

## Como pedir um cartão ao GPT

> Leia `AGENTS.md`, `docs/engineering/project-structure.md` e `docs/engineering/tasks/T0X-....md`. Implemente somente esse cartão em uma branch própria, com testes headless, rode `bash tools/run_tests.sh` e abra o PR descrevendo entregue, pendente e validação real.

## Decisões já resolvidas

Os pontos que estavam em aberto (checkpoint do Bosque, mapa de slots, fórmula de dano, valores numéricos, recursos, plugins, categoria do boss e equipamento) foram resolvidos em [`decisions-first-slice.md`](../decisions-first-slice.md) (D1–D8) e os números estão em [`experimental-values.md`](../experimental-values.md). **Não pergunte de novo**: siga o documento. Se achar um conflito real, registre no PR.

## Ainda em aberto (não bloqueiam)

Criptografia local do save, passivas P1/P2 do Bastião, distribuição de pontos de atributo, colisão contínua de golpes e a janela de esquiva do boss. Lista completa em `decisions-first-slice.md` §10.
