# T11 — Degradação inicial do hub

**Depende de:** T08, T09

## Objetivo

Refletir `progress.hub_stage` e os eventos tardios na Cidadela (visual e falas) sem tirar segurança nem serviços essenciais.

## Arquivos a criar/alterar

- `client/godot/services/hub_state_service.gd` (`class_name HubStateService`, `RefCounted`)
- Variações em `world/maps/citadel.tscn` (nós ativados por estágio)
- `client/godot/tests/test_hub_state.gd`

## Contrato

- `hub_stage` ∈ 0–4 (schema); estágio é derivado de fatos confirmados, nunca incrementado por reload.
- Invariante 8: serviços essenciais sobrevivem à deterioração.
- Fatos ocultos só aparecem após o gatilho de narrativa correspondente.

## Critérios de aceite

- Voltar à Cidadela após cada rota mostra a mudança uma vez.
- Hub continua sem dano/inimigos.

## Testes obrigatórios

Estágio derivado, idempotência, serviços essenciais ativos em todo estágio, reload não avança estágio.

## Fora de escopo

Estágios 3–4 completos, mapas 2–12.

## Leia antes

`docs/GDD/08_dialogos_hub_degradacao.md`; `architecture.md` invariantes 1 e 8.
