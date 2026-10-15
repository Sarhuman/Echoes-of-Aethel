# Convenções de conteúdo Unreal

**Escopo E1-03.** Este documento nomeia e organiza futuros assets da prova 3D. Ainda não cria assets nem muda IDs de jogo. O [manifesto da primeira rota](first-route-content.json) guarda os IDs de domínio; nomes de arquivo, atores, níveis e Blueprints são detalhes de apresentação e não entram no save.

## Pastas em `client/unreal/Content/Aethel/`

```text
Maps/          níveis da Cidadela, Bosque, interiores e arenas
Characters/    malhas, animações e Blueprints do jogador e NPCs
Enemies/       malhas, animações e Blueprints dos inimigos
Items/         modelos, ícones e recipientes
Data/          DataAssets de regras e conteúdo aprovado
Cinematics/    Level Sequences e câmeras
UI/            widgets, fontes e ícones de interface
Audio/         música, ambiência e efeitos
Materials/     materiais e instâncias reutilizáveis
VFX/           Niagara e texturas de efeitos
```

Crie apenas a pasta necessária à entrega em curso. Coloque variantes específicas de um mapa perto do mapa ou ator que as usa; compartilhe em `Materials/` ou `VFX/` só quando houver reuso real. Mantenha arquivos-fonte de arte em `client/unreal/SourceArt/` quando forem versionados, com proveniência registrada. Caches (`Binaries`, `Intermediate`, `Saved`, `DerivedDataCache`) não são conteúdo de projeto.

| Prefixo | Tipo Unreal | Exemplo de nome técnico |
|---|---|---|
| `L_` | nível `.umap` | `L_Citadel_Blockout` |
| `BP_` | Actor ou componente Blueprint | `BP_TwilightBastion` |
| `WBP_` | widget UMG | `WBP_CombatHUD` |
| `DA_` | DataAsset | `DA_EtherealWolf` |
| `SM_` | Static Mesh | `SM_CitadelGate` |
| `SK_` | Skeletal Mesh | `SK_TwilightBastion` |
| `ABP_` | Animation Blueprint | `ABP_TwilightBastion` |
| `A_` | animação | `A_Bastion_Idle` |
| `M_` / `MI_` | material / instância | `M_Stone` / `MI_CitadelStone` |
| `T_` | textura | `T_CitadelStone_BaseColor` |
| `NS_` | sistema Niagara | `NS_MossRoots` |
| `LS_` | Level Sequence | `LS_Prologue` |
| `S_` | fonte de áudio | `S_ForestAmbience` |

Use nomes em inglês, descritivos e estáveis, em `PascalCase` após o prefixo; use sufixos apenas para variantes reais (`_Blockout`, `_BaseColor`, `_Normal`). Texto visível ao jogador permanece em português. Não deduza ID de quest, item, NPC ou recompensa do nome do asset: ligue-o explicitamente ao ID do manifesto ou do catálogo e valide a referência. Não crie classes C++ ou Blueprints duplicados para uma regra já implementada em `Domain/` ou `Application/`.

Antes de adicionar `.uasset` ou `.umap`, confirme `git check-attr filter -- <caminho>` igual a `lfs` e verifique que o LFS está instalado e disponível no remoto. Registre autoria, licença ou cessão e transformação de qualquer material externo; a arte conceitual e os PNG Godot não são meshes 3D prontas.
