# Sprint 8 — Sistema de save e persistência de mundo

**Estado:** implementação estrutural offline testada em Godot 4.6; ainda não há jogo completo.

Este arquivo usa o caminho solicitado `08_save_system.md` e coexiste com `08_dialogos_hub_degradacao.md`, sem sobrescrevê-lo. O cliente mínimo está em `client/godot/`, portanto **`res://core/save_system.gd` corresponde a `client/godot/core/save_system.gd`**.

## 1. Arquivos entregues

| Arquivo | Responsabilidade |
|---|---|
| [save_system.gd](../../client/godot/core/save_system.gd) | Gravação, leitura, backup, recuperação, quest e checkpoint |
| [world_save_schema.gd](../../client/godot/core/world_save_schema.gd) | Versão, limites, validação e normalização |
| [game_state.gd](../../client/godot/core/game_state.gd) | Snapshot e restauração integral do mundo em memória |
| [event_bus.gd](../../client/godot/core/event_bus.gd) | Notificações confirmadas e gatilho de checkpoint |
| [project.godot](../../client/godot/project.godot) | Autoloads na ordem EventBus → GameState → SaveSystem |
| [test_save_system.gd](../../client/godot/tests/test_save_system.gd) | Integração de arquivo/estado/falhas e invariantes |
| [reload_probe.gd](../../client/godot/tests/reload_probe.gd) | Restauração em outro processo Godot |

Os anexos originais em `docs/reference/` continuam preservados. Estes novos scripts são implementações mínimas, não cópias dos placeholders anteriores. Não implementam combate, inventário, personagens, resolver de diálogo ou GameState completo de todos os módulos.

## 2. Modelo serializado

Snapshot contém cópias profundas de `world_flags`, `npc_states`, `faction_reputation`, escolhas principais, efeitos pendentes/aplicados e progresso (ato, mapa, estágio do hub, checkpoint e carga ritual).

| Campo | Contrato atual |
|---|---|
| world_flags | IDs limitados e valores booleanos; flags escalares genéricas de versões anteriores não são aceitas |
| npc_states | `alive`, `dead` ou `hostile` |
| faction_reputation | Inteiro −100…100 |
| quest_choices | Quest ID → option ID; somente principais no contador desta versão |
| pending_effects / applied_effects | IDs com true; nenhum ID nos dois conjuntos |
| progress | Ato 1–4 compatível com mapa 1–12; hub_stage 0–4; checkpoint em allowlist |
| ritual_charge | 0–12 e igual à quantidade de escolhas principais registradas |

Máximo 2.048 entradas por dicionário, ID de até 128 caracteres sem caminhos arbitrários, arquivo de até 1 MiB. JSON só contém dados; não carregar Nodes, Resources, scripts ou objetos por `get_var(allow_objects=true)`. IDs semânticos de quest/opção/efeito ainda devem ser conferidos pelo futuro catálogo de QuestService; este schema valida estrutura, não toda a campanha.

```gdscript
var candidate: Dictionary = GameState.snapshot()
var result: Dictionary = SaveSystem.commit_snapshot(candidate, "manual")
if not result["ok"]:
    # Mostrar falha; não afirmar que a operação foi persistida.
    print(result["code"])
```

JSON converte números para representação que pode retornar float; somente valores finitos e inteiros no intervalo são aceitos e depois normalizados. Restauração valida tudo antes de trocar qualquer dicionário, sem aplicar metade de um save.

## 3. Arquivo, hash e limites de segurança

Caminho fixo: `user://savegame.json`. Envelope: `schema_version=1`, `content_version=world-save-1`, `payload_json` (string JSON exata) e `sha256` dessa string. Hash da string evita divergência por ordem de chaves ou arredondamento ao recodificar.

**SHA-256 detecta alteração acidental; não autentica o jogador nem criptografa.** Quem controla o arquivo pode alterar o conteúdo e recalcular o hash. XOR é ofuscação e também não é anticheat. Não embutir uma chave universal fingindo proteger economia online.

Criptografia com FileAccess e chave externa pode proteger confidencialidade em dispositivo compartilhado, mas requer lifecycle de chave e recuperação definidos. Compactação reduz tamanho, não adulteração; nenhuma das duas está ativada neste MVP. O JSON permanece inspecionável e testável. Modo online continua autoridade FastAPI/PostgreSQL, sem importar esse snapshot econômico.

## 4. Gravação recuperável

1. Validar candidato e tamanho antes de tocar no save existente.
2. Recusar overwrite de primary inválido/incompatível; exigir recuperação explícita quando cabível.
3. Gravar `.tmp` no mesmo diretório, flush, fechar e reabrir validando hash/schema.
4. Se existe primary válido, remover backup antigo e renomear primary para `.bak`.
5. Promover `.tmp` para primary.
6. Só após sucesso substituir o estado em memória e emitir `save_completed`.

A sequência possui recuperação por backup, mas **não é uma transação ACID de filesystem**. Há intervalo entre rotação e promoção; se cair nesse ponto, load usa `.bak`. Primeiro save interrompido antes da promoção pode não existir. `.tmp` não é considerado commit e não é carregado automaticamente.

