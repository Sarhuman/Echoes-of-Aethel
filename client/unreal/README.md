# Projeto Unreal — Echoes of Aethel

Este diretório é a **raiz do projeto Unreal** dentro do repositório. O arquivo `EchoesOfAethel.uproject` e a árvore `Source/` seguem o diagrama recebido; `Target.cs` de jogo e editor completam o esqueleto C++. A associação foi ajustada para Unreal **5.8**, a versão instalada neste computador em 14/10/2026. Não copiar o valor 5.4 do exemplo recebido.

Abrir com `UnrealEditor.exe` da instalação 5.8 e este `.uproject` como argumento. A instalação local fora do repositório não é um caminho portável para CI ou outros colaboradores. `EnhancedInput` está habilitado; `Paper2D` e `ModelingToolsEditorMode` ficam desabilitados até uma necessidade concreta e revisão de versão/proveniência. O módulo declara UMG e NavigationSystem para futura HUD e navegação, mas ainda não contém widgets, níveis, personagens ou geração de dungeon.

O módulo agora contém infraestrutura de apresentação para [cinemáticas, música, voz e passos](../../docs/design/unreal-cinematics-and-audio.md) e as [fórmulas inteiras de atributos](../../docs/engineering/unreal-persistence-contract.md). Ainda não há cenas, personagens 3D, sons, animações, save ou portal conectados a ela. Em 15/10, `EchoesOfAethelEditor Win64 Development` compilou com MSVC 14.52.37025 Preview e Windows SDK 10.0.28000.0, após corrigir includes locais; o log completo está na PR #23. O build não equivale a uma cena jogável ou a um pacote Windows. O Godot em `../godot/` continua jogável e não recebe save compatível automaticamente. Direção, IDs e primeira rota: [transição](../../docs/engineering/unreal-transition.md), [Cidadela → Bosque](../../docs/design/first-route-3d.md). Para novos assets, siga [as convenções de Content](../../docs/engineering/unreal-content-conventions.md).

Na primeira abertura sem renderização, `UnrealEditor-Cmd.exe` registrou `Engine is initialized` e carregou o mapa padrão. O comando `Quit` foi executado, mas não encerrou o processo; a instância de verificação foi terminada manualmente. Isso não substitui a abertura visual e um fechamento normal pelo editor.

Para rodar o teste de atributos da E1-05 a partir da raiz do repositório:

```powershell
& .\tools\run_tests_unreal.ps1 -EngineRoot 'C:\caminho\para\UE_5.8'
```

O script recompila `EchoesOfAethelEditor` e executa `Aethel.Domain.PlayerStats` com `-nullrhi`. Ele exige no log um teste encontrado, `Result={Success}` e a fila encerrada; retorna código diferente de zero se falhar ou exceder o tempo limite. Em 15/10/2026, o comando executou **1 teste com `Success`** e retornou código 0 neste computador. Os logs ficam em `Saved/Logs/` e não são versionados.
