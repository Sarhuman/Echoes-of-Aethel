# T10 — HUD final, menus, controles e áudio mínimo

**Depende de:** T01, T05 · **Decisões:** D2, D5, D6

## Objetivo

Substituir o HUD técnico por UI jogável: barras HP/MP/stamina, slots de habilidade com recarga, inventário, menu de pausa, opções (efeitos reduzidos) e tela de controles.

## Arquivos a criar/alterar

- `client/godot/ui/hud/hud.tscn` + `hud.gd`; `ui/hud/skill_slot.gd`
- `client/godot/ui/menus/pause_menu.tscn`, `options_menu.tscn`, `controls_screen.tscn`, `inventory_screen.tscn` + scripts
- `client/godot/ui/theme/aethel_theme.tres`
- `client/godot/audio/` apenas SFX/música **gerados internamente ou com licença registrada**
- `client/godot/tests/test_ui.gd`

## Contrato

- HUD mostra HP, MP e stamina (sem barra de vigor); 4 slots visuais: Impacto, Guarda, Fenda e um slot reservado desabilitado, com custo em MP e recarga.
- UI **somente lê** estado/sinais (`Player.hp_changed`, `mp_changed`, `stamina_changed`, `attack_started`, `skill_unavailable`); nunca altera estado de domínio.
- Tela de controles espelha `project-scope.md` §4 e é gerada a partir do `InputMap` (sem texto duplicado desatualizável).
- Efeitos reduzidos persistem a preferência fora do save de mundo (arquivo de opções próprio em `user://`).
- Acessibilidade: texto legível em movimento e com efeitos reduzidos.
- Nenhum asset sem origem/licença registrada em `docs/media/` ou `versions.md`.

## Critérios de aceite

- Loop completo jogável sem console, com controles exibidos corretamente.
- Pausa não corrompe temporizadores de ataque/dash.

## Testes obrigatórios

Barras refletem sinais, slot reservado mostra indisponível, pausa congela simulação, opções persistem e recusam arquivo inválido.

## Fora de escopo

Remapeamento de teclas, localização, menu de save/slots, Maaack Game Template (D6: não será usado no slice), barra de vigor/fúria (D5).

## Leia antes

`docs/design/art-direction.md`; `docs/design/visual-proposal.md`; `docs/engineering/enhanced-presentation.md`.
