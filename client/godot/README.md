# Godot — harness de persistência e IA

Base executável de testes offline em Godot 4.6. Há main scene técnica com Player/dash, inimigo FSM, parede e HUD; ainda não há campanha ou mapa final.

Autoloads: EventBus, GameState e SaveSystem; schema em core/world_save_schema.gd. O caminho res://core/save_system.gd corresponde a este projeto.

## Verificação

```bash
godot --headless --editor --path client/godot --quit
godot --headless --path client/godot --script res://tests/test_save_system.gd
godot --headless --path client/godot --script res://tests/test_enemy_fsm.gd
godot --headless --path client/godot --script res://tests/test_player.gd
```

Executar a partir da raiz do repositório. A suíte apaga saves apenas do userdata deste harness; não compartilhar a pasta com produção. Não executar duas instâncias concorrentes sobre o mesmo slot.

Veja [documentação do save](../../docs/GDD/08_save_system.md) para recuperação, limites e contratos. Hash detecta corrupção e não impede adulteração deliberada.

ActorBase e EnemyFSM implementam o núcleo offline; CrimeService, dano completo, loot e navegação ainda não estão integrados. Veja [FSM](../../docs/GDD/09_enemy_fsm.md).

Abra o projeto no editor e use F5 para a cena. Controles/limitações em [Player e cena](../../docs/engineering/player-test-scene.md).

Apresentação opcional: F8 Base/Melhorado; F9 efeitos reduzidos. Flags e limitações em [apresentação](../../docs/engineering/enhanced-presentation.md). Teste: `godot --headless --path client/godot --script res://tests/test_presentation.gd`.
