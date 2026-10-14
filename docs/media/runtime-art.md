# Arte e áudio da primeira fatia jogável

As oito imagens em `client/godot/assets/painted/` foram geradas para Echoes of Aethel em 13–14/10/2026 com a ferramenta imagegen. O pedido usou [a imagem conceitual fornecida pelo usuário](concepts/aethel-visual-target.png) apenas como referência de paleta, perspectiva e atmosfera; os mapas, personagens e itens são novas composições. Foram copiadas para o projeto como PNG originais, sem bancos de assets de terceiros. O proprietário ainda não definiu uma licença geral para o código e a arte.

| Arquivo | Uso no jogo |
|---|---|
| `citadel_courtyard.png` | cenário explorável da Cidadela |
| `forest_village.png` | Bosque, aldeia, passagem e arena |
| `actors_atlas.png` | Bastião, Lobo Etéreo, Cultista Cego e Garras de Musgo |
| `items_atlas.png` | ícones de armas, armaduras, consumíveis e materiais |
| `bastion_walk.png` | quatro quadros de caminhada do Bastião |
| `bastion_attack.png` | quatro quadros de ataque com machado |
| `citadel_npcs.png` | Bram, Irian, Lyra, Maera e Sera em cinco regiões do atlas; gerado com pedido de cinco figuras isoladas em fantasia sombria |
| `wolf_walk.png` | quatro poses consecutivas de locomoção do Lobo Etéreo; gerado com `actors_atlas.png` como referência visual da espécie |

Prompts dos novos assets (imagegen integrado, transparência ativada):

- `citadel_npcs.png`: “Original transparent PNG sprite atlas for dark-fantasy 2D isometric ARPG. Five full-body NPCs in EXACTLY five equal-width cells in one horizontal row, no overlap, no text, no ground. Left to right: Bram older blacksmith in dark apron; Irian white-haired elder scholar in blue robe; Lyra female alchemist in teal coat; Maera female armored gatekeeper with spear; Sera middle-aged merchant in burgundy and brown with satchel. All same scale and painterly pixel-textured style, top-down three-quarter perspective, detailed but legible at 90 px. Transparent background, complete feet visible.”
- `wolf_walk.png`: “Using the wolf in the provided original Echoes of Aethel actors atlas strictly as visual design reference, create a NEW transparent production sprite sheet for the SAME ethereal wolf. Four distinct consecutive locomotion frames in exactly one horizontal row of four equal non-overlapping cells: 1 front paw extended/contact, 2 compressed passing stride, 3 opposite front paw extended/contact, 4 lifted passing stride. Maintain identical character size, camera three-quarter top-down angle, face orientation (looking left/front), silhouette, cyan spectral rim light, dark blue fur, anatomical consistency and foot baseline across every frame. Full wolf visible in each cell including tail and paws. Crisp painterly game art consistent with the existing atlas. No background, no ground, no text, no extra wolves or effects crossing cell boundaries.”

O código seleciona regiões dos atlas em tempo de renderização; os PNGs permanecem intactos. Os arquivos em [runtime/](runtime/) são **capturas reais do Godot** desta versão e mostram Cidadela, Bosque, arena, ataque, bolsa e comércio. As três imagens em [concepts/](concepts/) continuam sendo apenas referências de direção.

A música e os efeitos de combate próprios estão documentados em [runtime-audio.md](runtime-audio.md). A interface ainda usa um tom sintetizado breve para confirmar botões. A fonte é o fallback fornecido pela engine Godot.
