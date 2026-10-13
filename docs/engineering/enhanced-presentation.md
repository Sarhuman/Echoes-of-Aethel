# Apresentação Base / Melhorada — primeiro bloco

## Diagnóstico do projeto real

Godot 4.6/GDScript, Compatibility, 2D top-down com projeção de movimento 2:1. Cena `test_scene.tscn` de 960×540 com Polygon2D temporários, CharacterBody2D/StaticBody2D, Input Map de teclado, Player e EnemyFSM. HUD Control/CanvasLayer. Não há sprites definitivos, câmera de gameplay, áudio, luz/sombra dinâmica, inventário/loot ou pós-processamento implementados. Gamepad ainda não validado. Nenhuma medição FPS/GPU em build Windows disponível.

Os três anexos são prompts de melhoria, não evidências de testes deste repositório. O prompt Godot é aplicável; Unity fornece apenas comparações conceituais, sem instalar pipeline/pacotes Unity. Números de desempenho de outro projeto não são benchmark do Echoes.

## Escopo aplicado

[CombatPresentation](../../client/godot/presentation/enhanced/combat_presentation.gd) fica fora dos scripts de domínio. Cena recebe apenas um componente de apresentação; Player/ActorBase/FSM/save não foram reescritos.

| Recurso | Implementação / limite |
|---|---|
| Base/Melhorado | F8, padrão Base; `set_enhanced(bool)` restaura cor e remove efeitos ativos |
| Acessibilidade | F9, `set_reduced_effects(bool)`; cancela flash/impactos imediatamente |
| Flash de dano | Cor branca no Polygon2D por 33 ms, restaurada à cor capturada |
| Impacto | Anel/raios dourados desenhados com CanvasItem; máximo 24, expiram em 250 ms |
| Controle modular | `flashes_enabled` e `particles_enabled`, via export ou flags CLI |
| HUD | Indicador de modo em CanvasLayer separado; não recebe efeitos do mundo |

O indicador de modo permanece nos dois modos para comparação; a apresentação do mundo em Base retorna às cores originais. Nenhum asset externo, pacote ou shader foi instalado; formas e cores são código original. As propriedades visuais controladas pertencem exclusivamente ao componente na cena de teste; futuros AppearanceControllers precisam de contrato de composição para não disputar as mesmas cores.

I-frames são gameplay e permanecem em **ambos os modos**: 120 ms de tempo de simulação. HP inteiro da base, custo 25 stamina, cooldown 1,2 s, colisões e IA permanecem iguais. O trecho colado repete HP paralelo, dano float, `_process` com movimento e ausência de projeção; ele não substitui Player testado.

Somente `actor_damaged` confirmado produz impacto. Dash que evita dano não cria flash, dano ou recompensa visual falsa. Não altera Engine.time_scale, árvore pausada, save ou física. Alternar 50 vezes não cria novos nós, e desligar restaura efeitos no mesmo comando.

## Como comparar

Abrir client/godot/project.godot e F5. F8 alterna, F9 reduz efeitos. Receber ataque do inimigo vermelho para observar impacto. Parâmetros exportados permitem variar intensidade/duração sem tocar matemática.

```bash
godot --path client/godot -- --enhanced
godot --path client/godot -- --enhanced --no-flash
godot --path client/godot -- --enhanced --no-particles
godot --path client/godot -- --enhanced --reduced-effects
```

Flags após `--` são argumentos do jogo. Não salva preferências ou estado durante comparação. Esta versão não é uma build Windows exportada.

## Validação e pendências

Godot 4.6 Linux headless: **13 apresentação + 19 Player + 25 FSM + 27 save = 84 verificações**, zero falhas. Apresentação testa dano igual nos modos, ausência de feedback no MISS, restauração, limite/expiração, alternância sem alocar nós e save/parametrização inalterados. Isso não substitui inspeção visual real.

Sem captura A/B, benchmark FPS/1% low/pior frame ou export release nesta entrega. Recursos não aplicados: shake (não existe câmera de gameplay), hitstop/slow motion (alteram tempo de simulação), luzes/normais/bloom (assets e perfil pendentes), áudio (sem assets licenciados), clima/água/loot/inventário (sistemas ausentes). Não afirmar que ativar Melhorado aumenta FPS.

Próxima validação gráfica: Windows release, percurso e posições idênticos, captura por evento de dano e três rodadas por modo; relatar hardware, resolução, VSync e tempos de frame. Nenhuma alteração de renderer/escala, material global ou mecânica é implícita pelo cardápio dos anexos.

Referências: [Camera2D Godot 4.6](https://docs.godotengine.org/en/4.6/classes/class_camera2d.html) e [CanvasItem/custom drawing](https://docs.godotengine.org/en/4.6/tutorials/2d/custom_drawing_in_2d.html).
