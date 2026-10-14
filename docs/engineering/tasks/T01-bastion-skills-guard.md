# T01 — Habilidades do Bastião e guarda

**Depende de:** T00, T01A · **Decisões:** D2, D3, D5

## Objetivo

Implementar as habilidades ativas do Bastião nos slots `attack_basic` e `skill_1..4`: Guarda do Crepúsculo, Impacto do Baluarte (reconciliado com o pesado técnico atual), Fenda de Aço e as prioridades de interrupção, sem inventar valores além do GDD.

## Arquivos a criar/alterar

- `client/godot/combat/guard_controller.gd` (`class_name GuardController`, `RefCounted`)
- `client/godot/content/attacks/` novos `.tres` (`AttackDefinition`): `guard.tres` (se modelado como definição) e `steel_rift.tres` (Fenda). O Impacto continua em `combat/heavy.tres` (T01A). `ClassDefinition`/`twilight_bastion.tres` vêm do T01A: apenas **referencie** os ataques nele.
- Alterar `entities/player/player.gd` **somente** para mapear slots e usar `GuardController`; ampliar `tests/test_player.gd` e `tests/test_combat.gd`
- `client/godot/tests/test_guard.gd`

## Contrato

Fonte numérica: `docs/GDD/01_classes_e_atributos.md` §5 e `production-decisions.md` §2.

> **Decisão D2 (resolvida):** o Bastião tem 1 básico + 3 ativas. `skill_4` fica **reservada** no slice. Critério revisado: básico + três ativas implementadas e testadas (ver `decisions-first-slice.md` §9).

| Slot atual (InputMap) | Habilidade | Custo (MP) | Notas |
|---|---|---|---|
| `attack_basic` (1) | Golpe de Machado | 0 | já existe |
| `skill_1` (2/Q) | Impacto do Baluarte (o pesado atual, já reconciliado em T01A) | 12 | P=18, interrupção 0,5 s em comuns, boss não sofre stun; implementar a **interrupção** aqui |
| `skill_2` (3/E) | Guarda do Crepúsculo | 8 | redução frontal 40% por 2 s; costas desprotegidas; sem dano |
| `skill_3` (4/R) | Fenda de Aço | 20 | P=30; antecipação própria 0,6 s |
| `skill_4` (5/F) | **reservada** | — | continua avisando indisponibilidade, sem gasto |

`GuardController`: `request(seq, now, mp, can_act) -> Dictionary{status, mp}`; `reduce_damage(amount: int, hit_from_front: bool) -> int` (piso inteiro); janela de 2 s medida em tempo de simulação; recarga (CD) por habilidade em segundos conforme GDD.
- Comando recusado não gasta MP. Repetição do mesmo `seq` = `REPLAYED`.
- Teto compartilhado de redução = 60% (P1 Postura Firme fica **fora** deste cartão).
- Valores de tempo e recarga: ver `experimental-values.md` §2. Fenda: prep 600 ms (GDD), ativo/recup. EXPERIMENTAL.
- Interrupção: Impacto interrompe inimigos **comuns** por 0,5 s (cancela antecipação em curso); `moss_claws` ignora interrupção. Dano comum **não** interrompe (permanece como hoje).
- Cooldowns por habilidade (A1 0,9 s; Guarda 6 s; Impacto 7 s; Fenda 9 s) medidos em tempo de simulação, separados de custo.

## Critérios de aceite

- 3/E gasta 8 MP, reduz dano frontal em 40% por 2 s e não reduz dano pelas costas.
- 4/R gasta 20 MP e executa antecipação de 0,6 s com recurso recusado sem alterar MP.
- 5/F continua indisponível sem gasto.
- Todas as 125 verificações existentes continuam passando (ajuste somente as que dependem de valores reconciliados, e liste no PR).

## Testes obrigatórios

Custos exatos, MP insuficiente, replay, guarda frontal/costas, expiração de 2 s, uma intenção ofensiva por passo, precedência do básico, guarda não gasta stamina.

## Fora de escopo

Postura Firme (P1), Peso do Juramento (P2), árvore de talentos, outras classes, balanceamento final.

## Leia antes

`docs/GDD/01_classes_e_atributos.md` §5; `docs/GDD/05_combate_e_hud.md`; `docs/engineering/offline-combat.md`; `project-scope.md` §4.