Flush/rename não garantem durabilidade contra qualquer falha de hardware, power loss ou filesystem. Operações são síncronas e serializadas para este snapshot pequeno; _busy recusa reentrância. Não suportam duas instâncias do jogo gravando o mesmo slot simultaneamente. Evolução para múltiplos personagens/slots exige lock e identidade de campanha antes de ser ativada.

## 5. Load e recuperação

```gdscript
var result: Dictionary = SaveSystem.load_game()
if result["ok"] and result.get("recovered", false):
    # Informar recuperação do backup e que a operação mais recente pode ter sido perdida.
    print(result["primary_error"])
```

Primary válido é preferido. Primary inexistente/corrompido pode usar backup válido; isso não dispara consequências de quest novamente. Versão de schema ou conteúdo desconhecida bloqueia carregamento, **sem cair silenciosamente num backup mais antigo**. Não há migração implementada, porque esta é a primeira versão persistida.

`GameState.replace_validated()` substitui o snapshot inteiro e emite `EventBus.world_restored`. Consumers devem atualizar projeções de cenário/HUD/NPCs a partir do estado restaurado, sem gerar crimes, drops ou corrupção por reexecutar eventos de história.

Load do backup não altera o arquivo danificado automaticamente. `repair_primary_from_backup()` é operação explícita: valida backup, move primary inválido para `.corrupt` e promove cópia do backup. Recusa sobrescrever quarentena existente ou substituir versão futura. Informar antes de escolher recuperação que o backup pode representar progresso anterior. Recuperação não concede uma escolha nova nem apaga auditoria deliberadamente.

## 6. Quests e checkpoints

### Escolha principal

QuestService futuro valida disponibilidade, catálogo, classe/campanha e requisitos; chama a fronteira offline `commit_quest_choice`. Essa função monta candidato com escolha, flags, carga e efeitos pendentes, grava, substitui o mundo e só depois emite `quest_choice_applied`.

```gdscript
var effects: Array[String] = ["act1.spy_hunt.shelter_villagers.water"]
var result: Dictionary = SaveSystem.commit_quest_choice(
    "act1.spy_hunt", "shelter_villagers",
    {"act1.spy_hunt.shelter_villagers": true}, effects)
```

Mesma opção já confirmada retorna `replayed=true` sem novo efeito; opção diferente retorna `QUEST_ALREADY_DECIDED`. Flag inválida ou falha de arquivo não altera o candidato em memória. Não conectar autosave a um sinal de escolha emitido depois de mutar os dicionários; isso salvaria tarde demais e poderia confirmar visualmente uma decisão perdida.

O sistema ainda não aplica efeitos pendentes: o próximo QuestService deve mover o ID para applied e suas flags na **mesma commit_snapshot**, sem repetir após reload. Afinidades/knowledge/visible_flags e estados de Eclipse não estão nesta versão mínima; adicioná-los requer evolução explícita do schema e testes. Não habilitar permadeath com esse save incompleto.

### Hub/checkpoint

```gdscript
# Adapter de transição offline: somente depois de validar a zona segura.
EventBus.checkpoint_reached.emit("citadel", 1)
```

SaveSystem recebe o checkpoint e persiste candidato com zona, mapa/ato. Falha preserva progresso em memória e emite `save_failed`; o adapter não deve confirmar transição permanente sem observar sucesso. Sinal é gatilho local de aplicação, não prova de autorização online. Hub stage não é inferido apenas por entrar em um mapa: o futuro caso de uso de conclusão de ato fornece essa atualização conforme GDD 04.

Não depender exclusivamente de fechamento do jogo para salvar decisões. A passagem por quest/hub é a fronteira principal. Backend online não usa FileAccess do cliente como fonte de verdade.

## 7. Execução e validação

Projeto mínimo de teste, sem main scene jogável. Instalar Godot 4.6 e executar na raiz do repositório:

```bash
godot --headless --editor --path client/godot --quit
godot --headless --path client/godot --script res://tests/test_save_system.gd
```

A primeira execução registra a classe WorldSaveSchema no cache da engine. `.godot/` não entra no Git. Os testes limpam somente os arquivos de save do projeto de harness; não executar o harness com diretório de userdata compartilhado com um jogo em produção.

**Validação realizada:** Godot `4.6.stable.official.89cea1439`, Linux headless: 27 verificações, zero falhas. Incluem leitura ausente, round-trip de flags/NPC/reputação/efeitos, choice replay/conflito, schema/ranges, hash alterado, backup/quarentena, versão futura, arquivo grande, checkpoint, falha de escrita e restauração num segundo processo Godot.

Não foram simuladas falhas reais de energia nem exports Windows/web/mobile. No web, user:// depende do storage do navegador e persistência/flush do export; validar em browser antes de prometer equivalência ao filesystem desktop. Meta futura: teste de interrupção de cada etapa de rename e migrações de schema.

## 8. Referências oficiais

- [Godot 4.6 — FileAccess](https://docs.godotengine.org/en/4.6/classes/class_fileaccess.html)
- [Godot 4.6 — DirAccess](https://docs.godotengine.org/en/4.6/classes/class_diraccess.html)
- [Godot 4.6 — JSON](https://docs.godotengine.org/en/4.6/classes/class_json.html)

Próximo bloco pode ampliar inventário ou IA, preservando a mesma fronteira de commit. Este marco já permite testar persistência de mundo; não anuncia uma campanha completa funcionando.
