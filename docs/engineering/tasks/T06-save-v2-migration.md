# T06 — Save v2: personagem, inventário, recompensas e migração

**Depende de:** T05 · **Decisões:** D1

## Objetivo

Ampliar o payload de save para HP/MP/XP, inventário, recompensas únicas concedidas e checkpoint, com migração v1→v2 testada e recusa de versões futuras sem sobrescrever arquivo.

## Arquivos a criar/alterar

- Alterar `core/world_save_schema.gd`, `core/save_system.gd`, `core/game_state.gd` (**apenas o necessário**)
- `client/godot/core/save_migrations.gd` (`class_name SaveMigrations`, funções estáticas `migrate(envelope: Dictionary) -> Dictionary`)
- `client/godot/tests/test_save_migration.gd`; ampliar `tests/test_save_system.gd`
- Atualizar `docs/GDD/08_save_system.md` e `docs/engineering/versions.md`

## Contrato

- Novo `CONTENT_VERSION` (ex.: `"world-save-2"`) e `SCHEMA_VERSION` conforme necessário; **não** aumentar sem migração.
- Campos novos obrigatórios: `character` (`hp, mp, xp, level, unspent_points`), `inventory` (de `InventoryModel.to_dict`, incluindo `gold` e `equipped`), `granted_rewards` (IDs únicos, formato `reward.<zone>.<enemy>.<evento>`), além dos já existentes.
- Limites: `level` 1–30; `xp` ≥ 0 inteiro; `hp` 0–`hp_max`; `unspent_points` ≥ 0; `gold` ≥ 0.
- Mantém envelope `{schema_version, content_version, payload_json, sha256}`, escrita atômica, backup e validação de tamanho/tipos/IDs.
- Save v1 existente carrega e migra para v2 com defaults seguros; arquivo v1 não é apagado antes de v2 validado.
- Versão desconhecida: recusa com código e **não** sobrescreve.
- **Não** estender `ZONES` (D1): `progress.checkpoint` permanece `"citadel"` no slice.
- Criptografia local continua pendente (não implementar sem decisão de chave).

## Critérios de aceite

- Round-trip v2 completo.
- Migração v1→v2 preserva `quest_choices`, `npc_states`, flags e progresso.
- Falha de gravação não é reportada como sucesso.
- Todas as 27 verificações originais seguem passando (ou são migradas com justificativa).

## Testes obrigatórios

v1→v2, v2 corrompido com recuperação do backup, versão futura recusada, arquivo grande, JSON inválido, hash divergente, ID inválido, `granted_rewards` duplicado.

## Fora de escopo

Cloud save, criptografia, múltiplos slots, UI de save.

## Leia antes

`docs/GDD/08_save_system.md`; `docs/engineering/architecture.md` (Persistência); `production-decisions.md` §4.
