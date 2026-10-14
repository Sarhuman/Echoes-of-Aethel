# T09 — Diálogos e NPCs

**Depende de:** T08 · **Decisões:** D6

## Objetivo

Resource de diálogo tipado, resolver sem acoplamento de UI e NPCs da Cidadela/aldeia interagíveis (caixa de diálogo própria; **Dialogic não será usado** (D6)).

## Arquivos a criar/alterar

- `client/godot/content/dialogues/dialogue_definition.gd` (`class_name DialogueDefinition`, `Resource`), `dialogue_choice.gd`
- `client/godot/dialogue/dialogue_resolver.gd` (`class_name DialogueResolver`, `RefCounted`)
- `client/godot/entities/npcs/npc_actor.gd` (`class_name NpcActor extends ActorBase`, `narrative_npc = true`) + `.tscn`
- `client/godot/ui/menus/dialogue_box.gd` + `.tscn` (caixa simples)
- `client/godot/tests/test_dialogue.gd`

## Contrato

- `DialogueChoice` tem `option_id` estável e `requires` (lista de flags); **disponibilidade fail-closed** (requisito desconhecido = indisponível).
- Resolver usa só conhecimento desbloqueado: nenhum spoiler futuro na UI.
- NPC essencial (`npc.blacksmith`, `npc.alchemist`, `npc.elder`, `npc.gatekeeper`, `npc.merchant`) não pode ser morto por interação física; agressão recusa interação e chama guarda, sem combate no hub.
- Texto em português; cores/estilos conforme `docs/design/art-direction.md`.
- Escolhas de quest chamam `QuestService`, nunca alteram `GameState` direto.

## Critérios de aceite

- Conversar com cada NPC essencial na Cidadela.
- Opção indisponível não aparece/não pode ser escolhida.
- Diálogo pós-escolha reflete `mercy`/`judgment` (conteúdo mínimo do GDD 08).

## Testes obrigatórios

Resolver com flag ausente, choice sem ID recusado, spoiler bloqueado, NPC essencial indestrutível, escolha chama serviço.

## Fora de escopo

Retratos finais, voz, localização, Dialogic e Maaack (D6).

## Leia antes

`docs/GDD/07_roteiro_e_dialogos.md`; `docs/GDD/08_dialogos_hub_degradacao.md`; `docs/reference/godot-original/dialogue_node.gd` (apenas referência histórica, não reutilizar).
