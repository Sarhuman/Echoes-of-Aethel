# Player e cena de validação

Cena executável técnica Godot 4.6 em `client/godot/scenes/test_scene.tscn`, configurada como main scene. Usa formas geométricas temporárias, sem sprites do Aseprite ou mapa Bosque final.

## Controles

Abrir `client/godot/project.godot` no editor Godot 4.6 e pressionar F5. WASD/setas movem; Espaço faz dash; J coloca o jogador em recuperação normal de teste; K inicia windup pesado de teste. J/K não causam dano nem implementam as habilidades reais. O inimigo usa EnemyFSM e dano final fixo 15. HUD exibe HP/stamina/i-frames/cooldown/fase/morte. MP, combate ofensivo, quest, respawn de checkpoint e inventário ainda não estão presentes nessa cena.

## Regras integradas

Player herda HP inteiro e morte do ActorBase, sem current_hp paralelo. take_damage aceita int/ActorBase e retorna bool. Física chama move_and_slide uma vez; paredes continuam sólidas durante dash. Input.get_vector preserva magnitude analógica e direção é projetada `(x, y×0,5)`.

Baseline: velocidade 150, dash 400, duração 0,2 s, janela sem dano 0,12 s, custo **25 stamina**, cooldown 1,2 s e regeneração 12/s fora do dash. Custo 30, velocidade 750 e regeneração 25/s do trecho recebido não substituem os parâmetros de produção aprovados; parâmetros permanecem editáveis para testes.

Dash cancela recuperação normal, recusa windup/active e recuperação pesada. Mantém facing ao não mover. Duração física e invulnerabilidade são independentes; fração do último passo evita adicionar um tick inteiro ao deslocamento. Timers do Player processam antes do inimigo de teste. Janela de 120 ms é contrato de tempo de simulação; física discreta não garante dano amostrado em tempo contínuo exato em qualquer FPS. Testar ordem de eventos em CombatService antes de integrar habilidades.

Morte limpa dash/iframes, mantém evento único e colisões deferidas do ActorBase, sem queue_free imediato que esconda HUD. Respawn/scene transition ainda precisam de implementação. Não há save de HP/stamina/posição; persistência entregue anteriormente é somente mundo narrativo.

## Verificação

```bash
godot --headless --editor --path client/godot --quit
godot --headless --path client/godot --script res://tests/test_player.gd
godot --headless --path client/godot --script res://tests/test_enemy_fsm.gd
godot --headless --path client/godot --script res://tests/test_save_system.gd
godot --headless --path client/godot --quit-after 60
```

Linux headless Godot 4.6: **19 verificações Player + 25 FSM + 27 save = 71, zero falhas**. Cena principal abre/encerra sem erro. Testes incluem parede física, magnitude analógica, projeção, custo, cooldown, limite 119/120 ms, dash após i-frames, cancelamentos e morte.

Não foi feito export Windows, verificação visual manual da HUD nem teste de gamepad nesta entrega. Web/Android permanecem fora do marco. Essas verificações são de núcleo/cena técnica, não validação de toda a campanha ou do balanceamento.
