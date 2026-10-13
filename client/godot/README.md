# Godot — harness de persistência

Base executável de testes offline em Godot 4.6. Ainda não há mapa, combate ou main scene jogável.

Autoloads: EventBus, GameState e SaveSystem; schema em core/world_save_schema.gd. O caminho res://core/save_system.gd corresponde a este projeto.

## Verificação

```bash
godot --headless --editor --path client/godot --quit
godot --headless --path client/godot --script res://tests/test_save_system.gd
```

Executar a partir da raiz do repositório. A suíte apaga saves apenas do userdata deste harness; não compartilhar a pasta com produção. Não executar duas instâncias concorrentes sobre o mesmo slot.

Veja [documentação do save](../../docs/GDD/08_save_system.md) para recuperação, limites e contratos. Hash detecta corrupção e não impede adulteração deliberada.
