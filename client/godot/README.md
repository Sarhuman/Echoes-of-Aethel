# Projeto Godot

Abra `project.godot` no Godot 4.6 stable e pressione F5 para a jornada jogável. Cidadela, Bosque, combate, inventário, mercador, quest e save v2 estão ligados à cena inicial `res://scenes/game_root.tscn`. Os controles e o estado atualizado estão no [README da raiz](../../README.md).

A antiga arena de três encontros permanece em `res://playtest/arena.tscn` como cena técnica opcional. Ela tem sua própria suíte `res://tests/test_arena.gd` e não é o checkpoint da campanha.

Execute as suítes pela raiz com `bash tools/run_tests.sh` ou `tools/run_tests.ps1` no Windows. Testes de save usam `--test-harness` e um slot separado. Capturas em `res://qa/capture_slice.gd` só iniciam quando recebem `-- --test-harness`.
