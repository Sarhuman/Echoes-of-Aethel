# T08 — Quest A Caça ao Espião (`act1.spy_hunt`)

**Depende de:** T03, T06

## Objetivo

Runtime da quest com pistas, duas opções confirmáveis uma única vez e efeito tardio `first_return_to_citadel` persistido junto com a decisão.

## Arquivos a criar/alterar

- `client/godot/content/quests/quest_definition.gd` (`class_name QuestDefinition`, `Resource`) + `content/quests/act1_spy_hunt.tres`
- `client/godot/quest/quest_service.gd` (`class_name QuestService`, `RefCounted`)
- `client/godot/tests/test_quest.gd`; `tests/integration/test_spy_hunt_flow.gd`

## Contrato

- `quest_id = act1.spy_hunt`; opções `judge_suspects` (→ `judgment`) e `shelter_villagers` (→ `mercy`).
- `QuestService.choose(quest_id, option_id) -> Dictionary{ok, code}`: valida, aplica **na mesma atualização atômica**: `quest_choices[quest_id] = option_id`, efeitos imediatos, `pending_effects["first_return_to_citadel"]`, `progress.ritual_charge += 1`. Persistir antes de emitir `EventBus.quest_choice_applied`.
- Repetir a escolha (mesma ou outra opção) não duplica nada; segunda opção diferente é recusada.
- Efeito tardio é aplicado na primeira volta à Cidadela, vira `applied_effects`, e **nunca** dispara de novo após reload.
- Pistas (pegadas, depoimentos contraditórios, brilho no poço) são flags em `world_flags`; nenhuma confirma o diagnóstico sozinha.
- Consequências: A → Cidadela recebe armas sem donos, parentes evitam Maera; B → Lyra instala distribuição limpa, aprendiz de Bram adoece (oficina continua).
- Hub continua seguro e com serviços essenciais.

## Critérios de aceite

- Concluir as duas rotas em campanhas separadas, salvar/reabrir e observar a consequência uma vez.
- `ritual_charge == quest_choices.size()` sempre.

## Testes obrigatórios

Escolha única, replay, opção inválida, atomicidade (falha de save reverte), efeito tardio uma vez, reload após cada etapa.

## Fora de escopo

Outras quests do Ato I, UI final, diálogos completos (T09).

## Leia antes

`docs/GDD/04_mapas_e_quests.md` (Mapa 01); `docs/design/prologue-and-act-one.md`; `production-decisions.md` §6.
