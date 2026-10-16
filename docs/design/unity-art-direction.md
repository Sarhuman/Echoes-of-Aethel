# Direção visual vigente — Unity 3D

**Direção confirmada em 16/10/2026.** Echoes of Aethel é um ARPG 3D de fantasia sombria para Windows, com câmera elevada, silhuetas legíveis e materiais estilizados com aparência pintada. Pixel art 16-bit, sprites 32×32, tiles e projeção simulada 2:1 pertencem ao protótipo Godot.

Este guia rege apresentação junto da [transição Unity](../engineering/unity-transition.md). Não altera história, IDs, fórmulas, custos, loot, escolhas, hitboxes ou segurança dos hubs. Não comprova cena Unity, modelos finais ou desempenho.

## Linguagem visual

- Arquitetura gótica desgastada; pedra, madeira, ferro e tecido distinguíveis. Detalhe concentrado nas formas reconhecíveis pela câmera de jogo.
- Proporções estilizadas, volumes claros e texturas com aparência pintada. Complexidade final depende do piloto; conceitos não devem ser reconstruídos literalmente sem avaliação.
- Câmera elevada em três quartos sobre mundo 3D. Comparar ortográfica e perspectiva com campo reduzido em blockout antes de fixar projeção, ângulo ou distância.
- Preservar paletas regionais do [guia histórico](art-direction.md), atmosfera e identidade. Ajustar valores à iluminação real. Não igualar rostos, trajes e silhuetas de heróis/vilões; seguir [revisão do elenco](character-visual-review.md).
- Modelos, rigs e animações substituem sprites no mundo. Retratos/ícones podem continuar ilustrações 2D, sem obrigação de fonte pixel ou resolução 64×64.
- UI com tipografia legível, escala ajustável, foco de teclado e indicadores por texto/forma além da cor. Resolução da UI não deve limitar renderização do mundo.

## Combate, câmera e narrativa

Personagem, perigo, piso transitável e loot precisam ser reconhecíveis em movimento. Antecipação usa postura, área de chão e som; névoa, brilho e partículas não podem ocultar alvos ou ataques. Equipamento decorativo não amplia automaticamente hurtboxes.

Animação apresenta tempos do domínio. Não converter quadros de sprite em cadência 3D ou conceder dano/XP por evento visual. Não converter pixels, tiles ou projeção 2:1 em metros, velocidade ou navegação 3D.

Abertura, prólogo, introduções de chefes e finais continuam sujeitos aos roteiros e contratos de apresentação. Esta revisão não implementa cutscenes nem declara cobertura executável dos 14 confrontos. Pular e assistir devem preservar o mesmo estado; não há dano durante apresentação bloqueada.

## Produção no PC alvo

Hardware informado: Xeon 2630, RX 580, 16 GB RAM. URP é proposto; editor e pacotes serão fixados após conferir instalação e executar prova local.

Começar com geometria de complexidade moderada, materiais compartilhados, iluminação predominantemente pré-calculada e poucas luzes dinâmicas relevantes. Avaliar LOD, instanciamento, sombras, partículas e carregamento por região conforme gargalos medidos.

Medir abertura/compilação, pico de memória, tempo de CPU/GPU por quadro e carregamento no PC real; registrar resolução, VRAM e configuração. Não prometer FPS nem orçamento universal de polígonos por usar URP. Esta revisão não altera configuração de renderização.

## Piloto e aceite

1. Blockout Cidadela/Bosque, escala humana consistente e comparação de câmera em movimento.
2. Bastião, Lobo e Garras distinguíveis no mesmo enquadramento; inimigos visíveis contra piso e névoa.
3. Materiais legíveis, antecipações claras e efeitos reduzidos preservando informação.
4. UI ampliável e capturas reais identificadas. Conceitos existentes não são screenshots Unity.
5. Build Windows e medições no hardware alvo antes de expandir produção.

## Referências e fronteiras

| Documento | Aplicação |
|---|---|
| Este guia + transição Unity | Direção do novo cliente 3D |
| GDDs de classes, loot, mapas, combate e roteiro | Conteúdo/regras; cláusulas de pixels explicitamente históricas |
| art-direction.md e visual-proposal.md | Pixel art e briefing do protótipo Godot; paleta/atmosfera reutilizáveis |
| Tiled / Aseprite | Manutenção Godot e arte 2D apropriada; não pipeline obrigatório do mundo Unity |
| Documentação Unreal | Histórico e contratos reaproveitáveis; APIs não são implementação Unity |

Preservar documentos, assets e clientes anteriores. Não converter saves nem mudar regras por esta revisão visual.
