# T03 — Mundo: Cidadela, Bosque, zonas, checkpoint e morte/retry

**Depende de:** T00, T02 · **Decisões:** D1

## Objetivo

Conectar `citadel` e `forest_01` em cenas jogáveis, com transição de zona, checkpoint e recuperação após a morte sem perder decisões.

## Arquivos a criar/alterar

- `client/godot/world/zones/zone_definition.gd` (`class_name ZoneDefinition`, `Resource`: `zone_id`, `scene_path`, `is_safe`, `spawn_point`)
- `client/godot/services/zone_service.gd` (`class_name ZoneService`, `Node`; **não** adicionar autoload sem autorização — instanciar na cena raiz)
- `client/godot/world/maps/citadel.tscn`, `forest_01.tscn` (geometria provisória)
- `client/godot/world/interactables/zone_gate.gd` + `.tscn`, `checkpoint_point.gd` + `.tscn`
- `client/godot/scenes/game_root.tscn` + `.gd` (nova cena raiz; **não** trocar `run/main_scene` ainda)
- `client/godot/tests/integration/test_zone_flow.gd`

## Contrato

- `ZoneService.travel(zone_id) -> Dictionary{ok, code}`: valida e troca cena de forma deferida. `EventBus.checkpoint_reached("citadel", 1)` é emitido somente após `SaveSystem` confirmar a gravação ao **entrar na Cidadela**; entrar no Bosque não grava checkpoint.
- Zona `citadel`: `is_safe = true`; sem inimigos ativos, sem dano; serviços essenciais acessíveis.
- Morte do jogador: retorna ao último checkpoint válido; **não** apaga `quest_choices`, `npc_states` nem `applied_effects`; não restaura NPC `dead`.
- **Decisão D1 (resolvida):** o único checkpoint é `citadel`. **Não** altere `WorldSaveSchema.ZONES`. Morte no Bosque → retorno à Cidadela com HP/MP restaurados. Carregar save sempre reaparece na Cidadela. Progresso do Bosque persiste por flags (`forest_01.moss_claws.defeated`), nunca por zona.

## Critérios de aceite

- Sair da Cidadela, entrar no Bosque, morrer e reaparecer sem ficar preso.
- Salvar/carregar reaparece na Cidadela e preserva decisões, flags e progresso.
- Tentativa de ataque na Cidadela é recusada.

## Testes obrigatórios

Transição válida/inválida, falha de gravação não vira sucesso visual, morte→retorno, hub seguro, checkpoint idempotente.

## Fora de escopo

Mapas 2–12, tilesets finais, câmera cinematográfica, menus.

## Leia antes

`docs/GDD/04_mapas_e_quests.md` (Mapa 01); `docs/GDD/08_save_system.md`; `project-structure.md` §3.
